using System;
using System.Collections.Generic;
using Eze.Api.Entities;

namespace Eze.Api.Dtos
{
    public record RequestPendingDto(Guid Id, IEnumerable<Item> Items, DateTimeOffset CreatedDate, string StudentName, string ProfessorName, string Code, bool Status);
}