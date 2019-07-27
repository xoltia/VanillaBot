using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using YukoBot.Modules.Search;
using YukoBot.Services;

namespace YukoBot.Modules
{
    [Name("search")]
    public class SearchCommands : ModuleBase<SocketCommandContext>
    {
        private readonly HttpService _http;

        private readonly string bingSearchEndpoint;
        private readonly string bingSearchKey;

        public SearchCommands(HttpService http, IConfiguration config)
        {
            _http = http;

            bingSearchEndpoint = config["bing:endpoint"];
            bingSearchKey = config["bing:key"];
        }

        [Command("stackoverflow"), Alias("so")]
        [Summary("Search StackOverflow.")]
        public async Task StackOverflow([Remainder]string question)
        {
            string escapedQuestion = Uri.EscapeDataString(question);
            StackOverflowSearch search = await _http.GetObjectAsync<StackOverflowSearch>("https://api.stackexchange.com/2.2/search/advanced?site=stackoverflow.com&q=" + escapedQuestion, TimeSpan.FromMinutes(30));
            if (search.Questions.Count == 0)
            {
                await ReplyAsync("I couldn't find anything.");
            }

            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle("**Here's what I found.**")
                .WithDescription($"For more results stop being lazy and [check yourself.](https://stackoverflow.com/search?q={escapedQuestion})");

            foreach (Question q in search.Questions.Take(5))
            {
                embedBuilder.AddField(q.Title,
                    $"**[Question]({q.Link}) by [{q.Owner.DisplayName}]({q.Owner.Link})**\n" +
                    $"Answers: {q.AnswerCount}\n" +
                    $"Solved: {q.IsAnswered}\n" +
                    $"Score: {q.Score}");
            }

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("bing")]
        [Summary("Search Bing.")]
        public async Task Bing([Remainder]string search)
        {
            if (string.IsNullOrEmpty(bingSearchEndpoint) || string.IsNullOrEmpty(bingSearchKey))
            {
                await ReplyAsync("Bing hasn't been setup.");
                return;
            }
            
            string escapedSearch = Uri.EscapeDataString(search);
            string safeSearch = Context.Channel is ITextChannel text && text.IsNsfw ? "Off" : "Strict";

            HttpRequestMessage request = new HttpRequestMessage()
            {
                RequestUri = new Uri(bingSearchEndpoint + $"/search?count=5&q={escapedSearch}&safeSearch={safeSearch}"),
                Method = HttpMethod.Get,
                Headers =
                {
                    { "Ocp-Apim-Subscription-Key", bingSearchKey }
                }
            };

            BingSearch response = await _http.GetObjectAsync<BingSearch>(request, TimeSpan.FromMinutes(30));
            if (response.WebPages == null)
            {
                await ReplyAsync("I couldn't find anything.");
                return;
            }

            EmbedBuilder embedBuilder = new EmbedBuilder()
                .WithTitle($"For more results go to the Bing page.")
                .WithUrl(response.WebPages.WebSearchURL)
                .WithAuthor("Bing Results", "https://i.ibb.co/f94KyMB/bing.png")
                .WithFooter($"SafeSearch is set to {safeSearch} because your channel {(safeSearch == "Strict" ? "isn't" : "is")} NSFW.");

            foreach (WebPage q in response.WebPages.Value)
            {
                embedBuilder.Description += $"**[{q.Name}]({q.URL})**\n{q.Snippet}\n";
            }

            await ReplyAsync(embed: embedBuilder.Build());
        }

        [Command("whybing")]
        [Summary("Explains why I use bing instead of Google.")]
        public async Task WhyBing()
        {
            await ReplyAsync("Because us nonhumans need someone to pay for us to use a search engine interface that we can understand and Bing's free tier is better.");
        }
    }
}
