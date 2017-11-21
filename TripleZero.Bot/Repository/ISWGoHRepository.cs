using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TripleZero.Repository
{
    public interface ISWGoHRepository
    {
        Task<List<GuildCharacter>> GetGuildCharacters(int guildName);
        Task<GuildCharacter> GetGuildCharacter(int guildName, string characterName);
    }
}
