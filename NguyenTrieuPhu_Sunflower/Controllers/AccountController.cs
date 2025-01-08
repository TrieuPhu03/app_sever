using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NguyenTrieuPhu_Sunflower.Models;
using NguyenTrieuPhu_Sunflower.Repositories;
using NguyenTrieuPhu_Sunflower.Service;

namespace NguyenTrieuPhu_Sunflower.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailSender;
        private readonly IUserRepository _userRepository;
        public AccountController(UserManager<User> userManager, IEmailService emailSender, IUserRepository userRepository)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Không tiết lộ rằng người dùng không tồn tại
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account",
                new { email = email, token = token }, protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(email, "Reset Password",
                $"Vui lòng reset mật khẩu bằng cách <a href='{callbackUrl}'>clicking here</a>");

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromQuery] string email)
        {
            Console.WriteLine($"Token: {token}");
            Console.WriteLine($"Email: {email}");
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return View("Error", "Invalid password reset token or email.");
            }

            // Lấy thông tin user từ email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return View("Error", "User not found.");
            }

            // Verify token
            var isTokenValid = await _userManager.VerifyUserTokenAsync(
                user,
                _userManager.Options.Tokens.PasswordResetTokenProvider,
                "ResetPassword",
                token);

            if (!isTokenValid)
            {
                return View("Error", "Invalid or expired token.");
            }

            // Nếu token hợp lệ, hiển thị form reset password
            //var model = new ResetPasswordModel
            //{
            //    Token = token,
            //    Email = email
            //};

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model, [FromQuery] string token, [FromQuery] string email)
        {

            Console.WriteLine($"Token: {token}");
            Console.WriteLine($"Email: {email}");
            if (!ModelState.IsValid)
            {
                ViewBag.email = email;
                ViewBag.password = model.NewPassword;
                ViewBag.CFpassword = model.ConfirmPassword;
                return View(model); // Trả về view cùng với lỗi validation
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm thấy người dùng.");
                return View(model);
            }
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            // Thành công, chuyển hướng đến trang thông báo
            return RedirectToAction("ResetPasswordConfirmation", "Account");
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
