public class ReservationViewModel
{
    public int Id { get; set; }
    public string ResourceName { get; set; }
    public string UserName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool CanDelete { get; set; }


    public double TotalHours { get; set; }



    public string GetFormattedDuration()
    {
        var duration = EndDate - StartDate;
        var days = (int)duration.TotalDays;
        var hours = duration.Hours;

        return $"{days} day{(days == 1 ? "" : "s")}, {hours} hour{(hours == 1 ? "" : "s")}";
    }

    public double GetTotalHours()
    {
        return (EndDate - StartDate).TotalHours;
    }

}
