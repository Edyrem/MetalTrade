using MetalTrade.Application.Patterns.Strategy.Advertisement.Interfaces;
using MetalTrade.Application.Patterns.Strategy.Advertisement.Strategies;
using MetalTrade.Business;
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
using Microsoft.Extensions.DependencyInjection;
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

            builder.Services.AddAutoMapper(typeof(Business.Common.Mapping.MappingProfile));
            builder.Services.AddAutoMapper(typeof(Web.Common.Mapping.MappingProfile));


            builder.Services.AddScoped<IMetalService, MetalService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IImageUploadService, ImageUploadService>();
            builder.Services.AddScoped<ICommercialService, CommercialService>();
            builder.Services.AddScoped<IPromotionService, PromotionService>();
            
            var strategyType = builder.Configuration.GetValue<string>("Promotion:Strategy") ?? "TimeBased";
            var minViews = builder.Configuration.GetValue<int>("Promotion:MinViews");
            var minRating = builder.Configuration.GetValue<decimal>("Promotion:MinRating");
            builder.Services.AddScoped<IPromotionStrategy>(sp => strategyType switch
            {
                "TimeBased" => new TimeBasedPromotionStrategy(),
                "ViewsBased" => new ViewsBasedPromotionStrategy(minViews),
                "RatingBased" => new RatingBasedPromotionStrategy(minRating),
                _ => new TimeBasedPromotionStrategy()
            });

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

            app.Run();
        }
    }
}
