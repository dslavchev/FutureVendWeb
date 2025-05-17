using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.PaymentDevice;
using FutureVendWeb.Data.Models.User;
using Microsoft.EntityFrameworkCore;

namespace FutureVendWeb.Services.PaymentDevice
{
    public class PaymentDeviceService : IPaymentDeviceService
    {
        VendingDbContext _context;
        public PaymentDeviceService(VendingDbContext vendingDbContext)
        {
            _context = vendingDbContext;
        }

        public void Create(CreatePaymentDeviceModel createPaymentDevice ,UserData user)
        {
            PaymentDeviceEntity paymentDeviceEntity = new PaymentDeviceEntity();
            paymentDeviceEntity.Manufacturer = createPaymentDevice.Manufacturer;
            paymentDeviceEntity.OSVersion = createPaymentDevice.OSVersion;
            paymentDeviceEntity.Chip = createPaymentDevice.Chip;
            paymentDeviceEntity.Name = createPaymentDevice.Name;
            paymentDeviceEntity.NFC = createPaymentDevice.NFC;
            paymentDeviceEntity.UserId = user.Id;

            _context.Add(paymentDeviceEntity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            bool exist = _context.Devices.Any(x => x.PaymentDeviceId == id);
            if (exist)
            {
                throw new Exception("This payment device is already used");
            }


            PaymentDeviceEntity paymentDeviceEntity = FindByID(id);
            
            _context.PaymentDevices.Remove(paymentDeviceEntity);
            _context.SaveChanges();
        }

        public GetPaymentDeviceModel Get(int id)
        {
            PaymentDeviceEntity paymentDeviceEntity = FindByID(id); 

            GetPaymentDeviceModel getPaymentDevice = new GetPaymentDeviceModel();

            getPaymentDevice.Id = paymentDeviceEntity.Id;
            getPaymentDevice.Manufacturer = paymentDeviceEntity.Manufacturer;
            getPaymentDevice.Name = paymentDeviceEntity.Name;
            getPaymentDevice.NFC = paymentDeviceEntity.NFC; 
            getPaymentDevice.OSVersion = paymentDeviceEntity.OSVersion;
            getPaymentDevice.Chip = paymentDeviceEntity.Chip;
            return getPaymentDevice;
        }

        public List<GetAllPaymentDevicesViewModel> GetAll(UserData user)
        {
            return _context.PaymentDevices
                .Where(x=> x.UserId == user.Id)
                .Select(x => 
                    new GetAllPaymentDevicesViewModel()
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Manufacturer = x.Manufacturer,

                    }
                ).ToList();
        }

        public void Update(int id, UpdatePaymentDeviceModel updatePaymentDevice)
        {
           PaymentDeviceEntity paymentDeviceEntity = FindByID(id);
            paymentDeviceEntity.Name = updatePaymentDevice.Name;
            paymentDeviceEntity.Manufacturer= updatePaymentDevice.Manufacturer;
            paymentDeviceEntity.NFC= updatePaymentDevice.NFC;
            paymentDeviceEntity.OSVersion= updatePaymentDevice.OSVersion;
            paymentDeviceEntity.Chip= updatePaymentDevice.Chip;
            _context.Update(paymentDeviceEntity);
            _context.SaveChanges();
        }

        private PaymentDeviceEntity FindByID(int id)
        {
            PaymentDeviceEntity? paymentDevice = _context.PaymentDevices.FirstOrDefault(x => x.Id == id);
            if (paymentDevice == null)
            {
                throw new ArgumentException("Invalid payment device id");
            }
            return paymentDevice;
        }
    }
}
