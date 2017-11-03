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

        

        [Command("mods -speed")]
        [Summary("Get mods sorted with speed secondary of a given player.\nUsage : ***mods -speed {playerUserName} {rows(optional)}***")]
        public async Task GetSpeedMods(string playerUserName, int rows = 20)
        {
            var res = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName).Result;

            var sortedMods = (from Character in res.Characters.Where(p => p.Mods != null)
                              from Mod in Character.Mods.Where(p => p.SecondaryStat != null)
                              from Stats in Mod.SecondaryStat.Where(p => p.StatType == ModStatType.Speed)
                              select new
                              {
                                  Character.Name,
                                  Mod
                              }
                        ).OrderByDescending(t => t.Mod.SecondaryStat.Where(p => p.StatType == ModStatType.Speed).FirstOrDefault().Value).Take(rows).ToList();

            string retStr = "";
            if (res != null)
            {
                //await ReplyAsync($"***Guild : {fullGuildName} - Character : {fullCharacterName}***");

                foreach (var row in sortedMods)
                {
                    var speedModStats = row.Mod.SecondaryStat.Where(p => p.StatType == ModStatType.Speed).FirstOrDefault();
                    var newString = string.Format("Speed:{2} {1} {0}", row.Name.PadRight(25), row.Mod.Type.ToString().PadRight(10), speedModStats.Value.ToString().PadRight(2));

                    if (retStr.Length + newString.Length > 2000)
                        break;
                    retStr += "\n";
                    retStr += newString;
                    
                }

                await ReplyAsync($"{retStr}");
            }
            else
            {

                retStr = $"I didn't find any mods for username {playerUserName}`";
                await ReplyAsync($"{retStr}");
            }

        }


    }
}
