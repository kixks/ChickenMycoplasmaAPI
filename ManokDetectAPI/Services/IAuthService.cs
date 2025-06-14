using ManokDetectAPI.Entities;
using ManokDetectAPI.DTO;

namespace ManokDetectAPI.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(userRegisterDto request);
        Task<TokenResponseDto?> LoginAsync (userLoginDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RequestRefreshDto request);
        Task<User> FindOrRegisterGoogleUser(string email, string name);
    }
}
