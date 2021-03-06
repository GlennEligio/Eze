using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Eze.Api.Entities
{
    public class Item
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Condition { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}