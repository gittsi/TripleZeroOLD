﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.Dto
{
    internal class GuildCharacterDto
    {
        public string Name { get; set; }
        public List<GuildPlayerCharacterDto> Players { get; set; }
    }
}