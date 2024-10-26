public class Reservation
{
    public int Id { get; set; }

    public int StatusId { get; set; } = 1; // FK . 1 should mean "Active" if seeded properly
    public ReservationStatus StatusEntity { get; set; } // Navigation prop

    public string ResourceId { get; set; }
    public Resource ResourceEntity { get; set; }

    public string UserId { get; set; } // FK
    public User UserEntity { get; set; } // Navigation prop

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }


}
