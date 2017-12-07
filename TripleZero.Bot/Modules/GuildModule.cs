using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Helper;
using SWGoH.Model;
using SWGoH.Model.Enums;
using TripleZero.Core.Caching;

namespace TripleZero.Modules
{
    [Name("Guild")]
    [Summary("Guild Commands")]
    public class GuildModule : ModuleBase<SocketCommandContext>
    {
        private CacheClient cacheClient = IResolver.Current.CacheClient;

        [Command("guildCharacter", RunMode = RunMode.Async)]
        [Summary("Get report for specific character in the specified guild")]
        [Remarks("*guildCharacter {guildAlias or guildId} {characterAlias}*")]
        [Alias("gc")]
        public async Task GetGuildCharacter(string guildAlias, string characterAlias)
        {
            guildAlias = guildAlias.Trim();
            characterAlias = characterAlias.Trim();

            string loadingStr = $"```I am trying to load guild with alias '{guildAlias}' to show all players having {characterAlias}```";
            var messageLoading = await ReplyAsync($"{loadingStr}");

            string retStr = "";

            //get from cache if possible and exit sub
            string functionName = "guildCharacter";
            string key = string.Concat(guildAlias,characterAlias);
            retStr = cacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"{retStr}");
                return;
            }

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                return;
            }

            var characterConfig = IResolver.Current.CharacterSettings.GetCharacterConfigByAlias(characterAlias).Result;
            if (characterConfig == null)
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"I couldn't find any character with alias ***{characterAlias}***");
                return;
            }

            var res = await IResolver.Current.SWGoHRepository.GetGuildUnit(guildConfig.SWGoHId, characterConfig.Name);

            if (res != null)
            {                
                retStr += $"\n***Guild : {guildConfig.Name} - Character : {characterConfig.Name}***";

                int counter = 1;
                foreach (var player in res.Players.OrderByDescending(p => p.Rarity).ThenByDescending(t => t.Power))
                {
                    retStr += "\n";
                    retStr += string.Format("{4} : `{3}* - {2} - {1} : {0}`", player.PlayerName, player.Level, player.Power.ToString().Length < 5 ? string.Concat(player.Power.ToString(), " ") : player.Power.ToString(), player.Rarity, counter);
                    counter += 1;
                }
                await ReplyAsync($"{retStr}");
            }
            else
            {
                await messageLoading.DeleteAsync();
                retStr = $"I didn't find any players having `{guildConfig.Name} for guild {characterConfig.Name}`";                
                await ReplyAsync($"{retStr}");
            }
            await messageLoading.DeleteAsync();
            await cacheClient.AddToModuleCache(functionName, key, retStr);
        }

        //needs refactor with strategy
        [Command("guildShip", RunMode = RunMode.Async)]
        [Summary("Get report for specific ship in the specified guild")]
        [Remarks("*guildShip {guildAlias or guildId} {shipAlias}*")]
        [Alias("gs")]
        public async Task GetGuildShip(string guildAlias, string shipAlias)
        {
            guildAlias = guildAlias.Trim();
            shipAlias = shipAlias.Trim();

            string loadingStr = $"```I am trying to load guild with alias '{guildAlias}' to show all players having {shipAlias}```";
            var messageLoading = await ReplyAsync($"{loadingStr}");

            string retStr = "";

            //get from cache if possible and exit sub
            string functionName = "guildCharacter";
            string key = string.Concat(guildAlias, shipAlias);
            retStr = cacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"{retStr}");
                return;
            }

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                return;
            }

            var shipConfig = IResolver.Current.ShipSettings.GetShipConfigByAlias(shipAlias).Result;
            if (shipConfig == null)
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"I couldn't find any ship with alias ***{shipAlias}***");
                return;
            }

            var res = await IResolver.Current.SWGoHRepository.GetGuildUnit(guildConfig.SWGoHId, shipConfig.Name);

            if (res != null)
            {
                retStr += $"\n***Guild : {guildConfig.Name} - Ship : {shipConfig.Name}***";

                int counter = 1;
                foreach (var player in res.Players.OrderByDescending(p => p.Rarity).ThenByDescending(t => t.Power))
                {
                    retStr += "\n";
                    retStr += string.Format("{4} : `{3}* - {2} - {1} : {0}`", player.PlayerName, player.Level, player.Power.ToString().Length < 5 ? string.Concat(player.Power.ToString(), " ") : player.Power.ToString(), player.Rarity, counter);
                    counter += 1;
                }
                await ReplyAsync($"{retStr}");
            }
            else
            {
                await messageLoading.DeleteAsync();
                retStr = $"I didn't find any players having `{guildConfig.Name} for guild {shipConfig.Name}`";
                await ReplyAsync($"{retStr}");
            }
            await messageLoading.DeleteAsync();
            await cacheClient.AddToModuleCache(functionName, key, retStr);
        }

        [Command("slackers-level", RunMode = RunMode.Async)]
        [Summary("Get all players of guild with low level characters")]
        [Remarks("*slackers-level {guildAlias or guildId}*")]
        [Alias("slackers","sl")]
        public async Task GetSlackersLevel(string guildAlias)
        {
            guildAlias = guildAlias.Trim();

            string retStr = "";

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                return;
            }

            var res = await IResolver.Current.SWGoHRepository.GetGuildUnits(guildConfig.SWGoHId);

            if (res.FirstOrDefault().LoadedFromCache) await ReplyAsync($"{cacheClient.GetCachedDataRepositoryMessage()}");

            int counter = 1;
            int totalRows = 300;

            try
            {
                for (int level = 1; level < 50; level++)
                {
                    //var list = res.SelectMany(p => p.Players.Where(t => t.CombatType == UnitCombatType.Character && t.Level == level).ToList());
                    //var a = 1;

                    var characters = (from character in res
                                      from players in character.Players.Where(t => t.CombatType == UnitCombatType.Character && t.Level == level)
                                      select new
                                      {
                                          character.Name,
                                          players
                                      }
                                      ).ToList().OrderBy(p=>p.players.PlayerName);

                    var listCharacters = characters.Select(x => new Tuple<string, GuildPlayerUnit>(x.Name, x.players)).ToList();

                    if (listCharacters.Count() == 0) continue;                    

                    retStr += $"\n\n-------**Level {level}**-------";
                    foreach(var row in listCharacters.ToList() )
                    {
                        retStr += $"\n**{row.Item2.PlayerName}** : {row.Item1}";

                        if (retStr.Length > 1900)
                        {
                            await ReplyAsync($"{retStr}");
                            retStr = "";
                        }
                    }                   

                    //foreach (var guildCharacter in res)
                    //{
                    //    foreach (var player in guildCharacter.Players.Where(p => p.CombatType == UnitCombatType.Character))
                    //    {
                    //        if (player.Level == level)
                    //        {
                    //            retStr += "\n";
                    //            retStr += string.Format("{0} - {1} - level:{2}", player.PlayerName, guildCharacter.CharacterName, player.Level);

                    //            if (retStr.Length > 1800)
                    //            {
                    //                await ReplyAsync($"{retStr}");
                    //                retStr = "";
                    //            }

                    //            counter += 1;
                    //            if (counter > totalRows) break;
                    //        }
                    //    }
                    //}
                    if (counter > totalRows) break;
                }
            }
            catch (Exception ex)
            {
                Consoler.WriteLineInColor(string.Format("Slackers say : {0}", ex.Message), ConsoleColor.Red);
            }

            if (retStr.Length > 0)
                await ReplyAsync($"{retStr}");
        }

        [Command("tw", RunMode = RunMode.Async)]
        [Summary("Get all players of guild with characters having less than 6000 power")]
        [Remarks("*tw {guildAlias or guildId}*")]
        public async Task GetSlackersPower(string guildAlias)
        {
            guildAlias = guildAlias.Trim();

            string retStr = "";            

            string loadingStr = $"```I am trying to load guild with alias '{guildAlias}' to show players with less than 6k power```";
            var messageLoading = await ReplyAsync($"{loadingStr}");

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"```I couldn't find any guild with alias ***{guildAlias}***```");
                return;
            }

            var res = await IResolver.Current.SWGoHRepository.GetGuildUnits(guildConfig.SWGoHId);

            if (res.FirstOrDefault().LoadedFromCache) await ReplyAsync($"{cacheClient.GetCachedDataRepositoryMessage()}");

            var characters = (from character in res
                              from players in character.Players.Where(t => t.CombatType == UnitCombatType.Character && t.Power < 6000)
                              select new
                              {
                                  players.PlayerName,
                                  character.Name,
                                  players.Power
                              }
                                       ).ToList().OrderBy(p => p.PlayerName);
            
            //var charactersNoSlacking = (from character in res
            //                  from players in character.Players
            //                  where(from t in players.CombatType ==UnitCombatType.Character )
            //                  select new
            //                  {
            //                      players.PlayerName,
            //                      character.CharacterName,
            //                      players.Power
            //                  }
            //                           ).ToList().OrderBy(p => p.PlayerName);

            var listCharacters = characters.Select(x => new Tuple<string, string, int>(x.PlayerName, x.Name, x.Power)).ToList();
            var sumList = listCharacters.GroupBy(a => a.Item1).Select(p => new { PlayerName = p.Key, Count = p.Count() }).OrderByDescending(p=>p.Count);

            await messageLoading.DeleteAsync();

            retStr += $"```I found **{sumList.Count()}** slackers for guild **{guildConfig.Name}**```";

            foreach (var row in sumList)
            {
                if (retStr.Length > 1900)
                {
                    await ReplyAsync($"{retStr}");
                    retStr = "";
                }
                retStr += $"\n**{row.PlayerName}** : {row.Count} characters with less than 6000 power";
            }

            if (retStr.Length > 0)
                await ReplyAsync($"{retStr}");            
        }

        [Command("gp", RunMode = RunMode.Async)]
        [Summary("Get details about Galactic Power for the specified guild")]
        [Remarks("*gp {guildAlias or guildId}*")]
        public async Task GetCharacterGP(string guildAlias)
        {
            guildAlias = guildAlias.Trim();

            string retStr = "";

            //get from cache if possible and exit sub
            string functionName = "tb";
            string key = guildAlias;
            retStr = cacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            string loadingStr = $"```I am trying to load guild with alias '{guildAlias}' to show guild's Galactic Power details```";
            var messageLoading = await ReplyAsync($"{loadingStr}");

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                await messageLoading.DeleteAsync();
                return;
            }           

            var result = IResolver.Current.MongoDBRepository.GetGuildPlayers(guildConfig.Name).Result;
            if(result==null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                await messageLoading.DeleteAsync();
                return;
            }

            List<Player> guildPlayers = new List<Player>();

            retStr += string.Format("```I found **{0}** players for guild **{1}**```", result.Players.Count(), guildConfig.Name);
            retStr += string.Format("\nTotal GP **{0:n0}**", result.GalacticPower);
            retStr += string.Format("\nCharacter GP **{0:n0}**", result.Players.Sum(p => p.GalacticPowerCharacters));
            retStr += string.Format("\nShip GP **{0:n0}**", result.Players.Sum(p => p.GalacticPowerShips));

            await ReplyAsync($"{retStr}");
            await messageLoading.DeleteAsync();
            await cacheClient.AddToModuleCache(functionName, key, retStr);

        }

        [Command("guildPlayers", RunMode = RunMode.Async)]
        [Summary("Get available players in specified guild")]
        [Remarks("*guildPlayers {guildAlias or guildId} {searchString(optional)}*")]
        [Alias("guild")]
        public async Task GetGuildPlayers(string guildAlias, string searchStr = "")
        {
            guildAlias = guildAlias.Trim();
            searchStr = searchStr.Trim();

            string retStr = "";
            //get from cache if possible and exit sub
            string functionName = "guildPlayers";
            string key = string.Concat(guildAlias,searchStr);
            retStr = cacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            string loadingStr = $"```I am trying to load guild with alias '{guildAlias}' to show all players in the guild```";
            var messageLoading = await ReplyAsync($"{loadingStr}");

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                await messageLoading.DeleteAsync();
                return;
            }
            var result = IResolver.Current.MongoDBRepository.GetGuildPlayers(guildConfig.Name).Result;
            if (result.LoadedFromCache) await ReplyAsync($"{cacheClient.GetCachedDataRepositoryMessage()}");

            List<Player> guildPlayers = new List<Player>();            

            if (searchStr.Length == 0)
            {
                retStr = string.Format("```These are the players of guild {0}```", guildConfig.Name);
                guildPlayers = result.Players;
            }
            else
            {
                if (searchStr.Length >= 2)
                {
                    guildPlayers = result.Players.Where(p => p.PlayerNameInGame.ToLower().Contains(searchStr.ToLower()) || p.PlayerName.ToLower().Contains(searchStr.ToLower())).ToList();
                    retStr += retStr = $"```These are the players of guild {guildConfig.Name} containing \"{searchStr}\"```";
                }
                else
                {
                    await ReplyAsync($"\nYour search string is not valid. You will get all players of guild {guildConfig.Name}\n");
                }
            }
            
            int counter = 1;
            foreach (var player in guildPlayers)
            {

                retStr += $"\n{counter}) {player.PlayerName} ({player.PlayerNameInGame})";
                counter += 1;                
            }
            await cacheClient.AddToModuleCache(functionName, key, retStr);
            await ReplyAsync($"{retStr}");
            await messageLoading.DeleteAsync();
        }

        [Command("guildCharacterZeta", RunMode = RunMode.Async)]
        [Summary("Get zeta report for specific character in specified guild")]
        [Remarks("*guildZeta {guildAlias or guildId} {characterAlias}*")]
        [Alias("gz")]
        public async Task GetGuildZetas(string guildAlias,string characterAlias)
        {
            guildAlias = guildAlias.Trim();
            characterAlias = characterAlias.Trim();

            string retStr = "";

            string loadingStr = $"```I am trying to load guild with alias '{guildAlias}' to show all zeta in the specified character```";
            var messageLoading = await ReplyAsync($"{loadingStr}");

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                await messageLoading.DeleteAsync();
                return;
            }

            var characterConfig = IResolver.Current.CharacterSettings.GetCharacterConfigByAlias(characterAlias).Result;
            if (characterConfig == null)
            {
                await ReplyAsync($"I couldn't find any character with alias ***{characterAlias}***");
                await messageLoading.DeleteAsync();
                return;
            }         
            
            if(characterConfig.Abilities==null || characterConfig.Abilities.Count()==0)
            {
                await ReplyAsync($"```Warning : No Abilities```");
            }

            var result = IResolver.Current.MongoDBRepository.GetGuildPlayers(guildConfig.Name).Result;
            var players = IResolver.Current.MongoDBRepository.GetGuildCharacterAbilities(result.Players.Select(p=>p.PlayerName).ToList<string>() , characterConfig.Name).Result;
            if (players.FirstOrDefault().LoadedFromCache) await ReplyAsync($"{cacheClient.GetCachedDataRepositoryMessage()}");
            var orderedPlayers = players.OrderByDescending(t => t?.Characters?[0]?.Abilities?.Sum(m => m?.Level));

            var dictZeta = new Dictionary<string, int>();
            try
            {
                foreach (var player in orderedPlayers)
                {
                    retStr += "\n";
                    var character = player.Characters.FirstOrDefault();

                    if(player.PlayerName== "wortheon")
                    {
                        var a = 1;
                    }                    

                    if(!character.IsUnlocked)
                    {
                        dictZeta.Add(string.Concat(player.PlayerName, "(", player.PlayerNameInGame, ")", " : ***Locked***"), -1);
                        continue;
                    }

                    int countZeta = 0;
                    List<string> zetas = new List<string>();
                    foreach (var ability in character.Abilities)
                    {
                        

                        var configAbility = characterConfig.Abilities?.Where(p => p.Name == ability.Name).FirstOrDefault();

                        if (configAbility?.AbilityType == AbilityType.Zeta)
                        {
                            if (ability.Level == ability.MaxLevel)
                            {
                                countZeta += 1;
                                zetas.Add(ability.Name);
                            }
                        }
                    }
                    if (countZeta == 0)
                            dictZeta.Add(string.Concat(player.PlayerName, "(", player.PlayerNameInGame, ")", " : ***No Zeta***"),countZeta);
                    else
                        dictZeta.Add(string.Concat(player.PlayerName,"(", player.PlayerNameInGame , ")", " : ", string.Join(" - ", zetas.ToArray())), countZeta);
                }
            }
            catch(Exception ex)
            {
                await ReplyAsync($"***Something bad happened!***");
                await messageLoading.DeleteAsync();
                return;
            }
            

            var dictOrdered = dictZeta.OrderByDescending(p => p.Value);

            retStr += retStr = $"```This is the zeta report for \"{characterConfig.Name}\" for guild \"{guildConfig.Name}\"```";
            int count = 1;
            foreach (var keyvalue in dictOrdered)
            {
                retStr += $"{count}. {keyvalue.Key} (total zetas:{keyvalue.Value})\n";
                    count += 1;
                if (retStr.Length > 1800)
                {
                    await ReplyAsync($"{retStr}");
                    retStr = "";
                }
            }

            await ReplyAsync($"{retStr}");
            await messageLoading.DeleteAsync();
        }
    }
}
