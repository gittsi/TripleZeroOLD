using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SwGoh
{
    public class CharactersConfig
    {
        public List<CharacterConfig> Characters { get; set; }
    }

    public class CharacterConfig
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public List<string> Aliases { get; set; }
        public string SWGoHUrl { get; set; }
    }
}
