using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


public interface IReservationService
{
    IQueryable<Reservation> GetActiveReservationsQuery();
    IQueryable<Reservation> GetSortedActiveReservationsQuery();

    Task<List<Reservation>> GetActiveReservationsQAsync();
    Task<List<Reservation>> GetSortedActiveReservationsQAsync();


    Task<List<Reservation>> GetAllReservations();
    Task<List<ReservationDto>> GetActiveReservations();
    Task<List<Reservation>> GetReservationsByResource(int id);
    Task<Reservation> GetReservationsById(int id);
    Task<List<Reservation>> GetReservationsByResourceName(string name);

    Task<List<string>> GetResourceNameSuggestions(string name);

    Task<bool> IsResourceAvailable(string resourceId, DateTime startDate, DateTime endDate);

    bool IsValidHourReservation(DateTime startDate, DateTime endDate);
    bool IsFutureDate(DateTime startDate);
    bool IsValidDateRange(DateTime startDate, DateTime endDate);

    Task<Reservation> CreateReservationAsync(string resourceId, DateTime startDate, DateTime endDate, string userId);
    Task<bool> DeleteReservationAsync(int reservationId, string userId);

    Task<List<ResourceViewDto>> GetViewFormattedResources();

}
