using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Core.Caching;

namespace TripleZero.Modules
{
    [Name("Character")]
    [Summary("Character Commands")]
    public class CharacterModule : ModuleBase<SocketCommandContext>
    {
        private CacheClient cacheClient = IResolver.Current.CacheClient;

        [Command("character", RunMode = RunMode.Async)]
        [Summary("Get character stats for specific player")]
        [Remarks("*character {playerUserName} {characterAlias}*")]
        [Alias("c")]
        public async Task GetCharacterStats(string playerUserName, string characterAlias)
        {
            playerUserName = playerUserName.Trim();
            characterAlias = characterAlias.Trim();

            string retStr = "";
            //get from cache if possible and exit sub
            string functionName = "character";
            string key = string.Concat(playerUserName,characterAlias);
            retStr = cacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            string loadingStr = string.Format("\n**{0}** is loading...\n\n", playerUserName);

            await ReplyAsync($"{loadingStr}");

            var playerData = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName).Result;

            if (playerData == null)
            {
                await ReplyAsync($"I couldn't find data for player with name : ***{playerUserName}***.");
                return;
            }

            var characterConfig = IResolver.Current.CharacterSettings.GetCharacterConfigByAlias(characterAlias).Result;
            if (characterConfig == null)
            {
                await ReplyAsync($"I couldn't find any character with alias : ***{characterAlias}***");
                return;
            }

            var character = playerData.Characters.Where(p => p.Name.ToLower() == characterConfig.Name.ToLower()).FirstOrDefault();

            if (character == null)
            {
                await ReplyAsync($"I couldn't find data for character : ***{characterConfig.Name}*** for player : ***{playerUserName}***.");
                return;
            }
            
            retStr += string.Format("\n{0} - {1}* g{2} lvl:{3}", character.Name, character.Stars, character.Gear, character.Level);
            retStr += string.Format("\nPower {0} - StatPower {1}", character.Power, character.StatPower);

            retStr += "\n\n**General**";
            retStr += $"\nProtection: {character.GeneralStats.Protection}";
            retStr += $"\nHealth: {character.GeneralStats.Health}";
            retStr += $"\nSpeed: {character.GeneralStats.Speed}";
            retStr += $"\nHealth Steal: {character.GeneralStats.HealthSteal} %";
            retStr += $"\nCritical Damage: {character.GeneralStats.CriticalDamage} %";
            retStr += $"\nPotency: {character.GeneralStats.Potency} %";
            retStr += $"\nTenacity: {character.GeneralStats.Tenacity} %";

            retStr += "\n\n**Physical Offense**";
            retStr += $"\nPhysical Damage: {character.OffenseStats.PhysicalOffense.PhysicalDamage}";
            retStr += $"\nPhysical Critical Chance: {character.OffenseStats.PhysicalOffense.PhysicalCriticalChance} %";
            retStr += $"\nPhysical Accuracy: {character.OffenseStats.PhysicalOffense.PhysicalAccuracy} %";
            retStr += $"\nArmor Penetration: {character.OffenseStats.PhysicalOffense.ArmorPenetration} %";

            retStr += "\n\n**Special Offense**";
            retStr += $"\nSpecial Damage: {character.OffenseStats.SpecialOffense.SpecialDamage}";
            retStr += $"\nSpecial Critical Chance: {character.OffenseStats.SpecialOffense.SpecialCriticalChance} %";
            retStr += $"\nSpecial Accuracy: {character.OffenseStats.SpecialOffense.SpecialAccuracy} %";
            retStr += $"\nResistance Penetration: {character.OffenseStats.SpecialOffense.ResistancePenetration} %";

            retStr += "\n\n**Physical Survivability**";
            retStr += $"\nArmor: {character.Survivability.PhysicalSurvivability.Armor} %";
            retStr += $"\nDodge Chance: {character.Survivability.PhysicalSurvivability.DodgeChance} %";
            retStr += $"\nPhysical Critical Avoidance: {character.Survivability.PhysicalSurvivability.PhysicalCriticalAvoidance} %";

            retStr += "\n\n**Special Survivability**";
            retStr += $"\nResistance: {character.Survivability.SpecialSurvivability.Resistance} %";
            retStr += $"\nDeflection Chance: {character.Survivability.SpecialSurvivability.DeflectionChance} %";
            retStr += $"\nSpecial Critical Avoidance: {character.Survivability.SpecialSurvivability.SpecialCriticalAvoidance} %";

            await ReplyAsync($"{retStr}");
            await cacheClient.AddToModuleCache(functionName, key, retStr);
        }

