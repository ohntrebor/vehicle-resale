using Microsoft.EntityFrameworkCore;
using VehicleResale.Domain.Entities;
using VehicleResale.Domain.Enums;

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
                // Nome da tabela em snake_case
                entity.ToTable("vehicles");
        
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

                // Configuração do PaymentStatus com valor padrão 0 (Pending)
                entity.Property(e => e.PaymentStatus)
                    .HasConversion<int>() // Converte enum para int
                    .HasDefaultValue(PaymentStatus.Pending) // Valor padrão 0
                    .IsRequired();

                // Índice- payment_code diferente de null para performance
                entity.HasIndex(e => e.PaymentCode)
                    .HasDatabaseName("ix_vehicles_payment_code")
                    .IsUnique()
                    .HasFilter("payment_code IS NOT NULL");

                entity.HasIndex(e => e.IsSold)
                    .HasDatabaseName("ix_vehicles_is_sold");

                // Índice - Para buscar vendas por CPF do comprador
                entity.HasIndex(e => e.BuyerCpf)
                    .HasDatabaseName("ix_vehicles_buyer_cpf")
                    .HasFilter("buyer_cpf IS NOT NULL");

                // Índice composto - para queries de vendas (is_sold + sale_date)
                entity.HasIndex(e => new { e.IsSold, e.SaleDate })
                    .HasDatabaseName("ix_vehicles_sold_date");
            });
        }
    }
}