// program.cs
using api.Data;
using api.Services.Employees;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Necesario para que el servicio sepa qu� implementaci�n le corresponde y cumplir con la inversi�n de dependencias.
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Necesario para indicar a un repositorio que implementaci�n le corresponde
builder.Services.AddScoped<api.Repositories.Employee.IEmployeeRepository, api.Repositories.Employee.EmployeeRepository>();

builder.Services.AddControllers();

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

app.MapControllers();

app.Run();
