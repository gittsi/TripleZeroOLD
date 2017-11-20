﻿using AutoMapper;
using Newtonsoft.Json.Linq;
using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TripleZero.Repository._Mapping;
using TripleZero.Repository.Dto;
using TripleZero.Repository.Infrastructure.DI;

namespace TripleZero.Repository
{
    public class SWGoHRepository : ISWGoHRepository
    {
        private IMapper _Mapper = IResolver.Current.MappingConfiguration.GetConfigureMapper();        
        public async Task<GuildCharacter> GetGuildCharacter(int guildId, string characterName)
        {
            List<GuildCharacter> chars = null;
            chars = await GetGuildCharacters(guildId);
            var res = chars.Where(p => p.CharacterName.ToLower() == characterName.ToLower()).FirstOrDefault();

            return res;
        }
        public async Task<List<GuildCharacter>> GetGuildCharacters(int guildId)
        {
            var configCharacters = await IResolver.Current.CharacterSettings.GetCharactersConfig();

            var url = string.Format("https://swgoh.gg/api/guilds/{0}/units/", guildId.ToString());
            List<GuildCharacterDto> chars = new List<GuildCharacterDto>();
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                HttpContent content = response.Content;
                string reqResult = await content.ReadAsStringAsync();

                JObject json = new JObject();
                try
                {
                    json = JObject.Parse(reqResult);
                }
                catch (Exception ex)
                {
                    //swallow the error                    
                    return null;
                }

                foreach (var row in json)
                {
                    var charName = row.Key;
                    if(configCharacters.Where(p => p.Command?.ToLower() == row.Key.ToLower())!=null && configCharacters.Where(p => p.Command?.ToLower() == row.Key.ToLower()).Count()>0)
                    {
                        charName = configCharacters.Where(p => p.Command?.ToLower() == row.Key.ToLower()).FirstOrDefault().Name;
                    }

                    GuildCharacterDto gc = new GuildCharacterDto
                    {
                        Name = charName
                    };

                    List<GuildPlayerCharacterDto> players = new List<GuildPlayerCharacterDto>();
                    foreach (var player in row.Value)
                    {
                        players.Add(new GuildPlayerCharacterDto { Name = player["player"].ToString(), Level = (int)player["level"], Power = (int)player["power"], Rarity = (int)player["rarity"], Combat_Type = (int)player["combat_type"] });
                        gc.Players = players;
                    }
                    chars.Add(gc);
                }
            }
            List<GuildCharacter> guildCharacters = _Mapper.Map<List<GuildCharacter>>(chars);

            return guildCharacters;
        }
    }
}