using AutoMapper;
using Discord.Rest;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TripleZero._Mapping;
using TripleZero.Repository.Dto;

namespace TripleZero.Repository.SWGoHRepository
{
    public class SWGoHRepository : ISWGoHRepository
    {
        private IMapper _Mapper;

        public SWGoHRepository(IMappingConfiguration mappingConfiguration)
        {
            _Mapper = mappingConfiguration.GetConfigureMapper();
        }        

        public async Task<List<GuildCharacterDto>> GetGuild(int guildId, string characterName)
        {
            var url = string.Format("https://swgoh.gg/api/guilds/{0}/units/", guildId.ToString());
            List<GuildCharacterDto> chars = new List<GuildCharacterDto>();
            using (var client = new HttpClient())
            {
                HttpResponseMessage response2 = await client.GetAsync(url);
                HttpContent content = response2.Content;
                string reqResult = await content.ReadAsStringAsync();


                JObject json = new JObject();
                try
                {
                    json = JObject.Parse(reqResult);
                }
                catch(Exception ex)
                {
                    //swallow the error
                    return chars;
                }                

                foreach (var row in json)
                {
                    GuildCharacterDto gc = new GuildCharacterDto();
                    gc.Name = row.Key;

                    List<GuildPlayerCharacterDto> players = new List<GuildPlayerCharacterDto>();
                    foreach (var player in row.Value)
                    {
                        players.Add( new GuildPlayerCharacterDto{ Name = player["player"].ToString(), Level = (int)player["level"], Power = (int)player["power"], Rarity = (int)player["rarity"] } );
                        gc.Players = players;
                    }
                    chars.Add(gc);
                }
            }

            var res = chars.Where(p => p.Name.ToLower() == characterName.ToLower());

            //var guildCharacters = _Mapper.Map<List<GuildCharacterDto>>(res);

            //return guildCharacters;
            return res.ToList();
        }

    }
}
