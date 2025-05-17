using Microsoft.AspNetCore.Mvc;
using FutureVendWeb.Data.Models.VendingDevices;
using FutureVendWeb.Services.VendingDevice;
using FutureVendWeb.Services.User;
using FutureVendWeb.Data.Models.User;

namespace FutureVendWeb.Controllers
{
    public class VendingDevicesController : Controller
    {
        private readonly IVendingDeviceService _vendingDeviceService;

        private readonly IUserService _userService;

        public VendingDevicesController(IVendingDeviceService vendingDeviceService , IUserService userService) 
        {
            this._vendingDeviceService = vendingDeviceService;
            this._userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            List<GetAllVendingDevicesViewModel> vendingDevices = _vendingDeviceService.GetAll(user);
            return View(vendingDevices);
        }

        [HttpGet]
        public IActionResult Create()
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            return View();
        }

        [HttpPost]
        public IActionResult Create (CreateVendingDeviceModel vendingDevice)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            _vendingDeviceService.Create(vendingDevice, user);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            GetVendingDeviceViewModel getVendingDeviceViewModel = _vendingDeviceService.Get(id);
            return View(getVendingDeviceViewModel);
        }
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            GetVendingDeviceViewModel getVendingDeviceViewModel = _vendingDeviceService.Get(id); 
            return View(getVendingDeviceViewModel);
        }

        [HttpPost]

        public IActionResult Edit(int id , UpdateVendingDeviceModel updateVendingDevice)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            _vendingDeviceService.Update(id, updateVendingDevice);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");

            GetVendingDeviceViewModel getVendingDeviceViewModel = _vendingDeviceService.Get(id);
            return View(getVendingDeviceViewModel);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            UserData? user = _userService.GetUser();
            if (user == null) return RedirectToAction("Login", "User");
            try
            {
                _vendingDeviceService.Delete(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewData["Error"] = ex.Message;
                GetVendingDeviceViewModel device = _vendingDeviceService.Get(id);
                return View("Delete", device);
            }
        }
    }

    /*
/// <summary>
/// Controller for managing vending devices in the application.
/// All actions in this controller require the user to be authenticated.
/// </summary>
[Authorize]
public class VendingDevicesController : Controller
{
    private readonly VendingDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="VendingDevicesController"/> class.
    /// </summary>
    /// <param name="context">The database context used to interact with the data.</param>
    /// <param name="userManager">The user manager for handling user-related tasks.</param>
    public VendingDevicesController(VendingDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    /// <summary>
    /// Displays a list of vending devices associated with the logged-in user.
    /// </summary>
    /// <returns>A view with the list of vending devices.</returns>
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return RedirectToAction("Login", "Account");

        var devices = await _context.VendingDevices
            .Where(d => d.UserId == user.Id)
            .ToListAsync();

        return View(devices);
    }

    /// <summary>
    /// Displays the details of a specific vending device.
    /// </summary>
    /// <param name="id">The ID of the vending device.</param>
    /// <returns>A view with the details of the vending device.</returns>
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

    /// <summary>
    /// Displays the form for creating a new vending device.
    /// </summary>
    /// <returns>A view with the form for creating a vending device.</returns>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Handles the submission of a new vending device.
    /// </summary>
    /// <param name="device">The vending device to be created.</param>
    /// <returns>Redirects to the index view if successful, otherwise shows validation errors.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VendingDevice device)
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
    /// Displays the form for editing an existing vending device.
    /// </summary>
    /// <param name="id">The ID of the vending device to be edited.</param>
    /// <returns>A view with the form for editing the vending device.</returns>
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

    /// <summary>
    /// Handles the submission of edits to an existing vending device.
    /// </summary>
    /// <param name="id">The ID of the vending device to be updated.</param>
    /// <param name="vendingDevice">The updated vending device data.</param>
    /// <returns>Redirects to the index view if successful, otherwise shows validation errors.</returns>
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

    /// <summary>
    /// Displays the confirmation page for deleting a vending device.
    /// </summary>
    /// <param name="id">The ID of the vending device to be deleted.</param>
    /// <returns>A view to confirm the deletion of the vending device.</returns>
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

    /// <summary>
    /// Handles the deletion of a vending device.
    /// </summary>
    /// <param name="id">The ID of the vending device to be deleted.</param>
    /// <returns>Redirects to the index view after successful deletion.</returns>
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
                ModelState.AddModelError(string.Empty, "This record cannot be deleted.");
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

    /// <summary>
    /// Checks if a vending device exists in the database.
    /// </summary>
    /// <param name="id">The ID of the vending device.</param>
    /// <returns>True if the vending device exists, otherwise false.</returns>
    private bool VendingDeviceExists(int id)
    {
        return _context.VendingDevices.Any(e => e.Id == id);
    }
}
*/
}
