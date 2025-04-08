using ManokDetectAPI.Entities;
using ManokDetectAPI.Models;

namespace ManokDetectAPI.Services
{
    public interface IAuthService
    {
        Task<User?> RegisterAsync(userRegisterDto request);
        Task<string?> LoginAsync (userLoginDto request);

    }
}
