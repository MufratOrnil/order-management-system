using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;
using OrderManagement.Web.Models;

namespace OrderManagement.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IOrderRepository _orderRepository;

        public HomeController(ILogger<HomeController> logger, IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.GetAllAsync() ?? new List<Order>();
            var viewModel = new
            {
                TotalOrders = orders.Count(),
                PaidOrders = orders.Count(o => o.IsPaid),
                PendingOrders = orders.Count(o => !o.IsPaid),
                RecentOrders = orders.OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .Select(o => new
                    {
                        o.Id,
                        o.OrderDate,
                        o.CustomerName,
                        o.IsPaid,
                        ItemCount = o.OrderItems?.Count ?? 0,
                        FirstItemImage = o.OrderItems?.FirstOrDefault()?.ImagePath
                    }).ToList()
            };
            return View(viewModel);
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