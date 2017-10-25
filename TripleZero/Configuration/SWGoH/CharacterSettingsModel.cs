using System;
using System.Collections.Generic;
using System.Text;
using TripleZero.Model;

namespace TripleZero.Configuration.SWGoH
{
    public class CharacterSettingsModel
    {
        public List<SWGoHCharacterSettings> Characters { get; set; }        
    }

    public class SWGoHCharacterSettings
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public List<string> Aliases { get; set; }        
    }


}
