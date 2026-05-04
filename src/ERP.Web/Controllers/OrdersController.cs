using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ERP.Web.Data;
using ERP.Web.Models;
using System;

namespace ERP.Web.Controllers
{
    /// <summary>
    /// Controller for managing Order entities.
    /// Provides Create, Read, Update, and Delete (CRUD) operations.
    /// </summary>
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ERPDbContext _context;

        public OrdersController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: Orders - List all orders with customer info
        public async Task<IActionResult> Index()
        {
            var orders = _context.Orders.Include(o => o.Customer);
            return View(await orders.ToListAsync());
        }

        // GET: Orders/Details/5 - View order details and its items
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Orders/Create - Form to create a new order
        public IActionResult Create()
        {
            // Populate dropdown with customers
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            // Set default date to now so the browser knows the expected format
            return View(new Order { OrderDate = DateTime.Now });
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order, string orderDateString)
        {
            // 1. Handle Date Parsing manually to avoid culture/format binding issues
            if (!string.IsNullOrWhiteSpace(orderDateString))
            {
                if (DateTime.TryParse(orderDateString, out DateTime parsedDate))
                {
                    order.OrderDate = parsedDate;
                }
                else
                {
                    ModelState.AddModelError("OrderDate", "Invalid Date Format. Please use the date picker.");
                }
            }
            else
            {
                // Default to now if no date provided
                order.OrderDate = DateTime.Now;
            }

            // 2. Validate Model State
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                ViewBag.Error = string.Join("; ", errors);
                ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
                return View(order);
            }

            // 3. Initialize Order Total
            order.Total = 0;

            // 4. Save to Database
            try
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Capture DB errors (e.g., Foreign Key constraints)
                ViewBag.Error = "Database Error: " + (ex.InnerException?.Message ?? ex.Message);
                ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
                return View(order);
            }
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CustomerId,OrderDate")] Order order)
        {
            if (id != order.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", order.CustomerId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders.Include(o => o.Customer).FirstOrDefaultAsync(m => m.Id == id);
            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.Include(o => o.OrderItems).FirstOrDefaultAsync(m => m.Id == id);
            if (order != null)
            {
                // Remove associated items first to avoid FK constraint issues
                _context.OrderItems.RemoveRange(order.OrderItems);
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
