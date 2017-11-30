﻿using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TripleZero.Repository.Dto
{
    public class ShipDto
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
}
