using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using SWGoH.Enums.ModEnum;

namespace SWGoH
{
    public partial class ShipDto
    {
        [BsonElement("Nm")]
        [JsonProperty("Nm")]
        public string Name { get; set; }
        [JsonProperty("Url")]
        [BsonElement("Url")]
        public string SWGoHUrl { get; set; }
        [JsonProperty("S")]
        [BsonElement("S")]
        [DefaultValue(7)]
        [BsonDefaultValue(7)]
        public int Stars { get; set; }
        [JsonProperty("Lvl")]
        [BsonElement("Lvl")]
        [DefaultValue(85)]
        [BsonDefaultValue(85)]
        public int Level { get; set; }
        [JsonProperty("Pwr")]
        [BsonElement("Pwr")]
        public int Power { get; set; }
    }

    public partial class ShipDto
    {
        public static ShipDto FromJson(string json) => JsonConvert.DeserializeObject<ShipDto>(json, Converter.Settings);
    }
}
