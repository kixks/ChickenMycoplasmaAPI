using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ManokDetectAPI.Entities
{
    public class Snapshot
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int farmerId { get; set; }
        [ForeignKey("farmerId")]
        public User Farmer { get; set; }
        public string? snapshot { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
