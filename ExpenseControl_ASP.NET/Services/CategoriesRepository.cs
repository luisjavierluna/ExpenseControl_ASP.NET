using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface ICategoriesRepository
    {
        Task Create(Category category);
        Task<Category> GetById(int id, int userId);
        Task<IEnumerable<Category>> GetCategories(int userId);
        Task Update(Category category);
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

        public async Task<Category> GetById(int id, int userId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Category>(@"
                SELECT *
                FROM Categories
                WHERE Id = @Id AND UserId = @UserId",
                new { id, userId });
        }

        public async Task Update(Category category)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"
                UPDATE Categories
                SET Name = @Name, OperationTypeId = @OperationTypeId
                WHERE Id = @Id",
                category);
        }


    }
}
