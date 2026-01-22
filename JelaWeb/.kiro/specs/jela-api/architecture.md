# 🏗️ ARQUITECTURA DETALLADA - JELA.API

**Fecha:** Enero 21, 2026  
**Versión:** 1.0  
**Proyecto:** JELA.API  

---

## 🎯 VISIÓN ARQUITECTURAL

JELA.API implementa una arquitectura **modular y escalable** basada en principios SOLID, Clean Architecture y Domain-Driven Design (DDD), optimizada para Minimal APIs de .NET 8.

### Principios Fundamentales
- **Separation of Concerns**: Capas claramente definidas
- **Dependency Inversion**: Interfaces sobre implementaciones
- **Single Responsibility**: Un propósito por componente
- **Open/Closed**: Extensible sin modificar código existente
- **Liskov Substitution**: Interfaces intercambiables
- **Interface Segregation**: Interfaces específicas
- **Dependency Injection**: Inyección automática de dependencias

---

## 🏛️ ARQUITECTURA EN CAPAS

### Diagrama de Arquitectura
```
┌─────────────────────────────────────────────────┐
│                PRESENTATION LAYER               │
│  ┌─────────────────────────────────────────────┐ │
│  │           MINIMAL API ENDPOINTS             │ │
│  │  • AuthEndpoints.cs                        │ │
│  │  • CrudEndpoints.cs                        │ │
│  │  • WebhookEndpoints.cs                     │ │
│  └─────────────────────────────────────────────┘ │
│  ┌─────────────────────────────────────────────┐ │
│  │            MIDDLEWARE LAYER                 │ │
│  │  • RequestLoggingMiddleware.cs             │ │
│  │  • RateLimitingMiddleware.cs               │ │
│  └─────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────┐
│              APPLICATION LAYER                  │
│  ┌─────────────────────────────────────────────┐ │
│  │            SERVICES LAYER                   │ │
│  │  • JwtAuthService.cs                       │ │
│  │  • MySqlDatabaseService.cs                 │ │
│  │  • AzureOpenAIService.cs                   │ │
│  │  • TicketValidationService.cs              │ │
│  └─────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
                        │
                        ▼
┌─────────────────────────────────────────────────┐
│               INFRASTRUCTURE LAYER              │
│  ┌─────────────────────────────────────────────┐ │
│  │            EXTERNAL SERVICES                │ │
│  │  • Azure OpenAI                             │ │
│  │  • Azure Document Intelligence              │ │
│  │  • MySQL Database                           │ │
│  │  • VAPI, YCloud, Firebase                   │ │
│  └─────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
```

---

## 📋 DESCRIPCIÓN DE CAPAS

### 1. Presentation Layer (Capa de Presentación)

#### Minimal API Endpoints
**Responsabilidad:** Definir rutas HTTP y manejar requests/responses

**Características:**
- Endpoints RESTful
- Validación de entrada
- Mapeo request/response
- Error handling
- OpenAPI/Swagger generation

**Ejemplo:**
```csharp
public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest request,
            IAuthService authService) =>
        {
            var result = await authService.LoginAsync(request);
            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Unauthorized();
        });
    }
}
```

#### Middleware Layer
**Responsabilidad:** Interceptar y procesar requests/responses

**Middleware Implementado:**
- **RequestLoggingMiddleware**: Logging de todas las requests
- **RateLimitingMiddleware**: Control de tasa de requests
- **CORS Middleware**: Control de orígenes cruzados
- **Authentication Middleware**: Validación JWT

---

### 2. Application Layer (Capa de Aplicación)

#### Services Layer
**Responsabilidad:** Contener lógica de negocio y coordinar operaciones

**Tipos de Servicios:**
- **Domain Services**: Lógica de negocio pura
- **Application Services**: Coordinación de operaciones
- **Infrastructure Services**: Acceso a recursos externos

**Patrón de Servicio:**
```csharp
public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result<bool>> ValidateTokenAsync(string token);
}

public class JwtAuthService : IAuthService
{
    // Implementación
}
```

#### Cross-Cutting Concerns
- **Logging**: Serilog para logging estructurado
- **Caching**: In-memory cache para datos frecuentes
- **Validation**: FluentValidation para reglas de negocio
- **Error Handling**: Result pattern para manejo consistente

---

### 3. Infrastructure Layer (Capa de Infraestructura)

#### External Services Integration
**Responsabilidad:** Comunicación con servicios externos

**Servicios Integrados:**
- **Azure OpenAI**: Procesamiento de lenguaje natural
- **Azure Document Intelligence**: OCR de documentos
- **MySQL Database**: Persistencia de datos
- **VAPI**: Integración telefónica
- **YCloud**: WhatsApp Business API
- **Firebase**: Notificaciones push

**Patrón Adapter:**
```csharp
public interface IOpenAIService
{
    Task<Result<string>> GenerateResponseAsync(string prompt);
}

public class AzureOpenAIService : IOpenAIService
{
    private readonly OpenAIClient _client;

    public AzureOpenAIService(IOptions<AzureOpenAISettings> settings)
    {
        _client = new OpenAIClient(
            new Uri(settings.Value.Endpoint),
            new AzureKeyCredential(settings.Value.ApiKey));
    }
}
```

