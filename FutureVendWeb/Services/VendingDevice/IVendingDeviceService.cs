using FutureVendWeb.Data.Models.User;
using FutureVendWeb.Data.Models.VendingDevices;

namespace FutureVendWeb.Services.VendingDevice
{
    public interface IVendingDeviceService
    {
        List<GetAllVendingDevicesViewModel> GetAll(UserData userData);

        GetVendingDeviceViewModel Get(int id);

        void Create(CreateVendingDeviceModel createVendingDevice, UserData userData);

        void Update(int id, UpdateVendingDeviceModel updateVendingDevice);

        void Delete(int id);


    }
}
