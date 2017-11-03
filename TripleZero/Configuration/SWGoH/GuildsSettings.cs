using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SwGoh;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TripleZero.Helper;
using TripleZero.Infrastructure.DI;

namespace TripleZero.Configuration
{
    public class GuildsConfig
    {
        public async Task<List<GuildConfig>> GetGuildsConfig(string alias)
        {
            
            var apiKey = IResolver.Current.MongoDBSettings.ApiKey;

            string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Guild/?apiKey={0}",  apiKey);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    List<GuildConfig> ret = JsonConvert.DeserializeObject<List<GuildConfig>>(response, Converter.Settings);

                    return ret;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public async Task<GuildConfig> GetGuildConfig(string alias)
        {
            

            var queryData = string.Concat("q={\"Aliases\":\"", alias, "\"}");
            //var orderby = "s={\"LastUpdated\":1}";
            //var limit = "l=1";
            var apiKey = IResolver.Current.ApplicationSettings.Get().MongoDBSettings.ApiKey;

            string url = string.Format("https://api.mlab.com/api/1/databases/triplezero/collections/Config.Guild/?{0}&apiKey={1}", queryData, apiKey);

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    GuildConfig ret = JsonConvert.DeserializeObject<List<GuildConfig>>(response, Converter.Settings).FirstOrDefault();

                    return ret;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }


            //using (StreamReader r = new StreamReader("Configuration/guilds.json"))
            //{
            //    string json = r.ReadToEnd();

            //    try
            //    {
            //        GuildsConfig results = JsonConvert.DeserializeObject<GuildsConfig>(json);
            //        //var matched = results.Guilds.Where(p => p.Aliases.Contains(alias)).FirstOrDefault();

            //        var matched = results.Guilds.Where(p => p.Aliases.IndexOf(alias) !=1).FirstOrDefault();

            //        //var matched2 = results.Guilds.Select(p=>p.Aliases).Where(t => t.IndexOf(alias) != -1);

            //        return matched;
            //    }
            //    catch (Exception ex)
            //    {
            //        Consoler.WriteLineInColor(string.Format("GuildSettings Get : {0}", ex.Message), ConsoleColor.Red);
            //        return null;
            //    }
            //}            
        }
    }
}
