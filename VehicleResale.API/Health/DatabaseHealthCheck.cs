using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VehicleResale.Infrastructure.Data;

namespace VehicleResale.API.Health
{
    /// <summary>
    /// Health check para verificar a conectividade com o banco de dados
    /// </summary>
    public class DatabaseHealthCheck : IHealthCheck
    {
        private readonly ApplicationDbContext _context;

        public DatabaseHealthCheck(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Tenta executar uma query simples no banco
                await _context.Database.CanConnectAsync(cancellationToken);
                
                return HealthCheckResult.Healthy("Database is accessible");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Database is not accessible",
                    exception: ex);
            }
        }
    }
}