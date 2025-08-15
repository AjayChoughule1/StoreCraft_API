using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using System.Data;

namespace StoreCraft_API.ErrorHandling
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                // Save exception to DB
                await SaveLogToDatabaseAsync(ex);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task SaveLogToDatabaseAsync(Exception ex)
        {
            try
            {
                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                using var command = new SqlCommand(@"
                    INSERT INTO ProductLogs ([Date],[Thread],[Level],[Logger],[Message],[Exception])
                    VALUES (@Date,@Thread,@Level,@Logger,@Message,@Exception)", connection);

                command.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.UtcNow;
                command.Parameters.Add("@Thread", SqlDbType.NVarChar).Value = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
                command.Parameters.Add("@Level", SqlDbType.NVarChar).Value = "ERROR";
                command.Parameters.Add("@Logger", SqlDbType.NVarChar).Value = nameof(ExceptionHandlingMiddleware);
                command.Parameters.Add("@Message", SqlDbType.NVarChar).Value = ex.Message;
                command.Parameters.Add("@Exception", SqlDbType.NVarChar).Value = ex.ToString();

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception dbEx)
            {
                _logger.LogError(dbEx, "Failed to log error to ProductLogs table");
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                ArgumentException => HttpStatusCode.BadRequest,
                KeyNotFoundException => HttpStatusCode.NotFound,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                context.Response.StatusCode,
                ex.Message,
                Details = ex.InnerException?.Message
            };

            var json = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(json);
        }
    }
}
