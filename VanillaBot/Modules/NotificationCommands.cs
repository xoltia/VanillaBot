using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaBot.Services.Database;
using VanillaBot.Services.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace VanillaBot.Modules
{
    [Group("notify")]
    public class NotificationCommands : ModuleBase<SocketCommandContext>
    {
        private readonly VanillaContext _db;
        public NotificationCommands(VanillaContext dbContext)
        {
            _db = dbContext;
        }

        [Command]
        [Summary("Start receiving notifications for a certain user.")]
        public async Task NewNotification(IUser user)
        {
            string opter = Context.Message.Author.Id.ToString();
            string opted = user.Id.ToString();

            if (await _db.Notifications.SingleOrDefaultAsync(n => n.ReceiverId == opter && n.OptedId == opted) != null)
            {
                await ReplyAsync($"You've already opted to receive notifications for {user.Username}, silly!");
                return;
            }

            await _db.Notifications.AddAsync(new Notification()
            {
                ReceiverId = opter,
                OptedId = opted,
                GuildId = Context.Guild.Id.ToString()
            });
            await _db.SaveChangesAsync();
            await ReplyAsync($"You'll now receive notifications about {user.Username}!");
        }

        [Command("disable")]
        [Summary("Disable notifications for a specific user.")]
        public async Task DisableNotification(IUser user)
        {
            string opter = Context.User.Id.ToString();
            string opted = user.Id.ToString();

            Notification opt = await _db.Notifications.SingleOrDefaultAsync(n => n.OptedId == opted && n.ReceiverId == opter);
            if (opt == null)
            {
                await ReplyAsync("Nothing to disable here. You aren't receiving notifications about them, silly!");
                return;
            }

            if (!opt.Enabled)
            {
                await ReplyAsync("Nothing to disable here. I've already marked this as disabled.");
                return;
            }

            opt.Enabled = false;
            await _db.SaveChangesAsync();
            await ReplyAsync($"I'll remember not to send you notifications about {user.Username} until you tell me otherwise.");
        }

        [Command("enable")]
        [Summary("Enable notifications for a specific user.")]
        public async Task EnableNotification(IUser user)
        {
            string opter = Context.User.Id.ToString();
            string opted = user.Id.ToString();

            Notification opt = await _db.Notifications.SingleOrDefaultAsync(n => n.OptedId == opted && n.ReceiverId == opter);
            if (opt == null)
            {
                await ReplyAsync("I can't enable a notification that doesn't exist.");
                return;
            }

            if (opt.Enabled)
            {
                await ReplyAsync("Nothing to enable here. It wasn't disabled in the first place.");
                return;
            }

            opt.Enabled = true;
            await _db.SaveChangesAsync();
            await ReplyAsync($"I'll start notifying you about {user.Username} again.");
        }

        [Command("list")]
        [Summary("List every person you're receiving notifications for.")]
        public async Task ListNotifications()
        {
            string opter = Context.User.Id.ToString();
            Notification[] opts = await _db.Notifications.Where(n => n.ReceiverId == opter).ToArrayAsync();

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

            Notification opt = await _db.Notifications.SingleOrDefaultAsync(n => n.ReceiverId == opter && n.OptedId == opted);

            if (opt == null)
            {
                await ReplyAsync($"You're not opted to receive notifications about {user.Username}.");
                return;
            }

            _db.Notifications.Remove(opt);
            await _db.SaveChangesAsync();
            await ReplyAsync($"You'll no longer receive notifications about {user.Username}.");
        }

        [Group("game")]
        public class GameNotificationCommands : ModuleBase<SocketCommandContext>
        {
            private readonly VanillaContext _db;

            public GameNotificationCommands(VanillaContext dbContext)
            {
                _db = dbContext;
            }

            [Command, Priority(0)]
            [Summary("Receive notifications when someone you've opted to receive notifications about starts to play that game.")]
            public async Task NewGameNotification([Remainder]string game)
            {
                string opter = Context.User.Id.ToString();

                if (await _db.GameNotifications.SingleOrDefaultAsync(g => g.ReceiverId == opter && g.Game == game) != null)
                {
                    await ReplyAsync("You're already getting notifications for this game.");
                    return;
                }

                await _db.GameNotifications.AddAsync(new GameNotification()
                {
                    ReceiverId = opter,
                    Game = game
                });
                await _db.SaveChangesAsync();
                await ReplyAsync($"I'll now notify you when your friends start to play {game} without you.");
            }

            [Command("remove"), Priority(1)]
            [Summary("Stop receiving notifications about a game.")]
            public async Task RemoveGameNotification([Remainder]string game)
            {
                string opter = Context.User.Id.ToString();
                GameNotification notification = await _db.GameNotifications.SingleOrDefaultAsync(g => g.ReceiverId == opter && g.Game == game);

                if (notification == null)
                {
                    await ReplyAsync("You're already not getting notifications for that game!");
                    return;
                }

                _db.GameNotifications.Remove(notification);
                await _db.SaveChangesAsync();
                await ReplyAsync($"I'll no longer notify you when your friends play {game}.");
            }

            [Command("list"), Priority(1)]
            [Summary("List every game you're receiving notifications about.")]
            public async Task ListGameNotificatoins()
            {
                string opter = Context.User.Id.ToString();
                GameNotification[] opts = await _db.GameNotifications.Where(n => n.ReceiverId == opter).ToArrayAsync();

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
