
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using SwGoh;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Helper;
using TripleZero.Infrastructure.DI;

namespace TripleZero.Configuration
{
    public class CharactersConfig
    {
        public async Task<List<CharacterConfig>> GetCharactersConfig()
        {

            var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;

            string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character/?apiKey={0}", apiKey);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);

                    List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                    List<CharacterConfig> ret = document.Select(b => BsonSerializer.Deserialize<CharacterConfig>(b)).ToList() ;

                    return ret.OrderBy(p=>p.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<CharacterConfig> GetCharacterConfigByAlias(string alias)
        {
            await Task.FromResult(1);

            var result = GetCharactersConfig().Result;
            return result.Where(p => p.Aliases.Contains(alias.ToLower())).FirstOrDefault();
            
        }

        public async Task<CharacterConfig> GetCharacterConfigByName(string name)
        {
            await Task.FromResult(1);

            var result = GetCharactersConfig().Result;
            return result.Where(p => p.Name ==name).FirstOrDefault();

        }
    }
}
