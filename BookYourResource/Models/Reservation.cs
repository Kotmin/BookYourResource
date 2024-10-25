public class Reservation
{
    public int Id { get; set; }
    public string Resource { get; set; }
    public Resource ResourceEntity { get; set; }
    public string User { get; set; }
    public User UserEntity { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Status { get; set; }
    public ReservationStatus StatusEntity { get; set; }
}
