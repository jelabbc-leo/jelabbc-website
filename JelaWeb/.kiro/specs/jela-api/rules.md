# 📏 REGLAS DE PROGRAMACIÓN - JELA.API

**Fecha:** Enero 21, 2026  
**Versión:** 1.0  
**Alcance:** Todo el código en JELA.API  

---

## 🎯 PRINCIPIOS GENERALES

### Arquitectura
1. **Minimal APIs First**: Usar Minimal APIs de .NET 8 para todos los endpoints
2. **Inyección de Dependencias**: Registrar todos los servicios en `Program.cs`
3. **Separación de Responsabilidades**: Un servicio por funcionalidad específica
4. **Principio de Responsabilidad Única**: Cada clase/method tiene un propósito único

### Código
1. **Lenguaje**: C# 11+ exclusivamente
2. **Estilo**: Async/await para todas las operaciones I/O
3. **Nombres**: PascalCase para clases/métodos, camelCase para variables
4. **Comentarios**: XML documentation en métodos públicos

---

## 🏗️ ESTRUCTURA DE ARCHIVOS

### Organización por Capas
```
JELA.API/
├── Configuration/     # ⚙️ Configuración (JwtSettings, etc.)
├── Endpoints/         # 🌐 Definición de rutas y handlers
├── Middleware/        # 🔧 Middleware personalizado
├── Models/            # 📦 DTOs y modelos de datos
├── Services/          # 🔄 Lógica de negocio
└── Program.cs         # 🚀 Punto de entrada
```

### Convenciones de Nombres
- **Endpoints**: `{Feature}Endpoints.cs` (ej: `AuthEndpoints.cs`)
- **Services**: `{Feature}Service.cs` (ej: `JwtAuthService.cs`)
- **Models**: `{Feature}Models.cs` (ej: `AuthModels.cs`)
- **Configuration**: `{Feature}Settings.cs` (ej: `JwtSettings.cs`)

---

## 🔧 PROGRAMACIÓN ASÍNCRONA

### Regla Crítica
**TODAS** las operaciones I/O deben ser `async/await`:

```csharp
// ✅ CORRECTO
public async Task<IActionResult> GetDataAsync()
{
    var data = await _service.GetDataAsync();
    return Ok(data);
}

// ❌ INCORRECTO
public IActionResult GetData()
{
    var data = _service.GetData(); // Bloqueante
    return Ok(data);
}
```

### Excepciones
- Métodos de configuración en `Program.cs`
- Métodos de validación síncronos
- Constructores

---

## 🚀 ENDPOINTS - MINIMAL APIs

### Estructura Estándar
```csharp
// En {Feature}Endpoints.cs
public static class {Feature}Endpoints
{
    public static void Map{Feature}Endpoints(this IEndpointRouteBuilder app)
    {
        // Endpoints aquí
    }
}

// En Program.cs
app.MapAuthEndpoints();
app.MapCrudEndpoints();
// etc.
```

### Patrón de Endpoint
```csharp
app.MapGet("/api/{feature}", async (
    [FromServices] I{Feature}Service service,
    [FromQuery] string param) =>
{
    try
    {
        var result = await service.GetDataAsync(param);
        return Results.Ok(result);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error en endpoint");
        return Results.Problem("Error interno del servidor");
    }
})
.RequireAuthorization()
.WithName("Get{Feature}")
.WithOpenApi();
```

### Validación
- Usar `[FromBody]`, `[FromQuery]`, `[FromRoute]` explícitamente
- Validar entrada con `ValidationProblem`
- Retornar códigos HTTP apropiados

---

## 🔄 SERVICIOS

