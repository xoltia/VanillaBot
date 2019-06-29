using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VanillaBot.Services;
using VanillaBot.Services.Database;

namespace VanillaBot
{
    public class VanillaBot
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private IConfiguration _config;

        public VanillaBot()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Ready += Ready;

            try
            {
                _config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("config.json")
                    .Build();
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Please provide a configuration file.");
                Environment.Exit(1);
            }

            _services =  new ServiceCollection()
                .AddDbContext<VanillaContext>()
                .AddSingleton(_client)
                .AddSingleton(_config)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<NotificationService>()
                .BuildServiceProvider();
        }

        public Task Log(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        public async Task Ready()
        {
            UserStatus status;
            ActivityType activityType;

            if (Enum.TryParse(_config["status"], out status))
            {
                await _client.SetStatusAsync(status);
            }

            await _client.SetGameAsync(_config["game:name"],
                string.IsNullOrEmpty(_config["game:url"]) ? null : _config["game:url"],
                Enum.TryParse(_config["game:type"], out activityType) ? activityType : ActivityType.Playing);
        }

        public async Task Start(string token)
        {
            await _services.GetRequiredService<CommandHandler>().Initialize();
            await _services.GetRequiredService<NotificationService>().Initialize();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        public async Task StartFromConfig()
        {
            await Start(_config["token"]);
        }
    }
}
