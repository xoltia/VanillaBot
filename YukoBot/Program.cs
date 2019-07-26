using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace YukoBot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Console.Title = "YukoBot";

            YukoBot bot = new YukoBot();
            await bot.StartFromConfig();
            await Task.Delay(-1);
        }
    }
}
