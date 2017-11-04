using SwGoh;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Repository.Dto;

namespace TripleZero.Repository
{
    public interface IMongoDBRepository
    {
        Task<PlayerDto> GetPlayer(string userName);

        Task<GuildDto> GetGuildPlayers(string guildName);

     //   Task<GuildConfig> GetConfigGuild(string guildAlias);
    }
}
