using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface ICategoriesRepository
    {
        Task Create(Category category);
        Task<IEnumerable<Category>> GetCategories(int userId);
    }

    public class CategoriesRepository: ICategoriesRepository
    {
        private readonly string connectionString;

        public CategoriesRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Create(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"
                INSERT INTO Categories(Name, OperationTypeId, UserId)
                VALUES(@Name, @OperationTypeId, @UserId)
                SELECT SCOPE_IDENTITY();",
                category);
            category.Id = id;
        }

        public async Task<IEnumerable<Category>> GetCategories(int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(
                "SELECT * FROM Categories WHERE UserId = @UserId",
                new { userId });
        }

    }
}
