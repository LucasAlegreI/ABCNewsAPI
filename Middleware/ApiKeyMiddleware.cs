using System.Text;
using ABCNewsAPI.Data;
using ABCNewsAPI.Services;
using Microsoft.EntityFrameworkCore;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public ApiKeyMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.Value?.ToLower();

        if (path == "/api/apikey/add")
        {
            await _next(context);
            return;
        }

        var apiKeyHeader = context.Request.Headers["x-api-key"].ToString();

        if (string.IsNullOrEmpty(apiKeyHeader))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("API Key is missing.");
            return;
        }

        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var hmacService = scope.ServiceProvider.GetRequiredService<HmacService>();
            var apiKeyHmac = hmacService.GenerarFirma(apiKeyHeader);
            var apiKey = await dbContext.ApiKeys
                .FirstOrDefaultAsync(k => k.FirmaHmac == apiKeyHmac && k.Activo);

            if (apiKey == null)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Invalid API Key.");
                return;
            }

            var requestBody = await GetRequestBodyAsync(context.Request);
        }

        await _next(context);
    }

    // Obtener el cuerpo de la solicitud
    private async Task<string> GetRequestBodyAsync(HttpRequest request)
    {
        request.EnableBuffering();
        using (var reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
        {
            var body = await reader.ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}

