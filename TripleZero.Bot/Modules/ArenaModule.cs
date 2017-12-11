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

            string retStr = "```Players```";

            string loadingStr = $"```I am trying to load guild with alias '{guildAlias}' to show all arena teams```";
            var messageLoading = await ReplyAsync($"{loadingStr}");

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                await messageLoading.DeleteAsync();
                return;
            }

            var characterConfig = IResolver.Current.CharacterSettings.GetCharactersConfig().Result;
            if (characterConfig == null)
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"I couldn't load character config!");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.GetGuildPlayers(guildConfig.Name).Result;
            var playerData = IResolver.Current.MongoDBRepository.GetGuildPlayersArena(result.Players.Select(p => p.PlayerName).ToList<string>()).Result.OrderBy(t=>t.PlayerName);

            if (playerData.FirstOrDefault().LoadedFromCache) await ReplyAsync($"{cacheClient.GetCachedDataRepositoryMessage()}");
            if (playerData == null || playerData.Count()==0)
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"I couldn't find data for guild : ***{guildConfig.Name}***.");
                return;
            }
            Dictionary<string, string> dict = new Dictionary<string, string>();

            retStr += $"```css\nArena Report for guild {guildConfig.Name}```";
            retStr += "```Players```";
            foreach (var player in playerData)
            {
                int count = 1;
                string leader = "";
                List<string> aliasListNoLeader = new List<string>();
                foreach(var arenaCharacter in player.Arena.ArenaTeam)
                {                    
                    var alias = characterConfig.Where(p => p.Name == arenaCharacter).FirstOrDefault()?.Aliases?.FirstOrDefault();
                    if (string.IsNullOrEmpty(alias))
                        alias = arenaCharacter;                      

                    if (count == 1)
                        leader = string.Concat(alias,"(L)");
                    else
                        aliasListNoLeader.Add(alias);

                    count += 1;
                }
                aliasListNoLeader.Sort();
                List<string> aliasList = new List<string>();
                aliasList.Add(leader);
                aliasList.AddRange(aliasListNoLeader);

                retStr += $"{player.PlayerName}({player.PlayerNameInGame}) - **{string.Join(", ", aliasList)}** - Average Rank : {player.Arena.AverageRank}\n";

                dict.Add($"{player.PlayerName}({player.PlayerNameInGame}", $"{string.Join(", ", aliasList)}");

                if (retStr.Length > 1800)
                {                    
                    await ReplyAsync($"{retStr}");
                    retStr = "";
                }
            }
            //if(messageLoading!=null)
            //    await messageLoading.DeleteAsync();
            await messageLoading.DeleteAsync();
            await ReplyAsync($"{retStr}");

            var groupList=dict.GroupBy(r => r.Value).ToDictionary(t => t.Key, t => t.Select(r => r.Key).ToList());
            var orderedList = groupList.OrderByDescending(p => p.Value.Count()).ThenBy(t=>t.Key).ToList();

            string retStr2 = "\n\n```Teams```";
            foreach(var row in orderedList)
            {
                retStr2 += $"{row.Key} - **#{row.Value.Count()}**\n";
            }
            
            await ReplyAsync($"{retStr2}");
        }      

    }
}
