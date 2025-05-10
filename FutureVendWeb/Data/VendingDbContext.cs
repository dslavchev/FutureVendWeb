using FutureVendWeb.Data.Entities;
using FutureVendWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FutureVendWeb.Data
{
    public class VendingDbContext : IdentityDbContext<ApplicationUser>
    {
        public VendingDbContext(DbContextOptions<VendingDbContext> options) : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<PaymentDevice> PaymentDevices { get; set; }
        public DbSet<VendingDevice> VendingDevices { get; set; }
        public DbSet<VendingProduct> VendingProducts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CustomerVendingProduct> CustomerVendingProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Добави уникален индекс по UserId + TaxNumber
            modelBuilder.Entity<Customer>()
                .HasIndex(c => new { c.UserId, c.TaxNumber })
                .IsUnique();

            modelBuilder.Entity<Device>()
                .HasIndex(c => new { c.PaymentDeviceSerial, c.VendingDeviceSerial })
                .IsUnique();

            modelBuilder.Entity<VendingProduct>()
               .HasIndex(v => new { v.UserId, v.PLU })
               .IsUnique();
        }

        // Create procedure to insert transaction data
        public void CreateInsertTransactionProcedure()
        {
            var sql = @"
                DROP PROCEDURE IF EXISTS insert_transaction;

                CREATE PROCEDURE insert_transaction(
                    IN payment_type ENUM('cash', 'card'),
                    IN amount DECIMAL(10,2),
                    IN item_number VARCHAR(32),
                    IN currency_code ENUM('BGN','EUR'),
                    IN serial_number VARCHAR(32),
                    IN created_at TIMESTAMP
                )
                BEGIN
                    DECLARE device_id_value INT;
                    DECLARE device_user_id_value VARCHAR(255);
                    DECLARE vending_product_id_value INT;
                    
                    SELECT id, userid INTO device_id_value, device_user_id_value FROM devices WHERE PaymentDeviceSerial = serial_number;
                    SELECT id INTO vending_product_id_value FROM vendingproducts WHERE plu = item_number AND userid = device_user_id_value;

                    INSERT INTO transactions (
                        amount, currency, createdat, deviceid, vendingproductid, paymenttype
                    )
                    VALUES (
                        amount, currency_code, created_at, device_id_value, vending_product_id_value, payment_type
                    );
                END;";

            // Изпълнение на SQL
            this.Database.ExecuteSqlRaw(sql);
        }
    }
}
