using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface IAccountsRepository
    {
        Task Create(Account account);
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
    }
}
