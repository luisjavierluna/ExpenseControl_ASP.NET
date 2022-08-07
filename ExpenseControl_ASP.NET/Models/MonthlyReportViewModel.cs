namespace ExpenseControl_ASP.NET.Models
{
    public class MonthlyReportViewModel
    {
        public IEnumerable<ResultGetPerMonth> TransactionsPerMonth { get; set; }
        public decimal Incomes => TransactionsPerMonth.Sum(x => x.Income);
        public decimal Expenses => TransactionsPerMonth.Sum(x => x.Expense);
        public decimal GreatTotal => Incomes - Expenses;
        public int Year { get; set; }

    }
}
