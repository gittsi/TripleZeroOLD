using System;
using System.Collections.Generic;
using System.Text;

namespace SwGoh
{

    using System;
    using System.Net;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System.ComponentModel;
    using static SwGoh.ModEnum;
    using MongoDB.Bson.Serialization.Attributes;

    public partial class CharacterDto
    {
        [BsonElement("Nm")]
        [JsonProperty("Nm")]
        public string Name { get; set; }
        [JsonProperty("Url")]
        [BsonElement("Url")]
        public string SWGoHUrl { get; set; }
        [JsonProperty("S")]
        [BsonElement("S")]
        [DefaultValue (7)]
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
        public List<Ability> Abilities { get; set; }

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
        public int CriticalDamage { get; set; }
        [JsonProperty("Pot")]
        [BsonElement("Pot")]
        public double Potency { get; set; }
        [JsonProperty("Ten")]
        [BsonElement("Ten")]
        public double Tenacity { get; set; }
        [JsonProperty("HlS")]
        [BsonElement("HlS")]
        public int HealthSteal { get; set; }
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
        public int PhysicalAccuracy { get; set; }
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

        public List<Mod> Mods { get; set; }
    }

    public partial class Mod
    {
        public string Name { get; set; }
        [DefaultValue(15)]
        [BsonDefaultValue(15)]
        [BsonElement("Lvl")]
        [JsonProperty("Lvl")]
        public int Level { get; set; }
        [JsonProperty("S")]
        [BsonElement("S")]
        [DefaultValue(5)]
        [BsonDefaultValue(5)]
        public int Star { get; set; }
        public ModSlot Type { get; set; }
        public string Rarity { get; set; }
        [BsonElement("PStat")]
        [JsonProperty("PStat")]
        public ModStat PrimaryStat { get; set; }
        [JsonProperty("SStat")]
        [BsonElement("SStat")]
        public List<ModStat> SecondaryStat { get; set; }
    }

    public partial class ModStat
    {
        public ModValueType ValueType { get; set; }
        public ModStatType StatType { get; set; }
        public double Value { get; set; }
    }

    public partial class Ability
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


    


    public partial class CharacterDto
    {
        public static CharacterDto FromJson(string json) => JsonConvert.DeserializeObject<CharacterDto>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this CharacterDto self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
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
