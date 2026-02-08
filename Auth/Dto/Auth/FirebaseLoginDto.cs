using System.ComponentModel.DataAnnotations;

namespace VideoGameApi.Auth.Dto.Auth
{
    public class FirebaseLoginDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}
