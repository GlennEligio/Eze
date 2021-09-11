using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Eze.Api.Entities
{
    public class Account
    {
       [BsonId]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}