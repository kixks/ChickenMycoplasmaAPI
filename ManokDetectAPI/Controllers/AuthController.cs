using ManokDetectAPI.Entities;
using ManokDetectAPI.Models;
using ManokDetectAPI.Services;
using Microsoft.AspNetCore.Authorization;
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
    }
}
