using System.Collections.Generic;
using Newtonsoft.Json;
using System.ComponentModel;
using MongoDB.Bson.Serialization.Attributes;
using TripleZero.Repository.Repository.Dto;

namespace TripleZero.Repository.Dto
{
    internal class CharacterDto
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
        [JsonProperty("Ge")]
        [BsonElement("Ge")]
        public int Gear { get; set; }
        [JsonProperty("Lvl")]
        [BsonElement("Lvl")]
        [DefaultValue(85)]
        [BsonDefaultValue(85)]
        public int Level { get; set; }
        [JsonProperty("Pwr")]
        [BsonElement("Pwr")]
        public int Power { get; set; }
        [JsonProperty("SP")]
        [BsonElement("SP")]
        public int StatPower { get; set; }
        [JsonProperty("Ab")]
        [BsonElement("Ab")]
        public List<CharacterAbilityDto> Abilities { get; set; }
        public List<string> CharacterTags { get; set; }

        #region General
        [JsonProperty("Hl")]
        [BsonElement("Hl")]
        public int Health { get; set; }
        [JsonProperty("Pr")]
        [BsonElement("Pr")]
        public int Protection { get; set; }
        [JsonProperty("Sp")]
        [BsonElement("Sp")]
        public int Speed { get; set; }
        [JsonProperty("CD")]
        [BsonElement("CD")]
        [DefaultValue(150.0)]
        [BsonDefaultValue(150.0)]
        public double CriticalDamage { get; set; }
        [JsonProperty("Pot")]
        [BsonElement("Pot")]
        public double Potency { get; set; }
        [JsonProperty("Ten")]
        [BsonElement("Ten")]
        public double Tenacity { get; set; }
        [JsonProperty("HlS")]
        [BsonElement("HlS")]
        public double HealthSteal { get; set; }
        #endregion

        #region Physical Offense
        [JsonProperty("PD")]
        [BsonElement("PD")]
        public int PhysicalDamage { get; set; }
        [JsonProperty("PCC")]
        [BsonElement("PCC")]
        public double PhysicalCriticalChance { get; set; }
        [JsonProperty("AP")]
        [BsonElement("AP")]
        public int ArmorPenetration { get; set; }
        [JsonProperty("PA")]
        [BsonElement("PA")]
        [DefaultValue(0)]
        [BsonDefaultValue(0)]
        public double PhysicalAccuracy { get; set; }
        #endregion

        #region Physical Survivability
        [JsonProperty("Arm")]
        [BsonElement("Arm")]
        public double Armor { get; set; }
        [DefaultValue(2.0)]
        [BsonDefaultValue(2.0)]
        [JsonProperty("DC")]
        [BsonElement("DC")]
        public double DodgeChance { get; set; }
        [JsonProperty("PCA")]
        [BsonElement("PCA")]
        [DefaultValue(0.0)]
        [BsonDefaultValue(0.0)]
        public double PhysicalCriticalAvoidance { get; set; }
        #endregion

        #region Special Offense
        [JsonProperty("SD")]
        [BsonElement("SD")]
        public int SpecialDamage { get; set; }
        [JsonProperty("SCC")]
        [BsonElement("SCC")]
        public double SpecialCriticalChance { get; set; }
        [DefaultValue(0)]
        [BsonDefaultValue(0)]
        [JsonProperty("RP")]
        [BsonElement("RP")]
        public int ResistancePenetration { get; set; }
        [DefaultValue(0.0)]
        [BsonDefaultValue(0.0)]
        [JsonProperty("SA")]
        [BsonElement("SA")]
        public double SpecialAccuracy { get; set; }
        #endregion

        #region Special Survivability
        [JsonProperty("Res")]
        [BsonElement("Res")]
        public double Resistance { get; set; }
        [JsonProperty("DS")]
        [BsonElement("DS")]
        [DefaultValue(2.0)]
        [BsonDefaultValue(2.0)]
        public double DeflectionChance { get; set; }
        [JsonProperty("SCA")]
        [BsonElement("SCA")]
        [DefaultValue(0.0)]
        [BsonDefaultValue(0.0)]
        public double SpecialCriticalAvoidance { get; set; }
        #endregion

        public List<ModDto> Mods { get; set; }
    }
}
