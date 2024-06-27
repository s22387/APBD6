using System.Data.SqlClient;

namespace Solution.Repository;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;

    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public bool WarehouseExists(int warehouseId)
    {
        using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
        {
            connection.Open();
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Warehouse WHERE IdWarehouse = @WarehouseId", connection))
            {
                cmd.Parameters.AddWithValue("@WarehouseId", warehouseId);
                return (int)cmd.ExecuteScalar() > 0;
            }
        }
    }
}