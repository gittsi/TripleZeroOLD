using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Repository.Dto;

namespace TripleZero.Repository.SWGoHRepository
{
    public interface ISWGoHRepository
    {
        Task<List<GuildCharacterDto>> GetGuildCharacters(int guildName);
        Task<GuildCharacterDto> GetGuildCharacter(int guildName, string characterName);
        Task<CharacterDto> GetCharacter(string userName, string characterName);
    }
}
