using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Eze.Api.Entities
{
    public class Request
    {
        [BsonId]
        public Guid Id { get; set; }
        public List<Guid> ItemIds { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public string StudentName { get; set; }
        public Guid ProfessorId { get; set; }
        public string Code { get; set; }
        public bool Status { get; set; }
        public string Description { get; set; }
    }
}