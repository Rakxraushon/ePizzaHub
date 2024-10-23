using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.UI.Controllers
{
    public class PaymentController : BaseController
    {
        IConfiguration _configuration;
        IPaymentService _paymentService;
        IOrderService _orderService;
        public PaymentController(IConfiguration configuration, IPaymentService paymentService, IOrderService orderService)
        {
            _configuration = configuration;
            _paymentService = paymentService;
            _orderService = orderService;
        }

        public IActionResult Index()
        {
            PaymentModel payment = new PaymentModel();
            CartModel cart = TempData.Peek<CartModel>("Cart");
            if (cart != null)
            {
                payment.Cart = cart;
                payment.GrandTotal = Math.Round(cart.GrandTotal);
                payment.Currency = "INR";
                payment.Description = string.Join(",", cart.Items.Select(r => r.Name));
                payment.RazorpayKey = _configuration["Razorpay:Key"];
                payment.Receipt = Guid.NewGuid().ToString(); //Merchant Transaction Id
                payment.OrderId = _paymentService.CreateOrder(payment.GrandTotal * 100, payment.Currency, payment.Receipt);

                return View(payment);
            }
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Status(IFormCollection form)
        {
            if (form.Keys.Count > 0)
            {
                string paymentId = form["rzp_paymentid"];
                string orderId = form["rzp_orderid"];
                string signature = form["rzp_signature"];
                string transactionId = form["Receipt"];
                string currency = form["Currency"];

                bool IsSignVerified = _paymentService.VerifySignature(signature, orderId, paymentId);
                if (IsSignVerified)
                {
                    CartModel cart = TempData.Peek<CartModel>("Cart");
                    PaymentDetail model = new PaymentDetail();

                    model.CartId = cart.Id;
                    model.Total = cart.Total;
                    model.Tax = cart.Tax;
                    model.GrandTotal = cart.GrandTotal;
                    model.Currency = currency;
                    model.CreatedDate = DateTime.Now;

                    model.Status = "Success";
                    model.TransactionId = transactionId;
                    model.Id = paymentId;
                    model.Email = CurrentUser.Email;
                    model.UserId = CurrentUser.Id;

                    int status = _paymentService.SavePaymentDetails(model);
                    if (status > 0)
                    {
                        TempData.Remove("Cart");
                        Response.Cookies.Delete("CId");

                        AddressModel address = TempData.Peek<AddressModel>("Address");
                        _orderService.PlaceOrder(CurrentUser.Id, orderId, paymentId, cart, address);

                        TempData.Set("PaymentDetails", model);
                        return RedirectToAction("Receipt");
                    }
                    else
                    {
                        ViewBag.Message = "Due to some technical issues we are not able to receive order details. We will contact you soon..";
                    }
                }
            }

            ViewBag.Message = "Your payment has been failed. You can contact us at support@dotnettricks.com.";
            return View();
        }

        public IActionResult Receipt()
        {
            PaymentDetail model = TempData.Peek<PaymentDetail>("PaymentDetails");
            return View(model);
        }
    }
}
