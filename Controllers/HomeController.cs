using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Linq;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserInformationRepository _userInformationRepository;
        private readonly ICategoryRepository _categoryRepository;
        public HomeController(ILogger<HomeController> logger, IUserInformationRepository userInformationRepository, ICategoryRepository categoryRepository )
        {
            _logger = logger;
            _userInformationRepository = userInformationRepository; 
            _categoryRepository = categoryRepository;
        }

        public IActionResult Index()
        {
            var userInfo = new UserInformation()
            {
                UserName="",
                Password= ""
            };
            return View(userInfo);
        }
        [HttpPost]
        public IActionResult Index(UserInformation userInformation)
        {
            var userInfoList = _userInformationRepository.GetAll(includeProperties: null);
            var IsEnteredUserName = userInfoList.Where(x => x.UserName == userInformation.UserName);
            var IsEnteredPassword = userInfoList.Where(x => x.Password == userInformation.Password);          
            if (IsEnteredPassword.Any() && IsEnteredUserName.Any())
            {
                if (userInformation.UserName.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Category", new { area = "Admin" });
                }
                else if (userInformation.UserName.Contains("Customer"))
                {
                    return RedirectToAction("Index", "User", new { area = "Customer" });
                }
            }
            TempData["failure"] = "User does not exist in the records. Please Register as a new user";
            return View(userInformation);
        }
        public IActionResult RegisterUser()
        {
            IEnumerable<SelectListItem> categoryList = _categoryRepository.GetAll(includeProperties: null).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
            UserInformationViewModel userInformationViewModel = new UserInformationViewModel()
            {
                UserInformation = new UserInformation(),
                CategoryList = categoryList
            };

            return View(userInformationViewModel);
        }
        [HttpPost]
        public IActionResult RegisterUser( UserInformationViewModel userInformationViewModel )
        {
            if(ModelState.IsValid)
            {
                if(userInformationViewModel != null)
                {
                    _userInformationRepository.Add(userInformationViewModel.UserInformation);
                    _userInformationRepository.Save();
                    TempData["success"] = "User Registered Successfully";
                    return RedirectToAction("Index", "Home");
                }                
            }
            TempData["failure"] = "Unable to Register the User at the moment. Please try again later";
            return RedirectToAction("Index", "Home");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}