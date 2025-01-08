using System.ComponentModel.DataAnnotations;

namespace NguyenTrieuPhu_Sunflower.Models
{
    public class ResetPasswordModel
    {
        //[Required(ErrorMessage = "Email là bắt buộc")]
        //[EmailAddress(ErrorMessage = "Email không hợp lệ")]
        //public string Email { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }

        //public string? Token { get; set; }
    }
}
