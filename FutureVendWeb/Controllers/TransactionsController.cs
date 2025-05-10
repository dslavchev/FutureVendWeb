using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Models;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FutureVendWeb.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TransactionsController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: Transactions
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var transactions = await _context.Transactions
                .Include(t => t.Device)
                .Include(t => t.VendingProduct)
                .Where(t => t.Device.UserId == userId)
                .ToListAsync();

            return View(transactions);
        }

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
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var transaction = await _context.Transactions
                .Include(t => t.Device)
                .Include(t => t.VendingProduct)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (transaction == null) return NotFound();

            return View(transaction);
        }

        // POST: Transactions/Delete/5
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


    /*
    public class TransactionsController : Controller
    {
        private readonly VendingDbContext _context;

        public TransactionsController(VendingDbContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var vendingDbContext = _context.Transactions.Include(t => t.Device).Include(t => t.VendingProduct);
            return View(await vendingDbContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Device)
                .Include(t => t.VendingProduct)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["DeviceId"] = new SelectList(_context.Devices, "Id", "Id");
            ViewData["VendingProductId"] = new SelectList(_context.VendingProducts, "Id", "Id");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,Currency,CreatedAt,DeviceId,VendingProductId,PaymentType")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeviceId"] = new SelectList(_context.Devices, "Id", "Id", transaction.DeviceId);
            ViewData["VendingProductId"] = new SelectList(_context.VendingProducts, "Id", "Id", transaction.VendingProductId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["DeviceId"] = new SelectList(_context.Devices, "Id", "Id", transaction.DeviceId);
            ViewData["VendingProductId"] = new SelectList(_context.VendingProducts, "Id", "Id", transaction.VendingProductId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,Currency,CreatedAt,DeviceId,VendingProductId,PaymentType")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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
            ViewData["DeviceId"] = new SelectList(_context.Devices, "Id", "Id", transaction.DeviceId);
            ViewData["VendingProductId"] = new SelectList(_context.VendingProducts, "Id", "Id", transaction.VendingProductId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Device)
                .Include(t => t.VendingProduct)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
    }
    */
}
