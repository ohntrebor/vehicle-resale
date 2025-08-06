using Microsoft.EntityFrameworkCore;
using VehicleResale.Domain.Entities;
using VehicleResale.Domain.Interfaces;
using VehicleResale.Infrastructure.Data;

namespace VehicleResale.Infrastructure.Repositories
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly ApplicationDbContext _context;

        public VehicleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Vehicle?> GetByIdAsync(Guid id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<Vehicle?> GetByPaymentCodeAsync(string paymentCode)
        {
            return await _context.Vehicles
                .FirstOrDefaultAsync(v => v.PaymentCode == paymentCode);
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _context.Vehicles
                .Where(v => !v.IsSold)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetSoldVehiclesAsync()
        {
            return await _context.Vehicles
                .Where(v => v.IsSold)
                .ToListAsync();
        }

        public async Task<Vehicle> AddAsync(Vehicle vehicle)
        {
            await _context.Vehicles.AddAsync(vehicle);
            return vehicle;
        }

        public async Task UpdateAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Vehicles.AnyAsync(v => v.Id == id);
        }
    }
}