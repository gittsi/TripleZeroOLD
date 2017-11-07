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
using TripleZero.Model;
using TripleZero.Repository.Dto;
using TripleZero.Repository;
using MongoDB.Bson;
using SWGoH;
using MongoDB.Bson.Serialization;

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
            JObject data = new JObject(
                new JProperty("PlayerName", playerName),
                new JProperty("Date", DateTime.UtcNow),
                new JProperty("Status", 0),
                new JProperty("Priority", 1),
                new JProperty("Command", "up")
           );

            using (HttpClient client = new HttpClient())
            {
                var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;

                var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue.Player?apiKey={0}", apiKey);

                try
                {
                    using (HttpResponseMessage response = await client.PostAsync(requestUri, httpContent))
                    {
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();

                        BsonDocument document = BsonSerializer.Deserialize<BsonDocument>(responseBody);
                        var queuePlayer = BsonSerializer.Deserialize<QueuePlayer>(document);

                        return queuePlayer==null ? null : queuePlayer.Id.ToString();
                    }
                }
                catch(Exception ex)
                {
                    return null;
                }
                
            }
        }
    }
}
