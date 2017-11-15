﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class Player
    {   
        public string Id { get; set; }
        public string GuildName { get; set; }
        public string PlayerName { get; set; }
        public string PlayerNameInGame { get; set; }
        public DateTime SWGoHUpdateDate { get; set; }
        public DateTime EntryUpdateDate { get; set; }
        public int GalacticPowerCharacters { get; set; }
        public int GalacticPowerShips { get; set; }
        public List<Character> Characters { get; set; }
    }
}