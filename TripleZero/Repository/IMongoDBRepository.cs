using SwGoh;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TripleZero.Repository
{
    public interface IMongoDBRepository
    {
        Task<PlayerDto> GetPlayer(string userName);
        Task<GuildDto> GetGuildPlayers(string guildName);
        Task<List<PlayerDto>> GetAllPlayersWithoutCharacters();
        Task<string> SendPlayerToQueue(string playerName);
        Task<string> SendGuildToQueue(string guildName);
        Task<List<Queue>> GetQueue();
        Task<CharacterConfig> SetCharacterAlias(string characterFullName, string alias);
        Task<CharacterConfig> RemoveCharacterAlias(string characterFullName, string alias);
    }
}
