using Microsoft.EntityFrameworkCore;
using NguyenTrieuPhu_Sunflower.Models;

namespace NguyenTrieuPhu_Sunflower.Repositories
{
    public class MarkerRepository : IMarkerRepository
    {
        private readonly ApplicationDbContext _context;
        public MarkerRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Marker>> GetAllAsync()
        {
            var markers = await _context.Markers.ToListAsync();

            foreach (var marker in markers)
            {
                // Truy vấn thông tin User bằng UserId nếu cần
                var user = await _context.Users.FindAsync(marker.UserId);
                // Bạn có thể làm gì đó với thông tin user ở đây nếu cần
            }

            return markers;
        }
        public async Task AddAsync(Marker marker)
        {
            await _context.Markers.AddAsync(marker);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var marker = await _context.Markers.FindAsync(id);
            if (marker != null)
            {
                _context.Markers.Remove(marker);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Markers.AnyAsync(marker => marker.Id == id);
        }

        public async Task<Marker> GetByIdAsync(int id)
        {
            return await _context.Markers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
