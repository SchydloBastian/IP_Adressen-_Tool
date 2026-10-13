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
            if (!IPAddress.TryParse(ipString, out IPAddress? address) || address.AddressFamily != AddressFamily.InterNetwork)

            {

                errorMessage = "Ungültiges IPv4-Format.";

                return false;

            }


        }

    }



}