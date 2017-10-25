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

namespace TripleZero.Modules
{
    [Name("Guild")]
    [Summary("Do some guild test I guess")]
    public class GuildModule : ModuleBase<SocketCommandContext>
    {
        //private readonly SWGoHRepository _SWGoHRepository;

        //public GuildModule(SWGoHRepository SWGoHRepository)
        //{
        //    _SWGoHRepository = SWGoHRepository;
        //}

        [Command("guild")]
        [Summary("Get guild")]
        public async Task Say(string guildName , string characterName)
        {

            await ReplyAsync($"You asked for character `{characterName} for guild {guildName}`. \n\n");

            //var url = string.Format("https://swgoh.gg/api/guilds/{0}/units/", guildName);
            //List <GuildCharacter> chars = new List<GuildCharacter>();
            //using (var client = new HttpClient())
            //{
            //    HttpResponseMessage response2 = await client.GetAsync(url);
            //    HttpContent content = response2.Content;
            //    string reqResult = await content.ReadAsStringAsync();

            //    JObject json = JObject.Parse(reqResult);



            //    foreach (var row in json)
            //    {
            //        GuildCharacter gc = new GuildCharacter();
            //        gc.Character = new Character() { Name = row.Key };

            //        List<PlayerCharacter> players = new List<PlayerCharacter>();
            //        foreach (var player in row.Value)
            //        {
            //            players.Add(new PlayerCharacter() { Name = player["player"].ToString(), Stats = new CharacterStats() { Level = (int)player["level"], Power = (int)player["power"], Rarity = (int)player["rarity"] } });
            //            gc.Players = players;
            //        }
            //        chars.Add(gc);
            //    }
            //}
            //var res = chars.Where(p => p.Character.Name.ToLower() == characterName.ToLower());


            var res = IResolver.Current.SWGoHRepository.GetGuild(guildName, characterName).Result;
            //var res =_SWGoHRepository.GetGuild(guildName, characterName).Result;


            string str1 = res.FirstOrDefault().Name;
            await ReplyAsync($"{str1}");
            string str2 = "";

            foreach (var player in res.FirstOrDefault().Players.OrderByDescending(p => p.Rarity).ThenByDescending(t=>t.Power))
            {
                str2 += "\n";
                str2 += string.Format("{3}* - {2} - {1} : {0}", player.Name, player.Level, player.Power.ToString().Length < 5 ? string.Concat(player.Power.ToString(), " ") : player.Power.ToString(), player.Rarity);


            }
            

            await ReplyAsync($"{str2}");




        }
    }
}
