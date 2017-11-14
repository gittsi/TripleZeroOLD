
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using SWGoH;
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
        public async Task<CharacterConfig> GetCharacterConfigByAlias(string alias)
        {
            await Task.FromResult(1);

            var result = IResolver.Current.MongoDBRepository.GetCharactersConfig().Result;
            return result.Where(p => p.Aliases.Contains(alias.ToLower())).FirstOrDefault();            
        }

        public async Task<CharacterConfig> GetCharacterConfigByName(string name)
        {
            await Task.FromResult(1);

            var result = IResolver.Current.MongoDBRepository.GetCharactersConfig().Result;
            return result.Where(p => p.Name ==name).FirstOrDefault();
        }

        public async Task<List<CharacterConfig>> GetCharactersConfig()
        {
            await Task.FromResult(1);

            var result = IResolver.Current.MongoDBRepository.GetCharactersConfig().Result;
            return result;
        }
    }
}
