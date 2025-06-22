namespace ManokDetectAPI.DTO
{
    public class userDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? MobileNumber { get; set; }
        public string? UserType { get; set; }
        public Guid securityID { get; set; }

    }
}
