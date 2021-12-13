using Newtonsoft.Json;
using RemoteGallery.Models;
using Serilog;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RemoteGallery.Services
{
    public interface ITmdbResolverService
    {
        public Task<Title?> GetTitle(string titleId);

    }
    public class TmdbResolverService : ITmdbResolverService
    {
        private static byte[] _tmdbKey = new byte[] { 0xF5, 0xDE, 0x66, 0xD2, 0x68, 0x0E, 0x25, 0x5B, 0x2D, 0xF7, 0x9E, 0x74, 0xF8, 0x90, 0xEB, 0xF3, 0x49, 0x26, 0x2F, 0x61, 0x8B, 0xCA, 0xE2, 0xA9, 0xAC, 0xCD, 0xEE, 0x51, 0x56, 0xCE, 0x8D, 0xF2, 0xCD, 0xF2, 0xD4, 0x8C, 0x71, 0x17, 0x3C, 0xDC, 0x25, 0x94, 0x46, 0x5B, 0x87, 0x40, 0x5D, 0x19, 0x7C, 0xF1, 0xAE, 0xD3, 0xB7, 0xE9, 0x67, 0x1E, 0xEB, 0x56, 0xCA, 0x67, 0x53, 0xC2, 0xE6, 0xB0 };

        private HttpClient HttpClient;

        public TmdbResolverService()
        {
            HttpClient = new HttpClient();
            HttpClient.Timeout = TimeSpan.FromSeconds(20);
        }

        private string CreateTitleIdUrl(string titleId)
        {
            var encoding = new ASCIIEncoding();
            byte[] messageBytes = encoding.GetBytes(titleId);
            using (var sha1 = new HMACSHA1(_tmdbKey))
            {
                byte[] hashmessage = sha1.ComputeHash(messageBytes);
                var hash = BitConverter.ToString(hashmessage).ToUpper().Replace("-", string.Empty);
                return $"https://tmdb.np.dl.playstation.net/tmdb2/{titleId}_{hash}/{titleId}.json";
            }
        }

        public async Task<Title?> GetTitle(string titleId)
        {
            // Slow but needs to be done
            if (!titleId.EndsWith("_00"))
            {
                titleId += "_00";
            }

            var url = CreateTitleIdUrl(titleId);

            try
            {
                var response = await HttpClient.GetAsync(url);

                return JsonConvert.DeserializeObject<Title>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception ex)
            {
                Log.Debug(ex, $"Failed getting title id content for {titleId} ({url})");
                return default;
            }
        }
    }
}
