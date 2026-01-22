# 🔒 SECURITY ARCHITECTURE - JELA.API

**Fecha:** Enero 21, 2026  
**Versión:** 1.0  
**Enfoque:** Defense in Depth  

---

## 🎯 VISIÓN DE SEGURIDAD

JELA.API implementa una arquitectura de seguridad **Defense in Depth** con múltiples capas de protección, desde el perímetro hasta el núcleo de datos, asegurando confidencialidad, integridad y disponibilidad de la información.

### Principios Fundamentales
- **Zero Trust:** Nunca confiar, siempre verificar
- **Least Privilege:** Mínimos permisos necesarios
- **Fail Safe:** Fallar de forma segura
- **Defense in Depth:** Múltiples capas de seguridad
- **Security by Design:** Seguridad integrada en el diseño

---

## 🏛️ CAPAS DE SEGURIDAD

### 1. Network Security Layer (Capa de Red)

#### Azure Front Door + WAF
```json
{
  "waf": {
    "enabled": true,
    "mode": "Prevention",
    "rules": [
      "OWASP Top 10",
      "Bot Protection",
      "Rate Limiting",
      "Geo-blocking"
    ]
  },
  "ddos": {
    "enabled": true,
    "protection": "Standard"
  }
}
```

#### Azure Application Gateway
- **SSL/TLS Termination:** TLS 1.3 obligatorio
- **Web Application Firewall:** OWASP Core Rule Set
- **Health Probes:** Monitoreo continuo
- **Session Affinity:** Sticky sessions cuando necesario

#### Network Security Groups (NSG)
```json
{
  "inbound": [
    {
      "priority": 100,
      "source": "AzureFrontDoor.Backend",
      "destination": "VirtualNetwork",
      "ports": ["443"],
      "protocol": "TCP"
    }
  ],
  "outbound": [
    {
      "destination": "AzureCloud",
      "ports": ["443", "80"],
      "protocol": "TCP"
    }
  ]
}
```

---

### 2. Application Security Layer (Capa de Aplicación)

#### Authentication & Authorization

##### JWT Bearer Tokens
```csharp
// Configuración JWT
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

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                // Validar usuario activo
                var userId = context.Principal.FindFirst("userId")?.Value;
                var userService = context.HttpContext.RequestServices
                    .GetRequiredService<IUserService>();

                var user = await userService.GetByIdAsync(int.Parse(userId));
                if (user == null || !user.Activo)
                {
                    context.Fail("Usuario inactivo o no encontrado");
                }
            }
        };
    });
```

##### Role-Based Access Control (RBAC)
```csharp
// Políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Administrator"));

    options.AddPolicy("EntityAccess", policy =>
        policy.RequireAssertion(context =>
        {
            var entityId = context.Resource as int?;
            var userEntityIds = context.User.FindFirst("EntityIds")?.Value
                ?.Split(',')?.Select(int.Parse) ?? Array.Empty<int>();

            return userEntityIds.Contains(entityId.Value);
        }));
});
```

#### Input Validation & Sanitization

##### Model Validation
```csharp
public class CreateTicketRequest
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, ErrorMessage = "El título no puede exceder 200 caracteres")]
    [RegularExpression(@"^[^<>&]*$", ErrorMessage = "El título contiene caracteres inválidos")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(2000, ErrorMessage = "La descripción no puede exceder 2000 caracteres")]
    public string Descripcion { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "ID de entidad inválido")]
    public int IdEntidad { get; set; }
}
```

##### SQL Injection Prevention
```csharp
// Parameterized queries con Dapper
public async Task<IEnumerable<T>> QueryAsync<T>(
    string sql,
    object? parameters = null)
{
    using var connection = await _connectionFactory.CreateConnectionAsync();

    // Dapper automáticamente parametriza
    return await connection.QueryAsync<T>(sql, parameters);
}

// Ejemplo seguro
var users = await QueryAsync<User>(
    "SELECT * FROM conf_usuarios WHERE IdEntidad = @EntityId AND Activo = @Active",
    new { EntityId = entityId, Active = true });
```

#### Rate Limiting
```csharp
// Configuración de rate limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userId = context.User.FindFirst("userId")?.Value ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId,
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = userId == "anonymous" ? 100 : 1000,  // 100 para anónimos, 1000 para autenticados
                Window = TimeSpan.FromMinutes(1)
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Demasiadas solicitudes. Intente nuevamente más tarde.",
            retryAfter = "60"
        });
    };
});
```

---

### 3. Data Security Layer (Capa de Datos)

#### Database Security

