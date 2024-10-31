using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Rendering; //  req SelectList


[Authorize]
[Route("reservations")]
public class ReservationsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly IReservationService _reservationService;

    public ReservationsController(ApplicationDbContext context, UserManager<User> userManager, IReservationService reservationService)
    {
        _context = context;
        _userManager = userManager;
        _reservationService = new ReservationService(context);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllReservations()
    {
        var reservations = await _reservationService.GetAllReservations;
        return Json(reservations);
    }

    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveReservations()
    {
        var activeReservations = await _reservationService.GetActiveReservations();

        return Json(activeReservations);
    }



    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReservationsByResourceId(int id)
    {
        var reservations = await _reservationService.GetReservationsByResource(id);
        return Json(reservations);
    }

    [HttpGet("q/{name}")]
    public async Task<IActionResult> GetReservationsByResourceName(string name)
    {
    var reservations = await _reservationService.GetReservationsByResourceName(name);

        if (!reservations.Any())
        {
            var suggestions =  await _reservationService.GetResourceNameSuggestions(name);

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
        
        var reservations = _reservationService.GetActiveReservationsQuery()
        .Where(r => r.UserId == user.Id)
        .ToListAsync();

        return Json(reservations);
    }

    // [HttpPost("")]
    // public async Task<IActionResult> CreateReservation([FromBody] ReservationRequest request)
    // {
    //     var user = await _userManager.GetUserAsync(User);
    //     if (user == null) return Unauthorized();

    //     if (!IsValidHourReservation(request.StartDate, request.EndDate))
    //         return BadRequest("Reservations must be in full-hour increments.");


    //      if (!IsFutureDate(request.StartDate))
    //         return BadRequest("The start date cannot be in the past.");
        
    //     if (!IsValidDateRange(request.StartDate, request.EndDate))
    //         return BadRequest("The end date must be after the start date.");


    //     // bool isAvailable = await IsResourceAvailable(request.ResourceId, request.StartDate, request.EndDate);
    //     bool isAvailable = await _reservationService.IsResourceAvailable(request.ResourceId, request.StartDate, request.EndDate);

    //     if (!isAvailable)
    //         return BadRequest("Resource is not available in the selected time.");

    //     var reservation = new Reservation
    //     {
    //         UserId = user.Id,
    //         ResourceId = request.ResourceId,
    //         StartDate = request.StartDate,
    //         EndDate = request.EndDate,
    //         StatusId = 1 
    //     };

    //     _context.Reservations.Add(reservation);
    //     await _context.SaveChangesAsync();
    //     return CreatedAtAction(nameof(GetUserReservations), new { id = reservation.Id }, reservation);
    // }

    // public async Task<bool> IsResourceAvailable(string resourceId, DateTime startDate, DateTime endDate) 
    // {
    //     return await _context.Reservations
    //         .Where(r => r.ResourceId == resourceId && r.StatusId == 1) // StatusId 1 - active
    //         .AllAsync(r => endDate <= r.StartDate || startDate >= r.EndDate);
    // }

    // private bool IsValidHourReservation(DateTime startDate, DateTime endDate)
    // {
    //     // Check if Reservation time is a valid value, n*60 minutes, where n=1,2,3...
    //     var duration = endDate - startDate;
    //     return duration.TotalMinutes > 0 & duration.TotalMinutes % 60 == 0;
    // }

    // private bool IsFutureDate(DateTime startDate)
    // {
    //     return startDate >= DateTime.Now;
    // }

    // private bool IsValidDateRange(DateTime startDate, DateTime endDate)
    // {
    //     return endDate > startDate;
    // }

    // [HttpDelete("{id:int}")]
    // public async Task<IActionResult> DeleteReservation(int id)
    // {
    //     var user = await _userManager.GetUserAsync(User);
    //     if (user == null) return Unauthorized();

    //     var reservation = await _context.Reservations
    //         .Where(r => r.Id == id && r.UserId == user.Id)
    //         .FirstOrDefaultAsync();

    //     if (reservation == null) return NotFound("Reservation not found or you are not authorized.");

    //     reservation.StatusId = 2;
    //     await _context.SaveChangesAsync();

    //     return NoContent();
    // }

        [HttpPost("")]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (!_reservationService.IsValidHourReservation(request.StartDate, request.EndDate))
            return BadRequest("Reservations must be in full-hour increments.");

        if (!_reservationService.IsFutureDate(request.StartDate))
            return BadRequest("The start date cannot be in the past.");
        
        if (!_reservationService.IsValidDateRange(request.StartDate, request.EndDate))
            return BadRequest("The end date must be after the start date.");

        bool isAvailable = await _reservationService.IsResourceAvailable(request.ResourceId, request.StartDate, request.EndDate);

        if (!isAvailable)
            return BadRequest("Resource is not available in the selected time.");

        var reservation = await _reservationService.CreateReservationAsync(request.ResourceId, request.StartDate, request.EndDate, user.Id);

        return CreatedAtAction(nameof(GetUserReservations), new { id = reservation.Id }, reservation);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReservation(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        bool success = await _reservationService.DeleteReservationAsync(id, user.Id);

        if (!success) return NotFound("Reservation not found or you are not authorized.");

        return NoContent();
    }

    // Views 
    [AllowAnonymous]
    [HttpGet("")]
    public async Task<IActionResult> Index(string query = "")
    {
        var user = await _userManager.GetUserAsync(User);
      
        var activeReservations = await _reservationService.GetSortedActiveReservationsQuery().ToListAsync();;

        var reservations = activeReservations.Select(r => new ReservationViewModel
        {
            Id = r.Id,
            ResourceName = r.ResourceEntity.Name,
            UserName = r.UserEntity.UserName,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            TotalHours = (r.EndDate - r.StartDate).TotalHours,
            CanDelete = user != null && r.UserId == user.Id
        }).ToList();

        // If query is not empty, filter reservations
        if (!string.IsNullOrWhiteSpace(query))
        {
            reservations = reservations
                .Where(r => r.ResourceName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return View(reservations);
    }

    [HttpGet("create")]
    public async Task<IActionResult> CreateReservationForm()
    {
        var resources = await _reservationService.GetViewFormattedResources();

        ViewBag.Resources = new SelectList(resources, "Id", "DisplayName");

        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateReservationV([FromForm] ReservationRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (!_reservationService.IsValidHourReservation(request.StartDate, request.EndDate))
        {
            ModelState.AddModelError("", "Reservations must be in full-hour increments.");
            await LoadResources();
            return View("CreateReservationForm", request);
        }

        var apiResult = await CreateReservation(request);

        if (apiResult is BadRequestObjectResult badRequest)
        {
            ModelState.AddModelError("", badRequest.Value?.ToString());
            await LoadResources();
            return View("CreateReservationForm", request);
        }
        if (apiResult is UnauthorizedResult)
        {
            return Unauthorized();
        }

        return RedirectToAction("Index");
    }

    private async Task LoadResources()
    {
        var resources = await _reservationService.GetViewFormattedResources();

        ViewBag.Resources = new SelectList(resources, "Id", "DisplayName");
    }

    [HttpGet("v/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        // var reservation = await _context.Reservations
        //     .Include(r => r.UserEntity)
        //     .Include(r => r.ResourceEntity)
        //     .Where(r => r.Id == id)
        //     .FirstOrDefaultAsync();

        var reservation = await _reservationService.GetReservationsById(id);

        if (reservation == null) return NotFound();

        var model = new ReservationViewModel
        {
            Id = reservation.Id,
            ResourceName = reservation.ResourceEntity.Name,
            UserName = reservation.UserEntity.UserName,
            StartDate = reservation.StartDate,
            EndDate = reservation.EndDate,
            TotalHours = (reservation.EndDate - reservation.StartDate).TotalHours,
            CanDelete = reservation.UserId == _userManager.GetUserId(User)
        };

        return View(model);
    }

    [HttpGet("v/q/{name?}")]
    public async Task<IActionResult> GetReservationsByResourceNameV(string? name)
    {

        if (string.IsNullOrEmpty(name))
        {
            var reservationsAll = _reservationService.GetActiveReservationsQuery()
                .Select(r => new ReservationViewModel
                {
                    Id = r.Id,
                    ResourceName = r.ResourceEntity.Name,
                    UserName = r.UserEntity.UserName,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    CanDelete = r.UserId == _userManager.GetUserId(User),
                    TotalHours = (r.EndDate - r.StartDate).TotalHours
                })
                .ToListAsync();
            return Json(reservationsAll);
        }

        // This query retrieves the reservations based on the resource name instead of CodeName
        var reservations = _reservationService.GetActiveReservationsQuery()
            .Where(r => r.ResourceEntity.Name.Contains(name) || r.ResourceEntity.CodeName.Contains(name))
            .Select(r => new ReservationViewModel
            {
                Id = r.Id,
                ResourceName = r.ResourceEntity.Name,
                UserName = r.UserEntity.UserName,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                CanDelete = r.UserId == _userManager.GetUserId(User),
                TotalHours = (r.EndDate - r.StartDate).TotalHours
            })
            .ToListAsync();

        if (!reservations.Any())
        // if (!reservations)
        {
            var suggestions = await _reservationService.GetResourceNameSuggestions(name);

            return Json(new
            {
                Message = "No reservations found for the specified resource name.",
                Suggestions = suggestions
            });
        }

        return Json(reservations);
    }

    [AllowAnonymous]
    [HttpGet("grouped/{resourceName?}")]
    public async Task<IActionResult> GetReservationsGroupedByDayAndResource(string? resourceName, bool ownOnly = false)
    {

        var user = await _userManager.GetUserAsync(User);

        
        var query = _reservationService.GetActiveReservationsQuery();

        if (!string.IsNullOrEmpty(resourceName))
        {
            query = query.Where(r => r.ResourceEntity.Name.Contains(resourceName) || r.ResourceEntity.CodeName.Contains(resourceName));
        }

        if (ownOnly && user != null)
        {
            query = query.Where(r => r.UserId == user.Id);
        }

        var groupedReservations = await query
            .GroupBy(r => new { Day = r.StartDate.Date, ResourceName = r.ResourceEntity.Name })
            .Select(g => new
            {
                Day = g.Key.Day,
                ResourceName = g.Key.ResourceName,
                Reservations = g.Select(r => new
                {
                    Id = r.Id,
                    UserName = r.UserEntity.UserName,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    TotalHours = (r.EndDate - r.StartDate).TotalHours,
                    CanDelete = user != null && r.UserId == user.Id
                }).OrderBy(r => r.StartDate).ToList()
            })
            .OrderBy(g => g.Day)
            .ThenBy(g => g.ResourceName)
            .ToListAsync();

        return Json(groupedReservations);
    }
}

