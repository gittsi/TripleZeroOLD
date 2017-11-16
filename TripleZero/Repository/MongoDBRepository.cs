using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SWGoH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TripleZero._Mapping;
using TripleZero.Infrastructure.DI;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using TripleZero.Configuration;
using SWGoH.Model;
using SWGoH.Model.Enums;
using TripleZero.Strategy;

namespace TripleZero.Repository
{
    public class MongoDBRepository : IMongoDBRepository
    {
        private IMapper _Mapper;
        private readonly ApplicationSettingsModel appSettings = IResolver.Current.ApplicationSettings.Get();
        private readonly CachingStrategyContext _CachingStrategyContext;
        private readonly CachingRepositoryStrategy _CachingRepositoryStrategy;        

        public MongoDBRepository(IMappingConfiguration mappingConfiguration, CachingStrategyContext cachingStrategyContext, CachingRepositoryStrategy cachingRepositoryStrategy)
        {
            _Mapper = mappingConfiguration.GetConfigureMapper();            
            _CachingStrategyContext = cachingStrategyContext;
            _CachingRepositoryStrategy = cachingRepositoryStrategy;
            //set strategy for caching
            _CachingStrategyContext.SetStrategy(_CachingRepositoryStrategy);
        }
        private string BuildApiUrl(string collection, string query = "", string orderBy = "", string limit = "", string fields = "")
        {
            string url = string.Format("https://api.mlab.com/api/1/databases/{0}/collections/{1}/?apiKey={2}&q={3}&s={4}&l={5}&f={6}"
                , appSettings.MongoDBSettings.DB
                , collection
                , appSettings.MongoDBSettings.ApiKey
                , query
                , orderBy
                , limit
                , fields);
            return url;
        }
        private string BuildApiUrlFromId(string collection, string id)
        {            
            string url = string.Format("https://api.mlab.com/api/1/databases/{0}/collections/{1}/{2}?apiKey={3}"
                , appSettings.MongoDBSettings.DB
                , collection
                , id
                , appSettings.MongoDBSettings.ApiKey
                );
            return url;
        }
        public async Task<Player> GetPlayer(string userName)
        {
            await Task.FromResult(1);

            //get from cache if possible
            string strCacheKey = string.Concat("GetPlayerRepo-", userName);
            var objCache = _CachingStrategyContext.CacheGetFromKey(strCacheKey);
            if (objCache != null)
            {                
                return (Player)objCache;
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
                    List<PlayerDto> ret = JsonConvert.DeserializeObject<List<PlayerDto>>(response, Converter.Settings);

                    var players = _Mapper.Map<List<Player>>(ret);
                    //load to cache
                    _CachingStrategyContext.CacheAdd(strCacheKey, players.FirstOrDefault());
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
                    GuildDto ret = JsonConvert.DeserializeObject<List<GuildDto>>(response, Converter.Settings).FirstOrDefault();

                    Guild guild = _Mapper.Map<Guild>(ret);

                    return guild;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<string> SendPlayerToQueue(string playerName)
        {
            return await SendToQueue(playerName, QueueType.Player);
        }
        public async Task<string> SendGuildToQueue(string guildName)
        {
            return await SendToQueue(guildName, QueueType.Guild);
        }
        private async Task<string> SendToQueue(string name, QueueType queueType)
        {

            JObject data = new JObject(
                    new JProperty("Name", name),
                    new JProperty("InsertedDate", DateTime.UtcNow),
                    new JProperty("ProcessingStartDate", null),
                    new JProperty("NextRunDate", DateTime.UtcNow),
                    new JProperty("Status", QueueStatus.PendingProcess),
                    new JProperty("Priority", 3),
                    new JProperty("Command", queueType == QueueType.Player ? Command.UpdatePlayer : Command.UpdateGuildWithNoChars),
                    new JProperty("Type", queueType)
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
        public async Task<List<Player>> GetAllPlayersWithoutCharacters()
        {
            await Task.FromResult(1);

            var orderby = "{\"LastSwGohUpdated\":-1}";
            var fields = "{\"Characters\": 0}";

            string url = BuildApiUrl("Player", null, orderby, null, fields);
            //string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Player/?{0}&{1}&apiKey={2}", fields, orderby, apiKey);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<PlayerDto> ret = JsonConvert.DeserializeObject<List<PlayerDto>>(response, Converter.Settings);

                    var players = _Mapper.Map<List<Player>>(ret);
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
                    return charactersConfig;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<List<GuildConfig>> GetGuildsConfig()
        {
            string url = BuildApiUrl("Config.Guild", null, null, null, null);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<GuildConfigDto> ret = JsonConvert.DeserializeObject<List<GuildConfigDto>>(response, Converter.Settings);

                    List<GuildConfig> guildsConfig = _Mapper.Map<List<GuildConfig>>(ret);
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
