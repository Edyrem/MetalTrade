using MetalTrade.Business;
using MetalTrade.Business.Interfaces;
using MetalTrade.Business.Services;
using MetalTrade.DataAccess;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            
            builder.Services.AddDbContext<MetalTradeDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Identity + Password Options + Token Providers
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<MetalTradeDbContext>()
                .AddDefaultTokenProviders();

            // Business Layer services
            builder.Services.AddScoped<IAccountService, AccountService>();

            var app = builder.Build();

            // Создание администратора при старте
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                    await AdminInitializer.SeedAdminUser(roleManager, userManager);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
