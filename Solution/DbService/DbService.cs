using System.Data.SqlClient;
using Solution.DbService.Interfaces;
using Solution.DTO;

namespace Solution.DbService
{
    public sealed class DbService : IDbService
    {
        private readonly string _connectionString;

        public DbService(IConfiguration configuration)
        {
            _connectionString = configuration["DefaultConnection"] ?? throw new ArgumentException("Check connection string");
        }

        public async Task<int> CompleteOrder(CreateProductDto dto, int idOrder)
        {
            using var sqlConnection = new SqlConnection(_connectionString);

            await sqlConnection.OpenAsync();

            var updateSqlCommand = new SqlCommand
            {
                CommandText = "UPDATE [Order] SET FulfilledAt = @date WHERE IdOrder = @idOrder",
                Connection = sqlConnection
            };

            var actualDate = DateTime.Now;

            updateSqlCommand.Parameters.AddWithValue("@date", actualDate);
            updateSqlCommand.Parameters.AddWithValue("@idOrder", idOrder);

            await updateSqlCommand.ExecuteNonQueryAsync();

            var insertSqlCommand = new SqlCommand
            {
                CommandText = "INSERT INTO Product_Warehouse VALUES(@idWarehouse, @idProduct, @idOrder, @amount, (SELECT price FROM Product WHERE idProduct = @idProduct) * @amount, @date); SELECT SCOPE_IDENTITY()",
                Connection = sqlConnection
            };

            actualDate = DateTime.Now;

            insertSqlCommand.Parameters.AddWithValue("@idWarehouse", dto.IdWarehouse);
            insertSqlCommand.Parameters.AddWithValue("@idProduct", dto.IdProduct);
            insertSqlCommand.Parameters.AddWithValue("@idOrder", idOrder);
            insertSqlCommand.Parameters.AddWithValue("@amount", dto.Amount);
            insertSqlCommand.Parameters.AddWithValue("@date", actualDate);

            var result = await insertSqlCommand.ExecuteScalarAsync();

            if (result is decimal decimalNumber)
            {
                return Convert.ToInt32(decimalNumber);
            }

            return (int)result!;
        }

        public async Task<int?> GetOrderId(int idProduct, int amount, DateTime date)
        {
            using var sqlConnection = new SqlConnection(_connectionString);

            await sqlConnection.OpenAsync();

            var selectCom = new SqlCommand()
            {
                Connection = sqlConnection,
                CommandText = "SELECT idOrder FROM [Order] WHERE IdProduct = @idProduct AND Amount = @amount AND CreatedAt < @date"
            };

            selectCom.Parameters.AddWithValue("@idProduct", idProduct);
            selectCom.Parameters.AddWithValue("@amount", amount);
            selectCom.Parameters.AddWithValue("@date", date);

            var result = await selectCom.ExecuteScalarAsync();

            if (result is null)
                return null;

            return (int)result;
        }

        public async Task<bool> OrderIsCompleted(int idOrder)
        {
            using var sqlConnection = new SqlConnection(_connectionString);

            await sqlConnection.OpenAsync();

            var selectCom = new SqlCommand()
            {
                Connection = sqlConnection,
                CommandText = "SELECT COUNT(1) FROM Product_Warehouse WHERE IdOrder = @idOrder"
            };

            selectCom.Parameters.AddWithValue("@idOrder", idOrder);

            var result = await selectCom.ExecuteScalarAsync();

            if (result is null)
                return false;

            return (int)result > 0;
        }

        public async Task<bool> ProductExists(int id)
        {
            using var sqlConnection = new SqlConnection(_connectionString);

            await sqlConnection.OpenAsync();

            var selectCom = new SqlCommand()
            {
                Connection = sqlConnection,
                CommandText = "SELECT COUNT(1) FROM Product WHERE IdProduct = @idProduct"
            };

            selectCom.Parameters.AddWithValue("@idProduct", id);

            var result = await selectCom.ExecuteScalarAsync();

            if (result is null)
                return false;

            return (int)result > 0;
        }

        public async Task<bool> WarehouseExists(int id)
        {
            using var sqlConnection = new SqlConnection(_connectionString);

            await sqlConnection.OpenAsync();

            var selectCom = new SqlCommand()
            {
                Connection = sqlConnection,
                CommandText = "SELECT COUNT(1) FROM Warehouse WHERE IdWarehouse = @idWarehouse"
            };

            selectCom.Parameters.AddWithValue("@idWarehouse", id);

            var result = await selectCom.ExecuteScalarAsync();

            if (result is null)
                return false;

            return (int)result > 0;
        }
    }
}