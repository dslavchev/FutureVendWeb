using FutureVendWeb.Data;
using FutureVendWeb.Data.Entities;
using FutureVendWeb.Data.Models.Transaction;
using FutureVendWeb.Data.Models.User;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Mysqlx.Crud;
using System.Transactions;

namespace FutureVendWeb.Services.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly VendingDbContext _context;

        public TransactionService(VendingDbContext vendingDbContext)
        {
            _context = vendingDbContext;
        }

        public void Create(CreateTransactionModel createTransaction)
        {
            DeviceEntity? device = _context.Devices
                .FirstOrDefault( x => x.PaymentDeviceSerial == createTransaction.SerialNumber);

            if (device == null)
            {
                throw new ArgumentException("Invalid device serial number");
            }

            VendingProductEntity? vendingProduct = _context.VendingProducts
                .FirstOrDefault(x => x.PLU == createTransaction.ItemNumber && x.UserId == device.UserId);

            if (vendingProduct == null)
            {
                throw new ArgumentException("Invalid vending product PLU");
            }

            TransactionEntity transactionEntity = new TransactionEntity();

            transactionEntity.Currency = createTransaction.CurrencyCode;
            transactionEntity.Amount = createTransaction.Amount;
            transactionEntity.CreatedAt = createTransaction.CreatedAt;
            transactionEntity.PaymentType = createTransaction.PaymentType;
            transactionEntity.DeviceId = device.Id;
            transactionEntity.VendingProductId = vendingProduct.Id;
            
            _context.Add(transactionEntity);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            TransactionEntity transactionEntity = FindById(id);

            _context.Remove(transactionEntity);
            _context.SaveChanges();
        }


        public GetTransactionModel Get(int id)
        {
            TransactionEntity transactionEntity = FindById(id);
            
            GetTransactionModel getTransaction = new GetTransactionModel();
            getTransaction.Currency = transactionEntity.Currency;
            getTransaction.Amount = transactionEntity.Amount;
            getTransaction.CreatedAt = transactionEntity.CreatedAt;
            getTransaction.PaymentType = transactionEntity.PaymentType;

            DeviceEntity? device = _context.Devices
                .Include(x => x.PaymentDevice)
                .Include(x => x.Customer)
                .FirstOrDefault(x => x.Id == transactionEntity.DeviceId);
            if (device == null)
            {
                throw new ArgumentException("Invalid device id");
            }

            getTransaction.DeviceInformation = device.PaymentDevice.Name + " " + device.PaymentDevice.Manufacturer;

            getTransaction.CustomerInformation = device.Customer.FirstName + " " 
                                                 + device.Customer.LastName + " " + device.Customer.CompanyName;

            VendingProductEntity? vendingProductEntity = _context.VendingProducts
                .FirstOrDefault(x => x.Id == transactionEntity.VendingProductId);
            if (vendingProductEntity == null)
            {
                throw new ArgumentException("Invalid vending product id");
            }

            getTransaction.VendingProductInformation = vendingProductEntity.Name + " " + vendingProductEntity.Description;

            return getTransaction;
        }

        public List<GetAllTransactionWithViewModel> GetAll(UserData user)
        {
            return _context.Transactions
                .Include(x => x.Device)
                    .ThenInclude(x => x.Customer)
                .Include(x => x.Device)
                    .ThenInclude(x => x.PaymentDevice)
                .Include(x => x.VendingProduct)
                .Where(x => x.Device.UserId == user.Id)
                .Select(x =>
                new GetAllTransactionWithViewModel()
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    Currency = x.Currency,
                    CustomerInformation = x.Device.Customer.FirstName + " " + x.Device.Customer.LastName,
                    DeviceInformation = x.Device.PaymentDevice.Name + " " + x.Device.PaymentDevice.Manufacturer,
                    VendingProductInformation = x.VendingProduct.Name + " " + x.VendingProduct.Description
                }
                ).ToList();
        }

        private TransactionEntity FindById(int id)
        {
            TransactionEntity? entity = _context.Transactions.FirstOrDefault(x => x.Id == id);
            if (entity == null)
            {
                throw new ArgumentException("Invalid transaction id");
            }
            return entity;
        }
    }
}
