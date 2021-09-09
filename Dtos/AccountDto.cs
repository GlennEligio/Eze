using System;

namespace Eze.Dtos
{
    public record AccountDto(Guid Id, string Name, string Username, string Password);
}