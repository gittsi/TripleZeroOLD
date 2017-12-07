using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TripleZero.Repository.EnumDto;

namespace TripleZero.Repository.Dto
{
    internal class AbilityConfigDto
    {
        public string Name { get; set; }
        [JsonProperty("Type")]
        [BsonElement("Type")]
        [DefaultValue(1)]
        [BsonDefaultValue(1)]
        public AbilityType AbilityType { get; set; }
    }

    
}
