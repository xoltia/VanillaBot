using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;

namespace YukoBot.Services
{
    public class PointsService
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _config;
        private readonly DbService _db;
        private readonly LoggingService _logger;

        private readonly float _bonusReset;
        private readonly int _messageBonus;
        private readonly int _maxBonus;

        private readonly ConcurrentDictionary<ulong, int> pointBonuses = new ConcurrentDictionary<ulong, int>();

        public PointsService(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _config = services.GetRequiredService<ConfigService>();
            _logger = services.GetRequiredService<LoggingService>();
            _db = services.GetRequiredService<DbService>();

            _messageBonus = _config.GetConfigOption("points:messageBonus", 1, int.TryParse);
            _bonusReset = _config.GetConfigOption("points:bonusReset", 30f, float.TryParse);
            _maxBonus = _config.GetConfigOption("points:maxBonus", 10, int.TryParse);
        }

        private async void ResetBonuses(object sender, ElapsedEventArgs e)
        {
            using (var uow = _db.GetDbContext())
            {
                foreach (KeyValuePair<ulong, int> bonus in pointBonuses)
                {
                    await uow.Points.AddPointsAsync(bonus.Key.ToString(), bonus.Value);
                }
                await uow.SaveChangesAsync();
            }

            pointBonuses.Clear();
            await _logger.Info("PointsService", "Reset bonuses");
        }

        public Task Initialize()
        {
            Timer timer = new Timer(_bonusReset * 60 * 1000);

            timer.Elapsed += ResetBonuses;
            timer.Enabled = true;

            _client.MessageReceived += MessageReceived;

            return Task.CompletedTask;
        }

        private Task MessageReceived(SocketMessage message)
        {
            if (message.Author.IsBot)
                return Task.CompletedTask;

            if (pointBonuses.ContainsKey(message.Author.Id))
            {
                if (pointBonuses[message.Author.Id] == _maxBonus)
                    return Task.CompletedTask;
                pointBonuses[message.Author.Id] += _messageBonus;
            }
            else
            {
                pointBonuses[message.Author.Id] = _messageBonus;
            }

            return Task.CompletedTask;
        }
    }
}
