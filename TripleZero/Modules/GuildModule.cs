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
    [Summary("Do some guild test I guess")]
    public class GuildModule : ModuleBase<SocketCommandContext>
    {        

        [Command("guild")]
        [Summary("Get guild")]
        public async Task Say(string guildName , string characterName)
        {
            //characters
            var matchedCharacter =  IResolver.Current.CharacterSettings.Get(characterName);
            string commandCharacter = characterName;
            if (matchedCharacter != null)
            {
                commandCharacter = matchedCharacter.Command;
            }
            var fullCharacterName = matchedCharacter != null ? matchedCharacter.Name ?? characterName : characterName;


            //guilds
            string fullGuildName = "";
            string guildCommand = "";

            var boolISInt = int.TryParse(guildName,out int guildID);
            if(boolISInt)
            {
                guildCommand = guildID.ToString();
                fullGuildName = string.Format("id:{0}",guildID.ToString());
            }
            else
            {
                var matchedGuild = IResolver.Current.GuildSettings.Get(guildName);                
                if (matchedGuild != null)
                {
                    guildName = matchedGuild.SWGoHId;
                }

                fullGuildName = matchedGuild != null ? matchedGuild.Name ?? guildName : guildName;
                guildCommand = matchedGuild != null ? matchedGuild.SWGoHId ?? guildName : guildName;
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
        [Summary("Get slackers")]
        public async Task Say(string guildName)
        {
            //guilds
            string fullGuildName = "";
            string guildCommand = "";

            var boolISInt = int.TryParse(guildName, out int guildID);
            if (boolISInt)
            {
                guildCommand = guildID.ToString();
                fullGuildName = string.Format("id:{0}", guildID.ToString());
            }
            else
            {
                var matchedGuild = IResolver.Current.GuildSettings.Get(guildName);
                if (matchedGuild != null)
                {
                    guildName = matchedGuild.SWGoHId;
                }

                fullGuildName = matchedGuild != null ? matchedGuild.Name ?? guildName : guildName;
                guildCommand = matchedGuild != null ? matchedGuild.SWGoHId ?? guildName : guildName;
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
                                Console.WriteLine(maxCounter1.ToString());
                            }
                        }
                    }
                    if (counter > maxCounter) break;
                    Console.WriteLine("level" + level);
                }
            }
            catch(Exception ex)
            {
                Consoler.WriteLineInColor(string.Format("Slackers say : {0}", ex.Message), ConsoleColor.Red);
            }
            

            await ReplyAsync($"{retStr}");
            //var res =_SWGoHRepository.GetGuild(guildName, characterName).Result;

            //string retStr = "";
            //if (res.Count > 0)
            //{
            //    await ReplyAsync($"***Guild : {fullGuildName} - Character : {fullCharacterName}***");

            //    foreach (var player in res.FirstOrDefault().Players.OrderByDescending(p => p.Rarity).ThenByDescending(t => t.Power))
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
    }
}
