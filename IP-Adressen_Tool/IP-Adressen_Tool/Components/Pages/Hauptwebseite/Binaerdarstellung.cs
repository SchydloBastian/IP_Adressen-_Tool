using System.Net;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite
{
    public static class Binaerdarstellung
    {

        public static string ToBinary(string ipString)

        {

            if (!IPAddress.TryParse(ipString, out IPAddress? address)) return "Ungültig";

            byte[] bytes = address.GetAddressBytes();





        }
    }
}
