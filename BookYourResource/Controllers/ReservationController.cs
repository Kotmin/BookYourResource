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
    [HttpGet("active")]
    public async Task<IActionResult> GetActiveReservations()
    {
        var activeReservations = await GetSortedActiveReservationsQuery()
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                UserName = r.UserEntity.Email, 
                ResourceName = r.ResourceEntity.Name,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                
                TotalHours = (r.EndDate - r.StartDate).TotalHours
            })
            .ToListAsync();

        return Json(activeReservations);
    }


    private IQueryable<Reservation> GetActiveReservationsQuery()
    {
        return _context.Reservations
            .Where(r => r.StatusId == 1) // Just active Reservations
            .Include(r => r.UserEntity)
            .Include(r => r.ResourceEntity);
    }


    private IQueryable<Reservation> GetSortedActiveReservationsQuery()
    {
        return _context.Reservations
            .Where(r => r.StatusId == 1)
            .Include(r => r.UserEntity)
            .Include(r => r.ResourceEntity)
            .OrderBy(r => r.ResourceEntity.Name) 
            .ThenBy(r => r.StartDate.Date); 
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

    [HttpGet("q/{name}")]
    public async Task<IActionResult> GetReservationsByResourceName(string name)
    {
    var reservations = await GetActiveReservationsQuery()
        
        .Where(r => r.ResourceEntity.Name.Contains(name) || r.ResourceEntity.CodeName.Contains(name))
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

         if (!IsFutureDate(request.StartDate))
        {
            return BadRequest("The start date cannot be in the past.");
        }
        
        if (!IsValidDateRange(request.StartDate, request.EndDate))
        {
            return BadRequest("The end date must be after the start date.");
        }


        bool isAvailable = await IsResourceAvailable(request.ResourceId, request.StartDate, request.EndDate);

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

    public async Task<bool> IsResourceAvailable(string resourceId, DateTime startDate, DateTime endDate) 
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

    private bool IsFutureDate(DateTime startDate)
    {
        return startDate >= DateTime.Now;
    }

    private bool IsValidDateRange(DateTime startDate, DateTime endDate)
    {
        return endDate > startDate;
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



    // Views 


    [AllowAnonymous]
    [HttpGet("")]
    public async Task<IActionResult> Index(string query = "")
    {
        var user = await _userManager.GetUserAsync(User);
      
        var activeReservations = await GetSortedActiveReservationsQuery().ToListAsync();;

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
        var resources = await _context.Resources
            .Select(r => new 
            {
                r.Id,
                DisplayName = $"{r.Name} ({r.CodeName}) - {r.Details} [{r.ResourceType.Name}]"
            })
            .ToListAsync();

        ViewBag.Resources = new SelectList(resources, "Id", "DisplayName");

        return View();
    }


    [HttpPost("create")]
    public async Task<IActionResult> CreateReservationV([FromForm] ReservationRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (!IsValidHourReservation(request.StartDate, request.EndDate))
        {
            ModelState.AddModelError("", "Reservations must be in full-hour increments.");
            // Load the resources again when returning to the view
            await LoadResources();
            return View("CreateReservationForm", request);
        }

        var apiResult = await CreateReservation(request);

        if (apiResult is BadRequestObjectResult badRequest)
        {
            ModelState.AddModelError("", badRequest.Value?.ToString());
            // Load the resources again when returning to the view
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
        var resources = await _context.Resources
            .Select(r => new 
            {
                r.Id,
                DisplayName = $"{r.Name} ({r.CodeName}) - {r.Details} [{r.ResourceType.Name}]"
            })
            .ToListAsync();

        ViewBag.Resources = new SelectList(resources, "Id", "DisplayName");
    }


    [HttpGet("v/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var reservation = await _context.Reservations
            .Include(r => r.UserEntity)
            .Include(r => r.ResourceEntity)
            .Where(r => r.Id == id)
            .FirstOrDefaultAsync();

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
            var reservationsAll = await GetActiveReservationsQuery()
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
        var reservations = await GetActiveReservationsQuery()
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
        {
            var suggestions = await _context.Resources
                .Where(r => EF.Functions.Like(r.Name, $"%{name}%") || EF.Functions.Like(r.CodeName, $"%{name}%"))
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

    [AllowAnonymous]
    [HttpGet("grouped/{resourceName?}")]
    public async Task<IActionResult> GetReservationsGroupedByDayAndResource(string? resourceName, bool ownOnly = false)
    {

        var user = await _userManager.GetUserAsync(User);

        
        var query = GetActiveReservationsQuery();

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

