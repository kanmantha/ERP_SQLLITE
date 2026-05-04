using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ERP.Web.Data;
using ERP.Web.Models;
using System;
using System.Linq;

namespace ERP.Web.Controllers
{
    /// <summary>
    /// Controller for managing Invoices.
    /// </summary>
    [Authorize]
    public class InvoicesController : Controller
    {
        private readonly ERPDbContext _context;

        public InvoicesController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: Invoices
        public async Task<IActionResult> Index()
        {
            return View(await _context.Invoices
                .Include(i => i.Customer)
                .ToListAsync());
        }

        // GET: Invoices/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name");
            ViewData["OrderId"] = new SelectList(_context.Orders
                .Where(o => !_context.Invoices.Any(i => i.OrderId == o.Id))
                .Select(o => new { o.Id, o.Total, o.Customer.Name }), "Id", "Name");
            
            return View(new Invoice { InvoiceDate = DateTime.Now, DueDate = DateTime.Now.AddDays(30) });
        }

        // POST: Invoices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice, string invoiceDateString, string dueDateString)
        {
            // Parse Dates
            if (DateTime.TryParse(invoiceDateString, out DateTime invDate)) invoice.InvoiceDate = invDate;
            if (DateTime.TryParse(dueDateString, out DateTime dueDate)) invoice.DueDate = dueDate;

            // Validation
            if (ModelState.IsValid)
            {
                // If Order is selected, populate Amount and CustomerId from Order
                if (invoice.OrderId.HasValue)
                {
                    var order = await _context.Orders.FindAsync(invoice.OrderId.Value);
                    if (order != null)
                    {
                        invoice.Amount = order.Total;
                        invoice.CustomerId = order.CustomerId;
                        
                        // Generate simple Invoice No
                        var count = await _context.Invoices.CountAsync() + 1;
                        invoice.InvoiceNo = $"INV-{count:D4}";
                    }
                }

                _context.Add(invoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Name", invoice.CustomerId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", invoice.OrderId);
            return View(invoice);
        }

        // GET: Invoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var invoice = await _context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        // POST: Invoices/MarkPaid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                invoice.Status = "Paid";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Invoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var invoice = await _context.Invoices.FirstOrDefaultAsync(m => m.Id == id);
            if (invoice == null) return NotFound();
            return View(invoice);
        }

        // POST: Invoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice != null)
            {
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
