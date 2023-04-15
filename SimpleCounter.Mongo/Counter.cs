using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

using SimpleCounter.Common;

namespace SimpleCounter.Mongo
{
    public class Counter : ICounterItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public Guid CounterId { get; set; }
        public int CounterValue { get; set; }
    }
}
