using SWGoH;
using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TripleZero.Repository
{
    public interface IMongoDBRepository
    {
        Task<List<CharacterConfigDto>> GetCharactersConfig();
        Task<Player> GetPlayer(string userName);
        Task<GuildDto> GetGuildPlayers(string guildName);
        Task<List<Player>> GetAllPlayersWithoutCharacters();
        Task<string> SendPlayerToQueue(string playerName);
        Task<string> SendGuildToQueue(string guildName);
        Task<List<QueueDto>> GetQueue();
        Task<QueueDto> RemoveFromQueue(string name);
        Task<CharacterConfigDto> SetCharacterAlias(string characterFullName, string alias);
        Task<CharacterConfigDto> RemoveCharacterAlias(string characterFullName, string alias);
    }
}
