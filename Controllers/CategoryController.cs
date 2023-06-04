using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers
{
    public class CategoryController : Controller
    {
        protected readonly ApplicationContext _applicationContext;
        public CategoryController(ApplicationContext applicationContext) 
        {
            _applicationContext = applicationContext;
        }
        public IActionResult Index()
        {
            var categoryList= _applicationContext.Categories.ToList();
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
                _applicationContext.Categories.Add(category);
                _applicationContext.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to create category";
            return View();
           
        }
        public IActionResult EditCategory(int? id)
        {
           
            var categoryList = _applicationContext.Categories.ToList();
            var category= categoryList.FirstOrDefault(x=>x.Id == id);
            return View(category);
        }
        [HttpPost]
        public IActionResult EditCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _applicationContext.Categories.UpdateRange(category);
                _applicationContext.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
                TempData["failure"] = "Unable to update category";
                return View();

        }
        public IActionResult DeleteCategory(int? id)
        {

            var categoryList = _applicationContext.Categories.ToList();
            var category = categoryList.FirstOrDefault(x => x.Id == id);
            return View(category);
        }
        [HttpPost]
        public IActionResult DeleteCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                _applicationContext.Categories.RemoveRange(category);
                _applicationContext.SaveChanges();
                TempData["success"] = "Category deleted successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to delete category";
            return View();

        }
    }
}
