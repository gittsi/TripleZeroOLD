using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SwGoH;
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
        public static void AddPlayer(string PlayerName, string command, int priority)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    JObject data = new JObject(
                    new JProperty("PlayerName", PlayerName),
                    new JProperty("Date", DateTime.UtcNow),
                    new JProperty("Status", 0),
                    new JProperty("Priority", priority),
                    new JProperty("Command", command));

                    var apiKey = "JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O";

                    var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                    var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue.Player?apiKey={0}", apiKey);
                    HttpResponseMessage response = client.PostAsync(requestUri, httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        SwGoH.Core.Net.Log.ConsoleMessage("Added Player To Queu:" + PlayerName);
                    }
                }
            }
            catch(Exception e)
            {
                SwGoH.Core.Net.Log.ConsoleMessage("Error Adding Player To Queu:" + e.Message);
            }
        }
        public static void RemoveFromQueu(QueuePlayer q)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apikey = "JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O";
                    var deleteurl = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue.Player/{0}?apiKey={1}", q.Id, apikey);
                    WebRequest request = WebRequest.Create(deleteurl);
                    request.Method = "DELETE";

                    HttpWebResponse response1 = (HttpWebResponse)request.GetResponse();
                    if (response1.StatusCode == HttpStatusCode.OK)
                    {
                        SwGoH.Core.Net.Log.ConsoleMessage("Removed From Queu!");
                    }
                    else
                    {
                        SwGoH.Core.Net.Log.ConsoleMessage("Could not remove from Queu!");
                    }
                }
            }
            catch (Exception e)
            {
                SwGoH.Core.Net.Log.ConsoleMessage("Error Deleting From Queu:" + e.Message);
            }
        }
        public static QueuePlayer GetQueu()
        {
            SwGoH.Core.Net.Log.ConsoleMessage("Getting from Queu!!");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var queryData = string.Concat("q={\"Status\":0}");
                    var orderby = "s={\"Priority\":-1,\"Date\":1}";
                    var limit = "l=1";
                    string apikey = "JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O";
                    string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue.Player/?{0}&{1}&{2}&apiKey={3}", queryData, orderby, limit, apikey);

                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                        QueuePlayer result1 = BsonSerializer.Deserialize<QueuePlayer>(document.FirstOrDefault());
                        if (result1 != null)
                        {
                            //UPDATE with Status = 1
                            JObject data = new JObject(
                            new JProperty("PlayerName", result1.PlayerName),
                            new JProperty("Date", result1.Date),
                            new JProperty("Status", 1),
                            new JProperty("Priority", result1.Priority),
                            new JProperty("Command", result1.Command));

                            var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                            var requestUri = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue.Player/{0}?apiKey={1}", result1.Id, apikey);
                            using (HttpClient client1 = new HttpClient())
                            {
                                HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                            }
                            SwGoH.Core.Net.Log.ConsoleMessage("Got from Queu Player " + result1.PlayerName);
                        }
                        return result1;
                    }
                }
            }
            catch (Exception e)
            {
                SwGoH.Core.Net.Log.ConsoleMessage("Error getting from Queu!!" + e.Message);
                return null;
            }
            return null;
        }
        public static PlayerDto GetLastUpdatedPlayer(string guildname)
        {
            SwGoH.Core.Net.Log.ConsoleMessage("Getting LastUpdated From Queu!!");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var queryData = string.Concat("q={\"GuildName\" : \"" + GuildDto.GetGuildNameFromAlias(guildname) + "\" }");
                    var orderby = "s={\"LastSwGohUpdated\":1}";
                    var limit = "l=5";
                    var field = "f={\"PlayerName\": 1,\"LastSwGohUpdated\": 1}";
                    string apikey = "JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O";

                    string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Player/?{0}&{1}&{2}&{3}&apiKey={4}", queryData, field, limit, orderby, apikey);
                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<PlayerDto> result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PlayerDto>>(response);
                        if (result.Count > 0 )
                        {
                            foreach (PlayerDto item in result)
                            {
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
                SwGoH.Core.Net.Log.ConsoleMessage("Error getting LastUpdatedPlayerToQueu!! : " + e.Message);
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
                    var queryData = string.Concat("q={\"PlayerName\":\"", playerName, "\",  \"Status\" : 1 }");
                    string apikey = "JmQkm6eGcaYwn_EqePgpNm57-0LcgA0O";
                    string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Queue.Player/?{0}&apiKey={1}", queryData, apikey);

                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                        QueuePlayer result1 = BsonSerializer.Deserialize<QueuePlayer>(document.FirstOrDefault());
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
                SwGoH.Core.Net.Log.ConsoleMessage("Error getting from Queu!!" + e.Message);
                return false;
            }
            return false;
        }
    }
}
