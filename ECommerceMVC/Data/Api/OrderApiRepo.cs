using ECommerceMVC.Models;
using System.Data.Entity.Core;
using System.Net;
using System.Web.Http;

namespace ECommerceMVC.Data.Api
{
    public class OrderApiRepo
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient order = new HttpClient();

        public OrderApiRepo(IConfiguration configuration)
        {
            _configuration = configuration;
            order.BaseAddress = _configuration.GetValue<Uri>("ApiUri");
        }

        public async Task<List<Order>> GetAllOrders()
        {
            List<Order> orders = null;

            var getOrders = await order.GetAsync("order");
           

            if (getOrders.IsSuccessStatusCode)
            {
                orders = await getOrders.Content.ReadAsAsync<List<Order>>();
            }
            else
            {
                throw new Exception($"Error code: {(int)getOrders.StatusCode}; Message: {getOrders.ReasonPhrase}");
            }

            return orders;
        }

        public async Task<Order> GetOrderById(int id)
        {
            Order orderModel;

            var getOrder = await order.GetAsync($"order/{id}");
            if (getOrder.IsSuccessStatusCode)
            {
                orderModel = await getOrder.Content.ReadAsAsync<Order>();
            }
            else
            {
                throw new Exception($"Error code: {(int)getOrder.StatusCode}; Message: {getOrder.ReasonPhrase}");
            }

            return orderModel;
        }

        public async void CreateOrder(Order ord)
        {
           var createOrder = await order.PostAsJsonAsync<Order>("order", ord);

            if (!createOrder.IsSuccessStatusCode)
            {
                throw new Exception($"Error code: {(int)createOrder.StatusCode}; Message: {createOrder.ReasonPhrase}");
            }
        }

        public async void DeleteOrder(int id)
        {
            var deleteClient = await order.DeleteAsync($"order/{id}");

            if (!deleteClient.IsSuccessStatusCode)
            {
                throw new Exception($"Error code: {(int)deleteClient.StatusCode}; Message: {deleteClient.ReasonPhrase}");

            }
        }

        public async void UpdateOrder(int id, Order ordData)
        {
            var updateClient = await order.PutAsJsonAsync($"order/{id}", ordData);

            if (!updateClient.IsSuccessStatusCode)
            {
                throw new Exception($"Error code: {(int)updateClient.StatusCode}; Message: {updateClient.ReasonPhrase}");
            }

        }   
    }
}
