
using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Repository.Infrastructure.DI;

namespace TripleZero.Repository.Configuration
{
    public class ShipSettings
    {
        public async Task<ShipConfig> GetShipConfigByAlias(string alias)
        {
            await Task.FromResult(1);

            var result = IResolver.Current.MongoDBRepository.GetShipsConfig().Result;
            return result.Where(p => p.Aliases.Contains(alias.ToLower())).FirstOrDefault();
        }
        public async Task<ShipConfig> GetShipConfigByName(string name)
        {
            await Task.FromResult(1);

            var result = IResolver.Current.MongoDBRepository.GetShipsConfig().Result;
            return result.Where(p => p.Name.ToLower() == name.ToLower()).FirstOrDefault();
        }
        public async Task<List<ShipConfig>> GetShipConfig()
        {
            await Task.FromResult(1);

            var result = IResolver.Current.MongoDBRepository.GetShipsConfig().Result;
            return result;
        }
    }
}
