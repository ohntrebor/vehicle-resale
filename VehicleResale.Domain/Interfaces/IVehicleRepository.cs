using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleResale.Domain.Entities;

namespace VehicleResale.Domain.Interfaces
{
    /// <summary>
    /// Interface do repositório de veículos
    /// </summary>
    public interface IVehicleRepository
    {
        Task<Vehicle?> GetByIdAsync(Guid id);
        Task<Vehicle?> GetByPaymentCodeAsync(string paymentCode);
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
        Task<IEnumerable<Vehicle>> GetSoldVehiclesAsync();
        Task<Vehicle> AddAsync(Vehicle vehicle);
        Task UpdateAsync(Vehicle vehicle);
        Task<bool> ExistsAsync(Guid id);
    }
}