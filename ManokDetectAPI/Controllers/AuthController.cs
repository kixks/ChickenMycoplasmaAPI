using System.Security.Claims;
using ManokDetectAPI.Entities;
using ManokDetectAPI.DTO;
using ManokDetectAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

namespace ManokDetectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]  
        public async Task<ActionResult<User>> Register (userRegisterDto request)
        {
            var user = await authService.RegisterAsync(request);
            if(user is null)
            {
                return BadRequest("Mobile Number already exists");
            }
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>>Login (userLoginDto request)
        {
            var result = await authService.LoginAsync(request);
            if(result is null)
            {
                return BadRequest("Invalid Credentials");
            }
            return Ok(result);
        }


        [Authorize(Roles = "Vet")]
        [HttpGet("vet-only")]
        public IActionResult VetOnlyEndpoint()
        {
            return Ok("This can be accessed only by vets");
        }

        [Authorize(Roles = "Farmer")]
        [HttpGet("farmer-only")]
        public IActionResult FarmerOnlyEndpoint()
        {
            return Ok("This can be accessed only by farmers");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RequestRefreshDto request)
        {
            var result = await authService.RefreshTokenAsync(request);
            if(result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid Refresh Token");
            }
            return Ok(result);
        }

        [HttpGet("signin-google")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                // This is where you want users to end up after auth
                RedirectUri = "https://kingfish-wealthy-calf.ngrok-free.app/api/Auth/google-callback"
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            // Authenticate with Google
            var authResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authResult.Succeeded)
            {
                return BadRequest("Google auth failed");
            }

            // Extract claims
            var email = authResult.Principal.FindFirstValue(ClaimTypes.Email);
            var name = authResult.Principal.FindFirstValue(ClaimTypes.Name);

            // Find/create user
            var user = await authService.FindOrRegisterGoogleUser(email!, name!);
            if (user == null)
            {
                return BadRequest("User creation failed");
            }               

            var concreteAuthService = authService as AuthService;
            if (concreteAuthService == null)
            {
                return StatusCode(500, "Internal auth service error");
            }

            var tokens = await concreteAuthService.GenerateTokenForGoogleUser(user);
            var json = JsonSerializer.Serialize(tokens);
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            await HttpContext.SignOutAsync();

            return Redirect($"https://mycocheck.netlify.app/google-auth-success?data={base64}");

        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var securityIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (securityIdClaim == null || !Guid.TryParse(securityIdClaim, out var securityId))
            {
                return Unauthorized("Invalid user context.");
            }

            var user = await authService.GetUserBySecurityIdAsync(securityId);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            await authService.UpdateUserAsync(user);

            return Ok("Logged out successfully.");
        }



    }
}
