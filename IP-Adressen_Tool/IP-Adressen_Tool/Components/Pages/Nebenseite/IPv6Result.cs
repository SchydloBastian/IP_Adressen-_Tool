using System;
using System.Net;
using System.Net.Sockets;

namespace IP_Adressen_Tool.Components.Pages.Nebenseite;

public class IPv6Result
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";
    public string MappedIPv6 { get; set; } = "";
    public string Translated6to4 { get; set; } = "";
    public string BinaryRepresentation { get; set; } = "";
}

public static class IPv4_zu_IPv6_Converter
{
    public static IPv6Result Convert(string ipv4Input)
    {
        var result = new IPv6Result();

        if (string.IsNullOrWhiteSpace(ipv4Input))
        {
            result.ErrorMessage = "Bitte gib eine IPv4-Adresse ein.";
            return result;
        }

        if (!IPAddress.TryParse(ipv4Input.Trim(), out IPAddress? ipv4Address) || ipv4Address.AddressFamily != AddressFamily.InterNetwork)
        {
            result.ErrorMessage = "Ungültige IPv4-Adresse.";
            return result;
        }

        byte[] bytes = ipv4Address.GetAddressBytes();

        try
        {
            byte[] mappedBytes = new byte[16];
            mappedBytes[10] = 0xff;
            mappedBytes[11] = 0xff;
            mappedBytes[12] = bytes[0];
            mappedBytes[13] = bytes[1];
            mappedBytes[14] = bytes[2];
            mappedBytes[15] = bytes[3];
            result.MappedIPv6 = new IPAddress(mappedBytes).ToString();

            string hexPart = $"{bytes[0]:X2}{bytes[1]:X2}:{bytes[2]:X2}{bytes[3]:X2}";
            result.Translated6to4 = $"2002:{hexPart}::";

            result.BinaryRepresentation = $"{Convert.ToString(bytes[0], 2).PadLeft(8, '0')}." +
                                           $"{Convert.ToString(bytes[1], 2).PadLeft(8, '0')}." +
                                           $"{Convert.ToString(bytes[2], 2).PadLeft(8, '0')}." +
                                           $"{Convert.ToString(bytes[3], 2).PadLeft(8, '0')}";

            result.IsValid = true;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = "Fehler bei der Konvertierung: " + ex.Message;
        }

        return result;
    }
}