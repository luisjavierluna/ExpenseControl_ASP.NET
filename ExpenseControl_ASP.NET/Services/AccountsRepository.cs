using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface IAccountsRepository
    {
        Task Create(Account account);
        Task Delete(int id);
        Task<Account> GetById(int id, int userId);
        Task<IEnumerable<Account>> Search(int userId);
        Task Update(CreateAccountViewModel account);
    }

    public class AccountsRepository: IAccountsRepository
    {
        private readonly string connectionString;

        public AccountsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Account account)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                INSERT INTO Accounts(Name, AccountTypeId, Balance, Description)
                VALUES(@Name, @AccountTypeId, @Balance, @Description)
                SELECT SCOPE_IDENTITY();",
                account);
            account.Id = id;
        }

        public async Task<IEnumerable<Account>> Search(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Account>(@"
                SELECT Accounts.Id, Accounts.Name, Balance, acty.Name as AccountType
                FROM Accounts
                INNER JOIN AccountsTypes acty
                ON acty.Id = Accounts.AccountTypeId
                WHERE acty.UserId = @UserId
                ORDER BY acty.Sequence",
                new { userId });
        }

        public async Task<Account> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Account>(@"
                SELECT Accounts.Id, Accounts.Name, Balance, Description, AccountTypeId
                FROM Accounts
                INNER JOIN AccountsTypes acty
                ON acty.Id = Accounts.AccountTypeId
                WHERE acty.UserId = @UserId AND Accounts.Id = @Id",
                new { id, userId });
        }

        public async Task Update(CreateAccountViewModel account)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                UPDATE Accounts
                SET Name = @Name, Balance = @Balance, Description = @Description, AccountTypeId = @AccountTypeId
                WHERE Id = @Id;",
                account);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                "DELETE Accounts WHERE Id = @Id",
                new { id });
        }

    }
}
