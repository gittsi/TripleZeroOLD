using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.Dto
{
    public class ArenaDto
    {
        public int CurrentRank { get; set; }
        public int AverageRank { get; set; }
        public List<string> ArenaTeam { get; set; }
    }
}
