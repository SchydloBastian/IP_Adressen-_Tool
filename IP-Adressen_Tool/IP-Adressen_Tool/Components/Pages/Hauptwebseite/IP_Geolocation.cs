using System.Net;
using System.Net.Http.Json;

namespace IP_Adressen_Tool.Components.Pages.Hauptwebseite;

public class GeolocationResult
{
    public bool Gefunden { get; set; }
    public string Text { get; set; } = "";
    public string Land { get; set; } = "-";
    public string Region { get; set; } = "-";
    public string Stadt { get; set; } = "-";
    public string Anbieter { get; set; } = "-";
    public string Koordinaten { get; set; } = "-";
}

public static class IP_Geolocation
{
    public static async Task<GeolocationResult> LookupAsync(string ipString)
    {
        if (IstPrivatOderSpezial(ipString))
        {
            return new GeolocationResult
            {
                Gefunden = false,
                Text = "Kein Standort. Private, lokale oder spezielle Adresse."
            };
        }

        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(8) };
            var daten = await client.GetFromJsonAsync<IpWhoAntwort>($"https://ipwho.is/{ipString}");

            if (daten is null || !daten.Success)
            {
                return new GeolocationResult
                {
                    Gefunden = false,
                    Text = "Standort konnte nicht ermittelt werden."
                };
            }

            return new GeolocationResult
            {
                Gefunden = true,
                Text = $"{daten.City}, {daten.Region}, {daten.Country}",
                Land = daten.Country,
                Region = daten.Region,
                Stadt = daten.City,
                Anbieter = daten.Connection?.Isp ?? "-",
                Koordinaten = $"{daten.Latitude:0.####}, {daten.Longitude:0.####}"
            };
        }
        catch
        {
            return new GeolocationResult
            {
                Gefunden = false,
                Text = "Standortdienst nicht erreichbar."
            };
        }
    }

    private static bool IstPrivatOderSpezial(string ipString)
    {
        if (!IPAddress.TryParse(ipString, out IPAddress? adresse)) return true;
        byte[] b = adresse.GetAddressBytes();
        if (b.Length != 4) return true;

        if (b[0] == 10) return true;
        if (b[0] == 127) return true;
        if (b[0] == 0) return true;
        if (b[0] == 169 && b[1] == 254) return true;
        if (b[0] == 192 && b[1] == 168) return true;
        if (b[0] == 172 && b[1] >= 16 && b[1] <= 31) return true;
        if (b[0] >= 224) return true;
        return false;
    }

    private sealed class IpWhoAntwort
    {
        public bool Success { get; set; }
        public string Country { get; set; } = "";
        public string Region { get; set; } = "";
        public string City { get; set; } = "";
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public IpWhoVerbindung? Connection { get; set; }
    }

    private sealed class IpWhoVerbindung
    {
        public string Isp { get; set; } = "";
    }
}