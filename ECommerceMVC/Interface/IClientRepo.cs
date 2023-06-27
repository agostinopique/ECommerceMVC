using ECommerceMVC.Models;

namespace ECommerceMVC.Interface
{
    public interface IClientRepo
    {
        Task<List<Client>> GetAllClients();
        Task<Client> GetClientById(int id);
        Task CreateClient(Client clt);
        Task SaveChanges();
        void DeleteClient(Client clt);
    }
}
