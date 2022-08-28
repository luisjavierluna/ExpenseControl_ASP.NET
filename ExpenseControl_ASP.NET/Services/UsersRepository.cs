using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface IUsersRepository
    {
        Task<int> CreateUser(User user);
        Task<User> SearchUserByEmail(string normalizedEmail);
    }

    public class UsersRepository: IUsersRepository
    {
        private readonly string connectionString;

        public UsersRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<int> CreateUser(User user)
        {
            using var connection = new SqlConnection(connectionString);
            var userId = await connection.QuerySingleAsync<int>(@"
                INSERT INTO Users(Email, NormalizedEmail, PasswordHash)
                VALUES (@Email, @NormalizedEmail, @PasswordHash)
                SELECT SCOPE_IDENTITY();",
                user);

            await connection.ExecuteAsync("CreateNewUserData", new { userId },
                commandType: System.Data.CommandType.StoredProcedure);

            return userId;
        }

        public async Task<User> SearchUserByEmail(string normalizedEmail)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE NormalizedEmail = @NormalizedEmail",
                new { normalizedEmail });
        }
    }

}
