using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace YukoBot.Modules
{
    [Group("User")]
    public class UserCommands : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("Get information about a user.")]
        public async Task Default(IUser user)
        {
            TimeSpan accountAge = DateTime.Now - user.CreatedAt;

            EmbedBuilder embed = new EmbedBuilder()
                .WithTitle(user.Username)
                .WithDescription($"**ID:** {user.Id}\n" +
                $"**Created:** {user.CreatedAt.ToString("MM/dd/yyyy HH:mm:ss tt")}\n" +
                $"**Age:** {(accountAge.TotalDays >= 365 ? Math.Round(accountAge.TotalDays / 365, 2) + " years" : Math.Round(accountAge.TotalDays, 2) + " days")}\n" +
                $"**Is Bot:** {user.IsBot}" +
                $"**Status:** {user.Status}")
                .WithThumbnailUrl(user.GetAvatarUrl());

            await ReplyAsync(embed: embed.Build());
        }

        [Command("avatar"), Alias("pfp")]
        [Summary("Get a user's profile picture.")]
        public async Task Avatar(IUser user)
        {
            string avatarUrl = user.GetAvatarUrl(size: 2048) ?? user.GetDefaultAvatarUrl();

            await ReplyAsync(embed: new EmbedBuilder()
                .WithTitle($"{(user.Id == Context.Client.CurrentUser.Id ? "My" : user.Username + "'s" )} Avatar")
                .WithUrl(avatarUrl)
                .WithImageUrl(avatarUrl)
                .Build());
        }
    }
}
