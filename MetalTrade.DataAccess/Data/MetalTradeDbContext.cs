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
        public DbSet<TopAdvertisement> TopAdvertisements { get; set; }
        public DbSet<TopUser> TopUsers { get; set; }


        public MetalTradeDbContext(DbContextOptions<MetalTradeDbContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<MetalType>().HasQueryFilter(m => !m.IsDeleted);
            modelBuilder.Entity<Advertisement>().HasQueryFilter(a => !a.IsDeleted);
            modelBuilder.Entity<AdvertisementPhoto>().HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<Commercial>(entity =>
            {
                entity.HasQueryFilter(c => !c.IsDeleted);

                entity.HasOne(c => c.Advertisement)
                    .WithMany(a => a.Commercials)
                    .HasForeignKey(c => c.AdvertisementId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.Cost)
                    .HasColumnType("decimal(18,2)");

                entity.HasOne(c => c.CreatedBy)
                    .WithMany()
                    .HasForeignKey(c => c.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Navigation(c => c.Advertisement).AutoInclude(false);
                entity.Navigation(c => c.CreatedBy).AutoInclude(false);
            });

            modelBuilder.Entity<TopAdvertisement>(entity =>
            {
                entity.HasQueryFilter(c => !c.IsDeleted);

                entity.HasOne(c => c.Advertisement)
                    .WithMany(a => a.TopAdvertisements)
                    .HasForeignKey(c => c.AdvertisementId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.CreatedBy)
                    .WithMany()
                    .HasForeignKey(c => c.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Navigation(c => c.Advertisement).AutoInclude(false);
                entity.Navigation(c => c.CreatedBy).AutoInclude(false);
            });

            modelBuilder.Entity<TopUser>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.TargetUser)
                    .WithMany(u => u.TopUsers)
                    .HasForeignKey(e => e.TargetUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Navigation(e => e.TargetUser).AutoInclude(false);
                entity.Navigation(e => e.CreatedBy).AutoInclude(false);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
