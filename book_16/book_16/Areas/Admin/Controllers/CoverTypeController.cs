using book.DataAccess.Repository.IRepository;
using book.models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace book_16.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CoverTypeController : Controller
    {
        private readonly IunitOfWork _unitofWork;
        public CoverTypeController(IunitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
                return View(coverType);
         
            coverType = _unitofWork.CoverType.Get(id.GetValueOrDefault());
            if (coverType == null)
                return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (coverType == null)
                return NotFound();
            if (!ModelState.IsValid) return View(coverType);
            var param = new DynamicParameters();
            if (!ModelState.IsValid) return View(coverType);
            
            if (coverType.Id == 0)
                _unitofWork.CoverType.Add(coverType);
          
            else
        
                _unitofWork.CoverType.Upadate(coverType);
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var CoverList = _unitofWork.CoverType.GetAll();
         
            return Json(new { data = CoverList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var CoverTypeInDb = _unitofWork.CoverType.Get(id);
            if (CoverTypeInDb == null)
                return Json(new { succuess = false, message = "EEror while delete data" });
        
            _unitofWork.CoverType.remove(CoverTypeInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "delete data successfully!!!" });
        }
        #endregion
    }
}
