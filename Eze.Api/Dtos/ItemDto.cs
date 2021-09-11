using System;

namespace Eze.Api.Dtos
{
    public record ItemDto(Guid Id, string Name, string Description, string Condition, DateTimeOffset CreatedDate);
}