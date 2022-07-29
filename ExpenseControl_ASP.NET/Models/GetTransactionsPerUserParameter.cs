namespace ExpenseControl_ASP.NET.Models
{
    public class GetTransactionsPerUserParameter
    {
        public int UserId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

    }
}
