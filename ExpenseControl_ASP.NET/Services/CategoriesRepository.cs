using Dapper;
using ExpenseControl_ASP.NET.Models;
using Microsoft.Data.SqlClient;

namespace ExpenseControl_ASP.NET.Services
{
    public interface ICategoriesRepository
    {
        Task Create(Category category);
        Task Delete(int id);
        Task<Category> GetById(int id, int userId);
        Task<IEnumerable<Category>> GetCategories(int userId, OperationType operationTypeId);
        Task<IEnumerable<Category>> GetCategories(int userId, PaginationViewModel pagination);
        Task Update(Category category);
    }

    public class CategoriesRepository : ICategoriesRepository
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

        public async Task<IEnumerable<Category>> GetCategories(int userId, PaginationViewModel pagination)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(@$"
                SELECT * 
                FROM Categories WHERE 
                UserId = @UserId
                ORDER BY Name
                OFFSET {pagination.RecordsToAvoid} ROWS FETCH NEXT
                    {pagination.RecordsPerPage} ROWS ONLY",
                new { userId });
        }

        public async Task<IEnumerable<Category>> GetCategories(int userId, OperationType operationTypeId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Category>(
                @"SELECT *
                FROM Categories
                WHERE UserId = @UserId AND OperationTypeId = @OperationTypeId",
                new { userId, operationTypeId });
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

        public async Task Delete(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE Categories WHERE Id = @Id", new { id });
        }
    }
}
