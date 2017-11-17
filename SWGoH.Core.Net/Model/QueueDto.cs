using MongoDB.Bson;
using System;
using SWGoH.Enums.QueueEnum;
using MongoDB.Bson.Serialization.Attributes;

namespace SWGoH
{
    public class QueueDto
    {
        public Nullable<ObjectId> Id { get; set; }
        public string Name { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime InsertedDate { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public Nullable<DateTime> ProcessingStartDate { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public Nullable<DateTime> NextRunDate { get; set; }
        public Command Command { get; set; }
        public int Priority { get; set; }
        public QueueStatus Status { get; set; }
        public QueueType Type { get; set; }
    }
}
