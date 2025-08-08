using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VehicleResale.Domain.Enums;

namespace VehicleResale.Domain.Entities;

[Table("vehicles")]
/// <summary>
/// Entidade que representa um veículo no sistema
/// </summary>
public class Vehicle
{
    [Key]
    [Column("id")]
    public Guid Id { get; private set; }
    
    /// <summary>
    /// Marca do veículo
    /// </summary>
    [Column("brand")]
    public string Brand { get; private set; } 
    
    /// <summary>
    /// Modelo do veículo
    /// </summary>
    [Column("model")]
    public string Model { get; private set; }
    
    /// <summary>
    /// Ano do veículo
    /// </summary>
    [Column("year")]
    public int Year { get; private set; }
    
    /// <summary>
    /// Cor do veículo
    /// </summary>
    [Column("color")]
    public string Color { get; private set; }
    
    /// <summary>
    /// Preço do veículo
    /// </summary>
    [Column("price")]
    public decimal Price { get; private set; }
    
    /// <summary>
    /// Indica se o veículo foi vendido
    /// </summary>
    [Column("is_sold")]
    public bool IsSold { get; private set; }
    
    /// <summary>
    /// Data de criação do registro
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// Data da última atualização do registro
    /// </summary>
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; private set; }
    
    /// <summary>
    /// Identificador do comprador (CPF)
    /// </summary>
    [Column("buyer_cpf")]
    public string? BuyerCpf { get; private set; }
    
    /// <summary>
    /// Data da venda do veículo
    /// </summary>
    [Column("sale_date")]
    public DateTime? SaleDate { get; private set; }
    
    /// <summary>
    /// Código de pagamento
    /// </summary>
    [Column("payment_code")]
    public string? PaymentCode { get; private set; }
    
    /// <summary>
    /// Status do pagamento
    /// </summary>
    [Column("payment_status")]
    public PaymentStatus? PaymentStatus { get; private set; }

    protected Vehicle() { }

    public Vehicle(string brand, string model, int year, string color, decimal price)
    {
        Id = Guid.NewGuid();
        Brand = brand;
        Model = model;
        Year = year;
        Color = color;
        Price = price;
        IsSold = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(string brand, string model, int year, string color, decimal price)
    {
        Brand = brand;
        Model = model;
        Year = year;
        Color = color;
        Price = price;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RegisterSale(string buyerCpf, string paymentCode)
    {
        if (IsSold)
            throw new InvalidOperationException("Veículo já está vendido");

        BuyerCpf = buyerCpf;
        SaleDate = DateTime.UtcNow;
        PaymentCode = paymentCode;
        PaymentStatus = Enums.PaymentStatus.Pending;
        IsSold = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePaymentStatus(PaymentStatus status)
    {
        if (!IsSold)
            throw new InvalidOperationException("Veículo não está vendido");

        PaymentStatus = status;
        
        // Se o pagamento foi cancelado, reverter a venda
        if (status == Enums.PaymentStatus.Cancelled)
        {
            IsSold = false;
            BuyerCpf = null;
            SaleDate = null;
            PaymentCode = null;
        }
        
        UpdatedAt = DateTime.UtcNow;
    }
}
