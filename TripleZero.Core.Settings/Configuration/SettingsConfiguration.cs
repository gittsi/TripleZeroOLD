using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TripleZero.Core.Settings
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
            .SetBasePath(string.Concat(Directory.GetCurrentDirectory(), "/Configuration"))
            .AddJsonFile("configBot.json", optional: false, reloadOnChange: true)//bot settings
            .AddJsonFile("configRepository.json", optional: false, reloadOnChange: true);//repository settings

            Configuration = builder.Build();

            return Configuration;
        }
    }
}
