using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace TripleZero.Repository.Dto
{
    internal partial class PlayerDto
    {      
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
        public ArenaDto Arena { get; set; }
    }   
}