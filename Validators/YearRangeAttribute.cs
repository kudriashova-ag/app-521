using System.ComponentModel.DataAnnotations;

namespace myApp.Validators;

public class YearRangeAttribute : ValidationAttribute
{
    private readonly int _min;
    private readonly int _max;

    public YearRangeAttribute(int min)
    {
        _min = min;
        _max = DateTime.Now.Year;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not int year) return new ValidationResult("Year must be an integer.");
        if (year < _min || year > _max) return new ValidationResult($"Year must be between {_min} and {_max}.");
        return ValidationResult.Success;
    }
}

