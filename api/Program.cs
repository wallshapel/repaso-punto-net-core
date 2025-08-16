// program.cs
using api.Data;
using api.Infrastructure;
using api.Middlewares;
using api.Presentation.Validation;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Infraestructura / Application
builder.Services.AddApplicationServices();

// Controllers + JSON + Validación unificada
builder.Services
    .AddControllers()
    .AddJsonOptions(o =>
    { 
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; 
        o.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;    
    });

// Unifica los errores 400 de validación de los DTOs al mismo formato que el middleware
builder.Services.AddUnifiedValidationResponses();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core (pool + retries)
builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(maxRetryCount: 3)
    ));

// Perf “rápidas”
builder.Services.AddResponseCompression();
builder.Services.AddResponseCaching();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exceptions
app.UseGlobalExceptionHandling();

app.UseResponseCompression();
app.UseResponseCaching();

app.UseAuthorization();

app.MapControllers();
app.Run();
