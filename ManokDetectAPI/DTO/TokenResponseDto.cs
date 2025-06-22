using Azure.Core;

namespace ManokDetectAPI.DTO
{
    public class TokenResponseDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required userDto User { get; set; }

    }
}
