using ExpenseControl_ASP.NET.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ExpenseControl_ASP.NET.Models
{
    public class AccountType
    {
        public int Id { get; set; }
        [Required]
        [FirstUppercaseLetter]
        [Remote(action: "CheckAccountTypeAlreadyExists", controller:"AccountsTypes")]
        public string Name { get; set; }
        public int UserId { get; set; }
        public int Sequence { get; set; }
    }
}
