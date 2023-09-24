using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Linq;
using static BookStore.Utilities.Constants;
using Extensions.MV;
using BookStore.DataAccess.Repository;

namespace BookStore.Controllers
{
    [Area("Home")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository)
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
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}