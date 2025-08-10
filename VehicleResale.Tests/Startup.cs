// VehicleResale.Tests/Startup.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using VehicleResale.Infrastructure.Data;
using VehicleResale.Domain.Interfaces;
using VehicleResale.Infrastructure.Repositories;
using VehicleResale.Application.Mappings;
using System.Reflection;

namespace VehicleResale.Tests;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.Test.json", optional: true, reloadOnChange: true);
        
        Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // DbContext com InMemory (mesma configuração do Program.cs mas InMemory)
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        });

        // Repositories e Unit of Work (igual ao Program.cs)
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();

        // AutoMapper (igual ao Program.cs)
        services.AddAutoMapper(typeof(MappingProfile).Assembly);

        // MediatR (igual ao Program.cs)
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.Load("VehicleResale.Application"));
        });

        // Logging
        services.AddLogging();

        // Configuration
        services.AddSingleton<IConfiguration>(Configuration);
    }
}
