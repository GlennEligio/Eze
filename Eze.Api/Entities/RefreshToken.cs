using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Eze.Api.Entities
{
    public class RefreshToken
    {
        [BsonId]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public DateTimeOffset ExpiryDate { get; set; }
    }
}