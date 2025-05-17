using FutureVendWeb.Data.Models.Device;
using FutureVendWeb.Data.Models.User;

namespace FutureVendWeb.Services.Device
{
    public interface IDeviceService
    {
        List<GetAllDevicesWithViewModel> GetAll(UserData userData);

        void Create(CreateDeviceModel createDevice, UserData userData);

        CreateDeviceModel GetCreateDevice(UserData userData);

        DeviceDetailsModel GetDeviceDetails(int id);
        EditDeviceModel GetEditDevice(int id);
        void Update (int id , UpdateDeviceModel updateDevice);

        void Delete (int id);
    }
}
