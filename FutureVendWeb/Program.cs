using FutureVendWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using FutureVendWeb.Services.VendingDevice;
using FutureVendWeb.Services.VendingProducts;
using FutureVendWeb.Services.PaymentDevice;
using FutureVendWeb.Services.Customer;
using FutureVendWeb.Services.Device;
using FutureVendWeb.Services.Transaction;
using FutureVendWeb.Services.User;

namespace FutureVendWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();

            builder.Services.AddHttpContextAccessor();

            // Add Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "FutureVend API",
                    Version = "v1",
                    Description = "Decription for the public API to access from payment devices"
                });
            });

            // Add my services
            builder.Services.AddTransient<IVendingDeviceService, VendingDeviceService>();

            builder.Services.AddTransient<IVendingProductService, VendingProductService>();

            builder.Services.AddTransient<IPaymentDeviceService, PaymentDeviceService>();

            builder.Services.AddTransient<ICustomerService, CustomerService>();

            builder.Services.AddTransient<IDeviceService, DeviceService>();

            builder.Services.AddTransient<ITransactionService, TransactionService>();

            builder.Services.AddTransient<IUserService , UserService>();
            
            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add sessions support
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            // Add database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (connectionString == null)
            {
                throw new ArgumentException("Missing connection string");
            }
            builder.Services.AddDbContext<VendingDbContext>(options => options.UseMySQL(connectionString));

            var app = builder.Build();

            // Enable Swagger only in development mode
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
