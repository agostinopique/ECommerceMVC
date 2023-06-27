using ECommerceMVC.Interface;
using ECommerceMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Data.Entity.Core;
using System.Drawing;
using System.Net.Http;

namespace ECommerceMVC.Data.Api
{
    public class ClientApiRepo
    {

        private readonly IConfiguration _configuration;
        private readonly HttpClient client = new HttpClient();

        public ClientApiRepo(IConfiguration configuration)
        { 
            _configuration = configuration;
            client.BaseAddress = _configuration.GetValue<Uri>("ApiUri");
        }

        public async void CreateClient(Client clt)
        {
            var clientToCreate = await client.PostAsJsonAsync<Client>("client", clt);

            if (!clientToCreate.IsSuccessStatusCode)
            {
                throw new Exception($"Error code: {(int)clientToCreate.StatusCode}; Message: {clientToCreate.ReasonPhrase}");
            }
        }

        public async void DeleteClient(int id)
        {
            var deleteClient = await client.DeleteAsync($"client/{id}");

            if (!deleteClient.IsSuccessStatusCode)
            {
                throw new Exception($"Error code: {(int)deleteClient.StatusCode}; Message: {deleteClient.ReasonPhrase}");
            }
        }

        public async Task<List<Client>> GetAllClients()
        {
            List<Client> clients;

            var responseTask = await client.GetAsync("client");

            if (responseTask.IsSuccessStatusCode)
            {

                clients = await responseTask.Content.ReadAsAsync<List<Client>>();

            }
            else
            {
                throw new Exception($"Error code: {(int)responseTask.StatusCode}; Message: {responseTask.ReasonPhrase}");
            }

            return clients;
        }

        public async  Task<Client> GetClientById(int id)
        {
            Client clientModel;
            var getclient = await client.GetAsync($"client/{id}");
           
            if (getclient.IsSuccessStatusCode)
            {
                clientModel = await getclient.Content.ReadAsAsync<Client>();
            }
            else
            {
                throw new Exception($"Error code: {(int)getclient.StatusCode}; Message: {getclient.ReasonPhrase}");
            }

            return clientModel;
        }

        public async void UpdateClient(int id, Client cltData)
        {
            var updateClient = await client.PutAsJsonAsync($"client/{id}", cltData);

            if(!updateClient.IsSuccessStatusCode)
            {
                throw new Exception($"Error code: {(int)updateClient.StatusCode}; Message: {updateClient.ReasonPhrase}");
            }

        }
    }
}
