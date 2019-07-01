using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Modules
{
    [Name("moderation")]
    public class ModerationCommands : ModuleBase<SocketCommandContext>
    {
        // TODO: add reasons

        [Command("ban")]
        [Alias("banish", "removeof", "destroy")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have permission to do that!")]
        [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "I don't have permission to kick people.")]
        public async Task Ban(IGuildUser member)
        {
            if (Context.Guild.Owner.Id == member.Id)
            {
                await ReplyAsync("You can't ban the guild owner!");
                return;
            }

            await member.BanAsync();
            await ReplyAsync("I didn't like that guy anways.");
        }

        [Command("kick")]
        [Alias("showout")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have permission to do that!")]
        [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "I don't have permission to ban people.")]
        public async Task Kick(IGuildUser member)
        {
            if (Context.Guild.Owner.Id == member.Id)
            {
                await ReplyAsync("You can't kick the guild owner!");
                return;
            }

            await member.BanAsync();
            await ReplyAsync($"Later bud {member.Mention}");
        }
    }
}
