using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NguyenTrieuPhu_Sunflower.Models
{
    public class User: IdentityUser
    {
        [MaxLength(5)]
        public string? Initials { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }

        public string? Image { get; set; }

        [JsonIgnore]
        public ICollection<Post> Posts { get; set; }

        [JsonIgnore]
        public ICollection<Marker> Markers { get; set; }

    }
}
