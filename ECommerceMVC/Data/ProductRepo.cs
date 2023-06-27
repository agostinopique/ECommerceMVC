using ECommerceMVC.Interface;
using ECommerceMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Data
{
    public class ProductRepo : IProductRepo
    {
        private readonly EcommerceApiContext _context;

        public ProductRepo(EcommerceApiContext context)
        {
            _context = context;
        }
        public async Task CreateProduct(Product prod)
        {
            if(prod == null) { throw new ArgumentNullException(nameof(prod)); } 

            await _context.Products.AddAsync(prod);
        }

        public void DeleteProduct(Product prod)
        {
            if(prod == null) throw new ArgumentNullException(nameof(prod));
            _context.Products.Remove(prod);
        }

        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> GetProductById(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<List<Product>> GetRelatedProducts(int id)
        {

            var productsRelated = await _context.Orders.Include("Products").FirstAsync(x => x.Id == id);

            List<Product> products = productsRelated.Products;

            return products;
        }
    }
}
