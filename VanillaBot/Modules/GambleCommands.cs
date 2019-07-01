using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VanillaBot.Modules.Gambling;
using VanillaBot.Services;
using VanillaBot.Services.Database.Models;

namespace VanillaBot.Modules
{
    [Name("gambling")]
    public class GambleCommands : ModuleBase<SocketCommandContext>
    {
        private readonly PointsService _points;
        private readonly Random _random;

        public GambleCommands(PointsService points, Random random)
        {
            _points = points;
            _random = random;
        }

        [Command("coin")]
        [Summary("Gamble your points with a 50% chance.")]
        public async Task GambleCoin(Coin guess, uint amount)
        {
            Points points = await _points.GetPoints(Context.User);
            if (points == null || points.Amount < amount)
            {
                await ReplyAsync("You don't have enough points.");
                return;
            }

            Coin coin = (Coin)_random.Next(2);
            string coinImgUrl = coin == Coin.Heads ?
                "https://www.ssaurel.com/blog/wp-content/uploads/2017/01/heads.png": 
                "https://www.ssaurel.com/blog/wp-content/uploads/2017/01/tails.png";

            if (guess == coin)
            {
                Embed embed = new EmbedBuilder()
                    .WithTitle("You won! Congrats!")
                    .WithColor(Color.Green)
                    .WithDescription($"Your new balance is {points.Amount + amount}")
                    .WithThumbnailUrl(coinImgUrl)
                    .Build();

                await _points.AddPoints(Context.User, (int)amount);
                await ReplyAsync(embed: embed);
            }
            else
            {
                Embed embed = new EmbedBuilder()
                    .WithTitle("You lost! Better luck next time.")
                    .WithColor(Color.Red)
                    .WithDescription($"Your new balance is {points.Amount - amount}")
                    .WithThumbnailUrl(coinImgUrl)
                    .Build();

                await _points.AddPoints(Context.User, (int)-amount);
                await ReplyAsync(embed: embed);
            }
        }
    }
}
