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

            [Command]
            [Summary("Receive notifications when someone you've opted to receive notifications about starts to play that game.")]
            [Priority(0)]
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
                await ReplyAsync($"You'll now receive notifications for {game}.");
            }

            [Command("remove")]
            [Summary("Stop receiving notifications about a game.")]
            [Priority(1)]
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
                await ReplyAsync($"You're all set! You'll no longer be notified when your friends play {game}.");
            }
        }
    }
}
