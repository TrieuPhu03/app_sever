using Microsoft.EntityFrameworkCore;
using NguyenTrieuPhu_Sunflower.Models;

namespace NguyenTrieuPhu_Sunflower.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _context;

        public PostRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Post> GetByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Post>> GetAllAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId)
        {
            return await _context.Posts
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(Post post)
        {
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Post>> GetRecentPostsAsync(int count)
        {
            return await _context.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Posts.AnyAsync(post => post.Id == id);
        }

        public async Task<int> GetUserPostCountAsync(string userId)
        {
            return await _context.Posts
                .CountAsync(p => p.UserId == userId);
        }
        public async Task<Post> LikePostAsync(int postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                // Trả về null hoặc throw exception
                return null;
            }

            // Tăng số lượng like
            post.Like++;

            // Cập nhật vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<Post> UnlikePostAsync(int postId)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
            {
                return null;
            }

            // Giảm số lượng like (đảm bảo không âm)
            post.Like = Math.Max(0, post.Like - 1);

            // Cập nhật vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return post;
        }
    }
}
