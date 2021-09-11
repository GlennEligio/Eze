using System.ComponentModel.DataAnnotations;

namespace Eze.Api.Dtos
{
    public record CreateItemDto
    {
        [Required]
        public string Name { get; init; }
        [Required]
        public string Description { get; init; }
        [Required]
        public string Condition { get; init; }
    }
}