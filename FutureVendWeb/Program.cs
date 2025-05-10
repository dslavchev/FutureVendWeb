using FutureVendWeb.Data;
using FutureVendWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace FutureVendWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add database
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            if (connectionString == null)
            {
                throw new ArgumentException("Missing connection string");
            }
            builder.Services.AddDbContext<VendingDbContext>(options => options.UseMySQL(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
            }).AddEntityFrameworkStores<VendingDbContext>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<VendingDbContext>();
                dbContext.CreateInsertTransactionProcedure();
            }
            
            // Ако искате да добавите специален маршрут за Logout
            app.MapPost("/Account/Logout", async context =>
            {
                var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                await signInManager.SignOutAsync();
                context.Response.Redirect("/Account/Login"); // Пренасочване към Login
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
