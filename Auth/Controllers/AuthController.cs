using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FirebaseAdmin.Auth;
using VideoGameApi.Auth.Dto.Auth;
using VideoGameApi.Auth.Interfaces;
using VideoGameApi.Models.Users;

namespace VideoGameApi.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(request);

            if (result == null)
                return Unauthorized(new
                {
                    message = "Invalid email or password"
                });

            return Ok(result);
        }


        // register---------
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> Register([FromBody] RegisterDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);

            if (result == null)
                return BadRequest(new
                {
                    message = "Registration failed. Email or username may already exist."
                });

            return Ok(result);
        }

        [HttpGet("isAdmin")]
        [Authorize]
        public async Task<ActionResult> IsAdmin()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User Not Found");

            return Ok(new { isAdmin = user.IsAdmin });
        }

        [HttpGet("getUserByID")]
        [Authorize]
        public async Task<ActionResult> GetUserByID()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _authService.GetUserInfoAsync(userId);
            if (user == null)
                return BadRequest("User Not Found.");

            return Ok(new
            {
                name = user.Username,
                email = user.Email,
            });
        }

        [HttpGet("users/non-admin")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<User>>> GetNonAdminUsers()
        {
            var users = await _authService.GetNonAdminUserAsync();
            return Ok(users);
        }

        [HttpPut("update-password")]
        [Authorize]
        public async Task<ActionResult> UpdatePassword([FromBody] UpdatePasswordDto updatePasswordDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not identified.");

            var success = await _authService.UpdateUserPasswordAsync(userId, updatePasswordDto.NewPassword);
            if (!success)
                return BadRequest("Failed to update password.");

            return Ok("Password updated successfully.");
        }

        [HttpDelete("delete-user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (!int.TryParse(userId, out int parsedUserId))
                return BadRequest("Invalid user id.");

            var success = await _authService.DeleteUserAsync(parsedUserId);
            if (!success)
                return BadRequest("Failed to delete user.");

            return Ok("User deleted successfully.");
        }

        //google login authentication
        [HttpPost("firebase")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> FirebaseLogin(
             [FromBody] FirebaseAuthDto dto)
        {
            var decodedToken = await FirebaseAuth.DefaultInstance
                .VerifyIdTokenAsync(dto.IdToken);

            var email = decodedToken.Claims["email"]?.ToString();
            var name = decodedToken.Claims["name"]?.ToString();
            var uid = decodedToken.Uid;

            var createdAtUnix = long.Parse(decodedToken.Claims["auth_time"].ToString());
            var createdAt = DateTimeOffset.FromUnixTimeSeconds(createdAtUnix).UtcDateTime;

            var token = await _authService.LoginWithFirebaseAsync(
                email!,
                name ?? email!.Split('@')[0],
                uid,
                createdAt
            );

            return Ok(token);
        }

        //get email and profile picture
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? User.FindFirst("nameid");

            if (userIdClaim == null)
                return Unauthorized("Invalid token");

            var userId = userIdClaim.Value;

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                id = user.Id,
                name = user.Username,
                email = user.Email,
                provider = user.AuthProvider,
                isAdmin = user.IsAdmin,
                createdAt = user.CreatedAt,
                lastLoginAt = user.LastLoginAt
            });
        }
    }
}
    