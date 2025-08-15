// Middlewares/ExceptionHandlingMiddleware.cs
using api.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

namespace api.Middlewares
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await WriteErrorAsync(context, ex);
            }
        }

        private static async Task WriteErrorAsync(HttpContext ctx, Exception ex)
        {
            var (status, payload) = Map(ex, ctx.TraceIdentifier);

            ctx.Response.StatusCode = (int)status;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsJsonAsync(payload);
        }

        private static (HttpStatusCode status, object payload) Map(Exception ex, string traceId)
        {
            return ex switch
            {
                // 404
                NotFoundException nf => (HttpStatusCode.NotFound, Error("error", nf.Message, traceId)),

                // 400
                ArgumentException ae => (HttpStatusCode.BadRequest, Error("error", ae.Message, traceId)),

                // 409 por violación de unicidad (SQL Server: 2601, 2627) → cuerpo unificado con 'errors'
                DbUpdateException dbex when IsUniqueViolation(dbex)
                    => (HttpStatusCode.Conflict, Validation("Email already exists.", traceId,
                        new Dictionary<string, string[]>
                        {
                            // Único índice/constraint único actual: Email
                            ["Email"] = new[] { "Email already exists." }
                        })),

                // 400 por JSON malformado / request inválida
                JsonException jex => (HttpStatusCode.BadRequest, Validation("Malformed JSON.", traceId,
                    new Dictionary<string, string[]> { ["body"] = new[] { "Malformed JSON." } })),

                BadHttpRequestException brex => (HttpStatusCode.BadRequest, Validation("Invalid request.", traceId,
                    new Dictionary<string, string[]> { ["request"] = new[] { brex.Message } })),

                // fallback 500
                _ => (HttpStatusCode.InternalServerError, Error("error", "Unexpected server error.", traceId))
            };
        }

        private static bool IsUniqueViolation(DbUpdateException dbex)
        {
            // SQL Server arroja SqlException con Number 2627 (unique constraint) o 2601 (duplicate key)
            if (dbex.InnerException is SqlException sql)
                return sql.Number == 2627 || sql.Number == 2601;

            return false;
        }

        private static object Validation(string message, string traceId, IDictionary<string, string[]> errors)
            => new { status = "error", message, traceId, errors };

        private static object Error(string status, string message, string traceId)
            => new { status, message, traceId };
    }
}
