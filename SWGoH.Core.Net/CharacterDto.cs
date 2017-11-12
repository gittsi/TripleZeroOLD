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

    public partial class CharacterDto
    {
        [JsonProperty("nm")]
        public string Name { get; set; }
        [JsonProperty("url")]
        public string SWGoHUrl { get; set; }
        [JsonProperty("*")]
        [DefaultValue (7)]
        public int Stars { get; set; }
        [JsonProperty("gr")]
        public int Gear { get; set; }
        [JsonProperty("lvl")]
        [DefaultValue(85)]
        public int Level { get; set; }
        [JsonProperty("pwr")]
        public int Power { get; set; }
        [JsonProperty("statpwr")]
        public int StatPower { get; set; }
        [JsonProperty("abil")]
        public List<Ability> Abilities { get; set; }

        #region General
        [JsonProperty("Hlth")]
        public int Health { get; set; }
        [JsonProperty("Prot")]
        public int Protection { get; set; }
        [JsonProperty("Sp")]
        public int Speed { get; set; }
        [JsonProperty("CD")]
        public int CriticalDamage { get; set; }
        [JsonProperty("pot")]
        public decimal Potency { get; set; }
        [JsonProperty("ten")]
        public decimal Tenacity { get; set; }
        [JsonProperty("hlthst")]
        public int HealthSteal { get; set; }
        #endregion

        #region Physical Offense
        [JsonProperty("Physdmg")]
        public int PhysicalDamage { get; set; }
        [JsonProperty("physCD")]
        public decimal PhysicalCriticalChance { get; set; }
        [JsonProperty("ArmorPen")]
        public int ArmorPenetration { get; set; }
        [JsonProperty("PhyAccu")]
        [DefaultValue(0)]
        public int PhysicalAccuracy { get; set; }
        #endregion

        #region Physical Survivability
        [JsonProperty("Arm")]
        public decimal Armor { get; set; }
        [DefaultValue(2.0)]
        [JsonProperty("DodgeCh")]
        public decimal DodgeChance { get; set; }
        [JsonProperty("PhyCritAvoid")]
        [DefaultValue(0.0)]
        public decimal PhysicalCriticalAvoidance { get; set; }
        #endregion

        #region Special Offense
        [JsonProperty("specdmg")]
        public int SpecialDamage { get; set; }
        [JsonProperty("specCritChance")]
        public decimal SpecialCriticalChance { get; set; }
        [DefaultValue(0)]
        [JsonProperty("ResistPen")]
        public int ResistancePenetration { get; set; }
        [DefaultValue(0.0)]
        [JsonProperty("specialAccu")]
        public decimal SpecialAccuracy { get; set; }
        #endregion

        #region Special Survivability
        [JsonProperty("Res")]
        public decimal Resistance { get; set; }
        [JsonProperty("DefleChance")]
        [DefaultValue(2.0)]
        public decimal DeflectionChance { get; set; }
        [JsonProperty("SpecialcritAvoid")]
        [DefaultValue(0.0)]
        public decimal SpecialCriticalAvoidance { get; set; }
        #endregion

        public List<Mod> Mods { get; set; }
    }

    public partial class Mod
    {
        public string Name { get; set; }
        [DefaultValue(15)]
        [JsonProperty("lvl")]
        public long Level { get; set; }
        [JsonProperty("*")]
        [DefaultValue(5)]
        public long Star { get; set; }
        public ModSlot Type { get; set; }
        public string Rarity { get; set; }
        [JsonProperty("PrimStat")]
        public ModStat PrimaryStat { get; set; }
        [JsonProperty("SecStat")]
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
        [JsonProperty("lvl")]
        [DefaultValue(8)]
        public int Level { get; set; }
        [JsonProperty("Maxlvl")]
        [DefaultValue(8)]
        public int MaxLevel { get; set; }
    }


    public enum ModValueType
    {
        None=-1,
        Flat = 0,
        Percentage = 1
    }

    public enum ModStatType
    {
        None = 0,
        Speed = 1,
        CriticalChance = 2,
        CriticalDamage = 3,
        Potency = 4,
        Tenacity = 5,
        Accuracy = 6,
        CriticalAvoidance = 7,
        Offense = 8,
        Defense = 9,
        Health = 10,
        Protection = 11
    }

    public enum ModSlot
    {
        [Description("None")]
        None=0,
        [Description("Rectangle")]
        Transmitter = 1,
        [Description("Arrow")]
        Receiver = 2,
        [Description("Diamond")]
        Processor = 3,
        [Description("Triangle")]
        HoloArray = 4,
        [Description("Circle")]
        DataBus = 5,
        [Description("Cross")]
        Multiplexer = 6

    }

    public enum ModSet
    {
        None=0,
        Health = 1,
        Defense = 2,
        CriticalDamage = 3,
        CriticalChance = 4,
        Tenacity = 5,
        Offense = 6,
        Potency = 7,
        Speed = 8
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
