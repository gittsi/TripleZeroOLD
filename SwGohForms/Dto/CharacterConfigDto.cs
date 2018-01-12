using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace SWGoH
{
    public class CharacterConfigDto
    {
        public Nullable<ObjectId> Id { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public List<string> Aliases { get; set; }
        public string SWGoHUrl { get; set; }
        public List<ConfigAbility> Abilities { get; set; }
    }

    public class ShipConfigDto
    {
        public Nullable<ObjectId> Id { get; set; }
        public string Name { get; set; }
        public string Command { get; set; }
        public List<string> Aliases { get; set; }
        public string SWGoHUrl { get; set; }
    }
}
