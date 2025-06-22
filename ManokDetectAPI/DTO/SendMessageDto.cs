namespace ManokDetectAPI.DTO
{
    public class SendMessageDto
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public string? ImageUrl { get; set; }  
        public string Recipient { get; set; }

    }
}
