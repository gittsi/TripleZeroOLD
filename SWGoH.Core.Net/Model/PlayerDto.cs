using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SWGoH
{
    public partial class PlayerDto
    {
        private System.Net.WebClient web = null;
        
        public PlayerDto(string name)
        {
            PlayerName = name;
        }
        public Nullable<ObjectId> Id { get; set; }
        public string GuildName { get; set; }
        public string PlayerName { get; set; }
        public string PlayerNameInGame { get; set; }
        public DateTime LastSwGohUpdated { get; set; }
        public Nullable<DateTime> LastClassUpdated { get; set; }
        public int GPcharacters { get; set; }
        public int GPships { get; set; }
        public List<CharacterDto> Characters { get; set; }
        public List<ShipDto> Ships { get; set; }
    }
    public enum ExportMethodEnum
    {
        File = 1,
        Database = 2,
    }
}