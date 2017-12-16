using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class Arena
    {
        public int CurrentRank { get; set; }
        public int AverageRank { get; set; }
        public List<string> ArenaTeam { get; set; }
    }
}
