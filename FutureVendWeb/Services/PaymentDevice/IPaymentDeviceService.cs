using FutureVendWeb.Data.Models.PaymentDevice;
using FutureVendWeb.Data.Models.User;

namespace FutureVendWeb.Services.PaymentDevice
{
    public interface IPaymentDeviceService
    {
        List<GetAllPaymentDevicesViewModel> GetAll(UserData userData);

        void Create (CreatePaymentDeviceModel createPaymentDevice, UserData userData);

        GetPaymentDeviceModel Get(int id);
        void Update(int id, UpdatePaymentDeviceModel updatePaymentDevice);

        void Delete (int id);
    }
}