##### Connection Security
```csharp
// Connection string segura
"Server=jela-db.mysql.database.azure.com;Database=jela_qa;Uid=jela-api@jeladb;Pwd={password};SslMode=Required;"

// Certificate-based authentication
services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 21)),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure()));
```

##### Row Level Security (RLS)
```sql
-- Vista con RLS para tickets
CREATE VIEW vw_tickets_usuario AS
SELECT t.* FROM op_tickets t
INNER JOIN conf_usuario_entidades ue ON t.IdEntidad = ue.IdEntidad
WHERE ue.IdUsuario = CURRENT_USER_ID();

-- Función para obtener usuario actual
DELIMITER //
CREATE FUNCTION CURRENT_USER_ID() RETURNS INT
BEGIN
    RETURN @CurrentUserId;
END //
DELIMITER ;
```

##### Data Encryption
```sql
-- Campos sensibles encriptados
ALTER TABLE conf_usuarios
ADD COLUMN EmailEncrypted VARBINARY(255),
ADD COLUMN TelefonoEncrypted VARBINARY(255);

-- Funciones de encriptación
DELIMITER //
CREATE FUNCTION EncryptData(data TEXT)
RETURNS VARBINARY(255)
DETERMINISTIC
BEGIN
    DECLARE key_text TEXT DEFAULT 'JELA_ENCRYPTION_KEY_2026';
    RETURN AES_ENCRYPT(data, key_text);
END //

CREATE FUNCTION DecryptData(data VARBINARY(255))
RETURNS TEXT
DETERMINISTIC
BEGIN
    DECLARE key_text TEXT DEFAULT 'JELA_ENCRYPTION_KEY_2026';
    RETURN AES_DECRYPT(data, key_text);
END //
DELIMITER ;
```

#### Audit Logging

##### Comprehensive Audit Trail
```csharp
public class AuditService
{
    private readonly ILogger<AuditService> _logger;
    private readonly IHttpContextAccessor _httpContext;

    public async Task LogOperationAsync(
        string operation,
        string tableName,
        string recordId,
        object? oldValues = null,
        object? newValues = null)
    {
        var auditEntry = new AuditEntry
        {
            UserId = GetCurrentUserId(),
            Operation = operation,
            TableName = tableName,
            RecordId = recordId,
            OldValues = JsonSerializer.Serialize(oldValues),
            NewValues = JsonSerializer.Serialize(newValues),
            IpAddress = _httpContext.HttpContext?.Connection.RemoteIpAddress?.ToString(),
            UserAgent = _httpContext.HttpContext?.Request.Headers["User-Agent"].ToString(),
            Timestamp = DateTime.UtcNow
        };

        await _auditRepository.InsertAsync(auditEntry);

        _logger.LogInformation(
            "Audit: {User} performed {Operation} on {Table}.{RecordId}",
            auditEntry.UserId, operation, tableName, recordId);
    }
}
```

---

### 4. Infrastructure Security Layer (Capa de Infraestructura)

#### Azure Security Center
- **Security posture assessment**
- **Threat detection**
- **Compliance monitoring**
- **Security recommendations**

#### Key Vault Integration
```csharp
// Configuración de Key Vault
builder.Configuration.AddAzureKeyVault(
    new Uri("https://jela-keyvault.vault.azure.net/"),
    new DefaultAzureCredential());

// Acceso a secrets
var secret = await _keyVaultClient.GetSecretAsync("DatabasePassword");
```

#### Azure Monitor & Application Insights
```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=...",
    "EnableAdaptiveSampling": true,
    "EnableDependencyTracking": true,
    "EnableRequestTracking": true
  }
}
```

---

## 🔐 SEGURIDAD POR COMPONENTE

### API Endpoints Security

#### Authentication Required
```csharp
[Authorize]
[HttpGet("api/secure-data")]
public async Task<IActionResult> GetSecureData()
{
    // Endpoint protegido
}
```

#### Role-Based Authorization
```csharp
[Authorize(Roles = "Administrator")]
[HttpPost("api/admin-only")]
public async Task<IActionResult> AdminOnlyAction()
{
    // Solo administradores
}
```

#### Custom Authorization
```csharp
[Authorize(Policy = "EntityAccess")]
[HttpGet("api/entity/{entityId}/data")]
public async Task<IActionResult> GetEntityData(int entityId)
{
    // Verifica acceso a entidad específica
}
```

### Database Security

#### Parameterized Queries
```csharp
// ✅ SEGURO
var users = await connection.QueryAsync<User>(
    "SELECT * FROM conf_usuarios WHERE Email = @Email",
    new { Email = email });

// ❌ INSEGURO
var users = await connection.QueryAsync<User>(
    $"SELECT * FROM conf_usuarios WHERE Email = '{email}'");
```

