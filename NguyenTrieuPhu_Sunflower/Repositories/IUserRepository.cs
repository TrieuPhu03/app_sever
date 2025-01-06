using NguyenTrieuPhu_Sunflower.Models;

namespace NguyenTrieuPhu_Sunflower.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string id); // Sử dụng string vì id của User là string
        Task<IEnumerable<User>> GetAllAsync();
        Task<IEnumerable<User>> GetUsersByUserNameAsync(string userName);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id); // Sử dụng string vì id của User là string
        Task<bool> ExistsAsync(string id); // Kiểm tra sự tồn tại của User theo id
        Task<User> GetUserProfileAsync(string userName);
    }
}
