using System;

namespace Eze.Api.Dtos
{
    public record AccountDto(Guid Id, string Name, string Username, string Password, DateTimeOffset CreatedDate);
}