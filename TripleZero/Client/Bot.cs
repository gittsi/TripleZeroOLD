using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.WebSocket;

namespace TripleZero.Client
{
    public class Bot
    {
        DiscordSocketClient client;

        public Bot()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig() { LogLevel = LogSeverity.Info });

     
        }
    }
}
