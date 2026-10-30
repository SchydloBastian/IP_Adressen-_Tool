using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Numerics;

namespace IP_Adressen_Tool.Components.Pages.Nebenseite;

public class IPv6BereichResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";

    public string CidrNotation { get; set; } = "";
    public string FirstAddress { get; set; } = "";
    public string LastAddress { get; set; } = "";
    public string FirstAddressExpanded { get; set; } = "";
    public string LastAddressExpanded { get; set; } = "";
    public BigInteger TotalAddresses { get; set; }

    public BigInteger Networks64 { get; set; }
    public bool HasNetworks64 { get; set; }

    public string Range => $"{FirstAddress} – {LastAddress}";
    public string TotalAddressesText => Formatiere(TotalAddresses);
    public string Networks64Text => HasNetworks64 ? Formatiere(Networks64) : "Präfix ist größer als /64";

    private static string Formatiere(BigInteger zahl) =>
        zahl.ToString("N0", CultureInfo.GetCultureInfo("de-DE"));
}

public static class CIDR_Bereichs_Rechner_IPv6
{
    public static IPv6BereichResult Calculate(string ipInput, int prefixLength)
    {
        var result = new IPv6BereichResult();

        if (prefixLength < 0 || prefixLength > 128)
        {
            result.ErrorMessage = "Das Präfix muss zwischen 0 und 128 liegen.";
            return result;
        }

        if (string.IsNullOrWhiteSpace(ipInput)
            || !IPAddress.TryParse(ipInput.Trim(), out IPAddress? address)
            || address.AddressFamily != AddressFamily.InterNetworkV6)
        {
            result.ErrorMessage = "Ungültige IPv6-Adresse.";
            return result;
        }

        UInt128 ip = BytesToUInt128(address.GetAddressBytes());

        UInt128 mask = prefixLength == 0 ? UInt128.Zero : UInt128.MaxValue << (128 - prefixLength);
        UInt128 first = ip & mask;
        UInt128 last = first | ~mask;

        result.FirstAddress = ToIPv6(first);
        result.LastAddress = ToIPv6(last);
        result.FirstAddressExpanded = ToExpanded(first);
        result.LastAddressExpanded = ToExpanded(last);
        result.CidrNotation = $"{result.FirstAddress}/{prefixLength}";
        result.TotalAddresses = BigInteger.Pow(2, 128 - prefixLength);

        if (prefixLength <= 64)
        {
            result.HasNetworks64 = true;
            result.Networks64 = BigInteger.Pow(2, 64 - prefixLength);
        }

        result.IsValid = true;
        return result;
    }

    private static UInt128 BytesToUInt128(byte[] bytes)
    {
        UInt128 wert = UInt128.Zero;
        foreach (byte b in bytes)
        {
            wert = (wert << 8) | b;
        }
        return wert;
    }

    private static byte[] ToBytes(UInt128 wert)
    {
        byte[] bytes = new byte[16];
        for (int i = 15; i >= 0; i--)
        {
            bytes[i] = unchecked((byte)wert);
            wert >>= 8;
        }
        return bytes;
    }

    private static string ToIPv6(UInt128 wert) => new IPAddress(ToBytes(wert)).ToString();

    private static string ToExpanded(UInt128 wert)
    {
        byte[] b = ToBytes(wert);
        var gruppen = new string[8];
        for (int i = 0; i < 8; i++)
        {
            gruppen[i] = $"{b[i * 2]:x2}{b[i * 2 + 1]:x2}";
        }
        return string.Join(":", gruppen);
    }
}
