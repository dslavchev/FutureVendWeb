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
{
    [TestFixture]
    public class VendingProductsControllerTests
    {
        private VendingProductsController _controller;
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

            _controller = new VendingProductsController(_context, _userManagerMock.Object);
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

        private async Task<VendingProductEntity> CreateValidVendingProductAsync()
        {
            var product = new VendingProductEntity
            {
                PLU = "12345",
                Name = "Product1",
                Description = "Test product",
                Category = "Category1",
                UserId = _testUser.Id
            };

            _context.VendingProducts.Add(product);
            await _context.SaveChangesAsync();

            return product;
        }

        [Test]
        public async Task Index_ReturnsProductsForUser()
        {
            var product = await CreateValidVendingProductAsync();
            var result = await _controller.Index();

            var view = result as ViewResult;
            var model = view?.Model as List<VendingProductEntity>;

            Assert.IsNotNull(view);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(product.Name, model[0].Name);
        }

        [Test]
        public async Task Create_Post_ValidProduct_AddsToDatabase()
        {
            var product = new VendingProductEntity
            {
                PLU = "67890",
                Name = "Product2",
                Description = "Another test product",
                Category = "Category2"
            };

            var result = await _controller.Create(product);
            var redirect = result as RedirectToActionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual(1, _context.VendingProducts.Count());
        }

        [Test]
        public async Task Edit_Post_ValidProduct_UpdatesDatabase()
        {
            var product = await CreateValidVendingProductAsync();
            product.Name = "UpdatedProduct";

            var result = await _controller.Edit(product.Id, product);
            var redirect = result as RedirectToActionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            var updatedProduct = await _context.VendingProducts.FindAsync(product.Id);
            Assert.AreEqual("UpdatedProduct", updatedProduct.Name);
        }

        [Test]
        public async Task DeleteConfirmed_RemovesProduct()
        {
            var product = await CreateValidVendingProductAsync();

            var result = await _controller.DeleteConfirmed(product.Id);
            var redirect = result as RedirectToActionResult;

            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.IsNull(await _context.VendingProducts.FindAsync(product.Id));
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
