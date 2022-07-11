using System.ComponentModel.DataAnnotations;

namespace ExpenseControl_ASP.NET.Models
{
    public class Category
    {
        public int Id { get; set; }
        [StringLength(maximumLength: 50, ErrorMessage = "This field cannot be longer than {1} characters")]
        public string Name { get; set; }
        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; }
        public int UserId { get; set; }
    }
}
