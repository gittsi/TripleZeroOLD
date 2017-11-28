using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using SWGoH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SWGoH.Enums.QueueEnum;
using MongoDB.Driver;

namespace SWGoH
{
    public class QueueMethods
    {
        public static void AddPlayer(string PlayerName,Command cmd ,PriorityEnum priority , SWGoH.Enums.QueueEnum.QueueType type, DateTime nextrundate)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    JObject data = new JObject(
                    new JProperty("Name", PlayerName),
                    new JProperty("InsertedDate", DateTime.UtcNow.ToString("o")),
                    new JProperty("ProcessingStartDate", ""),
                    new JProperty("NextRunDate", nextrundate.ToString ("o")),
                    new JProperty("Status", SWGoH.Enums.QueueEnum.QueueStatus.PendingProcess),
                    new JProperty("Priority", priority),
                    new JProperty("Type", type),
                    new JProperty("Command", cmd),
                    new JProperty("ComputerName", ""));

                    var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                    var requestUri = string.Format(SWGoH.MongoDBRepo.BuildApiUrl("Queue", "", "", "", ""));
                    HttpResponseMessage response = client.PostAsync(requestUri, httpContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        SWGoH.Log.ConsoleMessage("Added Player To Queu:" + PlayerName);
                    }
                }
            }
            catch(Exception e)
            {
                SWGoH.Log.ConsoleMessage("Error Adding Player To Queu:" + e.Message);
            }
        }
        public static void UpdateQueueAndProcessLater(QueueDto q, object whotoupdate , double hours,bool fromnow)
        {
            PlayerDto player = whotoupdate as PlayerDto;
            SWGoH.GuildDto guild = whotoupdate as GuildDto;
            try
            {
                string nextrun = "";
                if (player != null) nextrun = player.LastSwGohUpdated.AddHours(hours).ToString("o");
                else if (guild != null) nextrun = guild.LastSwGohUpdated.AddHours(hours).ToString("o");
                else nextrun = DateTime.UtcNow.AddHours(hours).ToString("o");

                if (fromnow) nextrun = DateTime.UtcNow.AddHours(hours).ToString("o");

                JObject data = new JObject(
                                   new JProperty("Name", q.Name),
                                   new JProperty("InsertedDate", DateTime.UtcNow.ToString("o")),
                                   new JProperty("ProcessingStartDate", ""),
                                   new JProperty("NextRunDate", nextrun),
                                   new JProperty("Status", SWGoH.Enums.QueueEnum.QueueStatus.PendingProcess),
                                   new JProperty("Priority", q.Priority),
                                   new JProperty("Type", q.Type),
                                   new JProperty("Command", q.Command),
                                   new JProperty("ComputerName", ""));

                var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                var requestUri = SWGoH.MongoDBRepo.BuildApiUrlFromId("Queue", q.Id.ToString());
                using (HttpClient client1 = new HttpClient())
                {
                    HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                }
                SWGoH.Log.ConsoleMessage(q.Name + " Added To Queu to be processed later :");
            }
            catch (Exception e)
            {
                SWGoH.Log.ConsoleMessage("Error updating Queu to be processed later :" + e.Message);
            }
        }
        public static void RemoveFromQueu(QueueDto q)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var deleteurl = SWGoH.MongoDBRepo.BuildApiUrlFromId("Queue", q.Id.ToString());
                    WebRequest request = WebRequest.Create(deleteurl);
                    request.Method = "DELETE";

                    HttpWebResponse response1 = (HttpWebResponse)request.GetResponse();
                    if (response1.StatusCode == HttpStatusCode.OK)
                    {
                        SWGoH.Log.ConsoleMessage("Removed From Queu!");
                    }
                    else
                    {
                        SWGoH.Log.ConsoleMessage("Could not remove from Queu!");
                    }
                }
            }
            catch (Exception e)
            {
                SWGoH.Log.ConsoleMessage("Error Deleting From Queu:" + e.Message);
            }
        }
        public static QueueDto GetQueu()
        {
            try
            {
                MongoDBRepo mongo = new MongoDBRepo();
                IMongoDatabase db = mongo.Connect();
                db = null;
                if (db != null)
                {
                    SWGoH.Log.ConsoleMessageNotInFile("Getting from Queu!! (mongo)");
                    IMongoCollection <QueueDto> collection = db.GetCollection<QueueDto>("Queue");
                    if (collection != null)
                    {
                        //FilterDefinition<QueueDto> filter2 = Builders<QueueDto>.Filter.Eq("Priority", 2);
                        //UpdateDefinition<QueueDto> update2 = Builders<QueueDto>.Update.Set("Priority", 1);
                        //UpdateOptions opts2 = new UpdateOptions();
                        //opts2.IsUpsert = false;
                        //UpdateResult res2 = collection.UpdateMany(filter2, update2, opts2);

                        FilterDefinition<QueueDto> filter = Builders<QueueDto>.Filter.Eq("Status", 0);
                        UpdateDefinition<QueueDto> update = Builders<QueueDto>.Update.Set("Status", 1).Set ("ProcessingStartDate" , DateTime.UtcNow.ToString ("o")).Set ("ComputerName" , SWGoH.Settings.appSettings.ComputerName);
                        var opts = new FindOneAndUpdateOptions<QueueDto>()
                        {
                            IsUpsert = false,
                            ReturnDocument = ReturnDocument.After,
                            Sort = Builders<QueueDto>.Sort.Descending(r => r.Priority).Ascending(r => r.NextRunDate)
                        };
                        QueueDto found = collection.FindOneAndUpdate<QueueDto>(filter, update, opts);
                        if (found != null)
                        {
                            DateTime nextrun = DateTime.Parse(found.NextRunDate).ToUniversalTime();
                            if (DateTime.UtcNow < nextrun)
                            {
                                found.Status = QueueStatus.PendingProcess;

                                FilterDefinition<QueueDto> filter1 = Builders<QueueDto>.Filter.Eq("_id", found.Id);
                                UpdateDefinition<QueueDto> update1 = Builders<QueueDto>.Update.Set("Status", 0).Set("ComputerName", "");
                                UpdateOptions opts1 = new UpdateOptions();
                                opts1.IsUpsert = false;

                                UpdateResult res = collection.UpdateOne(filter1, update1, opts1);
                                return null;
                            }
                        }
                        return found;
                    }
                }
                else
                {
                    SWGoH.Log.ConsoleMessageNotInFile("Getting from Queu!!");
                    using (HttpClient client = new HttpClient())
                    {
                        string url = SWGoH.MongoDBRepo.BuildApiUrl("Queue", "&q={\"Status\":0}", "&s={\"Priority\":-1,\"NextRunDate\":1}", "&l=1", "");

                        string response = client.GetStringAsync(url).Result;
                        if (response != "" && response != "[  ]")
                        {
                            List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                            QueueDto result1 = BsonSerializer.Deserialize<QueueDto>(document.FirstOrDefault());
                            if (result1 != null)
                            {
                                //check nextrundate
                                DateTime nextrun = DateTime.Parse(result1.NextRunDate).ToUniversalTime();
                                if (DateTime.UtcNow > nextrun)
                                {
                                    //UPDATE with Status = 1
                                    JObject data = new JObject(
                                    new JProperty("Name", result1.Name),
                                    new JProperty("InsertedDate", result1.InsertedDate),
                                    new JProperty("ProcessingStartDate", DateTime.UtcNow.ToString("o")),
                                    new JProperty("NextRunDate", result1.NextRunDate),
                                    new JProperty("Status", SWGoH.Enums.QueueEnum.QueueStatus.Processing),
                                    new JProperty("Priority", result1.Priority),
                                    new JProperty("Type", result1.Type),
                                    new JProperty("Command", result1.Command),
                                    new JProperty("ComputerName", SWGoH.Settings.appSettings.ComputerName));

                                    var httpContent = new StringContent(data.ToString(), Encoding.UTF8, "application/json");
                                    var requestUri = SWGoH.MongoDBRepo.BuildApiUrlFromId("Queue", result1.Id.ToString());
                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        HttpResponseMessage updateresult = client1.PutAsync(requestUri, httpContent).Result;
                                    }
                                    SWGoH.Log.ConsoleMessage("Got from Queu Player " + result1.Name);
                                    return result1;
                                }
                                else return null;
                            }
                            return result1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SWGoH.Log.ConsoleMessage("Error getting from Queu!!" + e.Message);
                return null;
            }
            return null;
        }
        public static PlayerDto GetLastUpdatedPlayer(string guildname)
        {
            SWGoH.Log.ConsoleMessageNotInFile("Getting LastUpdated From Queu!!");
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var queryData = string.Concat("&q={\"GuildName\" : \"" + GuildDto.GetGuildNameFromAlias(guildname) + "\" }");
                    var field = "f={\"PlayerName\": 1,\"LastSwGohUpdated\": 1, \"LastClassUpdated\" : 1 }";
                    string url = SWGoH.MongoDBRepo.BuildApiUrl("Player", queryData, "&s={\"LastSwGohUpdated\":1}", "", field);

                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<PlayerDto> result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PlayerDto>>(response);
                        if (result.Count > 0 )
                        {
                            foreach (PlayerDto item in result)
                            {
                                DateTime lastc = item.LastClassUpdated.Value;
                                if (DateTime.UtcNow.Subtract (lastc).TotalHours < Settings.appSettings.HoursForNextCheckLastswGohUpdate) continue;
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
                SWGoH.Log.ConsoleMessage("Error getting LastUpdatedPlayerToQueu!! : " + e.Message);
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
                    var queryData = string.Concat("&q={\"Name\":\"", playerName, "\",  \"Status\" : 1 }");
                    string url = SWGoH.MongoDBRepo.BuildApiUrl("Queue", queryData, "", "", "");

                    string response = client.GetStringAsync(url).Result;
                    if (response != "" && response != "[  ]")
                    {
                        List<BsonDocument> document = BsonSerializer.Deserialize<List<BsonDocument>>(response);
                        QueueDto result1 = BsonSerializer.Deserialize<QueueDto>(document.FirstOrDefault());
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
                SWGoH.Log.ConsoleMessage("Error getting from Queu!!" + e.Message);
                return false;
            }
            return false;
        }
    }
}
