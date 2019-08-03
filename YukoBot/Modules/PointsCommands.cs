using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using YukoBot.Services;
using YukoBot.Services.Database.Models;

namespace YukoBot.Modules
{
    [Group("Points")]
    public class PointsCommands : ModuleBase<SocketCommandContext>
    {
        private readonly DbService _db;

        public PointsCommands(DbService db)
        {
            _db = db;
        }

        [Command, Summary("Check how many points you have.")]
        public async Task Points()
        {
            using (var uow = _db.GetDbContext())
            {
                int points = await uow.Points.GetPointsAsync(Context.User);
                await ReplyAsync($"You have {points} points!");
            }
        }

        [Command, Summary("Check how many points somone else has.")]
        public async Task Points(SocketGuildUser user)
        {
            using (var uow = _db.GetDbContext())
            {
                int points = await uow.Points.GetPointsAsync(user);
                await ReplyAsync($"{user.Username} has {points} points!");
            }
        }

        [Command("give"), Alias("gift"), Summary("Share some of your points.")]
        public async Task GivePoints(IUser user, uint amount)
        {
            if (user.Id == Context.User.Id)
            {
                await ReplyAsync("Stop trying to waste my time!");
                return;
            }

            using (var uow = _db.GetDbContext())
            {
                int points = await uow.Points.GetPointsAsync(Context.User);
                if (points < amount)
                {
                    await ReplyAsync("You don't have enough points!");
                    return;
                }
                await uow.Points.AddPointsAsync(Context.User, (int)-amount);
                await uow.Points.AddPointsAsync(user, (int)amount);
                await ReplyAsync($"I've given {user.Mention} {amount} of your points. Your new balance is {points-amount}.");
            }
        }
        
        // TODO: per guild leaderboard
        [Command("leaderboard")]
        public async Task Leaderboard()
        {
            var uow = _db.GetDbContext();
            Points[] points = await uow.Points.GetTopPointsAsync();
            uow.Dispose();

            EmbedBuilder embed = new EmbedBuilder()
                .WithDescription("Heres the top 5 global point holders.")
                .WithColor(0xffc0cb);

            for (int i = 0; i < points.Length; i++)
            {
                SocketUser user = Context.Client.GetUser(ulong.Parse(points[i].UserId));
                embed.AddField(f =>
                {
                    f.Name = user == null ? "Unknown" : $"{user.Username}#{user.Discriminator}";
                    f.Value = points[i].Amount;
                    f.IsInline = true;
                });
            }

            Chart chart = new Chart();
            chart.Size = new Size(1200, 500);
            chart.BackColor = System.Drawing.Color.Transparent;

            // TODO: possible to make this less.. the way it is?
            ChartArea chartArea = new ChartArea("Graph");
            chartArea.BackColor = System.Drawing.Color.Transparent;
            chartArea.BorderColor = System.Drawing.Color.WhiteSmoke;
            chartArea.AxisX.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
            chartArea.AxisY.MajorGrid.LineColor = System.Drawing.Color.WhiteSmoke;
            chartArea.AxisX.LineColor = System.Drawing.Color.WhiteSmoke;
            chartArea.AxisY.LineColor = System.Drawing.Color.WhiteSmoke;
            chartArea.AxisX.LabelStyle.ForeColor = System.Drawing.Color.WhiteSmoke;
            chartArea.AxisY.LabelStyle.ForeColor = System.Drawing.Color.WhiteSmoke;
            chartArea.AxisX.LabelAutoFitMinFontSize = 30;
            chartArea.AxisY.LabelAutoFitMinFontSize = 25;

            chart.ChartAreas.Add(chartArea);
            Series barSeries = new Series();
            barSeries.Color = System.Drawing.Color.FromArgb(255, 192, 203);
            barSeries.Points.DataBindXY(
                points.Select(p =>
                {
                    // TODO: only do this once above?
                    SocketUser user = Context.Client.GetUser(ulong.Parse(p.UserId));
                    return user == null ? "Unknown" : $"{(user.Username.Length > 10 ? user.Username.Substring(0, 10) + "..." : user.Username)}#{user.Discriminator}";
                }).ToList(),
                points.Select(p => p.Amount).ToList()
            );
            barSeries.ChartType = SeriesChartType.Column;
            barSeries.ChartArea = "Graph";
            chart.Series.Add(barSeries);

            MemoryStream fileStream = new MemoryStream();
            chart.SaveImage(fileStream, ChartImageFormat.Png);

            fileStream.Position = 0;
            await Context.Message.Channel.SendFileAsync(fileStream, "graph.png", embed: embed.WithImageUrl("attachment://graph.png").Build());

            fileStream.Dispose();
            chart.Dispose();
        }
    }
}
