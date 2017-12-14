using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TripleZero.Repository.Dto
{
    internal class ShipDto : UnitDto
    {        
        public List<ShipAbilityDto> Abilities { get; set; }
        public List<string> Crew { get; set; }        
    }
}
