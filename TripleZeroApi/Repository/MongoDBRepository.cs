using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using TripleZeroApi.Models;
using MongoDB.Driver;
using SwGoh;

namespace TripleZeroApi.Repository
{
    public class MongoDBRepository : IMongoDBRepository
    {

        public async Task<PlayerDto> Player_Add(PlayerDto player)
        {
            await Task.FromResult(1);

            MongoDBContext dbContext = new MongoDBContext();

            var aa = dbContext.TestCollection.Find(_ => true).ToList();
            //entity.Id = Guid.NewGuid();
            dbContext.TestCollection.InsertOne(new TestCollection() { test = "Asf" });

            dbContext.Players.InsertOne(player);

            return null;
        }

        public async Task<PlayerDto> Player_Get(string playerName)
        {
            MongoDBContext dbContext = new MongoDBContext();

            var filter = Builders<PlayerDto>.Filter.Eq("playerName", playerName);
            var result = await dbContext.Players.FindAsync(filter);

            if (result != null && result.Current != null && result.Current.Count() == 1)
            {
                return result.Current.Single();
            }

            return null;
        }
    }
}
