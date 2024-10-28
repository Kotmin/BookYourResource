using System.ComponentModel.DataAnnotations;

public class ReservationRequest
{
    [Required]
    public string ResourceId { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "Start Date and Time")]
    [FutureDate(ErrorMessage = "Start date must be in the future.")]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.DateTime)]
    [Display(Name = "End Date and Time")]
    [FutureDate(ErrorMessage = "End date must be in the future.")]
    public DateTime EndDate { get; set; }

 
}


public class FutureDateAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime dateTime && dateTime <= DateTime.Now)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
