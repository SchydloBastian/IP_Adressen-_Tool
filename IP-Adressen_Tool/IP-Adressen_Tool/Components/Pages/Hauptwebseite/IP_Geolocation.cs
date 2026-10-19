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

