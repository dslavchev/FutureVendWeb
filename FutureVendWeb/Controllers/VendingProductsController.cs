using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FutureVendWeb.Controllers
{
    [Authorize]
    public class VendingProductsController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VendingProductsController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var products = await _context.VendingProducts
                .Where(v => v.UserId == user.Id)
                .ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendingProduct vendingProduct)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Ако няма логнат потребител, пренасочваме към страницата за логин
                return RedirectToAction("Login", "Account");
            }

            // Задаване на UserId за новия клиент
            vendingProduct.UserId = user.Id;
            // Задаване на навигационното поле User
            vendingProduct.User = user;

            if (_context.VendingProducts.Any(c => c.PLU == vendingProduct.PLU && c.Id == vendingProduct.Id))
            {
                // Ако има дублиран запис
                ModelState.AddModelError(string.Empty, "Запис с този PlU вече съществува.");
                return View(vendingProduct);
            }
            if (ModelState.IsValid)
            {
                _context.Add(vendingProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vendingProduct);
        }

        // GET: VendingProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendingProduct = await _context.VendingProducts.FindAsync(id);
            if (vendingProduct == null)
            {
                return NotFound();
            }
            return View(vendingProduct);
        }

        // POST: VendingProducts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PLU,Name,Description,Category")] VendingProduct vendingProduct)
        {
            if (id != vendingProduct.Id)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Ако няма логнат потребител, пренасочваме към страницата за логин
                return RedirectToAction("Login", "Account");
            }

            // Задаване на UserId за новия клиент
            vendingProduct.UserId = user.Id;
            // Задаване на навигационното поле User
            vendingProduct.User = user;

            if (_context.VendingProducts.Any(c => c.PLU == vendingProduct.PLU && c.Id != id && c.UserId == user.Id))
            {
                // Ако има дублиран запис
                ModelState.AddModelError(string.Empty, "Запис с този PLU вече съществува.");
                return View(vendingProduct);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendingProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendingProductExists(vendingProduct.Id))
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
            return View(vendingProduct);
        }

        // GET: VendingProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendingProduct = await _context.VendingProducts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendingProduct == null)
            {
                return NotFound();
            }

            return View(vendingProduct);
        }

        // POST: VendingProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendingProduct = await _context.VendingProducts.FindAsync(id);
            if (vendingProduct != null)
            {
                _context.VendingProducts.Remove(vendingProduct);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendingProductExists(int id)
        {
            return _context.VendingProducts.Any(e => e.Id == id);
        }
    }
}
