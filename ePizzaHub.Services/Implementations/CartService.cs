using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ePizzaHub.Services.Implementations
{
    public class CartService : Service<Cart>, ICartService
    {
        ICartRepository _cartRepository;
        IRepository<CartItem> _cartItem;
        IConfiguration _configuration;
        public CartService(ICartRepository cartRepository, IRepository<CartItem> cartItem, IConfiguration config): base(cartRepository)
        {
            _cartRepository = cartRepository;
            _cartItem = cartItem;
            _configuration = config;
        }
        public Cart AddItem(int UserId, Guid CartId, int ItemId, decimal UnitPrice, int Quantity)
        {
            try
            {
                Cart cart = _cartRepository.GetCart(CartId);
                if (cart == null)
                {
                    cart = new Cart { Id = CartId, UserId = UserId, CreatedDate = DateTime.Now, IsActive = true };
                    CartItem item = new CartItem { ItemId = ItemId, Quantity = Quantity, UnitPrice = UnitPrice, CartId = CartId };

                    cart.CartItems.Add(item);

                    _cartRepository.Add(cart);
                    _cartRepository.SaveChanges();
                }
                else
                {
                    CartItem item = cart.CartItems.Where(p => p.ItemId == ItemId).FirstOrDefault();
                    if (item != null)
                    {
                        item.Quantity += Quantity;
                        _cartItem.Update(item);
                        _cartItem.SaveChanges();
                    }
                    else
                    {
                        item = new CartItem { ItemId = ItemId, Quantity = Quantity, UnitPrice = UnitPrice, CartId = CartId };

                        cart.CartItems.Add(item);
                        _cartItem.SaveChanges();
                    }
                }
                return cart;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int DeleteItem(Guid cartId, int ItemId)
        {
           return _cartRepository.DeleteItem(cartId, ItemId);
        }

        public int GetCartCount(Guid cartId)
        {
            var cart = _cartRepository.GetCart(cartId);
            if (cart != null)
            {
                return cart.CartItems.Count;
            }
            return 0;
        }

        public CartModel GetCartDetails(Guid cartId)
        {
            var model = _cartRepository.GetCartDetails(cartId);
            if (model != null && model.Items.Count > 0)
            {
                decimal subTotal = 0;
                foreach (var item in model.Items)
                {
                    item.Total = item.UnitPrice * item.Quantity;
                    subTotal += item.Total;
                }
                model.Total = subTotal;

                model.Tax = Math.Round((model.Total * Convert.ToInt32(_configuration["Tax:GST"])) / 100, 2);
                model.GrandTotal = model.Tax + model.Total;
            }
            return model;
        }

        public int UpdateCart(Guid CartId, int UserId)
        {
            return _cartRepository.UpdateCart(CartId, UserId);
        }

        public int UpdateQuantity(Guid CartId, int Id, int Quantity)
        {
            return _cartRepository.UpdateQuantity(CartId, Id, Quantity);
        }
    }
}
