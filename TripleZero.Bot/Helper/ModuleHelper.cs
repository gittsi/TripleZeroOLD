using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Bot.Helper
{
    public static class ModuleHelper 
    {
        public static Discord.IUserMessage PrintLoadingMessage(string Message , Discord.Commands.ModuleBase<Discord.Commands.SocketCommandContext> module)
        {
            string loadingStr = $"```I am trying to load guild with alias '{guildAlias}' to show all players in the guild```";
            var messageLoading = await com.repl ReplyAsync($"{loadingStr}");
        }
    }
}
