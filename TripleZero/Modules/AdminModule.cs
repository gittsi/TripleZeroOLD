using Discord.Commands;
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

namespace TripleZero.Modules
{
    [Name("Admin")]
    [Summary("Admin Commands")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("alias-remove")]
        //[Summary("Set alias for specific character(Admin Command).\nUsage : ***$alias -set {characterFullName}***")]
        [Summary("Remove alias for specific character(Admin Command)")]
        [Remarks("*alias-remove {characterFullName} {alias}*")]
        public async Task RemoveAlias(string characterFullName, string alias)
        {
            characterFullName = characterFullName.Trim();
            alias = alias.Trim();

            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var result = IResolver.Current.MongoDBRepository.RemoveCharacterAlias(characterFullName, alias.ToLower()).Result;

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

        [Command("alias-add")]
        //[Summary("Add alias for specific character(Admin Command).\nUsage : ***$alias -set {characterFullName}***")]
        [Summary("Add alias for specific character(Admin Command)")]
        [Remarks("*alias-add {characterFullName} {alias}*")]
        public async Task AddAlias(string characterFullName, string alias)
        {
            characterFullName = characterFullName.Trim();
            alias = alias.Trim();

            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
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

        [Command("characters-config")]
        //[Summary("Get config for specific character(Admin Command).\nUsage : ***$characters -config***")]
        [Summary("Get config all characters(Admin Command)")]
        [Remarks("*characters-config*")]
        public async Task GetCharacterConfig()
        {
            string retStr = "";
            string chStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
            if (!userAllowed)
            {
                retStr = "\nNot authorized!!!";
                await ReplyAsync($"{retStr}");
                return;
            }

            var charactersConfig = IResolver.Current.CharacterSettings.GetCharactersConfig().Result;
            int debugcount = 0;
            foreach (var characterConfig in charactersConfig)
            {
                chStr = string.Format("\n{0}", characterConfig.Name);
                string aliases = "";
                int countAliases = 0;
                foreach (var alias in characterConfig.Aliases)
                {
                    if (alias.ToString().ToLower() == "empty") break;

                    countAliases += 1;
                    aliases += string.Format("{0}", alias);
                    if (countAliases != characterConfig.Aliases.Count()) aliases += ", ";
                }
                debugcount = debugcount + 1;
                if (debugcount > 2000)
                {
                    break;
                }

                if (countAliases > 0)
                {
                    retStr += string.Format("{0} - Aliases:[{1}]", chStr, aliases);
                }
                else
                {
                    retStr += string.Format("{0} - **No aliases**", chStr);
                }

                if (retStr.Length > 1800)
                {
                    await ReplyAsync($"{retStr}");
                    retStr = "";
                }
            }

            await ReplyAsync($"{retStr}");
        }

        [Command("queue")]
        //[Summary("Set alias for specific character(Admin Command).\nUsage : ***$alias -set {characterFullName}***")]
        [Summary("Get current for specific character(Admin Command)")]
        [Remarks("*queue*")]
        public async Task GetQueue(string resultsRows = "10")
        {
            bool rowsIsNumber = int.TryParse(resultsRows, out int rows);
            if (!rowsIsNumber) { await ReplyAsync($"If you want to specify how many results want, you have to put a number as third parameter! '{rows}' is not a number!"); return; }


            string retStr = "";


            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
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
                await ReplyAsync($"Found **{result.Count()} rows in queue!**");
            }

            var guildQueues = result.Where(p => p.Type == QueueType.Guild).OrderByDescending(p => p.Status).ThenBy(p => p.NextRunDate).Take(rows);
            var playerQueues = result.Where(p => p.Type == QueueType.Player).OrderByDescending(p => p.Status).ThenBy(p => p.NextRunDate).Take(rows);

            if(guildQueues.Count()>0)
            {
                retStr += "\n**--------Guild Queue--------**";
                foreach(var guild in guildQueues)
                {
                    retStr += string.Format("\nGuild : **{0}** - Status : **{1}** - Next Run : **{2}**(UTC)", guild.Name, guild.Status, guild.NextRunDate?.ToString("yyyy-MM-dd HH:mm"));
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
                retStr += string.Format("\nPlayer : **{0}** - Status : **{1}** - Processing started by {3} at **{2}**(UTC)", queuePlayer.Name, queuePlayer.Status, queuePlayer.NextRunDate?.ToString("yyyy-MM-dd HH:mm"),queuePlayer.ProcessingBy);

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
            if(retStr.Length==0) { ReplyAsync("Empty queue!!!!"); return; }
            await ReplyAsync($"{retStr}");
        }

        [Command("queue-remove")]
        //[Summary("Remove row from queue(Admin Command).\nUsage : ***$queue-remove {characterFullName}***")]
        [Summary("Remove row from queue(Admin Command)")]
        [Remarks("*queue-remove {name}*")]
        public async Task RemoveQueue(string name)
        {
            name = name.Trim().ToLower();

            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
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

        [Command("player-update")]
        [Summary("Add a player for reload(Admin Command)")]
        [Remarks("*player-update {playerUserName}*")]
        public async Task SetPlayerUpdate(string playerUserName)
        {
            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
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

        [Command("guild-update")]
        [Summary("Set a guild for reload")]
        [Remarks("*guild-update {guildName}*")]
        public async Task SetGuildUpdate(string guildName)
        {
            string retStr = "";

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
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

        [Command("mem")]
        [Summary("Check application diagnostics")]
        [Remarks("*mem*")]
        public async Task CheckDiagnostics()
        {
            string retStr = "";
            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
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

        [Command("prune")]
        [Summary("Delete messages")]
        [Remarks("*prune {number of messages}*")]
        public async Task Prune(string countMessagesToDelete) 
        {
            bool rowsIsNumber = int.TryParse(countMessagesToDelete, out int rows);
            if (!rowsIsNumber) { await ReplyAsync($"If you want to specify how many results want, you have to put a number as a parameter! '{rows}' is not a number!"); return; }

            string retStr = "";
            await Context.Message.DeleteAsync();

            //check if user is in role in order to proceed with the action
            var adminRole = IResolver.Current.ApplicationSettings.Get().DiscordSettings.BotAdminRole;
            var userAllowed = Roles.UserInRole(Context, adminRole);
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
    
    }
}
