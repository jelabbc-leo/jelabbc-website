# 📚 DOCUMENTACIÓN TÉCNICA - JELA.API

**Fecha:** Enero 21, 2026  
**Versión:** 1.0  
**Proyecto:** JELA.API - API Backend para Sistema de jela-api-logistica  
**Framework:** .NET 8 Minimal APIs  

---

## 🎯 VISIÓN GENERAL

JELA.API es una API REST moderna construida con .NET 8 Minimal APIs que proporciona servicios backend para el sistema de jela-api-logistica JELA. Implementa un enfoque **100% dinámico** para operaciones CRUD, autenticación JWT, integración con IA (Azure OpenAI), y procesamiento de documentos.

### Arquitectura Principal
- **Minimal APIs** con .NET 8
- **Inyección de Dependencias** nativa
- **Autenticación JWT** con refresh tokens
- **Sistema CRUD Dinámico** sin queries hardcodeadas
- **Integración Multi-Canal** (WhatsApp, VAPI, Firebase)
- **Procesamiento de IA** con Azure OpenAI
- **OCR de Documentos** con Azure Document Intelligence

---

## 🏗️ ARQUITECTURA DEL SISTEMA

### Estructura de Carpetas
```
JELA.API/
├── JELA.API/                    # Proyecto principal
│   ├── Configuration/           # Clases de configuración
│   │   ├── JwtSettings.cs       # Configuración JWT
│   │   └── AllowedTablesSettings.cs # Tablas permitidas
│   ├── Endpoints/               # Definición de endpoints
│   │   ├── AuthEndpoints.cs     # Autenticación
│   │   ├── CrudEndpoints.cs     # CRUD dinámico
│   │   ├── WebhookEndpoints.cs  # Webhooks multi-canal
│   │   ├── HealthEndpoints.cs   # Health checks
│   │   ├── OpenAIEndpoints.cs   # Integración IA
│   │   └── DocumentIntelligenceEndpoints.cs # OCR
│   ├── Middleware/              # Middleware personalizado
│   │   ├── RequestLoggingMiddleware.cs
│   │   └── RateLimitingMiddleware.cs
│   ├── Models/                  # DTOs y modelos
│   │   ├── AuthModels.cs        # Modelos de autenticación
│   │   ├── CrudModels.cs        # Modelos CRUD
│   │   ├── TicketModels.cs      # Modelos de tickets
│   │   └── WebhookModels.cs     # Modelos de webhooks
│   ├── Services/                # Lógica de negocio
│   │   ├── JwtAuthService.cs    # Servicio de autenticación
│   │   ├── MySqlDatabaseService.cs # Acceso a BD
│   │   ├── AzureOpenAIService.cs # IA
│   │   ├── TicketValidationService.cs # Validación tickets
│   │   └── [otros servicios...]
│   ├── Program.cs               # Punto de entrada
│   ├── appsettings.json         # Configuración
│   └── JELA.API.csproj          # Archivo de proyecto
├── nuget.config                 # Configuración NuGet
├── JELA.API.sln                 # Solución
└── README.md                    # Documentación general
```

---

## 🔧 CONFIGURACIÓN Y DEPENDENCIAS

### appsettings.json
```json
{
  "ConnectionStrings": {
    "MySQL": "Server=...;Database=jela_qa;Uid=...;Pwd=...;SslMode=Required"
  },
  "Jwt": {
    "SecretKey": "your-secret-key-at-least-32-characters",
    "Issuer": "JELA.API",
    "Audience": "JelaWeb",
    "ExpirationMinutes": 60
  },
  "AllowedTables": {
    "AllowedPrefixes": ["cat_", "conf_", "op_", "log_", "vw_"],
    "BlockedTables": ["conf_refresh_tokens"]
  },
  "AzureOpenAI": {
    "Endpoint": "https://jelagpt.openai.azure.com/",
    "ApiKey": "your-api-key",
    "DeploymentName": "gpt-4"
  },
  "AzureDocumentIntelligence": {
    "Endpoint": "https://jelapdf.cognitiveservices.azure.com/",
    "ApiKey": "your-api-key"
  }
}
```

### NuGet Packages Críticos
- `Microsoft.AspNetCore.Authentication.JwtBearer` - Autenticación JWT
- `MySqlConnector` - Conexión MySQL
- `Azure.AI.OpenAI` - Integración OpenAI
- `Azure.AI.DocumentIntelligence` - OCR
- `Serilog` - Logging
- `Swashbuckle.AspNetCore` - Swagger

---

## 🚀 ENDPOINTS DISPONIBLES

### Autenticación
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/auth/login` | Inicio de sesión |
| POST | `/api/auth/refresh` | Renovación de token |
| GET | `/api/auth/validate` | Validación de token |

### CRUD Dinámico
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/crud?strQuery={sql}` | Consulta SELECT |
| POST | `/api/crud/{table}` | Insertar registro |
| PUT | `/api/crud/{table}/{id}` | Actualizar registro |
| DELETE | `/api/crud/{table}?idField={campo}&idValue={valor}` | Eliminar registro |

