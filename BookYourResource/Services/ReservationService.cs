using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;



public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;

    public ReservationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<Reservation> GetActiveReservationsQuery()
    {
        return _context.Reservations
            .Where(r => r.StatusId == 1) // Just active Reservations
            .Include(r => r.UserEntity)
            .Include(r => r.ResourceEntity);
    }

    public IQueryable<Reservation> GetSortedActiveReservationsQuery()
    {
        return _context.Reservations
            .Where(r => r.StatusId == 1)
            .Include(r => r.UserEntity)
            .Include(r => r.ResourceEntity)
            .OrderBy(r => r.ResourceEntity.Name) 
            .ThenBy(r => r.StartDate.Date); 
    }

    public async Task<List<Reservation>> GetActiveReservationsQAsync()
    {
        return await GetActiveReservationsQuery().ToListAsync();
    }

    public async Task<List<Reservation>> GetSortedActiveReservationsQAsync()
    {
        return await GetSortedActiveReservationsQuery().ToListAsync();
    }


    public async Task<List<Reservation>> GetAllReservations()
    {
        return await _context.Reservations
            .Include(r => r.UserEntity)
            .Include(r => r.ResourceEntity)
            .ToListAsync();

    }

    public async Task<List<ReservationDto>> GetActiveReservations()
    {
        return await GetSortedActiveReservationsQuery()
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
    }

    public async Task<List<Reservation>> GetReservationsByResource(int id)
    {
        return await _context.Reservations
            .Where(r => r.ResourceId == id.ToString())
            .Include(r => r.UserEntity)
            .ToListAsync();

    }
    public async Task<Reservation> GetReservationsById(int id)
    {
          return await _context.Reservations
            .Include(r => r.UserEntity)
            .Include(r => r.ResourceEntity)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Reservation>> GetReservationsByResourceId(int id)
    {
        return await _context.Reservations
            .Where(r => r.ResourceId == id.ToString())
            .Include(r => r.UserEntity)
            .ToListAsync();

    }

    public async Task<List<Reservation>> GetReservationsByResourceName(string name)
    {
    return await GetActiveReservationsQuery()
        .Where(r => r.ResourceEntity.Name.Contains(name) || r.ResourceEntity.CodeName.Contains(name))
        .ToListAsync();
    }

    public async Task<List<string>> GetResourceNameSuggestions(string name)
    {
        return await _context.Resources
                .Where(r => EF.Functions.Like(r.CodeName, $"%{name}%"))
                .Select(r => r.CodeName)
                .Distinct()
                .ToListAsync();
    }

    

    
    public async Task<bool> IsResourceAvailable(string resourceId, DateTime startDate, DateTime endDate)
    {
        return await _context.Reservations
            .Where(r => r.ResourceId == resourceId && r.StatusId == 1) // StatusId 1 - active
            .AllAsync(r => endDate <= r.StartDate || startDate >= r.EndDate);
    }

    public bool IsValidHourReservation(DateTime startDate, DateTime endDate)
    {
        var duration = endDate - startDate;
        return duration.TotalMinutes > 0 && duration.TotalMinutes % 60 == 0;
    }

    public bool IsFutureDate(DateTime startDate)
    {
        return startDate >= DateTime.Now;
    }

    public bool IsValidDateRange(DateTime startDate, DateTime endDate)
    {
        return endDate > startDate;
    }

    public async Task<Reservation> CreateReservationAsync(string resourceId, DateTime startDate, DateTime endDate, string userId)
    {
        var reservation = new Reservation
        {
            UserId = userId,
            ResourceId = resourceId,
            StartDate = startDate,
            EndDate = endDate,
            StatusId = 1
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return reservation;
    }

    public async Task<bool> DeleteReservationAsync(int reservationId, string userId)
    {
        var reservation = await _context.Reservations
            .Where(r => r.Id == reservationId && r.UserId == userId)
            .FirstOrDefaultAsync();

        if (reservation == null) return false;

        reservation.StatusId = 2; // StatusId 2 means "deleted" 
        await _context.SaveChangesAsync();

        return true;
    }



    // public async Task<List<Resource>> GetViewFormattedResources(){

    //     return await _context.Resources
    //         .Select(r => new 
    //         {
    //             r.Id,
    //             DisplayName = $"{r.Name} ({r.CodeName}) - {r.Details} [{r.ResourceType.Name}]"
    //         })
    //         .ToListAsync();
    //     }

    public async Task<List<ResourceViewDto>> GetViewFormattedResources()
    {
        return await _context.Resources
            .Select(r => new ResourceViewDto
            {
                Id = r.Id,
                DisplayName = $"{r.Name} ({r.CodeName}) - {r.Details} [{r.ResourceType.Name}]"
            })
            .ToListAsync();
    }

    

    
    
}
