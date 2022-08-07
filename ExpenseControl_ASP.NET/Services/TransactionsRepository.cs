using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface ITransactionsRepository
    {
        Task Create(Transaction transaction);
        Task Delete(int id);
        Task<IEnumerable<Transaction>> GetByAccountId(GetTransactionsByAccount model);
        Task<Transaction> GetById(int id, int userId);
        Task<IEnumerable<Transaction>> GetByUserId(GetTransactionsPerUserParameter model);
        Task<IEnumerable<ResultGetPerMonth>> GetPerMonth(int userId, int year);
        Task<IEnumerable<ResultGetPerWeek>> GetPerWeek(GetTransactionsPerUserParameter model);
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

        public async Task<IEnumerable<Transaction>> GetByAccountId(
            GetTransactionsByAccount model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>(@"
                SELECT t.Id, t.Amount, t.TransactionDate, c.Name as Category,
                acc.Name as Account, c.OperationTypeId
                FROM Transactions t
                INNER JOIN Categories c
                ON c.Id = t.CategoryId
                INNER JOIN Accounts acc
                ON acc.Id = t.AccountId
                WHERE t.AccountId = @AccountId AND t.UserId = @UserId
                AND TransactionDate BETWEEN @DateStart AND @DateEnd",
                model);
        }

        public async Task<IEnumerable<Transaction>> GetByUserId(
            GetTransactionsPerUserParameter model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaction>(@"
                SELECT t.Id, t.Amount, t.TransactionDate, c.Name as Category,
                acc.Name as Account, c.OperationTypeId
                FROM Transactions t
                INNER JOIN Categories c
                ON c.Id = t.CategoryId
                INNER JOIN Accounts acc
                ON acc.Id = t.AccountId
                WHERE t.UserId = @UserId
                AND TransactionDate BETWEEN @DateStart AND @DateEnd
                ORDER BY t.TransactionDate DESC",
                model);
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

        public async Task<IEnumerable<ResultGetPerWeek>> GetPerWeek(
            GetTransactionsPerUserParameter model)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultGetPerWeek>(@"
                SELECT DATEDIFF(d, @DateStart, TransactionDate) / 7 + 1 as Week,
                SUM(Amount) as Amount, cat.OperationTypeId
                FROM Transactions
                INNER JOIN Categories cat
                ON cat.Id = Transactions.CategoryId
                WHERE Transactions.UserId = @UserId AND
                TransactionDate BETWEEN @DateStart AND @DateEnd
                GROUP BY DATEDIFF(d, @DateStart, TransactionDate) / 7, cat.OperationTypeId",
                model);
        }

        public async Task<IEnumerable<ResultGetPerMonth>> GetPerMonth(int userId, int year)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<ResultGetPerMonth>(@"
                SELECT MONTH(TransactionDate) as Month,
                SUM(Amount) as Amount, cat.OperationTypeId
                FROM Transactions
                INNER JOIN Categories cat
                ON cat.Id = Transactions.CategoryId
                WHERE Transactions.UserId = @UserId AND YEAR(TransactionDate) = @Year
                GROUP BY MONTH(TransactionDate), cat.OperationTypeId",
                new { userId, year });
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
