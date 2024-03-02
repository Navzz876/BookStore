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
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BookStore.Controllers
{
    [Area("Home")] //Test
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        public HomeController(ILogger<HomeController> logger, IProductRepository productRepository, IShoppingCartRepository shoppingCartRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
        }

        public IActionResult Index()
        {
            var productList = _productRepository.GetAll(includeProperties: "Category");
            return View(productList);
        }
        public IActionResult Details(int id)
        {
            var product = _productRepository.Get(x => x.ProductId == id, includeProperties: "Category");
            var shoppingCart = new ShoppingCart()
            {
                Product = product,
                Count=1,
                ProductId= id
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity= (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = userId;
            var dbCart = _shoppingCartRepository.Get(x => x.ApplicationUserId == userId && x.ProductId == shoppingCart.ProductId, includeProperties:null);
            if(dbCart != null)
            {
                dbCart.Count += shoppingCart.Count;
                _shoppingCartRepository.Update(dbCart);
            }
            else
            {
                shoppingCart.Id = 0;
                _shoppingCartRepository.Add(shoppingCart);
            }
            _shoppingCartRepository.Save();
            TempData["success"] = "Product added to cart successfully";
            return RedirectToAction("Index","Cart");

        }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}