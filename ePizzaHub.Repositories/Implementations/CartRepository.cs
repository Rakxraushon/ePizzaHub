using ePizzaHub.Core;
using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace ePizzaHub.Repositories.Implementations
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {

        public CartRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }
        public int DeleteItem(Guid cartId, int itemId)
        {
            var cart = _db.Carts.Include(c => c.CartItems).Where(c => c.Id == cartId && c.IsActive == true).FirstOrDefault();
            if (cart != null)
            {
                var cartItem = cart.CartItems.Where(ci => ci.ItemId == itemId).FirstOrDefault();
                if (cartItem != null)
                {
                    _db.CartItems.Remove(cartItem);
                    return _db.SaveChanges();
                }
            }
            return 0;
        }

        public Cart GetCart(Guid CartId)
        {
            return _db.Carts.Include(c => c.CartItems).Where(c => c.Id == CartId && c.IsActive == true).FirstOrDefault();
        }

        public CartModel GetCartDetails(Guid CartId)
        {
            var model = (from c in _db.Carts
                         where c.Id == CartId && c.IsActive == true
                         select new CartModel
                         {
                             Id = c.Id,
                             UserId = c.UserId,
                             CreatedDate = c.CreatedDate,
                             Items = (from cartItem in _db.CartItems
                                      join item in _db.Items
                                      on cartItem.ItemId equals item.Id
                                      where cartItem.CartId == CartId
                                      select new ItemModel
                                      {
                                          Id = cartItem.Id,
                                          Quantity = cartItem.Quantity,
                                          UnitPrice = cartItem.UnitPrice,
                                          ItemId = item.Id,
                                          Name = item.Name,
                                          Description = item.Description,
                                          ImageUrl = item.ImageUrl
                                      }).ToList()
                         }).FirstOrDefault();
            return model;
        }

        public int UpdateCart(Guid cartId, int userId)
        {
            Cart cart = _db.Carts.Find(cartId);
            cart.UserId = userId;
            return _db.SaveChanges();
        }

        public int UpdateQuantity(Guid cartId, int Id, int Quantity)
        {
            var cart = _db.Carts.Include(c => c.CartItems).Where(c => c.Id == cartId && c.IsActive == true).FirstOrDefault();
            if (cart != null)
            {
                var cartItem = cart.CartItems.Where(ci => ci.Id == Id).FirstOrDefault();
                if (cartItem != null)
                {
                    cartItem.Quantity += Quantity;
                    return _db.SaveChanges();
                }
            }
            return 0;
        }
    }
}
