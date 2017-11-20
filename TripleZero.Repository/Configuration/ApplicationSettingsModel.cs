using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.Configuration
{
    internal class ApplicationSettingsModel
    {        
        public MongoDBSettings MongoDBSettings { get; set; }
        public CachingSettings CachingSettings { get; set; }
    }

    internal class MongoDBSettings
    {
        public string ApiKey { get; set; }
        public string DB { get; set; }
    }

    internal class CachingSettings
    {
        public int RepositoryCachingInMinutes { get; set; }        
    }
}
