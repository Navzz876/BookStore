using BookStore.Data;
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
    }
}
