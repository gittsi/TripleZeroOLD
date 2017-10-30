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
    public class PlayerModule : ModuleBase<SocketCommandContext>
    {        

        [Command("pl mods")]
        [Summary("Get pl")]
        public async Task GetMods(string username,string sortBy)
        {
            var res = IResolver.Current.SWGoHRepository.GetPlayer(username).Result;

            var mods = (from x in res.Characters.Take(5)
                        from y in x.Mods
                        from z in y.SecondaryStat.Where(p => p.StatType == ModStatType.Speed).OrderByDescending(t=>t.Value)
                        select y).ToList();

           // var modsSorted= mods.OrderByDescending(p=>p.SecondaryStat)

            var webinarsWithOrderedSessions = (from x in res.Characters.Take(5)
                                               from y in x.Mods
                                               from s in y.SecondaryStat
                                               orderby s.Value
                                               where s.StatType== ModStatType.Speed
                                               select x).ToList();

            var asfsda = res.Characters.OrderByDescending(p => p.Mods.OrderByDescending(t => t.Level)).Take(50).ToList();
            var aaaa = res.Characters.OrderByDescending(p => p.Mods.OrderByDescending(t => t.SecondaryStat.OrderByDescending(g => g.StatType == ModStatType.Speed))).Take(50).ToList();



            await ReplyAsync($"{sortBy}");
            
        }
    }
}
