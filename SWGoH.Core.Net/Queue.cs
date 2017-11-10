using MongoDB.Bson;
using System;
using static SwGoh.Enums.QueueEnum;

namespace SwGoh
{
    public class Queue
    {
        public Nullable<ObjectId> Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateForProcess { get; set; }
        public SwGoh.Enums.Command Command { get; set; }
        public int Priority { get; set; }
        public QueueStatus Status { get; set; }
        public QueueType Type { get; set; }
    }
}
