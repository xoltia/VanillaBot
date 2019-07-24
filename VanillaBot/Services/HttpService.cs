using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Services
{
    public class HttpService
    {
        private readonly HttpClient _httpClient;
        private readonly MemoryCache _cache;

        public HttpService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = new HttpClient();
            _cache = new MemoryCache(new MemoryCacheOptions());

            string userAgent = config["userAgent"];
            _httpClient.DefaultRequestHeaders.Add("User-Agent", string.IsNullOrEmpty(userAgent) ? "VanillaBot (https://github.com/xoltia/VanillaBot)" : userAgent);
        }

        public async Task<string> GetContent(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<T> GetObjectAsync<T>(string url, TimeSpan? cacheDuration = null)
        {
            string content;

            if (cacheDuration != null)
            {
                content = await _cache.GetOrCreateAsync(url, async (ICacheEntry entry) =>
                {
                    entry.AbsoluteExpirationRelativeToNow = cacheDuration;
                    return await GetContent(url);
                });
            }
            else if (!_cache.TryGetValue(url, out content))
            {
                content = await GetContent(url);
            }

            return JsonConvert.DeserializeObject<T>(content);
        }

        // Don't need right now but here for completeness sake.
        public Task<HttpResponseMessage> PostObjectAsync(string url, object obj)
        {
            string content = JsonConvert.SerializeObject(obj);
            ByteArrayContent byteContent = new ByteArrayContent(Encoding.UTF8.GetBytes(content));
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return _httpClient.PostAsync(url, byteContent);
        }
    }
}
