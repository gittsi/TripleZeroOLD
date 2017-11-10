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
using TripleZero.Infrastructure.DI;
using TripleZero.Repository.Dto;
using TripleZero.Repository;
using MongoDB.Bson;
using SWGoH;
using MongoDB.Bson.Serialization;
using static SWGoH.Enums.QueueEnum;

namespace TripleZero.Repository
{
    public class MongoDBRepository : IMongoDBRepository
    {
        private IMapper _Mapper;

        public MongoDBRepository(IMappingConfiguration mappingConfiguration)
        {
            _Mapper = mappingConfiguration.GetConfigureMapper();
        }

        //public Task<GuildConfig> GetConfigGuild(string guildAlias)
        //{
        //    await Task.FromResult(1);

            
        //}

        public async Task<PlayerDto> GetPlayer(string userName)
        {
            await Task.FromResult(1);
  
            var queryData = string.Concat("q={\"PlayerName\":\"", userName,"\"}");
            var orderby= "s={\"LastSwGohUpdated\":-1}";
            var limit = "l=1";   
            var apiKey= IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;

            string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Player/?{0}&{1}&{2}&apiKey={3}", queryData, orderby,limit, apiKey);
         
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<PlayerDto> ret = JsonConvert.DeserializeObject<List<PlayerDto>>(response, Converter.Settings);

                    return ret.FirstOrDefault();
                }
            }catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);                
            }
            
        }

        public async Task<GuildDto> GetGuildPlayers(string guildName)
        {
            await Task.FromResult(1);

            var queryData = string.Concat("q={\"Name\":\"", guildName, "\"}");
            var orderby = "s={\"LastSwGohUpdated\":-1}";
            var limit = "l=1";
            var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;

            string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Guild/?{0}&{1}&{2}&apiKey={3}", queryData, orderby, limit, apiKey);

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
            SendToQueue
        }

        public async Task<string> SendGuildToQueue(string guildName)
        {

        }

        private async Task<string> SendToQueue(string name, QueueType queueType )
        {
            JObject data = new JObject(
                new JProperty("Name", name),
                new JProperty("Date", DateTime.UtcNow),
                new JProperty("Status", QueueStatus.PendingProcess),
                new JProperty("Priority", 3),
                new JProperty("Command", queueType== QueueType.Player ? "up" : "ugnochars"),
                new JProperty("Type", queueType)
           );

            using (HttpClient client = new HttpClient())
            {
                var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;

                var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue?apiKey={0}", apiKey);

                try
                {
                    using (HttpResponseMessage response = await client.PostAsync(requestUri, httpContent))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(responseBody);
                        var queuePlayer = BsonSerializer.Deserialize<Queue>(document);

                        return queuePlayer?.Id.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                    //throw new ApplicationException(ex.Message);                    
                }

            }
        }

        public async Task<CharacterConfig> SetCharacterAlias(string characterFullName, string alias)
        {
            var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;
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
            var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;
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
            var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;

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
            var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character/{0}?apiKey={1}", characterConfig.Id, apiKey);
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage updateresult = await client.PutAsync(requestUri, httpContent);

                if (updateresult.StatusCode == HttpStatusCode.OK) return true; else return false;
            }
        }

        public async Task<List<PlayerDto>> GetAllPlayersWithoutCharacters()
        {
            await Task.FromResult(1);
                       
            var orderby = "s={\"LastSwGohUpdated\":-1}";
            var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;
            var fields = "f={\"Characters\": 0}";

            string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Player/?{0}&{1}&apiKey={2}", fields, orderby, apiKey);

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
    }
}
