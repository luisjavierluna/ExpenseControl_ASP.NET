using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface IAccountsRepository
    {
        Task Create(Account account);
        Task<IEnumerable<Account>> Search(int userId);
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
    }
}
