using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using TripleZero.Infrastructure.DI;
using TripleZero.Helper;
using SWGoH.Model.Enums;

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

            if (result == null)
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
    }
}
