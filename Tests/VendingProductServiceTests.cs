using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Data.Models.VendingProducts;
using FutureVendWeb.Services.VendingProducts;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;

namespace FutureVendWeb.Tests.Services
{
    public class VendingProductServiceTests
    {
        private VendingDbContext _context;
        private VendingProductService _service;
        private UserData _user;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<VendingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new VendingDbContext(options);
            _service = new VendingProductService(_context);
            _user = new UserData { Id = 1 };
        }
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
        [Test]
        public void Create_Should_Add_New_Product()
        {
            var model = new CreateVendingProductModel
            {
                PLU = "123",
                Name = "Choco Bar",
                Description = "Sweet",
                Category = "Snacks"
            };

            _service.Create(model, _user);

            var product = _context.VendingProducts.FirstOrDefault();
            Assert.NotNull(product);
            Assert.AreEqual("Choco Bar", product.Name);
            Assert.AreEqual("123", product.PLU);
        }

        [Test]
        public void Create_With_Existing_PLU_Should_Throw()
        {
            _context.VendingProducts.Add(new VendingProductEntity
            {
                PLU = "999",
                UserId = 1,
                Name = "Existing",
                Description = "Sweet",
                Category = "Snacks"
            });
            _context.SaveChanges();

            var model = new CreateVendingProductModel
            {
                PLU = "999",
                Name = "New One",
                Description = "Sweet",
                Category = "Snacks"
            };

            var ex = Assert.Throws<Exception>(() => _service.Create(model, _user));
            Assert.AreEqual("Product with this PLU already exists.", ex.Message);
        }

        [Test]
        public void GetAll_Should_Return_User_Products()
        {
            _context.VendingProducts.AddRange(
                new VendingProductEntity { Name = "User1Product", PLU = "1",
                    Description = "Sweet",
                    Category = "Snacks", UserId = 1 },
                new VendingProductEntity { Name = "User2Product", PLU = "2",
                    Description = "Sweet",
                    Category = "Snacks",UserId = 2 }
            );
            _context.SaveChanges();

            var result = _service.GetAll(_user);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("User1Product", result[0].Name);
        }

        [Test]
        public void Get_Should_Return_Valid_Product()
        {
            _context.VendingProducts.Add(new VendingProductEntity
            {
                Id = 10,
                PLU = "321",
                Name = "Coke",
                Category = "Drinks",
                Description = "Can",
                UserId = 1
            });
            _context.SaveChanges();

            var result = _service.Get(10);
            Assert.AreEqual("Coke", result.Name);
            Assert.AreEqual("321", result.PLU);
        }

        [Test]
        public void Get_With_Invalid_Id_Should_Throw()
        {
            var ex = Assert.Throws<ArgumentException>(() => _service.Get(999));
            Assert.AreEqual("Invalid vending product id", ex.Message);
        }

        [Test]
        public void Update_Should_Modify_Product()
        {
            _context.VendingProducts.Add(new VendingProductEntity
            {
                Id = 5,
                PLU = "888",
                Name = "Old",
                Description = "Sweet",
                Category = "Snacks",
                UserId = 1
            });
            _context.SaveChanges();

            var model = new UpdateVendingProductModel
            {
                PLU = "999",
                Name = "New",
                Category = "UpdatedCat",
                Description = "UpdatedDesc"
            };

            _service.Update(5, model);

            var updated = _context.VendingProducts.Find(5);
            Assert.AreEqual("New", updated.Name);
            Assert.AreEqual("999", updated.PLU);
        }

        [Test]
        public void Update_With_Duplicate_PLU_Should_Throw()
        {
            _context.VendingProducts.AddRange(
                new VendingProductEntity { Id = 1, PLU = "111", Name = "Product1",
                    Description = "Sweet",
                    Category = "Snacks", UserId = 1 },
                new VendingProductEntity { Id = 2, PLU = "222", Name = "Product2",
                    Description = "Sweet",
                    Category = "Snacks", UserId = 1 }
            );
            _context.SaveChanges();

            var model = new UpdateVendingProductModel
            {
                PLU = "222",
                Name = "DupPLU"
            };

            var ex = Assert.Throws<Exception>(() => _service.Update(1, model));
            Assert.AreEqual("Product with this PLU already exists.", ex.Message);
        }

        [Test]
        public void Delete_Should_Remove_Product()
        {
            _context.VendingProducts.Add(new VendingProductEntity
            {
                Id = 3,
                Name = "ToDelete",
                Description = "Sweet",
                Category = "Snacks",
                PLU = "1234",
                UserId = 1
            });
            _context.SaveChanges();

            _service.Delete(3);

            var exists = _context.VendingProducts.Any(x => x.Id == 3);
            Assert.IsFalse(exists);
        }

        [Test]
        public void Delete_Used_In_Transaction_Should_Throw()
        {
            _context.VendingProducts.Add(new VendingProductEntity
            {
                Id = 7,
                Name = "UsedProduct",
                Description = "Sweet",
                Category = "Snacks",
                PLU= "123",
                UserId = 1
            });
            _context.Transactions.Add(new TransactionEntity
            {
                Id = 1,
                Currency = "BGN",
                PaymentType = "card",
                VendingProductId = 7
            });
            _context.SaveChanges();

            var ex = Assert.Throws<Exception>(() => _service.Delete(7));
            Assert.AreEqual("This vending product is already used", ex.Message);
        }
    }
}
