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
using TripleZero.Helper;
using static SwGoh.Enums.QueueEnum;

namespace TripleZero.Modules
{
    [Name("Admin")]
    [Summary("Admin Commands")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        [Command("alias-remove")]
        //[Summary("Set alias for specific character(Admin Command).\nUsage : ***$alias -set {characterFullName}***")]
        [Summary("Remove alias for specific character(Admin Command)")]
        [Remarks("*alias-remove {characterFullName}*")]
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
                retStr += string.Format("\nCommand:**{0}**", result.Command.Length==0 ? "empty!!!": result.Command);
                retStr += string.Format("\nSWGoH url:**{0}**", result.SWGoHUrl);

                string aliases = "";
                int countAliases = 0;
                foreach (var _alias in result.Aliases)
                {
                    countAliases += 1;
                    aliases += _alias;
                    if (countAliases != result.Aliases.Count()) aliases += ", ";
                }

                retStr += string.Format("\nAliases: [**{0}**]", aliases.Count()>0 ? aliases : "empty!!!");
            }
            else
            {
                retStr = "Not updated. Probably something is wrong with your command!";
            }

            await ReplyAsync($"{retStr}");
        }

        [Command("alias-set")]
        //[Summary("Set alias for specific character(Admin Command).\nUsage : ***$alias -set {characterFullName}***")]
        [Summary("Set alias for specific character(Admin Command)")]
        [Remarks("*alias-set {characterFullName}*")]
        public async Task SetAlias(string characterFullName,string alias)
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

            if(result!=null)
            {
                retStr +=$"\nNew alias '**{alias}**' for '**{characterFullName}**' was added!\n";
                retStr += string.Format("\nName:**{0}**", result.Name);
                retStr += string.Format("\nCommand:**{0}**", result.Command.Length == 0 ? "empty!!!" : result.Command);
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
        [Summary("Get config for specific character(Admin Command)")]
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

            var charactersConfig = IResolver.Current.CharacterConfig.GetCharactersConfig().Result;
            int debugcount = 0;
            foreach(var characterConfig in charactersConfig)
            {
                chStr = string.Format("\n{0}", characterConfig.Name);
                string aliases = "";
                int countAliases = 0;
                foreach(var alias in characterConfig.Aliases)
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

                if(retStr.Length>1800)
                {
                    await ReplyAsync($"{retStr}");
                    retStr = "";
                }
            }

            await ReplyAsync($"{retStr}");
        }

        [Command("queue-get")]
        //[Summary("Set alias for specific character(Admin Command).\nUsage : ***$alias -set {characterFullName}***")]
        [Summary("Get current for specific character(Admin Command)")]
        [Remarks("*alias-remove {characterFullName}*")]
        public async Task GetQueue()
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

            var result = IResolver.Current.MongoDBRepository.GetQueue().Result;

            var guildQueues = result.Where(p => p.Type == QueueType.Guild);
            var playerQueues = result.Where(p => p.Type == QueueType.Player);

            if (result==null)
            {
                await ReplyAsync($"Problem!! Cannot get queue!!!");
                return;
            }
            
            retStr = "\n**Players**";
            foreach (var queuePlayer in playerQueues)
            {                
                retStr += string.Format("\nPlayer : **{0}** - Status : **{1}**", queuePlayer.Name, queuePlayer.Status);
            }

            await ReplyAsync($"{retStr}");
        }

        [Command("playerreload")]
        [Summary("Set a player for reload(Admin Command)")]
        [Remarks("*playerreload {playerUserName}*")]
        public async Task SetPlayerReload(string playerUserName)
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

        [Command("guildreload")]
        [Summary("Set a guild for reload")]
        [Remarks("*guildreload {guildName}*")]
        public async Task SetGuildReload(string guildName)
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
    }
}
