using book.DataAccess.Repository.IRepository;
using book.models;
using book.models.ViewModels;
using book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace book_16.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly IunitOfWork _unitofWork;

       
        public HomeController(ILogger<HomeController> logger, IunitOfWork unitofWork)
            
        {
            _logger = logger;
            _unitofWork = unitofWork;
        }

        public IActionResult Index()
        {
            var ProductList = _unitofWork.Product.GetAll(includeProperties: "category,coverType");
            //Session

            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var Claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (Claim != null)
            {
                var Count = _unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == Claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_Session, Count);
            }
            return View(ProductList);
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult chahat()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Details(int id)
        {
            var productInDb = _unitofWork.Product.FirstorDefault(p => p.Id== id, includeProperties: "category,coverType");
            if (productInDb == null)
                return NotFound();
            var shoppingCart=new ShoppingCart()
            {
                Product = productInDb,
                ProductId = productInDb.Id
            };
            return View(shoppingCart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart ShoppingCartobj)
        {
            ShoppingCartobj.Id = 0;
            if (ModelState.IsValid)

            {

                var ClaimIdentity = (ClaimsIdentity)User.Identity;
                var Claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                ShoppingCartobj.ApplicationUserId = Claim.Value;
                var ShoppingCartFromDb = _unitofWork.ShoppingCart.FirstorDefault
                     (u => u.ApplicationUserId == Claim.Value &&
                     u.ProductId == ShoppingCartobj.ProductId);

                if (ShoppingCartFromDb == null)
                {
                    //Add Cart
                    _unitofWork.ShoppingCart.Add(ShoppingCartobj);
                }
                else
                {
                    //update to cart
                    ShoppingCartFromDb.Count += ShoppingCartobj.Count;

                }
                _unitofWork.Save();
                // Session
                var Count = _unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == Claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_Session, Count);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productInDb = _unitofWork.Product.FirstorDefault(p => p.Id == ShoppingCartobj.ProductId, includeProperties: "Category,CoverType");
                var ShoppingCart = new ShoppingCart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id
                };
                return View(ShoppingCart);
            }
        }
    }
    
}
