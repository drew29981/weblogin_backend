using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using wblg.contexts;
using wblg.models;

namespace wblg.controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userInfo = new
            {
                Username = User.Identity?.Name,
            };
            if (userInfo == null || string.IsNullOrEmpty(userInfo.Username)) { return Unauthorized(); }

            var user = await _context.FindByUser(userInfo.Username);

            if (user == null)
                return NotFound("User not found");

            return Ok(new
            {
                user.Username,
                user.Email,
                user.Id
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (user == null)
            {
                return BadRequest("Invalid user registration details.");
            }
            if (string.IsNullOrWhiteSpace(user.Username)
            || string .IsNullOrWhiteSpace(user.Email)
            || string .IsNullOrWhiteSpace(user.Password))
            {
                return BadRequest("All fields are required.");
            }

            PasswordHasher<User> hasher = new();
            var hashedPassword = hasher.HashPassword(user, user.Password);

            var userDTO = new User
            {
                Username = user.Username,
                Email = user.Email,
                Password = hashedPassword // TODO: hash this later!
            };

            if (await _context.ContainsUsernameAsync(user.Username))
            {
                return BadRequest("User already exists!");
            }
            await _context.AddAsync(userDTO);
            await _context.SaveChangesAsync();

            return Ok(userDTO);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO user)
        {
            var userProfile = await _context.FindByUser(user.UsernameOrEmail);

            if (userProfile == null)
            {
                return Unauthorized("Invalid username or password.");
                
            }
            
            PasswordHasher<User> hasher = new();
            var result = hasher.VerifyHashedPassword(userProfile, userProfile.Password, user.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid username or password.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userProfile.Username),
                new Claim(ClaimTypes.NameIdentifier, userProfile.Id.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return Ok(new
            {
                userProfile.Id,
                userProfile.Username,
                userProfile.Email,
            });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return Ok("Logged out successfully.");
        }
    }
}