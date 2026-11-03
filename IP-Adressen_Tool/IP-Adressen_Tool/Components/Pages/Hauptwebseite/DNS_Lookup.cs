

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


