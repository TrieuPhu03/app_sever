using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenTrieuPhu_Sunflower.Models;
using NguyenTrieuPhu_Sunflower.Repositories;
using System.Security.Claims;

namespace NguyenTrieuPhu_Sunflower.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarkerController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMarkerRepository _markerRepository;
        private readonly UserManager<User> _userManager;

        public MarkerController(ApplicationDbContext context, IMarkerRepository markerRepository, IUserRepository userRepository, UserManager<User> userManager)
        {
            _markerRepository = markerRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _context = context;
        }

        // GET: api/Marker
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Marker>>> GetMarkers()
        {
            var markers = await _markerRepository.GetAllAsync();
            return Ok(markers);
        }

        // GET: api/Marker/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Marker>> GetMarker(int id)
        {
            var marker = await _markerRepository.GetAllAsync();
            var foundMarker = marker.FirstOrDefault(m => m.Id == id);
            if (foundMarker == null)
            {
                return NotFound();
            }

            return Ok(foundMarker);
        }

        // POST: api/Marker
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePost([FromBody] Marker marker)
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

            marker.UserId = user.Id;
            await _markerRepository.AddAsync(marker);
            return CreatedAtAction(nameof(GetMarker), new { id = marker.Id }, marker);
        }


        //// PUT: api/Marker/{id}
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateMarker(int id, Marker marker)
        //{
        //    if (id != marker.Id)
        //    {
        //        return BadRequest();
        //    }

        //    if (!await _markerRepository.ExistsAsync(id))
        //    {
        //        return NotFound();
        //    }

        //    // Update logic (you may need to map values)
        //    // _context.Entry(marker).State = EntityState.Modified;

        //    await _markerRepository.AddAsync(marker); // Or use a method for update logic
        //    return NoContent();
        //}

        // DELETE: api/Marker/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarker(int id)
        {
            if (!await _markerRepository.ExistsAsync(id))
            {
                return NotFound();
            }

            await _markerRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
