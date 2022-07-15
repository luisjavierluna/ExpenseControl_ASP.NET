using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExpenseControl_ASP.NET.Models
{
    public class CreateTransactionViewModel
    {
        public IEnumerable<SelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }

    }
}
