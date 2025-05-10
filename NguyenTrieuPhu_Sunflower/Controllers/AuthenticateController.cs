using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NguyenTrieuPhu_Sunflower.Models;
using NguyenTrieuPhu_Sunflower.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NguyenTrieuPhu_Sunflower.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthenticateController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
             IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userExists = await _userManager.FindByNameAsync(model.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new { Status = false, Message = "User already exists" });

            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Initials = model.Initials
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { Status = false, Message = "User creation failed" });

            // Assign Role if Provided
            if (!string.IsNullOrEmpty(model.Role))
            {
                if (!await _roleManager.RoleExistsAsync(model.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(model.Role));
                }
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            return Ok(new { Status = true, Message = "User created successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByNameAsync(model.Username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized(new { Status = false, Message = "Invalid username or password" });

            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, user.UserName), // Lưu username
            new Claim(ClaimTypes.NameIdentifier, user.Id), // Lưu id của user
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Lưu mã định danh token
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = GenerateToken(authClaims);
            return Ok(new { Status = true, Message = "Logged in successfully", Token = token });
        }


        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JWTKey");
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(Convert.ToDouble(jwtSettings["TokenExpiryTimeInHour"])),
                Issuer = jwtSettings["ValidIssuer"],
                Audience = jwtSettings["ValidAudience"],
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Lấy thông tin người dùng hiện tại từ Claims
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
                return Unauthorized(new { Status = false, Message = "User not authenticated" });

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return NotFound(new { Status = false, Message = "User not found" });

            // Thay đổi mật khẩu
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { Status = false, Errors = errors });
            }

            return Ok(new { Status = true, Message = "Password changed successfully" });
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Ok("Nếu email tồn tại, chúng tôi đã gửi liên kết đặt lại mật khẩu.");
            }
            //var ResetClaims = new List<Claim>
            //{
            //    new Claim(ClaimTypes.Name, user.UserName),
            //    new Claim(ClaimTypes.Email, user.Email),
            //    new Claim(ClaimValueTypes.Integer,user.Id),
            //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            //};


            //var resetToken = GenerateToken(ResetClaims);
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Tạo deep link thay vì URL web
            var resetLink = $"https://foundtanstone42.conveyor.cloud/Account/ResetPassword?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(user.Email)}";

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Đặt lại mật khẩu", $"Nhấp vào liên kết này để đặt lại mật khẩu: {resetLink}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Không thể gửi email. Vui lòng thử lại sau.");
            }

            return Ok("Nếu email tồn tại, chúng tôi đã gửi liên kết đặt lại mật khẩu.");
        }

        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel request)
        //{
        //    var user = await _userManager.FindByEmailAsync(request.Email);
        //    if (user == null)
        //        return BadRequest("Email không hợp lệ.");

        //    // Decode the token if it's URL-encoded
        //    var decodedToken = Uri.UnescapeDataString(request.Token);

        //    var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
        //    if (!result.Succeeded)
        //    {
        //        return BadRequest(result.Errors);
        //    }

        //    return Ok("Mật khẩu đã được đặt lại.");
        //}
    }
}
