using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using pedidosApp.Data;
using pedidosApp.Models;

namespace pedidosApp.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderItems
        public async Task<IActionResult> Index()
        {
            return View(await _context.OrderItems.ToListAsync());
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Quantity,Subtotal,OrderId,ProductId")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
                // Cal subtotal auto (Quantity x Price)
                orderItem.Subtotal = CalculateSubtotal(orderItem.ProductId, orderItem.Quantity);

                _context.Add(orderItem);
                await _context.SaveChangesAsync();

                // Act total del pedido despues de agregar item
                await UpdateOrderTotal(orderItem.OrderId);

                return RedirectToAction(nameof(Index));
            }
            return View(orderItem);
        }


        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,Subtotal,OrderId,ProductId")] OrderItem orderItem)
        {
            if (id != orderItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Recalcular subtotal al editar
                    orderItem.Subtotal = CalculateSubtotal(orderItem.ProductId, orderItem.Quantity);

                    _context.Update(orderItem);
                    await _context.SaveChangesAsync();

                    // Act total del pedido despues de editar item
                    await UpdateOrderTotal(orderItem.OrderId);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderItemExists(orderItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(orderItem);

        }
     

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                int orderId = orderItem.OrderId; // Guardar OrderId antes de eliminar
                _context.OrderItems.Remove(orderItem);
                await _context.SaveChangesAsync();

                //  Act total del pedido despues de eliminar item
                await UpdateOrderTotal(orderId);
            }
            else
            {
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool OrderItemExists(int id)
        {
            return _context.OrderItems.Any(e => e.Id == id);
        }

        // Cal subtotal (Cantidad z Precio del producto)
        private decimal CalculateSubtotal(int productId, int quantity)
        {
            var product = _context.Set<ProductModel>().Find(productId);
            if (product != null)
            {
                return quantity * product.Price;
            }
            return 0;
        }

        // Act total del pedido
        private async Task UpdateOrderTotal(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                var total = _context.OrderItems
                    .Where(oi => oi.OrderId == orderId)
                    .Sum(oi => (decimal?)oi.Subtotal) ?? 0;

                order.Total = total;
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
        }

    }
}
