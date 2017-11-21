using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.Configuration
{
    internal interface ISettingsConfiguration
    {
        IConfigurationRoot GetConfiguration();
    }
}
