using System.Globalization;
using System.Net;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public class HostPositionResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";
    public string Adresse { get; set; } = "";
    public long Nummer { get; set; }
    public long MaxHosts { get; set; }
}

public static class Host_Berechnung
{
    public static long GetMaxHosts(int cidr)
    {
        if (cidr >= 32) return 1;
        if (cidr == 31) return 2;
        if (cidr <= 0) return 4294967294L;
        return (1L << (32 - cidr)) - 2;
    }

    public static HostPositionResult HostAnPosition(string netz, int cidr, long nummer)
    {
        var result = new HostPositionResult();

        if (cidr < 0 || cidr > 32)
        {
            result.ErrorMessage = "Das CIDR-Präfix muss zwischen 0 und 32 liegen.";
            return result;
        }

        string netzText = (netz ?? "").Trim();
        if (!IP_Validierung.TryValidateIPv4(netzText, out string fehler, out _))
        {
            result.ErrorMessage = fehler;
            return result;
        }

        byte[] b = IPAddress.Parse(netzText).GetAddressBytes();
        uint ip = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];
        uint netzwerk = ip & CIDR_Unterstuetzung.CidrToMaskUint(cidr);

        long max = GetMaxHosts(cidr);
        result.MaxHosts = max;

        if (nummer < 1 || nummer > max)
        {
            result.ErrorMessage = $"Die Nummer muss zwischen 1 und {max.ToString("N0", CultureInfo.GetCultureInfo("de-DE"))} liegen.";
            return result;
        }

        long adresse = cidr >= 31 ? netzwerk + (nummer - 1) : netzwerk + nummer;

        result.Nummer = nummer;
        result.Adresse = AlsIp((uint)adresse);
        result.IsValid = true;
        return result;
    }

    private static string AlsIp(uint wert) =>
        $"{(wert >> 24) & 255}.{(wert >> 16) & 255}.{(wert >> 8) & 255}.{wert & 255}";
}