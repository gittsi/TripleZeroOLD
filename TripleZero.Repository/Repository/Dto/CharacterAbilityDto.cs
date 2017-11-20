using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TripleZero.Repository.Dto
{
    internal class CharacterAbilityDto
    {
        public string Name { get; set; }
        [BsonElement("Lvl")]
        [JsonProperty("Lvl")]
        [DefaultValue(8)]
        [BsonDefaultValue(8)]
        public int Level { get; set; }
        [JsonProperty("MLvl")]
        [BsonElement("MLvl")]
        [DefaultValue(8)]
        [BsonDefaultValue(8)]
        public int MaxLevel { get; set; }
    }
}
