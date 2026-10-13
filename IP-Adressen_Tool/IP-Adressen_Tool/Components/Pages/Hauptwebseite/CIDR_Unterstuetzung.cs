namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public static class CIDR_Unterstuetzung
{

    public static string CidrToSubnetMask(int cidr)

    {

        uint mask = CidrToMaskUint(cidr);

        return $"{(mask >> 24) & 255}.{(mask >> 16) & 255}.{(mask >> 8) & 255}.{mask & 255}";

    }


    public static uint CidrToMaskUint(int cidr)

    {

        if (cidr <= 0) return 0;

        if (cidr >= 32) return 0xFFFFFFFFu;

        return 0xFFFFFFFFu << (32 - cidr);

    }

}