namespace ExpenseControl_ASP.NET.Models
{
    public class ResultGetPerWeek
    {
        public int Week { get; set; }
        public decimal Amount { get; set; }
        public OperationType OperationTypeId { get; set; }
        public decimal Incomes { get; set; }
        public decimal Expenses { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

    }
}
