using System.Net;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public class NetzBefehleResult
{

    public bool IsValid { get; set; }

    public string ErrorMessage { get; set; } = "";

    public string Linux { get; set; } = "";

    public string Windows { get; set; } = "";

    public string Cisco { get; set; } = "";

}

// Baut nur Texte. Es wird nichts an echte Geräte geschickt.
public static class Netz_Befehle{

    public const string GatewayPlatzhalter = "<Gateway>";


    public static NetzBefehleResult Erzeugen(string netz, int cidr, string? gateway)

    {

        var result = new NetzBefehleResult();


        if (cidr < 0 || cidr > 32)

        {

            result.ErrorMessage = "Das CIDR-Präfix muss zwischen 0 und 32 liegen.";

            return result;

        }


        string netzText = (netz ?? "").Trim();

        if (!IP_Validierung.TryValidateIPv4(netzText, out string netzFehler, out _))

        {

            result.ErrorMessage = "Netz: " + netzFehler;

            return result;

        }


        string gw = (gateway ?? "").Trim();

        if (gw.Length == 0)

        {

            gw = GatewayPlatzhalter;

        }

        else if (!IP_Validierung.TryValidateIPv4(gw, out string gatewayFehler, out _))

        {

            result.ErrorMessage = "Gateway: " + gatewayFehler;

            return result;

        }


        byte[] b = IPAddress.Parse(netzText).GetAddressBytes();

        uint ip = ((uint)b[0] << 24) | ((uint)b[1] << 16) | ((uint)b[2] << 8) | b[3];

        uint netzwerk = ip & CIDR_Unterstuetzung.CidrToMaskUint(cidr);


        string netzAdresse = AlsIp(netzwerk);

        string maske = CIDR_Unterstuetzung.CidrToSubnetMask(cidr);


        result.Linux = $"ip route add {netzAdresse}/{cidr} via {gw}";

        result.Windows = $"route add {netzAdresse} mask {maske} {gw}";

        result.Cisco = $"ip route {netzAdresse} {maske} {gw}";

        result.IsValid = true;

        return result;

    }


    private static string AlsIp(uint wert) =>

        $"{(wert >> 24) & 255}.{(wert >> 16) & 255}.{(wert >> 8) & 255}.{wert & 255}";

}