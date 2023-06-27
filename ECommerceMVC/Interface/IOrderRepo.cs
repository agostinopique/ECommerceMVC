using ECommerceMVC.Models;

namespace ECommerceMVC.Interface
{
    public interface IOrderRepo
    {
        Task<List<Order>> GetAllOrders();
        Task<Order> GetOrderById(int id);
        Task<List<Order>> GetRelatedOrders(int id);
        Task CreateOrder(Order ord);
        Task SaveChanges();
        void DeleteOrder(Order ord);
        double CalculatePrice(List<Product> prods);
    }
}
