using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace YukoBot.Services
{
    public class NotificationService
    {
        private readonly DiscordSocketClient _client;
        private readonly DbService _db;

        public NotificationService(DiscordSocketClient client, DbService dbService)
        {
            _client = client;
            _db = dbService;
        }

        private async Task GuildMemberUpdated(SocketGuildUser old, SocketGuildUser current)
        {
            if (old.Status != current.Status && current.Status == UserStatus.Online)
            {
                using (var uow = _db.GetDbContext())
                {
                    ulong[] peopleToNotify = await uow.Notifications.GetPeopleToNotifyAsync(current, current.Guild);
                    foreach (ulong id in peopleToNotify)
                    {
                        Embed embed = new EmbedBuilder()
                            .WithColor(Color.Green)
                            .WithTitle($"{current.Username} is now online.")
                            .WithCurrentTimestamp()
                            .Build();

                        Console.WriteLine(id);
                        SocketUser optedUser = _client.GetUser(id);
                        await optedUser.SendMessageAsync($"", false, embed);
                    }
                }
            }
            else if (current.Activity != null && old.Activity?.Name != current.Activity.Name)
            {
                using (var uow = _db.GetDbContext())
                {
                    ulong[] peopleToNotify = await uow.Notifications.GetPeopleToNotifyAsync(current, current.Guild);
                    foreach (ulong id in peopleToNotify)
                    {
                        if (await uow.GameNotifications.GetNotificationAsync(id.ToString(), current.Activity.Name) != null)
                        {
                            Embed embed = new EmbedBuilder()
                                .WithColor(0xffc0cb)
                                .WithTitle($"{current.Username} has started playing {current.Activity.Name}!")
                                .WithCurrentTimestamp()
                                .Build();

                            SocketUser optedUser = _client.GetUser(id);
                            await optedUser.SendMessageAsync(embed: embed);
                        }
                    }
                }
            }
        }

        private async Task UserLeft(SocketGuildUser user)
        {
            using (var uow = _db.GetDbContext())
            {
                ulong[] peopleToNotify = await uow.Notifications.GetPeopleToNotifyAsync(user, user.Guild);
                foreach (ulong id in peopleToNotify)
                {
                    Embed embed = new EmbedBuilder()
                        .WithColor(Color.Orange)
                        .WithTitle($"Uh oh..")
                        .WithDescription($"{user.Username} has left the guild that you opted for notifications in! " +
                        "You will no longer receive notifications when they come online or start playing a game of interest. " +
                        "To resolve this disable your current notification opt and create a new one in a mutual guild. ")
                        .Build();

                    await _client.GetUser(id).SendMessageAsync("", false, embed);
                }
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
