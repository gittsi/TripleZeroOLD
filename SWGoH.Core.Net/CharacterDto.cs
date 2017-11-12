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
    using static SwGoH.ModEnum;

    public partial class CharacterDto
    {
        [JsonProperty("Nm")]
        public string Name { get; set; }
        [JsonProperty("Url")]
        public string SWGoHUrl { get; set; }
        [JsonProperty("S")]
        [DefaultValue (7)]
        public int Stars { get; set; }
        [JsonProperty("Ge")]
        public int Gear { get; set; }
        [JsonProperty("Lvl")]
        [DefaultValue(85)]
        public int Level { get; set; }
        [JsonProperty("Pwr")]
        public int Power { get; set; }
        [JsonProperty("SP")]
        public int StatPower { get; set; }
        [JsonProperty("Ab")]
        public List<Ability> Abilities { get; set; }

        #region General
        [JsonProperty("Hl")]
        public int Health { get; set; }
        [JsonProperty("Pr")]
        public int Protection { get; set; }
        [JsonProperty("Sp")]
        public int Speed { get; set; }
        [JsonProperty("CD")]
        public int CriticalDamage { get; set; }
        [JsonProperty("Pot")]
        public decimal Potency { get; set; }
        [JsonProperty("Ten")]
        public decimal Tenacity { get; set; }
        [JsonProperty("HlS")]
        public int HealthSteal { get; set; }
        #endregion

        #region Physical Offense
        [JsonProperty("PD")]
        public int PhysicalDamage { get; set; }
        [JsonProperty("PCC")]
        public decimal PhysicalCriticalChance { get; set; }
        [JsonProperty("AP")]
        public int ArmorPenetration { get; set; }
        [JsonProperty("PA")]
        [DefaultValue(0)]
        public int PhysicalAccuracy { get; set; }
        #endregion

        #region Physical Survivability
        [JsonProperty("Arm")]
        public decimal Armor { get; set; }
        [DefaultValue(2.0)]
        [JsonProperty("DC")]
        public decimal DodgeChance { get; set; }
        [JsonProperty("PCA")]
        [DefaultValue(0.0)]
        public decimal PhysicalCriticalAvoidance { get; set; }
        #endregion

        #region Special Offense
        [JsonProperty("SD")]
        public int SpecialDamage { get; set; }
        [JsonProperty("SCC")]
        public decimal SpecialCriticalChance { get; set; }
        [DefaultValue(0)]
        [JsonProperty("RP")]
        public int ResistancePenetration { get; set; }
        [DefaultValue(0.0)]
        [JsonProperty("SA")]
        public decimal SpecialAccuracy { get; set; }
        #endregion

        #region Special Survivability
        [JsonProperty("Res")]
        public decimal Resistance { get; set; }
        [JsonProperty("DS")]
        [DefaultValue(2.0)]
        public decimal DeflectionChance { get; set; }
        [JsonProperty("SCA")]
        [DefaultValue(0.0)]
        public decimal SpecialCriticalAvoidance { get; set; }
        #endregion

        public List<Mod> Mods { get; set; }
    }

    public partial class Mod
    {
        public string Name { get; set; }
        [DefaultValue(15)]
        [JsonProperty("Lvl")]
        public long Level { get; set; }
        [JsonProperty("S")]
        [DefaultValue(5)]
        public long Star { get; set; }
        public ModSlot Type { get; set; }
        public string Rarity { get; set; }
        [JsonProperty("PStat")]
        public ModStat PrimaryStat { get; set; }
        [JsonProperty("SStat")]
        public List<ModStat> SecondaryStat { get; set; }
    }

    public partial class ModStat
    {
        public ModValueType ValueType { get; set; }
        public ModStatType StatType { get; set; }
        public decimal Value { get; set; }
    }

    public partial class Ability
    {
        public string Name { get; set; }
        [JsonProperty("Lvl")]
        [DefaultValue(8)]
        public int Level { get; set; }
        [JsonProperty("MLvl")]
        [DefaultValue(8)]
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
