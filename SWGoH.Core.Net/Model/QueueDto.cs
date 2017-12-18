using MongoDB.Bson;
using System;
using SWGoH.Enums.QueueEnum;
using MongoDB.Bson.Serialization.Attributes;

namespace SWGoH
{
    public enum PriorityEnum
    {
        DailyUpdate = 1,
        ManualLoad = 2
    }

    public class QueueDto
    {
        public Nullable<ObjectId> Id { get; set; }
        public string Name { get; set; }
        public string Guild { get; set; }
        public string InsertedDate { get; set; }
        
        public string ProcessingStartDate { get; set; }
        
        public string NextRunDate { get; set; }
        public Command Command { get; set; }
        public PriorityEnum Priority { get; set; }
        public QueueStatus Status { get; set; }
        public QueueType Type { get; set; }
        public string ComputerName { get; set; }
    }
}
