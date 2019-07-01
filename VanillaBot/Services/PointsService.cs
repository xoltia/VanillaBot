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

namespace VanillaBot.Services
{
    public class PointsService
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _config;
        private readonly VanillaContext _db;
        private readonly LoggingService _logger;

        private readonly int _tickAmount;

        public PointsService(IServiceProvider services)
        {
            _client = services.GetRequiredService<DiscordSocketClient>();
            _config = services.GetRequiredService<IConfiguration>();
            _logger = services.GetRequiredService<LoggingService>();
            _db = services.GetRequiredService<VanillaContext>();

            if (int.TryParse(_config["points:amount"], out int tickAmount))
            {
                _tickAmount = tickAmount;
            }
            else
            {
                _logger.Log(LogSeverity.Warning, "PointsService", "Invalid or missing tick amount setting from configuration, defaulting to 10 per tick.");
                _tickAmount = 10;
            }
        }

        public Task<Points> GetPoints(IUser user)
        {
            return _db.Points.FindAsync(user.Id.ToString());
        }

        public async Task AddPoints(SocketUser user, int amount)
        {
            Points points = _db.Points.Where(p => p.UserId == user.Id.ToString()).FirstOrDefault();
            if (points == null)
            {
                points = new Points()
                {
                    UserId = user.Id.ToString(),
                    Amount = _tickAmount
                };
                await _db.Points.AddAsync(points);
            }
            else
            {
                points.Amount += _tickAmount;
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
                    if (ticked.Contains(user.Id))
                        continue;

                    await AddPoints(user, _tickAmount);
                    ticked.Add(user.Id);
                }
            }
        }

        public async Task Initialize()
        {
            Timer timer;

            if (float.TryParse(_config["points:frequency"], out float pointFrequency))
            {
                await _logger.Log(LogSeverity.Info, "PointsService", $"Points will update by {_tickAmount} every {pointFrequency} minutes");
                timer = new Timer(pointFrequency * 60 * 1000);
            }
            else
            {
                await _logger.Log(LogSeverity.Warning, "PointsService", "Invalid or missing point frequency in config, defaulting to 30 minutes.");
                timer = new Timer(30 * 60 * 1000);
            }

            timer.Elapsed += TickPoints;
            timer.Enabled = true;
        }
    }
}
