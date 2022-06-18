using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface IAccountsTypesRepository
    {
        Task Create(AccountType accountType);
    }

    public class AccountsTypesRepository : IAccountsTypesRepository
    {
        private readonly string connectionString;

        public AccountsTypesRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(AccountType accountType)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                INSERT INTO AccountsTypes (Name, UserId, Sequence)
                VALUES (@Name, @UserId, 0)
                SELECT SCOPE_IDENTITY();",
                accountType);
            accountType.Id = id;
        }
    }
}
