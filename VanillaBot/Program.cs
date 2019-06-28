using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace VanillaBot
{
    class Program
    {
        static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            VanillaBot bot = new VanillaBot();
            await bot.StartFromConfig();
            await Task.Delay(-1);
        }
    }
}
