using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ERP.Web.Data;
using ERP.Web.Models;
using System;
using System.Linq;

namespace ERP.Web.Controllers
{
    /// <summary>
    /// Controller for managing Inventory and Stock Movements.
    /// </summary>
    [Authorize]
    public class InventoryController : Controller
    {
        private readonly ERPDbContext _context;

        public InventoryController(ERPDbContext context)
        {
            _context = context;
        }

        // GET: Inventory - List all products with current stock levels
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Inventory/Movements - List stock history
        public async Task<IActionResult> Movements()
        {
            return View(await _context.StockMovements
                .Include(s => s.Product)
                .OrderByDescending(s => s.Date)
                .ToListAsync());
        }

        // GET: Inventory/AddMovement
        public IActionResult AddMovement()
        {
            ViewData["ProductId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Products, "Id", "Name");
            return View(new StockMovement { Date = DateTime.Now });
        }

        // POST: Inventory/AddMovement
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovement(StockMovement movement, string movementDateString)
        {
            if (!string.IsNullOrWhiteSpace(movementDateString))
            {
                if (DateTime.TryParse(movementDateString, out DateTime parsedDate))
                {
                    movement.Date = parsedDate;
                }
                else
                {
                    ModelState.AddModelError("Date", "Invalid date format");
                }
            }

            if (ModelState.IsValid)
            {
                // Update actual Product Stock
                var product = await _context.Products.FindAsync(movement.ProductId);
                if (product != null)
                {
                    if (movement.Type == "Out" || movement.Type == "Adjustment" && movement.Quantity < 0)
                    {
                        // For Out, ensure quantity is subtracted. 
                        // If user enters positive 5 for Out, we store 5 but subtract 5.
                        // Let's store signed integers in DB: In is +, Out is -.
                        
                        int qty = movement.Quantity;
                        if (movement.Type == "Out") qty = Math.Abs(qty) * -1;
                        if (movement.Type == "Adjustment") qty = qty; // Keep as is (signed)
                        if (movement.Type == "In") qty = Math.Abs(qty);

                        movement.Quantity = qty;
                        product.Stock += qty;
                    }
                    
                    _context.StockMovements.Add(movement);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewData["ProductId"] = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(_context.Products, "Id", "Name");
            return View(movement);
        }
    }
}
