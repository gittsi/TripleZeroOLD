using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH
{
    public class MongoDBRepo
    {
        public IMongoDatabase Connect()
        {
            try
            {
                string uri = @"mongodb://Dev:dev123qwe@ds" + SWGoH.Settings.appSettings.DatabaseID1.ToString() + ".mlab.com:"+ SWGoH.Settings.appSettings.DatabaseID2.ToString() + "/" + SWGoH.Settings.appSettings.Database;
                var client = new MongoClient(uri);
                
                IMongoDatabase db = client.GetDatabase(SWGoH.Settings.appSettings.Database);
                bool isMongoLive = db.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(2000);
                if (isMongoLive) return db;
                else return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public static string BuildApiUrl(string collection, string query = "", string orderBy = "", string limit = "", string fields = "")
        {
            string url = string.Format("https://api.mlab.com/api/1/databases/{0}/collections/{1}/?apiKey={2}{3}{4}{5}{6}"
                , SWGoH.Settings.appSettings.Database
                , collection
                , SWGoH.Settings.appSettings.MongoApiKey 
                , query
                , orderBy
                , limit
                , fields);
            return url;
        }
        public static string BuildApiUrlFromId(string collection, string id)
        {
            //var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Character/{0}?apiKey={1}", characterConfig.Id, apiKey);
            string url = string.Format("https://api.mlab.com/api/1/databases/{0}/collections/{1}/{2}?apiKey={3}"
                , SWGoH.Settings.appSettings.Database
                , collection
                , id
                , SWGoH.Settings.appSettings.MongoApiKey
                );
            return url;
        }
    }
}
