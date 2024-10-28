public class ReservationViewModel
{
    public int Id { get; set; }
    public string ResourceName { get; set; }
    public string UserName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool CanDelete { get; set; }
}
