using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eze.Api.Entities;

namespace Eze.Api.Dtos
{
    public record UpdateRequestDto
    {
        [Required]
        public bool Status { get; init; }
    }
}