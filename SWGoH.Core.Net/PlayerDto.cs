using System;
using System.Collections.Generic;

namespace SwGoh
{
    public partial class PlayerDto
    {
        private System.Net.WebClient web = null;
        //private int mDelayCharacter = 4000;
        private int mDelayCharacter = 500;
        private int mDelayError = 600000;
        public PlayerDto(string name)
        {
            PlayerName = name;
        }
        internal string id { get; set; }
        public string GuildName { get; set; }
        public string PlayerName { get; set; }
        public string PlayerNameInGame { get; set; }
        public DateTime LastSwGohUpdated { get; set; }
        public Nullable<DateTime> LastClassUpdated { get; set; }
        public int GPcharacters { get; set; }
        public int GPships { get; set; }
        public List<CharacterDto> Characters { get; set; }
    }
    public enum ExportMethodEnum
    {
        File = 1,
        Database = 2,
    }
}