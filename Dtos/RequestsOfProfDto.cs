using System.Collections.Generic;
using Eze.Entities;

namespace Eze.Dtos
{
    public record RequestOfProfDto
    {
        public IEnumerable<Request> Requests { get; init; }
    }
}