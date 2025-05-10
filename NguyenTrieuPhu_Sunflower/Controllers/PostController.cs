using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenTrieuPhu_Sunflower.Models;
using NguyenTrieuPhu_Sunflower.Repositories;
using System.Security.Claims;

namespace NguyenTrieuPhu_Sunflower.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;

        public PostController(ApplicationDbContext context, IPostRepository postRepository, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _context = context;
        }

        // GET: api/Post
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            var posts = await _postRepository.GetAllAsync();
            return Ok(posts);
        }

        // GET: api/Post/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        // GET: api/Post/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Post>>> GetPostsByUser(string userId)
        {
            var posts = await _postRepository.GetPostsByUserIdAsync(userId);
            return Ok(posts);
        }

        // POST: api/Post
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] Post post)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("User not authenticated");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                return BadRequest($"User not found with username: {username}");
            }

            post.UserId = user.Id;
            await _postRepository.AddAsync(post);
            return CreatedAtAction(nameof(GetPost), new { id = post.Id }, post);
        }

        // PUT: api/Post/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, Post post)
        {
            if (id != post.Id)
            {
                return BadRequest();
            }

            try
            {
                await _postRepository.UpdateAsync(post);
            }
            catch (Exception)
            {
                if (!await _postRepository.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }

            return NoContent();
        }
        // DELETE: api/Post/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _postRepository.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            await _postRepository.DeleteAsync(id);
            return NoContent();
        }
        [HttpPost("like/{postId}")]
        public async Task<IActionResult> LikePost(int postId)
        {
            var updatedPost = await _postRepository.LikePostAsync(postId);

            if (updatedPost == null)
            {
                return NotFound($"Không tìm thấy bài viết có ID {postId}");
            }

            return Ok(updatedPost);
        }
        [HttpPost("unlike/{postId}")]
        public async Task<IActionResult> UnlikePost(int postId)
        {
            var updatedPost = await _postRepository.UnlikePostAsync(postId);

            if (updatedPost == null)
            {
                return NotFound($"Không tìm thấy bài viết có ID {postId}");
            }

            return Ok(updatedPost);
        }
    }
}