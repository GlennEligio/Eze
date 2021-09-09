using System;

namespace Eze.Dtos
{
    public record ItemDto(Guid Id, string Name, string Description, string Condition);
}