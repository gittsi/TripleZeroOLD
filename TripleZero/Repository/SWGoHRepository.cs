using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwGoh;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TripleZero._Mapping;
using TripleZero.Helper;
using TripleZero.Repository.Dto;

namespace TripleZero.Repository
{
    public class SWGoHRepository : ISWGoHRepository
    {
        private IMapper _Mapper;

        public SWGoHRepository(IMappingConfiguration mappingConfiguration)
        {
            _Mapper = mappingConfiguration.GetConfigureMapper();
        }       


        public async Task<GuildCharacterDto> GetGuildCharacter(int guildId, string characterName)
        {
            List<GuildCharacterDto> chars = null;
            chars=await GetGuildCharacters(guildId);
            var res = chars.Where(p => p.Name.ToLower() == characterName.ToLower()).FirstOrDefault();

            return res;
        }

        public async Task<List<GuildCharacterDto>> GetGuildCharacters(int guildId)
        {
            var url = string.Format("https://swgoh.gg/api/guilds/{0}/units/", guildId.ToString());
            List<GuildCharacterDto> chars = new List<GuildCharacterDto>();
            using (var client = new HttpClient())
            {
                HttpResponseMessage response= await client.GetAsync(url);
                HttpContent content = response.Content;
                string reqResult = await content.ReadAsStringAsync();


                JObject json = new JObject();
                try
                {
                    json = JObject.Parse(reqResult);
                }
                catch(Exception ex)
                {
                    //swallow the error
                    Consoler.WriteLineInColor(ex.Message, ConsoleColor.Red);
                    return chars;
                }                

                foreach (var row in json)
                {
                    GuildCharacterDto gc = new GuildCharacterDto
                    {
                        Name = row.Key
                    };

                    List<GuildPlayerCharacterDto> players = new List<GuildPlayerCharacterDto>();
                    foreach (var player in row.Value)
                    {
                        players.Add( new GuildPlayerCharacterDto{ Name = player["player"].ToString(), Level = (int)player["level"], Power = (int)player["power"], Rarity = (int)player["rarity"] , Combat_Type=(int)player["combat_type"] } );
                        gc.Players = players;
                    }
                    chars.Add(gc);
                }
            }

            return chars;
        }

       

    }
}
