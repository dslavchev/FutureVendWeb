using FutureVendWeb.Controllers;
using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FutureVendWeb.Tests.Controllers
{
    [TestFixture]
    public class VendingDevicesControllerTests
    {
        private VendingDevicesController _controller;
        private VendingDbContext _context;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private ApplicationUser _testUser;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<VendingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new VendingDbContext(options);

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _testUser = new ApplicationUser { Id = "user1", UserName = "testuser", FullName = "Test User" };
            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(_testUser);

            _controller = new VendingDevicesController(_context, _userManagerMock.Object);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, _testUser.Id),
                        new Claim(ClaimTypes.Name, _testUser.UserName)
                    }, "TestAuth"))
                }
            };
        }

        private async Task<VendingDevice> CreateValidVendingDeviceAsync()
        {
            var device = new VendingDevice
            {
                Model = "VendingDevice1",
                Manufacturer = "Manufacturer1",
                SoftwareVersion = "1.0.0",
                UserId = _testUser.Id
            };

            _context.VendingDevices.Add(device);
            await _context.SaveChangesAsync();

            return device;
        }

        [Test]
        public async Task Index_ReturnsDevicesForUser()
        {
            var device = await CreateValidVendingDeviceAsync();
            var result = await _controller.Index();

            var view = result as ViewResult;
            var model = view?.Model as List<VendingDevice>;

            Assert.IsNotNull(view);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(device.Model, model[0].Model);
        }

        [Test]
        public async Task Create_Post_ValidDevice_AddsToDatabase()
        {
            var device = new VendingDevice
            {
                Model = "VendingDevice2",
                Manufacturer = "Manufacturer2",
                SoftwareVersion = "2.0.0"
            };

            var result = await _controller.Create(device);
            var redirect = result as RedirectToActionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual(1, _context.VendingDevices.Count());
        }

        [Test]
        public async Task Edit_Post_ValidDevice_UpdatesDatabase()
        {
            var device = await CreateValidVendingDeviceAsync();
            device.Model = "UpdatedVendingDevice";

            var result = await _controller.Edit(device.Id, device);
            var redirect = result as RedirectToActionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            var updatedDevice = await _context.VendingDevices.FindAsync(device.Id);
            Assert.AreEqual("UpdatedVendingDevice", updatedDevice.Model);
        }

        [Test]
        public async Task DeleteConfirmed_RemovesDevice()
        {
            var device = await CreateValidVendingDeviceAsync();

            var result = await _controller.DeleteConfirmed(device.Id);
            var redirect = result as RedirectToActionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.IsNull(await _context.VendingDevices.FindAsync(device.Id));
        }

        [TearDown]
        public void TearDown()
        {
            _controller?.Dispose();
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }
    }
}
