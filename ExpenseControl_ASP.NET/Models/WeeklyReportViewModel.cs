namespace ExpenseControl_ASP.NET.Models
{
    public class WeeklyReportViewModel
    {
        public decimal Incomes => TransactionsPerWeek.Sum(x => x.Incomes);
        public decimal Expenses => TransactionsPerWeek.Sum(x => x.Expenses);
        public decimal Total => Incomes - Expenses;
        public DateTime ReferenceDate { get; set; }
        public IEnumerable<ResultGetPerWeek> TransactionsPerWeek { get; set; }

    }
}
