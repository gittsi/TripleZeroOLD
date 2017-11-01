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
using TripleZero.Repository.Dto;
using SwGoh;

namespace TripleZero.Modules
{
    [Name("Player")]
    [Summary("Do some player test I guess")]
    public class ModsModule : ModuleBase<SocketCommandContext>
    {

        [Command("mods -check")]
        [Summary("Get mods with speed secondary")]
        public async Task CheckSlacker(string username)
        {
            var res = IResolver.Current.SWGoHRepository.GetPlayer(username).Result;


            var noMods = (from Character in res.Characters.Where(p => p.Mods == null)
                              select Character).ToList();

            var oneMod = (from Character in res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 1)
                         select Character).ToList();

            var twoMod = (from Character in res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 2)
                          select Character).ToList();

            var threeMod = (from Character in res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 3)
                          select Character).ToList();

            var fourMod = (from Character in res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 4)
                          select Character).ToList();

            var fiveMod = (from Character in res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 5)
                          select Character).ToList();

            var sixMod = (from Character in res.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 6)
                           select Character).ToList();

            string retStr = string.Format("Checking mods for player : {0}\n", username) ;
            retStr += string.Format("{0} characters with no mods\n", noMods.Count());            
            retStr += string.Format("{0} characters with 1 mod\n", oneMod.Count());            
            retStr += string.Format("{0} characters with 2 mods\n", twoMod.Count());            
            retStr += string.Format("{0} characters with 3 mods\n", threeMod.Count());            
            retStr += string.Format("{0} characters with 4 mods\n", fourMod.Count());            
            retStr += string.Format("{0} characters with 5 mods\n", fiveMod.Count());            
            retStr += string.Format("{0} characters with 6 mods\n", sixMod.Count());            

            await ReplyAsync($"{retStr}");
        }

        [Command("mods -speed")]
        [Summary("Get mods with speed secondary")]
        public async Task GetMods(string username,int topRows=1000)
        {
            var res = IResolver.Current.SWGoHRepository.GetPlayer(username).Result;            

            var sortedMods = (from Character in res.Characters.Where(p=>p.Mods!=null)
                        from Mod in Character.Mods.Where(p=>p.SecondaryStat!=null)
                        from Stats in Mod.SecondaryStat.Where(p=>p.StatType== ModStatType.Speed)
                        select new
                        {
                            Character.Name,
                            Mod
                        }
                        ).OrderByDescending(t=>t.Mod.SecondaryStat.Where(p=>p.StatType==ModStatType.Speed).FirstOrDefault().Value).Take(topRows).ToList();

            string retStr = "";
            if (res != null)
            {
                //await ReplyAsync($"***Guild : {fullGuildName} - Character : {fullCharacterName}***");

                foreach (var row in sortedMods)
                {
                    var speedModStats = row.Mod.SecondaryStat.Where(p => p.StatType == ModStatType.Speed).FirstOrDefault();
                    retStr += "\n";
                    retStr += string.Format("{0} :  {1} {2} speed:{3}", row.Name, row.Mod.Type.ToString(), row.Mod.Name , speedModStats.Value) ;
                }

                await ReplyAsync($"{retStr}");
            }
            else
            {

                retStr = $"I didn't find any mods for username {username}`";
                await ReplyAsync($"{retStr}");
            }            
            
        }
    }
}
