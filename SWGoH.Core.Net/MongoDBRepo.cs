﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH
{
    public class MongoDBRepo
    {
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