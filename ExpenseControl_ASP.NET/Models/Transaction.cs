using System.ComponentModel.DataAnnotations;

namespace ExpenseControl_ASP.NET.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]
        public DateTime TransactionDate { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        [Display(Name = "Category")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Category is a required field")]
        public int CategoryId { get; set; }
        [StringLength(maximumLength: 1000, ErrorMessage = "The note cannot be longer than {1} characters")]
        public string Note { get; set; }
        [Display(Name = "Account")]
        [Range(1, maximum: int.MaxValue, ErrorMessage = "Account is a required field")]
        public int AccountId { get; set; }
        [Display(Name = "Operation Type")]
        public OperationType OperationTypeId { get; set; } = OperationType.Income;
        public string Account { get; set; }
        public string Category { get; set; }

    }
}
