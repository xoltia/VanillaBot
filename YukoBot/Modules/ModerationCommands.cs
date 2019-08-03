using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using YukoBot.Services;

namespace YukoBot.Modules
{
    [Name("Moderation")]
    public class ModerationCommands : ModuleBase<SocketCommandContext>
    {
        // TODO: add per guild configuration or at least bot configuration
        private const string muteRoleName = "yuko-muted";

        private readonly GuildConfigService _guildConfig;
        public ModerationCommands(GuildConfigService guildConfig)
        {
            _guildConfig = guildConfig;
        }

        [Group("ban"), Alias("banish", "removeof", "destroy")]
        [RequireUserPermission(GuildPermission.BanMembers, ErrorMessage = "You don't have permission to do that!")]
        [RequireBotPermission(GuildPermission.BanMembers, ErrorMessage = "I don't have permission to ban people.")]
        public class BanCommands : ModuleBase<SocketCommandContext>
        {
            [Command]
            public async Task Ban(IGuildUser member)
            {
                if (Context.Guild.Owner.Id == member.Id)
                {
                    await ReplyAsync("You can't ban the guild owner!");
                    return;
                }

                await member.BanAsync();
                await ReplyAsync("I didn't like that guy anyways.");
            }
            
            [Command]
            public async Task Ban(IGuildUser member, [Remainder]string reason)
            {
                if (Context.Guild.Owner.Id == member.Id)
                {
                    await ReplyAsync("You can't ban the guild owner!");
                    return;
                }

                await member.BanAsync(reason: reason + $" (banned by {Context.User.Username} using YukoBot)");
                await ReplyAsync("I didn't like that guy anyways.");
            }
        }

        [Group("kick"), Alias("showout")]
        [RequireUserPermission(GuildPermission.KickMembers, ErrorMessage = "You don't have permission to do that!")]
        [RequireBotPermission(GuildPermission.KickMembers, ErrorMessage = "I don't have permission to kick people.")]
        public class KickCommands : ModuleBase<SocketCommandContext>
        {
            [Command]
            public async Task Kick(IGuildUser member)
            {
                if (Context.Guild.Owner.Id == member.Id)
                {
                    await ReplyAsync("You can't kick the guild owner!");
                    return;
                }

                await member.KickAsync();
                await ReplyAsync($"Later bud {member.Mention}");
            }

            [Command]
            public async Task Kick(IGuildUser member, [Remainder]string reason)
            {
                if (Context.Guild.Owner.Id == member.Id)
                {
                    await ReplyAsync("You can't kick the guild owner!");
                    return;
                }

                await member.KickAsync(reason: reason);
                await ReplyAsync($"Later bud {member.Mention}");
            }
        }

        [Command("mute")]
        [Summary("Prevent someone from sending messages in this guild.")]
        // Maybe use different permission since mute is meant for voice?
        [RequireUserPermission(GuildPermission.MuteMembers, ErrorMessage = "You need permission to mute people to use this command.")]
        [RequireBotPermission(GuildPermission.ManageRoles, ErrorMessage = "I need permission to manage roles.")]
        [RequireBotPermission(GuildPermission.ManageChannels, ErrorMessage = "I need permission to manage channels.")]
        /* TODO:
         * add optional time parameter
         * use custom type reader for time? 
         */
        public async Task Mute(IGuildUser member)
        {
            IRole muteRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == muteRoleName);

            // Make sure mute role exists
            if (muteRole == null)
                muteRole = await Context.Guild.CreateRoleAsync(muteRoleName);

            // Make sure all channels prohibit the role from sending messages
            foreach (SocketGuildChannel channel in Context.Guild.Channels)
            {
                OverwritePermissions? perms = channel.GetPermissionOverwrite(muteRole);
                if (!perms.HasValue || perms.Value.SendMessages != PermValue.Deny)
                    await channel.AddPermissionOverwriteAsync(muteRole, new OverwritePermissions(sendMessages: PermValue.Deny));
            }

            await member.AddRoleAsync(muteRole);
            await ReplyAsync($"I've muted {member.Username}.");
        }

        [Command("unmute")]
        [Summary("Unmute the specified member.")]
        [RequireUserPermission(GuildPermission.MuteMembers, ErrorMessage = "You don't have permission to do that.")]
        [RequireBotPermission(GuildPermission.ManageRoles, ErrorMessage = "I need permission to manage roles.")]
        public async Task Unmute(IGuildUser member)
        {
            IRole muteRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == muteRoleName);
            if (muteRole == null || !member.RoleIds.Contains(muteRole.Id))
            {
                await ReplyAsync("They're already not muted.");
                return;
            }

            await member.RemoveRoleAsync(muteRole);
            await ReplyAsync($"I'll allow {member.Username} to speak.");
        }

        [Command("autorole")]
        [Summary("Sets role to give users when they join.")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Autorole(IRole role = null)
        {
            if (role == null)
            {
                IRole autorole = await _guildConfig.GetAutoRole(Context.Guild);
                if (autorole == null)
                    await ReplyAsync("Auto role isn't setup in this server.");
                else
                    await ReplyAsync($"The auto role is set to {autorole.Mention}");
                return;
            }
            await _guildConfig.SetAutoRole(Context.Guild, role);
            await ReplyAsync($"Users who join the server will now have be given the {role.Mention} role.");
        }
    }
}
