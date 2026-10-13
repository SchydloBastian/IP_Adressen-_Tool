using System.Net;
using System.Net.Sockets;
namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public static class IP_Validierung
{
    public static bool TryValidateIPv4(string ipString, out string errorMessage, out string addressClass)
    {
        errorMessage = "";
        addressClass = "-";

        if (string.IsNullOrWhiteSpace(ipString))
        {
            errorMessage = "Die IP-Adresse darf nicht leer sein.";
            return false;
        }

        string[] parts = ipString.Split('.');
        if (parts.Length != 4)
        {
            errorMessage = "Eine IPv4-Adresse muss genau 4 Oktette besitzen.";
            return false;
        }

        foreach (string part in parts)
        {
            if (!int.TryParse(part, out int val) || val < 0 || val > 255)
            {
                errorMessage = $"Ungültiges Oktett '{part}'. Werte müssen zwischen 0 und 255 liegen.";
                return false;
            }

            if (part.Length > 1 && part.StartsWith('0'))
            {
                errorMessage = $"Ungültiges Oktett '{part}' (führende Nullen sind unzulässig).";
                return false;
            }
        }

        if (!IPAddress.TryParse(ipString, out IPAddress? address) || address.AddressFamily != AddressFamily.InterNetwork)
        {
            errorMessage = "Ungültiges IPv4-Format.";
            return false;
        }

        int first = address.GetAddressBytes()[0];
        if (ipString == "0.0.0.0") addressClass = "Special / This Network";
        else if (ipString == "255.255.255.255") addressClass = "Special / Limited Broadcast";
        else if (first == 127) addressClass = "Special / Loopback";
        else if (first is >= 1 and <= 126) addressClass = "Klasse A";
        else if (first is >= 128 and <= 191) addressClass = "Klasse B";
        else if (first is >= 192 and <= 223) addressClass = "Klasse C";
        else if (first is >= 224 and <= 239) addressClass = "Klasse D (Multicast)";
        else if (first is >= 240 and <= 255) addressClass = "Klasse E (Forschung / Reserviert)";
        else addressClass = "Unbekannt";

        return true;
    }
}
