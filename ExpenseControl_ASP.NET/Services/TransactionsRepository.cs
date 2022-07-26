using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface ITransactionsRepository
    {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<Transaction> GetById(int id, int userId);
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
            int previousAccountId)
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
                    previousAccountId
                },
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaction> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaction>(@"
                SELECT Transactions.*, cat.OperationTypeId
                FROM Transactions
                INNER JOIN Categories cat
                ON cat.Id = Transactions.CategoryId
                WHERE Transactions.Id = @Id AND Transactions.UserId = @UserId",
                new { id, userId });
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                "Transactions_Delete",
                new { id },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}
