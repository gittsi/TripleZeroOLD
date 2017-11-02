using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TripleZeroApi.Configuration
{
    public class SettingsConfiguration : ISettingsConfiguration
    {
        public static IConfigurationRoot Configuration { get; set; }
        public SettingsConfiguration()
        {
            Configuration = GetConfiguration();
        }

        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
            //.AddJsonFile("characters.json", optional: false, reloadOnChange: true)
            //.AddJsonFile("guilds.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            return Configuration;
        }
     
    }

}
