namespace ExpenseControl_ASP.NET.Models
{
    public class DetailedTransactionsReport
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public IEnumerable<TransactionsByDate> GroupedTransactions { get; set; }
        public decimal DepositsBalance => GroupedTransactions.Sum(x => x.DepositsBalance);
        public decimal WithdrawalsBalance => GroupedTransactions.Sum(x => x.WithdrawalsBalance);
        public decimal Total => DepositsBalance - WithdrawalsBalance;

        public class TransactionsByDate
        {
            public DateTime TransactionDate { get; set; }
            public IEnumerable<Transaction> Transactions { get; set; }
            public decimal DepositsBalance =>
                Transactions.Where(x => x.OperationTypeId == OperationType.Income)
                .Sum(x => x.Amount);
            public decimal WithdrawalsBalance =>
                Transactions.Where(x => x.OperationTypeId == OperationType.Expense)
                .Sum(x => x.Amount);
        }

    }
}
