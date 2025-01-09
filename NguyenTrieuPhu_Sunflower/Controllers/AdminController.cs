using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using NguyenTrieuPhu_Sunflower.Models;
using NguyenTrieuPhu_Sunflower.Repositories;
using Microsoft.EntityFrameworkCore;

namespace NguyenTrieuPhu_Sunflower.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới gọi được các API này
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IPostRepository _postRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMarkerRepository _markerRepository;

        public AdminController(
            UserManager<User> userManager,
            IPostRepository postRepository,
            ApplicationDbContext context,
            IMarkerRepository markerRepository
        )
        {
            _userManager = userManager;
            _postRepository = postRepository;
            _context = context;
            _markerRepository = markerRepository;
        }

        // ------------------------------------------------------------------
        // Lấy danh sách Users
        // GET: api/Admin/users
        // ------------------------------------------------------------------
        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            // Lấy toàn bộ User từ UserManager
            var users = _userManager.Users.ToList();
            return Ok(users);
        }

        // ------------------------------------------------------------------
        // Lấy danh sách Posts
        // GET: api/Admin/posts
        // ------------------------------------------------------------------
        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPosts()
        {
            var posts = await _context.Posts
                .Include(p => p.User) // Tải thông tin người tạo bài viết
                .ToListAsync();

            // Trả về dữ liệu có chứa thông tin của User
            var result = posts.Select(post => new
            {
                post.Id,
                post.Description,
                post.Image,
                UserName = post.User != null ? post.User.UserName : "Không rõ"
            });

            return Ok(result);
        }
        [HttpGet("markers")]
        public async Task<IActionResult> GetAllMarker()
        {
            var markers = await _context.Markers
                .Include(p => p.User) // Tải thông tin người tạo bài viết
                .ToListAsync();

            // Trả về dữ liệu có chứa thông tin của User
            var result = markers.Select(marker => new
            {
                marker.Id,
                marker.kinhDo,
                marker.viDo,
                marker.title,
                marker.image,
                UserName = marker.User != null ? marker.User.UserName : "Không rõ"
            });

            return Ok(result);
        }

        // ------------------------------------------------------------------
        // Xoá 1 User
        // DELETE: api/Admin/users/{id}
        // ------------------------------------------------------------------
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest("Error occurred while deleting user");
            }

            return NoContent(); // 204
        }

        // ------------------------------------------------------------------
        // Xoá 1 Post
        // DELETE: api/Admin/posts/{id}
        // ------------------------------------------------------------------
        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound("Post not found");
            }

            await _postRepository.DeleteAsync(id);
            return NoContent(); // 204
        }
        [HttpDelete("markers/{id}")]
        public async Task<IActionResult> DeleteMarker(int id)
        {
            var marker = await _markerRepository.GetByIdAsync(id);
            if (marker == null)
            {
                return NotFound("Post not found");
            }

            await _markerRepository.DeleteAsync(id);
            return NoContent(); // 204
        }
    }
}
