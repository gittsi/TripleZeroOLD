﻿using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Core.Caching;
using System;
using TripleZero.Bot.Validators;
using FluentValidation;


namespace TripleZero.Modules
{
    [Name("Player")]
    [Summary("Player Commands")]
    public class PlayerModule : ModuleBase<SocketCommandContext>
    {
        private CacheClient cacheClient = IResolver.Current.CacheClient;

        [Command("player-report", RunMode = RunMode.Async)]
        [Summary("Get full report for a player")]
        [Remarks("*player-report {playerUserName}*")]
        [Alias("pr")]
        public async Task GetPlayerReport(string playerUserName)
        {
            await Task.FromResult(1);

            playerUserName = playerUserName.Trim();
            string retStr = "";
            string loadingStr = "";

            //get from cache if possible and exit sub
            string functionName = "player-report";
            string key = playerUserName;
            retStr = cacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            loadingStr = string.Format("```{0} is loading...```", playerUserName);
            var messageLoading = await ReplyAsync($"{loadingStr}");

            var playerData = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName.ToLower()).Result;
            if (playerData == null)
            {
                await ReplyAsync($"I couldn't find data for player with name : ***{playerUserName}***.");
                return;
            }            

            //validation need refactor for DI
            try
            {
                var validationResult = DefaultValidatorExtensions.Validate(new PlayerValidator(), playerData, ruleSet: "Basic,WithCharacter,WithShip");                
                if (!validationResult.IsValid)
                {
                    await messageLoading.DeleteAsync();
                    await ReplyAsync(string.Join("\n", validationResult.Errors.Distinct()));
                    return;
                }
            }
            catch(Exception ex)
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"Problem validating Player data - player-report {playerUserName}");
                return;
            }

            if (playerData.LoadedFromCache) await ReplyAsync($"{cacheClient.GetCachedDataRepositoryMessage()}");

            retStr += $"```css\nPlayer Report for {playerData.PlayerName} - {playerData.PlayerNameInGame} \n```";
            retStr += string.Format("```Last update : {0}(UTC)```\n", playerData.SWGoHUpdateDate.ToString("yyyy-MM-dd HH:mm:ss"));            

            var notActivatedChars = playerData.Characters.Where(p => p.Level == 0).ToList();
            var notActivatedShips = playerData.Ships?.Where(p => p.Level == 0).ToList();

            //stars characters
            var chars1star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 1).ToList();
            var chars2star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 2).ToList();
            var chars3star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 3).ToList();
            var chars4star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 4).ToList();
            var chars5star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 5).ToList();
            var chars6star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 6).ToList();
            var chars7star = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Stars == 7).ToList();

            //stars ships
            var ships1star = playerData.Ships.Where(p => p.Level != 0).Where(p => p.Stars == 1).ToList();
            var ships2star = playerData.Ships.Where(p => p.Level != 0).Where(p => p.Stars == 2).ToList();
            var ships3star = playerData.Ships.Where(p => p.Level != 0).Where(p => p.Stars == 3).ToList();
            var ships4star = playerData.Ships.Where(p => p.Level != 0).Where(p => p.Stars == 4).ToList();
            var ships5star = playerData.Ships.Where(p => p.Level != 0).Where(p => p.Stars == 5).ToList();
            var ships6star = playerData.Ships.Where(p => p.Level != 0).Where(p => p.Stars == 6).ToList();
            var ships7star = playerData.Ships.Where(p => p.Level != 0).Where(p => p.Stars == 7).ToList();

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

            //level characters
            var charsLessThan50Level = playerData.Characters.Where(p => p.Level > 0 && p.Level < 50).ToList();
            var chars50_59Level = playerData.Characters.Where(p => p.Level >= 50 && p.Level < 60).ToList();
            var chars60_69Level = playerData.Characters.Where(p => p.Level >= 60 && p.Level < 70).ToList();
            var chars70_79Level = playerData.Characters.Where(p => p.Level >= 70 && p.Level < 80).ToList();
            var chars80_84Level = playerData.Characters.Where(p => p.Level >= 80 && p.Level < 85).ToList();
            var chars85Level = playerData.Characters.Where(p => p.Level == 85).ToList();

            //level characters
            var shipsLessThan50Level = playerData.Ships.Where(p => p.Level > 0 && p.Level < 50).ToList();
            var ships50_59Level = playerData.Ships.Where(p => p.Level >= 50 && p.Level < 60).ToList();
            var ships60_69Level = playerData.Ships.Where(p => p.Level >= 60 && p.Level < 70).ToList();
            var ships70_79Level = playerData.Ships.Where(p => p.Level >= 70 && p.Level < 80).ToList();
            var ships80_84Level = playerData.Ships.Where(p => p.Level >= 80 && p.Level < 85).ToList();
            var ships85Level = playerData.Ships.Where(p => p.Level == 85).ToList();

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
            var modsLevel9_11 = _allMods.Where(p => p._Mods.Level >= 9 && p._Mods.Level <= 11).ToList();
            var modsLevel12_14 = _allMods.Where(p => p._Mods.Level >= 12 && p._Mods.Level <= 14).ToList();
            var modsLevel15 = _allMods.Where(p => p._Mods.Level == 15).ToList();

            //gear
            var gearLessThan5 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear < 5).ToList();
            var gear5_7 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear >= 5 && p.Gear <= 7).ToList();
            var gear8_9 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear >= 8 && p.Gear <= 9).ToList();
            var gear10 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear == 10).ToList();
            var gear11 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear == 11).ToList();
            var gear12 = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Gear == 12).ToList();

            //power
            var powerLessThan6k = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Power < 6000).ToList();

            //build post string
            retStr += string.Format("{0} characters **not activated** (from total characters : {1})\n", notActivatedChars.Count(), playerData.Characters.Count());
            retStr += string.Format("{0} ships **not activated** (from total ships : {1})\n", notActivatedShips.Count(), playerData.Ships.Count());

            retStr += string.Format("Total GP: `{0}`\n", playerData.GalacticPowerShips + playerData.GalacticPowerCharacters);
            retStr += string.Format("Toons GP: `{0}`\n", playerData.GalacticPowerCharacters);
            retStr += string.Format("Ships GP: `{0}`\n", playerData.GalacticPowerShips);

            retStr += "\n**Stars Characters**\n";
            retStr += string.Format("**1*** : {0}\n", chars1star.Count());
            retStr += string.Format("**2*** : {0}\n", chars2star.Count());
            retStr += string.Format("**3*** : {0}\n", chars3star.Count());
            retStr += string.Format("**4*** : {0}\n", chars4star.Count());
            retStr += string.Format("**5*** : {0}\n", chars5star.Count());
            retStr += string.Format("**6*** : {0}\n", chars6star.Count());
            retStr += string.Format("**7*** : {0}\n", chars7star.Count());

            retStr += "\n**Stars Ships**\n";
            retStr += string.Format("**1*** : {0}\n", ships1star.Count());
            retStr += string.Format("**2*** : {0}\n", ships2star.Count());
            retStr += string.Format("**3*** : {0}\n", ships3star.Count());
            retStr += string.Format("**4*** : {0}\n", ships4star.Count());
            retStr += string.Format("**5*** : {0}\n", ships5star.Count());
            retStr += string.Format("**6*** : {0}\n", ships6star.Count());
            retStr += string.Format("**7*** : {0}\n", ships7star.Count());

            retStr += "\n**Abilities**\n";
            foreach (var character in missingAbilitiesTop10)
            {
                retStr += string.Format("{0} is missing **{1} abilities**\n", character.Key, character.MissingLevels);
            }

            retStr += "\n**Level Characters**\n";
            retStr += string.Format("**lvl<50** : {0}\n", charsLessThan50Level.Count());
            retStr += string.Format("**lvl 50-59** : {0}\n", chars50_59Level.Count());
            retStr += string.Format("**lvl 60-69** : {0}\n", chars60_69Level.Count());
            retStr += string.Format("**lvl 70-79** : {0}\n", chars70_79Level.Count());
            retStr += string.Format("**lvl 80-84** : {0}\n", chars80_84Level.Count());
            retStr += string.Format("**lvl 85** : {0}\n", chars85Level.Count());

            retStr += "\n**Level Ships**\n";
            retStr += string.Format("**lvl<50** : {0}\n", shipsLessThan50Level.Count());
            retStr += string.Format("**lvl 50-59** : {0}\n", ships50_59Level.Count());
            retStr += string.Format("**lvl 60-69** : {0}\n", ships60_69Level.Count());
            retStr += string.Format("**lvl 70-79** : {0}\n", ships70_79Level.Count());
            retStr += string.Format("**lvl 80-84** : {0}\n", ships80_84Level.Count());
            retStr += string.Format("**lvl 85** : {0}\n", ships85Level.Count());



            retStr += "\n**Mods**\n";
            retStr += string.Format("{0} activated characters with **no mods** : {0}\n", noMods.Count());
            retStr += string.Format("**1 mod** : {0}\n", oneMod.Count());
            retStr += string.Format("**2 mods** : {0}\n", twoMod.Count());
            retStr += string.Format("**3 mods** : {0}\n", threeMod.Count());
            retStr += string.Format("**4 mods** : {0}\n", fourMod.Count());
            retStr += string.Format("**5 mods** : {0}\n", fiveMod.Count());
            retStr += string.Format("**6 mods** : {0}\n", sixMod.Count());

            retStr += "\n**Mods Level**\n";
            retStr += string.Format("{0} mods at **level <9**\n", modsLevelLessThan9.Count());
            retStr += string.Format("{0} mods at **level 9-11**\n", modsLevel9_11.Count());
            retStr += string.Format("{0} mods at **level 12-14**\n", modsLevel12_14.Count());
            retStr += string.Format("{0} mods at **level 15**\n", modsLevel15.Count());

            retStr += "\n**Gear**\n";
            retStr += string.Format("**4 or less** : {0}\n", gearLessThan5.Count());
            retStr += string.Format("**5-7** : {0}\n", gear5_7.Count());
            retStr += string.Format("**8-9** : {0}\n", gear8_9.Count());
            retStr += string.Format("**10** : {0}\n", gear10.Count());
            retStr += string.Format("**11** : {0}\n", gear11.Count());
            retStr += string.Format("**12** : {0}\n", gear12.Count());

            //power less than 6k
            retStr += "\n**Power**\n";
            retStr += string.Format("{0} characters with **less than 6000 power**\n", powerLessThan6k.Count());

            await cacheClient.AddToModuleCache(functionName, key, retStr);
            await ReplyAsync($"{retStr}");
            await messageLoading.DeleteAsync();
        }

        [Command("player-tw", RunMode = RunMode.Async)]
        [Summary("Get which characters of specified player are ineligible for Territory Wars")]
        [Remarks("*player-tw {playerUserName}*")]
        [Alias("ptw")]
        public async Task GetPlayerReportTW(string playerUserName)
        {
            await Task.FromResult(1);

            playerUserName = playerUserName.Trim();
            string retStr = "";
            string loadingStr = "";

            //get from cache if possible and exit sub
            string functionName = "player-tw";
            string key = playerUserName;
            retStr = cacheClient.GetMessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            loadingStr = string.Format("```{0} is loading to show report about TW```", playerUserName);
            var messageLoading = await ReplyAsync($"{loadingStr}");

            var playerData = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName.ToLower()).Result;

            if (playerData == null)
            {
                await ReplyAsync($"I couldn't find data for player with name : ***{playerUserName}***.");
                await messageLoading.DeleteAsync();
                return;
            }

            //validation need refactor for DI
            try
            {
                var validationResult = DefaultValidatorExtensions.Validate(new PlayerValidator(), playerData, ruleSet: "Basic,WithCharacter");
                if (!validationResult.IsValid)
                {
                    await messageLoading.DeleteAsync();
                    await ReplyAsync(string.Join("\n", validationResult.Errors.Distinct()));
                    return;
                }
            }
            catch (Exception ex)
            {
                await messageLoading.DeleteAsync();
                await ReplyAsync($"Problem validating Player data - player-report {playerUserName}");
                return;
            }

            if (playerData.LoadedFromCache) await ReplyAsync($"{cacheClient.GetCachedDataRepositoryMessage()}");

            retStr += $"```css\nPlayer Report for {playerData.PlayerName} - {playerData.PlayerNameInGame} \n```";
            retStr += string.Format("```Last update : {0}(UTC)```\n", playerData.SWGoHUpdateDate.ToString("yyyy-MM-dd HH:mm:ss"));

            var notActivatedChars = playerData.Characters.Where(p => p.Level == 0).ToList();            

            //power
            var powerLessThan6k = playerData.Characters.Where(p => p.Level != 0).Where(p => p.Power < 6000).ToList().OrderByDescending(p => p.Power);

            //build post string
            retStr += string.Format("{0} characters **not activated** (from total characters : {1})\n", notActivatedChars.Count(), playerData.Characters.Count());

            //power less than 6k
            retStr += string.Format("\n**{0}** characters with **less than** `6000` **power**\n", powerLessThan6k.Count());
            foreach (var character in powerLessThan6k)
            {
                retStr += string.Format("{0} : `{1}`\n", character.Name, character.Power);
            }

            await cacheClient.AddToModuleCache(functionName, key, retStr);
            await ReplyAsync($"{retStr}");
            await messageLoading.DeleteAsync();
        }
    }
}
