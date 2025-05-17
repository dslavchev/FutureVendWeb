using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.Device;
using FutureVendWeb.Data.Models.User;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FutureVendWeb.Services.Device
{
    public class DeviceService : IDeviceService
    {
        VendingDbContext _context;
        public DeviceService(VendingDbContext context)
        {
            _context = context;
        }

        public void Create(CreateDeviceModel createDevice, UserData userData)
        {
            ValidatePaymentDeviceSerial(-1,userData.Id,createDevice.PaymentDeviceSerial);
            ValidateVendingDeviceSerial(-1,userData.Id,createDevice.VendingDeviceSerial);
            DeviceEntity deviceEntity = new DeviceEntity(); 
            deviceEntity.AcceptCard = createDevice.AcceptCard;
            deviceEntity.AcceptCash = createDevice.AcceptCash;
            deviceEntity.VendingDeviceId = createDevice.VendingDeviceId;
            deviceEntity.CustomerId = createDevice.CustomerId;
            deviceEntity.PaymentDeviceId = createDevice.PaymentDeviceId;
            deviceEntity.PaymentDeviceSerial = createDevice.PaymentDeviceSerial;
            deviceEntity.VendingDeviceSerial = createDevice.VendingDeviceSerial;
            deviceEntity.LocationLat = createDevice.LocationLat;
            deviceEntity.LocationLon = createDevice.LocationLon;
            deviceEntity.UserId = userData.Id;
            
            _context.Add(deviceEntity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            bool exist = _context.Transactions.Any(x => x.DeviceId == id);
            if (exist)
            {
                throw new Exception("This device is already used");
            }

            DeviceEntity deviceEntity = FindById(id);

            _context.Remove(deviceEntity);
            _context.SaveChanges();
        }


        public EditDeviceModel GetEditDevice(int id)
        {
            DeviceEntity device = FindById(id);

            EditDeviceModel getDevice = new EditDeviceModel();

            getDevice.AcceptCash = device.AcceptCash;
            getDevice.AcceptCard = device.AcceptCard;
            getDevice.PaymentDeviceId = device.PaymentDeviceId;
            getDevice.VendingDeviceId= device.VendingDeviceId;
            getDevice.CustomerId = device.CustomerId;
            getDevice.LocationLon = device.LocationLon;
            getDevice.LocationLat = device.LocationLat;
            getDevice.PaymentDeviceSerial= device.PaymentDeviceSerial;
            getDevice.VendingDeviceSerial= device.VendingDeviceSerial;
            getDevice.PaymentDevices = _context.PaymentDevices
            .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} ({p.Manufacturer})"
                })
            .ToList();
            getDevice.VendingDevices = _context.VendingDevices
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Model} ({p.Manufacturer})"
            }).ToList();
            getDevice.Customer = _context.Customers
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.FirstName} ({p.LastName}) ({p.CompanyName})"
            }).ToList();
            return getDevice;
        }

        public List<GetAllDevicesWithViewModel> GetAll(UserData userData)
        {
            return _context.Devices
               .Where(x => x.UserId == userData.Id)
               .Include(x => x.PaymentDevice)
               .Include(x => x.VendingDevice)
               .Include(x => x.Customer)
               .Select(x => new GetAllDevicesWithViewModel
               {
                   Id = x.Id,
                   PaymentDeviceSerial = x.PaymentDeviceSerial,
                   VendingDeviceSerial = x.VendingDeviceSerial,
                   PaymentDeviceInformation = x.PaymentDevice.Name + " "+ x.PaymentDevice.Manufacturer,
                   VendingDeviceInformation = x.VendingDevice.Model + x.VendingDevice.Manufacturer,
                   CustomerInformation = x.Customer.FirstName + " " + x.Customer.LastName + " "+x.Customer.CompanyName
               })
            .ToList();
        }

        public CreateDeviceModel GetCreateDevice(UserData userData)
        {
            CreateDeviceModel device = new CreateDeviceModel();
            device.PaymentDevices = _context.PaymentDevices
                .Where(x => x.UserId == userData.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name + " - " + x.Manufacturer
                })
                .ToList();

            device.VendingDevices = _context.VendingDevices
                .Where(x => x.UserId == userData.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Model + " - " + x.Manufacturer
                })
                .ToList();

            device.Customers = _context.Customers
                .Where(x => x.UserId == userData.Id)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FirstName + " " + x.LastName + " - " + x.CompanyName
                })
                .ToList();

            return device;
        }

        public void Update(int id, UpdateDeviceModel updateDevice)
        {
            DeviceEntity device = FindById( id );
            ValidatePaymentDeviceSerial(id,device.UserId,updateDevice.PaymentDeviceSerial);
            ValidateVendingDeviceSerial(id,device.UserId,updateDevice.VendingDeviceSerial);
            device.AcceptCash=updateDevice.AcceptCash;
            device.AcceptCard=updateDevice.AcceptCard;
            device.VendingDeviceId=updateDevice.VendingDeviceId;
            device.VendingDeviceSerial=updateDevice.VendingDeviceSerial;
            device.CustomerId=updateDevice.CustomerId;
            device.PaymentDeviceId=updateDevice.PaymentDeviceId;
            device.LocationLat=updateDevice.LocationLat;
            device.LocationLon=updateDevice.LocationLon;
            device.PaymentDeviceSerial=updateDevice.PaymentDeviceSerial;

            _context.Update(device);
            _context.SaveChanges();

        }

        private DeviceEntity FindById(int id)
        {
            DeviceEntity? entity = _context.Devices.FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                throw new ArgumentException("Invalid device id");
            }
            return entity;
        }

        public DeviceDetailsModel GetDeviceDetails(int id)
        {
            DeviceEntity? device = _context.Devices
                        .Include(x => x.VendingDevice)
                        .Include(x => x.PaymentDevice)
                        .Include(x => x.Customer)
                        .FirstOrDefault(x => x.Id == id);
            if (device == null)
            {
                throw new ArgumentException("Invalid device id");
            }

            DeviceDetailsModel getDevice = new DeviceDetailsModel();
            getDevice.AcceptCash = device.AcceptCash;
            getDevice.AcceptCard = device.AcceptCard;
            getDevice.PaymentDeviceId = device.PaymentDeviceId;
            getDevice.VendingDeviceId = device.VendingDeviceId;
            getDevice.CustomerId = device.CustomerId;
            getDevice.LocationLon = device.LocationLon;
            getDevice.LocationLat = device.LocationLat;
            getDevice.PaymentDeviceSerial = device.PaymentDeviceSerial;
            getDevice.VendingDeviceSerial = device.VendingDeviceSerial;
            getDevice.VendingDeviceInformation = device.VendingDevice.Model + " " 
                + device.VendingDevice.Manufacturer;
            getDevice.PaymentDeviceInformation = device.PaymentDevice.Name + " " 
                + device.PaymentDevice.Manufacturer;
            getDevice.CustomerInformation = device.Customer.FirstName + " " 
                                                + device.Customer.LastName + " " 
                                                + device.Customer.CompanyName;
            return getDevice;
        }

        private void ValidatePaymentDeviceSerial(int id, int userId, string serialNumber)
        {
            bool exists = _context.Devices.Any(c => c.Id != id  && c.PaymentDeviceSerial == serialNumber);
            if (exists)
            {
                throw new Exception("Device with this payment device serial already exists.");
            }
        }
        private void ValidateVendingDeviceSerial(int id, int userId, string serialNumber)
        {
            bool exists = _context.Devices.Any(c => c.Id != id &&  c.VendingDeviceSerial == serialNumber);
            if (exists)
            {
                throw new Exception("Device with this vending device serial already exists.");
            }
        }
    }
}
