using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Core.Settings
{
    public class SettingsTripleZeroRepository
    {
        public MongoDBSettings MongoDBSettings { get; set; }
        public CachingSettings CachingSettings { get; set; }
    }

    public class MongoDBSettings
    {
        public string ApiKey { get; set; }
        public string DB { get; set; }
    }

    public partial class CachingSettings
    {
        public int RepositoryCachingInMinutes { get; set; }
    }
}
