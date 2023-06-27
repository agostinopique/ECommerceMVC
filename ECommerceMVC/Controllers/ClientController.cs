using ECommerceMVC.Data;
using ECommerceMVC.Data.Api;
using ECommerceMVC.Interface;
using ECommerceMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Identity.Client;
using System.Security.Cryptography.Xml;
using PagedList;
using ECommerceMVC.Utilities;

namespace ECommerceMVC.Controllers
{
    public class ClientController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IClientRepo _clientRepo;
        private readonly IOrderRepo _orderRepo;
        private readonly IErrorLogger _errorLogger;
        private readonly ClientApiRepo _clientApiRepo;
        private readonly Utility _utility;
        private bool UseDb { get; set; }


        public ClientController(IClientRepo clientRepo, IOrderRepo orderRepo, IConfiguration configuration, ClientApiRepo clientApi, IErrorLogger errorLogger)
        {
            _clientRepo = clientRepo;
            _orderRepo = orderRepo;
            _configuration = configuration;
            _clientApiRepo = clientApi;
            UseDb = _configuration.GetValue<bool>("UseDb");
            _errorLogger = errorLogger;
            _utility = new Utility(errorLogger);
        }

        public async Task<ActionResult<List<Client>>> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.MailSortParm = sortOrder == "Mail" ? "mail_desc" : "Mail";

            List<Client> clients;

            try
            {
                if (!UseDb)
                {
                    clients = await _clientApiRepo.GetAllClients();
                }
                else
                {
                    clients = await _clientRepo.GetAllClients();
                }


                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;


                if (!String.IsNullOrEmpty(searchString))
                {
                    clients = clients.Where(s => s.FullName.Contains(searchString)
                                           || s.Email.Contains(searchString)).ToList();
                }

                int pageSize = 3;
                int pageNumber = (page ?? 1);

                //return View(clients.ToPagedList(pageNumber, pageSize));
                return View(clients);
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }

        public async Task<ActionResult<Client>> Details(int id)
        {
            Client clientModel;
            try
            {
                if (!UseDb)
                {
                    clientModel = await _clientApiRepo.GetClientById(id);
                }
                else
                {
                    clientModel = await _clientRepo.GetClientById(id);

                    if (clientModel == null) { return NotFound(); }

                    clientModel.Orders = await _orderRepo.GetRelatedOrders(id);
                }

                return View(clientModel);
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
            
        }

        public async Task<ActionResult<Client>> EditClient(int id)
        {
            Client clientToUpdate;

            try
            {
                if (!UseDb)
                {

                    clientToUpdate = await _clientApiRepo.GetClientById(id);

                }
                else
                {
                    clientToUpdate = await _clientRepo.GetClientById(id);

                    if (clientToUpdate == null) { return NotFound(); }

                    clientToUpdate.Orders = await _orderRepo.GetRelatedOrders(id);

                }

                return View("EditClient", clientToUpdate);
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
           
        }

        public async Task<ActionResult<Client>> Update(int id, Client cltData)
        {
            if (!ModelState.IsValid)
            {
                return View("EditClient", cltData);
            }

            Client clientFromDb;

            try
            {
                if (!UseDb)
                {
                    _clientApiRepo.UpdateClient(id, cltData);
                    return RedirectToAction("Details", cltData);
                }
                else
                {
                    clientFromDb = await _clientRepo.GetClientById(id);
                    if (clientFromDb == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        clientFromDb.FullName = cltData.FullName;
                        clientFromDb.Email = cltData.Email;
                        await _clientRepo.SaveChanges();
                    }
                    return RedirectToAction("Details", clientFromDb);
                }
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
            
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {

                if (!UseDb)
                {
                    _clientApiRepo.DeleteClient(id);

                    TempData["deleteClient"] = true;

                    return RedirectToAction("Index");
                }
                else
                {
                    Client clientFromDb = await _clientRepo.GetClientById(id);
                    if (clientFromDb == null) return NotFound();

                    _clientRepo.DeleteClient(clientFromDb);

                    await _clientRepo.SaveChanges();
                    TempData["deleteClient"] = true;

                    return RedirectToAction("Index");

                }
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }

        public ActionResult<Client> Create()
        {
            Client clientModel = new Client();

            return View("Create", clientModel);
        }


        public async Task<ActionResult<Client>> Store(Client clt)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    if (!UseDb)
                    {
                        _clientApiRepo.CreateClient(clt);
                    }
                    else
                    {
                        await _clientRepo.CreateClient(clt);

                        await _clientRepo.SaveChanges();

                    }
                    TempData["createdClient"] = true;
                    return RedirectToAction("Index");
                }

                return View("Create", clt);
            }
            catch (Exception e)
            {

                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }
    }
}
