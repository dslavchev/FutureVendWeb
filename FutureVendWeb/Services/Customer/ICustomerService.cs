using FutureVendWeb.Data.Models.Customer;
using FutureVendWeb.Data.Models.User;

namespace FutureVendWeb.Services.Customer
{
    public interface ICustomerService
    {
        List<GetAllCustomersWithViewModel> GetAll(UserData userData);

        GetCustomerModel Get(int id);

        void Create(CreateCustomerModel createCustomer, UserData userData);

        void Update(int id , UpdateCustomerModel updateCustomer);

        void Delete(int id);


    }
}
