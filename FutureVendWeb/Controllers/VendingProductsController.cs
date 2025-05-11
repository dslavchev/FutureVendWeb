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
    /// Controller for managing vending products in the application.
    /// All actions in this controller require the user to be authenticated.
    /// </summary>
    [Authorize]
    public class VendingProductsController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="VendingProductsController"/> class.
        /// </summary>
        /// <param name="context">The database context used to interact with the data.</param>
        /// <param name="userManager">The user manager for handling user-related tasks.</param>
        public VendingProductsController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays a list of vending products associated with the logged-in user.
        /// </summary>
        /// <returns>A view with the list of vending products.</returns>
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var products = await _context.VendingProducts
                .Where(v => v.UserId == user.Id)
                .ToListAsync();

            return View(products);
        }

        /// <summary>
        /// Displays the details of a specific vending product.
        /// </summary>
        /// <param name="id">The ID of the vending product.</param>
        /// <returns>A view with the details of the vending product.</returns>
        public async Task<IActionResult> Details(int? id)
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

        /// <summary>
        /// Displays the form for creating a new vending product.
        /// </summary>
        /// <returns>A view with the form for creating a vending product.</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the submission of a new vending product.
        /// </summary>
        /// <param name="vendingProduct">The vending product to be created.</param>
        /// <returns>Redirects to the index view if successful, otherwise shows validation errors.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendingProduct vendingProduct)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            vendingProduct.UserId = user.Id;
            vendingProduct.User = user;

            // Check if a product with the same PLU already exists
            if (_context.VendingProducts.Any(c => c.PLU == vendingProduct.PLU && c.Id != vendingProduct.Id))
            {
                ModelState.AddModelError(string.Empty, "A record with this PLU already exists.");
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

        /// <summary>
        /// Displays the form for editing an existing vending product.
        /// </summary>
        /// <param name="id">The ID of the vending product to be edited.</param>
        /// <returns>A view with the form for editing the vending product.</returns>
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

        /// <summary>
        /// Handles the submission of edits to an existing vending product.
        /// </summary>
        /// <param name="id">The ID of the vending product to be updated.</param>
        /// <param name="vendingProduct">The updated vending product data.</param>
        /// <returns>Redirects to the index view if successful, otherwise shows validation errors.</returns>
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
                return RedirectToAction("Login", "Account");
            }

            vendingProduct.UserId = user.Id;
            vendingProduct.User = user;

            // Check if a product with the same PLU already exists
            if (_context.VendingProducts.Any(c => c.PLU == vendingProduct.PLU && c.Id != id && c.UserId == user.Id))
            {
                ModelState.AddModelError(string.Empty, "A record with this PLU already exists.");
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

        /// <summary>
        /// Displays the confirmation page for deleting a vending product.
        /// </summary>
        /// <param name="id">The ID of the vending product to be deleted.</param>
        /// <returns>A view to confirm the deletion of the vending product.</returns>
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

        /// <summary>
        /// Handles the deletion of a vending product.
        /// </summary>
        /// <param name="id">The ID of the vending product to be deleted.</param>
        /// <returns>Redirects to the index view after successful deletion.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendingProduct = await _context.VendingProducts.FindAsync(id);
            if (vendingProduct != null)
            {
                bool exists = await _context.Transactions.AnyAsync(t => t.VendingProductId == vendingProduct.Id);
                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "This record cannot be deleted.");
                    return View(vendingProduct);
                }
                else
                {
                    _context.VendingProducts.Remove(vendingProduct);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checks if a vending product exists in the database.
        /// </summary>
        /// <param name="id">The ID of the vending product.</param>
        /// <returns>True if the vending product exists, otherwise false.</returns>
        private bool VendingProductExists(int id)
        {
            return _context.VendingProducts.Any(e => e.Id == id);
        }
    }
}
