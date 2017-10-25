using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Configuration
{
    public class GuildSettings
    {
        private readonly IConfigurationRoot _SettingsConfigurationRoot;
        public GuildSettings(ISettingsConfiguration settingsConfiguration)
        {
            _SettingsConfigurationRoot = settingsConfiguration.GetGuildsConfiguration();
        }

        public GuildSettingsModel Get()
        {
            GuildSettingsModel guildSettingsModel = new GuildSettingsModel();

            List<SWGohGuildSettings> SWGoHGuildsSettings = new List<SWGohGuildSettings>();
            foreach(var a in _SettingsConfigurationRoot.GetChildren())
            {
                SWGohGuildSettings guildSettings = new SWGohGuildSettings();
                guildSettings.SWGoHId = a.GetSection("Guilds")["SWGoHId"];

                SWGoHGuildsSettings.Add(guildSettings);

                guildSettingsModel.GuildsSettings = SWGoHGuildsSettings;
            }            
            

            return guildSettingsModel;
        }
    }
}
