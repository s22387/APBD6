using System.Data.SqlClient;

namespace Solution.Repository;

public class ProductWarehouseRepository : IProductWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public ProductWarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public int InsertProductWarehouse(int productId, int warehouseId, int orderId, int amount, decimal price, DateTime createdAt)
    {
        using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
        {
            connection.Open();
            using (var cmd = new SqlCommand("INSERT INTO Product_Warehouse (IdProduct, IdWarehouse, IdOrder, Amount, Price, CreatedAt) OUTPUT INSERTED.IdProductWarehouse VALUES (@ProductId, @WarehouseId, @OrderId, @Amount, @Price, @CreatedAt)", connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@Price", price);
                cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}