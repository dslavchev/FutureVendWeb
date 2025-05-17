using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Data.Models.VendingProducts;

namespace FutureVendWeb.Services.VendingProducts
{
    public interface IVendingProductService
    {
        List<GetAllVendingProductsViewModel> GetAll(UserData userData);

        GetVendingProductModel Get(int id);

        void Create(CreateVendingProductModel createVendingProduct, UserData userData);

        void Update(int id, UpdateVendingProductModel updateVendingProduct);

        void Delete(int id);
    }
}
