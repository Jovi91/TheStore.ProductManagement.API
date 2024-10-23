using System.ComponentModel.DataAnnotations;

namespace TheStore.ProductManagement.API.Attributes;

public class MustContainAtLeastOneItemAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        var list = value as IEnumerable<object>;

        if (list == null || !list.GetEnumerator().MoveNext())
        {
            return new ValidationResult(ErrorMessage ?? "The list must contain at least one item.");
        }

        return ValidationResult.Success;
    }
}