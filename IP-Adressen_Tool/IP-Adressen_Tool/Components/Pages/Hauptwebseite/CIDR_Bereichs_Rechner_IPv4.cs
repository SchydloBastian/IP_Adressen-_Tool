using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public class CidrBereichIPv4Result
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";

    public string CidrNotation { get; set; } = "";
    public string FirstAddress { get; set; } = "";
    public string LastAddress { get; set; } = "";
    public string WildcardMask { get; set; } = "";
    public long TotalAddresses { get; set; }

    public string Range => $"{FirstAddress} – {LastAddress}";
    public string TotalAddressesText => TotalAddresses.ToString("N0", CultureInfo.GetCultureInfo("de-DE"));
}

public class BereichNetz
{
    public string Netz { get; set; } = "";
    public int Cidr { get; set; }
    public long Anzahl { get; set; }

    public string CidrNotation => $"{Netz}/{Cidr}";
}

public class BereichZuCidrResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";
    public List<BereichNetz> Netze { get; set; } = new();
}

public class UeberlappungPaar
{
    public string NetzA { get; set; } = "";
    public string NetzB { get; set; } = "";

    public string Schnitt { get; set; } = "";
    public string SchnittBereich { get; set; } = "";
    public long SchnittAdressen { get; set; }
}

public class UeberlappungResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";
    public string Hinweis { get; set; } = "";
    public List<string> Zeilenfehler { get; set; } = new();
    public int AnzahlNetze { get; set; }
    public List<UeberlappungPaar> Paare { get; set; } = new();

    public bool KeineUeberlappung => IsValid && Paare.Count == 0;
}

public static class CIDR_Bereichs_Rechner_IPv4
{
    public static CidrBereichIPv4Result Calculate(string ipString, int cidr)
    {
        var result = new CidrBereichIPv4Result();

        if (cidr < 0 || cidr > 32)
        {
            result.ErrorMessage = "Das CIDR-Präfix muss zwischen 0 und 32 liegen.";
            return result;
        }

        if (string.IsNullOrWhiteSpace(ipString)
            || !IPAddress.TryParse(ipString.Trim(), out IPAddress? address)
            || address.AddressFamily != AddressFamily.InterNetwork)
        {
            result.ErrorMessage = "Ungültige IPv4-Adresse.";
            return result;
        }

        byte[] b = address.GetAddressBytes();
        uint ip = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];

        uint mask = CIDR_Unterstuetzung.CidrToMaskUint(cidr);
        uint first = ip & mask;
        uint last = first | ~mask;

        result.FirstAddress = AlsIp(first);
        result.LastAddress = AlsIp(last);
        result.WildcardMask = AlsIp(~mask);
        result.TotalAddresses = 1L << (32 - cidr);
        result.CidrNotation = $"{result.FirstAddress}/{cidr}";
        result.IsValid = true;
        return result;
    }

    private const int MaxNetzZeilen = 1000;
    private const int MaxPaare = 5000;

    
    public static BereichZuCidrResult BereichZuCidr(string von, string bis)
    {
        var result = new BereichZuCidrResult();

        if (!TryParseIPv4(von, out uint start, out string startFehler))
        {
            result.ErrorMessage = "Startadresse: " + startFehler;
            return result;
        }

        if (!TryParseIPv4(bis, out uint ende, out string endeFehler))
        {
            result.ErrorMessage = "Endadresse: " + endeFehler;
            return result;
        }

        if (start > ende)
        {
            result.ErrorMessage = "Die Startadresse darf nicht größer als die Endadresse sein.";
            return result;
        }

        long aktuell = start;
        long letzte = ende;

        while (aktuell <= letzte)
        {
            int hostBits = aktuell == 0 ? 32 : BitOperations.TrailingZeroCount((uint)aktuell);

            long rest = letzte - aktuell + 1;
            while (hostBits > 0 && (1L << hostBits) > rest) hostBits--;

            result.Netze.Add(new BereichNetz
            {
                Netz = AlsIp((uint)aktuell),
                Cidr = 32 - hostBits,
                Anzahl = 1L << hostBits
            });

            aktuell += 1L << hostBits;
        }

        result.IsValid = true;
        return result;
    }

    public static UeberlappungResult FindeUeberlappungen(string text)
    {
        var result = new UeberlappungResult();
        var netze = new List<(string Text, uint Start, uint Ende, int Cidr)>();

        string[] zeilen = (text ?? "").Split('\n');
        for (int i = 0; i < zeilen.Length; i++)
        {
            string zeile = zeilen[i].Trim();
            if (zeile.Length == 0) continue;

            if (netze.Count >= MaxNetzZeilen)
            {
                result.Hinweis = $"Es werden höchstens {MaxNetzZeilen} Netze gelesen. Der Rest wurde ignoriert.";
                break;
            }

            string[] teile = zeile.Split('/');
            if (teile.Length != 2 || !int.TryParse(teile[1].Trim(), out int cidr) || cidr < 0 || cidr > 32)
            {
                result.Zeilenfehler.Add($"Zeile {i + 1}: \"{zeile}\" ist kein Netz im Format IP/CIDR.");
                continue;
            }

            if (!TryParseIPv4(teile[0], out uint ip, out string fehler))
            {
                result.Zeilenfehler.Add($"Zeile {i + 1}: {fehler}");
                continue;
            }

            uint maske = CIDR_Unterstuetzung.CidrToMaskUint(cidr);
            uint netzStart = ip & maske;
            uint netzEnde = netzStart | ~maske;
            netze.Add(($"{AlsIp(netzStart)}/{cidr}", netzStart, netzEnde, cidr));
        }

        if (netze.Count < 2)
        {
            result.ErrorMessage = "Bitte mindestens zwei gültige Netze eingeben, eines pro Zeile.";
            return result;
        }

        result.AnzahlNetze = netze.Count;
        bool voll = false;

        for (int a = 0; a < netze.Count && !voll; a++)
        {
            for (int b = a + 1; b < netze.Count; b++)
            {
                uint schnittVon = Math.Max(netze[a].Start, netze[b].Start);
                uint schnittBis = Math.Min(netze[a].Ende, netze[b].Ende);
                if (schnittVon > schnittBis) continue;

                int schnittCidr = Math.Max(netze[a].Cidr, netze[b].Cidr);

                result.Paare.Add(new UeberlappungPaar
                {
                    NetzA = netze[a].Text,
                    NetzB = netze[b].Text,
                    Schnitt = $"{AlsIp(schnittVon)}/{schnittCidr}",
                    SchnittBereich = $"{AlsIp(schnittVon)} – {AlsIp(schnittBis)}",
                    SchnittAdressen = (long)schnittBis - schnittVon + 1
                });

                if (result.Paare.Count >= MaxPaare)
                {
                    result.Hinweis = $"Es werden höchstens {MaxPaare} Paare gezeigt.";
                    voll = true;
                    break;
                }
            }
        }

        result.IsValid = true;
        return result;
    }

    private static bool TryParseIPv4(string text, out uint wert, out string fehler)
    {
        wert = 0;
        string ip = (text ?? "").Trim();
        if (!IP_Validierung.TryValidateIPv4(ip, out fehler, out _)) return false;

        byte[] b = IPAddress.Parse(ip).GetAddressBytes();
        wert = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
        return true;
    }

    private static string AlsIp(uint wert) =>
        $"{(wert >> 24) & 255}.{(wert >> 16) & 255}.{(wert >> 8) & 255}.{wert & 255}";
}
