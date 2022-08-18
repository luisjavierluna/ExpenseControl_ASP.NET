using System.ComponentModel.DataAnnotations;

namespace ExpenseControl_ASP.NET.Models
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "This field must be a valid Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
