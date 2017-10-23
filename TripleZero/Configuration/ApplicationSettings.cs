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
                     Token= _SettingsConfigurationRoot.GetSection("Discord_Settings")["Token"]
                     ,
                     Prefix = _SettingsConfigurationRoot.GetSection("Discord_Settings")["Prefix"]

                },
            };

            return appSettings;
        }
    }
}
