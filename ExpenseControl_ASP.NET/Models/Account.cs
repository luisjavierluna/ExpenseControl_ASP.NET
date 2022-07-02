using ExpenseControl_ASP.NET.Validations;
using System.ComponentModel.DataAnnotations;

namespace ExpenseControl_ASP.NET.Models
{
    public class Account
    {
        public int Id { get; set; }
        [Required]
        [StringLength(maximumLength: 50)]
        [FirstUppercaseLetter]
        public string Name { get; set; }
        [Display(Name = "Account Type")]
        public int AccountTypeId { get; set; }
        public decimal Balance { get; set; }
        [StringLength(maximumLength: 1000)]
        public string Description { get; set; }

    }
}
