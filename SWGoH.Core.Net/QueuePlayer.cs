using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace SWGoH
{
    public class QueuePlayer
    {
        public ObjectId Id { get; set; }
        public string PlayerName { get; set; }
        public DateTime Date { get; set; }
    }
}
