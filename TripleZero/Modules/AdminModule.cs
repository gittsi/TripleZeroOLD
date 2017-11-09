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

namespace TripleZero.Modules
{
    [Name("Admin")]
    [Summary("Do some admin work")]
    public class AdminModule : ModuleBase<SocketCommandContext>
    {        

        [Command("alias -set")]
        [Summary("Get alias for specific character(Admin Command).\nUsage : ***$alias -set {characterFullName}***")]
        public async Task SetAlias(string characterFullName,string alias)
        {
            var result = IResolver.Current.MongoDBRepository.SetCharacterAlias(characterFullName, alias.ToLower());

            int i = 1;
        }

        [Command("characters -config")]
        [Summary("Get config for specific character(Admin Command).\nUsage : ***$characters -config***")]
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
