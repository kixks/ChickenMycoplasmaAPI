namespace ManokDetectAPI.DTO
{
    public class SnapshotRequestDto
    {
        public int farmerId { get; set; }
        public string snapshot { get; set; }
        public string confidenceScore { get; set; }
    }
}
