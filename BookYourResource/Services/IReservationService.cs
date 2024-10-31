using System;
using System.Threading.Tasks;

public interface IReservationService
{
    Task<bool> IsResourceAvailable(string resourceId, DateTime startDate, DateTime endDate);
}
