// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using swgoh_help_api;
//
//    var player = Player.FromJson(jsonString);

namespace swgoh_help_api
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Player
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("allyCode")]
        public long AllyCode { get; set; }

        [JsonProperty("guildName")]
        public string GuildName { get; set; }

        [JsonProperty("gpFull")]
        public long GpFull { get; set; }

        [JsonProperty("gpChar")]
        public long GpChar { get; set; }

        [JsonProperty("gpShip")]
        public long GpShip { get; set; }

        [JsonProperty("roster")]
        public List<Roster> Roster { get; set; }

        [JsonProperty("arena")]
        public Arena Arena { get; set; }

        [JsonProperty("updated")]
        public long Updated { get; set; }
    }

    public partial class Arena
    {
        [JsonProperty("char")]
        public Char Char { get; set; }

        [JsonProperty("ship")]
        public Char Ship { get; set; }
    }

    public partial class Char
    {
        [JsonProperty("rank")]
        public long Rank { get; set; }

        [JsonProperty("squad")]
        public List<Squad> Squad { get; set; }
    }

    public partial class Squad
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class Roster
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("defId")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public RosterType Type { get; set; }

        [JsonProperty("rarity")]
        public long Rarity { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("gp")]
        public double Gp { get; set; }

        [JsonProperty("xp")]
        public long Xp { get; set; }

        [JsonProperty("gear")]
        public long Gear { get; set; }

        [JsonProperty("equipped")]
        public List<Equipped> Equipped { get; set; }

        [JsonProperty("skills")]
        public List<Skill> Skills { get; set; }

        [JsonProperty("crew")]
        public List<Crew> Crew { get; set; }

        [JsonProperty("mods")]
        public List<Mod> Mods { get; set; }
    }

    public partial class Crew
    {
        [JsonProperty("unitId")]
        public string UnitId { get; set; }

        [JsonProperty("slot")]
        public long Slot { get; set; }

        //[JsonProperty("skillReferenceList")]
        //public List<SkillReferenceList> SkillReferenceList { get; set; }

        [JsonProperty("cp")]
        public long Cp { get; set; }

        [JsonProperty("gp")]
        public double Gp { get; set; }
    }
    //Error converting value "specialskill_TIEFIGHTERFOSF02" to type 'swgoh_help_api.SkillReferenceList'. Path '[0].roster[6].crew[0].skillReferenceList[0]', line 1, position 13852.
    public partial class SkillReferenceList
    {
        //[JsonProperty("skillId")]
        //public string SkillId { get; set; }

        //[JsonProperty("requiredTier")]
        //public long RequiredTier { get; set; }

        //[JsonProperty("requiredRarity")]
        //public long RequiredRarity { get; set; }

        [JsonProperty("id")]
        public string SkillId { get; set; }

        [JsonProperty("tier")]
        public long RequiredTier { get; set; }

        [JsonProperty("isZeta")]
        public bool IsZeta { get; set; }
    }

    public partial class Equipped
    {
        [JsonProperty("equipmentId")]
        public string EquipmentId { get; set; }

        [JsonProperty("slot")]
        public long Slot { get; set; }
    }

    public partial class Mod
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("slot")]
        public Slot Slot { get; set; }

        [JsonProperty("setId")]
        public long SetId { get; set; }

        [JsonProperty("set")]
        public Set Set { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }

        [JsonProperty("pips")]
        public long Pips { get; set; }

        [JsonProperty("primaryBonusType")]
        public PrimaryBonusType PrimaryBonusType { get; set; }

        [JsonProperty("primaryBonusValue")]
        public string PrimaryBonusValue { get; set; }

        [JsonProperty("secondaryType_1")]
        public SecondaryType SecondaryType1 { get; set; }

        [JsonProperty("secondaryValue_1")]
        public string SecondaryValue1 { get; set; }

        [JsonProperty("secondaryType_2")]
        public SecondaryType SecondaryType2 { get; set; }

        [JsonProperty("secondaryValue_2")]
        public string SecondaryValue2 { get; set; }

        [JsonProperty("secondaryType_3")]
        public SecondaryType SecondaryType3 { get; set; }

        [JsonProperty("secondaryValue_3")]
        public string SecondaryValue3 { get; set; }

        [JsonProperty("secondaryType_4")]
        public SecondaryType SecondaryType4 { get; set; }

        [JsonProperty("secondaryValue_4")]
        public string SecondaryValue4 { get; set; }
    }

    public partial class Skill
    {
        [JsonProperty("tier")]
        public long Tier { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("isZeta")]
        public bool IsZeta { get; set; }

        [JsonProperty("type")]
        public SkillType Type { get; set; }
    }

    public enum PrimaryBonusType { Accuracy, CriticalAvoidance, CriticalChance, CriticalDamage, Defense, Health, Offense, Potency, Protection, Speed, Tenacity };

    public enum SecondaryType { CriticalChance, Defense, Health, Offense, Potency, Protection, SecondaryTypeDefense, SecondaryTypeHealth, SecondaryTypeOffense, SecondaryTypeProtection, Speed, Tenacity };

    public enum Set { CritChance, CritDamage, Defense, Health, Offense, Potency, Speed, Tenacity };

    public enum Slot { Arrow, Circle, Cross, Diamond, Square, Triangle };

    public enum SkillType { Basic, Contract, Hardware, Leader, Special, Unique };

    public enum RosterType { Char, Ship };

    public partial class Player
    {
        public static List<Player> GuildFromJson(string json) => JsonConvert.DeserializeObject<List<Player>>(json, swgoh_help_api.Converter.Settings);
        public static Player FromJson(string json) => JsonConvert.DeserializeObject<Player>(json, swgoh_help_api.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<Player> self) => JsonConvert.SerializeObject(self, swgoh_help_api.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                PrimaryBonusTypeConverter.Singleton,
                SecondaryTypeConverter.Singleton,
                SetConverter.Singleton,
                SlotConverter.Singleton,
                SkillTypeConverter.Singleton,
                RosterTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class PrimaryBonusTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(PrimaryBonusType) || t == typeof(PrimaryBonusType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Accuracy":
                    return PrimaryBonusType.Accuracy;
                case "Critical Avoidance":
                    return PrimaryBonusType.CriticalAvoidance;
                case "Critical Chance":
                    return PrimaryBonusType.CriticalChance;
                case "Critical Damage":
                    return PrimaryBonusType.CriticalDamage;
                case "Defense %":
                    return PrimaryBonusType.Defense;
                case "Health %":
                    return PrimaryBonusType.Health;
                case "Offense %":
                    return PrimaryBonusType.Offense;
                case "Potency":
                    return PrimaryBonusType.Potency;
                case "Protection %":
                    return PrimaryBonusType.Protection;
                case "Speed":
                    return PrimaryBonusType.Speed;
                case "Tenacity":
                    return PrimaryBonusType.Tenacity;
                default:
                    return PrimaryBonusType.Tenacity;
            }
            throw new Exception("Cannot unmarshal type PrimaryBonusType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (PrimaryBonusType)untypedValue;
            switch (value)
            {
                case PrimaryBonusType.Accuracy:
                    serializer.Serialize(writer, "Accuracy");
                    return;
                case PrimaryBonusType.CriticalAvoidance:
                    serializer.Serialize(writer, "Critical Avoidance");
                    return;
                case PrimaryBonusType.CriticalChance:
                    serializer.Serialize(writer, "Critical Chance");
                    return;
                case PrimaryBonusType.CriticalDamage:
                    serializer.Serialize(writer, "Critical Damage");
                    return;
                case PrimaryBonusType.Defense:
                    serializer.Serialize(writer, "Defense %");
                    return;
                case PrimaryBonusType.Health:
                    serializer.Serialize(writer, "Health %");
                    return;
                case PrimaryBonusType.Offense:
                    serializer.Serialize(writer, "Offense %");
                    return;
                case PrimaryBonusType.Potency:
                    serializer.Serialize(writer, "Potency");
                    return;
                case PrimaryBonusType.Protection:
                    serializer.Serialize(writer, "Protection %");
                    return;
                case PrimaryBonusType.Speed:
                    serializer.Serialize(writer, "Speed");
                    return;
                case PrimaryBonusType.Tenacity:
                    serializer.Serialize(writer, "Tenacity");
                    return;
            }
            throw new Exception("Cannot marshal type PrimaryBonusType");
        }

        public static readonly PrimaryBonusTypeConverter Singleton = new PrimaryBonusTypeConverter();
    }

    internal class SecondaryTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SecondaryType) || t == typeof(SecondaryType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);

            switch (value)
            {
                case "Critical Chance":
                    return SecondaryType.CriticalChance;
                case "Defense":
                    return SecondaryType.SecondaryTypeDefense;
                case "Defense %":
                    return SecondaryType.Defense;
                case "Health":
                    return SecondaryType.Health;
                case "Health %":
                    return SecondaryType.SecondaryTypeHealth;
                case "Offense":
                    return SecondaryType.Offense;
                case "Offense %":
                    return SecondaryType.SecondaryTypeOffense;
                case "Potency":
                    return SecondaryType.Potency;
                case "Protection":
                    return SecondaryType.Protection;
                case "Protection %":
                    return SecondaryType.SecondaryTypeProtection;
                case "Speed":
                    return SecondaryType.Speed;
                case "Tenacity":
                    return SecondaryType.Tenacity;
                default:
                    return SecondaryType.Tenacity;
            }
            throw new Exception("Cannot unmarshal type SecondaryType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (SecondaryType)untypedValue;
            switch (value)
            {
                case SecondaryType.CriticalChance:
                    serializer.Serialize(writer, "Critical Chance");
                    return;
                case SecondaryType.SecondaryTypeDefense:
                    serializer.Serialize(writer, "Defense");
                    return;
                case SecondaryType.Defense:
                    serializer.Serialize(writer, "Defense %");
                    return;
                case SecondaryType.Health:
                    serializer.Serialize(writer, "Health");
                    return;
                case SecondaryType.SecondaryTypeHealth:
                    serializer.Serialize(writer, "Health %");
                    return;
                case SecondaryType.Offense:
                    serializer.Serialize(writer, "Offense");
                    return;
                case SecondaryType.SecondaryTypeOffense:
                    serializer.Serialize(writer, "Offense %");
                    return;
                case SecondaryType.Potency:
                    serializer.Serialize(writer, "Potency");
                    return;
                case SecondaryType.Protection:
                    serializer.Serialize(writer, "Protection");
                    return;
                case SecondaryType.SecondaryTypeProtection:
                    serializer.Serialize(writer, "Protection %");
                    return;
                case SecondaryType.Speed:
                    serializer.Serialize(writer, "Speed");
                    return;
                case SecondaryType.Tenacity:
                    serializer.Serialize(writer, "Tenacity");
                    return;
            }
            throw new Exception("Cannot marshal type SecondaryType");
        }

        public static readonly SecondaryTypeConverter Singleton = new SecondaryTypeConverter();
    }

    internal class SetConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Set) || t == typeof(Set?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Crit Chance":
                    return Set.CritChance;
                case "Crit Damage":
                    return Set.CritDamage;
                case "Defense":
                    return Set.Defense;
                case "Health":
                    return Set.Health;
                case "Offense":
                    return Set.Offense;
                case "Potency":
                    return Set.Potency;
                case "Speed":
                    return Set.Speed;
                case "Tenacity":
                    return Set.Tenacity;
            }
            throw new Exception("Cannot unmarshal type Set");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Set)untypedValue;
            switch (value)
            {
                case Set.CritChance:
                    serializer.Serialize(writer, "Crit Chance");
                    return;
                case Set.CritDamage:
                    serializer.Serialize(writer, "Crit Damage");
                    return;
                case Set.Defense:
                    serializer.Serialize(writer, "Defense");
                    return;
                case Set.Health:
                    serializer.Serialize(writer, "Health");
                    return;
                case Set.Offense:
                    serializer.Serialize(writer, "Offense");
                    return;
                case Set.Potency:
                    serializer.Serialize(writer, "Potency");
                    return;
                case Set.Speed:
                    serializer.Serialize(writer, "Speed");
                    return;
                case Set.Tenacity:
                    serializer.Serialize(writer, "Tenacity");
                    return;
            }
            throw new Exception("Cannot marshal type Set");
        }

        public static readonly SetConverter Singleton = new SetConverter();
    }

    internal class SlotConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Slot) || t == typeof(Slot?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Arrow":
                    return Slot.Arrow;
                case "Circle":
                    return Slot.Circle;
                case "Cross":
                    return Slot.Cross;
                case "Diamond":
                    return Slot.Diamond;
                case "Square":
                    return Slot.Square;
                case "Triangle":
                    return Slot.Triangle;
            }
            throw new Exception("Cannot unmarshal type Slot");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Slot)untypedValue;
            switch (value)
            {
                case Slot.Arrow:
                    serializer.Serialize(writer, "Arrow");
                    return;
                case Slot.Circle:
                    serializer.Serialize(writer, "Circle");
                    return;
                case Slot.Cross:
                    serializer.Serialize(writer, "Cross");
                    return;
                case Slot.Diamond:
                    serializer.Serialize(writer, "Diamond");
                    return;
                case Slot.Square:
                    serializer.Serialize(writer, "Square");
                    return;
                case Slot.Triangle:
                    serializer.Serialize(writer, "Triangle");
                    return;
            }
            throw new Exception("Cannot marshal type Slot");
        }

        public static readonly SlotConverter Singleton = new SlotConverter();
    }

    internal class SkillTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SkillType) || t == typeof(SkillType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Basic":
                    return SkillType.Basic;
                case "Contract":
                    return SkillType.Contract;
                case "Hardware":
                    return SkillType.Hardware;
                case "Leader":
                    return SkillType.Leader;
                case "Special":
                    return SkillType.Special;
                case "Unique":
                    return SkillType.Unique;
            }
            throw new Exception("Cannot unmarshal type SkillType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (SkillType)untypedValue;
            switch (value)
            {
                case SkillType.Basic:
                    serializer.Serialize(writer, "Basic");
                    return;
                case SkillType.Contract:
                    serializer.Serialize(writer, "Contract");
                    return;
                case SkillType.Hardware:
                    serializer.Serialize(writer, "Hardware");
                    return;
                case SkillType.Leader:
                    serializer.Serialize(writer, "Leader");
                    return;
                case SkillType.Special:
                    serializer.Serialize(writer, "Special");
                    return;
                case SkillType.Unique:
                    serializer.Serialize(writer, "Unique");
                    return;
            }
            throw new Exception("Cannot marshal type SkillType");
        }

        public static readonly SkillTypeConverter Singleton = new SkillTypeConverter();
    }

    internal class RosterTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(RosterType) || t == typeof(RosterType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "char":
                    return RosterType.Char;
                case "ship":
                    return RosterType.Ship;
            }
            throw new Exception("Cannot unmarshal type RosterType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (RosterType)untypedValue;
            switch (value)
            {
                case RosterType.Char:
                    serializer.Serialize(writer, "char");
                    return;
                case RosterType.Ship:
                    serializer.Serialize(writer, "ship");
                    return;
            }
            throw new Exception("Cannot marshal type RosterType");
        }

        public static readonly RosterTypeConverter Singleton = new RosterTypeConverter();
    }
}
