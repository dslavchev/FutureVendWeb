using FutureVendWeb.Controllers;
using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models;
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

/*
namespace FutureVendWeb.Tests.Controllers
{
    [TestFixture]
    [TestFixture]
    public class CustomersControllerTests
    {
        private CustomersController _controller;
        private Mock<UserManager<ApplicationUser>> _userManagerMock;
        private VendingDbContext _context;
        private ApplicationUser _testUser;

        private CustomerEntity CreateValidCustomer(string companyName = "Default Co", string taxNumber = "123456")
        {
            return new CustomerEntity
            {
                CompanyName = companyName,
                TaxNumber = taxNumber,
                UserId = _testUser.Id,
                FirstName = "Ivan",
                LastName = "Ivanov",
                Address = "ul. Bulgaria 1",
                City = "Sofia",
                PostCode = "1000",
                Country = "Bulgaria",
                Phone = "0888123456",
                Email = "ivan@example.com"
            };
        }

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<VendingDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new VendingDbContext(options);

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            _testUser = new ApplicationUser { Id = "user1", UserName = "testuser", FullName= "Test User" };

            _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_testUser);

            _controller = new CustomersController(_context, _userManagerMock.Object);

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

        [Test]
        public async Task Index_ReturnsCustomersForCurrentUser()
        {
            _context.Customers.Add(CreateValidCustomer("Test Co", "TAX001"));
            _context.Customers.Add(new CustomerEntity
            {
                CompanyName = "Other Co",
                TaxNumber = "TAX002",
                UserId = "other",
                FirstName = "Other",
                LastName = "User",
                Address = "Other St",
                City = "OtherCity",
                PostCode = "9999",
                Country = "Otherland",
                Phone = "0888999999",
                Email = "other@example.com"
            });
            await _context.SaveChangesAsync();

            var result = await _controller.Index();

            var viewResult = result as ViewResult;
            var model = viewResult?.Model as List<CustomerEntity>;

            Assert.IsNotNull(viewResult);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual("Test Co", model[0].CompanyName);
        }

        [Test]
        public async Task Create_Post_ValidCustomer_AddsToDatabase()
        {
            var customer = CreateValidCustomer("Test", "123456");

            var result = await _controller.Create(customer);
        
            var redirect = result as RedirectToActionResult;
        
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual(1, _context.Customers.Count());
            var addedCustomer = _context.Customers.FirstOrDefault(c => c.TaxNumber == "123456");
            Assert.IsNotNull(addedCustomer);
        }

        [Test]
        public async Task Create_Post_DuplicateTaxNumber_ReturnsError()
        {
            _context.Customers.Add(CreateValidCustomer("Original", "DUPL123"));
            await _context.SaveChangesAsync();

            var newCustomer = CreateValidCustomer("Duplicate", "DUPL123");

            var result = await _controller.Create(newCustomer);
            var viewResult = result as ViewResult;

            Assert.IsNotNull(viewResult);
            Assert.IsFalse(viewResult.ViewData.ModelState.IsValid);

            var errorMessage = viewResult.ViewData.ModelState[""].Errors[0].ErrorMessage;
            Assert.AreEqual("A customer with this TaxNumber already exists.", errorMessage);
        }

        [Test]
        public async Task DeleteConfirmed_DeletesCustomer()
        {
            var customer = CreateValidCustomer("ToDelete", "DEL999");
            customer.Id = 99;

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteConfirmed(99);
            var redirect = result as RedirectToActionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.IsFalse(_context.Customers.Any(c => c.Id == 99));
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
*/

