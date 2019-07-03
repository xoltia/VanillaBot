using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaBot.Services;
using VanillaBot.Services.Database.Models;

namespace VanillaBot.Modules
{
    [Group("points")]
    public class PointsCommands : ModuleBase<SocketCommandContext>
    {
        private readonly PointsService _points;

        public PointsCommands(PointsService points)
        {
            _points = points;
        }

        [Command, Summary("Check how many points you have.")]
        public async Task Points()
        {
            Points points = await _points.GetPoints(Context.User);
            if (points == null)
            {
                await ReplyAsync("You don't have any points, try again later!");
                return;
            }

            await ReplyAsync($"You have {points.Amount} points.");
        }

        [Command("give"), Alias("gift"), Summary("Share some of your points.")]
        public async Task GivePoints(IUser user, uint amount)
        {
            if (user.Id == Context.User.Id)
            {
                await ReplyAsync("Stop trying to waste my time!");
                return;
            }

            Points points = await _points.GetPoints(Context.User);
            if (points == null || points.Amount < amount)
            {
                await ReplyAsync("You don't have enough points!");
                return;
            }

            if (!(user is SocketUser socketUser))
            {
                // I don't think this should happen?
                return;
            }

            await _points.AddPoints(Context.User, (int)-amount);
            await _points.AddPoints(socketUser, (int)amount);
            await ReplyAsync($"I've given {user.Mention} {amount} of your points. Your new balance is {points.Amount}.");
        }
    }
}
