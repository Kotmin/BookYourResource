using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;



[Authorize]
[Route("reservations")]
public class ReservationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ReservationsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }


    [HttpGet("all")]
    public async Task<IActionResult> GetAllReservations()
    {
        var reservations = await _context.Reservations
            .Include(r => r.UserEntity)
            .Include(r => r.ResourceEntity)
            .ToListAsync();
        return Json(reservations);
    }

    [AllowAnonymous]
    [HttpGet("")]
    public async Task<IActionResult> GetActiveReservations()
    {
        var activeReservations = await GetActiveReservationsQuery().ToListAsync();

        return Json(activeReservations);
    }

    private IQueryable<Reservation> GetActiveReservationsQuery()
    {
        return _context.Reservations
            .Where(r => r.StatusId == 1) // Just active Reservations
            .Include(r => r.UserEntity)
            .Include(r => r.ResourceEntity);
    }




    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReservationsByResource(int id)
    {
        var reservations = await _context.Reservations
            .Where(r => r.ResourceId == id.ToString())
            .Include(r => r.UserEntity)
            .ToListAsync();
        return Json(reservations);
    }
    //  [ ]: Maybe LinQ use here? 
    // Filter resources by CODE name (should be unique)
    [HttpGet("q/{name}")]
    public async Task<IActionResult> GetReservationsByResourceName(string name)
    {
    var reservations = await GetActiveReservationsQuery()
        .Where(r => r.ResourceEntity.CodeName == name)
        .ToListAsync();

        if (!reservations.Any())
        {
            var suggestions = await _context.Resources
                .Where(r => EF.Functions.Like(r.CodeName, $"%{name}%"))
                .Select(r => r.CodeName)
                .Distinct()
                .ToListAsync();

            return Json(new
            {
                Message = "No reservations found for the specified resource name.",
                Suggestions = suggestions
            });
        }

        return Json(reservations);
    }


    [HttpGet("u/me")]
    public async Task<IActionResult> GetUserReservations()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();
        
        var reservations = await GetActiveReservationsQuery()
        .Where(r => r.UserId == user.Id)
        .ToListAsync();

        return Json(reservations);
    }


    [HttpPost("")]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (!IsValidHourReservation(request.StartDate, request.EndDate))
        {
            return BadRequest("Reservations must be in full-hour increments.");
        }

        
        bool isAvailable = await IsReservationAvailable(request.ResourceId, request.StartDate, request.EndDate);

        if (!isAvailable)
            return BadRequest("Resource is not available in the selected time.");

        var reservation = new Reservation
        {
            UserId = user.Id,
            ResourceId = request.ResourceId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            StatusId = 1 
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserReservations), new { id = reservation.Id }, reservation);
    }

    public async Task<bool> IsReservationAvailable(string resourceId, DateTime startDate, DateTime endDate)
    {
        return await _context.Reservations
            .Where(r => r.ResourceId == resourceId && r.StatusId == 1) // StatusId 1 - active
            .AllAsync(r => endDate <= r.StartDate || startDate >= r.EndDate);
    }

    private bool IsValidHourReservation(DateTime startDate, DateTime endDate)
    {
        // Check if Reservation time is a valid value, n*60 minutes, where n=1,2,3...
        var duration = endDate - startDate;
        return duration.TotalMinutes > 0 & duration.TotalMinutes % 60 == 0;
    }


    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var reservation = await _context.Reservations
            .Where(r => r.Id == id && r.UserId == user.Id)
            .FirstOrDefaultAsync();

        if (reservation == null) return NotFound("Reservation not found or you are not authorized.");

        reservation.StatusId = 2;
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

