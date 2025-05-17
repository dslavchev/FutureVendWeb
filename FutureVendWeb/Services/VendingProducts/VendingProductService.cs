using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Data.Models.VendingProducts;

namespace FutureVendWeb.Services.VendingProducts
{
    public class VendingProductService : IVendingProductService
    {
        private VendingDbContext _context;

        public VendingProductService(VendingDbContext context)
        {
            _context = context;
        }

        public void Create(CreateVendingProductModel createVendingProduct, UserData userData)
        {
            ValidatePLU(-1,userData.Id,createVendingProduct.PLU);
            VendingProductEntity vendingProductEntity = new VendingProductEntity();
            vendingProductEntity.PLU = createVendingProduct.PLU;
            vendingProductEntity.Category = createVendingProduct.Category;
            vendingProductEntity.Description = createVendingProduct.Description;
            vendingProductEntity.Name = createVendingProduct.Name;
            vendingProductEntity.UserId = userData.Id;

            _context.Add(vendingProductEntity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            bool exist = _context.Transactions.Any(x => x.VendingProductId == id);
            if (exist)
            {
                throw new Exception("This vending product is already used");
            }

            VendingProductEntity entity = FindVendingProduct(id);
            _context.VendingProducts.Remove(entity);
            _context.SaveChanges();
        }

        public List<GetAllVendingProductsViewModel> GetAll(UserData userData)
        {
            return _context.VendingProducts
                .Where(x => x.UserId == userData.Id)
                .Select( x => 
                    new GetAllVendingProductsViewModel
                    {
                        Id = x.Id,
                        Description = x.Description,
                        Name = x.Name,
                    }
                ).ToList();
        }
        
        public GetVendingProductModel Get(int id)
        {
            VendingProductEntity entity = FindVendingProduct(id);
            GetVendingProductModel vendingProduct = new GetVendingProductModel();
            vendingProduct.PLU = entity.PLU;
            vendingProduct.Category = entity.Category;  
            vendingProduct.Description = entity.Description;
            vendingProduct.Name = entity.Name;

            return vendingProduct;
        }
        public void Update(int id,UpdateVendingProductModel updateVendingProduct)
        {
            VendingProductEntity entity = FindVendingProduct(id);
            ValidatePLU(id,entity.UserId,updateVendingProduct.PLU);
            entity.PLU = updateVendingProduct.PLU;
            entity.Category = updateVendingProduct.Category;
            entity.Description = updateVendingProduct.Description;
            entity.Name = updateVendingProduct.Name;

            _context.Update(entity);
            _context.SaveChanges();
        }

        private VendingProductEntity FindVendingProduct(int id)
        {
            VendingProductEntity? entity = _context.VendingProducts.FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                throw new ArgumentException("Invalid vending product id");
            }
            return entity;
        }
        private void ValidatePLU(int id, int userId, string PLU)
        {
            bool exists = _context.VendingProducts.Any(c => c.Id != id && c.UserId == userId && c.PLU == PLU);
            if (exists)
            {
                throw new Exception("Product with this PLU already exists.");
            }
        }
    }
}
