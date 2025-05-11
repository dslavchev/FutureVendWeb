using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FutureVendWeb.Controllers
{
    /// <summary>
    /// Controller for managing customer records.
    /// </summary>
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomersController"/> class.
        /// </summary>
        public CustomersController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays a list of customers belonging to the current user.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var customers = await _context.Customers
                .Where(c => c.UserId == user.Id)
                .ToListAsync();

            return View(customers);
        }

        /// <summary>
        /// Displays details of a specific customer.
        /// </summary>
        /// <param name="id">The ID of the customer to view.</param>
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

        /// <summary>
        /// Returns the view to create a new customer.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the POST request to create a new customer.
        /// </summary>
        /// <param name="customer">The customer to create.</param>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CompanyName,FirstName,LastName,Address,City,PostCode,Country,Phone,Email,TaxNumber")] Customer customer)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            customer.UserId = user.Id;
            customer.User = user;

            if (_context.Customers.Any(c => c.TaxNumber == customer.TaxNumber))
            {
                ModelState.AddModelError(string.Empty, "A customer with this TaxNumber already exists.");
                return View(customer);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(customer);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save the customer. Please try again.");
                }
            }
            return View(customer);
        }

        /// <summary>
        /// Returns the view to edit a specific customer.
        /// </summary>
        /// <param name="id">The ID of the customer to edit.</param>
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

        /// <summary>
        /// Handles the POST request to update a customer.
        /// </summary>
        /// <param name="id">The ID of the customer.</param>
        /// <param name="customer">The updated customer data.</param>
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
                return RedirectToAction("Login", "Account");
            }

            customer.UserId = user.Id;
            customer.User = user;

            if (_context.Customers.Any(c => c.TaxNumber == customer.TaxNumber && c.Id != id))
            {
                ModelState.AddModelError(string.Empty, "A customer with this TaxNumber already exists.");
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

        /// <summary>
        /// Displays the confirmation page for deleting a customer.
        /// </summary>
        /// <param name="id">The ID of the customer to delete.</param>
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

        /// <summary>
        /// Handles the POST request to delete a customer.
        /// </summary>
        /// <param name="id">The ID of the customer to delete.</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                bool exists = await _context.Devices.AnyAsync(d => d.CustomerId == customer.Id);
                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "This customer cannot be deleted because it is linked to existing devices.");
                    return View(customer);
                }
                else
                {
                    _context.Customers.Remove(customer);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a customer with the given ID exists.
        /// </summary>
        /// <param name="id">The ID of the customer.</param>
        /// <returns>True if the customer exists; otherwise, false.</returns>
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
