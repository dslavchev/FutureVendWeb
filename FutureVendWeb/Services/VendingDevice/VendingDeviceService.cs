using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Data.Models.VendingDevices;

namespace FutureVendWeb.Services.VendingDevice
{
    public class VendingDeviceService : IVendingDeviceService
    {
        private VendingDbContext _context;

        public VendingDeviceService(VendingDbContext context)
        {
            _context = context;
        }

        public List<GetAllVendingDevicesViewModel> GetAll(UserData userData)
        {
            return _context.VendingDevices
                .Where(x => x.UserId == userData.Id)
                .Select(x =>
                    new GetAllVendingDevicesViewModel()
                    {
                        Id = x.Id,
                        Manufacturer = x.Manufacturer,
                        Model = x.Model,
                    }
                ).ToList();
        }

        public GetVendingDeviceViewModel Get(int id)
        {
            VendingDeviceEntity vendingDevice = FindVendingDevice(id);
            
            GetVendingDeviceViewModel getVendingDeviceViewModel = new GetVendingDeviceViewModel();
            getVendingDeviceViewModel.Manufacturer = vendingDevice.Manufacturer;
            getVendingDeviceViewModel.SoftwareVersion = vendingDevice.SoftwareVersion;
            getVendingDeviceViewModel.Model = vendingDevice.Model;

            return getVendingDeviceViewModel;
        }

        public void Create(CreateVendingDeviceModel createVendingDevice, UserData userData)
        {
            VendingDeviceEntity vendingDevice = new VendingDeviceEntity();
            vendingDevice.Manufacturer = createVendingDevice.Manufacturer;
            vendingDevice.Model = createVendingDevice.Model;
            vendingDevice.SoftwareVersion = createVendingDevice.SoftwareVersion;
            vendingDevice.UserId = userData.Id;

            _context.Add( vendingDevice );
            _context.SaveChanges();
        }

        public void Update(int id, UpdateVendingDeviceModel updateVendingDevice)
        {
            VendingDeviceEntity vendingDeviceEntity =   FindVendingDevice(id);

            vendingDeviceEntity.Manufacturer = updateVendingDevice.Manufacturer;   
            vendingDeviceEntity.Model = updateVendingDevice.Model;
            vendingDeviceEntity.SoftwareVersion = updateVendingDevice.SoftwareVersion;
            _context.Update( vendingDeviceEntity );    
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            bool exist = _context.Devices.Any(x => x.VendingDeviceId == id);
            if (exist)
            {
                throw new Exception("This vending device is already used");
            }


            VendingDeviceEntity vendingDevice = FindVendingDevice(id);
            _context.VendingDevices.Remove( vendingDevice );
            _context.SaveChanges();

        }

        private VendingDeviceEntity FindVendingDevice(int id)
        {
            VendingDeviceEntity? entry = _context.VendingDevices.FirstOrDefault(vd => vd.Id == id);
            if ( entry == null )
            {
                throw new ArgumentException("Invalid vending device id");
            }
            return entry;
        }


    }
}
