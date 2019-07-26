using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YukoBot.Services.Database;
using YukoBot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace YukoBot.Services
{
    public class NotificationService
    {
        private readonly DiscordSocketClient _client;
        private readonly YukoContext _db;

        public NotificationService(DiscordSocketClient client, YukoContext dbContext)
        {
            _client = client;
            _db = dbContext;
        }

        private async Task GuildMemberUpdated(SocketGuildUser old, SocketGuildUser current)
        {
            if (old.Status != current.Status && current.Status == UserStatus.Online)
            {
                Notification[] opts = await _db.Notifications
                    .Where(n => n.OptedId == current.Id.ToString() && n.GuildId == current.Guild.Id.ToString() && n.Enabled)
                    .ToArrayAsync();

                foreach (Notification opt in opts)
                {
                    Embed embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithTitle($"{current.Username} is now online.")
                        .WithCurrentTimestamp()
                        .Build();

                    SocketUser optedUser = _client.GetUser(ulong.Parse(opt.ReceiverId));
                    await optedUser.SendMessageAsync($"", false, embed);
                }
            }
            else if (current.Activity != null && old.Activity?.Name != current.Activity.Name)
            {
                Notification[] opts = await _db.Notifications
                    .Where(n => n.OptedId == current.Id.ToString() && n.GuildId == current.Guild.Id.ToString() && n.Enabled)
                    .ToArrayAsync();

                foreach (Notification opt in opts)
                {
                    if (await _db.GameNotifications.SingleOrDefaultAsync(g => g.ReceiverId == opt.ReceiverId && g.Game == current.Activity.Name) != null)
                    {
                        Embed embed = new EmbedBuilder()
                            .WithColor(0xffc0cb)
                            .WithTitle($"{current.Username} has started playing {current.Activity.Name}!")
                            .WithCurrentTimestamp()
                            .Build();

                        SocketUser optedUser = _client.GetUser(ulong.Parse(opt.ReceiverId));
                        await optedUser.SendMessageAsync(embed: embed);
                    }
                }
            }
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            Notification[] opts = await _db.Notifications
                .Where(n => n.OptedId == user.Id.ToString() && n.GuildId == user.Guild.Id.ToString())
                .ToArrayAsync();

            foreach (Notification opt in opts)
            {
                Embed embed = new EmbedBuilder()
                    .WithColor(Color.Orange)
                    .WithTitle($"Uh oh..")
                    .WithDescription($"{user.Username} has left the guild that you opted for notifications in! " +
                    "You will no longer receive notifications when they come online or start playing a game of interest. " +
                    "To resolve this disable your current notification opt and create a new one in a mutual guild. ")
                    .Build();

                await _client.GetUser(ulong.Parse(opt.ReceiverId)).SendMessageAsync("", false, embed);
            }
        }

        public Task Initialize()
        {
            _client.GuildMemberUpdated += GuildMemberUpdated;
            _client.UserLeft += UserLeft;

            return Task.CompletedTask;
        }
    }
}
