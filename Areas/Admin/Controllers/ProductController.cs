using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        protected readonly IProductRepository _productDb;
        public ProductController(IProductRepository productDb)
        {
            _productDb = productDb;
        }
        public IActionResult Index()
        {
            var categoryList = _productDb.GetAll();
            return View(categoryList);
        }
        public IActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _productDb.Add(product);
                _productDb.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to create category";
            return View();

        }
        public IActionResult EditProduct(int? id)
        {

            var category = _productDb.Get(x => x.ProductId == id);
            return View(category);
        }
        [HttpPost]
        public IActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _productDb.Update(product);
                _productDb.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to update category";
            return View();

        }
        public IActionResult DeleteProduct(int? id)
        {

            var product = _productDb.Get(x => x.ProductId == id);
            return View(product);
        }
        [HttpPost]
        public IActionResult DeleteProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _productDb.Delete(product);
                _productDb.Save();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to delete category";
            return View();

        }
    }
}
