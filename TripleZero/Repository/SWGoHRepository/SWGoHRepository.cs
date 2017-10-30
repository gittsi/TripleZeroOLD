using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwGoh;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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

        public async Task<CharacterDto> GetCharacter(string userName, string characterCommand)
        {

            CharacterDto character = new CharacterDto();

            var url = string.Format("https://swgoh.gg/u/{1}/collection/{0}/", characterCommand,userName);

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                HttpContent content = response.Content;
                string reqResult = await content.ReadAsStringAsync();

                int index = reqResult.IndexOf("<h5>General</h5>", reqResult.Length / 2);

                if (index != -1)
                {
                    string retver = reqResult.Substring(index);
                    string health = "";
                    for (int i = 17; i < 160; i++)
                    {
                        if (char.IsNumber(retver[i]))
                        {
                            health += retver[i];
                        }
                    }
                    int.TryParse(health, out int healthValue);
                    character.Health = healthValue;

                    index = retver.IndexOf("Health");
                    if (index != -1)
                    {
                        retver = retver.Substring(index);
                        string protection = "";
                        for (int i = 0; i < 150; i++)
                        {
                            if (char.IsNumber(retver[i]))
                            {
                                protection += retver[i];
                            }
                        }
                        int.TryParse(protection, out int protectionValue);
                        character.Protection = protectionValue;

                    }

                    
                }
            }

            return character;
                

            


        }


        public async Task<GuildCharacterDto> GetGuildCharacter(int guildName, string characterName)
        {
            List<GuildCharacterDto> chars = null;
            chars=await GetGuildCharacters(guildName);
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

            return chars;
        }

        public async Task<PlayerDto> GetPlayer(string userName)
        {
            await Task.FromResult(1);

            string directory = Directory.GetCurrentDirectory() + "/_Data/Order 66 41st/";
            string fname = directory + userName + @".json";
            if (File.Exists(fname))
            {
                var lines = File.ReadAllText(fname);
                PlayerDto ret = JsonConvert.DeserializeObject<PlayerDto>(lines, Converter.Settings);
                return ret;
            }


            return null;

            
        }
    }
}
