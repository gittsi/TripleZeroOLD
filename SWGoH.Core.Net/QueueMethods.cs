using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SwGoh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SwGoh
{
    public class QueueMethods
    {
        public static void AddPlayer(string PlayerName, string command, int priority , SwGoh.Enums.QueueEnum.QueueType type)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    JObject data = new JObject(
                    new JProperty("Name", PlayerName),
                    new JProperty("Date", DateTime.UtcNow),
                    new JProperty("Status", SwGoh.Enums.QueueEnum.QueueStatus.PendingProcess),
                    new JProperty("Priority", priority),
                    new JProperty("Type", type),
                    new JProperty("Command", SwGoh.Enums.Command.UpdatePlayer));

                    var apiKey = SwGoh.Settings.MongoApiKey;

                    var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                    var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue?apiKey={0}", apiKey);
                    HttpResponseMessage response = client.PostAsync(requestUri, httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        SwGoh.Log.ConsoleMessage("Added Player To Queu:" + PlayerName);
                    }
                }
            }
            catch(Exception e)
            {
                SwGoh.Log.ConsoleMessage("Error Adding Player To Queu:" + e.Message);
            }
        }
        public static void RemoveFromQueu(Queue q)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apikey = SwGoh.Settings.MongoApiKey;
                    var deleteurl = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue/{0}?apiKey={1}", q.Id, apikey);
                    WebRequest request = WebRequest.Create(deleteurl);
                    request.Method = "DELETE";

                    HttpWebResponse response1 = (HttpWebResponse)request.GetResponse();
                    if (response1.StatusCode == HttpStatusCode.OK)
                    {
                        SwGoh.Log.ConsoleMessage("Removed From Queu!");
                    }
                    else
                    {
                        SwGoh.Log.ConsoleMessage("Could not remove from Queu!");
                    }
                }
            }
            catch (Exception e)
            {
                SwGoh.Log.ConsoleMessage("Error Deleting From Queu:" + e.Message);
            }
        }
        public static Queue GetQueu()
        {
            SwGoh.Log.ConsoleMessage("Getting from Queu!!");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var queryData = string.Concat("q={\"Status\":0}");
                    var orderby = "s={\"Priority\":-1,\"Date\":1}";
                    var limit = "l=1";
                    string apikey = SwGoh.Settings.MongoApiKey; 
                    string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue/?{0}&{1}&{2}&apiKey={3}", queryData, orderby, limit, apikey);

                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                        Queue result1 = BsonSerializer.Deserialize<Queue>(document.FirstOrDefault());
                        if (result1 != null)
                        {
                            //UPDATE with Status = 1
                            JObject data = new JObject(
                            new JProperty("Name", result1.Name),
                            new JProperty("Date", result1.Date),
                            new JProperty("Status", SwGoh.Enums.QueueEnum.QueueStatus.Processing),
                            new JProperty("Priority", result1.Priority),
                            new JProperty("Type", result1.Type),
                            new JProperty("Command", result1.Command));

                            var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                            var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue/{0}?apiKey={1}", result1.Id, apikey);
                            using (HttpClient client1 = new HttpClient())
                            {
                                HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                            }
                            SwGoh.Log.ConsoleMessage("Got from Queu Player " + result1.Name);
                        }
                        return result1;
                    }
                }
            }
            catch (Exception e)
            {
                SwGoh.Log.ConsoleMessage("Error getting from Queu!!" + e.Message);
                return null;
            }
            return null;
        }
        public static PlayerDto GetLastUpdatedPlayer(string guildname)
        {
            SwGoh.Log.ConsoleMessage("Getting LastUpdated From Queu!!");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var queryData = string.Concat("q={\"GuildName\" : \"" + GuildDto.GetGuildNameFromAlias(guildname) + "\" }");
                    var orderby = "s={\"LastSwGohUpdated\":1}";
                    //var limit = "l=5";

                    var field = "f={\"PlayerName\": 1,\"LastSwGohUpdated\": 1, \"LastClassUpdated\" : 1 }";
                    string apikey = SwGoh.Settings.MongoApiKey;

                    //string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Player/?{0}&{1}&{2}&{3}&apiKey={4}", queryData, field, limit, orderby, apikey);
                    string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Player/?{0}&{1}&{2}&apiKey={3}", queryData, field, orderby, apikey);
                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<PlayerDto> result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PlayerDto>>(response);
                        if (result.Count > 0 )
                        {
                            foreach (PlayerDto item in result)
                            {
                                DateTime lastc = item.LastClassUpdated.Value;
                                if (DateTime.UtcNow.Subtract (lastc).TotalHours < SwGoh.Settings.HoursForNextCheckLastswGohUpdate) continue;
                                bool check = CheckStatusForPlayer(item.PlayerName);
                                if (check) continue;
                                return item;
                            }
                            
                        }
                    }
                }
            }
            catch(Exception e)
            {
                SwGoh.Log.ConsoleMessage("Error getting LastUpdatedPlayerToQueu!! : " + e.Message);
                return null;
            }
            return null;
        }

        private static bool CheckStatusForPlayer(string playerName)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var queryData = string.Concat("q={\"Name\":\"", playerName, "\",  \"Status\" : 1 }");
                    string apikey = SwGoh.Settings.MongoApiKey;
                    string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue/?{0}&apiKey={1}", queryData, apikey);

                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                        Queue result1 = BsonSerializer.Deserialize<Queue>(document.FirstOrDefault());
                        if (result1 != null)
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            catch (Exception e)
            {
                SwGoh.Log.ConsoleMessage("Error getting from Queu!!" + e.Message);
                return false;
            }
            return false;
        }
    }
}
