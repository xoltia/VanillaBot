using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaBot.Services.Database;
using VanillaBot.Services.Database.Models;

namespace VanillaBot.Services
{
    public class NotificationService
    {
        private readonly DiscordSocketClient _client;
        private readonly VanillaContext _db;

        public NotificationService(DiscordSocketClient client, VanillaContext dbContext)
        {
            _client = client;
            _db = dbContext;
        }

        // TODO: add commands to enable/disable all or single notifications
        // TODO: add command to remove game notification

        private async Task GuildMemberUpdated(SocketGuildUser old, SocketGuildUser current)
        {
            // TODO: use AsyncEnumerable

            if (old.Status != current.Status && current.Status == UserStatus.Online)
            {
                Notification[] opts = _db.Notifications
                    .Where(n => n.OptedId == current.Id.ToString() && n.GuildId == current.Guild.Id.ToString() && n.Enabled)
                    .ToArray();
                foreach (Notification opt in opts)
                {
                    Embed embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithTitle($"{current.Username} is now online.")
                        .Build();

                    SocketUser optedUser = _client.GetUser(ulong.Parse(opt.ReceiverId));
                    await optedUser.SendMessageAsync($"", false, embed);
                }
            }
            else if (current.Activity != null && old.Activity?.Name != current.Activity.Name)
            {
                Notification[] opts = _db.Notifications
                    .Where(n => n.OptedId == current.Id.ToString() && n.GuildId == current.Guild.Id.ToString() && n.Enabled)
                    .ToArray();

                foreach (Notification opt in opts)
                {
                    
                    if (_db.GameNotifications
                        .Where(g => g.ReceiverId == opt.ReceiverId && g.Game == current.Activity.Name)
                        .FirstOrDefault() != null)
                    {
                        Embed embed = new EmbedBuilder()
                            .WithColor(0xffc0cb)
                            .WithTitle($"{current.Username} has started playing {current.Activity.Name}!")
                            .Build();

                        SocketUser optedUser = _client.GetUser(ulong.Parse(opt.ReceiverId));
                        await optedUser.SendMessageAsync(embed: embed);
                    }
                }
            }
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            Notification[] opts = _db.Notifications
                .Where(n => n.OptedId == user.Id.ToString())
                .ToArray();

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
