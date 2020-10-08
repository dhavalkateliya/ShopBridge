using System;
using ShopBridge.Models;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using DataTables.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Data.Entity;
using System.Threading.Tasks;

namespace ShopBridge.Controllers
{
    public class ProductController : Controller
    {

        private ApplicationDbContext _dbContext;

        public ApplicationDbContext DbContext
        {
            get
            {
                return _dbContext ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _dbContext = value;
            }

        }

        public ProductController()
        {

        }

        public ProductController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: Product
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            IQueryable<Product> query = DbContext.Product;
            var totalCount = query.Count();

            // searching and sorting
            query = SearchProducts(requestModel, query);
            var filteredCount = query.Count();

            // Paging
            query = query.Skip(requestModel.Start).Take(requestModel.Length);



            var data = query.Select(product => new
            {
                ID = product.ID,
                ProdcutName = product.ProdcutName,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity
            }).ToList();

            return Json(new DataTablesResponse(requestModel.Draw, data, filteredCount, totalCount), JsonRequestBehavior.AllowGet);

        }


        // GET: Product/Create
        public ActionResult Create()
        {
            var model = new ProductViewModel();
            return View("_CreatePartial", model);
        }

        // POST: Product/Create
        [HttpPost]
        public async Task<ActionResult> Create(ProductViewModel productVM)
        {


            if (!ModelState.IsValid)
                return View("_CreatePartial", productVM);

            Product Product = MaptoModel(productVM);

            DbContext.Product.Add(Product);
            var task = DbContext.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to add the Product");
                return View("_CreatePartial", productVM);
            }

            return Content("success");
        }

        // GET: Product/Edit/5
        public ActionResult Edit(Int32 id)
        {
            var product = DbContext.Product.FirstOrDefault(x => x.ID == id);

            ProductViewModel productViewModel = MapToViewModel(product);

            if (Request.IsAjaxRequest())
                return PartialView("_EditPartial", productViewModel);
            return View(productViewModel);
        }

        // POST: Product/Edit/5
        [HttpPost]
        public async Task<ActionResult> Edit(ProductViewModel productVM)
        {


            if (!ModelState.IsValid)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", productVM);
            }

            Product Product = MaptoModel(productVM);

            DbContext.Product.Attach(Product);
            DbContext.Entry(Product).State = EntityState.Modified;
            var task = DbContext.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to update the Product");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return View(Request.IsAjaxRequest() ? "_EditPartial" : "Edit", productVM);
            }

            if (Request.IsAjaxRequest())
            {
                return Content("success");
            }

            return RedirectToAction("Index");

        }

        public async Task<ActionResult> Details(Int32 id)
        {
            var product = await DbContext.Product.FirstOrDefaultAsync(x => x.ID == id);
            var productVM = MapToViewModel(product);

            if (Request.IsAjaxRequest())
                return PartialView("_detailsPartial", productVM);

            return View(productVM);
        }

        // GET: Product/Delete/5
        public ActionResult Delete(Int32 id)
        {
            var product = DbContext.Product.FirstOrDefault(x => x.ID == id);

            ProductViewModel ProductViewModel = MapToViewModel(product);

            if (Request.IsAjaxRequest())
                return PartialView("_DeletePartial", ProductViewModel);
            return View(ProductViewModel);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> DeleteProduct(Int32 ID)
        {
            var Product = new Product { ID = ID };
            DbContext.Product.Attach(Product);
            DbContext.Product.Remove(Product);

            var task = DbContext.SaveChangesAsync();
            await task;

            if (task.Exception != null)
            {
                ModelState.AddModelError("", "Unable to Delete the Product");
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                ProductViewModel productVM = MapToViewModel(Product);
                return View(Request.IsAjaxRequest() ? "_DeletePartial" : "Delete", productVM);
            }

            if (Request.IsAjaxRequest())
            {
                return Content("success");
            }

            return RedirectToAction("Index");

        }



        private IQueryable<Product> SearchProducts(IDataTablesRequest requestModel, IQueryable<Product> query)
        {

            // Apply filters
            if (requestModel.Search.Value != string.Empty)
            {
                var value = requestModel.Search.Value.Trim();
                query = query.Where(p => p.ProdcutName.Contains(value));
            }


            var filteredCount = query.Count();

            // Sort
            var sortedColumns = requestModel.Columns.GetSortedColumns();
            var orderByString = String.Empty;

            foreach (var column in sortedColumns)
            {
                orderByString += orderByString != String.Empty ? "," : "";
                orderByString += (column.Data) + (column.SortDirection == Column.OrderDirection.Ascendant ? " asc" : " desc");
            }

            query = query.OrderBy(orderByString == string.Empty ? "Price asc" : orderByString);

            return query;

        }


        private ProductViewModel MapToViewModel(Product product)
        {

            ProductViewModel productViewModel = new ProductViewModel()
            {
                ID = product.ID,
                ProdcutName = product.ProdcutName,
                Quantity = product.Quantity,
                Price = product.Price,
                Description = product.Description

            };

            return productViewModel;
        }

        private Product MaptoModel(ProductViewModel productVM)
        {
            Product product = new Product()
            {
                ID = productVM.ID,
                ProdcutName = productVM.ProdcutName,
                Quantity = productVM.Quantity,
                Price = productVM.Price,
                Description = productVM.Description
            };

            return product;
        }
    }
}