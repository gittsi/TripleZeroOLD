using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH.Model
{
    public class Ship
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string SWGoHUrl { get; set; }
        public int Stars { get; set; }
        public int Level { get; set; }        
        public int Power { get; set; }        
    }
}
