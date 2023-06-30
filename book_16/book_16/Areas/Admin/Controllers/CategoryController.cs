using book.DataAccess.Repository.IRepository;
using book.models;
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
    public class CategoryController : Controller
    {
        private readonly IunitOfWork _unitofWork;
        public CategoryController(IunitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }
        public IActionResult Index()
        {
           
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
                return View(category);
            var CategoryInDb = _unitofWork.Category.Get(id.GetValueOrDefault());
            if (CategoryInDb == null)
                return NotFound();
            return View(CategoryInDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null)
                return NotFound();
            if (!ModelState.IsValid)
                return View(category);
            if (category.Id == 0)
                _unitofWork.Category.Add(category);
            else
                _unitofWork.Category.Update(category);
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));

        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var CategoryList = _unitofWork.Category.GetAll();
            return Json(new { data = CategoryList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var CategoryInDb = _unitofWork.Category.Get(id);
            if (CategoryInDb == null)
                return Json(new { success = false, message = "Error While Delete Data!!!" });
            _unitofWork.Category.remove(CategoryInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "delete data successfully!!!" });
        }
        #endregion
    }
}
