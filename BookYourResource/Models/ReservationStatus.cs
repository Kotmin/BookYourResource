public class ReservationStatus
{
    public int Id { get; set; }
    public string Name { get; set; }

     public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
