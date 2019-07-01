using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Modules
{
    [Name("basics")]
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _client;

        public BasicCommands(CommandService commandService, IConfiguration config, DiscordSocketClient client)
        {
            _commandService = commandService;
            _config = config;
            _client = client;
        }

        [Command("ping")]
        [Summary("Must do something..")]
        public async Task Ping()
        {
            await ReplyAsync("Pong!");
        }

        [Command("help")]
        [Summary("Shows list of commands.")]
        public async Task Help()
        {
            string prefix = _config["prefix"];
            EmbedBuilder builder = new EmbedBuilder()
                .WithColor(0xffc0cb)
                .WithTitle("These are the things I can do.")
                .WithDescription($"Type {prefix}help <command> for details on a specific one!\n" +
                $"Wrap command in quotes if it is a submodule")
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

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        [Summary("I think you've figured it out.")]
        public async Task Help(string command)
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
    }
}
