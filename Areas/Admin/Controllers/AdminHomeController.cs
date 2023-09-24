using BookStore.Controllers;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminHomeController : Controller
    {
        private readonly ILogger<AdminHomeController> _logger;
        private readonly IProductRepository _productRepository;
        public AdminHomeController(ILogger<AdminHomeController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var productList = _productRepository.GetAll(includeProperties: "Category");
            return View(productList);
        }
        public IActionResult Details(int id)
        {
            var product = _productRepository.Get(x => x.ProductId == id, includeProperties: "Category");
            return View(product);
        }
    }
}
