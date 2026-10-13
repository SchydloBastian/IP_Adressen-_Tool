using System.Net;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite
{
    public static class Binaerdarstellung
    {

        public static string ToBinary(string ipString)

        {

            if (!IPAddress.TryParse(ipString, out IPAddress? address)) return "Ungültig";

            byte[] bytes = address.GetAddressBytes();

            if (bytes.Length != 4) return "Ungültig";

            return Convert.ToString(bytes[0], 2).PadLeft(8, '0') + "." +

               Convert.ToString(bytes[1], 2).PadLeft(8, '0') + "." +

               Convert.ToString(bytes[2], 2).PadLeft(8, '0') + "." +

               Convert.ToString(bytes[3], 2).PadLeft(8, '0');

        }
    }
}
