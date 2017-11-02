using MongoDB.Bson;
using MongoDB.Driver;
using SwGoh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TripleZeroApi.Repository
{
    public class MongoDBContext
    {
        public static string ConnectionString { get; set; }
        public static string DatabaseName { get; set; }
        public static bool IsSSL { get; set; }

        private IMongoDatabase _database { get; }

        public MongoDBContext()
        {
            try
            {
                MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));
                if (IsSSL)
                {
                    settings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
                }
                //var mongoClient = new MongoClient(settings);

                var mongoClient = new MongoClient(ConnectionString);

                _database = mongoClient.GetDatabase(DatabaseName);

                _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait();

                var dsgfd= mongoClient.ListDatabases();
                var asfgasgf = _database.ListCollections();

                var a = _database.GetCollection<TestCollection>("testcollection");
                var aa = _database.GetCollection<TestCollection>("aasfasfa");

                var asgvaf = aa.Find("testcollection");
            }
            catch (Exception ex)
            {
                throw new Exception("Can not access to db server.", ex);
            }
        }

        public IMongoCollection<PlayerDto> Players
        {
            get
            {
                return _database.GetCollection<PlayerDto>("PlayerS");
            }
        }

        public IMongoCollection<TestCollection> TestCollection
        {
            get
            {
                return _database.GetCollection<TestCollection>("testcollection");
            }
        }
    }
}
