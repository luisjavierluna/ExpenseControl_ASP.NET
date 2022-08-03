namespace ExpenseControl_ASP.NET.Models
{
    public class ResultGetPerWeek
    {
        public int Week { get; set; }
        public decimal Amount { get; set; }
        public OperationType OperationTypeId { get; set; }

    }
}
