using System.Net.Http;
using System.Threading.Tasks;
using Gallery.Constants;

namespace Gallery.Services;

public class UpdateService
{
    public static async Task<string> GetVersion()
    {
        using (var client = new HttpClient())
        {
            var version = await client.GetStringAsync(Data.VERSION_URL);
            return version.Trim();
        }
    }

    public static async Task<bool> HasUpdate() => await GetVersion() != Data.VERSION;
}
