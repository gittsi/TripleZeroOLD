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
using Discord.WebSocket;
using Discord;
using TripleZero.Helper;

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
            string retStr = "";

            //check if user is in role in order to proceed with the action
            var userAllowed = Roles.UserInRole(Context, "botadmin");
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            playerUserName = playerUserName.Trim();

            var result = IResolver.Current.MongoDBRepository.SendPlayerToQueue(playerUserName).Result;

            
            if (result != null)
                retStr = string.Format("\nPlayer {0} added to queue. Please be patient, I need some time to retrieve data!!!",playerUserName);
            else
                retStr = string.Format("\nPlayer {0} not added to queue!!!!!");

            await ReplyAsync($"{retStr}");
        }

        [Command("playerReport")]
        [Summary("Get full report for a player. You can check available players of a guild by using ***$guildPlayers*** command.\nUsage : ***$playerReport {playerUserName}***")]
        public async Task CheckPlayer(string playerUserName)
        {
            playerUserName = playerUserName.Trim();

            string loadingStr = string.Format("\n**{0}** is loading...\n\n", playerUserName);

            await ReplyAsync($"{loadingStr}");
            //fil data
            var playerData = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName).Result;

            if (playerData == null)
            {
                await ReplyAsync($"I couldn't find data for player with name : ***{playerUserName}***.");
                return;
            }

            string retStr = string.Format("Last update : {0}(UTC)\n\n", playerData.LastSwGohUpdated.ToString("yyyy-MM-dd HH:mm:ss"));

            var notActivatedChars = playerData.Characters.Where(p => p.Level == 0).ToList();

            //stars
            var chars1star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 1).ToList();
            var chars2star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 2).ToList();
            var chars3star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 3).ToList();
            var chars4star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 4).ToList();
            var chars5star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 5).ToList();
            var chars6star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 6).ToList();
            var chars7star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 7).ToList();

            //abilities
            var _allAbilities = (from _Character in playerData.Characters.Where(p => p.Abilities != null)
                                 from _Abilities in _Character.Abilities
                                 select new
                                 {
                                     _Character.Name,
                                     _Abilities
                                 }
                            ).ToList();
            var missingAbilitiesTop10 = _allAbilities.GroupBy(d => d.Name)
                        .Select(
                            g => new
                            {
                                Key = g.Key,
                                SumLevels = g.Sum(s => s._Abilities.Level),
                                SumMaxLevels = g.Sum(s => s._Abilities.MaxLevel),
                                MissingLevels = g.Sum(s => s._Abilities.MaxLevel) - g.Sum(s => s._Abilities.Level)
                            }).OrderByDescending(p => p.MissingLevels).Take(10);

            //level
            var charsLessThan50Level = playerData.Characters.Where(p => p.Level > 0 && p.Level < 50).ToList();
            var chars50_59Level = playerData.Characters.Where(p => p.Level >= 50 && p.Level < 60).ToList();
            var chars60_69Level = playerData.Characters.Where(p => p.Level >= 60 && p.Level < 70).ToList();
            var chars70_79Level = playerData.Characters.Where(p => p.Level >= 70 && p.Level < 80).ToList();
            var chars80_84Level = playerData.Characters.Where(p => p.Level >= 80 && p.Level < 85).ToList();
            var chars85Level = playerData.Characters.Where(p => p.Level == 85).ToList();

            //number of mods
            var noMods = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Mods == null || p.Mods.Count == 0).ToList();
            var oneMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 1).ToList();
            var twoMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 2).ToList();
            var threeMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 3).ToList();
            var fourMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 4).ToList();
            var fiveMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 5).ToList();
            var sixMod = playerData.Characters.Where(p => p.Mods != null).Where(p => p.Mods.Count() == 6).ToList();


            //mods level
            var _allMods = (from _Character in playerData.Characters.Where(p => p.Mods != null)
                            from _Mods in _Character.Mods
                            select new
                            {
                                _Character.Name,
                                _Mods
                            }
                            ).ToList();
            var modsLevelLessThan9 = _allMods.Where(p => p._Mods.Level < 9).ToList();
            var modsLevel9_12 = _allMods.Where(p => p._Mods.Level >= 9 && p._Mods.Level <= 12).ToList();
            var modsLevel13_15 = _allMods.Where(p => p._Mods.Level >= 13 && p._Mods.Level <= 15).ToList();

            //gear
            var gear5orLess = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear <= 5).ToList();
            var gear6_8 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear >= 6 && p.Gear <= 8).ToList();
            var gear9_10 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear >= 9 && p.Gear <= 10).ToList();
            var gear11 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear == 11).ToList();
            var gear12 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear == 12).ToList();


            //build post string
            retStr += string.Format("{0} characters **not activated** (from total characters : {1})\n", notActivatedChars.Count(), playerData.Characters.Count());


            retStr += string.Format("Total GP: **{0}**\n", playerData.GPcharacters + playerData.GPships);
            retStr += string.Format("Toons GP: **{0}**\n", playerData.GPcharacters);
            retStr += string.Format("Ships GP: **{0}**\n", playerData.GPships);

            retStr += "\n**Stars**\n";
            retStr += string.Format("{0} characters at **1***\n", chars1star.Count());
            retStr += string.Format("{0} characters at **2***\n", chars2star.Count());
            retStr += string.Format("{0} characters at **3***\n", chars3star.Count());
            retStr += string.Format("{0} characters at **4***\n", chars4star.Count());
            retStr += string.Format("{0} characters at **5***\n", chars5star.Count());
            retStr += string.Format("{0} characters at **6***\n", chars6star.Count());
            retStr += string.Format("{0} characters at **7***\n", chars7star.Count());

            retStr += "\n**Abilities**\n";
            foreach (var character in missingAbilitiesTop10)
            {
                retStr += string.Format("{0} is missing **{1} abilities**\n", character.Key, character.MissingLevels);
            }

            retStr += "\n**Levels**\n";
            retStr += string.Format("{0} characters with **lvl<50**\n", charsLessThan50Level.Count());
            retStr += string.Format("{0} characters with **lvl 50-59**\n", chars50_59Level.Count());
            retStr += string.Format("{0} characters with **lvl 60-69**\n", chars60_69Level.Count());
            retStr += string.Format("{0} characters with **lvl 70-79**\n", chars70_79Level.Count());
            retStr += string.Format("{0} characters with **lvl 80-84**\n", chars80_84Level.Count());
            retStr += string.Format("{0} characters with **lvl 85**\n", chars85Level.Count());

            retStr += "\n**Mods**\n";
            retStr += string.Format("{0} activated characters with **no mods**\n", noMods.Count());
            retStr += string.Format("{0} characters with **1 mod**\n", oneMod.Count());
            retStr += string.Format("{0} characters with **2 mods**\n", twoMod.Count());
            retStr += string.Format("{0} characters with **3 mods**\n", threeMod.Count());
            retStr += string.Format("{0} characters with **4 mods**\n", fourMod.Count());
            retStr += string.Format("{0} characters with **5 mods**\n", fiveMod.Count());
            retStr += string.Format("{0} characters with **6 mods**\n", sixMod.Count());

            retStr += "\n**Mods Level**\n";
            retStr += string.Format("{0} mods at **level <9**\n", modsLevelLessThan9.Count());
            retStr += string.Format("{0} mods at **level 9-12**\n", modsLevel9_12.Count());
            retStr += string.Format("{0} mods at **level 13-15**\n", modsLevel13_15.Count());

            retStr += "\n**Gear**\n";
            retStr += string.Format("{0} characters with **gear 5 or less**\n", gear5orLess.Count());
            retStr += string.Format("{0} characters with **gear 6-8**\n", gear6_8.Count());
            retStr += string.Format("{0} characters with **gear 9-10**\n", gear9_10.Count());
            retStr += string.Format("{0} characters with **gear 11**\n", gear11.Count());
            retStr += string.Format("{0} characters with **gear 12**\n", gear12.Count());

            await ReplyAsync($"{retStr}");
        }

        //[Command("characterstats -c")]
        //[Summary("Compares character stats for 2 specific players.\nUsage : ***$characterstats -c {player1UserName} {player2UserName} {characterAlias}***")]
        //public async Task GetCharacterStatsCompare(string player1UserName, string player2UserName, string characterAlias)
        //{
        //    player1UserName = player1UserName.Trim();
        //    player2UserName = player2UserName.Trim();
        //    characterAlias = characterAlias.Trim();

        //    string loadingStr = string.Format("\n**{0} and {1}** are loading...\n\n", player1UserName, player2UserName);

        //    await ReplyAsync($"{loadingStr}");

        //    var player1Data = IResolver.Current.MongoDBRepository.GetPlayer(player1UserName).Result;
        //    if (player1Data == null)
        //    {
        //        await ReplyAsync($"I couldn't find data for player with name : ***{player1UserName}***.");
        //        return;
        //    }

        //    var player2Data = IResolver.Current.MongoDBRepository.GetPlayer(player2UserName).Result;
        //    if (player2Data == null)
        //    {
        //        await ReplyAsync($"I couldn't find data for player with name : ***{player2UserName}***.");
        //        return;
        //    }

        //    var characterConfig = IResolver.Current.CharacterConfig.GetCharacterConfigByAlias(characterAlias).Result;

        //    var character1 = player1Data.Characters.Where(p => p.Name.ToLower() == characterConfig.Name.ToLower()).FirstOrDefault();
        //    if (character1 == null)
        //    {
        //        await ReplyAsync($"I couldn't find data for character : ***{characterConfig.Name}*** for player : ***{player1UserName}***.");
        //        return;
        //    }

        //    var character2 = player2Data.Characters.Where(p => p.Name.ToLower() == characterConfig.Name.ToLower()).FirstOrDefault();
        //    if (character2 == null)
        //    {
        //        await ReplyAsync($"I couldn't find data for character : ***{characterConfig.Name}*** for player : ***{player2UserName}***.");
        //        return;
        //    }

        //    string retStr = "";
        //    retStr += string.Format("\n{0} - {1}* g{2} lvl:{3} - {4}* g{5} lvl:{6}  ", character1.Name, character1.Stars, character1.Gear, character1.Level, character2.Stars, character2.Gear, character2.Level);
        //    retStr += string.Format("\nPower {0} vs {2} - StatPower {1} vs {3}", character1.Power, character1.StatPower, character2.Power, character2.StatPower);

        //    var strAbilities = "\n\n**Abilities**";
        //    for(int i=0;i<character1.Abilities.Count();i++)
        //    {
        //        strAbilities += string.Format("\n{0} {1}/{2} vs {3}/{4}", character1.Abilities[i].Name, character1.Abilities[i].Level, character1.Abilities[i].MaxLevel, character2.Abilities[i].Level, character2.Abilities[i].MaxLevel);
        //    }
        //    retStr += strAbilities;

        //    //string strAbilities1 = "";
        //    //foreach (var ability1 in character1.Abilities)
        //    //{
        //    //    strAbilities1 += string.Format("{0}/{1} ", ability1.Level.ToString(), ability1.MaxLevel.ToString());
        //    //}
        //    //string strAbilities2 = "";
        //    //foreach (var ability2 in character2.Abilities)
        //    //{
        //    //    strAbilities2 += string.Format("{0}/{1} ", ability2.Level.ToString(), ability2.MaxLevel.ToString());
        //    //}
        //    //retStr += string.Format("\n{0} vs {1}", strAbilities1, strAbilities2);



        //    retStr += "\n\n**General**";
        //    retStr += $"\nProtection: {character1.Protection} - {character2.Protection}";
        //    retStr += $"\nHealth: {character1.Health} - {character2.Health}";
        //    retStr += $"\nSpeed: {character1.Speed} - {character2.Speed}";
        //    retStr += $"\nHealth Steal: {character1.HealthSteal} % - {character2.HealthSteal} %";
        //    retStr += $"\nCritical Damage: {character1.CriticalDamage} %";
        //    retStr += $"\nPotency: {character1.Potency} % - {character2.Potency} %";
        //    retStr += $"\nTenacity: {character1.Tenacity} % - {character2.Tenacity} %";

        //    retStr += "\n\n**Physical Offense**";
        //    retStr += $"\nPhysical Damage: {character1.PhysicalDamage} - {character2.PhysicalDamage}";
        //    retStr += $"\nPhysical Critical Chance: {character1.PhysicalCriticalChance} % - {character2.PhysicalCriticalChance} %";
        //    retStr += $"\nPhysical Accuracy: {character1.PhysicalAccuracy} % - {character2.PhysicalAccuracy} %";
        //    retStr += $"\nArmor Penetration: {character1.ArmorPenetration} - {character2.ArmorPenetration}";

        //    retStr += "\n\n**Special Offense**";
        //    retStr += $"\nSpecial Damage: {character1.SpecialDamage} - {character2.SpecialDamage}";
        //    retStr += $"\nSpecial Critical Chance: {character1.SpecialCriticalChance} % - {character2.SpecialCriticalChance} %";
        //    retStr += $"\nSpecial Accuracy: {character1.SpecialAccuracy} % - {character2.SpecialAccuracy} %";

        //    retStr += "\n\n**Physical Survivability**";
        //    retStr += $"\nArmor: {character1.Armor} % - {character2.Armor} %";
        //    retStr += $"\nDodge Chance: {character1.DodgeChance} % - {character2.DodgeChance} %";
        //    retStr += $"\nPhysical Critical Avoidance: {character1.PhysicalCriticalAvoidance} % - {character2.PhysicalCriticalAvoidance} %";

        //    retStr += "\n\n**Special Survivability**";
        //    retStr += $"\nResistance: {character1.Resistance} % - {character2.Resistance} %";
        //    retStr += $"\nDeflection Chance: {character1.DeflectionChance} % - {character2.DeflectionChance} %";
        //    retStr += $"\nSpecial Critical Avoidance: {character1.SpecialCriticalAvoidance} % - {character2.SpecialCriticalAvoidance} %";



        //    await ReplyAsync($"{retStr}");
        //}

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
