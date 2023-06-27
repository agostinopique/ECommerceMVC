using ECommerceMVC.Data;
using ECommerceMVC.Data.Api;
using ECommerceMVC.Interface;
using ECommerceMVC.Models;
using ECommerceMVC.Utilities;
using Microsoft.AspNetCore.Mvc;
using PagedList;


namespace ECommerceMVC.Controllers
{
    public class OrderController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepo _orderRepo;
        private readonly IProductRepo _productRepo;
        private readonly IClientRepo _clientRepo;
        private readonly IErrorLogger _errorLogger;
        private bool UseDb { get; set; }
        private readonly OrderApiRepo _orderApiRepo;
        private readonly ClientApiRepo _clientApiRepo;
        private readonly ProductApiRepo _productApiRepo;
        private readonly Utility _utility;


        public OrderController(IOrderRepo orderRepo, IConfiguration configuration, IProductRepo productRepo, IClientRepo clientRepo, OrderApiRepo orderApiRepo, ClientApiRepo clientApiRepo, ProductApiRepo productApiRepo, IErrorLogger errorLogger)
        {
            _configuration = configuration;

            _errorLogger = errorLogger;

            _orderRepo = orderRepo;
            _productRepo = productRepo;
            _clientRepo = clientRepo;

            _orderApiRepo = orderApiRepo;
            _clientApiRepo = clientApiRepo;
            _productApiRepo = productApiRepo;

            UseDb = _configuration.GetValue<bool>("UseDb");
            _utility = new Utility(errorLogger);


        }
        public async Task<ActionResult<IEnumerable<Order>>> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.IdSortParm = sortOrder == "Id" ? "id_desc" : "Id";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            List<Order> orders;

