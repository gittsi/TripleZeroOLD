// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using swgoh_help_api;
//
//    var events = Events.FromJson(jsonString);

namespace swgoh_help_api
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class SwgohEvents
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("desc")]
        public string Desc { get; set; }

        [JsonProperty("type")]
        public long Type { get; set; }

        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("schedule")]
        public List<Schedule> Schedule { get; set; }
    }

    public partial class Schedule
    {
        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("end")]
        public long End { get; set; }

        [JsonProperty("show")]
        public long Show { get; set; }

        [JsonProperty("hide")]
        public long Hide { get; set; }
    }

    public partial class SwgohEvents
    {
        public static List<SwgohEvents> FromJson(string json) => JsonConvert.DeserializeObject<List<SwgohEvents>>(json, swgoh_help_api.ConverterEvents.Settings);
    }

    public static class SerializeEvents
    {
        public static string ToJson(this List<SwgohEvents> self) => JsonConvert.SerializeObject(self, swgoh_help_api.ConverterEvents.Settings);
    }

    internal static class ConverterEvents
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
