using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Data.Models.VendingDevices;
using FutureVendWeb.Services.VendingDevice;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace FutureVendWeb.Tests.Services
{
    public class VendingDeviceServiceTests
    {
        private VendingDbContext _context;
        private VendingDeviceService _service;
        private UserData _user;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VendingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new VendingDbContext(options);
            _service = new VendingDeviceService(_context);
            _user = new UserData { Id = 1 };
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void Create_Should_Add_Device()
        {
            var model = new CreateVendingDeviceModel
            {
                Manufacturer = "Necta",
                Model = "Kikko",
                SoftwareVersion = "v1.0.0"
            };

            _service.Create(model, _user);

            var device = _context.VendingDevices.FirstOrDefault();
            Assert.IsNotNull(device);
            Assert.AreEqual("Necta", device.Manufacturer);
            Assert.AreEqual("Kikko", device.Model);
            Assert.AreEqual(1, device.UserId);
        }

        [Test]
        public void GetAll_Should_Return_Only_Devices_For_User()
        {
            _context.VendingDevices.AddRange(
                new VendingDeviceEntity { Manufacturer = "X", Model = "X1",SoftwareVersion = "2.4" , UserId = 1 },
                new VendingDeviceEntity { Manufacturer = "Y", Model = "Y1",SoftwareVersion = "2.5" , UserId = 2 }
            );
            _context.SaveChanges();

            var result = _service.GetAll(_user);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("X", result[0].Manufacturer);
        }

        [Test]
        public void Get_Should_Return_Correct_Device()
        {
            _context.VendingDevices.Add(new VendingDeviceEntity
            {
                Id = 10,
                Manufacturer = "Necta",
                Model = "Brio",
                SoftwareVersion = "2.0",
                UserId = 1
            });
            _context.SaveChanges();

            var device = _service.Get(10);

            Assert.AreEqual("Necta", device.Manufacturer);
            Assert.AreEqual("Brio", device.Model);
            Assert.AreEqual("2.0", device.SoftwareVersion);
        }

        [Test]
        public void Update_Should_Modify_Device()
        {
            _context.VendingDevices.Add(new VendingDeviceEntity
            {
                Id = 5,
                Manufacturer = "Old",
                Model = "OldModel",
                SoftwareVersion = "0.9",
                UserId = 1
            });
            _context.SaveChanges();

            var updateModel = new UpdateVendingDeviceModel
            {
                Manufacturer = "New",
                Model = "NewModel",
                SoftwareVersion = "1.1"
            };

            _service.Update(5, updateModel);

            var updated = _context.VendingDevices.Find(5);
            Assert.AreEqual("New", updated.Manufacturer);
            Assert.AreEqual("NewModel", updated.Model);
            Assert.AreEqual("1.1", updated.SoftwareVersion);
        }

        [Test]
        public void Delete_Should_Remove_Device()
        {
            _context.VendingDevices.Add(new VendingDeviceEntity
            {
                Id = 7,
                Manufacturer = "Del",
                Model = "ModelD",
                SoftwareVersion ="2.3",
                UserId = 1
            });
            _context.SaveChanges();

            _service.Delete(7);

            var exists = _context.VendingDevices.Any(x => x.Id == 7);
            Assert.IsFalse(exists);
        }

        [Test]
        public void Delete_When_Device_Is_Used_Should_Throw()
        {
            _context.VendingDevices.Add(new VendingDeviceEntity {
                Id = 8,
                Manufacturer = "Necta",
                Model = "Brio",
                SoftwareVersion = "2.0",
                UserId = 1 });
            _context.Devices.Add(new DeviceEntity { Id = 1,PaymentDeviceSerial = "1234" ,
                VendingDeviceSerial = "12345" , VendingDeviceId = 8 });
            _context.SaveChanges();

            var ex = Assert.Throws<Exception>(() => _service.Delete(8));
            Assert.AreEqual("This vending device is already used", ex.Message);
        }

        [Test]
        public void Get_With_Invalid_Id_Should_Throw()
        {
            var ex = Assert.Throws<ArgumentException>(() => _service.Get(999));
            Assert.AreEqual("Invalid vending device id", ex.Message);
        }
    }
}
