using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace IP_Adressen_Tool.Components.Pages.Nebenseite;

public class EUI64Result
{
    public bool IsValid { get; set; }
    public string ErrorMessage { get; set; } = "";

    public string Mac { get; set; } = "";

    public string InterfaceId { get; set; } = "";

    public bool HatAdresse { get; set; }
    public string AdresseKomprimiert { get; set; } = "";
    public string AdresseExpandiert { get; set; } = "";
}

public static class EUI64_Rechner
{
    private static readonly Regex MacMuster = new Regex(
        @"^(?:[0-9A-Fa-f]{2}([:-])(?:[0-9A-Fa-f]{2}\1){4}[0-9A-Fa-f]{2}|[0-9A-Fa-f]{12})$");

    public static EUI64Result Berechne(string mac, string? praefix)
    {
        var result = new EUI64Result();

        string macText = (mac ?? "").Trim();
        if (!MacMuster.IsMatch(macText))
        {
            result.ErrorMessage = "Ungültige MAC-Adresse. Erlaubt sind 00:11:22:33:44:55, 00-11-22-33-44-55 oder 001122334455.";
            return result;
        }

        byte[] m = Convert.FromHexString(macText.Replace(":", "").Replace("-", ""));
        byte[] kennung = { (byte)(m[0] ^ 0x02), m[1], m[2], 0xFF, 0xFE, m[3], m[4], m[5] };

        string macSchreibweise = string.Join(":", m.Select(x => x.ToString("X2")));
        string interfaceId =
            $"{kennung[0]:x2}{kennung[1]:x2}:{kennung[2]:x2}{kennung[3]:x2}:" +
            $"{kennung[4]:x2}{kennung[5]:x2}:{kennung[6]:x2}{kennung[7]:x2}";

        if (string.IsNullOrWhiteSpace(praefix))
        {
            result.Mac = macSchreibweise;
            result.InterfaceId = interfaceId;
            result.IsValid = true;
            return result;
        }

        string praefixText = praefix.Trim();
        string adressTeil = praefixText;
        int laenge = 64;

        int strich = praefixText.IndexOf('/');
        if (strich >= 0)
        {
            adressTeil = praefixText.Substring(0, strich).Trim();
            if (!int.TryParse(praefixText.Substring(strich + 1).Trim(), out laenge))
            {
                result.ErrorMessage = "Ungültige Präfixlänge.";
                return result;
            }
        }

        if (laenge != 64)
        {
            result.ErrorMessage = "Für EUI-64 muss das Präfix /64 lang sein.";
            return result;
        }

        if (!IPAddress.TryParse(adressTeil, out IPAddress? praefixAdresse)
            || praefixAdresse.AddressFamily != AddressFamily.InterNetworkV6)
        {
            result.ErrorMessage = "Ungültiges IPv6-Präfix.";
            return result;
        }

        byte[] adresse = new byte[16];
        Array.Copy(praefixAdresse.GetAddressBytes(), adresse, 8);
        Array.Copy(kennung, 0, adresse, 8, 8);

        result.Mac = macSchreibweise;
        result.InterfaceId = interfaceId;
        result.AdresseKomprimiert = new IPAddress(adresse).ToString();
        result.AdresseExpandiert = string.Join(":",
            Enumerable.Range(0, 8).Select(i => $"{adresse[i * 2]:x2}{adresse[i * 2 + 1]:x2}"));
        result.HatAdresse = true;
        result.IsValid = true;
        return result;
    }
}
