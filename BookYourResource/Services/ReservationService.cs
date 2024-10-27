using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;



namespace BookYourResource.Services;


// Not in use!!!@
public class ReservationService
{
    private readonly ApplicationDbContext _context;

    public ReservationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsReservationAvailable(string resourceId, DateTime startDate, DateTime endDate)
    {
        return await _context.Reservations
            .Where(r => r.ResourceId == resourceId && r.StatusId == 1) // StatusId 1 - active
            .AllAsync(r => endDate <= r.StartDate || startDate >= r.EndDate);
    }
}

