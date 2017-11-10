using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwGoH
{
    public class QueuePlayer
    {
        public Nullable <ObjectId> Id { get; set; }
        public string PlayerName { get; set; }
        public DateTime Date { get; set; }
        public string Command { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
    }
}
