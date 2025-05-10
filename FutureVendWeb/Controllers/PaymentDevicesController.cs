using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Castle.Core.Resource;

namespace FutureVendWeb.Controllers
{
    [Authorize]
    public class PaymentDevicesController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PaymentDevicesController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var devices = await _context.PaymentDevices
                .Where(d => d.UserId == user.Id)
                .ToListAsync();

            return View(devices);
        }

        // GET: PaymentDevices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentDevice = await _context.PaymentDevices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentDevice == null)
            {
                return NotFound();
            }

            return View(paymentDevice);
        }

        // GET: PaymentDevices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PaymentDevices/Create
         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentDevice device)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            device.UserId = user.Id;

            if (ModelState.IsValid)
            {
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(device);
        }

        // GET: PaymentDevices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentDevice = await _context.PaymentDevices.FindAsync(id);
            if (paymentDevice == null)
            {
                return NotFound();
            }
            return View(paymentDevice);
        }

        // POST: PaymentDevices/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Manufacturer,OSVersion,NFC,Chip")] PaymentDevice paymentDevice)
        {
            if (id != paymentDevice.Id)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            paymentDevice.UserId = user.Id;
            paymentDevice.User = user;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentDevice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentDeviceExists(paymentDevice.Id))
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
            return View(paymentDevice);
        }

        // GET: PaymentDevices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentDevice = await _context.PaymentDevices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentDevice == null)
            {
                return NotFound();
            }

            return View(paymentDevice);
        }

        // POST: PaymentDevices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentDevice = await _context.PaymentDevices.FindAsync(id);
            if (paymentDevice != null)
            {
                bool exists = await _context.Devices.AnyAsync(d => d.PaymentDeviceId == paymentDevice.Id);
                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "Този запис не може да бъде изтрит.");
                    return View(paymentDevice);
                }
                else
                {
                    _context.PaymentDevices.Remove(paymentDevice);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentDeviceExists(int id)
        {
            return _context.PaymentDevices.Any(e => e.Id == id);
        }
    }
}
