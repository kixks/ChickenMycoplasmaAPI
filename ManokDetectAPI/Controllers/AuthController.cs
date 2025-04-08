using ManokDetectAPI.Entities;
using ManokDetectAPI.Models;
using ManokDetectAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<User>>Login (userLoginDto request)
        {
            var token = await authService.LoginAsync(request);
            if(token is null)
            {
                return BadRequest("Invalid Credentials");
            }
            return Ok(token);
        }
    }
}
