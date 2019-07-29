using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using YukoBot.Services;
using YukoBot.Services.Database;

namespace YukoBot.Core
{
    public class YukoBot
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _services;
        private IConfiguration _config;

        public YukoBot()
        {
            _client = new DiscordSocketClient();
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

            _services = new ServiceCollection()
                .AddDbContext<YukoContext>(ServiceLifetime.Transient)
                .AddSingleton(_client)
                .AddSingleton(_config)
                .AddSingleton(new CommandService(new CommandServiceConfig() {
                    DefaultRunMode = RunMode.Async
                }))
                .AddSingleton<ConfigService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<NotificationService>()
                .AddSingleton<LoggingService>()
                .AddTransient<PointsService>()
                .AddSingleton<Random>()
                .AddSingleton<HttpClient>()
                .AddSingleton<HttpService>()
                .AddSingleton<GuildConfigService>()
                .BuildServiceProvider();
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

            await _services.GetRequiredService<LoggingService>().Info("YukoBot", $"In {_client.Guilds.Count} servers.");
        }

        public async Task Start(string token)
        {
            await InitServices();
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        public async Task StartFromConfig()
        {
            await Start(_config["token"]);
        }

        private async Task InitServices()
        {
            await _services.GetRequiredService<CommandHandler>().Initialize();
            await _services.GetRequiredService<NotificationService>().Initialize();
            await _services.GetRequiredService<LoggingService>().Initialize();
            await _services.GetRequiredService<PointsService>().Initialize();
        }
    }
}
