using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        protected readonly ICategoryRepository _categoryDb;
        public CategoryController(ICategoryRepository categoryDb)
        {
            _categoryDb = categoryDb;
        }
        public IActionResult Index()
        {
            var categoryList = _categoryDb.GetAll(includeProperties:null);
            return View(categoryList);
        }
        public IActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryDb.Add(category);
                _categoryDb.Save();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to create category";
            return View();

        }
        public IActionResult EditCategory(int? id)
        {

            var category = _categoryDb.Get(x => x.Id == id, includeProperties: null);
            return View(category);
        }
        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryDb.Update(category);
                _categoryDb.Save();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to update category";
            return View();

        }
        public IActionResult DeleteCategory(int? id)
        {

            var category = _categoryDb.Get(x => x.Id == id, includeProperties: null);
            return View(category);
        }
        [HttpPost]
        public IActionResult DeleteCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _categoryDb.Delete(category);
                _categoryDb.Save();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to delete category";
            return View();

        }
    }
}
