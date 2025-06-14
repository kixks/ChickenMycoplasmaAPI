namespace ManokDetectAPI.DTO
{
    public class SnapshotResponseDTO
    {
        public int Id { get; set; }
        public string SnapshotUrl { get; set; }
        public string ConfidenceScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
