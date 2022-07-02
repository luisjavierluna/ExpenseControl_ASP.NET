using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseControl_ASP.NET.Models
{
    public class CreateAccountViewModel:Account
    {
        public IEnumerable<SelectListItem> AccountsTypes { get; set; }
    }
}
