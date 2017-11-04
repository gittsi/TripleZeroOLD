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
    [Name("Character")]
    [Summary("Do some character test I guess")]
    public class CharacterModule : ModuleBase<SocketCommandContext>
    {        

        [Command("characterstats")]
        [Summary("Get character stats for specific player.\nUsage : ***$characterstats {playerUserName} {characterAlias}***")]
        public async Task GetCharacterStats(string playerUserName, string characterAlias)
        {
            //string loadingStr = string.Format("\n**{0}** is loading...\n\n", playerUserName);

            //await ReplyAsync($"{loadingStr}");
            ////fil data
            //var playerData = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName).Result;

            //if (playerData == null)
            //{
            //    await ReplyAsync($"I couldn't find data for player with name : ***{playerUserName}***.");
            //    return;
            //}

            //var matchedCharacter = IResolver.Current.CharacterSettings.Get(characterAlias);

            //string commandCharacter = characterAlias;
            //if (matchedCharacter != null)
            //{
            //    commandCharacter = matchedCharacter.SWGoHUrl;
            //}
            //var fullCharacterName = matchedCharacter != null ? matchedCharacter.Name ?? characterAlias : characterAlias;

            //var character = playerData.Characters.Where(p => p.Name.ToLower() == fullCharacterName.ToLower()).FirstOrDefault();

            //if(character==null)
            //{
            //    await ReplyAsync($"I couldn't find data for character : ***{fullCharacterName}*** for player : ***{playerUserName}***.");
            //    return;
            //}

            //string retStr = "";
            //retStr += string.Format("\n{0} - {1}* g{2} lvl:{3}", character.Name,character.Stars,character.Gear,character.Level);
            //retStr += "\n\n**General**";
            //retStr += $"\nProtection : {character.Protection}";
            //retStr += $"\nHealth : {character.Health}";
            //retStr += $"\nSpeed : {character.Speed}";
            //retStr += $"\nHealth Steal : {character.HealthSteal} %";
            //retStr += $"\nCritical Damage : {character.CriticalDamage} %";
            //retStr += $"\nPotency : {character.Potency} %";
            //retStr += $"\nTenacity : {character.Tenacity} %";

            //retStr += "\n\n**Physical Offense**";
            //retStr += $"\nPhysical Damage : {character.PhysicalDamage}";
            //retStr += $"\nPhysical Critical Chance: {character.PhysicalCriticalChance}";
            //retStr += $"\nPhysical Accuracy: {character.PhysicalAccuracy} %";
            //retStr += $"\nArmor Penetration: {character.ArmorPenetration} %";

            //await ReplyAsync($"{retStr}");
        }

        //public async Task GetCharacterStats(string playerUserName, string characterAlias)
        //{
        //    //characters
        //    var matchedCharacter =  IResolver.Current.CharacterSettings.Get(characterAlias);
        //    string commandCharacter = characterAlias;
        //    if (matchedCharacter != null)
        //    {
        //        commandCharacter = matchedCharacter.SWGoHUrl;
        //    }
        //    var fullCharacterName = matchedCharacter != null ? matchedCharacter.Name ?? characterAlias : characterAlias;


        //    CharacterDto character = new CharacterDto
        //    {
        //        Name = fullCharacterName
        //    };
        //    character = IResolver.Current.SWGoHRepository.GetCharacter(playerUserName, commandCharacter).Result;


        //    string retStr = "";
        //    if (character!=null)
        //    {
        //        await ReplyAsync($"***User : {playerUserName} - Character : {fullCharacterName}***");

        //        retStr += string.Format("\nProtection : {0}", character.Protection);
        //        retStr += string.Format("\nHealth : {0}", character.Health);

        //        await ReplyAsync($"{retStr}");
        //    }
        //    else
        //    {

        //        retStr = $"I didn't find `{playerUserName} having {fullCharacterName}`";
        //        await ReplyAsync($"{retStr}");
        //    }            
        //}
    }
}
