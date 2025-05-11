using FutureVendWeb.Data;
using FutureVendWeb.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace FutureVendWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

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

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<VendingDbContext>();
                dbContext.CreateInsertTransactionProcedure();
            }
            app.MapPost("/Account/Logout", async context =>
            {
                var signInManager = context.RequestServices.GetRequiredService<SignInManager<ApplicationUser>>();
                await signInManager.SignOutAsync();
                context.Response.Redirect("/Account/Login"); // redirect to Login
            });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