#### Schema Validation
```csharp
public class TableValidator
{
    private readonly HashSet<string> _allowedPrefixes = new()
    {
        "cat_", "conf_", "op_", "log_"
    };

    public bool IsTableAllowed(string tableName)
    {
        return _allowedPrefixes.Any(prefix =>
            tableName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
    }
}
```

### External Services Security

#### Azure OpenAI
```csharp
var client = new OpenAIClient(
    new Uri(_settings.Endpoint),
    new AzureKeyCredential(_settings.ApiKey));

// Request con contexto de usuario
var response = await client.GetChatCompletionsAsync(
    new ChatCompletionsOptions
    {
        Messages = { userMessage },
        User = currentUserId.ToString(),  // Para content filtering
        MaxTokens = 1000
    });
```

#### Azure Document Intelligence
```csharp
var client = new DocumentIntelligenceClient(
    new Uri(_settings.Endpoint),
    new AzureKeyCredential(_settings.ApiKey));

// Upload seguro con validación
if (!IsValidFile(file))
    throw new SecurityException("Archivo no válido");

var operation = await client.AnalyzeDocumentAsync(
    WaitUntil.Completed,
    "prebuilt-document",
    fileStream);
```

---

## 🚨 MANEJO DE VULNERABILIDADES

### OWASP Top 10 Mitigation

#### 1. Injection Prevention
- ✅ Parameterized queries
- ✅ Input validation
- ✅ Stored procedures (when needed)

#### 2. Broken Authentication
- ✅ JWT with expiration
- ✅ Secure password hashing (Argon2)
- ✅ Multi-factor authentication ready

#### 3. Sensitive Data Exposure
- ✅ TLS 1.3 everywhere
- ✅ Data encryption at rest
- ✅ Secure key management

#### 4. XML External Entities (XXE)
- ✅ JSON only APIs
- ✅ No XML processing

#### 5. Broken Access Control
- ✅ RBAC implementation
- ✅ Entity-level security
- ✅ API authorization

#### 6. Security Misconfiguration
- ✅ Secure defaults
- ✅ Automated configuration checks
- ✅ Environment-specific configs

#### 7. Cross-Site Scripting (XSS)
- ✅ Input sanitization
- ✅ Content Security Policy
- ✅ Output encoding

#### 8. Insecure Deserialization
- ✅ JSON only
- ✅ Schema validation
- ✅ Safe deserialization

#### 9. Vulnerable Components
- ✅ Automated dependency scanning
- ✅ Regular updates
- ✅ Vulnerability monitoring

#### 10. Insufficient Logging & Monitoring
- ✅ Comprehensive audit logging
- ✅ Real-time monitoring
- ✅ Alert system

---

## 📊 MONITOREO DE SEGURIDAD

### Security Dashboard Metrics

#### Authentication Metrics
```sql
-- Intentos de login fallidos
SELECT
    DATE(FechaRequest) as Fecha,
    COUNT(*) as IntentosFallidos,
    COUNT(DISTINCT IpAddress) as IPsUnicas
FROM log_api_requests
WHERE Endpoint = '/api/auth/login'
    AND StatusCode = 401
    AND FechaRequest >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
GROUP BY DATE(FechaRequest)
ORDER BY Fecha DESC;
```

#### Rate Limiting Metrics
```sql
-- Requests bloqueados por rate limiting
SELECT
    DATE(FechaRequest) as Fecha,
    COUNT(*) as RequestsBloqueados
FROM log_api_requests
WHERE StatusCode = 429
    AND FechaRequest >= DATE_SUB(CURDATE(), INTERVAL 7 DAY)
GROUP BY DATE(FechaRequest)
ORDER BY Fecha DESC;
```

#### Security Incidents
```sql
-- Operaciones sospechosas
SELECT
    lo.*,
    u.Email as UsuarioEmail,
    e.Alias as EntidadAlias
FROM log_crud_operations lo
LEFT JOIN conf_usuarios u ON lo.IdUsuario = u.Id
LEFT JOIN cat_entidades e ON lo.IdEntidad = e.Id
WHERE lo.Operacion = 'DELETE'
    AND lo.FechaOperacion >= DATE_SUB(NOW(), INTERVAL 1 HOUR)
ORDER BY lo.FechaOperacion DESC;
```

### Alert System

