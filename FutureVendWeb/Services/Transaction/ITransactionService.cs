using FutureVendWeb.Data.Models.Transaction;
using FutureVendWeb.Data.Models.User;

namespace FutureVendWeb.Services.Transaction
{
    public interface ITransactionService
    {
        List<GetAllTransactionWithViewModel> GetAll(UserData userData);

        GetTransactionModel Get(int id);

        void Create(CreateTransactionModel createTransaction);

        void Delete(int id);
    }
}
