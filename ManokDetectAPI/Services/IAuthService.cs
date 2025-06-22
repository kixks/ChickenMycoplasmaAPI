using ManokDetectAPI.Entities;
using ManokDetectAPI.DTO;

namespace ManokDetectAPI.Services
{
    public interface IAuthService
    {
        Task<userDto?> RegisterAsync(userRegisterDto request);
        Task<TokenResponseDto?> LoginAsync (userLoginDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RequestRefreshDto request);
        Task<User> FindOrRegisterGoogleUser(string email, string name);
        Task<User?> GetUserBySecurityIdAsync(Guid securityID);
        Task UpdateUserAsync(User user);
    }
}
