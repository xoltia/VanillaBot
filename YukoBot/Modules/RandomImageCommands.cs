using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using YukoBot.Services;

namespace YukoBot.Modules
{
    [Name("Images")]
    public class RandomImageCommands : ModuleBase<SocketCommandContext>
    {
        private readonly HttpService _http;

        public RandomImageCommands(HttpService http)
        {
            _http = http;
        }

        private async Task<string> GetNekosLifeImageUrl(string endpoint)
        {
            string response = await _http.GetContent("https://api.nekos.dev/api/v3/images" + endpoint);
            JObject json = JsonConvert.DeserializeObject<JObject>(response);
            if (json["data"]["status"]["code"].ToObject<int>() != 200)
            {
                throw new Exception(json["data"]["status"]["message"].ToString());
            }
            return json["data"]["response"]["url"].ToString();
        }

        private Task<IUserMessage> SendImageUrl(string url)
        {
            return ReplyAsync(embed: new EmbedBuilder()
                .WithTitle("Link")
                .WithUrl(url)
                .WithImageUrl(url)
                .Build());
        }

        private async Task<IUserMessage> SendRandomImageUrl(string endpoint)
        {
            string url = await GetNekosLifeImageUrl(endpoint);
            return await SendImageUrl(url);
        }

        // Promise I'm not a weeb
        [Command("kitsune")]
        [Summary("Sends a random kitsune image.")]
        public async Task Kitsune() =>
            await SendRandomImageUrl("/sfw/img/kitsune");

        [Command("neko")]
        [Summary("Sends a random neko image.")]
        public async Task Neko() =>
            await SendRandomImageUrl("/sfw/img/neko");

        [Command("neko-gif"), Alias("nekogif")]
        [Summary("Sends a random neko gif.")]
        public async Task NekoGif() => 
            await SendRandomImageUrl("/sfw/gif/neko");

        [Command("yuko")]
        [Summary("Get the best kitsune image.")]
        public async Task Yuko() =>
            await SendImageUrl("https://static.xoltia.com/images/yuko.jpg");
    }
}
