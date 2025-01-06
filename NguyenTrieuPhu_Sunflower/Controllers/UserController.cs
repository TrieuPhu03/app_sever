using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NguyenTrieuPhu_Sunflower.Models;
using NguyenTrieuPhu_Sunflower.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NguyenTrieuPhu_Sunflower.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Xử lý logic mật khẩu (tùy thuộc vào cấu hình của bạn)
            await _userRepository.AddAsync(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/User/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var userExists = await _userRepository.ExistsAsync(id);
            if (!userExists)
            {
                return NotFound();
            }

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var userExists = await _userRepository.ExistsAsync(id);
            if (!userExists)
            {
                return NotFound();
            }

            await _userRepository.DeleteAsync(id);
            return NoContent();
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userName = User.Identity.Name; // Lấy username từ claims trong token
            var user = await _userRepository.GetUserProfileAsync(userName); // Sử dụng repository

            if (user == null)
            {
                return NotFound(new { Status = false, Message = "User not found" });
            }

            var userProfile = new
            {
                user.UserName,
                user.Email,
                user.PhoneNumber,
                user.Initials,
                user.Image,
                user.BirthDay
            };

            return Ok(userProfile);
        }
        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(User user)
        {
            var userName = User.Identity.Name; // Lấy tên người dùng từ token (claims)

            var currentUser = await _userRepository.GetUserProfileAsync(userName);
            if (currentUser == null)
            {
                return NotFound(new { Status = false, Message = "User not found" });
            }

            // Cập nhật thông tin từ request
            currentUser.Email = user.Email ?? currentUser.Email;
            currentUser.PhoneNumber = user.PhoneNumber ?? currentUser.PhoneNumber;
            currentUser.Initials = user.Initials ?? currentUser.Initials;
            currentUser.BirthDay = user.BirthDay ?? currentUser.BirthDay;
            currentUser.Image = user.Image ?? currentUser.Image;

            // Lưu lại thay đổi trong database
            await _userRepository.UpdateAsync(currentUser);

            return Ok(new { Status = true, Message = "Profile updated successfully", User = currentUser });
        }

    }
}
