using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Core.Entities;
using OrderManagement.Core.Interfaces;
using OrderManagement.Web.ViewModels;

namespace OrderManagement.Web.Controllers
{
    [Authorize(Policy = "StaffOrAdmin")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public OrdersController(IOrderRepository repository, IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }

        // Helper method to map Order to OrderViewModel
        // CHANGE: Added to reduce code duplication in Index, Details, and Edit actions
        // REASON: Improves maintainability and consistency in ViewModel mapping
        private OrderViewModel MapToViewModel(Order order)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName,
                IsPaid = order.IsPaid,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    Id = oi.Id,
                    ProductName = oi.ProductName,
                    Quantity = oi.Quantity,
                    Price = oi.Price,
                    ImagePath = oi.ImagePath
                }).ToList() ?? new List<OrderItemViewModel>()
            };
        }

        public async Task<IActionResult> Index(string searchString, bool? isPaid)
        {
            var orders = await _repository.GetAllAsync();
            var viewModels = orders.Select(MapToViewModel); // CHANGE: Use helper method
            // REASON: Simplifies code and ensures consistent mapping

            // Apply filtering
            if (!string.IsNullOrEmpty(searchString))
            {
                viewModels = viewModels.Where(o => o.CustomerName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }
            if (isPaid.HasValue)
            {
                viewModels = viewModels.Where(o => o.IsPaid == isPaid.Value);
            }

            ViewData["SearchString"] = searchString;
            ViewData["IsPaid"] = isPaid;
            return View(viewModels);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var order = await _repository.GetByIdAsync(id.Value);
            if (order == null) return NotFound();
            var viewModel = MapToViewModel(order); // CHANGE: Use helper method
            // REASON: Simplifies code and ensures consistent mapping
            return View(viewModel);
        }

        [Authorize(Policy = "AdminOnly")]
        public IActionResult Create()
        {
            return View(new OrderViewModel());
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    OrderDate = viewModel.OrderDate,
                    CustomerName = viewModel.CustomerName,
                    IsPaid = viewModel.IsPaid,
                    OrderItems = new List<OrderItem>()
                };

                foreach (var vm in viewModel.OrderItems)
                {
                    string? imagePath = null;
                    if (vm.ImageFile != null && vm.ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                        Directory.CreateDirectory(uploadsFolder);
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await vm.ImageFile.CopyToAsync(stream);
                        }
                        imagePath = $"/Uploads/{fileName}";
                    }
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductName = vm.ProductName,
                        Quantity = vm.Quantity,
                        Price = vm.Price,
                        ImagePath = imagePath
                    });
                }

                await _repository.AddAsync(order);
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var order = await _repository.GetByIdAsync(id.Value);
            if (order == null) return NotFound();
            var viewModel = MapToViewModel(order); // CHANGE: Use helper method
            // REASON: Simplifies code and ensures consistent mapping
            return View(viewModel);
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var existingOrder = await _repository.GetByIdAsync(id);
                if (existingOrder == null) return NotFound();

                // CHANGE: Update existing order instead of creating a new one
                // REASON: Preserves existing OrderItems and their IDs, ensuring database consistency
                existingOrder.OrderDate = viewModel.OrderDate;
                existingOrder.CustomerName = viewModel.CustomerName;
                existingOrder.IsPaid = viewModel.IsPaid;

                // Update or add OrderItems
                existingOrder.OrderItems.Clear(); // Remove existing items
                foreach (var vm in viewModel.OrderItems)
                {
                    string? imagePath = vm.ImagePath;
                    if (vm.ImageFile != null && vm.ImageFile.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                        Directory.CreateDirectory(uploadsFolder);
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vm.ImageFile.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await vm.ImageFile.CopyToAsync(stream);
                        }
                        imagePath = $"/Uploads/{fileName}";
                    }
                    existingOrder.OrderItems.Add(new OrderItem
                    {
                        Id = vm.Id > 0 ? vm.Id : 0, // Preserve existing IDs
                        ProductName = vm.ProductName,
                        Quantity = vm.Quantity,
                        Price = vm.Price,
                        ImagePath = imagePath
                    });
                }

                try
                {
                    await _repository.UpdateAsync(existingOrder);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // CHANGE: Add error handling
                    // REASON: Provides feedback if update fails (e.g., database error)
                    ModelState.AddModelError("", $"Failed to update order: {ex.Message}");
                }
            }
            return View(viewModel);
        }

        // CHANGE: Removed GET Delete action
        // REASON: Modal-based deletion in Home/Index.cshtml and Orders/Index.cshtml directly calls DeleteConfirmed,
        // making the separate Delete view redundant
        /*
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var order = await _repository.GetByIdAsync(id.Value);
            if (order == null) return NotFound();
            var viewModel = MapToViewModel(order);
            return View(viewModel);
        }
        */

        [Authorize(Policy = "AdminOnly")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // CHANGE: Add order existence check and error handling
            // REASON: Prevents errors if order doesn't exist and handles database issues
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            try
            {
                await _repository.DeleteAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log the error (implement logging if needed)
                return StatusCode(500, $"Failed to delete order: {ex.Message}");
            }
        }

        [Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public IActionResult GetOrderItemRow(int index)
        {
            ViewData["Index"] = index;
            return PartialView("_OrderItem", new OrderItemViewModel());
        }
    }
}