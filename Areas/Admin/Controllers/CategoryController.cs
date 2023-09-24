using BookStore.DataAccess.Data;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.UserRoles.Admin)]
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
        #region APICalls
        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _categoryDb.GetAll(includeProperties: null);
            return Json(new { data = categoryList });
        }
        [HttpDelete]
        public IActionResult DeleteCategory(Category category, int?id)
        {
            category = _categoryDb.Get(x => x.Id == id, includeProperties: null);
            if (ModelState.IsValid)
            {
                _categoryDb.Delete(category);
                _categoryDb.Save();
                return Json(new { success = true, message = "Category Deleted Successfully" });
            }
            return Json(new { success = false, message = "Unable to Delete Product" });

        }
        #endregion
    }
}
