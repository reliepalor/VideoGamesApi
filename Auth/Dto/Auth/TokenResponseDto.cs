namespace VideoGameApi.Auth.Dto.Auth
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
