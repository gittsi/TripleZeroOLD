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
using TripleZero.Helper;
using SwGoh;

namespace TripleZero.Modules
{
    [Name("Guild")]
    [Summary("Get some useful guild data")]
    public class GuildModule : ModuleBase<SocketCommandContext>
    {        

        [Command("guildCharacter")]
        [Summary("Get report for specific character in the given guild.\nUsage : ***$guildCharacter {guildAlias or guildId} {characterAlias}***")]
        public async Task GetGuildCharacter(string guildAlias , string characterAlias)
        {
            guildAlias = guildAlias.Trim();
            characterAlias = characterAlias.Trim();

            string retStr = "";
            var guildConfig = IResolver.Current.GuildsConfig.GetGuildConfig(guildAlias).Result;
            if(guildConfig==null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                return;
            }
            
            var characterConfig = IResolver.Current.CharacterConfig.GetCharacterConfigByAlias(characterAlias).Result;
            if (characterConfig == null)
            {
                await ReplyAsync($"I couldn't find any character with alias ***{characterAlias}***");
                return;
            }

            var res = await IResolver.Current.SWGoHRepository.GetGuildCharacter(guildConfig.SWGoHId,characterConfig.Command);

            if (res != null)
            {
                await ReplyAsync($"***Guild : {guildConfig.Name} - Character : {characterConfig.Name}***");

                foreach (var player in res.Players.OrderByDescending(p => p.Rarity).ThenByDescending(t => t.Power))
                {
                    retStr += "\n";
                    retStr += string.Format("{3}* - {2} - {1} : {0}", player.Name, player.Level, player.Power.ToString().Length < 5 ? string.Concat(player.Power.ToString(), " ") : player.Power.ToString(), player.Rarity);
                }

                await ReplyAsync($"{retStr}");
            }
            else
            {

                retStr = $"I didn't find any players having `{guildConfig.Name} for guild {characterConfig.Name}`";
                await ReplyAsync($"{retStr}");
            }

        }


        [Command("slackers")]
        [Summary("Get all players of guild with low level characters.\nUsage : ***$slackers {guildAlias or guildId}***")]
        public async Task GetSlackers(string guildAlias)
        {
            guildAlias = guildAlias.Trim();

            string retStr = "";
            var guildConfig = IResolver.Current.GuildsConfig.GetGuildConfig(guildAlias).Result;

            var res = await IResolver.Current.SWGoHRepository.GetGuildCharacters(guildConfig.SWGoHId);

            int counter = 1;
            int totalRows = 25;

            try
            {
                for (int level = 1; level < 100; level++)
                {
                    foreach (var guildCharacter in res)
                    {
                        foreach (var player in guildCharacter.Players.Where(p=>p.Combat_Type==1))
                        {
                            if (player.Level == level)
                            {
                                retStr += "\n";
                                retStr += string.Format("{0} - {1} - level:{2}", player.Name, guildCharacter.Name, player.Level);
                                counter += 1;
                                if (counter > totalRows) break;
                                //Console.WriteLine(maxCounter1.ToString());
                            }
                        }
                    }
                    if (counter > totalRows) break;
                    //Console.WriteLine("level" + level);
                }
            }
            catch (Exception ex)
            {
                Consoler.WriteLineInColor(string.Format("Slackers say : {0}", ex.Message), ConsoleColor.Red);
            }


            await ReplyAsync($"{retStr}");           

        }

        [Command("tb")]
        [Summary("Get details about Galactic Power for the specified guild.\nUsage : ***$tb  {guildAlias or guildId}***")]
        public async Task GetCharacterGP(string guildAlias)
        {
            guildAlias = guildAlias.Trim();

            string retStr = "";
            var guildConfig = IResolver.Current.GuildsConfig.GetGuildConfig(guildAlias).Result;
            var result = IResolver.Current.MongoDBRepository.GetGuildPlayers(guildConfig.Name).Result;
            List<PlayerDto> guildPlayers = new List<PlayerDto>();

            retStr += string.Format("\nFound **{0}** players for guild **{1}**\n", result.Players.Count(), guildConfig.Name);
            retStr += string.Format("\nTotal GP **{0:n0}**", result.GP);
            retStr += string.Format("\nCharacter GP **{0:n0}**", result.Players.Sum(p=>p.GPcharacters));
            retStr += string.Format("\nShip GP **{0:n0}**", result.Players.Sum(p => p.GPships));           

            await ReplyAsync($"{retStr}");

        }


        [Command("guildPlayers")]
        [Summary("Get available players in specified guild.\nUsage : ***$guildPlayers {guildAlias or guildId} {searchString(optional)}***")]
        public async Task GetGuildPlayers(string guildAlias,string searchStr ="")
        {
            guildAlias = guildAlias.Trim();
            searchStr = searchStr.Trim();

            string retStr = "";
            var guildConfig = IResolver.Current.GuildsConfig.GetGuildConfig(guildAlias).Result;
            var result = IResolver.Current.MongoDBRepository.GetGuildPlayers(guildConfig.Name).Result;
            List<PlayerDto> guildPlayers=new List<PlayerDto>();

            retStr = string.Format("\n These are the players of guild **{0}**", guildConfig.Name);

            if (searchStr.Length==0)
            {
                guildPlayers = result.Players;    
                
            }else
            {
                if (searchStr.Length >= 2)
                {
                    guildPlayers = result.Players.Where(p => p.PlayerNameInGame.ToLower().Contains(searchStr.ToLower()) || p.PlayerName.ToLower().Contains(searchStr.ToLower())).ToList();
                    retStr += $" containing \"{searchStr}\"";
                }
                else
                {
                    await ReplyAsync($"\nYour search string is not valid. You will get all players of guild {guildConfig.Name}");
                }
            }

            retStr += "\n";
            int counter = 1;
            foreach (var player in guildPlayers)
            {
                
                retStr += $"\n{counter}) {player.PlayerName} ({player.PlayerNameInGame})";
                counter += 1;
                //retStr += string.Format("\n{0} {1} {2} {3}", player.GPcharacters.ToString().PadRight(7, ' '), player.GPships.ToString().PadRight(7,' '),player.PlayerNameInGame,player.PlayerName);
            }

            await ReplyAsync($"{retStr}");
           
        }


        
    }
}
