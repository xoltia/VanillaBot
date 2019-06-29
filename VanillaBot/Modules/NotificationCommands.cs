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
    [Group("notifications")]
    public class NotificationCommands : ModuleBase<SocketCommandContext>
    {
        private readonly VanillaContext _db;
        public NotificationCommands(VanillaContext dbContext)
        {
            _db = dbContext;
        }

        [Command("new")]
        public async Task NewNotification(IUser user)
        {
            string opter = Context.Message.Author.Id.ToString();
            string opted = user.Id.ToString();

            if (_db.NotificationOpts
                .Where(n => n.ReceiverId == opter && n.OptedId == opted)
                .FirstOrDefault() != null)
            {
                await ReplyAsync($"You've already opted to receive notifications regarding {user.Username}.");
                return;
            }

            await _db.NotificationOpts.AddAsync(new NotificationOpt()
            {
                ReceiverId = opter,
                OptedId = opted,
                GuildId = Context.Guild.Id.ToString()
            });
            await _db.SaveChangesAsync();
            await ReplyAsync($"You will now receive notifications regarding {user.Username}.");
        }

        [Command("remove")]
        public async Task RemoveNotification(IUser user)
        {
            string opter = Context.Message.Author.Id.ToString();
            string opted = user.Id.ToString();

            NotificationOpt opt = _db.NotificationOpts
                .Where(n => n.ReceiverId == opter && n.OptedId == opted)
                .FirstOrDefault();

            if (opt == null)
            {
                await ReplyAsync($"You're not opted to receive notifications about {user.Username}.");
                return;
            }

            _db.NotificationOpts.Remove(opt);
            await _db.SaveChangesAsync();
            await ReplyAsync($"You'll no long receive notifications about {user.Username}.");
        }
    }
}
