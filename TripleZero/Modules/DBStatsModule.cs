﻿using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Configuration;
using TripleZero.Repository.Dto;
using SwGoh;
using Discord.WebSocket;
using Discord;
using TripleZero.Helper;

namespace TripleZero.Modules
{
    [Name("DBStats")]
    [Summary("Stats Commands")]
    public class DBStatsModule : ModuleBase<SocketCommandContext>
    {      
        [Command("stats-players")]
        [Summary("Get stats about player collection")]
        [Remarks("*stats-players*")]
        public async Task GetStats()
        {
            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.GetAllPlayersWithoutCharacters().Result;


            if (result != null)
            {
                retStr += string.Format("\nTotal players loaded to DB : **{0}** ", result.Count());
                retStr += string.Format("\nSWGoH date - Latest: **{0}** - Oldest: **{1}** ", result.OrderByDescending(p=>p.LastSwGohUpdated).Take(1).FirstOrDefault().LastSwGohUpdated, result.OrderBy(p => p.LastSwGohUpdated).Take(1).FirstOrDefault().LastSwGohUpdated);
                retStr += string.Format("\nDB date - Latest: **{0}** - Oldest: **{1}** ", result.OrderByDescending(p => p.LastClassUpdated).Take(1).FirstOrDefault().LastClassUpdated, result.OrderBy(p => p.LastClassUpdated).Take(1).FirstOrDefault().LastClassUpdated);
            }
            else
                retStr = string.Format("\nSomething is wrong with stats -p!!!");

            await ReplyAsync($"{retStr}");
        }

        [Command("player-getall")]
        [Summary("Get all players in collection")]
        [Remarks("*player-getall*")]
        public async Task GetAllPlayers()
        {
            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.GetAllPlayersWithoutCharacters().Result;

            if (result != null)
            {
                retStr += string.Format("\nTotal players loaded to DB : **{0}**\n", result.Count());
                result = result.OrderBy(p => p.GuildName).ThenByDescending(p => p.LastSwGohUpdated).ToList();
                foreach (var player in result)
                {
                    //retStr += string.Format("\nGuild : ***{4}*** - PlayerName : ***{0}***({1}) - SWGoHUpdate: ***{2}*** - DBUpdate: ***{3}***  ", player.PlayerName,player.PlayerNameInGame,player.LastSwGohUpdated,player.LastClassUpdated,player.GuildName);
                    retStr += string.Format("\nGuild : ***{3}*** - PlayerName : ***{0}***({1}) - SWGoHUpdate: ***{2}***", player.PlayerName, player.PlayerNameInGame, player.LastSwGohUpdated, player.GuildName);

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
