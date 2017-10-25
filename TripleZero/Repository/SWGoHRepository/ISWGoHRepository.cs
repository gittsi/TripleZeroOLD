using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Repository.Dto;

namespace TripleZero.Repository.SWGoHRepository
{
    public interface ISWGoHRepository
    {
        Task<List<GuildCharacterDto>> GetGuild(int guildName, string characterName);
    }
}
