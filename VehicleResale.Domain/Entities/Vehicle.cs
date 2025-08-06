using System;
using VehicleResale.Domain.Enums;

namespace VehicleResale.Domain.Entities;

/// <summary>
/// Entidade que representa um veículo no sistema
/// </summary>
public class Vehicle
{
    public Guid Id { get; private set; }
    public string Brand { get; private set; } // Marca
    public string Model { get; private set; } // Modelo
    public int Year { get; private set; } // Ano
    public string Color { get; private set; } // Cor
    public decimal Price { get; private set; } // Preço
    public bool IsSold { get; private set; } // Indica se foi vendido
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    
    // Informações de venda
    public string? BuyerCpf { get; private set; }
    public DateTime? SaleDate { get; private set; }
    public string? PaymentCode { get; private set; }
    public PaymentStatus? PaymentStatus { get; private set; }

    protected Vehicle() { } // Para EF Core

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
            throw new InvalidOperationException("Vehicle is already sold");

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
            throw new InvalidOperationException("Vehicle is not sold");

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
