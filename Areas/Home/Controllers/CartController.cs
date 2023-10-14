using BookStore.Controllers;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Areas.Home.Controllers
{
    [Area("Home")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        public CartController(ILogger<HomeController> logger, IProductRepository productRepository, IShoppingCartRepository shoppingCartRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var shoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCartList = _shoppingCartRepository.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
            };
            foreach(var shoppingCart in shoppingCartViewModel.ShoppingCartList)
            {
                var price = shoppingCart.Product.Price * shoppingCart.Count;
                shoppingCartViewModel.OrderTotal += price;
            }
            return View(shoppingCartViewModel);          
        }
        [HttpPost]
        public IActionResult Plus(int cartId)
        {
            var shoppingCart = _shoppingCartRepository.Get(x => x.Id == cartId,includeProperties:null);
            shoppingCart.Count += 1;
            _shoppingCartRepository.Update(shoppingCart);
            _shoppingCartRepository.Save();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Minus(int cartId)
        {
            var shoppingCart = _shoppingCartRepository.Get(x => x.Id == cartId, includeProperties: null);
            
            if(shoppingCart.Count <= 1) 
            {
                _shoppingCartRepository.Delete(shoppingCart);
            }
            else
            {
                shoppingCart.Count -= 1;
                _shoppingCartRepository.Update(shoppingCart);
            }
           
            _shoppingCartRepository.Save();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int cartId)
        {
            var shoppingCart = _shoppingCartRepository.Get(x => x.Id == cartId, includeProperties: null);
            _shoppingCartRepository.Delete(shoppingCart);
            _shoppingCartRepository.Save();
            return RedirectToAction("Index");
        }
    }
}
