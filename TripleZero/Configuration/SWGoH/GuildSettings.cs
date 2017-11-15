﻿using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Infrastructure.DI;

namespace TripleZero.Configuration
{
    public class GuildSettings
    {
        public async Task<List<GuildConfig>> GetGuildsConfig()
        {
            await Task.FromResult(1);

            var result = IResolver.Current.MongoDBRepository.GetGuildsConfig().Result;
            return result;
        }
        public async Task<GuildConfig> GetGuildConfigByName(string name)
        {
            await Task.FromResult(1);

            var result = IResolver.Current.MongoDBRepository.GetGuildsConfig().Result;
            return result.Where(p => p.Name == name).FirstOrDefault();
        }
        public async Task<GuildConfig> GetGuildConfigByAlias(string alias)
        {
            await Task.FromResult(1);

            var result = IResolver.Current.MongoDBRepository.GetGuildsConfig().Result;
            return result.Where(p => p.Aliases.Contains(alias.ToLower())).FirstOrDefault();
        }
    }
}
