using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VanillaBot.Modules
{
    public class BasicCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task PingCommand()
        {
            await ReplyAsync("Pong!");
        }
    }
}
