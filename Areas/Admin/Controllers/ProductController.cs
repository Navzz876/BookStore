
using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        protected readonly IProductRepository _productDb;
        protected readonly ICategoryRepository _categoryDb;
        public ProductController(IProductRepository productDb,ICategoryRepository categoryDb)
        {
            _productDb = productDb;
            _categoryDb = categoryDb;
        }
        public IActionResult Index()
        {
            var categoryList = _productDb.GetAll();
            return View(categoryList);
        }
        public IActionResult CreateProduct()
        {
            IEnumerable<SelectListItem> categoryList= _categoryDb.GetAll().Select(x=> new SelectListItem() { Text= x.Name, Value= x.Id.ToString()});
            ProductViewModel productViewModel = new ProductViewModel()
            {
                Product = new Product(),
                CategoryList = categoryList

            };
            return View(productViewModel);
        }
        [HttpPost]
        public IActionResult CreateProduct(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                _productDb.Add(productViewModel.Product);
                _productDb.Save();
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to create product";
            return View(productViewModel);

        }
        public IActionResult EditProduct(int? id)
        {
            var productViewModel = new ProductViewModel();
            productViewModel.Product = _productDb.Get(x => x.ProductId == id);
            return View(productViewModel);
        }
        [HttpPost]
        public IActionResult EditProduct(ProductViewModel productViewModel, int? id)
        {
            if (ModelState.IsValid)
            {
                productViewModel.Product = _productDb.Get(x => x.ProductId == id);
                _productDb.Update(productViewModel.Product);
                _productDb.Save();
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to update product";
            return View(productViewModel);

        }
        public IActionResult DeleteProduct(int? id)
        {

            var productViewModel = new ProductViewModel();
            productViewModel.Product = _productDb.Get(x => x.ProductId == id);
            return View(productViewModel);
        }

        [HttpPost]
        public IActionResult DeleteProduct(ProductViewModel productViewModel, int? id)
        {
            if (ModelState.IsValid)
            {
                productViewModel.Product = _productDb.Get(x => x.ProductId == id);
                _productDb.Delete(productViewModel.Product);
                _productDb.Save();
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to delete product";
            return View(productViewModel);

        }
    }
}
