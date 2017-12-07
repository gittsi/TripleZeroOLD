using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using SWGoH.Model;
using TripleZero.Repository.Infrastructure.DI;
using TripleZero.Repository.Dto;
using SWGoH.Model.Enums;
using TripleZero.Repository.Helper;
using TripleZero.Core.Settings;
using TripleZero.Core.Caching;

namespace TripleZero.Repository
{
    public class MongoDBRepository : IMongoDBRepository
    {
        private CacheClient cacheClient = IResolver.Current.CacheClient;

        private IMapper _Mapper = IResolver.Current.MappingConfiguration.GetConfigureMapper();
        private readonly SettingsTripleZeroRepository settingsTripleZeroRepository = IResolver.Current.ApplicationSettings.GetTripleZeroRepositorySettings();
        private string BuildApiUrl(string collection, string query = "", string orderBy = "", string limit = "", string fields = "")
        {
            if (string.IsNullOrWhiteSpace(limit)) limit = "1000";
            string url = string.Format("https://api.mlab.com/api/1/databases/{0}/collections/{1}/?apiKey={2}&q={3}&s={4}&l={5}&f={6}"
                , settingsTripleZeroRepository.MongoDBSettings.DB
                , collection
                , settingsTripleZeroRepository.MongoDBSettings.ApiKey
                , query
                , orderBy
                , limit
                , fields);
            return url;
        }
        private string BuildApiUrlFromId(string collection, string id)
        {            
            string url = string.Format("https://api.mlab.com/api/1/databases/{0}/collections/{1}/{2}?apiKey={3}"
                , settingsTripleZeroRepository.MongoDBSettings.DB
                , collection
                , id
                , settingsTripleZeroRepository.MongoDBSettings.ApiKey
                );
            return url;
        }
        private async Task<string> SendToQueue(string name, QueueType queueType, Command command)
        {

            JObject data = new JObject(
                    new JProperty("Name", name),
                    new JProperty("InsertedDate", DateTime.UtcNow),
                    new JProperty("ProcessingStartDate", null),
                    new JProperty("NextRunDate", DateTime.UtcNow),
                    new JProperty("Status", QueueStatus.PendingProcess),
                    new JProperty("Priority", Priority.ManualLoad),
                    new JProperty("Command", command),
                    new JProperty("Type", queueType),
                    new JProperty("ComputerName", string.Empty)
               );

            using (HttpClient client = new HttpClient())
            {
                var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                string requestUri = BuildApiUrl("Queue", null, null, null, null);
                //var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue?apiKey={0}", apiKey);

                try
                {
                    using (HttpResponseMessage response = await client.PostAsync(requestUri, httpContent))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(responseBody);
                        var queue = BsonSerializer.Deserialize<QueueDto>(document);

                        return queue?.Id.ToString();
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
            }
        }
        public async Task<Player> GetPlayer(string userName)
        {
            await Task.FromResult(1);
           
            string functionName = "GetPlayerRepo";
            string key = userName;
            var objCache = cacheClient.GetDataFromRepositoryCache(functionName, key);
            if (objCache != null)
            {
                var player = (Player)objCache;
                player.LoadedFromCache = true;
                return player;
            }

            var queryData = string.Concat("{\"PlayerName\":\"", userName, "\"}");
            var orderby = "{\"LastSwGohUpdated\":-1}";
            var limit = "1";

            string url = BuildApiUrl("Player", queryData, orderby, limit, null);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<PlayerDto> ret = JsonConvert.DeserializeObject<List<PlayerDto>>(response, JSonConverterSettings.Settings);

                    var players = _Mapper.Map<List<Player>>(ret);
                    if (players == null || players.Count == 0) return players.FirstOrDefault();
                    //load to cache
                    await cacheClient.AddToRepositoryCache(functionName, key, players.FirstOrDefault());
                    return players.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<Guild> GetGuildPlayers(string guildName)
        {
            await Task.FromResult(1);

            string functionName = "GetGuildPlayersRepo";
            string key = guildName;
            var objCache = cacheClient.GetDataFromRepositoryCache(functionName, key);
            if (objCache != null)
            {
                var guild = (Guild)objCache;
                guild.LoadedFromCache = true;
                return guild;
            }

            var queryData = string.Concat("{\"Name\":\"", guildName, "\"}");
            var orderby = "{\"LastSwGohUpdated\":-1}";
            var limit = "1";

            string url = BuildApiUrl("Guild", queryData, orderby, limit, null);
            //string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Guild/?{0}&{1}&{2}&apiKey={3}", queryData, orderby, limit, apiKey);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    GuildDto ret = JsonConvert.DeserializeObject<List<GuildDto>>(response, JSonConverterSettings.Settings).FirstOrDefault();

                    Guild guild = _Mapper.Map<Guild>(ret);

                    //load to cache
                    await cacheClient.AddToRepositoryCache(functionName, key, guild);
                    return guild;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<string> SendCharacterConfigToQueue()
        {
            return await SendToQueue("Aramil", QueueType.Player,Command.GetNewCharacters);
        }
        public async Task<string> SendPlayerToQueue(string playerName)
        {
            return await SendToQueue(playerName, QueueType.Player,Command.UpdatePlayer);
        }
        public async Task<string> SendGuildToQueue(string guildName)
        {
            return await SendToQueue(guildName, QueueType.Guild,Command.UpdateGuildWithNoChars);
        }        
        public async Task<List<Queue>> GetQueue()
        {
            string requestUri = BuildApiUrl("Queue", null, null, null, null);
            //var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue/?apiKey={0}", apiKey);

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(requestUri);
                string responseBody = await response.Content.ReadAsStringAsync();
                List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(responseBody);
                List<QueueDto> queueDto = document.Select(b => BsonSerializer.Deserialize<QueueDto>(b)).ToList();

                List<Queue> queue = null;
                try
                {
                    queue = _Mapper.Map<List<Queue>>(queueDto);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }

                return queue;
            }
        }
        public async Task<Queue> RemoveFromQueue(string name)
        {
            var queue = GetQueue().Result.Where(p => p.Name == name && p.Status == QueueStatus.PendingProcess).FirstOrDefault();
            if (queue == null || queue.Id == null) return null;

            string requestUri = BuildApiUrlFromId("Queue", queue.Id.ToString());
            //var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue/{0}?apiKey={1}", queue.Id, apiKey);
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage updateresult = await client.DeleteAsync(requestUri);

                if (updateresult.StatusCode == HttpStatusCode.OK) return queue; else return null;
            }
        }
        public async Task<CharacterConfig> SetCharacterAlias(string characterFullName, string alias)
        {
            CharacterConfig characterConfig = IResolver.Current.CharacterSettings.GetCharacterConfigByName(characterFullName).Result;
            if (characterConfig == null) return null;

            characterConfig.Aliases.Add(alias);
            var result = PutCharacterConfig(characterConfig).Result;
            if (!result) return null;

            characterConfig = await IResolver.Current.CharacterSettings.GetCharacterConfigByName(characterFullName);
            return characterConfig;
        }
        public async Task<CharacterConfig> RemoveCharacterAlias(string characterFullName, string alias)
        {
            CharacterConfig characterConfig = IResolver.Current.CharacterSettings.GetCharacterConfigByName(characterFullName).Result;
            if (characterConfig == null) return null;

            bool isRemoved = characterConfig.Aliases.Remove(alias);
            if (!isRemoved) return null;
            var result = PutCharacterConfig(characterConfig).Result;
            if (!result) return null;

            characterConfig = await IResolver.Current.CharacterSettings.GetCharacterConfigByName(characterFullName);
            return characterConfig;
        }
        public async Task<CharacterConfig> SetCharacterCommand(string characterFullName, string command)
        {
            CharacterConfig characterConfig = IResolver.Current.CharacterSettings.GetCharacterConfigByName(characterFullName).Result;
            if (characterConfig == null) return null;

            characterConfig.Command=command;
            var result = PutCharacterConfig(characterConfig).Result;
            if (!result) return null;

            characterConfig = await IResolver.Current.CharacterSettings.GetCharacterConfigByName(characterFullName);
            return characterConfig;
        }
        public async Task<CharacterConfig> RemoveCharacterCommand(string characterFullName)
        {
            CharacterConfig characterConfig = IResolver.Current.CharacterSettings.GetCharacterConfigByName(characterFullName).Result;
            if (characterConfig == null) return null;

            characterConfig.Command="";            
            var result = PutCharacterConfig(characterConfig).Result;
            if (!result) return null;

            characterConfig = await IResolver.Current.CharacterSettings.GetCharacterConfigByName(characterFullName);
            return characterConfig;
        }
        private async Task<bool> PutCharacterConfig(CharacterConfig characterConfig)
        {
            JObject data = null;
            try
            {
                data = new JObject(
                                       new JProperty("Name", characterConfig.Name),
                                       new JProperty("Command", characterConfig.Command),
                                       new JProperty("SWGoHUrl", characterConfig.SWGoHUrl),
                                       new JProperty("Aliases", characterConfig.Aliases)
                                       );
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

            var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");

            string requestUri = BuildApiUrlFromId("Config.Character", characterConfig.Id.ToString());
            //var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character/{0}?apiKey={1}", characterConfig.Id, apiKey);
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage updateresult = await client.PutAsync(requestUri, httpContent);

                if (updateresult.StatusCode == HttpStatusCode.OK) return true; else return false;
            }
        }
        public async Task<IEnumerable<Player>> GetGuildCharactersAbilities(List<string> playersName)
        {
            await Task.FromResult(1);

            string functionName = "GetGuildCharactersAbilitiesRepo";
            string key = $"{HashKey.GetStringSha256Hash(string.Join("",playersName))}";
            var objCache = cacheClient.GetDataFromRepositoryCache(functionName, key);
            if (objCache != null)
            {
                var players = (List<Player>)objCache;
                ////var retGuild = new List<Player>(guild);
                //List<Player> retGuild=  new List<Player>();
                //retGuild.AddRange(guild);

                players.ForEach(p => p.LoadedFromCache = true);
                return players;
            }

            //var queryData = string.Concat("{\"Characters.Nm\":\"", characterFullName, "\"}"); didn't work
            var orderby = "{\"LastSwGohUpdated\":-1}";
            var fields = "{\"PlayerName\": 1,\"PlayerNameInGame\": 1,\"Characters.Ab\": 1,\"Characters.Nm\": 1,\"Characters.Lvl\": 1}";

            string url = BuildApiUrl("Player", null /*queryData*/, orderby, null, fields);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<PlayerDto> ret = JsonConvert.DeserializeObject<List<PlayerDto>>(response, JSonConverterSettings.Settings);
                    List<PlayerDto> p = ret.Where(pl => playersName.Contains(pl.PlayerName)).ToList();

                    var players = _Mapper.Map<List<Player>>(p);
                    List<Player> retPlayers = new List<Player>();                    
                    //load to cache
                    await cacheClient.AddToRepositoryCache(functionName, key, players, 30);
                    return players;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<IEnumerable<Player>> GetGuildCharacterAbilities(List<string> playersName,string characterFullName)
        {
            var players = await GetGuildCharactersAbilities(playersName);
            var retPlayers = from player in players
                             from character in player.Characters
                             where (character.Name == characterFullName)
                             select new Player { PlayerName= player.PlayerName, PlayerNameInGame= player.PlayerNameInGame, Characters = new List<Character>() { character } } ;            

            return retPlayers;            
        }
        public async Task<List<Player>> GetAllPlayersNoCharactersNoShips()
        {
            await Task.FromResult(1);

            string functionName = "GetAllPlayersWithoutCharactersRepo";
            string key = "key";
            var objCache = cacheClient.GetDataFromRepositoryCache(functionName, key);
            if (objCache != null)
            {
                var guild = (List<Player>)objCache;
                guild.ForEach(p => p.LoadedFromCache = true);                
                return guild;
            }

            var orderby = "{\"LastSwGohUpdated\":-1}";
            var fields = "{\"Characters\": 0,\"Ships\": 0}";

            string url = BuildApiUrl("Player", null, orderby, null, fields);
            //string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Player/?{0}&{1}&apiKey={2}", fields, orderby, apiKey);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<PlayerDto> ret = JsonConvert.DeserializeObject<List<PlayerDto>>(response, JSonConverterSettings.Settings);

                    var players = _Mapper.Map<List<Player>>(ret);
                    //load to cache
                    await cacheClient.AddToRepositoryCache(functionName, key, players,30);
                    return players;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<List<CharacterConfig>> GetCharactersConfig()
        {
            string functionName = "GetCharactersConfigRepo";
            string key = "key";
            var objCache = cacheClient.GetDataFromRepositoryCache(functionName, key);
            if (objCache != null)
            {
                var charactersConfig = (List<CharacterConfig>)objCache;
                charactersConfig.ForEach(p => p.LoadedFromCache = true);
                return charactersConfig;
            }

            string url = BuildApiUrl("Config.Character", null, null, null, null);
            //string url = string.Format("https://api.mlab.com/api/1/databases/{1}/collections/Config.Character/?apiKey={0}", apiKey, db);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);

                    List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                    List<CharacterConfigDto> ret = document.Select(b => BsonSerializer.Deserialize<CharacterConfigDto>(b)).ToList();

                    var charactersConfigDto = ret.OrderBy(p => p.Name).ToList();

                    List<CharacterConfig> charactersConfig = _Mapper.Map<List<CharacterConfig>>(charactersConfigDto);
                    //load to cache
                    await cacheClient.AddToRepositoryCache(functionName, key, charactersConfig, 30);
                    return charactersConfig;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<List<ShipConfig>> GetShipsConfig()
        {
            string functionName = "GetShipsConfigRepo";
            string key = "key";
            var objCache = cacheClient.GetDataFromRepositoryCache(functionName, key);
            if (objCache != null)
            {
                var shipsConfig = (List<ShipConfig>)objCache;
                shipsConfig.ForEach(p => p.LoadedFromCache = true);
                return shipsConfig;
            }

            string url = BuildApiUrl("Config.Ship", null, null, null, null);            

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);

                    List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                    List<ShipConfigDto> ret = document.Select(b => BsonSerializer.Deserialize<ShipConfigDto>(b)).ToList();

                    var shipsConfigDto = ret.OrderBy(p => p.Name).ToList();

                    List<ShipConfig> shipsConfig = _Mapper.Map<List<ShipConfig>>(shipsConfigDto);
                    //load to cache
                    await cacheClient.AddToRepositoryCache(functionName, key, shipsConfig, 30);
                    return shipsConfig;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<List<GuildConfig>> GetGuildsConfig()
        {
            string functionName = "GetGuildsConfigRepo";
            string key = "key";
            var objCache = cacheClient.GetDataFromRepositoryCache(functionName, key);
            if (objCache != null)
            {
                var guildsConfig = (List<GuildConfig>)objCache;
                guildsConfig.ForEach(p => p.LoadedFromCache = true);
                return guildsConfig;
            }

            string url = BuildApiUrl("Config.Guild", null, null, null, null);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<GuildConfigDto> ret = JsonConvert.DeserializeObject<List<GuildConfigDto>>(response, JSonConverterSettings.Settings);

                    List<GuildConfig> guildsConfig = _Mapper.Map<List<GuildConfig>>(ret);
                    //load to cache
                    await cacheClient.AddToRepositoryCache(functionName, key, guildsConfig, 30);
                    return guildsConfig;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
