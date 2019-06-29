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

        private async Task GuildMemberUpdated(SocketGuildUser old, SocketGuildUser current)
        {
            if (old.Status == current.Status)
            {
                return;
            }

            if (current.Status == UserStatus.Online)
            {
                NotificationOpt[] opts = _db.NotificationOpts
                    .Where(n => n.OptedId == current.Id.ToString() && n.GuildId == current.Guild.Id.ToString() && n.Enabled)
                    .ToArray();
                foreach (NotificationOpt opt in opts)
                {
                    Embed embed = new EmbedBuilder()
                        .WithColor(Color.Green)
                        .WithTitle($"{current.Username} is now online.")
                        .Build();

                    SocketUser optedUser = _client.GetUser(ulong.Parse(opt.ReceiverId));
                    await optedUser.SendMessageAsync($"", false, embed);
                }
            }
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            NotificationOpt[] opts = _db.NotificationOpts
                .Where(n => n.OptedId == user.Id.ToString())
                .ToArray();

            foreach (NotificationOpt opt in opts)
            {
                Embed embed = new EmbedBuilder()
                    .WithColor(Color.Orange)
                    .WithTitle($"Uh oh..")
                    .WithDescription($"{user.Username} has left the guild that you opted for notifications in! " +
                    "You will no longer receive notifications when they come online. " +
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
