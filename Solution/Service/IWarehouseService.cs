using Solution.Model;

namespace Solution.Service;

public interface IWarehouseService
{
    int AddProductToWarehouse(ProductRequest request);
}