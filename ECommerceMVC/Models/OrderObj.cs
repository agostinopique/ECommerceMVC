using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerceMVC.Models
{
    public class OrderObj
    {
        public Order Order { get; set; }

        public List<Client> ClientId { get; set; }
        public List<Product> Products { get; set; }

        public List<int> SelectedProducts { get; set; }

        public OrderObj()
        {
            ClientId = new List<Client>();
            Products = new List<Product>();
            SelectedProducts = new List<int>();
        }

    }
}
