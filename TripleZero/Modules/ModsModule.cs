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

        [Command("mods -speed")]
        [Summary("Get mods with speed secondary")]
        public async Task GetMods(string username,int topRows=1000)
        {
            var res = IResolver.Current.SWGoHRepository.GetPlayer(username).Result;            

            var mods = (from x in res.Characters.Where(p=>p.Mods!=null)
                        from y in x.Mods.Where(p=>p.SecondaryStat!=null)
                        from z in y.SecondaryStat.Where(p=>p.StatType== ModStatType.Speed)
                        select y).OrderByDescending(t=>t.SecondaryStat.Where(p=>p.StatType==ModStatType.Speed).FirstOrDefault().Value).Take(topRows).ToList();


            var mods2 = (from x in res.Characters.Where(p => p.Mods != null)
                         from y in x.Mods.Where(p => p.SecondaryStat != null)
                         from z in y.SecondaryStat.Where(p => p.StatType == ModStatType.Speed)
                         select x).OrderByDescending(p => p.Mods.Where(t => t.SecondaryStat.Where(n => n.StatType == ModStatType.Speed).FirstOrDefault().Value > 0).Take(topRows)).ToList();
                //t => t.SecondaryStat.Where(p => p.StatType == ModStatType.Speed).FirstOrDefault().Value).Take(topRows).ToList();

            var a = 1;
            


            await ReplyAsync($"{username}");
            
        }
    }
}
