using Discord.Commands;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Strategy;
using TripleZero.Helper;

namespace TripleZero.Modules
{
    [Name("Player")]
    [Summary("Player Commands")]
    public class PlayerModule : ModuleBase<SocketCommandContext>
    {
        [Command("player-report")]
        [Summary("Get full report for a player")]
        [Remarks("*player-report {playerUserName}*")]
        public async Task GetPlayerReport(string playerUserName)
        {
            await Task.FromResult(1);

            playerUserName = playerUserName.Trim();
            string retStr = "";
            string loadingStr = "";

            //get from cache if possible and exit sub
            string functionName = "player-report";
            string key = playerUserName;
            retStr = ModuleCache.MessageFromCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            loadingStr = string.Format("\n**{0}** is loading...\n\n", playerUserName);

            await ReplyAsync($"{loadingStr}");

            var playerData = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName).Result;

            if (playerData == null)
            {
                await ReplyAsync($"I couldn't find data for player with name : ***{playerUserName}***.");
                return;
            }

            retStr = string.Format("Last update : {0}(UTC)\n\n", playerData.SWGoHUpdateDate.ToString("yyyy-MM-dd HH:mm:ss"));

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


            //build post string
            retStr += string.Format("{0} characters **not activated** (from total characters : {1})\n", notActivatedChars.Count(), playerData.Characters.Count());

            retStr += string.Format("Total GP: **{0}**\n", playerData.GalacticPowerShips + playerData.GalacticPowerCharacters);
            retStr += string.Format("Toons GP: **{0}**\n", playerData.GalacticPowerCharacters);
            retStr += string.Format("Ships GP: **{0}**\n", playerData.GalacticPowerShips);

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
            retStr += string.Format("{0} mods at **level 9-11**\n", modsLevel9_11.Count());
            retStr += string.Format("{0} mods at **level 12-14**\n", modsLevel12_14.Count());
            retStr += string.Format("{0} mods at **level 15**\n", modsLevel15.Count());

            retStr += "\n**Gear**\n";
            retStr += string.Format("{0} characters with **gear 4 or less**\n", gearLessThan5.Count());
            retStr += string.Format("{0} characters with **gear 5-7**\n", gear5_7.Count());
            retStr += string.Format("{0} characters with **gear 8-9**\n", gear8_9.Count());
            retStr += string.Format("{0} characters with **gear 10**\n", gear10.Count());
            retStr += string.Format("{0} characters with **gear 11**\n", gear11.Count());
            retStr += string.Format("{0} characters with **gear 12**\n", gear12.Count());


            await ModuleCache.AddToCache(functionName, key, retStr);
            await ReplyAsync($"{retStr}");
        }
    }
}
