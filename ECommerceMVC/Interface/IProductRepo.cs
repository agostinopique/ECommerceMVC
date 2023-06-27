using ECommerceMVC.Models;

namespace ECommerceMVC.Interface
{
    public interface IProductRepo
    {
        Task<List<Product>> GetAllProducts();
        Task<Product> GetProductById(int id);
        Task<List<Product>> GetRelatedProducts(int id);
        Task CreateProduct(Product prod);
        Task SaveChanges();
        void DeleteProduct(Product prod);
    }
}
