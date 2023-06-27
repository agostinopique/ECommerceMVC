//using AspNetCore;
using ECommerceMVC.Data;
using ECommerceMVC.Data.Api;
using ECommerceMVC.Interface;
using ECommerceMVC.Models;
using ECommerceMVC.Utilities;
using Microsoft.AspNetCore.Mvc;
using PagedList;

namespace ECommerceMVC.Controllers
{
    public class ProductController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IProductRepo _productRepo;
        private readonly ProductApiRepo _productApiRepo;
        private readonly IErrorLogger _errorLogger;
        private readonly Utility _utility;
        private bool UseDb { get; set; }


        public ProductController(IProductRepo productRepo, IConfiguration configuration, ProductApiRepo productApiRepo, IErrorLogger errorLogger)
        {
            _configuration = configuration;

            _errorLogger = errorLogger;

            _productRepo = productRepo;
            _productApiRepo = productApiRepo;
            UseDb = _configuration.GetValue<bool>("UseDb");
            _utility = new Utility(errorLogger);

        }
        public async Task<ActionResult<IEnumerable<Product>>> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            List<Product> products;

            try
            {

                if (!UseDb)
                {
                    products = await _productApiRepo.GetAllProducts();
                }
                else
                {
                    products = await _productRepo.GetAllProducts();
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
                    products = products.Where(p => p.Name.Contains(searchString)).ToList();
                }


                switch (sortOrder)
                {
                    case "name_desc":
                        products = products.OrderByDescending(c => c.Name).ToList();
                        break;
                    case "Price":
                        products = products.OrderBy(c => c.Price).ToList();
                        break;
                    case "price_desc":
                        products = products.OrderByDescending(c => c.Price).ToList();
                        break;
                    default:
                        products = products.OrderBy(C => C.Name).ToList();
                        break;
                }

                int pageSize = 3;
                int pageNumber = (page ?? 1);

                return View(products.ToPagedList(pageNumber, pageSize));
                //return View(products);
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }

        public async Task<ActionResult<Product>> Details(int id)
        {
            Product productModel;

            try
            {
                if (!UseDb)
                {
                    productModel = await _productApiRepo.GetProductById(id);
                }
                else
                {

                    productModel = await _productRepo.GetProductById(id);

                    if (productModel == null) { return NotFound(); }
                }

                return View(productModel);
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }

        public ActionResult<Product> Create()
        {
            Product productModel = new Product();

            return View("Create", productModel);
        }

        public async Task<ActionResult<Product>> Store(Product prod)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (!UseDb)
                    {
                        _productApiRepo.CreateProduct(prod);
                    }
                    else
                    {
                        await _productRepo.CreateProduct(prod);

                        await _productRepo.SaveChanges();

                    }
                    TempData["createProduct"] = true;
                    return RedirectToAction("Index");
                }

                return View("Create", prod);
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }

        public async Task<ActionResult<Client>> EditProduct(int id)
        {
            Product productToUpdate;

            try
            {
                if (!UseDb)
                {
                    productToUpdate = await _productApiRepo.GetProductById(id);
                }
                else
                {
                    productToUpdate = await _productRepo.GetProductById(id);

                    if (productToUpdate == null) { return NotFound(); }

                }

                return View("EditProduct", productToUpdate);
            }
            catch (Exception e)
            {
                ErrorLog log = await _utility.Logger(e);

                return View("ErrorPage", log);
            }
        }

        public async Task<ActionResult<Product>> Update(int id, Product prodData)
        {
            if (!ModelState.IsValid)
            {
                return View("EditClient", prodData);
            }

            Product productFromDb;
            try
            {

                if (!UseDb)
                {
                    _productApiRepo.UpdateProduct(id, prodData);
                    //productFromDb = await _productApiRepo.GetProductById(id);
                    return RedirectToAction("Details", id);
                }
                else
                {
                    productFromDb = await _productRepo.GetProductById(id);

                    if (productFromDb == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        productFromDb.Name = prodData.Name;
                        productFromDb.Price = prodData.Price;
                        await _productRepo.SaveChanges();
                    }
                    return RedirectToAction("Details", productFromDb);
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
                    _productApiRepo.DeleteProduct(id);

                    TempData["deleteProduct"] = true;

                    return RedirectToAction("Index");
                }
                else
                {
                    Product productFromDb = await _productRepo.GetProductById(id);
                    if (productFromDb == null) return NotFound();

                    _productRepo.DeleteProduct(productFromDb);

                    await _productRepo.SaveChanges();

                    TempData["deleteProduct"] = true;

                    return RedirectToAction("Index");
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
