using System.ComponentModel.DataAnnotations;
using TheStore.ProductManagement.API.Models;

namespace TheStore.ProductManagement.API.Attributes;
public class DateRangeAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (validationContext.ObjectInstance is Price price)
        {

            if (price.StartDate >= price.EndDate)
            {
                return new ValidationResult("StartDate must be earlier than EndDate.");
            }
        }
        else
        {

            return ValidationResult.Success;
        }

        return ValidationResult.Success;
    }
}