namespace ManokDetectAPI.Models
{
    public class userRegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Plaintext, temporary
        public string Address { get; set; }
        public string MobileNumber { get; set; }

    }
}
