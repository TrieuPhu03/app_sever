using System.ComponentModel.DataAnnotations;

namespace NguyenTrieuPhu_Sunflower.Models
{
    public class Post
    {
        public int Id { get; set; }

        public string? Image { get; set; }

        [Required]
        public string Description { get; set; }

        public int Like { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required]
        public string UserId { get; set; }

        public virtual User? User { get; set; }
    }
}
