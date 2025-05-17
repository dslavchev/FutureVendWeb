using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Castle.Core.Resource;
using FutureVendWeb.Data.Models;
using FutureVendWeb.Services.PaymentDevice;
using FutureVendWeb.Data.Models.PaymentDevice;
using FutureVendWeb.Services.User;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Data.Models.Device;

namespace FutureVendWeb.Controllers

{
    public class PaymentDevicesController : Controller
    {
        private readonly IPaymentDeviceService _paymentDeviceService;
        private readonly IUserService _userService;

        public PaymentDevicesController(IPaymentDeviceService paymentDeviceService, IUserService userService)
        {
            _paymentDeviceService = paymentDeviceService;
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            List<GetAllPaymentDevicesViewModel> getAllPaymentDevicesViewModels = _paymentDeviceService.GetAll(user);
            return View(getAllPaymentDevicesViewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreatePaymentDeviceModel createPaymentDevice)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            _paymentDeviceService.Create(createPaymentDevice, user);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            GetPaymentDeviceModel getPaymentDevice = _paymentDeviceService.Get(id);
            return View(getPaymentDevice);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            GetPaymentDeviceModel getPaymentDevice = _paymentDeviceService.Get(id);
            return View(getPaymentDevice);
        }

        [HttpPost]
        public IActionResult Edit(int id, UpdatePaymentDeviceModel updatePaymentDevice)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            _paymentDeviceService.Update(id, updatePaymentDevice);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            GetPaymentDeviceModel getPaymentDevice = _paymentDeviceService.Get(id);
            return View(getPaymentDevice);
        }

        [HttpPost]

        public IActionResult DeletePaymentDevice(int id)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            try
            {
                _paymentDeviceService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                GetPaymentDeviceModel device = _paymentDeviceService.Get(id);
                return View("Delete", device);
            }
        }

        /*
        /// <summary>
        /// Controller for managing payment devices in the application.
        /// All actions in this controller require the user to be authenticated.
        /// </summary>
        [Authorize]
        public class PaymentDevicesController : Controller
        {
            private readonly VendingDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;

            /// <summary>
            /// Initializes a new instance of the <see cref="PaymentDevicesController"/> class.
            /// </summary>
            /// <param name="context">The database context used to interact with the data.</param>
            /// <param name="userManager">The user manager for handling user-related tasks.</param>
            public PaymentDevicesController(VendingDbContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            /// <summary>
            /// Displays a list of payment devices associated with the logged-in user.
            /// </summary>
            /// <returns>A view with the list of payment devices.</returns>
            public async Task<IActionResult> Index()
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                var devices = await _context.PaymentDevices
                    .Where(d => d.UserId == user.Id)
                    .ToListAsync();

                return View(devices);
            }

            /// <summary>
            /// Displays the details of a specific payment device.
            /// </summary>
            /// <param name="id">The ID of the payment device.</param>
            /// <returns>A view with the details of the payment device.</returns>
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

            /// <summary>
            /// Displays the form for creating a new payment device.
            /// </summary>
            /// <returns>A view with the form for creating a payment device.</returns>
            public IActionResult Create()
            {
                return View();
            }

            /// <summary>
            /// Handles the submission of a new payment device.
            /// </summary>
            /// <param name="device">The payment device to be created.</param>
            /// <returns>Redirects to the index view if successful, otherwise shows validation errors.</returns>
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Create(PaymentDeviceEntity device)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return RedirectToAction("Login", "Account");

                device.UserId = user.Id;

                if (ModelState.IsValid)
                {
                    _context.Add(device);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(device);
            }

            /// <summary>
            /// Displays the form for editing an existing payment device.
            /// </summary>
            /// <param name="id">The ID of the payment device to be edited.</param>
            /// <returns>A view with the form for editing the payment device.</returns>
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

            /// <summary>
            /// Handles the submission of edits to an existing payment device.
            /// </summary>
            /// <param name="id">The ID of the payment device to be updated.</param>
            /// <param name="paymentDevice">The updated payment device data.</param>
            /// <returns>Redirects to the index view if successful, otherwise shows validation errors.</returns>
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Manufacturer,OSVersion,NFC,Chip")] PaymentDeviceEntity paymentDevice)
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

            /// <summary>
            /// Displays the confirmation page for deleting a payment device.
            /// </summary>
            /// <param name="id">The ID of the payment device to be deleted.</param>
            /// <returns>A view to confirm the deletion of the payment device.</returns>
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

            /// <summary>
            /// Handles the deletion of a payment device.
            /// </summary>
            /// <param name="id">The ID of the payment device to be deleted.</param>
            /// <returns>Redirects to the index view after successful deletion.</returns>
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
                        ModelState.AddModelError(string.Empty, "This record cannot be deleted.");
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

            /// <summary>
            /// Checks if a payment device exists in the database.
            /// </summary>
            /// <param name="id">The ID of the payment device.</param>
            /// <returns>True if the payment device exists, otherwise false.</returns>
            private bool PaymentDeviceExists(int id)
            {
                return _context.PaymentDevices.Any(e => e.Id == id);
            }
        }
        */
    }
}
