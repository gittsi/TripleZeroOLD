﻿using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Helper;
using SWGoH.Model.Enums;
using System.Diagnostics;
using System.Globalization;
using Discord;
using TripleZero.Core.Caching;

namespace TripleZero.Modules
{
    [Name("Admin")]
    [Summary("Admin Commands")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private CacheClient cacheClient = IResolver.Current.CacheClient;

        [Command("alias-remove", RunMode = RunMode.Async)]
        [Summary("Remove alias for specific character(Admin Command)")]
        [Remarks("*alias-remove {characterFullName} {alias}*")]
        [Alias("ar")]
        public async Task RemoveAlias(string characterFullName, string alias)
        {
            characterFullName = characterFullName.Trim();
            alias = alias.Trim();

            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.RemoveCharacterAlias(characterFullName, alias.ToLower()).Result;

            if (result != null)
            {
                retStr += $"\nAlias '**{alias}**' for '**{characterFullName}**' was removed!\n";
                retStr += string.Format("\nName:**{0}**", result.Name);
                retStr += string.Format("\nCommand:**{0}**", result.Command != null ? result.Command.Length == 0 ? "empty!!!" : result.Command : "empty!!!");
                retStr += string.Format("\nSWGoH url:**{0}**", result.SWGoHUrl);

                string aliases = "";
                int countAliases = 0;
                foreach (var _alias in result.Aliases)
                {
                    countAliases += 1;
                    aliases += _alias;
                    if (countAliases != result.Aliases.Count()) aliases += ", ";
                }

                retStr += string.Format("\nAliases: [**{0}**]", aliases.Count() > 0 ? aliases : "empty!!!");
            }
            else
            {
                retStr = "Not updated. Probably something is wrong with your command!";
            }

            await ReplyAsync($"{retStr}");
        }

        [Command("alias-add", RunMode = RunMode.Async)]
        [Summary("Add alias for specific character(Admin Command)")]
        [Remarks("*alias-add {characterFullName} {alias}*")]
        [Alias("aa")]
        public async Task AddAlias(string characterFullName, string alias)
        {
            characterFullName = characterFullName.Trim();
            alias = alias.Trim();

            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.SetCharacterAlias(characterFullName, alias.ToLower()).Result;

            if (result != null)
            {
                retStr += $"\nNew alias '**{alias}**' for '**{characterFullName}**' was added!\n";
                retStr += string.Format("\nName:**{0}**", result.Name);
                retStr += string.Format("\nCommand:**{0}**", result.Command != null ? result.Command.Length == 0 ? "empty!!!" : result.Command : "empty!!!");
                retStr += string.Format("\nSWGoH url:**{0}**", result.SWGoHUrl);

                string aliases = "";
                int countAliases = 0;
                foreach (var _alias in result.Aliases)
                {
                    countAliases += 1;
                    aliases += _alias;
                    if (countAliases != result.Aliases.Count()) aliases += ", ";
                }

                retStr += string.Format("\nAliases: [**{0}**]", aliases.Count() > 0 ? aliases : "empty!!!");
            }
            else
            {
                retStr = "Not updated. Probably something is wrong with your command!";
            }

            await ReplyAsync($"{retStr}");


        }

        [Command("command-remove", RunMode = RunMode.Async)]        
        [Summary("Remove command for specific character(Admin Command)")]
        [Remarks("*command-remove {characterFullName}*")]
        [Alias("cr")]
        public async Task RemoveCommand(string characterFullName)
        {
            characterFullName = characterFullName.Trim();            

            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.RemoveCharacterCommand(characterFullName).Result;

            if (result != null)
            {
                retStr += $"\nCommand for '**{characterFullName}**' was deleted!\n";
                retStr += string.Format("\nName:**{0}**", result.Name);                
                retStr += string.Format("\nCommand:**{0}**", result.Command != null ? result.Command.Length == 0 ? "empty!!!" : result.Command : "empty!!!");
                retStr += string.Format("\nSWGoH url:**{0}**", result.SWGoHUrl);

                string aliases = "";
                int countAliases = 0;
                foreach (var _alias in result.Aliases)
                {
                    countAliases += 1;
                    aliases += _alias;
                    if (countAliases != result.Aliases.Count()) aliases += ", ";
                }

                retStr += string.Format("\nAliases: [**{0}**]", aliases.Count() > 0 ? aliases : "empty!!!");
            }
            else
            {
                retStr = "Not updated. Probably something is wrong with your command!";
            }

            await ReplyAsync($"{retStr}");
        }

        [Command("command-add", RunMode = RunMode.Async)]
        [Summary("Add command for specific character(Admin Command)")]
        [Remarks("*command-add {characterFullName} {command}*")]
        [Alias("ca")]
        public async Task AddCommand(string characterFullName, string command)
        {
            characterFullName = characterFullName.Trim();
            command = command.Trim().ToLower();

            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.SetCharacterCommand(characterFullName, command.ToLower()).Result;

            if (result != null)
            {
                retStr += $"\nNew command '**{command}**' for '**{characterFullName}**' was added!\n";
                retStr += string.Format("\nName:**{0}**", result.Name);
                retStr += string.Format("\nCommand:**{0}**", result.Command != null ? result.Command.Length == 0 ? "empty!!!" : result.Command : "empty!!!");
                retStr += string.Format("\nSWGoH url:**{0}**", result.SWGoHUrl);

                string aliases = "";
                int countAliases = 0;
                foreach (var _alias in result.Aliases)
                {
                    countAliases += 1;
                    aliases += _alias;
                    if (countAliases != result.Aliases.Count()) aliases += ", ";
                }

                retStr += string.Format("\nAliases: [**{0}**]", aliases.Count() > 0 ? aliases : "empty!!!");
            }
            else
            {
                retStr = "Not updated. Probably something is wrong with your command!";
            }

            await ReplyAsync($"{retStr}");


        }

        [Command("queue", RunMode = RunMode.Async)]
        //[Summary("Set alias for specific character(Admin Command).\nUsage : ***$alias -set {characterFullName}***")]
        [Summary("Get current for specific character(Admin Command)")]
        [Remarks("*queue*")]
        [Alias("q")]
        public async Task GetQueue(string resultsRows = "10")
        {
            bool rowsIsNumber = int.TryParse(resultsRows, out int rows);
            if (!rowsIsNumber) { await ReplyAsync($"If you want to specify how many results want, you have to put a number as third parameter! '{rows}' is not a number!"); return; }


            string retStr = "";


            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.GetQueue().Result;            

            if (result == null)
            {
                await ReplyAsync($"Problem!! Cannot get queue!!!");
                return;
            }
            else
            {
                await ReplyAsync($"I Found **{result.Count()} total entries in queue!**");
            }

            var guildQueues = result.Where(p => p.Type == QueueType.Guild).OrderByDescending(p => p.Status).ThenBy(p => p.NextRunDate).Take(rows);
            var playerQueues = result.Where(p => p.Type == QueueType.Player).OrderByDescending(p => p.Status).ThenBy(p => p.NextRunDate).Take(rows);

            if(guildQueues.Count()>0)
            {
                retStr += "\n**--------Guild Queue--------**";
                foreach(var guild in guildQueues)
                {
                    retStr += string.Format("\nGuild : **{0}** - Status : **{1}** - Next Run : **{2}**(UTC){3}"
                        , guild.Name
                        , guild.Status
                        , guild.NextRunDate?.ToString("yyyy-MM-dd HH:mm")
                        , string.IsNullOrWhiteSpace(guild.ProcessingBy) ? "" : string.Format(" - Processing started by : {0} at {1}(UTC)", guild.ProcessingBy,guild.ProcessingStartDate?.ToString("yyyy-MM-dd HH:mm")));
                }
            }

            if(playerQueues.Count()>0)
            {
                retStr += "\n\n**--------Player Queue--------**";
            }            

            var processingPlayer = playerQueues.Where(p => p.Status == QueueStatus.Processing);
            var pendingPlayer = playerQueues.Where(p => p.Status == QueueStatus.PendingProcess);
            var failedPlayer = playerQueues.Where(p => p.Status == QueueStatus.Failed);
            
            if (processingPlayer.Count()>0) retStr += "\n**--Processing**";
            foreach (var queuePlayer in processingPlayer)
            {
                retStr += string.Format("\nPlayer : **{0}** - Status : **{1}** - Processing started by {3} at **{2}**(UTC)", queuePlayer.Name, queuePlayer.Status, queuePlayer.ProcessingStartDate?.ToString("yyyy-MM-dd HH:mm"),queuePlayer.ProcessingBy);

                if (retStr.Length > 1800)
                {
                    await ReplyAsync($"{retStr}");
                    retStr = "";
                }
            }

            if (pendingPlayer.Count() > 0) retStr += "\n**--Pending Process**";
            foreach (var queuePlayer in pendingPlayer)
            {
                retStr += string.Format("\nPlayer : **{0}** - Status : **{1}** - Next Run : **{2}**(UTC)", queuePlayer.Name, queuePlayer.Status, queuePlayer.NextRunDate?.ToString("yyyy-MM-dd HH:mm"));

                if (retStr.Length > 1800)
                {
                    await ReplyAsync($"{retStr}");
                    retStr = "";
                }
            }

            if (failedPlayer.Count() > 0) retStr += "\n**--Failed**";
            foreach (var queuePlayer in failedPlayer)
            {
                retStr += string.Format("\nPlayer : **{0}** - Status : **{1}** - Next Run : **{2}**(UTC)", queuePlayer.Name, queuePlayer.Status, queuePlayer.NextRunDate?.ToString("yyyy-MM-dd HH:mm"));

                if (retStr.Length > 1800)
                {
                    await ReplyAsync($"{retStr}");
                    retStr = "";
                }
            }
            if(retStr.Length==0) { await ReplyAsync("Empty queue!!!!"); return; }
            await ReplyAsync($"{retStr}");
        }

        [Command("queue-remove", RunMode = RunMode.Async)]
        //[Summary("Remove row from queue(Admin Command).\nUsage : ***$queue-remove {characterFullName}***")]
        [Summary("Remove row from queue(Admin Command)")]
        [Remarks("*queue-remove {name}*")]
        [Alias("qr")]
        public async Task RemoveQueue(string name)
        {
            name = name.Trim().ToLower();

            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.RemoveFromQueue(name).Result;

            if (result != null)
            {
                retStr += $"\nQueue row for '**{name}**' was removed!\n";
                retStr += string.Format("\nId:**{0}**", result.Id.ToString());
                retStr += string.Format("\nName:**{0}**", result.Name);
                retStr += string.Format("\nStatus:**{0}**", result.Status.ToString());
                retStr += string.Format("\nType:**{0}**", result.Type);
            }
            else
            {
                retStr = "Not updated. Probably something is wrong with your command!";
            }

            await ReplyAsync($"{retStr}");
        }

        [Command("player-update", RunMode = RunMode.Async)]
        [Summary("Add a player for reload(Admin Command)")]
        [Remarks("*player-update {playerUserName}*")]
        [Alias("pu")]
        public async Task SetPlayerUpdate(string playerUserName)
        {
            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            playerUserName = playerUserName.Trim();

            var result = IResolver.Current.MongoDBRepository.SendPlayerToQueue(playerUserName).Result;


            if (result != null)
                retStr = string.Format("\nPlayer {0} added to queue. Please be patient, I need some time to retrieve data!!!", playerUserName);
            else
                retStr = string.Format("\nPlayer {0} not added to queue!!!!!");

            await ReplyAsync($"{retStr}");
        }

        [Command("guild-update", RunMode = RunMode.Async)]
        [Summary("Set a guild for reload")]
        [Remarks("*guild-update {guildName}*")]
        [Alias("gu")]
        public async Task SetGuildUpdate(string guildName)
        {
            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            guildName = guildName.Trim();

            var result = IResolver.Current.MongoDBRepository.SendGuildToQueue(guildName).Result;


            if (result != null)
                retStr = string.Format("\nGuild {0} added to queue. Please be patient, I need tons of time to retrieve data!!!", guildName);
            else
                retStr = string.Format("\nGuild {0} not added to queue!!!!!");

            await ReplyAsync($"{retStr}");
        }

        [Command("characterconfig-update", RunMode = RunMode.Async)]
        [Summary("Reload character config(Admin Command)")]
        [Remarks("*characterconfig-update*")]
        [Alias("ccu")]
        public async Task SetCharacterConfigUpdate()
        {
            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }            

            var result = IResolver.Current.MongoDBRepository.SendCharacterConfigToQueue().Result;


            if (result != null)
                retStr = "\nCharacter config update added to queue. Please be patient, I need some time to retrieve data!!!";
            else
                retStr = string.Format("\nNot added to queue!!!!!");

            await ReplyAsync($"{retStr}");
        }

