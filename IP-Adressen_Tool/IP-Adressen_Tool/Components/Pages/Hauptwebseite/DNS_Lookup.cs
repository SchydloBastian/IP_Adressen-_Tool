

using IP_Adressen_Tool.Components.Pages.Hauptwebseite;
using System.Net;
using System.Net.Sockets;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public class DnsLookupResult
{

    public bool Gefunden { get; set; }

    public string Text { get; set; } = "";

    public List<string> Namen { get; set; } = new();

    public List<string> IPv4Adressen { get; set; } = new();

    public List<string> IPv6Adressen { get; set; } = new();

}
public static class DNS_Lookup
{

    private static readonly TimeSpan Zeitlimit = TimeSpan.FromSeconds(5);


    // IP gibt den PTR-Namen zurück, ein Hostname gibt A- und AAAA-Adressen zurück.
    public static async Task<DnsLookupResult> AbfragenAsync(string eingabe)
    {

        string text = (eingabe ?? "").Trim();

        if (text.Length == 0) return Fehler("Bitte eine IP oder einen Hostnamen angeben.");


        if (IPAddress.TryParse(text, out IPAddress? adresse))

        {

            return await PtrAbfragenAsync(adresse);

        }


        return await NameAbfragenAsync(text);

    }


    private static async Task<DnsLookupResult> PtrAbfragenAsync(IPAddress adresse)
    {

        if (IstPrivatOderSpezial(adresse))
        {

            return Fehler("Private oder spezielle Adresse. Es wird nicht abgefragt.");

        }


        try
        {

            IPHostEntry eintrag = await Dns.GetHostEntryAsync(adresse).WaitAsync(Zeitlimit);

            string name = eintrag.HostName;


            if (string.IsNullOrWhiteSpace(name) || name == adresse.ToString())

            {

                return Fehler("Kein DNS-Name gefunden.");

            }


            var result = new DnsLookupResult { Gefunden = true, Text = name };

            result.Namen.Add(name);

            return result;

        }

        catch (TimeoutException)
        {

            return Fehler("Keine Antwort nach 5 Sekunden.");

        }

        catch (SocketException)
        {

            return Fehler("Kein DNS-Name gefunden.");

        }

        catch (Exception)
        {

            return Fehler("DNS-Abfrage nicht möglich.");

        }

    }


    private static async Task<DnsLookupResult> NameAbfragenAsync(string hostname)
    {

        if (Uri.CheckHostName(hostname) != UriHostNameType.Dns)

        {

            return Fehler("Das ist weder eine gültige IP noch ein gültiger Hostname.");

        }


        try
        {

            IPAddress[] adressen = await Dns.GetHostAddressesAsync(hostname).WaitAsync(Zeitlimit);

            var result = new DnsLookupResult();


            foreach (IPAddress a in adressen)

            {

                if (a.AddressFamily == AddressFamily.InterNetwork) result.IPv4Adressen.Add(a.ToString());

                else if (a.AddressFamily == AddressFamily.InterNetworkV6) result.IPv6Adressen.Add(a.ToString());

            }


            if (result.IPv4Adressen.Count == 0 && result.IPv6Adressen.Count == 0)

            {

                return Fehler("Keine A- oder AAAA-Einträge gefunden.");

            }


            var teile = new List<string>();

            if (result.IPv4Adressen.Count > 0) teile.Add("A: " + string.Join(", ", result.IPv4Adressen));

            if (result.IPv6Adressen.Count > 0) teile.Add("AAAA: " + string.Join(", ", result.IPv6Adressen));


            result.Gefunden = true;

            result.Text = string.Join(" | ", teile);

            return result;

        }

        catch (TimeoutException)

        {

            return Fehler("Keine Antwort nach 5 Sekunden.");

        }

        catch (SocketException)

        {

            return Fehler("Hostname nicht gefunden.");

        }

        catch (Exception)

        {

            return Fehler("DNS-Abfrage nicht möglich.");

        }

    }


    private static DnsLookupResult Fehler(string text) => new DnsLookupResult { Gefunden = false, Text = text };


    // Private und spezielle Adressen werden nicht abgefragt.
    private static bool IstPrivatOderSpezial(IPAddress adresse)
    {

        if (adresse.IsIPv4MappedToIPv6) adresse = adresse.MapToIPv4();

        byte[] b = adresse.GetAddressBytes();


        if (b.Length == 4)

        {

            if (b[0] == 0 || b[0] == 10 || b[0] == 127) return true;

            if (b[0] == 100 && b[1] >= 64 && b[1] <= 127) return true;          // Shared Address Spaceif (b[0] == 169 && b[1] == 254) return true;                          // Link-Localif (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;

            if (b[0] == 192 && b[1] == 0 && b[2] == 0) return true;

            if (b[0] == 192 && b[1] == 0 && b[2] == 2) return true;               // Dokumentationif (b[0] == 192 && b[1] == 168) return true;

            if (b[0] == 198 && (b[1] == 18 || b[1] == 19)) return true;           // Benchmarkif (b[0] == 198 && b[1] == 51 && b[2] == 100) return true;            // Dokumentationif (b[0] == 203 && b[1] == 0 && b[2] == 113) return true;             // Dokumentationif (b[0] >= 224) return true;                                         // Multicast, reserviert, Broadcastreturn false;

        }


        // IPv6: nur globale Unicast-Adressen (2000::/3), ohne den Dokumentationsbereich 2001:db8::/32if ((b[0] & 0xE0) != 0x20) return true;

        if (b[0] == 0x20 && b[1] == 0x01 && b[2] == 0x0d && b[3] == 0xb8) return true;

        return false;

    }

}

