using MetalTrade.DataAccess.Data;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess;

public class AdvertisementInitializer
{
    public static async Task SeedAdvertisementAsync(MetalTradeDbContext context)
    {
        if (!await context.Advertisements.AnyAsync())
        {
            var firstSupplier = context.Users.FirstOrDefault(u => u.Email == "supplier1@gmail.com");
            var secondSupplier = context.Users.FirstOrDefault(u => u.Email == "supplier2@gmail.com");
            var reinforcement = context.Products.FirstOrDefault(p => p.Name.ToLower() == "арматура");
            var pipe = context.Products.FirstOrDefault(p => p.Name.ToLower() == "труба");
            var sheet = context.Products.FirstOrDefault(p => p.Name.ToLower() == "лист");

            if (firstSupplier != null && reinforcement != null && pipe != null)
            {
                var advertisements = new[]
                {
                    new Advertisement
                    {
                        Title = "Продается арматура",
                        Body = "Из стали",
                        Price = 100,
                        Address = "ул. Чуй 1",
                        PhoneNumber = "0555555555",
                        City = "Бишкек",
                        ProductId = reinforcement.Id,
                        UserId = firstSupplier.Id,
                        Photoes = new List<AdvertisementPhoto>()
                        {
                            new (){ PhotoLink = "https://airon-rnd.ru/upload/iblock/8fa/g6hdrfw2ozrl9tcgkwq3wg8s7mn97c4i.jpg" },
                            new () { PhotoLink = "https://kzmc.kg/media/uploads/images/stalnaya_armatura_A2_A300_kategoriya_CvHuoSF.jpg" }
                        }
                    },
                    new Advertisement
                    {
                        Title = "Продаются трубы",
                        Body = "Железные трубы",
                        Price = 200,
                        Address = "ул. Чуй 1",
                        PhoneNumber = "0555555555",
                        City = "Бишкек",
                        ProductId = pipe.Id,
                        UserId = firstSupplier.Id,
                        Photoes = new List<AdvertisementPhoto>()
                        {
                            new (){ PhotoLink = "https://www.kuluke.kg/img/solution-single/4.jpg" },
                            new () { PhotoLink = "https://mtt.kg/img/298x275/images/67187f47de74f.jpeg" }
                        }
                    }
                };
                context.Advertisements.AddRange(advertisements);
                await context.SaveChangesAsync();
            }
        
            if (secondSupplier != null && sheet != null)
            {
                var advertisements = new[]
                {
                    new Advertisement
                    {
                        Title = "Продаются железные листы",
                        Body = "Железные листы",
                        Price = 300,
                        Address = "ул. Чуй 2",
                        PhoneNumber = "0555555555",
                        City = "Бишкек",
                        ProductId = sheet.Id,
                        UserId = secondSupplier.Id,
                        Photoes = new List<AdvertisementPhoto>()
                        {
                            new (){ PhotoLink = "https://pt-k.ru/upload/iblock/6af/6af6c9c0f67ef3d365e498303df0816b.jpg" },
                            new () { PhotoLink = "https://ir.ozone.ru/s3/multimedia-5/6067847729.jpg" }
                        }
                    }
                };

                context.Advertisements.AddRange(advertisements);
                await context.SaveChangesAsync();
            }
        }
    }
}