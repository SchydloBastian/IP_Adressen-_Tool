using System.Globalization;
using System.Net;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public class VlsmZeile
{
    public string Hosts { get; set; } = "";
    public string Netz { get; set; } = "";
    public int Cidr { get; set; }
    public string Erster { get; set; } = "";
    public string Letzter { get; set; } = "";
    public string Broadcast { get; set; } = "";
    public long Nutzbar { get; set; }
    public string Fehler { get; set; } = "";

    public bool HatFehler => Fehler.Length > 0;
    public string CidrNotation => HatFehler ? "" : $"{Netz}/{Cidr}";
}

public class VlsmResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";
    public string Gesamtnetz { get; set; } = "";
    public long GesamtAdressen { get; set; }
    public long VergebeneAdressen { get; set; }
    public List<VlsmZeile> Zeilen { get; set; } = new();

    public long FreieAdressen => GesamtAdressen - VergebeneAdressen;

    public List<ExportVlsm> ExportListe() => Zeilen.Select(z => new ExportVlsm
    {
        Hosts = z.Hosts,
        Netz = z.Netz,
        CIDR = z.HatFehler ? "" : $"/{z.Cidr}",
        Erster = z.Erster,
        Letzter = z.Letzter,
        Broadcast = z.Broadcast,
        Nutzbar = z.HatFehler ? "" : z.Nutzbar.ToString(CultureInfo.InvariantCulture),
        Fehler = z.Fehler
    }).ToList();
}

public static class VLSM_Rechner
{
    private const int MaxAnforderungen = 200;

    public static VlsmResult Berechne(string netzText, string hostsText)
    {
        var result = new VlsmResult();

        string[] teile = (netzText ?? "").Trim().Split('/');
        if (teile.Length != 2 || !int.TryParse(teile[1].Trim(), out int cidr) || cidr < 0 || cidr > 30)
        {
            result.ErrorMessage = "Bitte das Netz als IP/CIDR angeben, mit einem Präfix von 0 bis 30 (z. B. 192.168.0.0/24).";
            return result;
        }

        string ip = teile[0].Trim();
        if (!IP_Validierung.TryValidateIPv4(ip, out string fehler, out _))
        {
            result.ErrorMessage = fehler;
            return result;
        }

        byte[] b = IPAddress.Parse(ip).GetAddressBytes();
        uint wert = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
        uint start = wert & CIDR_Unterstuetzung.CidrToMaskUint(cidr);
        long ende = (long)start + (1L << (32 - cidr)) - 1;

        string[] woerter = (hostsText ?? "").Split(new[] { '\n', '\r', ',', ';', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (woerter.Length == 0)
        {
            result.ErrorMessage = "Bitte mindestens eine Hostzahl eingeben.";
            return result;
        }
        if (woerter.Length > MaxAnforderungen)
        {
            result.ErrorMessage = $"Es sind höchstens {MaxAnforderungen} Netze auf einmal möglich.";
            return result;
        }

        var gueltig = new List<long>();
        var ungueltig = new List<string>();
        foreach (string wort in woerter)
        {
            if (long.TryParse(wort, out long hosts) && hosts >= 1 && hosts <= 4294967292L) gueltig.Add(hosts);
            else ungueltig.Add(wort);
        }

        result.Gesamtnetz = $"{AlsIp(start)}/{cidr}";
        result.GesamtAdressen = 1L << (32 - cidr);

        long position = start;
        foreach (long hosts in gueltig.OrderByDescending(h => h))
        {
            int hostBits = 2;
            while ((1L << hostBits) < hosts + 2) hostBits++;
            long groesse = 1L << hostBits;

            if (position + groesse - 1 > ende)
            {
                result.Zeilen.Add(new VlsmZeile
                {
                    Hosts = hosts.ToString(CultureInfo.InvariantCulture),
                    Fehler = "Kein Platz mehr im Netz."
                });
                continue;
            }

            int blockCidr = 32 - hostBits;
            string netz = AlsIp((uint)position);
            SubnetResult s = Subnet_Calculator.Calculate(netz, blockCidr);

            result.Zeilen.Add(new VlsmZeile
            {
                Hosts = hosts.ToString(CultureInfo.InvariantCulture),
                Netz = netz,
                Cidr = blockCidr,
                Erster = s.FirstHost,
                Letzter = s.LastHost,
                Broadcast = s.BroadcastAddress,
                Nutzbar = s.UsableHosts
            });

            result.VergebeneAdressen += groesse;
            position += groesse;
        }

        foreach (string wort in ungueltig)
        {
            result.Zeilen.Add(new VlsmZeile { Hosts = wort, Fehler = "Keine gültige Hostzahl." });
        }

        result.IsValid = true;
        return result;
    }

    private static string AlsIp(uint wert) =>
        $"{(wert >> 24) & 255}.{(wert >> 16) & 255}.{(wert >> 8) & 255}.{wert & 255}";
}
