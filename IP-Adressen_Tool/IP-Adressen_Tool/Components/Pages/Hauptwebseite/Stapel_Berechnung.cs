using System.Globalization;using System.Net;
namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;
public class StapelZeile{
    public int Nummer { get; set; }
    public string Eingabe { get; set; } = "";
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";
    public ExportIPv4 Daten { get; set; } = new();
}
public class StapelResult{
    public List<StapelZeile> Zeilen { get; set; } = new();
    public string Hinweis { get; set; } = "";

    public int Gueltig => Zeilen.Count(z => z.IsValid);
    public int Fehlerhaft => Zeilen.Count - Gueltig;

    public List<ExportIPv4> ExportListe() => Zeilen.Select(z => z.Daten).ToList();
}
public static class Stapel_Berechnung{
    private const int MaxZeilen = 1000;

    // Eine IP oder IP/CIDR pro Zeile. Steht kein CIDR da, wird standardCidr genommen.// Fehlerhafte Zeilen werden einzeln markiert, die Berechnung läuft weiter.
    public static StapelResult Berechne(string text, int standardCidr)
    {
        var result = new StapelResult();
        standardCidr = Math.Clamp(standardCidr, 0, 32);

        string[] zeilen = (text ?? "").Split('\n');
        for (int i = 0; i < zeilen.Length; i++)
        {
            string eingabe = zeilen[i].Trim();
            if (eingabe.Length == 0) continue;

            if (result.Zeilen.Count >= MaxZeilen)
            {
                result.Hinweis = $"Es werden höchstens {MaxZeilen} Zeilen berechnet. Der Rest wurde ignoriert.";
                break;
            }

            var zeile = new StapelZeile { Nummer = i + 1, Eingabe = eingabe };
            string ip = eingabe;
            int cidr = standardCidr;
            string[] teile = eingabe.Split('/');

            if (teile.Length > 2)
            {
                zeile.ErrorMessage = "Erwartet wird eine IP oder IP/CIDR.";
            }
            else            {
                ip = teile[0].Trim();
                if (teile.Length == 2 && (!int.TryParse(teile[1].Trim(), out cidr) || cidr < 0 || cidr > 32))
                {
                    zeile.ErrorMessage = "Das CIDR-Präfix muss eine Zahl zwischen 0 und 32 sein.";
                }
                else if (!IP_Validierung.TryValidateIPv4(ip, out string fehler, out _))
                {
                    zeile.ErrorMessage = fehler;
                }
            }

            if (zeile.ErrorMessage.Length == 0)
            {
                zeile.Daten = ExportDaten(ip, cidr);
                zeile.IsValid = true;
            }
            else            {
                zeile.Daten = new ExportIPv4 { IP = eingabe, Fehler = zeile.ErrorMessage };
            }

            result.Zeilen.Add(zeile);
        }

        return result;
    }

    // Baut alle IPv4-Werte für eine gültige IP und ein CIDR. Standort, Anbieter und Reverse-DNS bleiben leer,// die füllt die Seite selbst (Standortdienst und DNS-Abfrage laufen nicht im Stapel).// Wird auch von Erste_Seite für den Export benutzt.
    public static ExportIPv4 ExportDaten(string ip, int cidr)
    {
        IP_Validierung.TryValidateIPv4(ip, out _, out string klasse);
        SubnetResult subnetz = Subnet_Calculator.Calculate(ip, cidr);
        CidrBereichIPv4Result bereich = CIDR_Bereichs_Rechner_IPv4.Calculate(ip, cidr);

        byte[] b = IPAddress.Parse(ip).GetAddressBytes();
        uint wert = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
        uint maske = CIDR_Unterstuetzung.CidrToMaskUint(cidr);
        long netz = wert & maske;
        long block = 1L << (32 - cidr);

        return new ExportIPv4
        {
            IP = ip,
            CIDR = $"/{cidr}",
            Art = AdressArt(wert),
            Klasse = klasse,
            Maske = subnetz.SubnetMask,
            Wildcard = bereich.WildcardMask,
            Netz = subnetz.NetworkAddress,
            ErsterHost = subnetz.FirstHost,
            LetzterHost = subnetz.LastHost,
            Broadcast = subnetz.BroadcastAddress,
            NutzbareHosts = subnetz.UsableHosts.ToString(CultureInfo.InvariantCulture),
            Hex = "0x" + wert.ToString("X8"),
            Ganzzahl = wert.ToString(CultureInfo.InvariantCulture),
            VorherigesNetz = netz - block >= 0 ? $"{AlsIp((uint)(netz - block))}/{cidr}" : "-",
            NaechstesNetz = netz + block <= uint.MaxValue ? $"{AlsIp((uint)(netz + block))}/{cidr}" : "-"        };
    }

    private static string AdressArt(uint ip)
    {
        uint a = ip >> 24;
        uint b = (ip >> 16) & 255;
        uint c = (ip >> 8) & 255;

        if (ip == 0xFFFFFFFFu) return "Broadcast";
        if (a == 0) return "Dieses Netzwerk (0.0.0.0/8)";
        if (a == 10) return "Privat (10.0.0.0/8)";
        if (a == 100 && b >= 64 && b <= 127) return "Shared Address Space (100.64.0.0/10)";
        if (a == 127) return "Loopback";
        if (a == 169 && b == 254) return "Link-Local (APIPA)";
        if (a == 172 && b >= 16 && b <= 31) return "Privat (172.16.0.0/12)";
        if (a == 192 && b == 168) return "Privat (192.168.0.0/16)";
        if (a == 192 && b == 0 && c == 2) return "Dokumentation (TEST-NET-1)";
        if (a == 198 && (b == 18 || b == 19)) return "Benchmark-Tests (198.18.0.0/15)";
        if (a == 198 && b == 51 && c == 100) return "Dokumentation (TEST-NET-2)";
        if (a == 203 && b == 0 && c == 113) return "Dokumentation (TEST-NET-3)";
        if (a >= 224 && a <= 239) return "Multicast";
        if (a >= 240) return "Reserviert (Klasse E)";
        return "Öffentlich";
    }

    private static string AlsIp(uint wert) =>
        $"{(wert >> 24) & 255}.{(wert >> 16) & 255}.{(wert >> 8) & 255}.{wert & 255}";
}
 