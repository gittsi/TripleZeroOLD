using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.Helper
{
    internal class JSonConverterSettings
    {

        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
        };
    }

}
