using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FutureVendWeb.Controllers
{
    /// <summary>
    /// Handles operations related to vending machine transactions,
    /// including listing, viewing details, creating via stored procedure, and deleting transactions.
    /// </summary>
    public class TransactionsController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionsController"/> class.
        /// </summary>
        public TransactionsController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Displays all transactions associated with the currently logged-in user.
        /// </summary>
        /// <returns>A view showing the list of transactions.</returns>
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var transactions = await _context.Transactions
                .Include(t => t.Device)
                    .ThenInclude(d => d.PaymentDevice)
                .Include(t => t.Device)
                    .ThenInclude(d => d.Customer)
                .Include(t => t.VendingProduct)
                .Where(t => t.Device.UserId == userId)
                .ToListAsync();

            return View(transactions);
        }

        /// <summary>
        /// Shows detailed information about a specific transaction.
        /// </summary>
        /// <param name="id">The ID of the transaction.</param>
        /// <returns>A view with transaction details, or 404 if not found.</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Device)
                    .ThenInclude(d => d.PaymentDevice)
                .Include(t => t.Device)
                    .ThenInclude(d => d.Customer)
                .Include(t => t.VendingProduct)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        /// <summary>
        /// Adds a new transaction using a stored procedure.
        /// This endpoint is intended for external integration via HTTP POST.
        /// </summary>
        /// <param name="model">The transaction data provided in the request body.</param>
        /// <returns>A success message or an error response.</returns>
        [HttpPost]
        [Route("api/transactions/add")]
        public async Task<IActionResult> AddTransactionViaProcedure([FromBody] TransactionRequestModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var conn = _context.Database.GetDbConnection();
                await conn.OpenAsync();

                using var command = conn.CreateCommand();
                command.CommandText = "insert_transaction";
                command.CommandType = System.Data.CommandType.StoredProcedure;

                command.Parameters.Add(new MySqlParameter("payment_type", model.PaymentType));
                command.Parameters.Add(new MySqlParameter("amount", model.Amount));
                command.Parameters.Add(new MySqlParameter("item_number", model.ItemNumber));
                command.Parameters.Add(new MySqlParameter("currency_code", model.Currency));
                command.Parameters.Add(new MySqlParameter("serial_number", model.SerialNumber));
                command.Parameters.Add(new MySqlParameter("created_at", model.CreatedAt));

                await command.ExecuteNonQueryAsync();

                return Ok(new { message = "Transaction added successfully via procedure." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Displays a confirmation page for deleting a specific transaction.
        /// </summary>
        /// <param name="id">The ID of the transaction.</param>
        /// <returns>A view with transaction details or a 404 error if not found.</returns>
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Device)
                    .ThenInclude(d => d.PaymentDevice)
                .Include(t => t.Device)
                    .ThenInclude(d => d.Customer)
                .Include(t => t.VendingProduct)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        /// <summary>
        /// Permanently deletes a transaction from the database.
        /// </summary>
        /// <param name="id">The ID of the transaction to delete.</param>
        /// <returns>A redirect to the Index view after deletion.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

