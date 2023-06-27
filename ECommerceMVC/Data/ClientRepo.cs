using ECommerceMVC.Interface;
using ECommerceMVC.Models;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;

namespace ECommerceMVC.Data
{
    public class ClientRepo : IClientRepo
    {

        private readonly EcommerceApiContext _context;

        public ClientRepo(EcommerceApiContext context) 
        { 
            _context = context; 
        }

        public async Task CreateClient(Client clt)
        {
            if (clt == null) { throw new ArgumentNullException(nameof(clt)); }
            await _context.Clients.AddAsync(clt);
        }

        public void DeleteClient(Client clt)
        {
            if (clt == null) { throw new ArgumentNullException(nameof(clt)); }

            _context.Clients.Remove(clt);
        }


        public async Task<List<Client>> GetAllClients()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<Client> GetClientById(int id)
        {
            return await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task SaveChanges()
        {
           await _context.SaveChangesAsync();
        }
    }
}