---

## 🔄 FLUJOS DE DATOS

### Request Flow
```
Client Request
       ↓
   Middleware
       ↓
  Endpoint Handler
       ↓
   Service Layer
       ↓
Infrastructure Layer
       ↓
External Services
       ↓
   Response
```

### Authentication Flow
```
Login Request
     ↓
Validate Credentials
     ↓
Generate JWT Token
     ↓
Store Refresh Token
     ↓
Return Access Token
```

### CRUD Dynamic Flow
```
CRUD Request
     ↓
Validate Table Name
     ↓
Check Permissions
     ↓
Execute Query
     ↓
Log Operation
     ↓
Return Result
```

---

## 📊 PATRONES DE DISEÑO IMPLEMENTADOS

### Repository Pattern
```csharp
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<int> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

public class MySqlRepository<T> : IRepository<T>
{
    // Implementación con Dapper
}
```

### Unit of Work Pattern
```csharp
public interface IUnitOfWork
{
    IRepository<User> Users { get; }
    IRepository<Order> Orders { get; }
    Task<int> SaveChangesAsync();
}

public class MySqlUnitOfWork : IUnitOfWork
{
    // Implementación con transacciones
}
```

### Result Pattern
```csharp
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public int StatusCode { get; private set; }

    public static Result<T> Success(T value, int statusCode = 200) =>
        new Result<T> { IsSuccess = true, Value = value, StatusCode = statusCode };

    public static Result<T> Failure(string error, int statusCode = 400) =>
        new Result<T> { IsSuccess = false, Error = error, StatusCode = statusCode };
}
```

### CQRS Pattern (Command Query Responsibility Segregation)
```csharp
// Commands
public record CreateUserCommand(string Name, string Email);

// Queries
public record GetUserQuery(int UserId);

// Handlers
public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<int>>
{
    // Implementation
}
```

---

## 🔐 SEGURIDAD ARQUITECTURAL

### Defense in Depth
1. **Network Level**: WAF, DDoS protection
2. **Application Level**: Input validation, authentication
3. **Data Level**: Parameterized queries, encryption
4. **Monitoring**: Logging, alerting

### Authentication & Authorization
- **JWT Bearer Tokens**: Stateless authentication
- **Role-Based Access Control**: Permisos por rol
- **Claim-Based Authorization**: Claims personalizados
- **Refresh Tokens**: Rotación automática

### Data Protection
- **Encryption at Rest**: Datos sensibles encriptados
- **Encryption in Transit**: TLS 1.3 obligatorio
- **API Keys**: Rotación automática
- **Secrets Management**: Azure Key Vault

---

## ⚡ OPTIMIZACIONES DE PERFORMANCE

### Caching Strategy
```csharp
public class CacheService
{
    private readonly IDistributedCache _cache;

    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null)
    {
        var cached = await _cache.GetStringAsync(key);
        if (cached != null)
            return JsonSerializer.Deserialize<T>(cached);

        var result = await factory();
        await _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(result),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
            });

        return result;
    }
}
```

### Database Optimization
- **Connection Pooling**: Configurado automáticamente
- **Query Optimization**: Indexes apropiados
- **Batch Operations**: Para operaciones masivas
- **Read Replicas**: Para consultas de solo lectura

### Async Programming
- **Async/Await**: Todas las operaciones I/O
- **Task.WhenAll**: Para operaciones paralelas
- **CancellationToken**: Para cancelación de operaciones
- **ConfigureAwait(false)**: En librerías

---

## 🧪 ESTRATEGIA DE TESTING

### Testing Pyramid
```
End-to-End Tests (E2E)
       ▲
Integration Tests
       ▲
Unit Tests (Base)
```

### Unit Tests
```csharp
[Fact]
public async Task LoginAsync_ValidCredentials_ReturnsToken()
{
    // Arrange
    var service = new JwtAuthService(_mockUserRepo, _mockJwtSettings);

    // Act
    var result = await service.LoginAsync(new LoginRequest
    {
        Email = "user@example.com",
        Password = "password"
    });

    // Assert
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value.Token);
}
```

### Integration Tests
```csharp
[Fact]
public async Task LoginEndpoint_ValidRequest_ReturnsOk()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.PostAsJsonAsync("/api/auth/login", new
    {
        email = "user@example.com",
        password = "password"
    });

    // Assert
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
    Assert.NotNull(result?.Token);
}
```

---

## 📈 MONITOREO Y OBSERVABILIDAD

### Application Insights
- **Request Tracking**: Latencia, throughput, error rates
- **Dependency Tracking**: Llamadas a servicios externos
- **Custom Metrics**: Métricas de negocio
- **Distributed Tracing**: Seguimiento de requests

### Health Checks
```csharp
builder.Services.AddHealthChecks()
    .AddMySql(_connectionString)
    .AddAzureOpenAI(options => options.UseApiKey(_openAiKey))
    .AddUrlGroup(new Uri("https://external-service.com/health"));
```

