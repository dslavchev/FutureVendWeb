using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.Device;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Services.Device;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace FutureVendTests.Services
{
    public class DeviceServiceTests
    {
        private VendingDbContext _context;
        private DeviceService _deviceService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VendingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new VendingDbContext(options);

            SeedData();
            _deviceService = new DeviceService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        private void SeedData()
        {
            _context.PaymentDevices.Add(new PaymentDeviceEntity { Id = 1, Name = "ReaderX", Manufacturer = "PayTech",OSVersion = "1.2", UserId = 1 });
            _context.VendingDevices.Add(new VendingDeviceEntity { Id = 1, Model = "VendPro", Manufacturer = "VendCorp",SoftwareVersion = "3.4", UserId = 1 });
            _context.Customers.Add(new CustomerEntity { Id = 1,
                FirstName = "New",
                LastName = "Name",
                TaxNumber = "BGX",
                CompanyName = "NewCo",
                City = "Plovdiv",
                Address = "ul Y",
                Country = "BG",
                Email = "new@co.bg",
                Phone = "099999999",
                PostCode = "4000", 
                UserId = 1 });

            _context.SaveChanges();
        }

        [Test]
        public void Create_ShouldAddDeviceSuccessfully()
        {
            var model = new CreateDeviceModel
            {
                PaymentDeviceSerial = "PD123",
                VendingDeviceSerial = "VD456",
                PaymentDeviceId = 1,
                VendingDeviceId = 1,
                CustomerId = 1,
                AcceptCard = true,
                AcceptCash = false,
                LocationLat = 42.6977,
                LocationLon = 23.3219
            };

            var user = new UserData { Id = 1 };

            // Act
            _deviceService.Create(model, user);

            // Assert
            var device = _context.Devices.FirstOrDefault(d => d.PaymentDeviceSerial == "PD123");
            Assert.IsNotNull(device);
            Assert.AreEqual("VD456", device.VendingDeviceSerial);
        }

        [Test]
        public void Delete_ShouldRemoveDevice_WhenNotUsedInTransactions()
        {
            // Arrange
            var device = new DeviceEntity
            {
                PaymentDeviceSerial = "ToDelete",
                VendingDeviceSerial = "ToDelete",
                PaymentDeviceId = 1,
                VendingDeviceId = 1,
                CustomerId = 1,
                AcceptCard = true,
                AcceptCash = true,
                LocationLat = 0,
                LocationLon = 0,
                UserId = 1
            };
            _context.Devices.Add(device);
            _context.SaveChanges();

            // Act
            _deviceService.Delete(device.Id);

            // Assert
            Assert.IsFalse(_context.Devices.Any(d => d.Id == device.Id));
        }

        [Test]
        public void Delete_ShouldThrowException_WhenDeviceUsedInTransactions()
        {
            // Arrange
            var device = new DeviceEntity
            {
                PaymentDeviceSerial = "HasTransaction",
                VendingDeviceSerial = "HasTransaction",
                PaymentDeviceId = 1,
                VendingDeviceId = 1,
                CustomerId = 1,
                AcceptCard = true,
                AcceptCash = true,
                LocationLat = 0,
                LocationLon = 0,
                UserId = 1
            };
            _context.Devices.Add(device);
            _context.SaveChanges();

            _context.Transactions.Add(new TransactionEntity
            {
                DeviceId = device.Id,
                Amount = 1.5m,
                Currency = "BGN",
                PaymentType = "card",
                CreatedAt = DateTime.Now
            });
            _context.SaveChanges();

            // Assert
            Assert.Throws<Exception>(() => _deviceService.Delete(device.Id));
        }

        [Test]
        public void Update_ShouldModifyDeviceSuccessfully()
        {
            // Arrange
            var device = new DeviceEntity
            {
                PaymentDeviceSerial = "OldSerial",
                VendingDeviceSerial = "OldVending",
                PaymentDeviceId = 1,
                VendingDeviceId = 1,
                CustomerId = 1,
                AcceptCard = true,
                AcceptCash = true,
                LocationLat = 10,
                LocationLon = 10,
                UserId = 1
            };
            _context.Devices.Add(device);
            _context.SaveChanges();

            var updateModel = new UpdateDeviceModel
            {
                PaymentDeviceSerial = "NewSerial",
                VendingDeviceSerial = "NewVending",
                PaymentDeviceId = 1,
                VendingDeviceId = 1,
                CustomerId = 1,
                AcceptCard = false,
                AcceptCash = false,
                LocationLat = 20,
                LocationLon = 20
            };

            // Act
            _deviceService.Update(device.Id, updateModel);

            // Assert
            var updatedDevice = _context.Devices.First(d => d.Id == device.Id);
            Assert.AreEqual("NewSerial", updatedDevice.PaymentDeviceSerial);
            Assert.IsFalse(updatedDevice.AcceptCard);
        }

        [Test]
        public void Create_ShouldThrowException_WhenDuplicatePaymentSerial()
        {
            // Arrange
            _context.Devices.Add(new DeviceEntity
            {
                PaymentDeviceSerial = "DUPLICATE",
                VendingDeviceSerial = "UNIQUE",
                PaymentDeviceId = 1,
                VendingDeviceId = 1,
                CustomerId = 1,
                AcceptCard = true,
                AcceptCash = false,
                LocationLat = 0,
                LocationLon = 0,
                UserId = 1
            });
            _context.SaveChanges();

            var model = new CreateDeviceModel
            {
                PaymentDeviceSerial = "DUPLICATE",
                VendingDeviceSerial = "ANOTHER",
                PaymentDeviceId = 1,
                VendingDeviceId = 1,
                CustomerId = 1,
                AcceptCard = true,
                AcceptCash = true,
                LocationLat = 0,
                LocationLon = 0
            };

            var user = new UserData { Id = 1 };

           
            Assert.Throws<Exception>(() => _deviceService.Create(model, user));
        }
    }
}
