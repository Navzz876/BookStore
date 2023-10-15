using BookStore.Controllers;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IApplicationUserRepository _applicationUserRepository;
        public CartController(ILogger<HomeController> logger, IProductRepository productRepository, IShoppingCartRepository shoppingCartRepository, IApplicationUserRepository applicationUserRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _applicationUserRepository = applicationUserRepository;
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
        public IActionResult Index(ShoppingCartViewModel shoppingCartViewModel)
        {
               var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
                 shoppingCartViewModel = new ShoppingCartViewModel()
                {
                    ShoppingCartList = _shoppingCartRepository.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
                };
                foreach (var shoppingCart in shoppingCartViewModel.ShoppingCartList)
                {
                    var price = shoppingCart.Product.Price * shoppingCart.Count;
                    shoppingCartViewModel.OrderTotal += price;
                }           
            if(shoppingCartViewModel != null && shoppingCartViewModel.OrderTotal > 0)
            {
                return RedirectToAction("CheckOutForm", "Cart",shoppingCartViewModel);
            }
            TempData["failure"] = "Unable to check out at the moment, Please try again later";
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
        public IActionResult CheckOutForm(ShoppingCartViewModel shoppingCartViewModel)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartViewModel.ShoppingCartList = _shoppingCartRepository.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");
            shoppingCartViewModel.TotalCount = shoppingCartViewModel.ShoppingCartList.Count();
            var appUserDetails = _applicationUserRepository.Get(x => x.Id == userId, includeProperties: null);
            var billingDetails = new BillingDetails()
            {
                Name= appUserDetails.Name,
                EmailAddress= appUserDetails.Email,
                State= appUserDetails.State,
                Address= appUserDetails.StreetAddress,
                Address2= appUserDetails.StreetAddress,
                ZipCode= appUserDetails.PostalCode,
                City= appUserDetails.City,

            };
            shoppingCartViewModel.BillingDetails= billingDetails;
            return View(shoppingCartViewModel);
        }
    }
}
