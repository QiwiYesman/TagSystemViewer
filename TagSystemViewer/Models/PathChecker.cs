using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace TagSystemViewer.Models;

public static class PathChecker
{
    public static bool IsLocalFile(string path) => File.Exists(path);

    public static async Task<bool> IsWebConnected(string url)
    {
        try
        {
            
            using var client = new HttpClient();
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head,  url));
            return response.StatusCode == HttpStatusCode.OK;
        }            
        catch
        {
            return false;
        }
    }
}