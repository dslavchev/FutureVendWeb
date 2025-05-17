using Humanizer;

namespace FutureVendWeb.Data.Models.Transaction
{
    public class CreateTransactionModel
    {
       public  string PaymentType { get; set; } 
        public decimal Amount { get; set; } 
        public  string ItemNumber {  get; set; }   
        public string CurrencyCode {  get; set; }
        public string SerialNumber {  get; set; }
        public DateTime CreatedAt {  get; set; }
    }
}