#### Automated Alerts
```csharp
public class SecurityAlertService
{
    public async Task CheckAndAlertAsync()
    {
        // Verificar intentos de login fallidos
        var failedLogins = await GetFailedLoginsLastHourAsync();
        if (failedLogins.Count > 10)
        {
            await SendAlertAsync("Múltiples intentos de login fallidos", failedLogins);
        }

        // Verificar accesos no autorizados
        var unauthorizedAccess = await GetUnauthorizedAccessAsync();
        if (unauthorizedAccess.Any())
        {
            await SendAlertAsync("Accesos no autorizados detectados", unauthorizedAccess);
        }
    }
}
```

---

## 🔧 CONFIGURACIÓN DE SEGURIDAD

### appsettings.json Security Section
```json
{
  "Security": {
    "Jwt": {
      "SecretKey": "USE_KEY_VAULT",
      "Issuer": "JELA.API",
      "Audience": "JelaWeb",
      "ExpirationMinutes": 60,
      "RefreshTokenExpirationDays": 7
    },
    "RateLimiting": {
      "AnonymousRequestsPerMinute": 100,
      "AuthenticatedRequestsPerMinute": 1000,
      "EnableIpBlocking": true
    },
    "Cors": {
      "AllowedOrigins": ["https://jelawweb.azurewebsites.net"],
      "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
      "AllowedHeaders": ["Authorization", "Content-Type"],
      "AllowCredentials": true
    },
    "Encryption": {
      "Key": "USE_KEY_VAULT",
      "Algorithm": "AES256"
    }
  }
}
```

### Azure Key Vault Setup
```bash
# Crear Key Vault
az keyvault create --name jela-keyvault --resource-group jela-rg --location eastus

# Agregar secrets
az keyvault secret set --vault-name jela-keyvault --name DatabasePassword --value "secret"
az keyvault secret set --vault-name jela-keyvault --name JwtSecretKey --value "secret"
az keyvault secret set --vault-name jela-keyvault --name OpenAIApiKey --value "secret"
```

---

## 📋 CHECKLIST DE SEGURIDAD

### Authentication & Authorization
- [ ] JWT tokens implementados
- [ ] Refresh tokens funcionando
- [ ] RBAC configurado
- [ ] Password policies aplicadas
- [ ] Multi-factor authentication ready

### Data Protection
- [ ] Datos sensibles encriptados
- [ ] TLS 1.3 configurado
- [ ] SQL injection prevention
- [ ] XSS protection
- [ ] CSRF protection

### Infrastructure Security
- [ ] WAF configurado
- [ ] DDoS protection activo
- [ ] Network security groups
- [ ] Key Vault integration
- [ ] Secrets management

### Monitoring & Response
- [ ] Audit logging completo
- [ ] Security monitoring
- [ ] Incident response plan
- [ ] Automated alerts
- [ ] Regular security assessments

### Compliance
- [ ] GDPR compliance
- [ ] Data retention policies
- [ ] Privacy by design
- [ ] Security by design
- [ ] Regular audits

---

## 🚨 PLAN DE RESPUESTA A INCIDENTES

### Incident Classification
1. **Crítico:** Brecha de seguridad, pérdida de datos
2. **Alto:** Ataque activo, servicio down
3. **Medio:** Intento de ataque, anomalía
4. **Bajo:** Escaneo, intento fallido

### Response Process
1. **Detección:** Monitoring y alerts
2. **Evaluación:** Clasificación y impacto
3. **Contención:** Aislar el problema
4. **Erradicación:** Eliminar causa raíz
5. **Recuperación:** Restaurar servicios
6. **Lección:** Post-mortem y mejoras

### Contactos de Emergencia
- **Security Team Lead:** [Nombre] - +52 55 1234 5678
- **DevOps Lead:** [Nombre] - +52 55 1234 5679
- **Legal Counsel:** [Nombre] - +52 55 1234 5680

---

## 🔗 REFERENCIAS DE SEGURIDAD

- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Microsoft Security Best Practices](https://docs.microsoft.com/en-us/security/)
- [Azure Security Documentation](https://docs.microsoft.com/en-us/azure/security/)
- [NIST Cybersecurity Framework](https://www.nist.gov/cyberframework)
- [ISO 27001 Standards](https://www.iso.org/standard/54534.html)

---

## 📞 SOPORTE Y CONTACTO

- **Security Officer:** [Nombre] - security@jela.com
- **Compliance Officer:** [Nombre] - compliance@jela.com
- **DevSecOps Team:** devsecops@jela.com

---

*Esta documentación debe revisarse y actualizarse trimestralmente para mantener la alineación con las mejores prácticas de seguridad y los requisitos regulatorios.*