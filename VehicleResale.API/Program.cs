using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using VehicleResale.Application.Mappings;
using VehicleResale.Domain.Interfaces;
using VehicleResale.Infrastructure.Data;
using VehicleResale.Infrastructure.Repositories;
using VehicleResale.Application.Validators;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation.AspNetCore;
using VehicleResale.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// ===========================================
// CONFIGURAÇÃO DOS SERVIÇOS
// ===========================================

// 1. Controllers com validação
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddFluentValidation(fv =>
    {
        fv.RegisterValidatorsFromAssemblyContaining<CreateVehicleValidator>();
        fv.ImplicitlyValidateChildProperties = true;
    });

// 2. Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("VehicleResale.Infrastructure");
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorCodesToAdd: null);
    });
    
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// 3. Repositories e Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

// 4. AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// 5. MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.Load("VehicleResale.Application"));
});

// 6. Health Checks Simples
builder.Services.AddHealthChecks();

// 7. Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vehicle Resale API",
        Version = "v1",
        Description = "API para gerenciamento de revenda de veículos automotores"
    });

    // Tentar incluir XML comments se existir
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// 8. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// ===========================================
// BUILD DA APLICAÇÃO
// ===========================================
var app = builder.Build();

// ===========================================
// PIPELINE DE MIDDLEWARE
// ===========================================

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vehicle API");
    c.RoutePrefix = "swagger";
});

// CORS
app.UseCors("AllowAll");

// Routing
app.UseRouting();

// Controllers
app.MapControllers();

// Health Check simples
app.MapHealthChecks("/health");

// ===========================================
// APLICAR MIGRATIONS E SEED
// ===========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Verificando banco de dados...");
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Tentar conectar com retry para Docker
        var retries = 0;
        var maxRetries = 10;
        
        while (retries < maxRetries)
        {
            try
            {
                // Criar banco se não existir e aplicar migrations
                context.Database.Migrate();
                
                logger.LogInformation("Banco de dados pronto!");
                
                // 🎲 SEED Data
                logger.LogInformation("Executando seed de dados...");
                await VehicleSeedData.SeedAsync(context);
                
                break;
            }
            catch (Exception)
            {
                retries++;
                if (retries == maxRetries) throw;
                
                logger.LogWarning($"Banco não disponível. Tentativa {retries}/{maxRetries}. Aguardando 5 segundos...");
                await Task.Delay(5000);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao conectar com o banco de dados!");
        // Continuar mesmo com erro para não travar o container
    }
}

app.Run();