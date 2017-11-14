using MongoDB.Bson;
using System;
using SWGoH.Enums.QueueEnum;

namespace SWGoH
{
    public class Queue
    {
        public Nullable<ObjectId> Id { get; set; }
        public string Name { get; set; }
        public DateTime InsertedDate { get; set; }
        public Nullable<DateTime> ProcessingStartDate { get; set; }
        public Nullable<DateTime> NextRunDate { get; set; }
        public Command Command { get; set; }
        public int Priority { get; set; }
        public QueueStatus Status { get; set; }
        public QueueType Type { get; set; }
    }
}
