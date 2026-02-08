using System.Collections.Generic;
using System.Threading.Tasks;
using VideoGameApi.Auth.Dto.Auth;
using VideoGameApi.Models.Users;


namespace VideoGameApi.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> LoginAsync(LoginDto request);
        Task<TokenResponseDto?> RegisterAsync(RegisterDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserInfoAsync(int id);
        Task<IEnumerable<User>> GetNonAdminUserAsync();
        Task<bool> UpdateUserPasswordAsync(string userId, string newPassword);
        Task<bool> DeleteUserAsync(int userId);
        Task<User> GetUserByIdAsync(string userId);
        Task<User> GetUserInfoAsync(string userId);

        // Firebase login
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> CreateFirebaseUserAsync(string email, string firebaseUid);
        Task<TokenResponseDto> GenerateJwtForUserAsync(User user);
        Task<TokenResponseDto> LoginWithFirebaseAsync(string email, string username, string firebaseUid, DateTime createdAt);
    }
}
