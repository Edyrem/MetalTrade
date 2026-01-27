using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MetalTrade.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MetalTrade.DataAccess.Data
{
    public class MetalTradeDbContext: IdentityDbContext<User, IdentityRole<int>, int>
    {
        public DbSet<User> Users { get; set; }
        public DbSet<MetalType> MetalTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Advertisement> Advertisements { get; set; }
        public DbSet<AdvertisementPhoto> AdvertisementPhotos { get; set; }
        public DbSet<Commercial> Commercials { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        

        public MetalTradeDbContext(DbContextOptions<MetalTradeDbContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<MetalType>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<Advertisement>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<AdvertisementPhoto>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Commercial>()
                .HasOne(c => c.Advertisement)
                .WithMany(a => a.Commercials)
                .HasForeignKey(c => c.AdvertisementId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ChatUser>()
                .HasKey(x => new { x.ChatId, x.UserId });
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Advertisement)
                .WithMany()
                .HasForeignKey(c => c.AdvertisementId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }
    }
}
