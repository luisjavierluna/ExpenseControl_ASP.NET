using System.ComponentModel.DataAnnotations;

namespace ExpenseControl_ASP.NET.Validations
{
    public class FirstUppercaseLetterAttribute:ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return ValidationResult.Success;
            }

            var firstLetter = value.ToString()[0].ToString();

            if (firstLetter != firstLetter.ToUpper())
            {
                return new ValidationResult("The first letter of this field must be uppercase");
            }

            return ValidationResult.Success;
        }
    }
}
