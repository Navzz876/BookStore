using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models;
using BookStore.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserInformationRepository _userInformationRepository;
        public HomeController(ILogger<HomeController> logger, IUserInformationRepository userInformationRepository)
        {
            _logger = logger;
            _userInformationRepository = userInformationRepository; 
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
                if (userInformation.UserName.Contains("admin"))
                {
                    return RedirectToAction("Index", "Category", new { area = "Admin" });
                }
                else if (userInformation.UserName.Contains("org"))
                {
                    userInformation.UserType = "Company";
                }
            }
            TempData["failure"] = "User does not exist in the records. Please Register as a new user";
            return View(userInformation);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}