using System.ComponentModel.DataAnnotations;

namespace Eze.Api.Dtos
{
    public record CreateAccountDto
    {
        [Required]
        public string Name { get; init; }

        [Required]
        public string Username { get; init; }
        
        [Required]
        public string Password { get; init; }
    }
}