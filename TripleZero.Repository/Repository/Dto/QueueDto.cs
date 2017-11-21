using MongoDB.Bson;
using System;
using TripleZero.Repository.EnumDto;

namespace TripleZero.Repository.Dto
{   

    internal class QueueDto
    {
        public Nullable<ObjectId> Id { get; set; }
        public string Name { get; set; }        
        public string InsertedDate { get; set; }        
        public string ProcessingStartDate { get; set; }        
        public string NextRunDate { get; set; }
        public QueueCommand Command { get; set; }
        public PriorityEnum Priority { get; set; }
        public QueueStatus Status { get; set; }
        public QueueType Type { get; set; }
        public string ComputerName { get; set; }
    }
}
