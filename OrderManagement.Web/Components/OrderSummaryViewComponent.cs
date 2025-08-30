using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Web.ViewModels;

namespace OrderManagement.Web.Components
{
    public class OrderSummaryViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(OrderViewModel order)
        {
            var total = order.OrderItems.Sum(oi => oi.Quantity * oi.Price);
            ViewBag.Total = total;
            ViewBag.IsPaid = order.IsPaid;
            return View(order);
        }
    }
}