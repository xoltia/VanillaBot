using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using YukoBot.Services;
using YukoBot.Services.Database;
using YukoBot.Services.Database.Models;

namespace YukoBot.Modules
{
    [Name("Notifications")]
    [Group("notify")]
    public class NotificationCommands : ModuleBase<SocketCommandContext>
    {
        private readonly DbService _db;

        public NotificationCommands(DbService dbService)
        {
            _db = dbService;
        }

        [Command]
        [Summary("Start receiving notifications for a certain user.")]
        [RequireContext(ContextType.Guild)]
        public async Task NewNotification(IUser user)
        {
            string opter = Context.Message.Author.Id.ToString();
            string opted = user.Id.ToString();

            using (var uow = _db.GetDbContext())
            {
                if (await uow.Notifications.GetNotificationAsync(opter, opted) != null)
                {
                    await ReplyAsync($"You've already opted to receive notifications for {user.Username}, silly!");
                    return;
                }

                await uow.Notifications.AddAsync(new Notification()
                {
                    ReceiverId = opter,
                    OptedId = opted,
                    GuildId = Context.Guild.Id.ToString()
                });
                await uow.SaveChangesAsync();
                await ReplyAsync($"You'll now receive notifications about {user.Username}!");
            }
        }

        [Command("disable")]
        [Summary("Disable notifications for a specific user.")]
        public async Task DisableNotification(IUser user)
        {
            string opter = Context.User.Id.ToString();
            string opted = user.Id.ToString();

            using (var uow = _db.GetDbContext())
            {
                UpdateResult result = await uow.Notifications.TrySetNotificationEnabledAsync(opter, opted, false);
                if (result == UpdateResult.DoesNotExist)
                {
                    await ReplyAsync("Nothing to disable here. You aren't receiving notifications about them, silly!");
                    return;
                }

                if (result == UpdateResult.NoChangesMade)
                {
                    await ReplyAsync("Nothing to disable here. I've already marked this as disabled.");
                    return;
                }

                await uow.SaveChangesAsync();
                await ReplyAsync($"I'll remember not to send you notifications about {user.Username} until you tell me otherwise.");
            }
        }

        [Command("enable")]
        [Summary("Enable notifications for a specific user.")]
        public async Task EnableNotification(IUser user)
        {
            string opter = Context.User.Id.ToString();
            string opted = user.Id.ToString();

            using (var uow = _db.GetDbContext())
            {
                UpdateResult result = await uow.Notifications.TrySetNotificationEnabledAsync(opter, opted, true);
                if (result == UpdateResult.DoesNotExist)
                {
                    await ReplyAsync("Nothing to enable here. You're not even receiving notifications about them, silly!");
                    return;
                }

                if (result == UpdateResult.NoChangesMade)
                {
                    await ReplyAsync("Nothing to enable here. Your notification is already enabled.");
                    return;
                }

                await uow.SaveChangesAsync();
                await ReplyAsync($"I'll remember to notify you about {user.Username}.");
            }
        }

        [Command("list")]
        [Summary("List every person you're receiving notifications for.")]
        public async Task ListNotifications()
        {
            var uow = _db.GetDbContext();
            string opter = Context.User.Id.ToString();
            Notification[] opts = await uow.Notifications.GetNotificationsAsync(opter, false);
            uow.Dispose();

            EmbedBuilder embed = new EmbedBuilder().WithColor(0xffc0cb);

            if (opts.Length == 0)
            {
                await ReplyAsync(embed: embed.WithTitle("You do not have any user notifications set.").Build());
                return;
            }

            embed.Description = "These are the people I will notify you about when they play a game you have specified or come online.";

            foreach (Notification opt in opts)
            {
                embed.AddField(f => {
                    f.Name = Context.Client.GetUser(ulong.Parse(opt.OptedId)).Username;
                    f.Value = opt.Enabled ? "Enabled" : "Disabled";
                    f.IsInline = true;
                });
            }

            await ReplyAsync(embed: embed.Build());
        }

        [Command("remove")]
        [Summary("Stop receiving notifications for a specific user.")]
        public async Task RemoveNotification(IUser user)
        {
            string opter = Context.Message.Author.Id.ToString();
            string opted = user.Id.ToString();

            using (var uow = _db.GetDbContext())
            {
                Notification opt = await uow.Notifications.GetNotificationAsync(opter, opted);

                if (opt == null)
                {
                    await ReplyAsync($"You're not opted to receive notifications about {user.Username}.");
                    return;
                }

                uow.Notifications.Remove(opt);
                await uow.SaveChangesAsync();
                await ReplyAsync($"You'll no longer receive notifications about {user.Username}.");
            }
        }

        [Group("game")]
        public class GameNotificationCommands : ModuleBase<SocketCommandContext>
        {
            private readonly DbService _db;

            public GameNotificationCommands(DbService dbService)
            {
                _db = dbService;
            }

            [Command, Priority(0)]
            [Summary("Receive notifications when someone you've opted to receive notifications about starts to play that game.")]
            public async Task NewGameNotification([Remainder]string game)
            {
                string opter = Context.User.Id.ToString();

                using (var uow = _db.GetDbContext())
                {
                    if (await uow.GameNotifications.GetNotificationAsync(opter, game) != null)
                    {
                        await ReplyAsync("You're already getting notifications for this game.");
                        return;
                    }

                    await uow.GameNotifications.AddAsync(new GameNotification()
                    {
                        ReceiverId = opter,
                        Game = game
                    });
                    await uow.SaveChangesAsync();
                    await ReplyAsync($"I'll now notify you when your friends start to play {game} without you.");
                }
            }

            [Command("remove"), Priority(1)]
            [Summary("Stop receiving notifications about a game.")]
            public async Task RemoveGameNotification([Remainder]string game)
            {
                string opter = Context.User.Id.ToString();
                using (var uow = _db.GetDbContext())
                {
                    GameNotification notification = await uow.GameNotifications.GetNotificationAsync(opter, game);

                    if (notification == null)
                    {
                        await ReplyAsync("You're already not getting notifications for that game!");
                        return;
                    }

                    uow.GameNotifications.Remove(notification);
                    await uow.SaveChangesAsync();
                    await ReplyAsync($"I'll no longer notify you when your friends play {game}.");
                }
            }

            [Command("list"), Priority(1)]
            [Summary("List every game you're receiving notifications about.")]
            public async Task ListGameNotificatoins()
            {
                string opter = Context.User.Id.ToString();
                var uow = _db.GetDbContext();
                GameNotification[] opts = await uow.GameNotifications.GetNotificationsAsync(opter);
                uow.Dispose();

                EmbedBuilder embed = new EmbedBuilder().WithColor(0xffc0cb);

                if (opts.Length == 0)
                {
                    await ReplyAsync(embed: embed.WithTitle("You do not have any game notifications set.").Build());
                    return;
                }

                embed.Description = "When someone you've opted to receive notifications for plays one of these games I'll tell you.\n";

                for (int i = 0; i < opts.Length; i++)
                    embed.Description += $"\n**{i+1}. {opts[i].Game}**";

                await ReplyAsync(embed: embed.Build());
            }
        }
    }
}
