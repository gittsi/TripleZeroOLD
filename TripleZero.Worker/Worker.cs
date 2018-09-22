using System;
using swgoh_help_api;
using WebApplication1.SWGOH_Help_Api_C_Sharp_master.connectionHelper;
using Newtonsoft.Json;

namespace TripleZero.Worker
{
    class Worker
    {
        static void Main(string[] args)
        {
            var userSettings = new UserSettings();
            userSettings.password = "00000000";
            userSettings.username = "TSiTaS";
            var helper = new swgohHelpApiHelper(userSettings);

            helper.login();

            //JObject json = new JObject();
            //try
            //{
            //    json = JObject.Parse(player);
            //}
            //catch (Exception ex)
            //{
            //    throw new ApplicationException(ex.Message);
            //}


            var playerJson = helper.fetchPlayer(new int[] { 462747278 });
            Player[] saaa = JsonConvert.DeserializeObject<Player[]>(playerJson);

           
            var guild = helper.fetchGuild(new int[] { 462747278 });

            var units = helper.fetchUnits(new int[] { 462747278 });
            var zetas = helper.fetchZetas();
            var squads = helper.fetchSquads();
            var events1 = helper.fetchEvents();

            //var events = Events.FromJson(jsonString);

            var battles = helper.fetchBattles();
            var data = helper.fetchData(DataEndpointConstants.abilityList);
            var test = data;
        }
    }
}
