using book.DataAccess.Repository.IRepository;
using book.models;
using book.models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace book_16.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {

        private readonly IunitOfWork _unitofWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IunitOfWork UnitofWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitofWork = UnitofWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitofWork.Category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString()
                }),
                CoverTypeList = _unitofWork.CoverType.GetAll().Select(ct => new SelectListItem()
                {
                    Text = ct.Name,
                    Value = ct.Id.ToString()
                })

            };
            if (id == null)
                return View(productVM);
            productVM.Product = _unitofWork.Product.Get(id.GetValueOrDefault());
            return View(productVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVm)
        {
            if (ModelState.IsValid)
            {
                var webrootpath = _webHostEnvironment.WebRootPath;
                var file = HttpContext.Request.Form.Files;
                if (file.Count > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var Uploads = Path.Combine(webrootpath, @"images\product");
                    var extension = Path.GetExtension(file[0].FileName);
                    if (productVm.Product.Id != 0)
                    {
                        var imageExsits = _unitofWork.Product.Get(productVm.Product.Id).Imageurl;
                        productVm.Product.Imageurl = imageExsits;
                    }
                    if (productVm.Product.Imageurl != null)
                    {
                        var imagePath = Path.Combine(webrootpath, productVm.Product.Imageurl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filestrem = new FileStream(Path.Combine(Uploads, fileName + extension), FileMode.Create))
                    {
                        file[0].CopyTo(filestrem);
                    }
                    productVm.Product.Imageurl = @"\images\product\" + fileName + extension;
                }
                else
                {
                    if (productVm.Product.Id != 0)
                    {
                        var imageExists = _unitofWork.Product.Get(productVm.Product.Id).Imageurl;
                        productVm.Product.Imageurl = imageExists;
                    }
                }
                if (productVm.Product.Id == 0)
                    _unitofWork.Product.Add(productVm.Product);
                else
                    _unitofWork.Product.Update(productVm.Product);
                _unitofWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVm = new ProductVM()
                {

                    CategoryList = _unitofWork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _unitofWork.CoverType.GetAll().Select(ct => new SelectListItem()
                    {
                        Text = ct.Name,
                        Value = ct.Id.ToString()
                    })

                };
                if (productVm.Product.Id != 0)
                {
                    productVm.Product = _unitofWork.Product.Get(productVm.Product.Id);
                }
                return View(productVm);
            }
        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {

            var productlist = _unitofWork.Product.GetAll(includeProperties: "category,coverType");
            return Json(new { data = productlist });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var productInDb = _unitofWork.Product.Get(id);
            if (productInDb == null)
                return Json(new { success = false, message = "Errer While delete data!!!" });
            if (productInDb.Imageurl != "")
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, productInDb.Imageurl.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _unitofWork.Product.remove(productInDb);
            _unitofWork.Save();
            return Json(new { success = true, message = "data delete successfully" });
        }
        #endregion

    }
}
