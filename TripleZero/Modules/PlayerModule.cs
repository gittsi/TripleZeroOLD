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
    [Summary("Player Commands")]
    public class PlayerModule : ModuleBase<SocketCommandContext>
    {        

        [Command("playerreload")]
        [Summary("Set a player for reload.\nUsage : ***$playerreload {playerUserName}***")]
        public async Task SetPlayerReload(string playerUserName)
        {
            var result = IResolver.Current.MongoDBRepository.SendPlayerToQueue(playerUserName).Result;

            string retStr = "";
            if (result != null)
                retStr = string.Format("\nPlayer {0} added to queue. Please be patient, I need some time to retrieve data!!!",playerUserName);
            else
                retStr = string.Format("\nPlayer {0} not added to queue!!!!!");

            await ReplyAsync($"{retStr}");
        }

        [Command("characterstats -c")]
        [Summary("Compares character stats for 2 specific players.\nUsage : ***$characterstats -c {player1UserName} {player2UserName} {characterAlias}***")]
        public async Task GetCharacterStatsCompare(string player1UserName, string player2UserName, string characterAlias)
        {
            string loadingStr = string.Format("\n**{0} and {1}** are loading...\n\n", player1UserName, player2UserName);

            await ReplyAsync($"{loadingStr}");

            var player1Data = IResolver.Current.MongoDBRepository.GetPlayer(player1UserName).Result;
            if (player1Data == null)
            {
                await ReplyAsync($"I couldn't find data for player with name : ***{player1UserName}***.");
                return;
            }

            var player2Data = IResolver.Current.MongoDBRepository.GetPlayer(player2UserName).Result;
            if (player2Data == null)
            {
                await ReplyAsync($"I couldn't find data for player with name : ***{player2UserName}***.");
                return;
            }

            var characterConfig = IResolver.Current.CharacterConfig.GetCharacterConfigByAlias(characterAlias).Result;

            var character1 = player1Data.Characters.Where(p => p.Name.ToLower() == characterConfig.Name.ToLower()).FirstOrDefault();
            if (character1 == null)
            {
                await ReplyAsync($"I couldn't find data for character : ***{characterConfig.Name}*** for player : ***{player1UserName}***.");
                return;
            }

            var character2 = player2Data.Characters.Where(p => p.Name.ToLower() == characterConfig.Name.ToLower()).FirstOrDefault();
            if (character2 == null)
            {
                await ReplyAsync($"I couldn't find data for character : ***{characterConfig.Name}*** for player : ***{player2UserName}***.");
                return;
            }

            string retStr = "";
            retStr += string.Format("\n{0} - {1}* g{2} lvl:{3} - {4}* g{5} lvl:{6}  ", character1.Name, character1.Stars, character1.Gear, character1.Level, character2.Stars, character2.Gear, character2.Level);
            retStr += string.Format("\nPower {0} vs {2} - StatPower {1} vs {3}", character1.Power, character1.StatPower, character2.Power, character2.StatPower);

            var strAbilities = "\n\n**Abilities**";
            for(int i=0;i<character1.Abilities.Count();i++)
            {
                strAbilities += string.Format("\n{0} {1}/{2} vs {3}/{4}", character1.Abilities[i].Name, character1.Abilities[i].Level, character1.Abilities[i].MaxLevel, character2.Abilities[i].Level, character2.Abilities[i].MaxLevel);
            }
            retStr += strAbilities;

            //string strAbilities1 = "";
            //foreach (var ability1 in character1.Abilities)
            //{
            //    strAbilities1 += string.Format("{0}/{1} ", ability1.Level.ToString(), ability1.MaxLevel.ToString());
            //}
            //string strAbilities2 = "";
            //foreach (var ability2 in character2.Abilities)
            //{
            //    strAbilities2 += string.Format("{0}/{1} ", ability2.Level.ToString(), ability2.MaxLevel.ToString());
            //}
            //retStr += string.Format("\n{0} vs {1}", strAbilities1, strAbilities2);

            

            retStr += "\n\n**General**";
            retStr += $"\nProtection: {character1.Protection} - {character2.Protection}";
            retStr += $"\nHealth: {character1.Health} - {character2.Health}";
            retStr += $"\nSpeed: {character1.Speed} - {character2.Speed}";
            retStr += $"\nHealth Steal: {character1.HealthSteal} % - {character2.HealthSteal} %";
            retStr += $"\nCritical Damage: {character1.CriticalDamage} %";
            retStr += $"\nPotency: {character1.Potency} % - {character2.Potency} %";
            retStr += $"\nTenacity: {character1.Tenacity} % - {character2.Tenacity} %";

            retStr += "\n\n**Physical Offense**";
            retStr += $"\nPhysical Damage: {character1.PhysicalDamage} - {character2.PhysicalDamage}";
            retStr += $"\nPhysical Critical Chance: {character1.PhysicalCriticalChance} % - {character2.PhysicalCriticalChance} %";
            retStr += $"\nPhysical Accuracy: {character1.PhysicalAccuracy} % - {character2.PhysicalAccuracy} %";
            retStr += $"\nArmor Penetration: {character1.ArmorPenetration} - {character2.ArmorPenetration}";

            retStr += "\n\n**Special Offense**";
            retStr += $"\nSpecial Damage: {character1.SpecialDamage} - {character2.SpecialDamage}";
            retStr += $"\nSpecial Critical Chance: {character1.SpecialCriticalChance} % - {character2.SpecialCriticalChance} %";
            retStr += $"\nSpecial Accuracy: {character1.SpecialAccuracy} % - {character2.SpecialAccuracy} %";

            retStr += "\n\n**Physical Survivability**";
            retStr += $"\nArmor: {character1.Armor} % - {character2.Armor} %";
            retStr += $"\nDodge Chance: {character1.DodgeChance} % - {character2.DodgeChance} %";
            retStr += $"\nPhysical Critical Avoidance: {character1.PhysicalCriticalAvoidance} % - {character2.PhysicalCriticalAvoidance} %";

            retStr += "\n\n**Special Survivability**";
            retStr += $"\nResistance: {character1.Resistance} % - {character2.Resistance} %";
            retStr += $"\nDeflection Chance: {character1.DeflectionChance} % - {character2.DeflectionChance} %";
            retStr += $"\nSpecial Critical Avoidance: {character1.SpecialCriticalAvoidance} % - {character2.SpecialCriticalAvoidance} %";



            await ReplyAsync($"{retStr}");
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
