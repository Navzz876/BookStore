using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

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
            _applicationContext.Categories.Add(category);
            _applicationContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
