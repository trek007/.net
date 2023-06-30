using book.DataAccess.Repository.IRepository;
using book.models;
using book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace book_16.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin + "," +SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IunitOfWork _unitofWork;
        public CompanyController(IunitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Campany campany = new Campany();
            if (id == null)
            {
                return View(campany);
            }

            else
            {
                var companyInDb = _unitofWork.Campany.Get(id.GetValueOrDefault());
                return View(companyInDb);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Campany campany)
        {
            if (campany == null)
                return NotFound();
            if (!ModelState.IsValid) return View(campany);
            if (campany.Id == 0)
                _unitofWork.Campany.Add(campany);
            else
                _unitofWork.Campany.Update(campany);
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitofWork.Campany.GetAll() });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var CompanyInDb = _unitofWork.Campany.Get(id);
            if (CompanyInDb == null)
                return Json(new { success = false, message = "Error _Delete data" });
            _unitofWork.Campany.remove(CompanyInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "Data Delete successfully" });
        }

        #endregion

    }
}

