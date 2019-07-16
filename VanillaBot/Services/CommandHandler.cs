using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace VanillaBot.Services
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly LoggingService _logger;
        private readonly IServiceProvider _services;

        private readonly string _prefix;

        public UInt64 CommandsExecuted { get; private set; } = 0;

        public CommandHandler(IServiceProvider services)
        {
            
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetRequiredService<LoggingService>();
            _services = services;

            _client.MessageReceived += MessageReceivedAsync;
            _commands.CommandExecuted += CommandExecutedAsync;

            IConfiguration conf = services.GetRequiredService<IConfiguration>();
            _prefix = conf["prefix"];
        }

        private async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // Shouldn't happen but just in case search fails for some reason.
            if (!command.IsSpecified)
            {
                // Don't show error message
                return;
            }

            if (!result.IsSuccess)
            {
                EmbedBuilder embedBuilder = new EmbedBuilder();

                if (result.Error >= CommandError.Exception)
                {
                    // Stuff user doesn't need to know about
                    await _logger.Log(LogSeverity.Error, "CommandHandler", $"Error during execution of {command.GetValueOrDefault()?.Name} command: {result.ErrorReason}");

                    embedBuilder.Title = "An unexpected error occurred executing your command.";
                    embedBuilder.Color = Color.Orange;
                }
                else
                {
                    embedBuilder.Title = result.ErrorReason;
                    embedBuilder.Color = Color.Red;
                }

                await context.Channel.SendMessageAsync("", false, embedBuilder.Build());
            }

            CommandsExecuted++;
        }

        private async Task MessageReceivedAsync(SocketMessage socketMessage)
        {
            int argPos = 0;

            if (!(socketMessage is SocketUserMessage message) || message.Source != MessageSource.User ||
               (!message.HasMentionPrefix(_client.CurrentUser, ref argPos) && !message.HasStringPrefix(_prefix, ref argPos)))
            {
                return;
            }

            SocketCommandContext context = new SocketCommandContext(_client, message);
            SearchResult result = _commands.Search(context, argPos);
            if (result.IsSuccess)
            {
                using (IDisposable typing = message.Channel.EnterTypingState())
                {
                    await _commands.ExecuteAsync(context, argPos, _services);
                }
            }
        }

        public async Task Initialize()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }
    }
}
