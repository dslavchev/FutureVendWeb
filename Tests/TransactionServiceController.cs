using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.Transaction;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Services.Transaction;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace FutureVendWeb.Tests.Services
{
    public class TransactionServiceTests
    {
        private VendingDbContext _context;
        private TransactionService _service;
        private UserData _user;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VendingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new VendingDbContext(options);
            _service = new TransactionService(_context);
            _user = new UserData { Id = 1 };
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void Create_ValidTransaction_AddsSuccessfully()
        {
            var customer = new CustomerEntity { Id = 1,
                FirstName = "New",
                LastName = "Name",
                TaxNumber = "BGX",
                CompanyName = "NewCo",
                City = "Plovdiv",
                Address = "ul Y",
                Country = "BG",
                Email = "new@co.bg",
                Phone = "099999999",
                PostCode = "4000"
            };
            var paymentDevice = new PaymentDeviceEntity { Id = 1, Name = "PD100", Manufacturer = "VendTech" , OSVersion = "2.3"};
            var device = new DeviceEntity
            {
                Id = 1,
                UserId = 1,
                PaymentDeviceId = 1,
                CustomerId = 1,
                PaymentDeviceSerial = "ABC123",
                VendingDeviceSerial = "231432523",
                PaymentDevice = paymentDevice,
                Customer = customer
            };

            var vendingProduct = new VendingProductEntity
            {
                Id = 1,
                PLU = "12345",
                Name = "Water",
                Description = "500ml",
                Category = "drinks",
                UserId = 1
            };

            _context.Customers.Add(customer);
            _context.PaymentDevices.Add(paymentDevice);
            _context.Devices.Add(device);
            _context.VendingProducts.Add(vendingProduct);
            _context.SaveChanges();

            var model = new CreateTransactionModel
            {
                SerialNumber = "ABC123",
                ItemNumber = "12345",
                Amount = 2.5m,
                CurrencyCode = "EUR",
                PaymentType = "cash",
                CreatedAt = DateTime.UtcNow
            };

            _service.Create(model);

            var transaction = _context.Transactions.FirstOrDefault();
            Assert.NotNull(transaction);
            Assert.AreEqual(2.5m, transaction.Amount);
            Assert.AreEqual("EUR", transaction.Currency);
        }

        [Test]
        public void Create_With_InvalidDeviceSerial_Throws()
        {
            var model = new CreateTransactionModel
            {
                SerialNumber = "INVALID",
                ItemNumber = "123",
                Amount = 1.0m,
                CurrencyCode = "EUR",
                PaymentType = "cash",
                CreatedAt = DateTime.UtcNow
            };

            var ex = Assert.Throws<ArgumentException>(() => _service.Create(model));
            Assert.AreEqual("Invalid device serial number", ex.Message);
        }

        [Test]
        public void Create_With_InvalidPLU_Throws()
        {
            var device = new DeviceEntity
            {
                Id = 1,
                UserId = 1,
                PaymentDeviceSerial = "ABC123",
                VendingDeviceSerial = "1234"

            };
            _context.Devices.Add(device);
            _context.SaveChanges();

            var model = new CreateTransactionModel
            {
                SerialNumber = "ABC123",
                ItemNumber = "BADPLU",
                Amount = 1.0m,
                CurrencyCode = "EUR",
                PaymentType = "card",
                CreatedAt = DateTime.UtcNow
            };

            var ex = Assert.Throws<ArgumentException>(() => _service.Create(model));
            Assert.AreEqual("Invalid vending product PLU", ex.Message);
        }

        [Test]
        public void Delete_RemovesTransaction()
        {
            _context.Transactions.Add(new TransactionEntity
            {
                Id = 1,
                Amount = 1.0m,
                Currency = "EUR",
                CreatedAt = DateTime.UtcNow,
                DeviceId = 1,
                VendingProductId = 1,
                PaymentType = "cash"
            });
            _context.SaveChanges();

            _service.Delete(1);

            var exists = _context.Transactions.Any(x => x.Id == 1);
            Assert.IsFalse(exists);
        }

        [Test]
        public void Get_ReturnsTransactionWithDetails()
        {
            var customer = new CustomerEntity { Id = 1,
                FirstName = "Anna",
                LastName = "Smith",
                TaxNumber = "BGX",
                CompanyName = "NewCo",
                City = "Plovdiv",
                Address = "ul Y",
                Country = "BG",
                Email = "new@co.bg",
                Phone = "099999999",
                PostCode = "4000",
                UserId = 1
            };
            var paymentDevice = new PaymentDeviceEntity { Id = 1, Name = "SmartReader", Manufacturer = "SmartCorp",OSVersion = "2.3"};
            var device = new DeviceEntity
            {
                Id = 1,
                PaymentDeviceSerial = "SN1",
                VendingDeviceSerial = "12345",
                CustomerId = customer.Id,
                UserId = 1,
                PaymentDeviceId = paymentDevice.Id
            };
            var product = new VendingProductEntity 
            { 
                Id = 2, 
                Name = "Soda", 
                Description = "Cola",
                Category = "drinks", 
                PLU = "SODA1", 
                UserId = 1 
            };
            var transaction = new TransactionEntity
            {
                Id = 5,
                Amount = 3.0m,
                Currency = "EUR",
                CreatedAt = DateTime.UtcNow,
                DeviceId = device.Id,
                VendingProductId = product.Id,
                PaymentType = "card",
                
            };

            _context.Customers.Add(customer);
            _context.PaymentDevices.Add(paymentDevice);
            _context.Devices.Add(device);
            _context.VendingProducts.Add(product);
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            var result = _service.Get(transaction.Id);

            Assert.AreEqual("EUR", result.Currency);
            Assert.IsTrue(result.DeviceInformation.Contains("SmartReader"));
            Assert.IsTrue(result.CustomerInformation.Contains("Anna Smith"));
            Assert.IsTrue(result.VendingProductInformation.Contains("Soda Cola"));
        }

        [Test]
        public void GetAll_ReturnsOnlyUserTransactions()
        {
            var customer1 = new CustomerEntity { Id = 1,
                FirstName = "New",
                LastName = "Name",
                TaxNumber = "BGX",
                CompanyName = "NewCo",
                City = "Plovdiv",
                Address = "ul Y",
                Country = "BG",
                Email = "new@co.bg",
                Phone = "099999999",
                PostCode = "4000"
            };
            var customer2 = new CustomerEntity { Id = 2,
                FirstName = "N",
                LastName = "vv",
                TaxNumber = "BGX",
                CompanyName = "NewCo",
                City = "Plovdiv",
                Address = "ul Y",
                Country = "BG",
                Email = "new@co.bg",
                Phone = "099999999",
                PostCode = "4000"
            };
            var paymentDevice = new PaymentDeviceEntity { Id = 1, Name = "Reader", Manufacturer = "X" , OSVersion="2.3"};

            _context.Customers.AddRange(customer1, customer2);
            _context.PaymentDevices.Add(paymentDevice);

            _context.Devices.AddRange(
                new DeviceEntity { Id = 1, UserId = 1,PaymentDeviceSerial = "123",VendingDeviceSerial = "12321432", PaymentDeviceId = 1, CustomerId = 1, PaymentDevice = paymentDevice, Customer = customer1 },
                new DeviceEntity { Id = 2, UserId = 2, PaymentDeviceSerial = "123124" , VendingDeviceSerial = "12354" , PaymentDeviceId = 1, CustomerId = 2, PaymentDevice = paymentDevice, Customer = customer2 }
            );

            _context.VendingProducts.Add(new VendingProductEntity { Id = 1, Name = "Chips", Description = "Salty", Category = "drinks",PLU = "12343", UserId = 1 });
            _context.VendingProducts.Add(new VendingProductEntity { Id = 2, Name = "Candy", Description = "Sweet",Category = "drinks",PLU = "12343", UserId = 2 });

            _context.Transactions.AddRange(
                new TransactionEntity { Id = 1, Amount = 1, Currency = "EUR", DeviceId = 1, VendingProductId = 1, CreatedAt = DateTime.Now, PaymentType = "cash" },
                new TransactionEntity { Id = 2, Amount = 2, Currency = "USD", DeviceId = 2, VendingProductId = 2, CreatedAt = DateTime.Now, PaymentType = "card" }
            );
            _context.SaveChanges();

            var result = _service.GetAll(new UserData { Id = 1 });
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("EUR", result[0].Currency);
        }
    }
}
