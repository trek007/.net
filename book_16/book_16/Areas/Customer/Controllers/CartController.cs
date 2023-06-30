using book.DataAccess.Repository.IRepository;
using book.models;
using book.models.ViewModels;
using book.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace book_16.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IunitOfWork _unitofWork;
        public CartController(IunitOfWork unitofWork)
        {
            _unitofWork = unitofWork;
        }
        [BindProperty]
        public ShoppincartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {

            var ClaimIdentity = (ClaimsIdentity)User.Identity;
            var Claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (Claim == null)
            {
                ShoppingCartVM=new ShoppincartVM()
                {
                    ListCart = new List<ShoppingCart>()

                };
                return View(ShoppingCartVM);
            }
            //***********
            ShoppingCartVM = new ShoppincartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == Claim.Value, includeProperties: "Product")
            };
            ShoppingCartVM.OrderHeader.OrderTotal = 0;
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstorDefault(u => u.Id == Claim.Value, includeProperties: "Campany");
            foreach (var List in ShoppingCartVM.ListCart)
            {
                List.Price = SD.GetPriceBasedOnQuantity(List.Count, List.Product.Price, List.Product.Price50, List.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (List.Price * List.Count);
                List.Product.Discription = SD.ConverToRowhtml(List.Product.Discription);
                if (List.Product.Discription.Length > 100)
                {
                    List.Product.Discription = List.Product.Discription.Substring(0, 99) + "....";
                }
            }
            return View(ShoppingCartVM);
        }
        public IActionResult Plus(int cartId)
        {
            var cart = _unitofWork.ShoppingCart.FirstorDefault(sc => sc.Id == cartId, includeProperties: "Product");
            cart.Count += 1;
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cart = _unitofWork.ShoppingCart.FirstorDefault(sc => sc.Id == cartId, includeProperties: "Product");
            cart.Count -= 1;
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _unitofWork.ShoppingCart.FirstorDefault(sc => sc.Id == cartId, includeProperties: "Product");
            _unitofWork.ShoppingCart.remove(cart);
            _unitofWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            var claimIdantity = (ClaimsIdentity)(User.Identity);
            var claim = claimIdantity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM = new ShoppincartVM()
            {
                OrderHeader = new OrderHeader(),
                ListCart = _unitofWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")

            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstorDefault(u => u.Id == claim.Value, includeProperties: "Campany");
            foreach (var List in ShoppingCartVM.ListCart)
            {

                List.Price = SD.GetPriceBasedOnQuantity(List.Count, List.Product.Price, List.Product.Price50, List.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal += (List.Price * List.Count);
                List.Product.Discription = SD.ConverToRowhtml(List.Product.Discription);
            }
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.Postalcode;

            return View(ShoppingCartVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken)
        {
            var claimIdentity = (ClaimsIdentity)(User.Identity);
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitofWork.ApplicationUser.FirstorDefault(u => u.Id == claim.Value, includeProperties: "Campany");

            ShoppingCartVM.ListCart = _unitofWork.ShoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value, includeProperties: "Product");

            ShoppingCartVM.OrderHeader.Paymentstatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.Orderstatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.OderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            _unitofWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitofWork.Save();

            foreach (var List in ShoppingCartVM.ListCart)
            {
                List.Price = SD.GetPriceBasedOnQuantity(List.Count, List.Product.Price, List.Product.Price50, List.Product.Price100);

                OrderDetails orderDetails=new OrderDetails()
                {
                    ProductId = List.ProductId,
                    OrderHederId = ShoppingCartVM.OrderHeader.Id,
                    Price = List.Price,
                    Count = List.Count
                };

                ShoppingCartVM.OrderHeader.OrderTotal += (orderDetails.Price * orderDetails.Count);

                _unitofWork.OrderDetail.Add(orderDetails);
                // _unitofWork.Save();
            }
            //_unitofWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitofWork.Save();

            HttpContext.Session.SetInt32(SD.Ss_Session, 0);

            #region Stripe Payment
            if (stripeToken == null)
            {
                ShoppingCartVM.OrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartVM.OrderHeader.Paymentstatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.OrderHeader.Orderstatus = SD.PaymentStatusApproved;
            }
            else
            {
                var Options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.OrderHeader.OrderTotal),
                    Currency = "usd",
                    Description = "order Id:" + ShoppingCartVM.OrderHeader.Id,
                    Source = stripeToken
                };
                //Paymat
                var Service = new ChargeService();
                Charge change = Service.Create(Options);
                if (change.BalanceTransactionId == null)
                    ShoppingCartVM.OrderHeader.Paymentstatus = SD.PaymentStatusReject;
                ShoppingCartVM.OrderHeader.TranSactionId = change.BalanceTransactionId;
                if (change.Status.ToLower() == "Succeeded")
                {
                    ShoppingCartVM.OrderHeader.Paymentstatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.OrderHeader.Orderstatus = SD.OrderStatusApproved;
                    ShoppingCartVM.OrderHeader.PaymentDate = DateTime.Now;
                }

            }
            _unitofWork.Save();

            #endregion

            return RedirectToAction("OrderConfirmation", "Cart", new { id = ShoppingCartVM.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}
