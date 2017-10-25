using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TripleZero.Configuration.SWGoH;

namespace TripleZero.Configuration
{
    public class CharacterSettings
    {        

        public SWGoHCharacterSettings Get(string alias)
        {           

            using (StreamReader r = new StreamReader("Configuration/characters.json"))
            {
                string json = r.ReadToEnd();

                try
                {
                    CharacterSettingsModel result = JsonConvert.DeserializeObject<CharacterSettingsModel>(json);
                    var matched = result.Characters.Where(p => p.Aliases.Contains(alias)).FirstOrDefault();

                    return matched;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            
        }
    }
}
