namespace ExpenseControl_ASP.NET.Models
{
    public class GetTransactionsByAccount
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

    }
}
