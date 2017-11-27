using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TripleZero.Modules
{
    [Name("Fun")]
    [Summary("Have some fun with commands!!!")]
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        [Command("roll", RunMode = RunMode.Async)]
        [Summary("Roll a dice")]
        [Remarks("*I am sure you can guess it!!!*")]
        public async Task Say()
        {
            Random rand1 = new Random();

            int roll = rand1.Next(0, 100);

            await ReplyAsync($"{this.Context.User.Username} rolled {roll}!!!");
        }

        [Command("imba", RunMode = RunMode.Async)]
        [Summary("Shows who is imba currently by analyzing all data")]
        [Remarks("*imba*")]        
        public async Task SayImba()
        {
            await ReplyAsync("TSiTaS is imba!\nNo need to retrieve any data!\nFor now and till the end of time!!!");
        }
    }
}
