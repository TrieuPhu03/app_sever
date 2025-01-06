using NguyenTrieuPhu_Sunflower.Models;

namespace NguyenTrieuPhu_Sunflower.Repositories
{
    public interface IPostRepository
    {
        Task<Post> GetByIdAsync(int id);
        Task<IEnumerable<Post>> GetAllAsync();
        Task<IEnumerable<Post>> GetPostsByUserIdAsync(string userId);
        Task AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<Post> LikePostAsync(int postId);
        Task<Post> UnlikePostAsync(int postId);
    }
}
