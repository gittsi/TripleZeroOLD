using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.Configuration
{
    internal class ApplicationSettings
    {
        private readonly IConfigurationRoot _SettingsConfigurationRoot;
        public ApplicationSettings(ISettingsConfiguration settingsConfiguration)
        {
            _SettingsConfigurationRoot = settingsConfiguration.GetConfiguration();
        }
        public ApplicationSettingsModel Get()
        {
            var boolRepositoryCachingInMinutes = int.TryParse(_SettingsConfigurationRoot.GetSection("Caching_Settings")["RepositoryCachingInMinutes"], out int RepositoryCachingInMinutes);

            ApplicationSettingsModel appSettings = new ApplicationSettingsModel
            {   
                MongoDBSettings = new MongoDBSettings()
                {
                    ApiKey = _SettingsConfigurationRoot.GetSection("MongoDB_Settings")["ApiKey"]
                    ,
                    DB = _SettingsConfigurationRoot.GetSection("MongoDB_Settings")["DB"]
                }
                ,
                CachingSettings = new CachingSettings()
                {

                    RepositoryCachingInMinutes = RepositoryCachingInMinutes             
                }
            };

            return appSettings;
        }
    }
}
