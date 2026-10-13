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

    }



}