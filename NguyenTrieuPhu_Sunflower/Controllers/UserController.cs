using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NguyenTrieuPhu_Sunflower.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NguyenTrieuPhu_Sunflower.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: api/User
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { Status = false, Message = "User not found" });
            }

            return Ok(user);
        }

        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user, string password)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return BadRequest(new { Status = false, Errors = result.Errors });
            }

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { Status = false, Message = "User not found" });
            }

            user.Email = updatedUser.Email ?? user.Email;
            user.PhoneNumber = updatedUser.PhoneNumber ?? user.PhoneNumber;
            user.Initials = updatedUser.Initials ?? user.Initials;
            user.Image = updatedUser.Image ?? user.Image;
            user.BirthDay = updatedUser.BirthDay ?? user.BirthDay;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { Status = false, Errors = result.Errors });
            }

            return NoContent();
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound(new { Status = false, Message = "User not found" });
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { Status = false, Errors = result.Errors });
            }

            return NoContent();
        }

        // GET: api/User/me
        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { Status = false, Message = "User not authenticated" });
            }

            var user = await _userManager.FindByNameAsync(userName);
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

        // PUT: api/User/update
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDTO model)
        {
            var userName = User.Identity?.Name;
            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { Status = false, Message = "User not authenticated" });
            }

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return NotFound(new { Status = false, Message = "User not found" });
            }

            // Update user properties
            user.Email = model.Email ?? user.Email;
            user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
            user.Initials = model.Initials ?? user.Initials;
            user.Image = model.Image ?? user.Image;
            user.BirthDay = model.BirthDay ?? user.BirthDay;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(new { Status = false, Errors = result.Errors });
            }

            return Ok(new
            {
                Status = true,
                Message = "Profile updated successfully",
                User = new
                {
                    user.UserName,
                    user.Email,
                    user.PhoneNumber,
                    user.Initials,
                    user.Image,
                    user.BirthDay
                }
            });
        }
    }
}
