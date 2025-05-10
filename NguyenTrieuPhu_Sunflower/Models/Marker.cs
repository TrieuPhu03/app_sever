using System.ComponentModel.DataAnnotations;

namespace NguyenTrieuPhu_Sunflower.Models
{
    public class Marker
    {
        public int Id { get; set; }
        public string? kinhDo { get; set; }
        public string? viDo { get; set; }
        public string? title { get; set; }
        public string? image { get;set; }
        [Required]
        public string UserId { get; set; }
        public User? User { get; set; }
    }
}
