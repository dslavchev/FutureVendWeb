using FutureVendWeb.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FutureVendWeb.Data
{
    public class VendingDbContext : DbContext
    {
        public VendingDbContext(DbContextOptions<VendingDbContext> options) : base(options)
        {
        }

        public DbSet<CustomerEntity> Customers { get; set; }
        public DbSet<DeviceEntity> Devices { get; set; }
        public DbSet<PaymentDeviceEntity> PaymentDevices { get; set; }
        public DbSet<VendingDeviceEntity> VendingDevices { get; set; }
        public DbSet<VendingProductEntity> VendingProducts { get; set; }
        public DbSet<TransactionEntity> Transactions { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Добави уникален индекс по UserId + TaxNumber
            modelBuilder.Entity<CustomerEntity>()
                .HasIndex(c => new { c.UserId, c.TaxNumber })
                .IsUnique();

            modelBuilder.Entity<DeviceEntity>()
                .HasIndex(c => new { c.PaymentDeviceSerial, c.VendingDeviceSerial })
                .IsUnique();

            modelBuilder.Entity<VendingProductEntity>()
               .HasIndex(v => new { v.UserId, v.PLU })
               .IsUnique();


        }

    
    }
}

//dotnet ef database drop
//dotnet ef migrations add AddProductTable
//dotnet ef database update

