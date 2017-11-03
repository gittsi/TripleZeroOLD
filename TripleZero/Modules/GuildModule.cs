using Discord.Commands;
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
            ////characters
            //var matchedCharacter =  IResolver.Current.CharacterSettings.Get(characterAlias);
            //string commandCharacter = characterAlias;
            //if (matchedCharacter != null)
            //{
            //    commandCharacter = matchedCharacter.Command;
            //}
            //var fullCharacterName = matchedCharacter != null ? matchedCharacter.Name ?? characterAlias : characterAlias;


            ////guilds
            //string fullGuildName = "";
            //string guildCommand = "";

            //var boolISInt = int.TryParse(guildAlias,out int guildID);
            //if(boolISInt)
            //{
            //    guildCommand = guildID.ToString();
            //    fullGuildName = string.Format("id:{0}",guildID.ToString());
            //}
            //else
            //{
            //    var matchedGuild = IResolver.Current.GuildSettings.Get(guildAlias);                
            //    if (matchedGuild != null)
            //    {
            //        guildAlias = matchedGuild.SWGoHId;
            //    }

            //    fullGuildName = matchedGuild != null ? matchedGuild.Name ?? guildAlias : guildAlias;
            //    guildCommand = matchedGuild != null ? matchedGuild.SWGoHId ?? guildAlias : guildAlias;
            //}

            //int.TryParse(guildCommand, out int guildId);

            //var res = IResolver.Current.SWGoHRepository.GetGuildCharacter(guildId, commandCharacter).Result;
            ////var res =_SWGoHRepository.GetGuild(guildName, characterName).Result;

            //string retStr = "";
            //if (res!=null)
            //{                
            //    await ReplyAsync($"***Guild : {fullGuildName} - Character : {fullCharacterName}***");                

            //    foreach (var player in res.Players.OrderByDescending(p => p.Rarity).ThenByDescending(t => t.Power))
            //    {
            //        retStr += "\n";
            //        retStr += string.Format("{3}* - {2} - {1} : {0}", player.Name, player.Level, player.Power.ToString().Length < 5 ? string.Concat(player.Power.ToString(), " ") : player.Power.ToString(), player.Rarity);
            //    }

            //    await ReplyAsync($"{retStr}");
            //}
            //else
            //{
                
            //    retStr = $"I didn't find any players having `{fullCharacterName} for guild {fullGuildName}`";
            //    await ReplyAsync($"{retStr}");
            //}

        }


        [Command("slackers")]
        [Summary("Get all players of guild with low level characters.\nUsage : ***$slackers {guildAlias or guildId}***")]
        public async Task GetSlackers(string guildAlias)
        {
            ////guilds
            //string fullGuildName = "";
            //string guildCommand = "";

            //var boolISInt = int.TryParse(guildAlias, out int guildID);
            //if (boolISInt)
            //{
            //    guildCommand = guildID.ToString();
            //    fullGuildName = string.Format("id:{0}", guildID.ToString());
            //}
            //else
            //{
            //    var matchedGuild = IResolver.Current.GuildSettings.Get(guildAlias);
            //    if (matchedGuild != null)
            //    {
            //        guildAlias = matchedGuild.SWGoHId;
            //    }

            //    fullGuildName = matchedGuild != null ? matchedGuild.Name ?? guildAlias : guildAlias;
            //    guildCommand = matchedGuild != null ? matchedGuild.SWGoHId ?? guildAlias : guildAlias;
            //}

            //int.TryParse(guildCommand, out int guildId);

            //var res = IResolver.Current.SWGoHRepository.GetGuildCharacters(guildId).Result;

            //string retStr = "";

            //int counter = 1;
            //int maxCounter = 25;
            //int maxCounter1 = 0;

            //try
            //{
            //    for (int level = 1; level < 100; level++)
            //    {                   
            //        foreach (var guildCharacter in res)
            //        {
            //            foreach (var player in guildCharacter.Players)
            //            {
            //                if (player.Level == level)
            //                {
            //                    retStr += "\n";
            //                    retStr += string.Format("{0} - {1} - level:{2}", player.Name, guildCharacter.Name, player.Level);
            //                    counter += 1;
            //                    if (counter > maxCounter) break;
            //                    //Console.WriteLine(maxCounter1.ToString());
            //                }
            //            }
            //        }
            //        if (counter > maxCounter) break;
            //        //Console.WriteLine("level" + level);
            //    }
            //}
            //catch(Exception ex)
            //{
            //    Consoler.WriteLineInColor(string.Format("Slackers say : {0}", ex.Message), ConsoleColor.Red);
            //}
            

            //await ReplyAsync($"{retStr}");          

        }


        [Command("guildPlayers")]
        [Summary("Get available players in specified guild.\nUsage : ***$guildPlayers {guildAlias or guildId}***")]
        public async Task GetGuildPlayers(string guildAlias)
        {
            var guildConfig = IResolver.Current.GuildsConfig.GetGuildConfig(guildAlias).Result;

            var result = IResolver.Current.MongoDBRepository.GetGuildPlayers(guildConfig.Name).Result;

            string retStr = string.Format("\n These are the players of guild **{0}**\n", guildConfig.Name);
            foreach (var player in result.Players)
            {
                retStr += $"\n{player.PlayerName}";
            }

            await ReplyAsync($"{retStr}");

            //// guilds
            //string fullGuildName = "";
            //string guildCommand = "";

            //var boolISInt = int.TryParse(guildAlias, out int guildID);
            //if (boolISInt)
            //{
            //    guildCommand = guildID.ToString();
            //    fullGuildName = string.Format("id:{0}", guildID.ToString());
            //}
            //else
            //{
            //    var matchedGuild = IResolver.Current.GuildSettings.Get(guildAlias);
            //    if (matchedGuild != null)
            //    {
            //        guildAlias = matchedGuild.SWGoHId;
            //    }

            //    fullGuildName = matchedGuild != null ? matchedGuild.Name ?? guildAlias : guildAlias;
            //    guildCommand = matchedGuild != null ? matchedGuild.SWGoHId ?? guildAlias : guildAlias;
            //}

            //var result = IResolver.Current.SWGoHRepository.GetGuildPlayers(fullGuildName).Result;

            //string retStr = "";
            //foreach(var row in result)
            //{
            //    retStr += $"\n{row}";
            //}

            //await ReplyAsync($"{retStr}");
        }


        [Command("playerReport")]
        [Summary("Get full report for a player. You can check available players of a guild by using ***$guildPlayers*** command.\nUsage : ***$playerReport {playerUserName}***")]
        public async Task CheckPlayer(string playerUserName)
        {
            string loadingStr = string.Format("\n**{0}** is loading...\n\n", playerUserName);

            await ReplyAsync($"{loadingStr}");
            //fil data
            var playerData = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName).Result;

            if(playerData==null)
            {
                await ReplyAsync($"I couldn't find data for player with name : ***{playerUserName}***.");
                return;
            }

            string retStr = string.Format("Last update : {0}(UTC)\n\n", playerData.LastSwGohUpdated.ToString("yyyy-MM-dd HH:mm:ss")) ;

            var notActivatedChars = playerData.Characters.Where(p => p.Level == 0).ToList();

            //stars
            var chars1star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 1).ToList();
            var chars2star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 2).ToList();
            var chars3star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 3).ToList();
            var chars4star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 4).ToList();
            var chars5star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 5).ToList();
            var chars6star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 6).ToList();
            var chars7star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 7).ToList();

            //abilities
            var _allAbilities = (from _Character in playerData.Characters.Where(p => p.Abilities != null)
                            from _Abilities in _Character.Abilities
                            select new
                            {
                                _Character.Name,
                                _Abilities
                            }
                            ).ToList();
            var missingAbilitiesTop10 = _allAbilities.GroupBy(d => d.Name)
                        .Select(
                            g => new
                            {
                                Key = g.Key,                                
                                SumLevels = g.Sum(s => s._Abilities.Level),
                                SumMaxLevels = g.Sum(s => s._Abilities.MaxLevel),
                                MissingLevels = g.Sum(s => s._Abilities.MaxLevel)- g.Sum(s => s._Abilities.Level)
                            }).OrderByDescending(p=>p.MissingLevels).Take(10);

            //level
            var charsLessThan50Level = playerData.Characters.Where(p => p.Level > 0 && p.Level < 50).ToList();
            var chars50_59Level = playerData.Characters.Where(p => p.Level >= 50 && p.Level < 60).ToList();
            var chars60_69Level = playerData.Characters.Where(p => p.Level >= 60 && p.Level < 70).ToList();
            var chars70_79Level = playerData.Characters.Where(p => p.Level >= 70 && p.Level < 80).ToList();
            var chars80_84Level = playerData.Characters.Where(p => p.Level >= 80 && p.Level < 85).ToList();
            var chars85Level = playerData.Characters.Where(p => p.Level == 85).ToList();

            //number of mods
            var noMods = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Mods == null || p.Mods.Count == 0).ToList();
            var oneMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 1).ToList();
            var twoMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 2).ToList();
            var threeMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 3).ToList();
            var fourMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 4).ToList();
            var fiveMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 5).ToList();
            var sixMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 6).ToList();


            //mods level
            var _allMods = (from _Character in playerData.Characters.Where(p => p.Mods != null)
                            from _Mods in _Character.Mods
                            select new
                            {
                                _Character.Name,
                                _Mods
                            }
                            ).ToList();            
            var modsLevelLessThan9 = _allMods.Where(p => p._Mods.Level < 9).ToList();
            var modsLevel9_12 = _allMods.Where(p => p._Mods.Level >= 9 && p._Mods.Level <= 12).ToList();
            var modsLevel13_15 = _allMods.Where(p => p._Mods.Level >= 13 && p._Mods.Level <= 15).ToList();

            //gear
            var gear5orLess = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear <= 5).ToList();
            var gear6_8 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear >= 6 && p.Gear <= 8).ToList();
            var gear9_10 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear >= 9 && p.Gear <= 10).ToList();
            var gear11 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear == 11).ToList();
            var gear12 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear == 12).ToList();            


            //build post string
            retStr += string.Format("{0} characters **not activated** (from total characters : {1})\n", notActivatedChars.Count(), playerData.Characters.Count());


            retStr += string.Format("Total GP: **{0}**\n", playerData.GPcharacters+ playerData.GPships);
            retStr += string.Format("Toons GP: **{0}**\n", playerData.GPcharacters);
            retStr += string.Format("Ships GP: **{0}**\n", playerData.GPships);

            retStr += "\n**Stars**\n";
            retStr += string.Format("{0} characters at **1***\n", chars1star.Count());
            retStr += string.Format("{0} characters at **2***\n", chars2star.Count());
            retStr += string.Format("{0} characters at **3***\n", chars3star.Count());
            retStr += string.Format("{0} characters at **4***\n", chars4star.Count());
            retStr += string.Format("{0} characters at **5***\n", chars5star.Count());
            retStr += string.Format("{0} characters at **6***\n", chars6star.Count());
            retStr += string.Format("{0} characters at **7***\n", chars7star.Count());

            retStr += "\n**Abilities**\n";
            foreach(var character in missingAbilitiesTop10)
            {
                retStr += string.Format("{0} is missing **{1} abilities**\n", character.Key, character.MissingLevels);
            }

            retStr += "\n**Levels**\n";
            retStr += string.Format("{0} characters with **lvl<50**\n", charsLessThan50Level.Count());
            retStr += string.Format("{0} characters with **lvl 50-59**\n", chars50_59Level.Count());
            retStr += string.Format("{0} characters with **lvl 60-69**\n", chars60_69Level.Count());
            retStr += string.Format("{0} characters with **lvl 70-79**\n", chars70_79Level.Count());
            retStr += string.Format("{0} characters with **lvl 80-84**\n", chars80_84Level.Count());
            retStr += string.Format("{0} characters with **lvl 85**\n", chars85Level.Count());

            retStr += "\n**Mods**\n";
            retStr += string.Format("{0} activated characters with **no mods**\n", noMods.Count());
            retStr += string.Format("{0} characters with **1 mod**\n", oneMod.Count());
            retStr += string.Format("{0} characters with **2 mods**\n", twoMod.Count());
            retStr += string.Format("{0} characters with **3 mods**\n", threeMod.Count());
            retStr += string.Format("{0} characters with **4 mods**\n", fourMod.Count());
            retStr += string.Format("{0} characters with **5 mods**\n", fiveMod.Count());
            retStr += string.Format("{0} characters with **6 mods**\n", sixMod.Count());

            retStr += "\n**Mods Level**\n";
            retStr += string.Format("{0} mods at **level <9**\n", modsLevelLessThan9.Count());
            retStr += string.Format("{0} mods at **level 9-12**\n", modsLevel9_12.Count());
            retStr += string.Format("{0} mods at **level 13-15**\n", modsLevel13_15.Count());

            retStr += "\n**Gear**\n";
            retStr += string.Format("{0} characters with **gear 5 or less**\n", gear5orLess.Count());
            retStr += string.Format("{0} characters with **gear 6-8**\n", gear6_8.Count());
            retStr += string.Format("{0} characters with **gear 9-10**\n", gear9_10.Count());
            retStr += string.Format("{0} characters with **gear 11**\n", gear11.Count());
            retStr += string.Format("{0} characters with **gear 12**\n", gear12.Count());

            await ReplyAsync($"{retStr}");
        }
    }
}