        [Command("mem", RunMode = RunMode.Async)]
        [Summary("Check application diagnostics")]
        [Remarks("*mem*")]
        public async Task CheckDiagnostics()
        {
            string retStr = "";
            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            Process currentProc = Process.GetCurrentProcess();

            var threads = currentProc.Threads;
            long memoryUsed = currentProc.PrivateMemorySize64;

            retStr += string.Format("\nMemory : {0} - Threads : {1}", memoryUsed.ToString("#,##0,Kb", CultureInfo.InvariantCulture), threads.Count);

            await ReplyAsync($"{retStr}");
        }

        [Command("prune", RunMode = RunMode.Async)]
        [Summary("Delete messages")]
        [Remarks("*prune {number of messages}*")]
        public async Task Prune(string countMessagesToDelete) 
        {
            bool rowsIsNumber = int.TryParse(countMessagesToDelete, out int rows);
            if (!rowsIsNumber) { await ReplyAsync($"If you want to specify how many results want, you have to put a number as a parameter! '{rows}' is not a number!"); return; }

            string retStr = "";
            //await Context.Message.DeleteAsync();

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.GetTripleZeroBotSettings().DiscordSettings.BotAdminRole;
            var userAllowed = DiscordRoles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var messagesToDelete = await Context.Channel.GetMessagesAsync(rows).Flatten();
            await Context.Channel.DeleteMessagesAsync(messagesToDelete);

            var lastmessage = await Context.Channel.SendMessageAsync($"`{Context.User.Username} deleted {messagesToDelete.Count()} messages`");
            await Task.Delay(2000);
            await lastmessage.DeleteAsync();
        }

        //[Command("delay", RunMode = RunMode.Async)]
        //[Summary("delay")]
        //[Remarks("*delay {ms}*")]
        //public async Task Delay(string ms)
        //{
        //    bool msIsNumber = int.TryParse(ms, out int delay);
        //    await Task.Delay(delay);            
        //    await ReplyAsync($"Delayed for {delay}ms");
        //}

        [Command("clearcache", RunMode = RunMode.Async)]
        [Summary("Clear Cache")]
        [Remarks("*clearcache*")]
        public async Task ClearCache()
        {
            try
            {
                await cacheClient.ClearAllCaches();
                await ReplyAsync($"Caching is gone");
            }
            catch(Exception ex)
            {
                await ReplyAsync($"{ex.Message}");
            }
        }

    }
}
