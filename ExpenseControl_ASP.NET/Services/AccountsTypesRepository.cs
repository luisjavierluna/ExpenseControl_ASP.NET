using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface IAccountsTypesRepository
    {
        Task Create(AccountType accountType);
        Task Delete(int id);
        Task<bool> Exists(string name, int usuarioId);
        Task<IEnumerable<AccountType>> GetAccountsTypes(int userId);
        Task<AccountType> GetById(int id, int userId);
        Task Update(AccountType accountType);
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

        public async Task<IEnumerable<AccountType>> GetAccountsTypes(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<AccountType>(@"
                SELECT Id, Name, Sequence
                FROM AccountsTypes
                WHERE UserId = @UserId",
                new { userId });
        }

        public async Task<AccountType> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<AccountType>(@"
                SELECT Id, Name, Sequence
                FROM AccountsTypes
                WHERE Id = @Id AND UserId = @UserId",
                new { id, userId});
        }

        public async Task Update(AccountType accountType)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                UPDATE AccountsTypes
                SET Name = @Name
                WHERE Id = @Id",
                accountType);
        }

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                DELETE AccountsTypes
                WHERE Id = @Id",
                new { id });
        }
    }
}
