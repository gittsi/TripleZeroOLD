using AutoMapper;
using Newtonsoft.Json.Linq;
using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TripleZero.Core.Caching;
using TripleZero.Repository.Dto;
using TripleZero.Repository.Infrastructure.DI;

namespace TripleZero.Repository
{
    public class SWGoHRepository : ISWGoHRepository
    {
        private CacheClient cacheClient = IResolver.Current.CacheClient;

        private IMapper _Mapper = IResolver.Current.MappingConfiguration.GetConfigureMapper();        
        public async Task<GuildUnit> GetGuildUnit(int guildId, string characterName)
        {
            List<GuildUnit> chars = null;
            chars = await GetGuildUnits(guildId);
            var res = chars.Where(p => p.Name.ToLower() == characterName.ToLower()).FirstOrDefault();

            return res;
        }
        public async Task<List<GuildUnit>> GetGuildUnits(int guildId)
        {
            string functionName = "GetGuildUnitsRepo";
            string key = guildId.ToString();
            var objCache = cacheClient.GetDataFromRepositoryCache(functionName, key);
            if (objCache != null)
            {
                var _guildCharacters = (List<GuildUnit>)objCache;
                _guildCharacters.ForEach(p => p.LoadedFromCache = true);
                return _guildCharacters;
            }

            var configCharacters = await IResolver.Current.CharacterSettings.GetCharactersConfig();
            var configShips = await IResolver.Current.ShipSettings.GetShipConfig();

            var url = string.Format("https://swgoh.gg/api/guilds/{0}/units/", guildId.ToString());
            List<GuildUnitDto> chars = new List<GuildUnitDto>();
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
                    var unitName = row.Key;
                    if(configCharacters.Where(p => p.Command?.ToLower() == row.Key.ToLower())!=null && configCharacters.Where(p => p.Command?.ToLower() == row.Key.ToLower()).Count()>0)                    
                    {
                        unitName = configCharacters.Where(p => p.Command?.ToLower() == row.Key.ToLower()).FirstOrDefault().Name;
                    }
                    else if (configShips.Where(p => p.Command?.ToLower() == row.Key.ToLower()) != null && configShips.Where(p => p.Command?.ToLower() == row.Key.ToLower()).Count() > 0)
                        {
                        unitName = configShips.Where(p => p.Command?.ToLower() == row.Key.ToLower()).FirstOrDefault().Name;
                    }

                    GuildUnitDto gc = new GuildUnitDto
                    {
                        Name = unitName
                    };

                    List<GuildPlayerUnitDto> players = new List<GuildPlayerUnitDto>();
                    foreach (var player in row.Value)
                    {
                        players.Add(new GuildPlayerUnitDto { Name = player["player"].ToString(), Level = (int)player["level"], Power = (int)player["power"], Rarity = (int)player["rarity"], Combat_Type = (int)player["combat_type"] });
                        gc.Players = players;
                    }
                    chars.Add(gc);
                }
            }
            List<GuildUnit> guildCharacters = _Mapper.Map<List<GuildUnit>>(chars);

            await cacheClient.AddToRepositoryCache(functionName, key, guildCharacters, 30);
            return guildCharacters;
        }       
    }
}
