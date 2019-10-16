using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YukoBot.Modules
{
    [Name("Fun")]
    public class FunCommands : ModuleBase<SocketCommandContext>
    {
        private readonly Random _random;

        public FunCommands(Random random)
        {
            _random = random;
        }

        private static readonly string[] numbers = new string[7]
        {
            "\u0030\u20E3",
            "\u0031\u20E3",
            "\u0032\u20E3",
            "\u0033\u20E3",
            "\u0034\u20E3",
            "\u0035\u20E3",
            "\u0036\u20E3",
        };

        public bool ValidPos(int x, int y, uint size) =>
            (x >= 0 && x < size) && (y >= 0 && y < size);

        [Command("minesweeper"), Alias("mines")]
        [Summary("Play minesweeper! (kinda)")]
        public async Task Minesweeper(uint size = 9)
        {
            if (size >= 15)
            {
                await ReplyAsync("That's too big!");
                return;
            }

            bool[,] board = new bool[size, size];
            for (int i = 0; i < size; i++)
            {
                int x, y;
                do
                {
                    x = _random.Next((int)size);
                    y = _random.Next((int)size);
                } while (board[x, y]);
                board[x, y] = true;
            }

            List<string> messageBoard = new List<string>((int)(size * size + size));
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (board[x, y])
                    {
                        messageBoard.Add("||💣||");
                        continue;
                    }

                    int count = 0;
                    for (int i = -1; i <= 1; i++)
                    {
                        for (int j = -1; j <= 1; j++)
                        {
                            if (ValidPos(x + i, y + j, size) && board[x + i, y + j])
                                count++;
                        }
                    }
                    messageBoard.Add("||" + numbers[count] + "||");
                }

                messageBoard.Add("\n");
            }

            await ReplyAsync(string.Join("", messageBoard));
        }
    }
}
