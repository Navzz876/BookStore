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
        private readonly ILogger<HomeController> _logger;
        private readonly IUserInformationRepository _userInformationRepository;
        private readonly ICategoryRepository _categoryRepository;
        public AdminHomeController(ILogger<HomeController> logger, IUserInformationRepository userInformationRepository, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _userInformationRepository = userInformationRepository;
            _categoryRepository = categoryRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RegisterUser()
        {
            IEnumerable<SelectListItem> categoryList = _categoryRepository.GetAll(includeProperties:null).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
            UserInformationViewModel userInformationViewModel = new UserInformationViewModel()
            {
                UserInformation = new UserInformation(),
                CategoryList = categoryList
            };

            return View(userInformationViewModel);
        }
        [HttpPost]
        public IActionResult RegisterUser(UserInformationViewModel userInformationViewModel)
        {
            if (ModelState.IsValid)
            {
                if (userInformationViewModel != null)
                {
                    _userInformationRepository.Add(userInformationViewModel.UserInformation);
                    _userInformationRepository.Save();
                    TempData["success"] = "User Registered Successfully";
                    return RedirectToAction("Index", "AdminHome");
                }
            }
            TempData["failure"] = "Unable to Register the User at the moment. Please try again later";
            IEnumerable<SelectListItem> categoryList = _categoryRepository.GetAll(includeProperties: null).Select(x => new SelectListItem() { Text = x.Name, Value = x.Id.ToString() });
            userInformationViewModel.CategoryList= categoryList;
            return View(userInformationViewModel);
        }
    }
}
