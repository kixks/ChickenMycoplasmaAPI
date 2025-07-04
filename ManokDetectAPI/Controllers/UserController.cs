﻿using ManokDetectAPI.Database;
using ManokDetectAPI.DTO;
using ManokDetectAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManokDetectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly manokDetectDBContext _context;

        public UserController(manokDetectDBContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<userDto>>> GetAllUsers()
        {
            var users = await _context.Users
                .Select(user => new userDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Address = user.Address,
                    MobileNumber = user.MobileNumber,
                    UserType = user.UserType,
                    securityID = user.securityID
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<userDto>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var userDetails = new userDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                MobileNumber = user.MobileNumber,
                UserType = user.UserType,
                securityID = user.securityID
            };
            return Ok(userDetails);
        }
        [HttpPut]
        public async Task<ActionResult<User>> UpdateUser([FromBody] userUpdateDto request)
        {
            var user = await _context.Users.FindAsync(request.Id);

            if (user == null)
                return NotFound($"User with ID {request.Id} not found.");

            // Update fields
            user.Name = request.Name;
            user.Email = request.Email;
            user.Address = request.Address;
            user.MobileNumber = request.MobileNumber;

            if (!string.IsNullOrEmpty(request.Password))
            {
                var hashedPassword = new PasswordHasher<User>().HashPassword(user, request.Password);  
                user.PasswordHash = hashedPassword;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Failed to update user: {ex.Message}");
            }

            return Ok(user); 
        }
    }
}
