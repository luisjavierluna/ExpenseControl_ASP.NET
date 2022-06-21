using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface IAccountsTypesRepository
    {
        Task Create(AccountType accountType);
        Task<bool> Exists(string name, int usuarioId);
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

        public async Task<bool> Exists(string name, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            var exists = await connection.QueryFirstOrDefaultAsync<int>(@"
                SELECT 1
                FROM AccountsTypes
                WHERE Name = @Name AND UserId = @UserId",
                new {name, userId });
            return exists == 1;
        }
    }
}
