using MetalTrade.Business;
using MetalTrade.Business.Helpers;
using MetalTrade.Business.Interfaces;
using MetalTrade.Business.Services;
using MetalTrade.DataAccess;
using MetalTrade.DataAccess.Data;
using MetalTrade.DataAccess.Interceptors;
using MetalTrade.DataAccess.Interfaces.Repositories;
using MetalTrade.DataAccess.Repositories;
using MetalTrade.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using MetalTrade.Web.Hubs;

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
                        
            builder.Services.AddAutoMapper(typeof(Business.Common.Mapping.MappingProfile));
            builder.Services.AddAutoMapper(typeof(Web.Common.Mapping.MappingProfile));

            builder.Services.AddScoped<IPromotionStrategyProvider, PromotionStrategyProvider>();
            builder.Services.AddScoped<IPromotionService, PromotionService>();
            builder.Services.AddScoped<IPromotionValidator, PromotionValidator>();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();


            builder.Services.AddScoped<IMetalService, MetalService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
            builder.Services.AddScoped<ITopAdvertisementRepository, TopAdvertisementRepository>();
            builder.Services.AddScoped<ITopUserRepository, TopUserRepository>();

            builder.Services.AddScoped<IAdvertisementImportService, AdvertisementImportService>();
            builder.Services.AddScoped<IChatService, ChatService>();

            builder.Services.AddSignalR();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/Advertisement"))
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

            var app = builder.Build();
            
            var supportedCultures = new[] { new CultureInfo("ru"), new CultureInfo("ky-KG") };
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
                app.UseExceptionHandler("/Account/Error");
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
                pattern: "{controller=Advertisement}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapHub<ChatHub>("/chatHub");
            
            
            app.Run();
        }
    }
}
