using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BookStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IProductRepository _productRepository;
        public UserController(ILogger<UserController> logger, IProductRepository productRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
        }

        public IActionResult Index()
        {
            var productList = _productRepository.GetAll(includeProperties: "Category");
            return View(productList);
        }
    }
}