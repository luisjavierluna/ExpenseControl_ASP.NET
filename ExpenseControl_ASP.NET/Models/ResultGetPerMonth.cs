namespace ExpenseControl_ASP.NET.Models
{
    public class ResultGetPerMonth
    {
        public int Month { get; set; }
        public DateTime ReferenceDate { get; set; }
        public decimal Amount { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public OperationType OperationTypeId { get; set; }

    }
}
