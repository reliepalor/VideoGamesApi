using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using VideoGameApi.Data;
using System.ComponentModel.DataAnnotations;
using VideoGameApi.Auth.Dto.Auth;
using VideoGameApi.Auth.Interfaces;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Auth.Services
{
    public class AuthService : IAuthService
    {
        private readonly VideoGameDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(VideoGameDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginDto request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return null;

            var isValidPassword = BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.Password
            );

            if (!isValidPassword)
                return null;

            var token = CreateToken(user);

            return new TokenResponseDto
            {
                AccessToken = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.IsAdmin ? "Admin" : "User"
            };
        }

        public async Task<TokenResponseDto?> RegisterAsync(RegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return null;

            var newUser = new User
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Username = request.Email.Split('@')[0],
                IsAdmin = false
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreateTokenResponse(newUser);
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return null;

            return CreateTokenResponse(user);
        }

        public async Task<IEnumerable<User>> GetUsersAsync() => await _context.Users.ToListAsync();

        public async Task<User?> GetUserInfoAsync(int id) => await _context.Users.FindAsync(id);

        public async Task<IEnumerable<User>> GetNonAdminUserAsync() =>
            await _context.Users.Where(u => !u.IsAdmin).ToListAsync();

        public async Task<bool> UpdateUserPasswordAsync(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }

        // Implement string userId version (parse to int and call int version)
        public async Task<bool> UpdateUserPasswordAsync(string userId, string newPassword)
        {
            if (!int.TryParse(userId, out int id))
                return false;

            return await UpdateUserPasswordAsync(id, newPassword);
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private TokenResponseDto CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user)
            };
        }

        private string CreateToken(User user)
        {
            var tokenKey = _configuration["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(tokenKey))
                throw new Exception("JWT Key is missing in configuration");

            var claims = new List<Claim>
            {
                new Claim("nameid", user.Id.ToString()),
                new Claim("email", user.Email),
                new Claim("username", user.Username),
                new Claim("role", user.IsAdmin ? "Admin" : "User")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            if (!int.TryParse(userId, out int id))
                return null;

            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserInfoAsync(string userId)
        {
            if (!int.TryParse(userId, out int id))
                return null;

            return await _context.Users.FindAsync(id);
        }

        // Firebase login methods
        public async Task<User> CreateFirebaseUserAsync(string email, string firebaseUid)
        {
            var user = new User
            {
                Email = email,
                Username = email.Split('@')[0],
                FirebaseUid = firebaseUid,
                AuthProvider = "Google",
                IsAdmin = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<TokenResponseDto> GenerateJwtForUserAsync(User user)
        {
            // Generate JWT and return TokenResponseDto
            var token = CreateToken(user);
            return await Task.FromResult(new TokenResponseDto
            {
                AccessToken = token,
                UserId = user.Id,
                Email = user.Email,
                Role = user.IsAdmin ? "Admin" : "User"
            });
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<TokenResponseDto> LoginWithFirebaseAsync(string email, string username)
        {
            // Use current UTC time for createdAt
            return await LoginWithFirebaseAsync(email, username, null, DateTime.UtcNow);
        }

        // Optionally, update the existing LoginWithFirebaseAsync to make createdAt optional

        public async Task<TokenResponseDto> LoginWithFirebaseAsync(
        string email,
        string username,
        string firebaseUid,
        DateTime createdAt)
        {
            if (string.IsNullOrEmpty(firebaseUid))
                throw new Exception("Firebase UID is required");

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.FirebaseUid == firebaseUid);

            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    Username = username,
                    FirebaseUid = firebaseUid,
                    AuthProvider = "Google",
                    IsExternalAuth = true,
                    CreatedAt = createdAt,
                    LastLoginAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
            }
            else
            {
                user.LastLoginAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                UserId = user.Id,
                Email = user.Email,
                Role = user.IsAdmin ? "Admin" : "User"
            };
        }


    }
}
