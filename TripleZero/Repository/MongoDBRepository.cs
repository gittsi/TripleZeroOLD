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
            var orderby= "s={\"LastUpdated\":1}";
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

        public async Task<List<string>> GetGuildPlayers(string guildName)
        {
            await Task.FromResult(1);

            var queryData = string.Concat("q={\"Name\":\"", guildName, "\"}");
            var orderby = "s={\"LastUpdated\":1}";
            var limit = "l=1";
            var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;

            string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Guild/?{0}&{1}&{2}&apiKey={3}", queryData, orderby, limit, apiKey);

            return null;

            //var filter = "*.json";
            //string path = string.Format("{0}/_Data/{1}/", Directory.GetCurrentDirectory(), guildName);
            //string[] files = Directory.GetFiles(path, filter);

            //for (int i = 0; i < files.Length; i++)
            //{
            //    var lastUpdate = File.GetLastWriteTimeUtc(files[i]).ToString("yyyy-MM-dd HH:mm:ss");
            //    files[i] = string.Format("{0} - Last update : {1}", Path.GetFileName(files[i].Replace(".json", "")), lastUpdate);
            //}
            //return files.ToList();
        }

        
    }
}
