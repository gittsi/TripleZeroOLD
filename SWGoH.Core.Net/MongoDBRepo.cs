using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
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

        public static void SetWorking(bool working)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string computername = SWGoH.Settings.appSettings.ComputerName;

                    JObject data = new JObject(
                                           new JProperty("ComputerName", computername),
                                           new JProperty("Working", working));

                    string url = SWGoH.MongoDBRepo.BuildApiUrl("Parsers", "&q={\"ComputerName\":\"" + computername + "\"}", "", "&l=1", "");
                    string response = client.GetStringAsync(url).Result;
                    response = response.Replace(" ", "");
                    if (response == "" || response == "[]")
                    {
                        var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                        var requestUri = string.Format(SWGoH.MongoDBRepo.BuildApiUrl("Parsers", "", "", "", ""));
                        HttpResponseMessage updateresult1 = client.PostAsync(requestUri, httpContent).Result;
                        if (updateresult1.IsSuccessStatusCode)
                        {
                            SWGoH.Log.ConsoleMessage("Added Working PC:" + computername);
                        }
                    }
                    else
                    {

                        var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                        var requestUri = string.Format(SWGoH.MongoDBRepo.BuildApiUrl("Parsers", "", "", "", ""));
                        HttpResponseMessage updateresult = client.PutAsync(requestUri, httpContent).Result;
                        if (updateresult.IsSuccessStatusCode)
                        {
                            SWGoH.Log.ConsoleMessage("Added Working PC:" + computername);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SWGoH.Log.ConsoleMessage("Error Adding Working PC:");
            }
        }
    }
}
