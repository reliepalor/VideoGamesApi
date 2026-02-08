using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Auth.Dto.Auth
{
    public class UserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
