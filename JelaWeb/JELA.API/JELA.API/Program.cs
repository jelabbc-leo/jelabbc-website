using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using JELA.API.Configuration;
using JELA.API.Endpoints;
using JELA.API.Middleware;
using JELA.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

// Configurar Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/jela-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("Iniciando JELA.API...");

    var builder = WebApplication.CreateBuilder(args);

    // Usar Serilog
    builder.Host.UseSerilog();

    // ========================================
    // CONFIGURACIÓN DE SERVICIOS
    // ========================================

    // Configuración desde appsettings
    builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
    builder.Services.Configure<AllowedTablesSettings>(builder.Configuration.GetSection("AllowedTables"));

    // Registrar servicios
    builder.Services.AddScoped<IDatabaseService, MySqlDatabaseService>();
    builder.Services.AddScoped<IAuthService, JwtAuthService>();
    builder.Services.AddScoped<IDocumentIntelligenceService, AzureDocumentIntelligenceService>();
    builder.Services.AddScoped<IOpenAIService, AzureOpenAIService>();
    
    // Servicios del módulo de tickets
    builder.Services.AddScoped<ITicketValidationService, TicketValidationService>();
    builder.Services.AddScoped<ITicketNotificationService, TicketNotificationService>();
    builder.Services.AddScoped<ITicketMetricsService, TicketMetricsService>();
    builder.Services.AddScoped<IPromptTuningService, PromptTuningService>();
    builder.Services.AddScoped<IYCloudService, YCloudService>();
    builder.Services.AddScoped<IVapiService, VapiService>();
    
    // HttpClient para servicios externos
    builder.Services.AddHttpClient<AzureDocumentIntelligenceService>();
    builder.Services.AddHttpClient<AzureOpenAIService>();
    builder.Services.AddHttpClient<IYCloudService, YCloudService>();
    builder.Services.AddHttpClient<IVapiService, VapiService>();

    // Configurar JSON para usar PascalCase (compatibilidad con JelaWeb)
    builder.Services.ConfigureHttpJsonOptions(options =>
    {
        options.SerializerOptions.PropertyNamingPolicy = null; // PascalCase
        options.SerializerOptions.WriteIndented = false;
    });

    // ========================================
    // CONFIGURACIÓN DE AUTENTICACIÓN JWT
    // ========================================
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
        ?? new JwtSettings { SecretKey = "DefaultSecretKeyForDevelopment_MustBeAtLeast32Characters!" };

    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });

    builder.Services.AddAuthorization();

    // ========================================
    // CONFIGURACIÓN DE CORS
    // ========================================
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowJelaWeb", policy =>
        {
            policy.WithOrigins(
                    "http://localhost",
                    "http://localhost:44300",
                    "https://localhost:44300",
                    "http://localhost:21754",
                    "http://jelaweb.azurewebsites.net",
                    "https://jelaweb.azurewebsites.net",
                    "http://jela-front.azurewebsites.net",
                    "https://jela-front.azurewebsites.net"
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });

        // Política más permisiva para desarrollo
        options.AddPolicy("Development", policy =>
        {
            policy.SetIsOriginAllowed(origin => 
                origin.StartsWith("http://localhost") || 
                origin.StartsWith("https://localhost"))
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    // ========================================
    // CONFIGURACIÓN DE RATE LIMITING
    // ========================================
    builder.Services.AddRateLimiter(options =>
    {
        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1)
                }));

        options.OnRejected = async (context, token) =>
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.HttpContext.Response.WriteAsJsonAsync(new
            {
                mensaje = "Demasiadas solicitudes. Intente de nuevo más tarde.",
                retryAfter = "60 segundos"
            }, cancellationToken: token);
        };
    });

    // ========================================
    // CONFIGURACIÓN DE SWAGGER
    // ========================================
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "JELA API",
            Version = "v1",
            Description = "API dinámica para el sistema JELA - Gestión de Condominios",
            Contact = new OpenApiContact
            {
                Name = "JELA BBC",
                Email = "soporte@jelabbc.com"
            }
        });

        // Configurar autenticación en Swagger
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Ingrese el token JWT en el formato: Bearer {token}"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    // ========================================
    // HEALTH CHECKS
    // ========================================
    builder.Services.AddHealthChecks()
        .AddMySql(
            builder.Configuration.GetConnectionString("MySQL") ?? "",
            name: "mysql",
            tags: new[] { "db", "mysql" });

    var app = builder.Build();

    // ========================================
    // CONFIGURACIÓN DEL PIPELINE
    // ========================================

    // Middleware de manejo de excepciones (primero)
    app.UseExceptionHandling();

    // Middleware de logging de requests
    app.UseRequestLogging();

    // Swagger (siempre habilitado para facilitar pruebas)
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "JELA API v1");
        options.RoutePrefix = "swagger"; // Swagger en /swagger (estándar)
    });

    // CORS
    if (app.Environment.IsDevelopment())
    {
        app.UseCors("Development");
    }
    else
    {
        app.UseCors("AllowJelaWeb");
    }

    // Rate Limiting
    app.UseRateLimiter();

    // Autenticación y Autorización
    app.UseAuthentication();
    app.UseAuthorization();

    // ========================================
    // MAPEAR ENDPOINTS
    // ========================================
    app.MapHealthEndpoints();
    app.MapAuthEndpoints();
    app.MapCrudEndpoints();
    app.MapDocumentIntelligenceEndpoints();
    app.MapOpenAIEndpoints();
    
    // Endpoints del módulo de tickets
    app.MapWebhookEndpoints();

    // Health checks de ASP.NET Core
    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = _ => false // Solo verifica que la app responda
    });

    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("db") // Verifica la BD
    });

    Log.Information("JELA.API iniciado correctamente en {Urls}", string.Join(", ", app.Urls));

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar");
}
finally
{
    Log.CloseAndFlush();
}