### Patrón de Servicio
```csharp
public interface I{Feature}Service
{
    Task<Result<T>> GetDataAsync(string id);
    Task<Result<bool>> CreateAsync(T data);
}

public class {Feature}Service : I{Feature}Service
{
    private readonly ILogger<{Feature}Service> _logger;
    private readonly IConfiguration _config;

    public {Feature}Service(
        ILogger<{Feature}Service> logger,
        IConfiguration config)
    {
        _logger = logger;
        _config = config;
    }

    public async Task<Result<T>> GetDataAsync(string id)
    {
        try
        {
            // Lógica aquí
            return Result.Success(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data for {Id}", id);
            return Result.Failure<T>("Error retrieving data");
        }
    }
}
```

### Registro en Program.cs
```csharp
builder.Services.AddScoped<I{Feature}Service, {Feature}Service>();
```

---

## 📦 MODELOS Y DTOs

### Convenciones
```csharp
// Request DTOs
public record Create{Entity}Request(
    string Name,
    string Description);

// Response DTOs
public record {Entity}Response(
    int Id,
    string Name,
    DateTime CreatedAt);

// Internal models
public class {Entity}
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Validación
```csharp
public class CreateEntityRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name too long")]
    public string Name { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email")]
    public string? Email { get; set; }
}
```

---

## 🔐 AUTENTICACIÓN Y AUTORIZACIÓN

### JWT en Endpoints
```csharp
app.MapGet("/api/secure", async (ClaimsPrincipal user) =>
{
    var userId = user.FindFirst("userId")?.Value;
    // Lógica aquí
})
.RequireAuthorization();
```

### Roles y Políticas
```csharp
// En Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Administrator"));
});

// En endpoint
.RequireAuthorization("AdminOnly");
```

---

## 🗄️ ACCESO A BASE DE DATOS

### MySqlDatabaseService
```csharp
public async Task<IEnumerable<T>> QueryAsync<T>(
    string sql,
    object? parameters = null)
{
    using var connection = new MySqlConnection(_connectionString);
    return await connection.QueryAsync<T>(sql, parameters);
}
```

### CRUD Dinámico
- **NO usar queries hardcodeadas**
- **Usar parámetros preparados**
- **Validar nombres de tablas**
- **Loggear todas las operaciones**

### Transacciones
```csharp
using var transaction = await connection.BeginTransactionAsync();
try
{
    // Operaciones aquí
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

---

## 🚨 MANEJO DE ERRORES

### Patrón Estándar
```csharp
public async Task<IResult> HandleRequestAsync()
{
    try
    {
        var result = await _service.ProcessAsync();
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Error);
    }
    catch (ValidationException ex)
    {
        return Results.ValidationProblem(ex.Errors);
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Unauthorized();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error");
        return Results.Problem(
            "Internal server error",
            statusCode: 500);
    }
}
```

### Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public string? Error { get; }

    public static Result<T> Success(T value) =>
        new Result<T> { IsSuccess = true, Value = value };

    public static Result<T> Failure(string error) =>
        new Result<T> { IsSuccess = false, Error = error };
}
```

---

## 📊 LOGGING

### Niveles
- **Trace**: Información detallada para debugging
- **Debug**: Información útil durante desarrollo
- **Information**: Eventos importantes de negocio
- **Warning**: Situaciones problemáticas pero no críticas
- **Error**: Errores que requieren atención
- **Critical**: Errores que detienen la aplicación

### Uso
```csharp
_logger.LogInformation("User {UserId} logged in", userId);
_logger.LogError(ex, "Failed to process request for user {UserId}", userId);
_logger.LogWarning("Suspicious activity detected for IP {IpAddress}", ip);
```

### Structured Logging
```csharp
_logger.LogInformation(
    "Order {OrderId} processed successfully",
    new { OrderId = order.Id, Amount = order.Amount });
