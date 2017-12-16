using SWGoH.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TripleZero.Repository
{
    public interface ISWGoHRepository
    {
        Task<List<GuildUnit>> GetGuildUnits(int guildName);
        Task<GuildUnit> GetGuildUnit(int guildName, string characterName);
    }
}
