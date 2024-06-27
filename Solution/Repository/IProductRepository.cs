namespace Solution.Repository;

public interface IProductRepository
{
    bool ProductExists(int productId);
    decimal GetProductPrice(int productId);
}