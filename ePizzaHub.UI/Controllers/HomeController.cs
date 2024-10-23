using ePizzaHub.Models;
using ePizzaHub.Services.Interfaces;
using ePizzaHub.UI.Helpers;
using ePizzaHub.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using System.Diagnostics;

namespace ePizzaHub.UI.Controllers
{
    public class HomeController : Controller
    {
        IItemService _itemService;
        private IMemoryCache _cache;
        private IDistributedCache _cacheRedis;
        public HomeController(IItemService itemService, IMemoryCache cache, IDistributedCache cacheRedis)
        {
            _itemService = itemService;
            _cache = cache;
            _cacheRedis = cacheRedis;
        }

        public IActionResult Index()
        {
            try
            {
                //key-value pair
                string cacheKey = "Items";

                _cache.Remove(cacheKey); //clear cache

                //memory cache
                var items = _cache.GetOrCreate(cacheKey, entry =>
                {
                    entry.AbsoluteExpiration = DateTime.Now.AddMinutes(15);
                    entry.SlidingExpiration = TimeSpan.FromMinutes(15);
                    var data = _itemService.GetItems().ToList(); //immediate execution
                    return data;
                });

                //distributed cache: redis
                //var items = _cacheRedis.GetRecordAsync<IEnumerable<ItemModel>>(cacheKey).Result;
                //if (items == null)
                //{
                //    var data = _itemService.GetItems();
                //    _cacheRedis.SetRecordAsync(cacheKey, data).Wait();
                //}

                ////var items = _itemService.GetItems();
                
                return View(items);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
