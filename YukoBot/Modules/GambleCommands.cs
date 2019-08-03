using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YukoBot.Modules.Gambling;
using YukoBot.Services;

namespace YukoBot.Modules
{
    [Name("Gambling")]
    public class GambleCommands : ModuleBase<SocketCommandContext>
    {
        private readonly DbService _db;
        private readonly Random _random;

        private static List<string> slotShapes = new List<string>()
        {
            "💎",
            "🍋",
            "🍊",
            "🍒",
            "🔔",
            "🍆",
            "🍇",
            "🍉",
            "🍅",
        };

        public GambleCommands(DbService dbService, Random random)
        {
            _db = dbService;
            _random = random;
        }

        [Command("coin")]
        [Summary("Gamble your points with a 50% chance.")]
        public async Task GambleCoin(Coin guess, uint amount)
        {
            using (var uow = _db.GetDbContext())
            {
                int points = await uow.Points.GetPointsAsync(Context.User);
                if (points < amount)
                {
                    await ReplyAsync("You don't have enough points.");
                    return;
                }

                Coin coin = (Coin)_random.Next(2);
                string coinImgUrl = coin == Coin.Heads ?
                    "https://www.ssaurel.com/blog/wp-content/uploads/2017/01/heads.png" :
                    "https://www.ssaurel.com/blog/wp-content/uploads/2017/01/tails.png";

                if (guess == coin)
                {
                    Embed embed = new EmbedBuilder()
                        .WithTitle("You won! Congrats!")
                        .WithColor(Color.Green)
                        .WithDescription($"Your new balance is {points + amount}")
                        .WithThumbnailUrl(coinImgUrl)
                        .Build();

                    await uow.Points.AddPointsAsync(Context.User, (int)amount);
                    await ReplyAsync(embed: embed);
                }
                else
                {
                    Embed embed = new EmbedBuilder()
                        .WithTitle("You lost! Better luck next time.")
                        .WithColor(Color.Red)
                        .WithDescription($"Your new balance is {points - amount}")
                        .WithThumbnailUrl(coinImgUrl)
                        .Build();

                    await uow.Points.AddPointsAsync(Context.User, (int)-amount);
                    await ReplyAsync(embed: embed);
                }
            }

        }

        [Command("slot")]
        [Summary("Gamble your points on a slot machine.")]
        public async Task GambleSlot(uint amount)
        {
            using (var uow = _db.GetDbContext())
            {
                int points = await uow.Points.GetPointsAsync(Context.User);
                if (points < amount)
                {
                    await ReplyAsync("You don't have enough points.");
                    return;
                }

                string[] randomSlots = new string[3];
                for (int i = 0; i < randomSlots.Length; i++)
                    randomSlots[i] = slotShapes[_random.Next(slotShapes.Count)];

                // Number of duplicate items
                int duplicates = randomSlots.GroupBy(x => x).Select(x => x.Count()).OrderByDescending(x => x).First();

                EmbedBuilder embed = new EmbedBuilder();

                // This is where the points are added to the associated user
                if (duplicates == 3)
                {
                    await uow.Points.AddPointsAsync(Context.User, (int)amount * 3);
                    embed.Color = Color.Green;
                }
                else if (duplicates == 2)
                {
                    await uow.Points.AddPointsAsync(Context.User, (int)amount * 2);
                    embed.Color = Color.DarkGreen;
                }
                else
                {
                    await uow.Points.AddPointsAsync(Context.User, (int)amount * -1);
                    embed.Color = Color.LighterGrey;
                }

                embed.Description = $"[ {string.Join(" | ", randomSlots)} ]";
                embed.Footer = new EmbedFooterBuilder().WithText($"Your new balance is {points-amount}");

                await ReplyAsync(embed: embed.Build());
            }
        }
    }
}
