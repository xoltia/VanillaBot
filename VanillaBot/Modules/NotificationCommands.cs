using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaBot.Services.Database;
using VanillaBot.Services.Database.Models;

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

            if (_db.Notifications
                .Where(n => n.ReceiverId == opter && n.OptedId == opted)
                .FirstOrDefault() != null)
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

            Notification opt = _db.Notifications
                .Where(n => n.ReceiverId == opter && n.OptedId == opted)
                .FirstOrDefault();

            if (opt == null)
            {
                await ReplyAsync($"You're not opted to receive notifications about {user.Username}.");
                return;
            }

            _db.Notifications.Remove(opt);
            await _db.SaveChangesAsync();
            await ReplyAsync($"You'll no longer receive notifications about {user.Username}.");
        }
    }
}