### Salud y Monitoreo
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/health` | Estado general |
| GET | `/api/version` | Versión del API |
| GET | `/health/live` | Health check liveness |
| GET | `/health/ready` | Health check readiness |

### Webhooks Multi-Canal
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/webhooks/vapi` | Webhook llamadas VAPI |
| POST | `/api/webhooks/y-cloud` | Webhook WhatsApp YCloud |
| POST | `/api/webhooks/firebase` | Webhook app móvil |
| POST | `/api/webhooks/chat-web` | Webhook chat web |

### IA y Documentos
| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/openai/ticket-analysis` | Análisis de tickets con IA |
| POST | `/api/document-intelligence` | OCR de documentos |

---

## 🔐 SEGURIDAD

### Autenticación JWT
- **Algoritmo:** HS256
- **Expiración:** 60 minutos (configurable)
- **Refresh Tokens:** Soportados
- **Issuer:** JELA.API
- **Audience:** JelaWeb

### Autorización
- **Rate Limiting:** 100 requests/minuto por IP/usuario
- **Validación de Tablas:** Solo prefijos autorizados (`cat_`, `conf_`, `op_`, etc.)
- **SQL Injection Prevention:** Uso de parámetros preparados
- **Auditoría:** Logging completo de operaciones

### CORS
- Configurado para permitir solo orígenes autorizados
- Headers de seguridad incluidos

---

## 📊 BASE DE DATOS

### Estructura de Tablas
- **cat_***: Catálogos (entidades, proveedores, colonias, etc.)
- **conf_***: Configuración (usuarios, roles, opciones, prompts)
- **op_***: Operativas (tickets, interacciones, pagos, etc.)
- **log_***: Auditoría y logs

### Sistema CRUD Dinámico
- **Sin queries hardcodeadas**
- **Validación automática de esquemas**
- **Soporte para cualquier tabla**
- **Campos auto-detectados**

---

## 🤖 INTEGRACIÓN CON IA

### Azure OpenAI
- **Modelo:** GPT-4
- **Prompts dinámicos:** Almacenados en `conf_ticket_prompts`
- **Canales soportados:** VAPI, YCloud, ChatWeb, Firebase
- **Sistema 100% dinámico:** Sin fallbacks hardcodeados

### Azure Document Intelligence
- **Procesamiento:** INE, Licencias de Conducir
- **Formato salida:** JSON estructurado
- **Validación automática**

---

## 📈 MONITOREO Y LOGGING

### Serilog Configuration
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/jela-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

### Health Checks
- **Database connectivity**
- **External services** (Azure OpenAI, Document Intelligence)
- **Memory usage**
- **Response times**

---

## 🧪 TESTING

### Estrategia
- **Unit Tests:** Servicios individuales
- **Integration Tests:** Endpoints completos
- **API Tests:** Postman/Newman
- **Load Tests:** Simulación de carga

### Herramientas
- **xUnit** para unit tests
- **TestServer** para integration tests
- **Postman** para API testing
- **Application Insights** para monitoreo

---

## 🚀 DEPLOYMENT

### Azure App Service
- **Runtime:** .NET 8
- **Database:** Azure Database for MySQL
- **Configuration:** Variables de entorno
- **Scaling:** Auto-scaling configurado

### CI/CD
- **GitHub Actions**
- **Azure DevOps Pipelines**
- **Automated testing**
- **Blue-green deployments**

---

## 📋 CHECKLIST DE CALIDAD

### Código
- [ ] Sin warnings de compilación
- [ ] Cobertura de tests > 80%
- [ ] Code analysis aprobado
- [ ] Documentación actualizada

### Seguridad
- [ ] Secrets en Key Vault
- [ ] HTTPS obligatorio
- [ ] Headers de seguridad
- [ ] Rate limiting activo

### Performance
- [ ] Response time < 500ms
- [ ] Memory usage optimizado
- [ ] Database queries optimizadas
- [ ] Caching implementado

---

## 🔄 MIGRACIÓN Y COMPATIBILIDAD

### Desde API Anterior
- **WebService/WebApplication1** → **JELA.API**
- Endpoints compatibles
- Base de datos compartida
- Autenticación migrada

### Versionado
- **Semantic versioning**
- **API versioning** con headers
- **Backward compatibility**
- **Deprecation notices**

---

## 👥 EQUIPO Y ROLES

### Desarrollo
- **Arquitecto:** Diseño de sistema y arquitectura
- **Backend Developers:** Implementación de servicios
- **DevOps:** Deployment y monitoreo
- **QA:** Testing y calidad

### Operaciones
- **SRE:** Site reliability engineering
- **DBA:** Administración de base de datos
- **Security:** Auditorías de seguridad

---

## 📚 REFERENCIAS

- [README.md](../README.md) - Documentación general
- [Program.cs](./Program.cs) - Punto de entrada
- [appsettings.json](./appsettings.json) - Configuración
- [.kiro/specs/jela-api/rules.md](./rules.md) - Reglas de programación
- [.kiro/specs/jela-api/architecture.md](./architecture.md) - Arquitectura detallada
