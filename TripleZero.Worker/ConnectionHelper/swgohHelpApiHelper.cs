using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using WebApplication1.SWGOH_Help_Api_C_Sharp_master.connectionHelper;

namespace swgoh_help_api
{
    public class swgohHelpApiHelper
    {
        string user = "";
        string token = "";
        string url = "";
        string signin = "";
        string data = "";
        string player = "";
        string units = "";
        string zetas = "";
        string squads = "";
        string events = "";
        string battles = "";
        string guild = "";

        //JavaScriptSerializer javaSerializer;

        Dictionary<string, string> userAsDictonary;

        private HttpClient client = new HttpClient();

        public swgohHelpApiHelper(UserSettings settings)
        {

            user = "username=" + settings.username;
            user += "&password=" + settings.password;
            user += "&grant_type=password";
            user += "&client_id=" + settings.client_id;
            user += "&client_secret=" + settings.client_secret;

            //javaSerializer = new JavaScriptSerializer();
            //javaSerializer.RegisterConverters(new JavaScriptConverter[] { new ExpandoJSONConverter() });

            //user = "{ \"username\" : " + "\"" + settings.username + "\"" + ",";
            //user += "\"password\" : " + "\"" + settings.password + "\"" + ",";
            //user += "\"grant_type\" : " + "\"" + "\"password\"" + "\"" + ",";
            //user += "\"client_id\" : " + "\"" + settings.client_id + "\"" + ",";
            //user += "\"client_secret\" : " + "\"" + settings.client_secret + "\"" + " }";

            this.token = null;

            string protocol = string.IsNullOrEmpty(settings.protocol) ? "https" : settings.protocol;
            string host = string.IsNullOrEmpty(settings.host) ? "api.swgoh.help" : settings.host;
            string port = string.IsNullOrEmpty(settings.port) ? "" : settings.port;

            this.url = protocol + "://" + host + port;
            this.signin = this.url + "/auth/signin/";
            this.data = this.url + "/swgoh/data/";
            this.player = this.url + "/swgoh/player/";
            this.guild = this.url + "/swgoh/guild/";
            this.units = this.url + "/swgoh/units/";
            this.zetas = this.url + "/swgoh/zetas/";
            this.squads = this.url + "/swgoh/squads/";
            this.events = this.url + "/swgoh/events/";
            this.battles = this.url + "/swgoh/battles/";

        }

        public void login()
        {

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.signin);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                byte[] byteArray = Encoding.UTF8.GetBytes(user);
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var loginresponse = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                var loginResponseObject = JsonConvert.DeserializeObject<LoginResponse>(loginresponse);
                token = loginResponseObject.access_token;

            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public string fetchApi(string url, object param)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + token + "");

                var json = JsonConvert.SerializeObject(param);

                byte[] byteArray = Encoding.UTF8.GetBytes(json);

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    writer.Write(json);
                }

                //Stream dataStream = request.GetRequestStream();
                //dataStream.Write(byteArray, 0, byteArray.Length);
                //dataStream.Close();
                WebResponse response = request.GetResponse();
                Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                var dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                var apiResponse = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return apiResponse;

            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public string fetchPlayer(int[] allycodes, string language = null, bool? enums = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            obj.allycodes = allycodes;
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (project != null)
                obj.project = project;

            var response = this.fetchApi(this.player, obj);

            return response;

        }

        public string fetchZetas(object project = null)
        {
            dynamic obj = new ExpandoObject();
            if (project != null)
                obj.project = project;

            var response = this.fetchApi(this.zetas, obj);

            return response;

        }

        public string fetchSquads(object project = null)
        {
            dynamic obj = new ExpandoObject();
            if (project != null)
                obj.project = project;

            var response = this.fetchApi(this.squads, obj);

            return response;

        }

        public string fetchEvents(string language = null, bool? enums = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (project != null)
                obj.project = project;

            var response = this.fetchApi(this.events, obj);

            return response;

        }

        public string fetchBattles(string language = null, bool? enums = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (project != null)
                obj.project = project;

            var response = this.fetchApi(this.battles, obj);

            return response;

        }

        public string fetchData(string collection, string language = null, bool? enums = null, object match = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            obj.collection = collection;
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (match != null)
                obj.match = match;
            if (project != null)
                obj.project = project;

            var response = this.fetchApi(this.battles, obj);

            return response;

        }

        public string fetchUnits(int[] allycodes, string language = null, bool? enums = null, bool? mods = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            obj.allycodes = allycodes;
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (mods.HasValue)
                obj.mods = mods;
            if (project != null)
                obj.project = project;

            var response = this.fetchApi(this.units, obj);

            return response;

        }

        public string fetchGuild(int[] allycodes, string language = null, bool? enums = null, bool? roster = null, bool? units = null, bool? mods = null, object project = null)
        {
            dynamic obj = new ExpandoObject();
            obj.allycodes = allycodes;
            if (language != null)
                obj.language = language;
            if (enums.HasValue)
                obj.enums = enums;
            if (roster.HasValue)
                obj.roster = roster;
            if (units.HasValue)
                obj.units = units;
            if (mods.HasValue)
                obj.mods = mods;
            if (project != null)
                obj.project = project;

            var response = this.fetchApi(this.guild, obj);

            return response;
        }

    }

}