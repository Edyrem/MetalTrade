using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess;

public class ProductInitializer
{
    public static async Task SeedProductAsync(MetalTradeDbContext context)
    {
        if (!await context.MetalTypes.AnyAsync())
        {
            var metals = new[]
            {
                new MetalType { Name = "сталь" },
                new MetalType { Name = "железо" },
                new MetalType { Name = "медь" },
                new MetalType { Name = "алюминий" }
            };
            context.MetalTypes.AddRange(metals);
            await context.SaveChangesAsync();
        }

        if (!await context.Products.AnyAsync())
        {
            var iron = context.MetalTypes.FirstOrDefault(p => p.Name.ToLower() == "железо");
            var aluminum = context.MetalTypes.FirstOrDefault(p => p.Name.ToLower() == "алюминий");
            var steel = context.MetalTypes.FirstOrDefault(p => p.Name.ToLower() == "сталь");
            var products = new[]
            {
                new Product { Name = "труба", MetalTypeId = iron.Id },
                new Product { Name = "арматура", MetalTypeId = steel.Id },
                new Product { Name = "лист", MetalTypeId = iron.Id },
                new Product { Name = "уголок", MetalTypeId = aluminum.Id },
            };
            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
        
    }
}