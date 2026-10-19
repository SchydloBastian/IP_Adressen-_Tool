using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace IP_Adressen_Tool.Components.Pages.Nebenseite;

public static class EigeneIP_Helper
{
    private static readonly HttpClient httpClient = new HttpClient();

    
    public static string GetLokaleIP()
    {
        try
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                if (socket.LocalEndPoint is IPEndPoint endPoint)
                {
                    return endPoint.Address.ToString();
                }
            }
        }
        catch
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
            }
            catch { }
        }

        return "127.0.0.1";
    }

    
    public static async Task<string> GetOeffentlicheIPAsync()
    {
        try
        {
            var response = await httpClient.GetStringAsync("https://api.ipify.org");
            return response.Trim();
        }
        catch
        {
            return GetLokaleIP();
        }
    }
}