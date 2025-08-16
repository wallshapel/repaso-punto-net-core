// Infrastructure/ServiceCollectionExtensions.cs
namespace api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Indicamos la implementación del servicio de Employee
        services.AddScoped<api.Services.Employees.IEmployeeService, api.Services.Employees.EmployeeService>();
        // Indicamos la implementación del repositorio de Employee
        services.AddScoped<api.Repositories.Employee.IEmployeeRepository, api.Repositories.Employee.EmployeeRepository>();
        // Mapeador
        services.AddSingleton<api.Mapping.IObjectMapper, api.Mapping.ReflectionObjectMapper>();
        return services;
    }
}
