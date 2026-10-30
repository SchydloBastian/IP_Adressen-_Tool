using System.Globalization;
using System.Net;
using System.Net.Sockets;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public class CidrBereichIPv4Result
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";

    // z. B. 192.168.1.0/24
    public string CidrNotation { get; set; } = "";
    public string FirstAddress { get; set; } = "";
    public string LastAddress { get; set; } = "";
    public string WildcardMask { get; set; } = "";
    public long TotalAddresses { get; set; }

    public string Range => $"{FirstAddress} – {LastAddress}";
    public string TotalAddressesText => TotalAddresses.ToString("N0", CultureInfo.GetCultureInfo("de-DE"));
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

    private static string AlsIp(uint wert) =>
        $"{(wert >> 24) & 255}.{(wert >> 16) & 255}.{(wert >> 8) & 255}.{wert & 255}";
}
