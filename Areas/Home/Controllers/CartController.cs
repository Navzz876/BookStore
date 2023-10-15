using BookStore.Controllers;
using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using BookStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

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
        private readonly IBillingDetailsRepository _billingDetailsRepository;
        public CartController(ILogger<HomeController> logger, IProductRepository productRepository, IShoppingCartRepository shoppingCartRepository, IApplicationUserRepository applicationUserRepository,
            IBillingDetailsRepository billingDetailsRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _shoppingCartRepository = shoppingCartRepository;
            _applicationUserRepository = applicationUserRepository;
            _billingDetailsRepository = billingDetailsRepository;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var shoppingCartViewModel = new ShoppingCartViewModel()
            {
                ShoppingCartList = _shoppingCartRepository.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product"),
            };
            foreach (var shoppingCart in shoppingCartViewModel.ShoppingCartList)
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
            if (shoppingCartViewModel != null && shoppingCartViewModel.OrderTotal > 0)
            {
                return RedirectToAction("CheckOutForm", "Cart", shoppingCartViewModel);
            }
            TempData["failure"] = "Unable to check out at the moment, Please try again later";
            return View(shoppingCartViewModel);
        }
        [HttpPost]
        public IActionResult Plus(int cartId)
        {
            var shoppingCart = _shoppingCartRepository.Get(x => x.Id == cartId, includeProperties: null);
            shoppingCart.Count += 1;
            _shoppingCartRepository.Update(shoppingCart);
            _shoppingCartRepository.Save();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Minus(int cartId)
        {
            var shoppingCart = _shoppingCartRepository.Get(x => x.Id == cartId, includeProperties: null);

            if (shoppingCart.Count <= 1)
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
                Name = appUserDetails.Name,
                EmailAddress = appUserDetails.Email,
                State = appUserDetails.State,
                Address = appUserDetails.StreetAddress,
                Address2 = appUserDetails.StreetAddress,
                ZipCode = appUserDetails.PostalCode,
                City = appUserDetails.City,

            };
            shoppingCartViewModel.BillingDetails = billingDetails;
            return View(shoppingCartViewModel);
        }
        [HttpPost]
        [ActionName("CheckOutForm")]
        public IActionResult CheckOutFormPOST(ShoppingCartViewModel shoppingCartViewModel)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCartViewModel.ShoppingCartList = _shoppingCartRepository.GetAll(x => x.ApplicationUserId == userId, includeProperties: "Product");
            shoppingCartViewModel.TotalCount = shoppingCartViewModel.ShoppingCartList.Count();
            shoppingCartViewModel.BillingDetails.OrderStatus = Constants.PaymentStatus.Pending;
            shoppingCartViewModel.BillingDetails.PaymentStatus = Constants.PaymentStatus.Pending;

            _billingDetailsRepository.Add(shoppingCartViewModel.BillingDetails);
            _billingDetailsRepository.Save();

            //Payment processing
            var domain = "https://localhost:7172/";
            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Home/Cart/OrderConfirmation?id={shoppingCartViewModel.BillingDetails.BillingId}",
                CancelUrl = domain + $"Home/Cart",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
            };
            foreach(var cart in shoppingCartViewModel.ShoppingCartList)
            {
                var sessionItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = ((long)cart.Product.Price * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = cart.Product.Title,
                        }
                    },
                    Quantity = cart.Count
                };
                options.LineItems.Add(sessionItem);
            }
            var service = new SessionService();
            var session= service.Create(options);
            _billingDetailsRepository.UpdatePaymentId(shoppingCartViewModel.BillingDetails.BillingId, session.Id, session.PaymentIntentId);
            _billingDetailsRepository.Save();

            Response.Headers.Add("Location",session.Url);
            return new StatusCodeResult (303);
        }
        public IActionResult OrderConfirmation(int? id)
        {
            var billingDetails = _billingDetailsRepository.Get(x => x.BillingId == id, includeProperties: null);
            var service= new SessionService();
            var session = service.Get(billingDetails.SessionId);
            if(session.Status.ToLower() == "paid")
            {
                _billingDetailsRepository.UpdatePaymentId(id, session.Id, session.PaymentIntentId);
                _billingDetailsRepository.UpdateStatus(id, Constants.PaymentStatus.Approved, Constants.PaymentStatus.Approved);
                _billingDetailsRepository.Save();
            }
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            var shoppingCart = _shoppingCartRepository.GetAll(x => x.ApplicationUserId == userId);
            _shoppingCartRepository.DeleteRange(shoppingCart);
            _shoppingCartRepository.Save();
            return View(id);
        }
    }
}
