using System.ComponentModel.DataAnnotations;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API.Attributes;
public class DateRangeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (validationContext.ObjectInstance is Price price)
        {
            if (DateTime.TryParse(price.StartDate, out DateTime startDate) &&
                DateTime.TryParse(price.EndDate, out DateTime endDate))
            {
                if (startDate >= endDate)
                {
                    return new ValidationResult("The StartDate must be earlier than the EndDate.");
                }
            }
            else
            {
                return new ValidationResult("Invalid date format. Date must be in the format yyyy-MM-dd");
            }
        }
        return ValidationResult.Success;
    }
}