using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using api.Presentation.Validation;

namespace Api.Tests;

public class ApiBehaviorExtensionsTests
{
    [Fact]
    public void InvalidModelStateResponseFactory_Returns_BadRequest_With_Expected_Payload()
    {
        // Arrange: configurar el contenedor con la extensión
        var services = new ServiceCollection();
        services.AddUnifiedValidationResponses(); // extensión a testear
        var sp = services.BuildServiceProvider();

        // Obtener las opciones de ApiBehaviorOptions configuradas por la extensión
        var opts = sp.GetRequiredService<IOptions<ApiBehaviorOptions>>().Value;
        Assert.NotNull(opts.InvalidModelStateResponseFactory);

        // Simular un ActionContext con errores de ModelState
        var modelState = new ModelStateDictionary();
        modelState.AddModelError("Name", "The name cannot exceed 100 characters.");
        modelState.AddModelError("Email", "The email is not in a valid format.");

        var httpCtx = new DefaultHttpContext();
        httpCtx.TraceIdentifier = "trace-123";

        var actionCtx = new ActionContext(httpCtx, new RouteData(), new ActionDescriptor(), modelState);

        // Act
        var result = opts.InvalidModelStateResponseFactory!(actionCtx);

        // Assert: es 400 con el payload esperado
        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, badRequest.StatusCode);

        // El payload es un objeto anónimo; lo validamos serializando a JSON
        var json = JsonSerializer.Serialize(badRequest.Value);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("error", root.GetProperty("status").GetString());
        Assert.Equal("Validation failed.", root.GetProperty("message").GetString());
        Assert.Equal("trace-123", root.GetProperty("traceId").GetString());

        var errors = root.GetProperty("errors");
        Assert.True(errors.TryGetProperty("Name", out var nameArr) && nameArr.GetArrayLength() == 1);
        Assert.Contains("The name cannot exceed 100 characters.",
            nameArr.EnumerateArray().Select(e => e.GetString()));

        Assert.True(errors.TryGetProperty("Email", out var emailArr) && emailArr.GetArrayLength() == 1);
        Assert.Contains("The email is not in a valid format.",
            emailArr.EnumerateArray().Select(e => e.GetString()));
    }
}
