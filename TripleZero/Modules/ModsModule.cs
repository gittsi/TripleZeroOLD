using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using SWGoH.Model.Extensions;
using SWGoH.Model.Enums;
using SWGoH.Model;
using TripleZero.Helper;

namespace TripleZero.Modules
{
    [Name("Mods")]
    [Summary("Mods Commands")]
    public class ModsModule : ModuleBase<SocketCommandContext>
    {
        #region "Filter Mods"
        [Command("mods")]
        [Summary("test")]
        [Remarks("*test")]
        public async Task GetMods()
        {
            await ReplyAsync($"test");
        }
        #endregion

        #region "Secondary stats"
        private async void SendSecondaryModReply(string playerUserName, ModStatType modStatType, ModValueType secondaryStatValueType, List<Tuple<string, Mod>> result)
        {
            string retStr = "";
            if (result != null)
            {
                foreach (var row in result)
                {
                    var modStats = row.Item2.SecondaryStat.Where(p => p.StatType == modStatType && p.ValueType == secondaryStatValueType).FirstOrDefault();
                    var newString = string.Format("{3}: **{2}{4}** {1} {0}", row.Item1.PadRight(25), EnumExtensions.GetDescription(row.Item2.Type).ToString().PadRight(10), modStats.Value.ToString().PadRight(2), modStatType.ToString(), secondaryStatValueType == ModValueType.Percentage ? "%" : "");

                    retStr += "\n";
                    retStr += newString;

                    if (retStr.Length > 1800)
                    {
                        await ReplyAsync($"{retStr}");
                        retStr = "";
                    }
                }
                if (retStr.Length > 0)
                    await ReplyAsync($"{retStr}");
                else
                    await ReplyAsync($"No mods found!");
            }
            else
            {
                retStr = $"I didn't find any mods for username {playerUserName}`";
                await ReplyAsync($"{retStr}");
            }
            string functionName = "mods-s";
            string key = string.Concat(playerUserName, modStatType.GetDescription());
            await CacheClient.AddToModuleCache(functionName, key, retStr);
        }
        private async Task<List<Tuple<string, Mod>>> GetSpecificSecondaryMods(string playerUserName, ModStatType modStatType, ModValueType modValueType, int rows = 20)
        {
            var res = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName).Result;

            if (res == null)
            {
                await ReplyAsync($"I couldn't find player : {playerUserName}...");
                return null;
            }

            var sortedMods = (from Character in res.Characters.Where(p => p.Mods != null)
                              from Mod in Character.Mods.Where(p => p.SecondaryStat != null)
                              from Stats in Mod.SecondaryStat.Where(p => p.StatType == modStatType && p.ValueType == modValueType)
                              select new
                              {
                                  Character.Name,
                                  Mod
                              }
                        ).OrderByDescending(t => t.Mod.SecondaryStat.Where(p => p.StatType == modStatType && p.ValueType == modValueType).FirstOrDefault().Value).Take(rows).ToList();

            return sortedMods.Select(x => new Tuple<string, Mod>(x.Name, x.Mod)).ToList();
        }
        [Command("mods-s")]
        [Summary("Get mods sorted by a **secondary** stat of a given player")]
        [Remarks("*mods-s {playerUserName} {modType(add **%** if you want percentage)} { {rows(optional)}\n\n examples \n1) $mods-s playerName defense \n2) $mods-s playerName defense% 5)*")]
        public async Task GetSecondaryStatMods(string playerUserName, string modType, int rows = 20)
        {
            playerUserName = playerUserName.Trim();
            modType = modType.Trim();

            //get from cache if possible and exit sub
            string functionName = "mods-s";
            string key = string.Concat(playerUserName,modType);
            string retStr = CacheClient.MessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            ModValueType secondaryStatValueType = ModValueType.None;
            if (modType.Substring(modType.Length - 1, 1) == "%")
            {
                secondaryStatValueType = ModValueType.Percentage;
                modType = modType.Replace("%", "");
            }
            else
            {
                secondaryStatValueType = ModValueType.Flat;
            }

            ModStatType secondaryStatType = (ModStatType)EnumExtensions.GetEnumFromDescription(modType.ToLower(), typeof(ModStatType));

            if (secondaryStatType == ModStatType.None)
            {
                await ReplyAsync($"Something is wrong with your command...");
                return;
            }

            var result = await GetSpecificSecondaryMods(playerUserName, secondaryStatType, secondaryStatValueType, rows);
            SendSecondaryModReply(playerUserName, secondaryStatType, secondaryStatValueType, result);
        }
        #endregion

