namespace NguyenTrieuPhu_Sunflower.Models
{
    public class Marker
    {
        public int Id { get; set; }
        public string? longiTude { get; set; }
        public string? latiTude { get; set; }
        public string? image { get;set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
