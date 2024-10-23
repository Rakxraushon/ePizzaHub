using ePizzaHub.Core.Entities;
using ePizzaHub.Models;
using ePizzaHub.Repositories.Interfaces;
using ePizzaHub.Services.Interfaces;

namespace ePizzaHub.Services.Implementations
{
    public class ItemService : Service<Item>, IItemService
    {
        IRepository<Item> _itemRepository;  
        public ItemService(IRepository<Item> itemRepo): base(itemRepo)
        {
            _itemRepository = itemRepo;
        }

        public IEnumerable<ItemModel> GetItems()
        {
            var data = _itemRepository.GetAll().OrderBy(item=>item.CategoryId).Select(i=>new ItemModel
            {
                Id = i.Id,
                Name = i.Name,
                CategoryId = i.CategoryId,
                Description = i.Description,
                ImageUrl = i.ImageUrl,
                ItemTypeId = i.ItemTypeId,
                UnitPrice = i.UnitPrice
            });
            return data;
        }
    }
}
