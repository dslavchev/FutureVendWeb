using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.PaymentDevice;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Services.PaymentDevice;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace FutureVendWeb.Tests.Services
{
    public class PaymentDeviceServiceTests
    {
        private VendingDbContext _context;
        private PaymentDeviceService _service;
        private UserData _user;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VendingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new VendingDbContext(options);
            _service = new PaymentDeviceService(_context);
            _user = new UserData { Id = 1 };
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void Create_Should_Add_PaymentDevice()
        {
            var model = new CreatePaymentDeviceModel
            {
                Name = "PayWave",
                Manufacturer = "Verifone",
                NFC = true,
                Chip = true,
                OSVersion = "2.1"
            };

            _service.Create(model, _user);

            var device = _context.PaymentDevices.FirstOrDefault();
            Assert.IsNotNull(device);
            Assert.AreEqual("PayWave", device.Name);
            Assert.AreEqual("Verifone", device.Manufacturer);
            Assert.IsTrue(device.NFC);
            Assert.AreEqual(1, device.UserId);
        }

        [Test]
        public void GetAll_Should_Return_Only_User_PaymentDevices()
        {
            _context.PaymentDevices.AddRange(
                new PaymentDeviceEntity { Name = "X", Manufacturer = "A",OSVersion = "1.3", UserId = 1 },
                new PaymentDeviceEntity { Name = "Y", Manufacturer = "B",OSVersion = "1.4", UserId = 2 }
            );
            _context.SaveChanges();

            var result = _service.GetAll(_user);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("X", result[0].Name);
        }

        [Test]
        public void Get_Should_Return_Correct_Device()
        {
            _context.PaymentDevices.Add(new PaymentDeviceEntity
            {
                Id = 10,
                Name = "MyPay",
                Manufacturer = "Ingenico",
                OSVersion = "1.0",
                NFC = true,
                Chip = true,
                UserId = 1
            });
            _context.SaveChanges();

            var result = _service.Get(10);

            Assert.AreEqual("MyPay", result.Name);
            Assert.AreEqual("Ingenico", result.Manufacturer);
            Assert.AreEqual("1.0", result.OSVersion);
            Assert.IsTrue(result.NFC);
        }

        [Test]
        public void Update_Should_Modify_PaymentDevice()
        {
            _context.PaymentDevices.Add(new PaymentDeviceEntity
            {
                Id = 5,
                Name = "OldName",
                Manufacturer = "OldMfg",
                OSVersion = "0.9",
                NFC = false,
                Chip = false,
                UserId = 1
            });
            _context.SaveChanges();

            var updateModel = new UpdatePaymentDeviceModel
            {
                Name = "NewName",
                Manufacturer = "NewMfg",
                OSVersion = "1.1",
                NFC = true,
                Chip = true
            };

            _service.Update(5, updateModel);

            var updated = _context.PaymentDevices.Find(5);
            Assert.AreEqual("NewName", updated.Name);
            Assert.AreEqual("NewMfg", updated.Manufacturer);
            Assert.AreEqual("1.1", updated.OSVersion);
            Assert.IsTrue(updated.NFC);
            Assert.IsTrue(updated.Chip);
        }

        [Test]
        public void Delete_Should_Remove_Device()
        {
            _context.PaymentDevices.Add(new PaymentDeviceEntity
            {
                Id = 7,
                Name = "ToDelete",
                Manufacturer = "Mfg",
                OSVersion = "1.3",
                UserId = 1
            });
            _context.SaveChanges();

            _service.Delete(7);

            var exists = _context.PaymentDevices.Any(x => x.Id == 7);
            Assert.IsFalse(exists);
        }

        [Test]
        public void Delete_When_Device_Is_Used_Should_Throw()
        {
            _context.PaymentDevices.Add(new PaymentDeviceEntity {
                Id = 8,
                Name = "ToDelete",
                Manufacturer = "Mfg",
                OSVersion = "1.3",
                UserId = 1 });
            _context.Devices.Add(new DeviceEntity {
                Id = 1,
                PaymentDeviceSerial = "123",
                VendingDeviceSerial = "1234",
                PaymentDeviceId = 8 });
            _context.SaveChanges();

            var ex = Assert.Throws<Exception>(() => _service.Delete(8));
            Assert.AreEqual("This payment device is already used", ex.Message);
        }

        [Test]
        public void Get_With_Invalid_Id_Should_Throw()
        {
            var ex = Assert.Throws<ArgumentException>(() => _service.Get(999));
            Assert.AreEqual("Invalid payment device id", ex.Message);
        }
    }
}
