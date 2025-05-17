using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.Customer;
using FutureVendWeb.Data.Models.User;

namespace FutureVendWeb.Services.Customer
{
    public class CustomerService:ICustomerService   
    {
        VendingDbContext _context;
        public CustomerService(VendingDbContext context)
        {
            _context = context;
        }

        public void Create(CreateCustomerModel createCustomer, UserData userData)
        {
            ValidateTaxNumber(-1,userData.Id , createCustomer.TaxNumber);

            CustomerEntity customer = new CustomerEntity();
            customer.Address = createCustomer.Address;
            customer.City = createCustomer.City;
            customer.Email = createCustomer.Email;
            customer.Phone = createCustomer.Phone;
            customer.FirstName = createCustomer.FirstName;
            customer.LastName = createCustomer.LastName;
            customer.Country = createCustomer.Country;
            customer.CompanyName = createCustomer.CompanyName;
            customer.TaxNumber = createCustomer.TaxNumber;
            customer.PostCode = createCustomer.PostCode;
            customer.UserId = userData.Id;

            _context.Add(customer);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            CustomerEntity customer = FindById(id);
            bool exist = _context.Devices.Any(x => x.CustomerId == id);
            if (exist)
            {
                throw new Exception("This customer is already used");
            }

            _context.Remove(customer);
            _context.SaveChanges();
        }

        public GetCustomerModel Get(int id)
        {
            CustomerEntity customer = FindById(id);

            GetCustomerModel getCustomer = new GetCustomerModel();

            getCustomer.Address = customer.Address;
            getCustomer.City = customer.City;
            getCustomer.Email = customer.Email;
            getCustomer.Phone = customer.Phone;
            getCustomer.FirstName = customer.FirstName;
            getCustomer.LastName = customer.LastName;
            getCustomer.Country = customer.Country;
            getCustomer.CompanyName = customer.CompanyName;
            getCustomer.TaxNumber = customer.TaxNumber;
            getCustomer.PostCode = customer.PostCode;
       
            return getCustomer; 
        }

        public List<GetAllCustomersWithViewModel> GetAll(UserData userData)
        {
            return _context.Customers
                .Where(x => x.UserId == userData.Id)
                .Select(x => new GetAllCustomersWithViewModel()
                    {
                        Id = x.Id,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        CompanyName = x.CompanyName,

                    }
            ).ToList();
        }

        public void Update(int id, UpdateCustomerModel updateCustomer)
        {

            CustomerEntity customer = FindById(id);
            ValidateTaxNumber(id,customer.UserId,updateCustomer.TaxNumber);
            customer.FirstName = updateCustomer.FirstName;
            customer.LastName = updateCustomer.LastName;
            customer.Country = updateCustomer.Country;
            customer.Email = updateCustomer.Email;
            customer.Phone = updateCustomer.Phone;
            customer.City = updateCustomer.City;
            customer.TaxNumber = updateCustomer.TaxNumber;
            customer.CompanyName = updateCustomer.CompanyName;
            customer.PostCode = updateCustomer.PostCode;    
            customer.Address = updateCustomer.Address;
            
            _context.Update(customer);
            _context.SaveChanges();
        }

        private CustomerEntity FindById(int id)
        {
            CustomerEntity? entity = _context.Customers.FirstOrDefault(x => x.Id == id);

            if (entity == null)
            {
                throw new ArgumentException("Invalid customer id");
            }

            return entity;
        }

        private void ValidateTaxNumber(int id , int userId,string taxNumber)
        {
            bool exists = _context.Customers.Any(c => c.Id!=id && c.UserId == userId && c.TaxNumber == taxNumber);
            if (exists)
            {
                throw new Exception("Client with this tax number already exists.");
            }
        }
    }
}
