using BookStore.DataAccess.Repository.IRepository;
using BookStore.Models.Models;
using BookStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BookStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Constants.UserRoles.Admin)]
    public class CompanyController : Controller
    {
        protected readonly ICompanyRepository _companyDb;
        public CompanyController(ICompanyRepository companyDb)
        {
            _companyDb = companyDb;
        }
        public IActionResult Index()
        {
            var companyList = _companyDb.GetAll(includeProperties: null);
            return View(companyList);
        }
        public IActionResult CreateCompany()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCompany(Company company)
        {
            if (ModelState.IsValid)
            {
                _companyDb.Add(company);
                _companyDb.Save();
                TempData["success"] = "Company created successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to create company";
            return View();

        }
        public IActionResult EditCompany(int? id)
        {

            var company = _companyDb.Get(x => x.Id == id, includeProperties: null);
            return View(company);
        }
        [HttpPost]
        public IActionResult EditCompany(Company company)
        {
            if (ModelState.IsValid)
            {
                _companyDb.Update(company);
                _companyDb.Save();
                TempData["success"] = "Company updated successfully";
                return RedirectToAction("Index");
            }
            TempData["failure"] = "Unable to update company";
            return View();

        }
        #region APICalls
        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = _companyDb.GetAll(includeProperties: null);
            return Json(new { data = companyList });
        }
        [HttpDelete]
        public IActionResult DeleteCompany(Company company, int? id)
        {
            company = _companyDb.Get(x => x.Id == id, includeProperties: null);
            if (ModelState.IsValid)
            {
                _companyDb.Delete(company);
                _companyDb.Save();
                return Json(new { success = true, message = "Company Deleted Successfully" });
            }
            return Json(new { success = false, message = "Unable to Delete Company" });

        }
        #endregion
    }
}