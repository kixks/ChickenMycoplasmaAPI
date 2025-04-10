using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ManokDetectAPI.Entities;
using ManokDetectAPI.Model;
using ManokDetectAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ManokDetectAPI.Services
{
    public class AuthService(manokDetectDBContext context, IConfiguration configuration) : IAuthService
    {
        public async Task<string?> LoginAsync(userLoginDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.MobileNumber == request.MobileNumber);
            if(user is null)
            {
                return null;
            }
            if(new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return CreateToken(user);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.MobilePhone, user.MobileNumber),
                new Claim(ClaimTypes.NameIdentifier, user.securityID.ToString()),
                new Claim(ClaimTypes.Role, user.UserType)
            };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration.GetValue<string>("Appsettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: configuration.GetValue<string>("Appsettings:Issuer"),
                audience: configuration.GetValue<string>("Appsettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials:creds
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<User?> RegisterAsync(userRegisterDto request)
        {
            if (await context.Users.AnyAsync(u => u.MobileNumber == request.MobileNumber))
            {
                return null;
            }
            var user = new User();
            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user,request.Password);

            user.Name = request.Name;
            user.Email = request.Email;
            user.Address = request.Address;
            user.MobileNumber = request.MobileNumber;
            user.PasswordHash = hashedPassword;
            user.securityID = Guid.NewGuid();    

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user;

        }
    }
}
