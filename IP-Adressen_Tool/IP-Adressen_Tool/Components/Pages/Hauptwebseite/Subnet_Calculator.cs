using System.Net;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public class SubnetResult
{
    public string NetworkAddress { get; set; } = "";
    public string BroadcastAddress { get; set; } = "";
    public string FirstHost { get; set; } = "";
    public string LastHost { get; set; } = "";
    public long UsableHosts { get; set; }
    public string SubnetMask { get; set; } = "";
}

public static class Subnet_Calculator
{
    public static SubnetResult Calculate(string ipString, int cidr)
    {
        var result = new SubnetResult();
        if (cidr < 0) cidr = 0;
        if (cidr > 32) cidr = 32;

        result.SubnetMask = CIDR_Unterstuetzung.CidrToSubnetMask(cidr);
        if (!IPAddress.TryParse(ipString, out IPAddress? ipAddress)) return result;

        byte[] b = ipAddress.GetAddressBytes();
        uint ip =
            ((uint)b[0] << 24) |
            ((uint)b[1] << 16) |
            ((uint)b[2] << 8) |
            b[3];

        uint mask = CIDR_Unterstuetzung.CidrToMaskUint(cidr);
        uint netz = ip & mask;
        uint broadcast = netz | ~mask;

        result.NetworkAddress = AlsIp(netz);
        result.BroadcastAddress = AlsIp(broadcast);
        result.UsableHosts = Host_Berechnung.GetMaxHosts(cidr);

        if (cidr >= 32)
        {
            result.FirstHost = result.NetworkAddress;
            result.LastHost = result.NetworkAddress;
        }
        else if (cidr == 31)
        {
            result.FirstHost = result.NetworkAddress;
            result.LastHost = result.BroadcastAddress;
        }
        else
        {
            result.FirstHost = AlsIp(netz + 1);
            result.LastHost = AlsIp(broadcast - 1);
        }

        return result;
    }

    private static string AlsIp(uint wert) =>
        $"{(wert >> 24) & 255}.{(wert >> 16) & 255}.{(wert >> 8) & 255}.{wert & 255}";
}