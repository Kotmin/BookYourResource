using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public class ReservationService : IReservationService
{
    private readonly ApplicationDbContext _context;

    public ReservationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsResourceAvailable(string resourceId, DateTime startDate, DateTime endDate)
    {
        // Check for overlapping reservations in the database
        return await _context.Reservations
            .Where(r => r.ResourceId == resourceId && r.StatusId == 1) // StatusId 1 - active reservations
            .AllAsync(r => endDate <= r.StartDate || startDate >= r.EndDate);
    }
}
