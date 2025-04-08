namespace ManokDetectAPI.Entities
{
    public class User
    {
        public int Id { get; set; }
        public Guid securityID { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string PasswordHash { get; set; }
        public string? Address { get; set; }
        public string MobileNumber { get; set; }
        public string? UserType { get; set; }

    }
}
