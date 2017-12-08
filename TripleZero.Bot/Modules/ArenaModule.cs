using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Helper;
using SWGoH.Model.Enums;
using System.Diagnostics;
using System.Globalization;
using Discord;
using TripleZero.Core.Caching;

namespace TripleZero.Modules
{
    [Name("Arena")]
    [Summary("Arena Commands")]
    public class ArenaModule : ModuleBase<SocketCommandContext>
    {
        private CacheClient cacheClient = IResolver.Current.CacheClient;

        [Command("guild-arena", RunMode = RunMode.Async)]
        [Summary("Get arena guild report")]
        [Remarks("*guild-arena {guildAlias}*")]
        [Alias("ga")]
        public async Task GuildArena(string guildAlias)
        {
            guildAlias = guildAlias.Trim();

            string retStr = "";

            string loadingStr = $"```I am trying to load guild with alias '{guildAlias}' to show all arena teams```";
            var messageLoading = await ReplyAsync($"{loadingStr}");

            await ReplyAsync($"{retStr}");
        }      

    }
}
