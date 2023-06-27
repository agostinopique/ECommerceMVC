using ECommerceMVC.Models;
using System.Data.Entity.Core;

namespace ECommerceMVC.Data.Api
{
    public class ProductApiRepo
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient product = new HttpClient();

        public ProductApiRepo(IConfiguration configuration)
        {
            _configuration = configuration;
            product.BaseAddress = _configuration.GetValue<Uri>("ApiUri");
        }
        public async Task<List<Product>> GetAllProducts()
        {
            List<Product> products;
            var getProducts = await product.GetAsync("product");

            if (getProducts.IsSuccessStatusCode)
            {

                products = await getProducts.Content.ReadAsAsync<List<Product>>();
            }
            else
            {
                throw new Exception($"Error code: {(int)getProducts.StatusCode}; Message: {getProducts.ReasonPhrase}");
            }

            return products;
        }
        
      
        public async Task<Product> GetProductById(int id)
        {
            Product productModel;
            var getProduct = await product.GetAsync($"product/{id}");

            if (getProduct.IsSuccessStatusCode)
            {
                productModel = await getProduct.Content.ReadAsAsync<Product>();
            }
            else
            {
                throw new Exception($"Error code: {(int)getProduct.StatusCode}; Message: {getProduct.ReasonPhrase}");
            }
            return productModel;
        }

        public async void CreateProduct(Product prod)
        {
            var createProduct = await product.PostAsJsonAsync<Product>("product", prod);

            if (!createProduct.IsSuccessStatusCode)
            {
                throw new Exception($"Error code: {(int)createProduct.StatusCode}; Message: {createProduct.ReasonPhrase}");
            }
        }

        public async void UpdateProduct(int id, Product prodData)
        {
            var updateProduct = await product.PutAsJsonAsync($"product/{id}", prodData);

            if (!updateProduct.IsSuccessStatusCode)
            {
                throw new Exception($"Error code: {(int)updateProduct.StatusCode}; Message: {updateProduct.ReasonPhrase}");
            }
        }

        public async void DeleteProduct(int id)
        {
            var deleteProduct = await product.DeleteAsync($"product/{id}");

            if (!deleteProduct.IsSuccessStatusCode)
            {
                throw new Exception($"Error code: {(int)deleteProduct.StatusCode}; Message: {deleteProduct.ReasonPhrase}");
            }
        }
    }
}
