using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MusicSchoolWpf
{
    public class ItunesMusicService
    {
        private readonly HttpClient client = new HttpClient();

        public async Task<List<ItunesTrack>> SearchSongsAsync(string searchText, int limit = 10)
        {
            if (string.IsNullOrWhiteSpace(searchText))
                return new List<ItunesTrack>();

            string encoded = Uri.EscapeDataString(searchText);

            string url = $"https://itunes.apple.com/search?term={encoded}&entity=song&limit={limit}";

            try
            {
                string json = await client.GetStringAsync(url);

                ItunesSearchResponse? response =
                    JsonSerializer.Deserialize<ItunesSearchResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                return response?.Results ?? new List<ItunesTrack>();
            }
            catch
            {
                return new List<ItunesTrack>();
            }
        }

        public async Task<string?> DownloadImageAsBase64Async(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            try
            {
                HttpResponseMessage response = await client.GetAsync(imageUrl);

                if (!response.IsSuccessStatusCode)
                    return null;

                byte[] imageBytes = await response.Content.ReadAsByteArrayAsync();

                if (imageBytes.Length == 0)
                    return null;

                return Convert.ToBase64String(imageBytes);
            }
            catch
            {
                return null;
            }
        }
    }
}