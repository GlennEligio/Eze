using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eze.Entities;

namespace Eze.Dtos
{
    public record UpdateRequestDto
    {
        [Required]
        public bool Status { get; init; }
    }
}