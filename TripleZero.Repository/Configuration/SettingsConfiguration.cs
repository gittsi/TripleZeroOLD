using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TripleZero.Repository.Configuration
{
    internal class SettingsConfiguration : ISettingsConfiguration
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
            .AddJsonFile("configRepository.json", optional: false, reloadOnChange: true);
            //.AddJsonFile("characters.json", optional: false, reloadOnChange: true)
            //.AddJsonFile("guilds.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();

            return Configuration;
        }
    }
}
