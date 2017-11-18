using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Configuration
{
    public class ApplicationSettings
    {
        private readonly IConfigurationRoot _SettingsConfigurationRoot;
        public ApplicationSettings(ISettingsConfiguration settingsConfiguration)
        {
            _SettingsConfigurationRoot = settingsConfiguration.GetConfiguration();
        }
        public ApplicationSettingsModel Get()
        {
            var boolRepositoryCachingInMinutes = int.TryParse(_SettingsConfigurationRoot.GetSection("Caching_Settings")["RepositoryCachingInMinutes"], out int RepositoryCachingInMinutes);
            var boolModuleCachingInMinutes = int.TryParse(_SettingsConfigurationRoot.GetSection("Caching_Settings")["ModuleCachingInMinutes"], out int ModuleCachingInMinutes);


            ApplicationSettingsModel appSettings = new ApplicationSettingsModel
            {

                //general settings
                GeneralSettings = new GeneralSettings()
                {
                    ApplicationName = _SettingsConfigurationRoot.GetSection("General_Settings")["ApplicationName"]
                    ,
                    Environment = _SettingsConfigurationRoot.GetSection("General_Settings")["Environment"]
                    ,
                    JsonSettingsVersion = _SettingsConfigurationRoot.GetSection("General_Settings")["JsonSettingsVersion"]
                },

                //discord settings
                DiscordSettings = new DiscordSettings()
                {
                    Token = _SettingsConfigurationRoot.GetSection("Discord_Settings")["Token"]
                     ,
                    Prefix = _SettingsConfigurationRoot.GetSection("Discord_Settings")["Prefix"]
                     ,
                    BotAdminRole = _SettingsConfigurationRoot.GetSection("Discord_Settings")["BotAdminRole"]

                },
                MongoDBSettings = new MongoDBSettings()
                {
                    ApiKey = _SettingsConfigurationRoot.GetSection("MongoDB_Settings")["ApiKey"]
                    ,
                    DB = _SettingsConfigurationRoot.GetSection("MongoDB_Settings")["DB"]
                }
                ,
                CachingSettings = new CachingSettings()
                {

                    RepositoryCachingInMinutesApiKey = RepositoryCachingInMinutes
                    ,
                    ModuleCachingInMinutes = ModuleCachingInMinutes
                }
            };

            return appSettings;
        }
    }
}