### Logging Strategy
- **Structured Logging**: Con Serilog
- **Log Levels**: Trace, Debug, Information, Warning, Error, Critical
- **Correlation IDs**: Para tracking de requests
- **PII Filtering**: Datos sensibles filtrados

---

## 🚀 ESCALABILIDAD Y RESILIENCIA

### Horizontal Scaling
- **Stateless Design**: Sin estado en servidor
- **External Session Store**: Redis para sesiones
- **Load Balancing**: Azure Front Door
- **Auto-scaling**: Basado en métricas

### Resilience Patterns
- **Circuit Breaker**: Para servicios externos
- **Retry Policy**: Con exponential backoff
- **Timeout**: Configurable por operación
- **Fallback**: Respuestas por defecto

### Database Scaling
- **Read Replicas**: Para consultas de lectura
- **Sharding**: Por entidad/tenant
- **Connection Pooling**: Optimizado
- **Query Optimization**: Indexes y query plans

---

## 🔄 ESTRATEGIA DE DEPLOYMENT

### CI/CD Pipeline
```
Source Code
     ↓
   Build
     ↓
   Tests
     ↓
   Security Scan
     ↓
   Artifact
     ↓
   Staging
     ↓
   Production
```

### Blue-Green Deployment
- **Zero Downtime**: Tráfico redirigido gradualmente
- **Rollback**: Capacidad de rollback inmediato
- **Canary Releases**: Deployments graduales
- **Feature Flags**: Activación/desactivación de features

### Configuration Management
- **Environment Variables**: Para configuración específica
- **Azure App Configuration**: Para configuración centralizada
- **Key Vault**: Para secrets
- **Feature Flags**: Para control de features

---

## 📊 MÉTRICAS DE ARQUITECTURA

### Performance Targets
- **Response Time**: P95 < 500ms
- **Availability**: 99.9% uptime
- **Error Rate**: < 0.1%
- **Throughput**: 1000 requests/second

### Code Quality Metrics
- **Cyclomatic Complexity**: < 10 por método
- **Code Coverage**: > 80%
- **Technical Debt**: < 5%
- **Maintainability Index**: > 80

### Security Metrics
- **Vulnerabilities**: 0 críticas
- **Compliance**: SOC 2, GDPR
- **Penetration Tests**: Trimestrales
- **Incident Response**: < 1 hora

---

## 🔮 EVOLUCIÓN ARQUITECTURAL

### Roadmap
1. **Q1 2026**: Microservicios para módulos críticos
2. **Q2 2026**: Event-driven architecture
3. **Q3 2026**: Multi-cloud deployment
4. **Q4 2026**: AI/ML integration avanzada

### Technical Debt Reduction
- [ ] Migrar servicios legacy
- [ ] Implementar CQRS completo
- [ ] Agregar event sourcing
- [ ] Optimizar queries N+1

### Innovation Opportunities
- **GraphQL API**: Para queries complejas
- **gRPC**: Para comunicación interna
- **WebSockets**: Para real-time features
- **Edge Computing**: Para baja latencia

---

## 📚 REFERENCIAS ARQUITECTURALES

### Patrones Implementados
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design](https://dddcommunity.org/)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Result Pattern](https://www.pluralsight.com/tech-blog/result-pattern-csharp)

### Frameworks y Librerías
- [.NET 8 Minimal APIs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [MediatR](https://github.com/jbogard/MediatR) - Mediator pattern
- [FluentValidation](https://fluentvalidation.net/) - Validation
- [Polly](https://github.com/App-vNext/Polly) - Resilience

### Herramientas
- [Azure Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
- [Serilog](https://serilog.net/) - Logging
- [Dapper](https://github.com/DapperLib/Dapper) - ORM
- [xUnit](https://xunit.net/) - Testing

---

## 👥 ROLES Y RESPONSABILIDADES

### Arquitecto de Software
- Diseño de arquitectura general
- Revisión de código crítico
- Definición de estándares
- Roadmap técnico

### Tech Lead
- Liderazgo del equipo de desarrollo
- Code reviews
- Mentoring técnico
- Coordinación con otros equipos

### Backend Developers
- Implementación de servicios
- Testing y debugging
- Documentación técnica
- Optimización de performance

### DevOps Engineer
- CI/CD pipelines
- Infrastructure as Code
- Monitoring y alerting
- Security hardening

### QA Engineer
- Test planning y execution
- Automation frameworks
- Performance testing
- Quality metrics

---

## 📞 CONTACTOS

- **Chief Architect:** [Nombre] - Visión arquitectural
- **Technical Lead:** [Nombre] - Implementación técnica
- **DevOps Lead:** [Nombre] - Infraestructura y deployment
- **Security Officer:** [Nombre] - Seguridad y compliance

---

## 🔗 ENLACES RELACIONADOS

- [README.md](./README.md) - Documentación general
- [rules.md](./rules.md) - Reglas de programación
- [.kiro/specs/jela-api/endpoints.md](./endpoints.md) - Documentación de endpoints
- [.kiro/specs/jela-api/database.md](./database.md) - Diseño de base de datos
- [.kiro/specs/jela-api/security.md](./security.md) - Arquitectura de seguridad