using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.Dto
{
    internal class GuildUnitDto
    {
        public string Name { get; set; }
        public List<GuildPlayerUnitDto> Players { get; set; }
    }
}
