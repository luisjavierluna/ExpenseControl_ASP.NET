using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ExpenseControl_ASP.NET.Models
{
    public class CreateTransactionViewModel: Transaction
    {
        public IEnumerable<SelectListItem> Accounts { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; } = OperationType.Income;
    }
}
