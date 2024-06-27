using System.Data.SqlClient;

namespace Solution.Repository;

public class ProductRepository : IProductRepository
{
    private readonly IConfiguration _configuration;

    public ProductRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool ProductExists(int productId)
    {
        using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
        {
            connection.Open();
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Product WHERE IdProduct = @ProductId", connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }

    public decimal GetProductPrice(int productId)
    {
        using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
        {
            connection.Open();
            using (var cmd = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @ProductId", connection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                return (decimal)cmd.ExecuteScalar();
            }
        }
    }
}