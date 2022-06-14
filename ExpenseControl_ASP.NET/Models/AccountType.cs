using System.ComponentModel.DataAnnotations;

namespace ExpenseControl_ASP.NET.Models
{
    public class AccountType
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int UserId { get; set; }
        public int Order { get; set; }
    }
}
