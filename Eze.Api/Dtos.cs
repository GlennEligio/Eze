using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eze.Api.Entities;

namespace Eze.Api.Dtos
{
    //Account related objects
    public record AccountDto(Guid Id, string Name, string Username, string Password, DateTimeOffset CreatedDate);
    public record CreateAccountDto([Required] string Name, [Required] string Username, [Required] string Password);
    public record UpdateAccountDto([Required] string Name, [Required] string Username, [Required] string Password);

    //Item related objects
    public record ItemDto(Guid Id, string Name, string Description, string Condition, DateTimeOffset CreatedDate);
    public record CreateItemDto([Required] string Name, [Required] string Description, [Required] string Condition);
    public record UpdateItemDto([Required] string Name, [Required] string Description, [Required] string Condition);

    //Request related objects
    public record RequestDto(Guid Id, IEnumerable<Guid> ItemIds, DateTimeOffset CreatedDate, string StudentName, Guid ProfessorId, string Code, bool Status, string Description);
    public record CreateRequestDto([Required] IEnumerable<Guid> ItemIds, [Required] string StudentName, [Required] Guid ProfessorId, [Required] string Description);
    public record UpdateRequestDto([Required] bool Status);
    public record RequestPendingDto(Guid Id, IEnumerable<Item> Items, DateTimeOffset CreatedDate, string StudentName, string ProfessorName, string Code, bool Status);
}