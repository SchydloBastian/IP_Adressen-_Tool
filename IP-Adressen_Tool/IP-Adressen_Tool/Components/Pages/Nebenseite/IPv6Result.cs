using System.Net;

namespace IP_Adressen_Tool.Components.Pages.Nebenseite;

public class IPv6Result
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = ""; public string FullIPv6 { get; set; } = ""; public string CompressedIPv6 { get; set; } = ""; public string BinaryIPv6 { get; set; } = "";
}
{     public static IPv6Result Convert(string input)
{
    var result = new IPv6Result(); if (string.IsNullOrWhiteSpace(input)) { result.ErrorMessage = "Bitte gib eine IPv6-Adresse ein."; return result; }
    if (!IPAddress.TryParse(input.Trim(), out IPAddress? address) || address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6) { result.ErrorMessage = "Ungültiges IPv6-Format."; return result; }
    byte[] bytes = address.GetAddressBytes();         // 1. Vollständige / Expandierte Form (8 Blöcke à 4 Hex-Zeichen)var fullBuilder = new StringBuilder();         for (int i = 0; i < 16; i += 2)         {             ushort block = (ushort)((bytes[i] << 8) | bytes[i + 1]);             fullBuilder.Append(block.ToString("x4"));

