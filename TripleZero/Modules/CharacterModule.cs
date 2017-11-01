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
    [Name("Character")]
    [Summary("Do some character test I guess")]
    public class CharacterModule : ModuleBase<SocketCommandContext>
    {        

        [Command("characterstats")]
        [Summary("Get character stats.\nUsage : ***$characterstats {playerUserName} {characterAlias}***")]
        public async Task Say(string playerUserName, string characterAlias)
        {
            //characters
            var matchedCharacter =  IResolver.Current.CharacterSettings.Get(characterAlias);
            string commandCharacter = characterAlias;
            if (matchedCharacter != null)
            {
                commandCharacter = matchedCharacter.SWGoHUrl;
            }
            var fullCharacterName = matchedCharacter != null ? matchedCharacter.Name ?? characterAlias : characterAlias;


            CharacterDto character = new CharacterDto
            {
                Name = fullCharacterName
            };
            character = IResolver.Current.SWGoHRepository.GetCharacter(playerUserName, commandCharacter).Result;


            string retStr = "";
            if (character!=null)
            {
                await ReplyAsync($"***User : {playerUserName} - Character : {fullCharacterName}***");
                
                retStr += string.Format("\nProtection : {0}", character.Protection);
                retStr += string.Format("\nHealth : {0}", character.Health);

                await ReplyAsync($"{retStr}");
            }
            else
            {

                retStr = $"I didn't find `{playerUserName} having {fullCharacterName}`";
                await ReplyAsync($"{retStr}");
            }            
        }
    }
}
