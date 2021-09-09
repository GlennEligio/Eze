using System;
using System.Collections.Generic;
using Eze.Entities;

namespace Eze.Dtos
{
    public record RequestDto(Guid Id, IEnumerable<Item> Items, DateTimeOffset CreatedDate, string StudentName, string ProfessorName, string Code, bool Status);
}