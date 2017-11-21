using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Helper;
using SWGoH.Model;
using SWGoH.Model.Enums;

namespace TripleZero.Modules
{
    [Name("Guild")]
    [Summary("Guild Commands")]
    public class GuildModule : ModuleBase<SocketCommandContext>
    {
        [Command("guildCharacter")]
        [Summary("Get report for specific character in the given guild")]
        [Remarks("*guildCharacter {guildAlias or guildId} {characterAlias}*")]
        public async Task GetGuildCharacter(string guildAlias, string characterAlias)
        {
            guildAlias = guildAlias.Trim();
            characterAlias = characterAlias.Trim();

            string retStr = "";

            //get from cache if possible and exit sub
            string functionName = "guildCharacter";
            string key = string.Concat(guildAlias,characterAlias);
            retStr = CacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                return;
            }

            var characterConfig = IResolver.Current.CharacterSettings.GetCharacterConfigByAlias(characterAlias).Result;
            if (characterConfig == null)
            {
                await ReplyAsync($"I couldn't find any character with alias ***{characterAlias}***");
                return;
            }

            var res = await IResolver.Current.SWGoHRepository.GetGuildCharacter(guildConfig.SWGoHId, characterConfig.Command);

            if (res != null)
            {                
                retStr += $"\n***Guild : {guildConfig.Name} - Character : {characterConfig.Name}***";

                foreach (var player in res.Players.OrderByDescending(p => p.Rarity).ThenByDescending(t => t.Power))
                {
                    retStr += "\n";
                    retStr += string.Format("{3}* - {2} - {1} : {0}", player.PlayerName, player.Level, player.Power.ToString().Length < 5 ? string.Concat(player.Power.ToString(), " ") : player.Power.ToString(), player.Rarity);
                }
                await ReplyAsync($"{retStr}");
            }
            else
            {
                retStr = $"I didn't find any players having `{guildConfig.Name} for guild {characterConfig.Name}`";                
                await ReplyAsync($"{retStr}");
            }

            await CacheClient.AddToModuleCache(functionName, key, retStr);
        }

        [Command("slackers")]
        [Summary("Get all players of guild with low level characters")]
        [Remarks("*slackers {guildAlias or guildId}*")]
        public async Task GetSlackers(string guildAlias)
        {
            guildAlias = guildAlias.Trim();

            string retStr = "";

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                return;
            }

            var res = await IResolver.Current.SWGoHRepository.GetGuildCharacters(guildConfig.SWGoHId);

            int counter = 1;
            int totalRows = 300;

            try
            {
                for (int level = 1; level < 50; level++)
                {
                    foreach (var guildCharacter in res)
                    {
                        foreach (var player in guildCharacter.Players.Where(p => p.CombatType == UnitCombatType.Character))
                        {
                            if (player.Level == level)
                            {
                                retStr += "\n";
                                retStr += string.Format("{0} - {1} - level:{2}", player.PlayerName, guildCharacter.CharacterName, player.Level);

                                if (retStr.Length > 1800)
                                {
                                    await ReplyAsync($"{retStr}");
                                    retStr = "";
                                }

                                counter += 1;
                                if (counter > totalRows) break;
                            }
                        }
                    }
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

        [Command("tb")]
        [Summary("Get details about Galactic Power for the specified guild")]
        [Remarks("*tb {guildAlias or guildId}*")]
        public async Task GetCharacterGP(string guildAlias)
        {
            guildAlias = guildAlias.Trim();

            string retStr = "";
            //get from cache if possible and exit sub
            string functionName = "tb";
            string key = guildAlias;
            retStr = CacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                return;
            }
            var result = IResolver.Current.MongoDBRepository.GetGuildPlayers(guildConfig.Name).Result;
            List<Player> guildPlayers = new List<Player>();

            retStr += string.Format("\nFound **{0}** players for guild **{1}**\n", result.Players.Count(), guildConfig.Name);
            retStr += string.Format("\nTotal GP **{0:n0}**", result.GalacticPower);
            retStr += string.Format("\nCharacter GP **{0:n0}**", result.Players.Sum(p => p.GalacticPowerCharacters));
            retStr += string.Format("\nShip GP **{0:n0}**", result.Players.Sum(p => p.GalacticPowerShips));

            await ReplyAsync($"{retStr}");
            await CacheClient.AddToModuleCache(functionName, key, retStr);

        }

        [Command("guildPlayers")]
        [Summary("Get available players in specified guild")]
        [Remarks("*guildPlayers {guildAlias or guildId} {searchString(optional)}*")]
        public async Task GetGuildPlayers(string guildAlias, string searchStr = "")
        {
            guildAlias = guildAlias.Trim();
            searchStr = searchStr.Trim();

            string retStr = "";
            //get from cache if possible and exit sub
            string functionName = "guildPlayers";
            string key = string.Concat(guildAlias,searchStr);
            retStr = CacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            var guildConfig = IResolver.Current.GuildSettings.GetGuildConfigByAlias(guildAlias).Result;
            if (guildConfig == null)
            {
                await ReplyAsync($"I couldn't find any guild with alias ***{guildAlias}***");
                return;
            }
            var result = IResolver.Current.MongoDBRepository.GetGuildPlayers(guildConfig.Name).Result;
            List<Player> guildPlayers = new List<Player>();

            retStr = string.Format("\n These are the players of guild **{0}**", guildConfig.Name);

            if (searchStr.Length == 0)
            {
                guildPlayers = result.Players;
            }
            else
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
            await CacheClient.AddToModuleCache(functionName, key, retStr);
            await ReplyAsync($"{retStr}");

        }

    }
}