        #region "Primary stats"
        private async void SendPrimaryModReply(string playerUserName, ModStatType modStatType, List<Tuple<string, Mod>> result)
        {
            string retStr = "";
            if (result != null)
            {
                foreach (var row in result)
                {
                    var modStats = row.Item2.PrimaryStat;
                    var newString = string.Format("{3}: **{2}{4}** {1} {0}", row.Item1.PadRight(25), row.Item2.Type.ToString().PadRight(10), modStats.Value.ToString().PadRight(2), modStatType.ToString(), modStats.ValueType == ModValueType.Percentage ? "%" : "");
                    retStr += "\n";
                    retStr += newString;

                    if (retStr.Length > 1800)
                    {
                        await ReplyAsync($"{retStr}");
                        retStr = "";
                    }
                }

                if (retStr.Length > 0)
                    await ReplyAsync($"{retStr}");
                else
                    await ReplyAsync($"No mods found!");
            }
            else
            {
                retStr = $"I didn't find any mods for username {playerUserName}`";
                await ReplyAsync($"{retStr}");
            }

            string functionName = "mods-p";
            string key = string.Concat(playerUserName, modStatType.GetDescription());
            await CacheClient.AddToModuleCache(functionName, key, retStr);
        }
        private async Task<List<Tuple<string, Mod>>> GetSpecificPrimaryMods(string playerUserName, ModStatType modStatType, int rows = 20)
        {
            var res = IResolver.Current.MongoDBRepository.GetPlayer(playerUserName).Result;

            if (res == null)
            {
                await ReplyAsync($"I couldn't find player : {playerUserName}...");
                return null;
            }

            var sortedMods = (from Character in res.Characters.Where(p => p.Mods != null)
                              from Mod in Character.Mods.Where(p => p.PrimaryStat != null && p.PrimaryStat.StatType == modStatType)
                              select new
                              {
                                  Character.Name,
                                  Mod
                              }
                        ).OrderByDescending(t => t.Mod.PrimaryStat.Value).Take(rows).ToList();

            return sortedMods.Select(x => new Tuple<string, Mod>(x.Name, x.Mod)).ToList();
        }
        [Command("mods-p")]
        [Summary("Get mods sorted by a **primary** stat of a given player")]
        [Remarks("*mods-p {playerUserName} {modType(add **%** if you want percentage)} { {rows(optional)}\n\n example \n$mods-p playerName speed 5)*")]
        public async Task GetPrimaryStatMods(string playerUserName, string modType, int rows = 20)
        {
            playerUserName = playerUserName.Trim();
            modType = modType.Trim();

            //get from cache if possible and exit sub
            string functionName = "mods-p";
            string key = string.Concat(playerUserName, modType);
            string retStr = CacheClient.MessageFromModuleCache(functionName, key);
            if (!string.IsNullOrWhiteSpace(retStr))
            {
                await ReplyAsync($"{retStr}");
                return;
            }

            ModStatType primaryStatType = (ModStatType)EnumExtensions.GetEnumFromDescription(modType.ToLower(), typeof(ModStatType));

            if (primaryStatType == ModStatType.None)
            {
                await ReplyAsync($"Something is wrong with your command...");
                return;
            }

            var result = await GetSpecificPrimaryMods(playerUserName, primaryStatType, rows);
            SendPrimaryModReply(playerUserName, primaryStatType, result);
        }
        #endregion


    }
}
