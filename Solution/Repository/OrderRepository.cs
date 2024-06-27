using System.Data.SqlClient;

namespace Solution.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly IConfiguration _configuration;

    public OrderRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public bool OrderExists(int productId, int amount, DateTime createdAt)
    {
        using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
        {
            connection.Open();
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt", connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
    
    public bool OrderCompleted(int productId, int amount, DateTime createdAt)
    {
        using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
        {
            connection.Open();
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Product_Warehouse WHERE IdOrder IN (SELECT IdOrder FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt)", connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }

    public void UpdateOrderFulfilledAt(int productId, int amount, DateTime createdAt)
    {
        using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
        {
            connection.Open();
            using (var cmd = new SqlCommand("UPDATE [Order] SET FulfilledAt = @FulfilledAt WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt", connection))
            {
                cmd.Parameters.AddWithValue("@FulfilledAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public int? GetOrderId(int productId, int amount, DateTime createdAt)
    {
        using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
        {
            connection.Open();
            using (var cmd = new SqlCommand("SELECT IdOrder FROM [Order] WHERE IdProduct = @ProductId AND Amount = @Amount AND CreatedAt < @CreatedAt", connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                cmd.Parameters.AddWithValue("@Amount", amount);
                cmd.Parameters.AddWithValue("@CreatedAt", createdAt);
                var dr = cmd.ExecuteReader();
        
                if (!dr.Read()) return null;

                return (int)dr["IdOrder"];
            }
        }
    }
}