        [Command("character-compare", RunMode = RunMode.Async)]
        [Summary("Compares character stats for 2 specific players")]
        [Remarks("*character-compare {player1UserName} {player2UserName} {characterAlias}*")]
        [Alias("cc")]
        public async Task GetCharacterStatsCompare(string player1UserName, string player2UserName, string characterAlias)
        {
            player1UserName = player1UserName.Trim();
            player2UserName = player2UserName.Trim();
            characterAlias = characterAlias.Trim();

            string retStr = "";
            //get from cache if possible and exit sub
            string functionName = "character-compare";
            string key = string.Concat(player1UserName, player2UserName, characterAlias);
            retStr = cacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

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

            var characterConfig = IResolver.Current.CharacterSettings.GetCharacterConfigByAlias(characterAlias).Result;
            if (characterConfig == null)
            {
                await ReplyAsync($"I couldn't find any character with alias : ***{characterAlias}***");
                return;
            }

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
            
            retStr += string.Format("\n{0} - {1}* g{2} lvl:{3} - {4}* g{5} lvl:{6}  ", character1.Name, character1.Stars, character1.Gear, character1.Level, character2.Stars, character2.Gear, character2.Level);
            retStr += string.Format("\nPower {0} vs {2} - StatPower {1} vs {3}", character1.Power, character1.StatPower, character2.Power, character2.StatPower);

            var strAbilities = "\n\n**Abilities**";
            for (int i = 0; i < character1.Abilities.Count(); i++)
            {
                strAbilities += string.Format("\n{0} {1}/{2} vs {3}/{4}", character1.Abilities[i].Name, character1.Abilities[i].Level, character1.Abilities[i].MaxLevel, character2.Abilities[i].Level, character2.Abilities[i].MaxLevel);
            }
            retStr += strAbilities;

            retStr += "\n\n**General**";
            retStr += $"\nProtection: {character1.GeneralStats.Protection} - {character2.GeneralStats.Protection}";
            retStr += $"\nHealth: {character1.GeneralStats.Health} - {character2.GeneralStats.Health}";
            retStr += $"\nSpeed: {character1.GeneralStats.Speed} - {character2.GeneralStats.Speed}";
            retStr += $"\nHealth Steal: {character1.GeneralStats.HealthSteal} % - {character2.GeneralStats.HealthSteal} %";
            retStr += $"\nCritical Damage: {character1.GeneralStats.CriticalDamage} % - {character2.GeneralStats.CriticalDamage} %";
            retStr += $"\nPotency: {character1.GeneralStats.Potency} % - {character2.GeneralStats.Potency} %";
            retStr += $"\nTenacity: {character1.GeneralStats.Tenacity} % - {character2.GeneralStats.Tenacity} %";

            retStr += "\n\n**Physical Offense**";
            retStr += $"\nPhysical Damage: {character1.OffenseStats.PhysicalOffense.PhysicalDamage} - {character2.OffenseStats.PhysicalOffense.PhysicalDamage}";
            retStr += $"\nPhysical Critical Chance: {character1.OffenseStats.PhysicalOffense.PhysicalCriticalChance} % - {character2.OffenseStats.PhysicalOffense.PhysicalCriticalChance} %";
            retStr += $"\nPhysical Accuracy: {character1.OffenseStats.PhysicalOffense.PhysicalAccuracy} % - {character2.OffenseStats.PhysicalOffense.PhysicalAccuracy} %";
            retStr += $"\nArmor Penetration: {character1.OffenseStats.PhysicalOffense.ArmorPenetration} - {character2.OffenseStats.PhysicalOffense.ArmorPenetration}";

            retStr += "\n\n**Special Offense**";
            retStr += $"\nSpecial Damage: {character1.OffenseStats.SpecialOffense.SpecialDamage} - {character2.OffenseStats.SpecialOffense.SpecialDamage}";
            retStr += $"\nSpecial Critical Chance: {character1.OffenseStats.SpecialOffense.SpecialCriticalChance} % - {character2.OffenseStats.SpecialOffense.SpecialCriticalChance} %";
            retStr += $"\nSpecial Accuracy: {character1.OffenseStats.SpecialOffense.SpecialAccuracy} % - {character2.OffenseStats.SpecialOffense.SpecialAccuracy} %";
            retStr += $"\nResistance Penetration: {character1.OffenseStats.SpecialOffense.ResistancePenetration} % - {character2.OffenseStats.SpecialOffense.ResistancePenetration} %";

            retStr += "\n\n**Physical Survivability**";
            retStr += $"\nArmor: {character1.Survivability.PhysicalSurvivability.Armor} % - {character2.Survivability.PhysicalSurvivability.Armor} %";
            retStr += $"\nDodge Chance: {character1.Survivability.PhysicalSurvivability.DodgeChance} % - {character2.Survivability.PhysicalSurvivability.DodgeChance} %";
            retStr += $"\nPhysical Critical Avoidance: {character1.Survivability.PhysicalSurvivability.PhysicalCriticalAvoidance} % - {character2.Survivability.PhysicalSurvivability.PhysicalCriticalAvoidance} %";

            retStr += "\n\n**Special Survivability**";
            retStr += $"\nResistance: {character1.Survivability.SpecialSurvivability.Resistance} % - {character2.Survivability.SpecialSurvivability.Resistance} %";
            retStr += $"\nDeflection Chance: {character1.Survivability.SpecialSurvivability.DeflectionChance} % - {character2.Survivability.SpecialSurvivability.DeflectionChance} %";
            retStr += $"\nSpecial Critical Avoidance: {character1.Survivability.SpecialSurvivability.SpecialCriticalAvoidance} % - {character2.Survivability.SpecialSurvivability.SpecialCriticalAvoidance} %";

            await ReplyAsync($"{retStr}");
            await cacheClient.AddToModuleCache(functionName, key, retStr);
        }
    }
}