            try
            {

                if (!UseDb)
                {
                    orders = await _orderApiRepo.GetAllOrders();
                }
                else
                {
                    orders = await _orderRepo.GetAllOrders();
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
                    orders = orders.Where(s => s.Client.FullName.Contains(searchString)
                                            || s.IdentificationNumber.ToString().Contains(searchString)
                                            || s.TotalPrice.ToString().Contains(searchString)).ToList();
                }

                switch (sortOrder)
                {
                    case "name_desc":
                        orders = orders.OrderByDescending(c => c.Client.FullName).ToList();
                        break;
                    case "Id":
                        orders = orders.OrderBy(c => c.IdentificationNumber).ToList();
                        break;
                    case "id_desc":
                        orders = orders.OrderByDescending(c => c.IdentificationNumber).ToList();
                        break;
                    case "Price":
                        orders = orders.OrderBy(c => c.TotalPrice).ToList();
                        break;
                    case "price_desc":
                        orders = orders.OrderByDescending(c => c.TotalPrice).ToList();
                        break;
                    default:
                        orders = orders.OrderBy(C => C.Client.FullName).ToList();
                        break;
                }
                int pageSize = 3;
                int pageNumber = (page ?? 1);

                //return View(orders.ToPagedList(pageNumber, pageSize));
                return View(orders);
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }

        }

        public async Task<ActionResult<Order>> Details(int id)
        {
            Order orderModel;
            try
            {
                if (!UseDb)
                {
                    orderModel = await _orderApiRepo.GetOrderById(id);
                    orderModel.Client = await _clientApiRepo.GetClientById(orderModel.ClientId);
                }
                else
                {
                    orderModel = await _orderRepo.GetOrderById(id);

                    if (orderModel == null) { return NotFound(); }

                    orderModel.Products = await _productRepo.GetRelatedProducts(id);
                    orderModel.Client = await _clientRepo.GetClientById(orderModel.ClientId);

                }

                return View(orderModel);
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }

        public async Task<ActionResult<OrderObj>> Create()
        {
            OrderObj orderModel= new OrderObj();
            orderModel.Order = new Order();

            try
            {

                if (!UseDb)
                {
                    orderModel.ClientId = await _clientApiRepo.GetAllClients();
                    orderModel.Products = await _productApiRepo.GetAllProducts();
                }
                else
                {
                    orderModel.ClientId = await _clientRepo.GetAllClients();
                    orderModel.Products = await _productRepo.GetAllProducts();
                }


                return View("Create", orderModel);
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
                    _orderApiRepo.DeleteOrder(id);
                }
                else
                {

                    Order orderFromDb = await _orderRepo.GetOrderById(id);
                    if (orderFromDb == null) return NotFound();

                    _orderRepo.DeleteOrder(orderFromDb);

                    await _productRepo.SaveChanges();
                }
                TempData["deleteOrder"] = true;
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }
        
        public async Task<ActionResult<Order>> Edit(int id)
        {
            OrderObj orderBlueprint = new OrderObj();

            try
            {
                if (!UseDb)
                {
                    orderBlueprint.Order = await _orderApiRepo.GetOrderById(id); ;

                    if (orderBlueprint.Order == null) { return NotFound(); }

                    orderBlueprint.ClientId = await _clientApiRepo.GetAllClients();

                    orderBlueprint.Products = await _productApiRepo.GetAllProducts();

                    foreach (Product prod in orderBlueprint.Order.Products)
                    {
                        orderBlueprint.SelectedProducts.Add(prod.Id);
                    }

                }
                else
                {
                    orderBlueprint.Order = await _orderRepo.GetOrderById(id);

                    if (orderBlueprint.Order == null) { return NotFound(); }

                    orderBlueprint.Products = await _productRepo.GetAllProducts();
                    orderBlueprint.ClientId = await _clientRepo.GetAllClients();

                    foreach (Product prod in orderBlueprint.Order.Products)
                    {
                        orderBlueprint.SelectedProducts.Add(prod.Id);
                    }

                }

                return View("Edit", orderBlueprint);
            }
            catch (Exception e )
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        
        }


        public async Task<ActionResult<OrderObj>> Store(OrderObj clt)
        {
            Order orderToSave = new Order();

            orderToSave.ClientId = clt.Order.ClientId;

            try
            {
                if (!UseDb)
                {
                    foreach (int prodId in clt.SelectedProducts)
                    {
                        orderToSave.Products.Add(await _productApiRepo.GetProductById(prodId));
                    }

                    _orderApiRepo.CreateOrder(orderToSave);

                    TempData["createOrder"] = true;

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (int prodId in clt.SelectedProducts)
                    {
                        orderToSave.Products.Add(await _productRepo.GetProductById(prodId));
                    }

                    await _orderRepo.CreateOrder(orderToSave);

                    await _orderRepo.SaveChanges();

                    TempData["createOrder"] = true;

                    return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        
        }

    
        public async Task<ActionResult<Order>> Update(int id, OrderObj orderData)
        {
            Order orderToUpdate;
            if (ModelState.IsValid)
            {
                return View("Edit", orderData);
            }

            try
            {
                if (!UseDb)
                {
                    orderToUpdate = new Order();

                    orderToUpdate.ClientId = orderData.Order.ClientId;

                    foreach (int prodId in orderData.SelectedProducts)
                    {
                        orderToUpdate.Products.Add(await _productApiRepo.GetProductById(prodId));
                    }

                    _orderApiRepo.UpdateOrder(id, orderToUpdate);

                    return RedirectToAction("details", _orderApiRepo.GetOrderById(id));
                }
                else
                {
                    orderToUpdate = await _orderRepo.GetOrderById(id);

                    orderToUpdate.Products = await _productRepo.GetRelatedProducts(orderToUpdate.Id);
                    orderToUpdate.Products.Clear();

                    if (orderToUpdate != null)
                    {
                        orderToUpdate.ClientId = orderData.Order.ClientId;
                        foreach (int prodId in orderData.SelectedProducts)
                        {
                            orderToUpdate.Products.Add(await _productRepo.GetProductById(prodId));
                        }

                        orderToUpdate.TotalPrice = _orderRepo.CalculatePrice(orderToUpdate.Products);

                        await _orderRepo.SaveChanges();
                    }
                    else
                    {
                        return NotFound();
                    }

                    return RedirectToAction("Details", orderToUpdate);
                }
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }
    }
}
