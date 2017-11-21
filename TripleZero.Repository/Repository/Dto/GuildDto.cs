using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TripleZero.Repository.Dto
{
    internal partial class GuildDto
    {
        public string Name { get; set; }
        public DateTime LastSwGohUpdated { get; set; }
        public DateTime LastClassUpdated { get; set; }
        public int GP { get; set; }
        public int GPaverage { get; set; }
        public List<string> PlayerNames { get; set; }
        public List<PlayerDto> Players { get; set; }
    }
}
