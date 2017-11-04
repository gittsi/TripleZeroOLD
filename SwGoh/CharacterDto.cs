using System;
using System.Collections.Generic;
using System.Text;

namespace SwGoh
{

    using System;
    using System.Net;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public partial class CharacterDto
    {
        public string Name { get; set; }
        public string SWGoHUrl { get; set; }
        public int Stars { get; set; }
        public int Gear { get; set; }
        public int Level { get; set; }
        public int Power { get; set; }
        public int StatPower { get; set; }
        public List<Ability> Abilities { get; set; }

        #region General
        public int Health { get; set; }
        public int Protection { get; set; }
        public int Speed { get; set; }
        public int CriticalDamage { get; set; }
        public decimal Potency { get; set; }
        public decimal Tenacity { get; set; }
        public int HealthSteal { get; set; }
        #endregion

        #region Physical Offense
        public int PhysicalDamage { get; set; }
        public decimal PhysicalCriticalChance { get; set; }
        public int ArmorPenetration { get; set; }
        public int PhysicalAccuracy { get; set; }
        #endregion

        #region Physical Survivability
        public decimal Armor { get; set; }
        public decimal DodgeChance { get; set; }
        public decimal PhysicalCriticalAvoidance { get; set; }
        #endregion

        #region Special Offense
        public int SpecialDamage { get; set; }
        public decimal SpecialCriticalChance { get; set; }
        public int ResistancePenetration { get; set; }
        public decimal SpecialAccuracy { get; set; }
        #endregion

        #region Special Survivability
        public decimal Resistance { get; set; }
        public decimal DeflectionChance { get; set; }
        public decimal SpecialCriticalAvoidance { get; set; }
        #endregion

        public List<Mod> Mods { get; set; }
    }

    public partial class Mod
    {
        public string Name { get; set; }
        public long Level { get; set; }
        public long Star { get; set; }
        public ModSlot Type { get; set; }
        public string Rarity { get; set; }
        public ModStat PrimaryStat { get; set; }
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
        public int Level { get; set; }
        public int MaxLevel { get; set; }
    }


    public enum ModValueType
    {
        Flat = 0,
        Percentage = 1
    }

    public enum ModStatType
    {
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
        Transmitter = 1,
        Receiver = 2,
        Processor = 3,
        HoloArray = 4,
        DataBus = 5,
        Multiplexer = 6

    }

    public enum ModSet
    {
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
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }

}
