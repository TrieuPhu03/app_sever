namespace NguyenTrieuPhu_Sunflower.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string? Like { get; set; }
        public string? Comment { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
