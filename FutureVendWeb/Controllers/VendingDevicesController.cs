using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FutureVendWeb.Controllers
{
    [Authorize]
    public class VendingDevicesController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VendingDevicesController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var devices = await _context.VendingDevices
                .Where(d => d.UserId == user.Id)
                .ToListAsync();

            return View(devices);
        }

        // GET: VendingDevices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendingDevice = await _context.VendingDevices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendingDevice == null)
            {
                return NotFound();
            }

            return View(vendingDevice);
        }

        // GET: VendingDevices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: VendingDevices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VendingDevice device)
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

        // GET: VendingDevices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendingDevice = await _context.VendingDevices.FindAsync(id);
            if (vendingDevice == null)
            {
                return NotFound();
            }
            return View(vendingDevice);
        }

        // POST: VendingDevices/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Model,Manufacturer,SoftwareVersion")] VendingDevice vendingDevice)
        {
            if (id != vendingDevice.Id)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            { 
                return RedirectToAction("Login", "Account");
            }

            vendingDevice.UserId = user.Id;
            vendingDevice.User = user;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vendingDevice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VendingDeviceExists(vendingDevice.Id))
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
            return View(vendingDevice);
        }

        // GET: VendingDevices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vendingDevice = await _context.VendingDevices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vendingDevice == null)
            {
                return NotFound();
            }

            return View(vendingDevice);
        }

        // POST: VendingDevices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vendingDevice = await _context.VendingDevices.FindAsync(id);
            if (vendingDevice != null)
            {
                bool exists = await _context.Devices.AnyAsync(d => d.VendingDeviceId == vendingDevice.Id);
                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "Този запис не може да бъде изтрит.");
                    return View(vendingDevice);
                }
                else
                {
                    _context.VendingDevices.Remove(vendingDevice);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VendingDeviceExists(int id)
        {
            return _context.VendingDevices.Any(e => e.Id == id);
        }
    }
}
