using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ePizzaHub.UI.Controllers
{
    public class CartController : BaseController
    {
        ICartService _cartService;
        Guid CartId
        {
            get
            {
                Guid Id;
                if (Request.Cookies["CId"] == null)
                {
                    Id = Guid.NewGuid();
                    Response.Cookies.Append("CId", Id.ToString(), new CookieOptions { Expires = DateTime.Now.AddDays(1) });
                }
                else
                {
                    Id = Guid.Parse(Request.Cookies["CId"]);
                }
                return Id;
            }
        }
        public CartController(ICartService cartService, ICartService cartService1)
        {
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            CartModel cartModel = _cartService.GetCartDetails(CartId);
            return View(cartModel);
        }

        [Route("Cart/AddToCart/{ItemId}/{UnitPrice}/{Quantity}")]
        public IActionResult AddToCart(int ItemId, decimal UnitPrice, int Quantity)
        {
            int UserId = CurrentUser != null ? CurrentUser.Id : 0;
            if (ItemId > 0 && UnitPrice > 0 && Quantity > 0)
            {
                Cart cart = _cartService.AddItem(UserId, CartId, ItemId, UnitPrice, Quantity);
                if (cart != null)
                {
                    return Json(new { status = "success", count = cart.CartItems.Count });
                }
            }
            return Json(new { status = "failed", count = 0 });
        }

        [Route("Cart/DeleteItem/{ItemId}")]
        public IActionResult DeleteItem(int ItemId)
        {
            int count = _cartService.DeleteItem(CartId, ItemId);
            return Json(count);
        }

        [Route("Cart/UpdateQuantity/{Id}/{Quantity}")]
        public IActionResult UpdateQuantity(int Id, int Quantity)
        {
            int count = _cartService.UpdateQuantity(CartId, Id, Quantity);
            return Json(count);
        }

        public IActionResult GetCartCount()
        {
            int count = _cartService.GetCartCount(CartId);
            return Json(count);
        }

        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckOut(AddressModel model)
        {
            if (ModelState.IsValid)
            {
                CartModel cartModel = _cartService.GetCartDetails(CartId);
                int UserId = CurrentUser != null ? CurrentUser.Id : 0;
                cartModel.UserId = UserId;
                _cartService.UpdateCart(CartId, UserId);

                TempData.Set("Address", model);
                TempData.Set("Cart", cartModel);

                return RedirectToAction("Index", "Payment");
            }
            return View(model);
        }
    }
}
