using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using static SWGoH.Enums.QueueEnum;

namespace SwGoH
{
    public class Queue
    {
        public Nullable<ObjectId> Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateForProcess { get; set; }
        public string Command { get; set; }
        public int Priority { get; set; }
        public QueueStatus Status { get; set; }
        public QueueType Type { get; set; }
    }
}