```

---

## 🧪 TESTING

### Unit Tests
```csharp
[Fact]
public async Task GetDataAsync_ValidId_ReturnsData()
{
    // Arrange
    var service = new FeatureService(_mockLogger, _mockConfig);

    // Act
    var result = await service.GetDataAsync("valid-id");

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
}
```

### Integration Tests
```csharp
[Fact]
public async Task GetEndpoint_ReturnsOk()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.GetAsync("/api/feature");

    // Assert
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    // Assertions here
}
```

---

## 🔒 SEGURIDAD

### Principios
1. **Defense in Depth**: Múltiples capas de seguridad
2. **Fail Safe**: Fallar de forma segura
3. **Least Privilege**: Mínimos permisos necesarios
4. **Zero Trust**: Verificar todo

### Prácticas
- **Input Validation**: Validar toda entrada
- **Output Encoding**: Codificar salida
- **SQL Injection Prevention**: Usar parámetros
- **XSS Prevention**: Sanitizar HTML
- **CSRF Protection**: Tokens anti-falsificación

### Headers de Seguridad
```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});
```

---

## ⚡ PERFORMANCE

### Optimizaciones
1. **Async/Await**: Para todas las operaciones I/O
2. **Connection Pooling**: Configurado en MySQL
3. **Caching**: Para datos frecuentemente accedidos
4. **Pagination**: Para grandes datasets
5. **Lazy Loading**: Solo cargar cuando necesario

### Monitoreo
- **Response Times**: < 500ms promedio
- **Memory Usage**: Monitorear leaks
- **Database Queries**: Optimizar lentas
- **Error Rates**: Alertas automáticas

---

## 📚 DOCUMENTACIÓN

### Código
```csharp
/// <summary>
/// Processes the specified request and returns a result.
/// </summary>
/// <param name="request">The request to process.</param>
/// <returns>A task representing the asynchronous operation.</returns>
public async Task<Result<T>> ProcessAsync(Request request)
```

### API
- **Swagger/OpenAPI**: Automáticamente generado
- **README.md**: Documentación general
- **CHANGELOG.md**: Historial de cambios

---

## 🔄 VERSIONADO Y COMPATIBILIDAD

### API Versioning
```csharp
app.MapGet("/api/v1/feature", ...)
app.MapGet("/api/v2/feature", ...)
```

### Backward Compatibility
- No romper cambios en versiones menores
- Deprecation warnings para APIs obsoletas
- Migration guides para versiones mayores

---

## 🚀 DEPLOYMENT

### Checklist Pre-Deployment
- [ ] Tests pasan
- [ ] Code analysis aprobado
- [ ] Secrets en variables de entorno
- [ ] Configuración validada
- [ ] Database migrations aplicadas
- [ ] Health checks funcionando

### Variables de Entorno
```bash
# Database
MYSQL_CONNECTION_STRING="..."

# JWT
JWT_SECRET_KEY="..."
JWT_ISSUER="JELA.API"
JWT_AUDIENCE="JelaWeb"

# Azure Services
AZURE_OPENAI_API_KEY="..."
AZURE_DOCUMENT_INTELLIGENCE_KEY="..."
```

---

## 📋 CHECKLIST DE CÓDIGO

### Antes de Commit
- [ ] Código compila sin warnings
- [ ] Tests pasan
- [ ] Code style consistente
- [ ] Documentación actualizada
- [ ] Secrets no hardcodeados
- [ ] Logs apropiados
- [ ] Errores manejados

### Code Review
- [ ] Arquitectura correcta
- [ ] Seguridad implementada
- [ ] Performance optimizada
- [ ] Tests incluidos
- [ ] Documentación completa

---

## 📞 CONTACTOS

- **Arquitecto:** [Nombre] - Diseño de sistema
- **Tech Lead:** [Nombre] - Liderazgo técnico
- **DevOps:** [Nombre] - Deployment y infraestructura
- **Security:** [Nombre] - Auditorías de seguridad

---

## 🔗 REFERENCIAS

- [.NET 8 Minimal APIs Documentation](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [C# Coding Standards](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- [OWASP Security Guidelines](https://owasp.org/www-project-top-ten/)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)