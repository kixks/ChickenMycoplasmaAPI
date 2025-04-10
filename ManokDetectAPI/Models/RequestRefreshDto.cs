namespace ManokDetectAPI.Models
{
    public class RequestRefreshDto
    {
        public Guid securityID { get; set; }
        public required string RefreshToken { get; set; }
    }
}
