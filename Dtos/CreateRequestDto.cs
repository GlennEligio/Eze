using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Eze.Entities;

namespace Eze.Dtos
{
    public record CreateRequestDto
    {
        [Required]
        public List<Guid> ItemIds { get; init; }

        [Required]
        public string StudentName { get; init; }

        [Required]
        public Guid ProfessorId { get; init; }
        
        public string Description { get; init; }
    }
}