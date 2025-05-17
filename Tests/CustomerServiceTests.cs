using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.Customer;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Services.Customer;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace FutureVendWeb.Tests.Services
{
    public class CustomerServiceTests
    {
        private VendingDbContext _context;
        private CustomerService _service;
        private UserData _testUser;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VendingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            _context = new VendingDbContext(options);
            _service = new CustomerService(_context);

            _testUser = new UserData { Id = 1 };
        }

        [TearDown]
        public void Teardown()
        {
            _context.Dispose();
        }

        [Test]
        public void Create_Should_Add_Customer()
        {
            var model = new CreateCustomerModel
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                CompanyName = "FutureVend",
                Address = "ul. Bulgaria 1",
                City = "Sofia",
                Country = "Bulgaria",
                PostCode = "1000",
                Email = "ivan@example.com",
                Phone = "0888123456",
                TaxNumber = "BG123456789"
            };

            _service.Create(model, _testUser);

            var customer = _context.Customers.FirstOrDefault();
            Assert.IsNotNull(customer);
            Assert.AreEqual("Ivan", customer.FirstName);
            Assert.AreEqual(1, customer.UserId);
        }

        [Test]
        public void Create_With_Duplicate_TaxNumber_Should_Throw()
        {
            _context.Customers.Add(new CustomerEntity
            {
                FirstName = "Pesho",
                LastName = "Peshov",
                TaxNumber = "BG999999999",
                CompanyName = "X",
                City = "Sofia",
                Country = "BG",
                Email = "x@y.bg",
                Phone = "0000",
                Address = "ul X",
                PostCode = "1000",
                UserId = _testUser.Id
            });
            _context.SaveChanges();

            var model = new CreateCustomerModel
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                TaxNumber = "BG999999999",
                CompanyName = "X",
                City = "Sofia",
                Country = "BG",
                Email = "x@y.bg",
                Phone = "0000",
                Address = "ul X",
                PostCode = "1000"
            };

            var ex = Assert.Throws<Exception>(() => _service.Create(model, _testUser));
            Assert.AreEqual("Client with this tax number already exists.", ex.Message);
        }

        [Test]
        public void Delete_Should_Remove_Customer()
        {
            var customer = new CustomerEntity
            {
                Id = 1,
                FirstName = "Pesho",
                LastName = "Peshov",
                TaxNumber = "BG999999999",
                CompanyName = "X",
                City = "Sofia",
                Country = "BG",
                Email = "x@y.bg",
                Phone = "0000",
                Address = "ul X",
                PostCode = "1000",
                UserId = _testUser.Id
            };
            _context.Customers.Add(customer);
            _context.SaveChanges();

            _service.Delete(1);

            var exists = _context.Customers.Any(x => x.Id == 1);
            Assert.IsFalse(exists);
        }

        [Test]
        public void Delete_When_Device_Exists_Should_Throw()
        {
            var customer = new CustomerEntity
            {
                Id = 1,
                FirstName = "Pesho",
                LastName = "Peshov",
                TaxNumber = "BG999999999",
                CompanyName = "X",
                City = "Sofia",
                Country = "BG",
                Email = "x@y.bg",
                Phone = "0000",
                Address = "ul X",
                PostCode = "1000",
                UserId = _testUser.Id
            };
            _context.Customers.Add(customer);
           
            var device = new DeviceEntity
            {
                Id = 1,
                CustomerId = customer.Id,
                PaymentDeviceSerial = "123456",
                VendingDeviceSerial = "654321"
            };
            _context.Devices.Add(device);
            _context.SaveChanges();

            var ex = Assert.Throws<ArgumentException>(() => _service.Delete(2));
            Assert.AreEqual("Invalid customer id", ex.Message);
        }

        [Test]
        public void Get_Should_Return_Customer()
        {
            _context.Customers.Add(new CustomerEntity
            {
                Id = 3,
                FirstName = "Maria",
                LastName = "Marinova",
                TaxNumber = "BG111",
                UserId = _testUser.Id,
                City = "Sofia",
                Address = "ul X",
                PostCode = "1000",
                Country = "BG",
                CompanyName = "Test",
                Email = "maria@example.com",
                Phone = "0888000000"
            });
            _context.SaveChanges();

            var result = _service.Get(3);
            Assert.AreEqual("Maria", result.FirstName);
            Assert.AreEqual("Marinova", result.LastName);
        }

        [Test]
        public void Update_Should_Change_Fields()
        {
            _context.Customers.Add(new CustomerEntity
            {
                Id = 4,
                FirstName = "Old",
                LastName = "Name",
                TaxNumber = "BGX",
                City = "Sofia",
                Address = "ul X",
                PostCode = "1000",
                Country = "BG",
                CompanyName = "Test",
                Email = "maria@example.com",
                Phone = "0888000000",
                UserId = _testUser.Id
            });
            _context.SaveChanges();

            var updateModel = new UpdateCustomerModel
            {
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

            _service.Update(4, updateModel);

            var updated = _context.Customers.Find(4);
            Assert.AreEqual("New", updated.FirstName);
            Assert.AreEqual("Plovdiv", updated.City);
            Assert.AreEqual("NewCo", updated.CompanyName);
        }
    }
}
