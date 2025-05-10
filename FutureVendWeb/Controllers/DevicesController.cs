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
    public class DevicesController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public DevicesController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private void SetViewBag(ApplicationUser user)
        {
            ViewBag.CustomerId = new SelectList(_context.Customers
            .Select(c => new
            {
                Id = c.Id,
                UserId = c.UserId,
                FullInfo = c.CompanyName + " - " + c.FirstName + " " + c.LastName
            }).Where(c => c.UserId == user.Id), "Id", "FullInfo");


            ViewBag.PaymentDeviceId = new SelectList(_context.PaymentDevices
                .Select(p => new {
                    Id = p.Id,
                    UserId = p.UserId,
                    Info = p.Name + " - " + p.Manufacturer
                }).Where(p => p.UserId == user.Id), "Id", "Info");

            ViewBag.VendingDeviceId = new SelectList(_context.VendingDevices
                .Select(v => new {
                    Id = v.Id,
                    UserId = v.UserId,
                    Info = v.Model + " - " + v.Manufacturer
                }).Where(v => v.UserId == user.Id), "Id", "Info");
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var devices = await _context.Devices
                .Include(d => d.Customer)
                .Include(d => d.VendingDevice)
                .Include(d => d.PaymentDevice)
                .Where(d => d.UserId == user.Id)
                .ToListAsync();

            return View(devices);
        }

        // GET: Devices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Customer)
                .Include(d => d.PaymentDevice)
                .Include(d => d.VendingDevice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // GET: Devices/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            SetViewBag(user);
            return View();
        }

        // POST: Devices/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PaymentDeviceSerial,VendingDeviceSerial,PaymentDeviceId,VendingDeviceId,CustomerId,AcceptCard,AcceptCash,LocationLat,LocationLon")] Device device)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            device.UserId = user.Id;
            device.User = user;

            // Check if a device with this VendingDeviceSerial , PaymentDeviceSerial already exists

            if (_context.Devices.Any(c => c.VendingDeviceSerial == device.VendingDeviceSerial ))
            {
                ModelState.AddModelError(string.Empty, "Запис с този VendingDeviceSerial вече съществува.");
                SetViewBag(user);
                return View(device);
            }
            if (_context.Devices.Any(c => c.PaymentDeviceSerial == device.PaymentDeviceSerial))
            {
                ModelState.AddModelError(string.Empty, "Запис с този PaymentDeviceSerial вече съществува.");
                SetViewBag(user);
                return View(device);
            }

            if (ModelState.IsValid)
            {
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers.Where(c => c.UserId == user.Id), "Id", "CompanyName");
            ViewData["VendingDeviceId"] = new SelectList(_context.VendingDevices.Where(v => v.UserId == user.Id), "Id", "Model");
            ViewData["PaymentDeviceId"] = new SelectList(_context.PaymentDevices.Where(p => p.UserId == user.Id), "Id", "SerialNumber");

            return View(device);
        }

        // GET: Devices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices.FindAsync(id);
            if (device == null)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            SetViewBag(user);
            return View(device);
        }

        // POST: Devices/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PaymentDeviceSerial,VendingDeviceSerial,PaymentDeviceId,VendingDeviceId,CustomerId,AcceptCard,AcceptCash,LocationLat,LocationLon")] Device device)
        {
            if (id != device.Id)
            {
                return NotFound();
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Задай UserId и навигационното поле
            device.UserId = user.Id;
            device.User = user;


            if (_context.Devices.Any(c => c.VendingDeviceSerial == device.VendingDeviceSerial && c.Id != id))
            {
                ModelState.AddModelError(string.Empty, "Запис с този VendingDeviceSerial вече съществува.");
                SetViewBag(user);
                return View(device);
            }
            if (_context.Devices.Any(c => c.PaymentDeviceSerial == device.PaymentDeviceSerial && c.Id != id))
            {
                ModelState.AddModelError(string.Empty, "Запис с този PaymentDeviceSerial вече съществува.");
                SetViewBag(user);
                return View(device);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(device);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DeviceExists(device.Id))
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
            //ViewData["CustomerId"] = new SelectList(_context.Customers, "Id", "Id", device.CustomerId);
            //ViewData["PaymentDeviceId"] = new SelectList(_context.PaymentDevices, "Id", "Id", device.PaymentDeviceId);
            //ViewData["VendingDeviceId"] = new SelectList(_context.VendingDevices, "Id", "Id", device.VendingDeviceId);
            return View(device);
        }

        // GET: Devices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var device = await _context.Devices
                .Include(d => d.Customer)
                .Include(d => d.PaymentDevice)
                .Include(d => d.VendingDevice)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (device == null)
            {
                return NotFound();
            }

            return View(device);
        }

        // POST: Devices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var device = await _context.Devices.FindAsync(id);
            if (device != null)
            {
                bool exists = await _context.Transactions.AnyAsync(t => t.DeviceId == device.Id);
                if (exists)
                {
                    ModelState.AddModelError(string.Empty, "Този запис не може да бъде изтрит.");
                    return View(device);
                }
                else
                {
                    _context.Devices.Remove(device);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.Id == id);
        }
    }
}
