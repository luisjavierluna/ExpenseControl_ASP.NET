using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface ITransactionsRepository
    {
        Task Create(Transaction transaction);
        Task Update(Transaction transaction, decimal previousAmount, int previousAccount);
    }

    public class TransactionsRepository: ITransactionsRepository
    {
        private readonly string connectionString;

        public TransactionsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Transaction transaction)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(
                "Transactions_Insert",
                new
                {
                    transaction.UserId,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategoryId,
                    transaction.AccountId,
                    transaction.Note
                },
                commandType: System.Data.CommandType.StoredProcedure);

            transaction.Id = id;
        }

        public async Task Update(
            Transaction transaction,
            decimal previousAmount,
            int previousAccount)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                "Transactions_Update",
                new
                {
                    transaction.Id,
                    transaction.TransactionDate,
                    transaction.Amount,
                    transaction.CategoryId,
                    transaction.AccountId,
                    transaction.Note,
                    previousAmount,
                    previousAccount
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
