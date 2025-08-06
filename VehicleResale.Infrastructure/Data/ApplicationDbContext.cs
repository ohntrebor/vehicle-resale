using Microsoft.EntityFrameworkCore;
using VehicleResale.Domain.Entities;

namespace VehicleResale.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.Brand)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Model)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2);

                entity.Property(e => e.BuyerCpf)
                    .HasMaxLength(14);

                entity.Property(e => e.PaymentCode)
                    .HasMaxLength(50);

                entity.HasIndex(e => e.PaymentCode)
                    .IsUnique()
                    .HasFilter("[PaymentCode] IS NOT NULL");

                entity.HasIndex(e => e.IsSold);
            });
        }
    }
}