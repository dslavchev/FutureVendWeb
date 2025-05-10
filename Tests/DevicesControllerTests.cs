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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FutureVendWeb.Tests.Controllers
{
    [TestFixture]
    public class DevicesControllerTests
    {
        private DevicesController _controller;
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

            _controller = new DevicesController(_context, _userManagerMock.Object);
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

        private async Task<Device> CreateValidDeviceAsync(string vendingSerial = "VS123", string paymentSerial = "PS123")
        {
            var vending = new VendingDevice
            {
                Model = "VModel",
                Manufacturer = "VMan",
                SoftwareVersion = "1.0.0",
                UserId = _testUser.Id
            };

            var payment = new PaymentDevice
            {
                Name = "PName",
                Manufacturer = "PMan",
                OSVersion = "2.0.0",
                NFC = true,
                Chip = true,
                UserId = _testUser.Id
            };

            var customer = new Customer
            {
                CompanyName = "CC",
                UserId = _testUser.Id,
                FirstName = "John",
                LastName = "Doe",
                Address = "Test St",
                City = "Sofia",
                Country = "Bulgaria",
                Email = "john@doe.com",
                Phone = "0888123456",
                PostCode = "1000",
                TaxNumber = "TAX" + Guid.NewGuid().ToString("N").Substring(0, 5)
            };

            await _context.VendingDevices.AddAsync(vending);
            await _context.PaymentDevices.AddAsync(payment);
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();

            var device = new Device
            {
                VendingDeviceSerial = vendingSerial,
                PaymentDeviceSerial = paymentSerial,
                VendingDeviceId = vending.Id,
                PaymentDeviceId = payment.Id,
                CustomerId = customer.Id,
                AcceptCard = true,
                AcceptCash = true,
                LocationLat = 42.0,
                LocationLon = 23.0,
                UserId = _testUser.Id
            };

            return device;
        }

        [Test]
        public async Task Index_ReturnsDevicesForUser()
        {
            var device = await CreateValidDeviceAsync("VS123", "PS123");
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            var result = await _controller.Index();

            var viewResult = result as ViewResult;
            var model = viewResult?.Model as List<Device>;

            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("VS123", model[0].VendingDeviceSerial);
        }

        [Test]
        public async Task Create_Post_ValidDevice_AddsToDatabase()
        {
            var device = await CreateValidDeviceAsync("VS999", "PS999");

            var result = await _controller.Create(device);
            var redirect = result as RedirectToActionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual(1, _context.Devices.Count());
        }

        [Test]
        public async Task Create_Post_DuplicateSerial_ReturnsError()
        {
            var existing = await CreateValidDeviceAsync("VS111", "PS111");
            _context.Devices.Add(existing);
            await _context.SaveChangesAsync();

            var duplicate = await CreateValidDeviceAsync("VS111", "PS111");

            var result = await _controller.Create(duplicate);
            var view = result as ViewResult;

            Assert.IsNotNull(view);
            Assert.IsFalse(view.ViewData.ModelState.IsValid);
        }

        [Test]
        public async Task Edit_Post_DuplicateSerial_ReturnsError()
        {
            var d1 = await CreateValidDeviceAsync("VS1", "PS1");
            var d2 = await CreateValidDeviceAsync("VS2", "PS2");
            _context.Devices.AddRange(d1, d2);
            await _context.SaveChangesAsync();

            d2.VendingDeviceSerial = "VS1"; // conflict

            var result = await _controller.Edit(d2.Id, d2);
            var view = result as ViewResult;

            Assert.IsNotNull(view);
            Assert.IsFalse(view.ViewData.ModelState.IsValid);
        }

        [Test]
        public async Task DeleteConfirmed_RemovesDevice()
        {
            var device = await CreateValidDeviceAsync("VDEL", "PDEL");
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(device.Id);
            var redirect = result as RedirectToActionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.IsFalse(_context.Devices.Any(d => d.Id == device.Id));
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
