namespace ExpenseControl_ASP.NET.Models
{
    public class UpdateTransactionsViewModel: CreateTransactionViewModel
    {
        public int PreviousAccountId { get; set; }
        public decimal PreviousAmount { get; set; }

    }
}
