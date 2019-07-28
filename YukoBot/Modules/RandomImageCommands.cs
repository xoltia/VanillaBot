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
    [Name("image")]
    public class RandomImageCommands : ModuleBase<SocketCommandContext>
    {
        private readonly HttpService _http;

        public RandomImageCommands(HttpService http)
        {
            _http = http;
        }

        // Promise I'm not a weeb
        [Command("kitsune")]
        [Summary("Sends random kitsune image.")]
        public async Task Kitsune()
        {
            string response = await _http.GetContent("https://api.nekos.dev/api/v3/images/sfw/img/kitsune/");
            JObject json = JsonConvert.DeserializeObject<JObject>(response);
            // TODO: error checking
            string url = json["data"]["response"]["url"].ToString();
            await ReplyAsync(embed: new EmbedBuilder()
                .WithTitle("Link")
                .WithUrl(url)
                .WithImageUrl(url)
                .Build());
        }

        [Command("yuko")]
        [Summary("Get the best kitsune image.")]
        public async Task Yuko()
        {
            await ReplyAsync(embed: new EmbedBuilder()
                .WithTitle("Link")
                .WithUrl("https://static.xoltia.com/images/yuko.jpg")
                .WithImageUrl("https://static.xoltia.com/images/yuko.jpg")
                .Build());
        }
    }
}
