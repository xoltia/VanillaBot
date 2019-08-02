using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using YukoBot.Services.Database;
using Discord;
using YukoBot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace YukoBot.Services
{
    public class PointsService
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _config;
        private readonly YukoContext _db;
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
            _db = services.GetRequiredService<YukoContext>();

            _messageBonus = _config.GetConfigOption("points:messageBonus", 1, int.TryParse);
            _bonusReset = _config.GetConfigOption("points:bonusReset", 30f, float.TryParse);
            _maxBonus = _config.GetConfigOption("points:maxBonus", 10, int.TryParse);
        }

        public Task<Points> GetPoints(IUser user)
        {
            return _db.Points.FindAsync(user.Id.ToString());
        }

        public async Task AddPoints(IUser user, int amount) =>
            await AddPoints(user.Id.ToString(), amount);

        public async Task AddPoints(string userId, int amount)
        {
            Points points = await _db.Points.SingleOrDefaultAsync(p => p.UserId == userId);
            if (points == null)
            {
                points = new Points()
                {
                    UserId = userId,
                    Amount = amount
                };
                await _db.Points.AddAsync(points);
            }
            else
            {
                points.Amount += amount;
                _db.Points.Update(points);
            }

            await _db.SaveChangesAsync();
        }

        private async void ResetBonuses(object sender, ElapsedEventArgs e)
        {
            foreach (KeyValuePair<ulong, int> bonus in pointBonuses)
            {
                await AddPoints(bonus.Key.ToString(), bonus.Value);
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
