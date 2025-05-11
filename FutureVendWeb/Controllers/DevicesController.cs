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
    /// <summary>
    /// The DevicesController class manages the CRUD operations for devices in the application.
    /// It handles the creation, editing, deletion, and viewing of device records, with proper validation checks
    /// and user authorization. It also manages the setting of ViewBag data for associated entities like Customer,
    /// VendingDevice, and PaymentDevice based on the authenticated user's ID.
    /// </summary>
    [Authorize]
    public class DevicesController : Controller
    {
        private readonly VendingDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DevicesController"/> class with the specified database context and user manager.
        /// </summary>
        /// <param name="context">The database context for accessing data.</param>
        /// <param name="userManager">The user manager for managing user-related operations.</param>
        public DevicesController(VendingDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Sets the ViewBag properties for Customer, PaymentDevice, and VendingDevice select lists
        /// based on the provided user information.
        /// </summary>
        /// <param name="user">The authenticated user whose related data is used for ViewBag lists.</param>
        private void SetViewBag(ApplicationUser user)
        {
            ViewBag.CustomerId = new SelectList(_context.Customers
                .Select(c => new { Id = c.Id, UserId = c.UserId, FullInfo = $"{c.CompanyName} - {c.FirstName} {c.LastName} - {c.TaxNumber}" })
                .Where(c => c.UserId == user.Id), "Id", "FullInfo");

            ViewBag.PaymentDeviceId = new SelectList(_context.PaymentDevices
                .Select(p => new { Id = p.Id, UserId = p.UserId, Info = $"{p.Name} - {p.Manufacturer} - {p.OSVersion}" })
                .Where(p => p.UserId == user.Id), "Id", "Info");

            ViewBag.VendingDeviceId = new SelectList(_context.VendingDevices
                .Select(v => new { Id = v.Id, UserId = v.UserId, Info = $"{v.Model} - {v.Manufacturer} - {v.SoftwareVersion}" })
                .Where(v => v.UserId == user.Id), "Id", "Info");
        }

        /// <summary>
        /// Displays a list of devices associated with the authenticated user.
        /// </summary>
        /// <returns>The view displaying the list of devices.</returns>
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

        /// <summary>
        /// Displays detailed information about a specific device.
        /// </summary>
        /// <param name="id">The ID of the device to display.</param>
        /// <returns>The view displaying the device details.</returns>
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

        /// <summary>
        /// Displays the device creation form.
        /// </summary>
        /// <returns>The view displaying the device creation form.</returns>
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            SetViewBag(user);
            return View();
        }

        /// <summary>
        /// Handles the creation of a new device and saves it to the database.
        /// Checks if a device with the same serial numbers already exists.
        /// </summary>
        /// <param name="device">The device object to create.</param>
        /// <returns>Redirects to the device index page or redisplays the creation form with validation errors.</returns>
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

            // Check if a device with the same serial number already exists
            if (_context.Devices.Any(c => c.VendingDeviceSerial == device.VendingDeviceSerial))
            {
                ModelState.AddModelError(string.Empty, "A device with this VendingDeviceSerial already exists.");
                SetViewBag(user);
                return View(device);
            }
            if (_context.Devices.Any(c => c.PaymentDeviceSerial == device.PaymentDeviceSerial))
            {
                ModelState.AddModelError(string.Empty, "A device with this PaymentDeviceSerial already exists.");
                SetViewBag(user);
                return View(device);
            }

            if (ModelState.IsValid)
            {
                _context.Add(device);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            SetViewBag(user);
            return View(device);
        }

        /// <summary>
        /// Displays the device editing form for a specific device.
        /// </summary>
        /// <param name="id">The ID of the device to edit.</param>
        /// <returns>The view displaying the device editing form.</returns>
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
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            SetViewBag(user);
            return View(device);
        }

        /// <summary>
        /// Handles the update of an existing device and saves the changes to the database.
        /// </summary>
        /// <param name="id">The ID of the device to update.</param>
        /// <param name="device">The updated device object.</param>
        /// <returns>Redirects to the device index page or redisplays the edit form with validation errors.</returns>
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

            device.UserId = user.Id;
            device.User = user;

            // Check for duplicate serial numbers excluding the current device
            if (_context.Devices.Any(c => c.VendingDeviceSerial == device.VendingDeviceSerial && c.Id != id))
            {
                ModelState.AddModelError(string.Empty, "A device with this VendingDeviceSerial already exists.");
                SetViewBag(user);
                return View(device);
            }
            if (_context.Devices.Any(c => c.PaymentDeviceSerial == device.PaymentDeviceSerial && c.Id != id))
            {
                ModelState.AddModelError(string.Empty, "A device with this PaymentDeviceSerial already exists.");
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

            SetViewBag(user);
            return View(device);
        }

        /// <summary>
        /// Displays the device deletion confirmation page for a specific device.
        /// </summary>
        /// <param name="id">The ID of the device to delete.</param>
        /// <returns>The view displaying the deletion confirmation page.</returns>
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

        /// <summary>
        /// Confirms the deletion of a device and removes it from the database.
        /// If the device has related transactions, it cannot be deleted.
        /// </summary>
        /// <param name="id">The ID of the device to delete.</param>
        /// <returns>Redirects to the device index page or redisplays the deletion confirmation with an error message.</returns>
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
                    ModelState.AddModelError(string.Empty, "This record cannot be deleted because it is associated with a transaction.");
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

        /// <summary>
        /// Checks whether a device with the given ID exists in the database.
        /// </summary>
        /// <param name="id">The ID of the device to check.</param>
        /// <returns>True if the device exists, otherwise false.</returns>
        private bool DeviceExists(int id)
        {
            return _context.Devices.Any(e => e.Id == id);
        }
    }

}
