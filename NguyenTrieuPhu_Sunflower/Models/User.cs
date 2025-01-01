using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NguyenTrieuPhu_Sunflower.Models
{
    public class User: IdentityUser
    {
        public string? Initials { get; set; }
        //User = IdentityUser + string Inititals
        public DateTime? birthDay { get; set; }
        public string? image { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Marker> Markers { get; set; }

    }
}
