using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ManokDetectAPI.Entities;
using ManokDetectAPI.Database;
using ManokDetectAPI.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ManokDetectAPI.Services
{
    public class AuthService(manokDetectDBContext context, IConfiguration configuration) : IAuthService
    {
        //private methods--------------------------------------------------------------------
        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = CreateToken(user),
                RefreshToken = await SaveRefreshTokenAsync(user)
            };
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task<string> SaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            context.Users.Update(user);
            await context.SaveChangesAsync();
            return refreshToken;
        }

        private async Task<User?> ValidateRefreshTokenAsync(Guid securityID, string refreshToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(s => s.securityID == securityID);
            if (user is null || user.RefreshToken != refreshToken
                || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.MobilePhone, user.MobileNumber!),
                new Claim(ClaimTypes.NameIdentifier, user.securityID.ToString()),
                new Claim(ClaimTypes.Role, user.UserType!)
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
        //----------------------------------------------------------------------------------------


        //public methods-----------------------------------------------------------------------
        public async Task<TokenResponseDto?> LoginAsync(userLoginDto request)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.MobileNumber == request.MobileNumber);
            if (user is null)
            {
                return null;
            }
            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
            {
                return null;
            }
            return await CreateTokenResponse(user);
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
        public async Task<TokenResponseDto?> RefreshTokenAsync(RequestRefreshDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.securityID, request.RefreshToken);
            if (user is null)
            {
                return null;
            }
            return await CreateTokenResponse(user);
        }

        public async Task<User> FindOrRegisterGoogleUser(string email, string name)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if(user is null)
            {
                user = new User
                {
                    Email = email,
                    Name = name,
                    securityID = Guid.NewGuid(),
                    Address = "",
                    MobileNumber = "",
                    UserType = "Farmer",
                    PasswordHash = ""
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }
            return user;
        }
        public async Task<TokenResponseDto>GenerateTokenForGoogleUser(User user)
        {
            return await CreateTokenResponse(user);
        }

        //-------------------------------------------------------------------------------------
    }
}
