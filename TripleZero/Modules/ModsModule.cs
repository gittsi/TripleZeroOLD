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
using TripleZero.Repository.Dto;
using SwGoh;

namespace TripleZero.Modules
{
    [Name("Player")]
    [Summary("Do some player test I guess")]
    public class ModsModule : ModuleBase<SocketCommandContext>
    {
        private async void SendModReply(string playerUserName, ModStatType modStatType, ModValueType secondaryStatValueType ,List<Tuple<string, Mod>> result)
        {
            string retStr = "";
            if (result != null)
            {
                //await ReplyAsync($"***Guild : {fullGuildName} - Character : {fullCharacterName}***");

                foreach (var row in result)
                {
                    var modStats = row.Item2.SecondaryStat.Where(p => p.StatType == modStatType && p.ValueType==secondaryStatValueType ).FirstOrDefault();
                    var newString = string.Format("{3}: **{2}{4}** {1}{0}", row.Item1.PadRight(25), row.Item2.Type.ToString().PadRight(10), modStats.Value.ToString().PadRight(2), modStatType.ToString(), secondaryStatValueType== ModValueType.Percentage ? "%" : "");

                    if (retStr.Length + newString.Length > 2000)
                        break;
                    retStr += "\n";
                    retStr += newString;

                }

                if(retStr.Length>0)                
                    await ReplyAsync($"{retStr}");
                else
                    await ReplyAsync($"No mods found!");

            }
            else
            {

                retStr = $"I didn't find any mods for username {playerUserName}`";
                    await ReplyAsync($"{retStr}");
            }
        }

        private async Task<List<Tuple<string, Mod>>> GetSpecificMods(string playerUserName,ModStatType modStatType, ModValueType modValueType, int rows =20)
        {
            var res = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName).Result;


            if (res == null)
            {
                await ReplyAsync($"I couldn't find player : {playerUserName}...");
                return null;
            }

            var sortedMods = (from Character in res.Characters.Where(p => p.Mods != null)
                              from Mod in Character.Mods.Where(p => p.SecondaryStat != null)
                              from Stats in Mod.SecondaryStat.Where(p => p.StatType == modStatType && p.ValueType == modValueType)
                              select new
                              {
                                  Character.Name,
                                  Mod
                              }
                        ).OrderByDescending(t => t.Mod.SecondaryStat.Where(p => p.StatType == modStatType && p.ValueType== modValueType).FirstOrDefault().Value).Take(rows).ToList();

            return sortedMods.Select(x => new Tuple<string, Mod>(x.Name, x.Mod)).ToList();
        }

        [Command("mods -s")]
        [Summary("Get mods sorted by a secondary stat of a given player.\nUsage : ***mods {playerUserName} {modType(add *%* if you want percentage)} { {rows(optional)}***\n examples \n1) $mods playerName defense \n2) $mods playerName defense% 5)")]
        public async Task GetSpeedMods(string playerUserName, string modType, int rows = 20)
        {
            ModStatType secondaryStatType=ModStatType.None;
            ModValueType secondaryStatValueType = ModValueType.None;

            if (modType.Substring(modType.Length - 1, 1) == "%")
            {
                secondaryStatValueType = ModValueType.Percentage;
                modType=modType.Replace("%", "");
            }else
            {
                secondaryStatValueType = ModValueType.Flat;
            }

            if(modType.ToLower() == "speed") secondaryStatType = ModStatType.Speed;
            if(modType.ToLower() == "potency") secondaryStatType = ModStatType.Potency;
            if(modType.ToLower() == "accuracy") secondaryStatType = ModStatType.Accuracy;
            if(modType.ToLower() == "criticalavoidance") secondaryStatType = ModStatType.CriticalAvoidance;
            if(modType.ToLower() == "criticalchance") secondaryStatType = ModStatType.CriticalChance;
            if(modType.ToLower() == "criticaldamage") secondaryStatType = ModStatType.CriticalDamage;
            if(modType.ToLower() == "defense") secondaryStatType = ModStatType.Defense;
            if(modType.ToLower() == "health") secondaryStatType = ModStatType.Health;
            if(modType.ToLower() == "offense") secondaryStatType = ModStatType.Offense;
            if(modType.ToLower() == "protection") secondaryStatType = ModStatType.Protection;
            if(modType.ToLower() == "tenacity") secondaryStatType = ModStatType.Tenacity;

            if (secondaryStatType == ModStatType.None)
            {
                await ReplyAsync($"Something is wrong with your command...");
                return;
            }

            var result =await GetSpecificMods(playerUserName, secondaryStatType, secondaryStatValueType, rows);
            SendModReply(playerUserName, secondaryStatType, secondaryStatValueType, result);
        }

        //[Command("mods -potency")]
        //[Summary("Get mods sorted with potency secondary of a given player.\nUsage : ***mods -speed {playerUserName} {rows(optional)}***")]
        //public async Task GetSpeedMods(string playerUserName, int rows = 20)
        //{
        //    var result = GetSpecificMods(playerUserName, ModStatType.Speed, rows);
        //    SendModReply(playerUserName, ModStatType.Speed, result);
        //}


    }
}
