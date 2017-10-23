using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TripleZero.Modules
{
    [Name("Test")]
    [Summary("Do some test I guess")]
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("test1")]
        [Summary("Get test1")]
        public async Task Say()
        {            
            await ReplyAsync($"You asked for test1 command");
        }
    }
}
