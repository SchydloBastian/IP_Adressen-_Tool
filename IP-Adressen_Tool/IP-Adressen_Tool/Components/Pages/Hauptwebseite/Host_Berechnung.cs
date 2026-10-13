namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public static class Host_Berechnung
{
    public static long GetMaxHosts(int cidr)
    {
        if (cidr >= 32) return 1;
        if (cidr == 31) return 2;
        if (cidr <= 0) return 4294967294L;
        return (1L << (32 - cidr)) - 2;
    }
}