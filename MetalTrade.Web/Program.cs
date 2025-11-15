using MetalTrade.Business;
using MetalTrade.Business.Interfaces;
using MetalTrade.Business.Services;
using MetalTrade.DataAccess;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interceptors;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace MetalTrade.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services
                .AddControllersWithViews()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization();

            builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
            
            builder.Services.AddSingleton<SoftDeleteInterceptor>();
            builder.Services.AddDbContext<MetalTradeDbContext>((serviceProvider, options) =>
            {
                var conn = builder.Configuration.GetConnectionString("DefaultConnection");
                options.UseNpgsql(conn)
                    .AddInterceptors(serviceProvider.GetRequiredService<SoftDeleteInterceptor>());
            });
            
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
            
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
            builder.Services.AddAutoMapper(typeof(MetalTrade.Business.Common.Mapping.MappingProfile));
            builder.Services.AddAutoMapper(typeof(MetalTrade.Web.Common.Mapping.MappingProfile));


            var app = builder.Build();
            
            var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ru") };
            var localizationOptions = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("ru"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            app.UseRequestLocalization(localizationOptions);
            
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                    await AdminInitializer.SeedAdminUser(roleManager, userManager);

                    var context = services.GetRequiredService<MetalTradeDbContext>();
                    await UserInitializer.SeedUserAsync(userManager);
                    await ProductInitializer.SeedProductAsync(context);
                    await AdvertisementInitializer.SeedAdvertisementAsync(context);
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

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
