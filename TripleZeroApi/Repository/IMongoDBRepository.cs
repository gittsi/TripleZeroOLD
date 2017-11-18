using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using TripleZeroApi.Models;
using SWGoH;

namespace TripleZeroApi.Repository
{
    public interface IMongoDBRepository
    {
        Task<PlayerDto> Player_Add(PlayerDto player);
        Task<PlayerDto> Player_Get(string playerName);
    }
}
