using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VanillaBot.Services.Database;
using Discord;
using VanillaBot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace VanillaBot.Services
{
    public class PointsService
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigService _config;
        private readonly VanillaContext _db;
        private readonly LoggingService _logger;

        private readonly int _tickAmount;
        private readonly int _messageBonus;
        private readonly int _maxBonus;

        private readonly Dictionary<ulong, int> pointBonuses = new Dictionary<ulong, int>();

        public PointsService(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _config = services.GetRequiredService<ConfigService>();
            _logger = services.GetRequiredService<LoggingService>();
            _db = services.GetRequiredService<VanillaContext>();

            _tickAmount = _config.GetConfigOption("points:amount", 10, int.TryParse);
            _messageBonus = _config.GetConfigOption("points:messageBonus", 1, int.TryParse);
            _maxBonus = _config.GetConfigOption("points:maxBonus", 10, int.TryParse);
        }

        public Task<Points> GetPoints(IUser user)
        {
            return _db.Points.FindAsync(user.Id.ToString());
        }

        public async Task AddPoints(SocketUser user, int amount)
        {
            Points points = await _db.Points.SingleOrDefaultAsync(p => p.UserId == user.Id.ToString());
            if (points == null)
            {
                points = new Points()
                {
                    UserId = user.Id.ToString(),
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

        private async void TickPoints(object sender, ElapsedEventArgs e)
        {
            await _logger.Log(LogSeverity.Info, "PointsService", "Updated member points");

            List<ulong> ticked = new List<ulong>();
            foreach (SocketGuild guild in _client.Guilds)
            {
                foreach (SocketUser user in guild.Users)
                {
                    if (ticked.Contains(user.Id) || user.IsBot)
                        continue;

                    await AddPoints(user, pointBonuses.ContainsKey(user.Id) ? _tickAmount + pointBonuses[user.Id] :_tickAmount);
                    ticked.Add(user.Id);
                }
            }
        }

        public async Task Initialize()
        {
            float pointFrequency = _config.GetConfigOption("points:frequency", 30f, float.TryParse);
            await _logger.Log(LogSeverity.Info, "PointsService", $"Points will update by {_tickAmount} every {pointFrequency} minutes");
            Timer timer = new Timer(pointFrequency * 60 * 1000);

            timer.Elapsed += TickPoints;
            timer.Enabled = true;

            _client.MessageReceived += MessageReceived;
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
