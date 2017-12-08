﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Helper;
using TripleZero.Core.Caching;

namespace TripleZero.Modules
{
    [Name("DBStats")]
    [Summary("Stats Commands")]
    public class DBStatsModule : ModuleBase<SocketCommandContext>
    {
        private CacheClient cacheClient = IResolver.Current.CacheClient;

        [Command("stats-players", RunMode = RunMode.Async)]
        [Summary("Get stats about player collection")]
        [Remarks("*stats-players*")]
        [Alias("sp")]
        public async Task GetStats()
        {
            string retStr = "";           

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            //get from cache if possible and exit sub
            string functionName = "stats-players";
            string key = "all";
            retStr = cacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.GetAllPlayersNoCharactersNoShips().Result;

            if (result != null)
            {
                //if (result.FirstOrDefault().LoadedFromCache) retStr += CacheClient.GetCachedDataRepositoryMessage();
                if (result.FirstOrDefault().LoadedFromCache) await ReplyAsync($"{cacheClient.GetCachedDataRepositoryMessage()}");

                retStr += string.Format("\nTotal players loaded to DB : **{0}** ", result.Count());
                retStr += string.Format("\nSWGoH date - Latest: **{0}** - Oldest: **{1}** ", result.OrderByDescending(p => p.SWGoHUpdateDate).Take(1).FirstOrDefault().SWGoHUpdateDate, result.OrderBy(p => p.SWGoHUpdateDate).Take(1).FirstOrDefault().SWGoHUpdateDate);
                retStr += string.Format("\nDB date - Latest: **{0}** - Oldest: **{1}** ", result.OrderByDescending(p => p.EntryUpdateDate).Take(1).FirstOrDefault().EntryUpdateDate, result.OrderBy(p => p.EntryUpdateDate).Take(1).FirstOrDefault().EntryUpdateDate);
            }
            else
                retStr = string.Format("\nSomething is wrong with stats -p!!!");

            await ReplyAsync($"{retStr}");
            await cacheClient.AddToModuleCache(functionName, key, retStr);
        }

        [Command("player-getall", RunMode = RunMode.Async)]
        [Summary("Get all players in collection")]
        [Remarks("*player-getall*")]
        [Alias("players")]
        public async Task GetAllPlayers()
        {
            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.GetAllPlayersNoCharactersNoShips().Result;

            if (result != null)
            {
                //if (result.FirstOrDefault().LoadedFromCache) retStr += CacheClient.GetCachedDataRepositoryMessage();
                if (result.FirstOrDefault().LoadedFromCache) await ReplyAsync($"{cacheClient.GetCachedDataRepositoryMessage()}");

                retStr += string.Format("\nTotal players loaded to DB : **{0}**\n", result.Count());
                result = result.OrderBy(p => p.GuildName).ThenByDescending(p => p.SWGoHUpdateDate).ToList();
                foreach (var player in result)
                {
                    //retStr += string.Format("\nGuild : ***{4}*** - PlayerName : ***{0}***({1}) - SWGoHUpdate: ***{2}*** - DBUpdate: ***{3}***  ", player.PlayerName,player.PlayerNameInGame,player.LastSwGohUpdated,player.LastClassUpdated,player.GuildName);
                    retStr += string.Format("\nGuild : ***{3}*** - PlayerName : ***{0}***({1}) - SWGoHUpdate: ***{2}***", player.PlayerName, player.PlayerNameInGame, player.SWGoHUpdateDate, player.GuildName);

                    if (retStr.Length > 1800)
                    {
                        await ReplyAsync($"{retStr}");
                        retStr = "";
                    }
                }
            }
            else
                retStr = string.Format("\nSomething is wrong with stats -p!!!");

            await ReplyAsync($"{retStr}");
        }
    }
}
