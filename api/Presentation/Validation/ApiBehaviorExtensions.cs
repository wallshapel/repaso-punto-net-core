// Presentation/Validation/ApiBehaviorExtensions.cs
using Microsoft.AspNetCore.Mvc;

namespace api.Presentation.Validation;
public static class ApiBehaviorExtensions
{
    // Unifica los errores 400 de validación de los DTOs al mismo formato que el middleware 
    public static IServiceCollection AddUnifiedValidationResponses(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(options =>
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

        return services;
    }
}
