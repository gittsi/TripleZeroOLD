using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using TripleZero.Repository.Repository.Dto;

namespace TripleZero.Repository.Dto
{
    internal class CharacterDto : UnitDto
    {
        [JsonProperty("Ge")]
        [BsonElement("Ge")]
        public int Gear { get; set; }

        [JsonProperty("Ab")]
        [BsonElement("Ab")]
        public List<CharacterAbilityDto> Abilities { get; set; }

        public List<string> CharacterTags { get; set; }

        public List<ModDto> Mods { get; set; }
    }
}
