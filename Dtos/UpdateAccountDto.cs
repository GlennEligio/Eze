using System.ComponentModel.DataAnnotations;

namespace Eze.Dtos
{
    public record UpdateAccountDto
    {
        [Required]
        public string Name { get; init; }
        [Required]
        public string Username { get; init; }
        [Required]
        public string Password { get; init; }
    }
}