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
    public class CustomersController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomersController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var customers = await _context.Customers
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            return View(customers);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyName,FirstName,LastName,Address,City,PostCode,Country,Phone,Email,TaxNumber")] Customer customer)
        {
            // Получаване на логнатия потребител
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                // Ако няма логнат потребител, пренасочваме към страницата за логин
                return RedirectToAction("Login", "Account");
            }

            // Задаване на UserId за новия клиент
            customer.UserId = user.Id;
            // Задаване на навигационното поле User
            customer.User = user;

            // Проверка дали съществува клиент с този UserId и TaxNumber
            if (_context.Customers.Any(c => c.TaxNumber == customer.TaxNumber))
            {
                // Ако има дублиран запис
                ModelState.AddModelError(string.Empty, "Запис с този TaxNumber вече съществува.");
                return View(customer);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Добавяме клиента към контекста
                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index)); // Пренасочваме към списъка с клиенти
                }
                catch (DbUpdateException ex)
                {
                    // Ако се появи грешка при записване в базата, показваме съобщение
                    ModelState.AddModelError(string.Empty, "Не може да се запише клиентът. Моля, опитайте отново.");
                }
            }

            // Ако има грешки, връщаме потребителя обратно на формата за създаване
            return View(customer);
        }
    

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CompanyName,FirstName,LastName,Address,City,PostCode,Country,Phone,Email,TaxNumber")] Customer customer)
        {
            if (id != customer.Id)
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
            customer.UserId = user.Id;
            // Задаване на навигационното поле User
            customer.User = user;

            // Проверка дали съществува клиент с този UserId и TaxNumber
            if (_context.Customers.Any(c => c.TaxNumber == customer.TaxNumber && c.Id != id))
            {
                // Ако има дублиран запис
                ModelState.AddModelError(string.Empty, "Запис с този TaxNumber вече съществува.");
                return View(customer);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
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
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
