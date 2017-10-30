using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TripleZero.Modules
{
    [Name("Fun")]
    [Summary("Have some fun!!!")]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        [Command("roll")]
        [Summary("Rolls a dice")]
        public async Task Say()
        {
            Random rand1 = new Random();

            int roll= rand1.Next(0, 100);                        

            await ReplyAsync($"You rolled {roll}!!!");
        }

        [Command("imba")]
        [Summary("Rolls a dice")]
        public async Task SayImba()
        { 
            await ReplyAsync("TSiTaS is imba!!everyone knows that!!!");
        }
    }
}
