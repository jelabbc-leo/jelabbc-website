using JELA.API.Models;
using JELA.API.Services;

namespace JELA.API.Endpoints;

/// <summary>
/// Endpoints de health check
/// </summary>
public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api")
            .WithTags("Health")
            .WithOpenApi();

        // GET: Health Check
        group.MapGet("/health", GetHealth)
            .WithName("HealthCheck")
            .WithDescription("Verifica el estado del API y la conexión a la base de datos")
            .AllowAnonymous()
            .Produces<HealthResponse>(StatusCodes.Status200OK)
            .Produces<HealthResponse>(StatusCodes.Status503ServiceUnavailable);

        // GET: Version
        group.MapGet("/version", GetVersion)
            .WithName("Version")
            .WithDescription("Retorna la versión del API")
            .AllowAnonymous()
            .Produces<VersionResponse>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> GetHealth(IDatabaseService database)
    {
        var dbHealthy = await database.VerificarConexionAsync();

        var response = new HealthResponse
        {
            Status = dbHealthy ? "Healthy" : "Unhealthy",
            Timestamp = DateTime.UtcNow,
            Checks = new Dictionary<string, CheckResult>
            {
                ["database"] = new CheckResult
                {
                    Status = dbHealthy ? "Healthy" : "Unhealthy",
                    Description = dbHealthy ? "MySQL connection OK" : "MySQL connection failed"
                },
                ["api"] = new CheckResult
                {
                    Status = "Healthy",
                    Description = "API is running"
                }
            }
        };

        return dbHealthy
            ? Results.Ok(response)
            : Results.Json(response, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    private static IResult GetVersion()
    {
        var assembly = System.Reflection.Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version?.ToString() ?? "1.0.0";

        return Results.Ok(new VersionResponse
        {
            Version = version,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            Framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
            Timestamp = DateTime.UtcNow
        });
    }
}

public class HealthResponse
{
    public string Status { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public Dictionary<string, CheckResult> Checks { get; set; } = new();
}

public class CheckResult
{
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class VersionResponse
{
    public string Version { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string Framework { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
