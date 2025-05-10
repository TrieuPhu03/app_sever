using NguyenTrieuPhu_Sunflower.Models;

namespace NguyenTrieuPhu_Sunflower.Repositories
{
    public interface IMarkerRepository
    {
        Task<IEnumerable<Marker>> GetAllAsync();
        Task<Marker> GetByIdAsync(int id);
        Task AddAsync(Marker marker);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
