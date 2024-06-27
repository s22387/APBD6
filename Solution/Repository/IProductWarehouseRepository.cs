namespace Solution.Repository;

public interface IProductWarehouseRepository
{
    int InsertProductWarehouse(int productId, int warehouseId, int orderId, int amount, decimal price, DateTime createdAt);
}