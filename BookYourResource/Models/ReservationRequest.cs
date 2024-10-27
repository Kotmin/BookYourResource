// namespace MyApp.Models;
public class ReservationRequest
{
    public string UserId { get; set; }
    public string ResourceId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

