using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VanillaBot.Modules.General;
using VanillaBot.Services;

namespace VanillaBot.Modules
{
    [Name("general")]
    public class GeneralCommands : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _client;
        private readonly LoggingService _logger;
        private readonly HttpService _http;
        private readonly CommandHandler _commands;

        public GeneralCommands(IServiceProvider services)
        {
            _commandService = services.GetRequiredService<CommandService>();
            _config = services.GetRequiredService<IConfiguration>();
            _client = services.GetRequiredService<DiscordSocketClient>();
            _logger = services.GetRequiredService<LoggingService>();
            _commands = services.GetRequiredService<CommandHandler>();
            _http = services.GetRequiredService<HttpService>();
        }

        [Command("ping"), Summary("Must do something..")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!");
        }

        [Command("help"), Summary("Shows list of commands.")]
        public async Task Help()
        {
            string prefix = _config["prefix"];
            EmbedBuilder builder = new EmbedBuilder()
                .WithColor(0xffc0cb)
                .WithTitle("These are the things I can do.")
                .WithDescription($"Type {prefix}help <command> for details on a specific one!")
                .WithThumbnailUrl(_client.CurrentUser.GetAvatarUrl());

            foreach (ModuleInfo module in _commandService.Modules)
            {
                string description = string.Empty;
                string fieldName = module.Parent?.Name ?? module.Name;
                foreach (var cmd in module.Commands)
                {
                    var result = await cmd.CheckPreconditionsAsync(Context);
                    if (result.IsSuccess)
                        description += $"{prefix}{cmd.Aliases.First()} {(cmd.Parameters.Count > 0 ? "<":"")}{string.Join(" <", cmd.Parameters.Select(p => p.Name + ">"))}\n";
                }

                // Module is empty, probably contains submodules rather than commands
                // Builder will error if value is null or empty
                if (string.IsNullOrEmpty(description))
                {
                    continue;
                }

                // So submodules are in same field as parent
                if (builder.Fields.Exists(f => f.Name == fieldName))
                {
                    builder.Fields.Find(f => f.Name == fieldName).Value += description;
                }
                else
                {
                    builder.AddField(f =>
                    {
                        f.Name = fieldName;
                        f.Value = description;
                        f.IsInline = true;
                    });
                }
            }

            // Fields with least commands goes to the bottom because it looks nasty otherwise
            builder.Fields.Sort((EmbedFieldBuilder f1, EmbedFieldBuilder f2) =>
            {
                return f2.Value.ToString().Length - f1.Value.ToString().Length;
            });

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help"), Summary("I think you've figured it out.")]
        public async Task Help([Remainder]string command)
        {
            SearchResult result = _commandService.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find anything.");
                return;
            }

            string prefix = _config["prefix"];
            var builder = new EmbedBuilder()
                .WithColor(0xffc0cb)
                .WithDescription($"Here's some info you may find of interest.");

            foreach (CommandMatch match in result.Commands)
            {
                CommandInfo cmd = match.Command;
                builder.AddField(f =>
                {
                    f.Name = string.Join(", ", cmd.Aliases);
                    f.Value = $"Parameters: {(cmd.Parameters.Count > 0 ? string.Join(", ", cmd.Parameters.Select(p => p.Name)) : "None")}\n" +
                              $"Summary: {cmd.Summary ?? "None"}";
                    f.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("info"), Alias("information")]
        [Summary("Get information about the bot's environment, current stats, and repository.")]
        public async Task Stats()
        {
            // TODO: cache for a certain amount of time since it probably won't change too often
            GithubCommit lastCommit = (await _http.GetObjectAsync<List<GithubCommit>>("https://api.github.com/repos/xoltia/VanillaBot/commits"))[0];
            List<CommitActivity> commitActivity = await _http.GetObjectAsync<List<CommitActivity>>("https://api.github.com/repos/xoltia/VanillaBot/stats/commit_activity");

            Embed embed = new EmbedBuilder()
                .WithTitle("**Bot Information**")
                .WithDescription($"**Stats**\n" +
                $"Duration: {DateTime.Now - Process.GetCurrentProcess().StartTime}\n" +
                $"Commands executed: {_commands.CommandsExecuted}\n" +
                $"Guilds: {Context.Client.Guilds.Count}\n" +
                $"\n**Environment**\n" +
                $"Discord.NET version: {FileVersionInfo.GetVersionInfo(Path.GetFullPath("Discord.Net.Core.dll")).FileVersion}\n." +
                $"NET Version: {Environment.Version}\n" +
                $"Host OS: {Environment.OSVersion} ({(Environment.Is64BitOperatingSystem ? 64 : 32)} bit)\n" +
                $"Host processor count: {Environment.ProcessorCount}\n" +
                $"\n**Repository**\n" +
                $"Last commit: {lastCommit.Commit.Message}\n" +
                $"Commits this week: {commitActivity[51].Total}\n" +
                $"Commits this year: {commitActivity.Sum(c => c.Total)}")
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}
