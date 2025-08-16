using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using api.Middlewares;
using api.Exceptions;

namespace Api.Tests;

public class ExceptionHandlingMiddlewareTests
{
    private static async Task<(int statusCode, string json, string contentType)> InvokeAndReadAsync(System.Exception ex)
    {
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "trace-xyz";
        context.Response.Body = new MemoryStream();

        RequestDelegate next = _ => throw ex;
        using var loggerFactory = LoggerFactory.Create(b => { /* no-op */ });
        var logger = loggerFactory.CreateLogger<ExceptionHandlingMiddleware>();

        var mw = new ExceptionHandlingMiddleware(next, logger);
        await mw.Invoke(context);

        context.Response.Body.Position = 0;
        var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
        return (context.Response.StatusCode, body, context.Response.ContentType ?? "");
    }

    [Fact]
    public async Task NotFoundException_Returns_404_With_Error_Payload()
    {
        // Arrange
        var ex = new NotFoundException("Employee", 123);

        // Act
        var (status, json, contentType) = await InvokeAndReadAsync(ex);

        // Assert
        Assert.Equal(StatusCodes.Status404NotFound, status);
        Assert.StartsWith("application/json", contentType, StringComparison.OrdinalIgnoreCase);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("error", root.GetProperty("status").GetString());
        Assert.Equal("Employee with key '123' was not found.", root.GetProperty("message").GetString());
        Assert.Equal("trace-xyz", root.GetProperty("traceId").GetString());
    }

    [Fact]
    public async Task ArgumentException_Returns_400_With_Error_Payload()
    {
        // Arrange
        var ex = new System.ArgumentException("Route id and body id do not match.");

        // Act
        var (status, json, contentType) = await InvokeAndReadAsync(ex);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.StartsWith("application/json", contentType, StringComparison.OrdinalIgnoreCase);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("error", root.GetProperty("status").GetString());
        Assert.Equal("Route id and body id do not match.", root.GetProperty("message").GetString());
        Assert.Equal("trace-xyz", root.GetProperty("traceId").GetString());
    }

    [Fact]
    public async Task JsonException_Returns_400_With_Validation_Payload()
    {
        // Arrange
        var ex = new System.Text.Json.JsonException("bad json");

        // Act
        var (status, json, contentType) = await InvokeAndReadAsync(ex);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.StartsWith("application/json", contentType, StringComparison.OrdinalIgnoreCase);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("error", root.GetProperty("status").GetString());
        Assert.Equal("Malformed JSON.", root.GetProperty("message").GetString());
        Assert.Equal("trace-xyz", root.GetProperty("traceId").GetString());

        var errors = root.GetProperty("errors");
        Assert.True(errors.TryGetProperty("body", out var arr));
        Assert.Equal(1, arr.GetArrayLength());
        Assert.Equal("Malformed JSON.", arr[0].GetString());
    }

    [Fact]
    public async Task BadHttpRequestException_Returns_400_With_Validation_Payload()
    {
        // Arrange
        var ex = new BadHttpRequestException("bad request");

        // Act
        var (status, json, contentType) = await InvokeAndReadAsync(ex);

        // Assert
        Assert.Equal(StatusCodes.Status400BadRequest, status);
        Assert.StartsWith("application/json", contentType, StringComparison.OrdinalIgnoreCase);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("error", root.GetProperty("status").GetString());
        Assert.Equal("Invalid request.", root.GetProperty("message").GetString());
        Assert.Equal("trace-xyz", root.GetProperty("traceId").GetString());

        var errors = root.GetProperty("errors");
        Assert.True(errors.TryGetProperty("request", out var arr));
        Assert.Equal(1, arr.GetArrayLength());
        Assert.Equal("bad request", arr[0].GetString());
    }

    [Fact]
    public async Task UnknownException_Returns_500_With_Generic_Error_Payload()
    {
        // Arrange
        var ex = new System.Exception("boom");

        // Act
        var (status, json, contentType) = await InvokeAndReadAsync(ex);

        // Assert
        Assert.Equal(StatusCodes.Status500InternalServerError, status);
        Assert.StartsWith("application/json", contentType, StringComparison.OrdinalIgnoreCase);

        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        Assert.Equal("error", root.GetProperty("status").GetString());
        Assert.Equal("Unexpected server error.", root.GetProperty("message").GetString());
        Assert.Equal("trace-xyz", root.GetProperty("traceId").GetString());
    }
}
