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

namespace TripleZero.Modules
{
    [Name("Character")]
    [Summary("Do some character test I guess")]
    public class CharacterModule : ModuleBase<SocketCommandContext>
    {        

        [Command("ch")]
        [Summary("Get ch")]
        public async Task Say(string username, string characterName)
        {
            //characters
            var matchedCharacter =  IResolver.Current.CharacterSettings.Get(characterName);
            string commandCharacter = characterName;
            if (matchedCharacter != null)
            {
                commandCharacter = matchedCharacter.SWGoHUrl;
            }
            var fullCharacterName = matchedCharacter != null ? matchedCharacter.Name!=null ? matchedCharacter.Name : characterName : characterName;


            CharacterDto character = new CharacterDto();
            character.Name = fullCharacterName;
            character = IResolver.Current.SWGoHRepository.GetCharacter(username, commandCharacter).Result;


            string retStr = "";
            if (character!=null)
            {
                await ReplyAsync($"***User : {username} - Character : {fullCharacterName}***");
                
                retStr += string.Format("\n Protection : {0}", character.Protection);
                retStr += string.Format("\n Health : {0}", character.Health);


                await ReplyAsync($"{retStr}");
            }
            else
            {

                retStr = $"I didn't find `{username} having {fullCharacterName}`";
                await ReplyAsync($"{retStr}");
            }

//            await ReplyAsync($"{matchedCharacter}");
            
        }
    }
}
