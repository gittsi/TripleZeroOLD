using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwGoh;
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
using static SwGoh.Enums.QueueEnum;
using SwGoh.Enums;
using TripleZero.Configuration;

namespace TripleZero.Repository
{
    public class MongoDBRepository : IMongoDBRepository
    {
        private IMapper _Mapper;
        public ApplicationSettingsModel appSettings = IResolver.Current.ApplicationSettings.Get();

        public MongoDBRepository(IMappingConfiguration mappingConfiguration)
        {
            _Mapper = mappingConfiguration.GetConfigureMapper();
        }        

        private string BuildApiUrl(string collection, string query="",string orderBy="",string limit="", string fields="")
        {
            string url = string.Format("https://api.mlab.com/api/1/databases/{0}/collections/{1}/?apiKey={2}{3}{4}{5}{6}"
                , appSettings.MongoDBSettings.DB                
                , collection
                , appSettings.MongoDBSettings.ApiKey
                , query
                , orderBy
                , limit
                ,fields);
            return url;
        }
        private string BuildApiUrlFromId(string collection,string id)
        {
            //var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character/{0}?apiKey={1}", characterConfig.Id, apiKey);
            string url = string.Format("https://api.mlab.com/api/1/databases/{0}/collections/{1}/{2}?apiKey={3}"
                , appSettings.MongoDBSettings.DB
                , collection
                ,id
                , appSettings.MongoDBSettings.ApiKey
                );
            return url;
        }
        public async Task<PlayerDto> GetPlayer(string userName)
        {
            await Task.FromResult(1);

            var queryData = string.Concat("&q={\"PlayerName\":\"", userName, "\"}");
            var orderby = "&s={\"LastSwGohUpdated\":-1}";
            var limit = "&l=1";            

            string url = BuildApiUrl("Player",queryData,orderby,limit,null);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<PlayerDto> ret = JsonConvert.DeserializeObject<List<PlayerDto>>(response, Converter.Settings);

                    return ret.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public async Task<GuildDto> GetGuildPlayers(string guildName)
        {
            await Task.FromResult(1);

            var queryData = string.Concat("&q={\"Name\":\"", guildName, "\"}");
            var orderby = "&s={\"LastSwGohUpdated\":-1}";
            var limit = "&l=1";            

            string url = BuildApiUrl("Guild", queryData, orderby, limit, null);
            //string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Guild/?{0}&{1}&{2}&apiKey={3}", queryData, orderby, limit, apiKey);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    GuildDto ret = JsonConvert.DeserializeObject<List<GuildDto>>(response, Converter.Settings).FirstOrDefault();

                    return ret;
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
                        var queue = BsonSerializer.Deserialize<Queue>(document);

                        return queue?.Id.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
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
                List<Queue> queue = document.Select(b => BsonSerializer.Deserialize<Queue>(b)).ToList();

                return queue;
            }
        }
        public async Task<Queue> RemoveFromQueue(string name)
        {
            var queue = GetQueue().Result.Where(p=>p.Name==name && p.Status== QueueStatus.PendingProcess).FirstOrDefault();
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
            CharacterConfig characterConfig = IResolver.Current.CharacterConfig.GetCharacterConfigByName(characterFullName).Result;
            if (characterConfig == null) return null;

            characterConfig.Aliases.Add(alias);
            var result = PutCharacterConfig(characterConfig).Result;
            if (!result) return null;

            characterConfig = await IResolver.Current.CharacterConfig.GetCharacterConfigByName(characterFullName);
            return characterConfig;
        }
        public async Task<CharacterConfig> RemoveCharacterAlias(string characterFullName, string alias)
        {
            CharacterConfig characterConfig = IResolver.Current.CharacterConfig.GetCharacterConfigByName(characterFullName).Result;
            if (characterConfig == null) return null;

            bool isRemoved = characterConfig.Aliases.Remove(alias);
            if (!isRemoved) return null;
            var result = PutCharacterConfig(characterConfig).Result;
            if (!result) return null;

            characterConfig = await IResolver.Current.CharacterConfig.GetCharacterConfigByName(characterFullName);
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
        public async Task<List<PlayerDto>> GetAllPlayersWithoutCharacters()
        {
            await Task.FromResult(1);

            var orderby = "&s={\"LastSwGohUpdated\":-1}";            
            var fields = "&f={\"Characters\": 0}";

            string url = BuildApiUrl("Player",null, orderby, null, fields);
            //string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Player/?{0}&{1}&apiKey={2}", fields, orderby, apiKey);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<PlayerDto> ret = JsonConvert.DeserializeObject<List<PlayerDto>>(response, Converter.Settings);

                    return ret;
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
                    List<CharacterConfig> ret = document.Select(b => BsonSerializer.Deserialize<CharacterConfig>(b)).ToList();

                    return ret.OrderBy(p => p.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
