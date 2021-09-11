using System;
using System.Collections.Generic;
using Eze.Api.Entities;

namespace Eze.Api.Dtos
{
    public record RequestDto(Guid Id, IEnumerable<Guid> ItemIds, DateTimeOffset CreatedDate, string StudentName, Guid ProfessorId, string Code, bool Status, string Description);
}