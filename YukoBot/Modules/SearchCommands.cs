using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public SearchCommands(HttpService http)
        {
            _http = http;
        }

        [Command("StackOverflow"), Alias("so")]
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
    }
}
