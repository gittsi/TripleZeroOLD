using Discord.Commands;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Repository.SWGoHRepository;
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
        public async Task Say(string guildAlias , string characterAlias)
        {
            //characters
            var matchedCharacter =  IResolver.Current.CharacterSettings.Get(characterAlias);
            string commandCharacter = characterAlias;
            if (matchedCharacter != null)
            {
                commandCharacter = matchedCharacter.Command;
            }
            var fullCharacterName = matchedCharacter != null ? matchedCharacter.Name ?? characterAlias : characterAlias;


            //guilds
            string fullGuildName = "";
            string guildCommand = "";

            var boolISInt = int.TryParse(guildAlias,out int guildID);
            if(boolISInt)
            {
                guildCommand = guildID.ToString();
                fullGuildName = string.Format("id:{0}",guildID.ToString());
            }
            else
            {
                var matchedGuild = IResolver.Current.GuildSettings.Get(guildAlias);                
                if (matchedGuild != null)
                {
                    guildAlias = matchedGuild.SWGoHId;
                }

                fullGuildName = matchedGuild != null ? matchedGuild.Name ?? guildAlias : guildAlias;
                guildCommand = matchedGuild != null ? matchedGuild.SWGoHId ?? guildAlias : guildAlias;
            }

            int.TryParse(guildCommand, out int guildId);

            var res = IResolver.Current.SWGoHRepository.GetGuildCharacter(guildId, commandCharacter).Result;
            //var res =_SWGoHRepository.GetGuild(guildName, characterName).Result;

            string retStr = "";
            if (res!=null)
            {                
                await ReplyAsync($"***Guild : {fullGuildName} - Character : {fullCharacterName}***");                

                foreach (var player in res.Players.OrderByDescending(p => p.Rarity).ThenByDescending(t => t.Power))
                {
                    retStr += "\n";
                    retStr += string.Format("{3}* - {2} - {1} : {0}", player.Name, player.Level, player.Power.ToString().Length < 5 ? string.Concat(player.Power.ToString(), " ") : player.Power.ToString(), player.Rarity);
                }

                await ReplyAsync($"{retStr}");
            }
            else
            {
                
                retStr = $"I didn't find any players having `{fullCharacterName} for guild {fullGuildName}`";
                await ReplyAsync($"{retStr}");
            }

        }


        [Command("slackers")]
        [Summary("Get all players of guild with low level characters.\nUsage : ***$slackers {guildAlias or guildId}***")]
        public async Task GetSlackers(string guildAlias)
        {
            //guilds
            string fullGuildName = "";
            string guildCommand = "";

            var boolISInt = int.TryParse(guildAlias, out int guildID);
            if (boolISInt)
            {
                guildCommand = guildID.ToString();
                fullGuildName = string.Format("id:{0}", guildID.ToString());
            }
            else
            {
                var matchedGuild = IResolver.Current.GuildSettings.Get(guildAlias);
                if (matchedGuild != null)
                {
                    guildAlias = matchedGuild.SWGoHId;
                }

                fullGuildName = matchedGuild != null ? matchedGuild.Name ?? guildAlias : guildAlias;
                guildCommand = matchedGuild != null ? matchedGuild.SWGoHId ?? guildAlias : guildAlias;
            }

            int.TryParse(guildCommand, out int guildId);

            var res = IResolver.Current.SWGoHRepository.GetGuildCharacters(guildId).Result;

            string retStr = "";

            int counter = 1;
            int maxCounter = 25;
            int maxCounter1 = 0;

            try
            {
                for (int level = 1; level < 100; level++)
                {                   
                    foreach (var guildCharacter in res)
                    {
                        foreach (var player in guildCharacter.Players)
                        {
                            if (player.Level == level)
                            {
                                retStr += "\n";
                                retStr += string.Format("{0} - {1} - level:{2}", player.Name, guildCharacter.Name, player.Level);
                                counter += 1;
                                if (counter > maxCounter) break;
                                //Console.WriteLine(maxCounter1.ToString());
                            }
                        }
                    }
                    if (counter > maxCounter) break;
                    //Console.WriteLine("level" + level);
                }
            }
            catch(Exception ex)
            {
                Consoler.WriteLineInColor(string.Format("Slackers say : {0}", ex.Message), ConsoleColor.Red);
            }
            

            await ReplyAsync($"{retStr}");          

        }


        [Command("guildPlayers")]
        [Summary("Get available players in specified guild.\nUsage : ***$guildPlayers {guildAlias or guildId}***")]
        public async Task GetGuildPlayers(string guildAlias)
        {
            // guilds
            string fullGuildName = "";
            string guildCommand = "";

            var boolISInt = int.TryParse(guildAlias, out int guildID);
            if (boolISInt)
            {
                guildCommand = guildID.ToString();
                fullGuildName = string.Format("id:{0}", guildID.ToString());
            }
            else
            {
                var matchedGuild = IResolver.Current.GuildSettings.Get(guildAlias);
                if (matchedGuild != null)
                {
                    guildAlias = matchedGuild.SWGoHId;
                }

                fullGuildName = matchedGuild != null ? matchedGuild.Name ?? guildAlias : guildAlias;
                guildCommand = matchedGuild != null ? matchedGuild.SWGoHId ?? guildAlias : guildAlias;
            }

            var result = IResolver.Current.SWGoHRepository.GetGuildPlayers(fullGuildName).Result;

            string retStr = "";
            foreach(var row in result)
            {
                retStr += $"\n{row}";
            }

            await ReplyAsync($"{retStr}");
        }


        [Command("playerReport")]
        [Summary("Get full report for a player. You can check available players of a guild by using ***$guildPlayers*** command.\nUsage : ***$playerReport {playerUserName}***")]
        public async Task CheckSlacker(string playerUserName)
        {
            string loadingStr = string.Format("\n**{0}** is loading...\n\n", playerUserName);

            await ReplyAsync($"{loadingStr}");

            var res = IResolver.Current.SWGoHRepository.GetPlayer(playerUserName).Result;

            string retStr = string.Format("Last update : {0}(UTC)\n",res.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss")) ;

            var notActivatedChars = res.Characters.Where(p => p.Level == 0).ToList();

            var charsLessThan50Level = res.Characters.Where(p => p.Level > 0 && p.Level < 50).ToList();
            var chars50_59Level = res.Characters.Where(p => p.Level >= 50 && p.Level < 60).ToList();
            var chars60_69Level = res.Characters.Where(p => p.Level >= 60 && p.Level < 70).ToList();
            var chars70_79Level = res.Characters.Where(p => p.Level >= 70 && p.Level < 80).ToList();
            var chars80_84Level = res.Characters.Where(p => p.Level >= 80 && p.Level < 85).ToList();
            var chars85Level = res.Characters.Where(p => p.Level == 85).ToList();

            var noMods = res.Characters.Where(p => p.Level != 0).Where(p => p.Mods == null || p.Mods.Count == 0).ToList();
            var oneMod = res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 1).ToList();
            var twoMod = res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 2).ToList();
            var threeMod = res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 3).ToList();
            var fourMod = res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 4).ToList();
            var fiveMod = res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 5).ToList();
            var sixMod = res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 6).ToList();


            var _allMods = (from _Character in res.Characters.Where(p => p.Mods != null)
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

            var gear5orLess = res.Characters.Where(p => p.Level != 0).Where(p => p.Gear <= 5).ToList();
            var gear6_8 = res.Characters.Where(p => p.Level != 0).Where(p => p.Gear >= 6 && p.Gear <= 8).ToList();
            var gear9_10 = res.Characters.Where(p => p.Level != 0).Where(p => p.Gear >= 9 && p.Gear <= 10).ToList();
            var gear11 = res.Characters.Where(p => p.Level != 0).Where(p => p.Gear == 11).ToList();
            var gear12 = res.Characters.Where(p => p.Level != 0).Where(p => p.Gear == 12).ToList();

            

            retStr += string.Format("{0} characters **not activated** (from total characters : {1})\n", notActivatedChars.Count(), res.Characters.Count());

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
