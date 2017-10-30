using SwGoh;
using System;
using System.Collections.Generic;
using System.Text;

namespace TripleZero.Repository.Dto
{
    public class PlayerDto
    {
        public string PlayerName { get; set; }
        public DateTime LastUpdated { get; set; }
        public List<CharacterDto> Characters { get; set; }
    }


}
