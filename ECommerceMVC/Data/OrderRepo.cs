using ECommerceMVC.Interface;
using ECommerceMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Data
{
    public class OrderRepo : IOrderRepo
    {
        private readonly EcommerceApiContext _context;

        public OrderRepo(EcommerceApiContext context)
        {
            _context = context;
        }
        public async Task CreateOrder(Order ord)
        {
            if (ord == null) throw new ArgumentNullException(nameof(ord));

            List<Product> products = new List<Product>();

            foreach (Product prod in ord.Products)
            {
                products.Add(_context.Products.Find(prod.Id));
            }
            
            ord.Products = products;
            ord.TotalPrice = CalculatePrice(ord.Products);

            await _context.AddAsync(ord);
        }

        public void DeleteOrder(Order ord)
        {
            if(ord == null) { throw new ArgumentNullException(nameof(ord)); }
            _context.Orders.Remove(ord);
        }

        public async Task<List<Order>> GetAllOrders()
        {
           return await _context.Orders.ToListAsync();
        }

        public Task<Order> GetOrderById(int id)
        {
            return _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public double CalculatePrice(List<Product> prods)
        {
            double totalPrice = 0;
            foreach (Product prod in prods)
            {
                totalPrice += prod.Price;
            }

            return Math.Round(totalPrice, 2);
        }

        public async Task<List<Order>> GetRelatedOrders(int id)
        {

            var ordersRelated = await _context.Clients.Include("Orders").FirstAsync(x => x.Id == id);

            List<Order> orders = ordersRelated.Orders;

            return orders;
        }
    }
}
