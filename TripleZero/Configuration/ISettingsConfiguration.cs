using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Configuration
{
    public interface ISettingsConfiguration
    {
        IConfigurationRoot GetConfiguration();

        //IConfigurationRoot GetGuildsConfiguration();

        //IConfigurationRoot GetCharactersConfiguration();
    }
}
