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

