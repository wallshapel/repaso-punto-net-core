// program.cs
using api.Data;
using api.Services.Employees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Necesario para que el servicio sepa qué implementación le corresponde y cumplir con la inversión de dependencias.
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Necesario para indicar a un repositorio que implementación le corresponde
builder.Services.AddScoped<api.Repositories.Employee.IEmployeeRepository, api.Repositories.Employee.EmployeeRepository>();

// Mapper
builder.Services.AddSingleton<api.Mapping.IObjectMapper, api.Mapping.ReflectionObjectMapper>();

builder.Services.AddControllers();

// Unifica los errores 400 de validación de los DTOs al mismo formato que el middleware
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
        {
        var errors = context.ModelState
            .Where(kvp => kvp.Value?.Errors.Count > 0)
        .ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
        );
        
        var payload = new
        {
            status = "error",
            message = "Validation failed.",
            traceId = context.HttpContext.TraceIdentifier,
            errors
        };
        
            return new BadRequestObjectResult(payload);
        };
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Manejo global de excepciones
app.UseMiddleware<api.Middlewares.ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();
