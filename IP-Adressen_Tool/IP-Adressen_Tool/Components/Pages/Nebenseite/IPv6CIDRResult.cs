using System;
using System.Net;
using System.Text;
using System.Numerics;
namespace IP_Adressen_Tool.Components.Pages.Nebenseite;
public class IPv6CIDRResult
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";
    public string NetworkAddress { get; set; } = "";
    public string LastHost { get; set; } = "";
    public BigInteger TotalHosts { get; set; }
}
public static class IPv6_CIDR_Rechner
{
    public static IPv6CIDRResult Calculate(string ipInput, int prefixLength)
    {
        var result = new IPv6CIDRResult();
        if (prefixLength < 0 || prefixLength > 128)
        {
            result.ErrorMessage = "Das Präfix muss zwischen 0 und 128 liegen.";
            return result;
        }
 
        if (string.IsNullOrWhiteSpace(ipInput) || !IPAddress.TryParse(ipInput.Trim(), out IPAddress? address) || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            result.ErrorMessage = "Ungültige IPv6-Adresse.";
            return result;
        }
 
        byte[] ipBytes = address.GetAddressBytes();
        BigInteger ipInt = BytesToBigInt(ipBytes);
        BigInteger maskInt = prefixLength == 0 ? 0 : (BigInteger.MinusOne << (128 - prefixLength));
        BigInteger netInt = ipInt & maskInt;
        BigInteger broadcastInt = netInt | ~maskInt;

        result.NetworkAddress = BigIntToIPv6(netInt);
        result.LastHost = BigIntToIPv6(broadcastInt);
        result.TotalHosts = prefixLength == 128 ? 1 : BigInteger.Pow(2, 128 - prefixLength);
        result.IsValid = true;
        return result;

    }private static BigInteger BytesToBigInt(byte[] bytes)
    {
        byte[] reversed = (byte[])bytes.Clone();
        Array.Reverse(reversed);
        byte[] extended = new byte[reversed.Length + 1];
        Array.Copy(reversed, extended, reversed.Length);
        return new BigInteger(extended);
    }
 
    private static string BigIntToIPv6(BigInteger bigInt)
    {
        byte[] bytes = bigInt.ToByteArray();
        byte[] ipBytes = new byte[16];
        int copyLength = Math.Min(bytes.Length, 16);

        Array.Copy(bytes, 0, ipBytes, 16 - copyLength, copyLength);

        Array.Reverse(ipBytes);
 
        var sb = new StringBuilder();

        for (int i = 0; i < 16; i += 2)
        {
            ushort block = (ushort)((ipBytes[i] << 8) | ipBytes[i + 1]);

            sb.Append(block.ToString("x4"));

            if (i < 14) sb.Append(':');
        }
 
        if (IPAddress.TryParse(sb.ToString(), out IPAddress? addr))
        {
            return addr.ToString();

        }
        return sb.ToString();

    }

}
 