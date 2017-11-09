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

namespace TripleZero.Modules
{
    [Name("Admin")]
    [Summary("Admin Commands")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {        

        [Command("alias -set")]
        //[Summary("Set alias for specific character(Admin Command).\nUsage : ***$alias -set {characterFullName}***")]
        [Summary("Set alias for specific character(Admin Command)")]
        [Remarks("*alias -set {characterFullName}*")]
        public async Task SetAlias(string characterFullName,string alias)
        {
            characterFullName = characterFullName.Trim();
            alias = alias.Trim();

            string retStr = "";

            //check if user is in role in order to proceed with the action
            var userAllowed = Roles.UserInRole(Context, "botadmin");
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
                retStr += string.Format("\nCommand:**{0}**", result.Command);
                retStr += string.Format("\nSWGoH url:**{0}**", result.SWGoHUrl);

                string aliases = "";
                int countAliases = 0;
                foreach (var _alias in result.Aliases)
                {
                    countAliases += 1;
                    aliases += _alias;
                    if (countAliases != result.Aliases.Count()) aliases += ", ";
                }
                    
                retStr += string.Format("\nAliases: [**{0}**]", aliases);
            }
            else
            {
                retStr = "Not updated. Probably something is wrong with your command!";
            }

            await ReplyAsync($"{retStr}");

       
        }

        [Command("characters -config")]
        //[Summary("Get config for specific character(Admin Command).\nUsage : ***$characters -config***")]
        [Summary("Get config for specific character(Admin Command)")]
        [Remarks("*characters -config*")]
        public async Task GetCharacterConfig()
        {
            string retStr = "";
            string chStr = "";

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
    }
}
