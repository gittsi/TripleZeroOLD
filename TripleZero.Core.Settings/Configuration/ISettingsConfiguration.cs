using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Core.Settings
{
    public interface ISettingsConfiguration
    {
        IConfigurationRoot GetConfiguration();
    }
}
