# JELA.API

API dinámica para el sistema JELA - jela-api-logistica.

## Tecnologías

- **.NET 8** (Minimal API)
- **MySQL** (Azure Database for MySQL)
- **JWT** para autenticación
- **Serilog** para logging
- **Swagger/OpenAPI** para documentación

## Estructura del Proyecto

```
JELA.API/
├── JELA.API/
│   ├── Configuration/       # Clases de configuración
│   ├── Endpoints/           # Endpoints de la API
│   ├── Middleware/          # Middleware personalizado
│   ├── Models/              # DTOs y modelos
│   ├── Services/            # Servicios de negocio
│   ├── Program.cs           # Punto de entrada
│   ├── appsettings.json     # Configuración
│   └── JELA.API.csproj      # Archivo de proyecto
├── nuget.config             # Configuración de NuGet
├── JELA.API.sln             # Archivo de solución
└── README.md
```

## Endpoints

### Health
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/health` | Estado del API y BD |
| GET | `/api/version` | Versión del API |
| GET | `/health/live` | Liveness probe |
| GET | `/health/ready` | Readiness probe |

### Autenticación
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/auth/login` | Iniciar sesión |
| POST | `/api/auth/refresh` | Renovar token |
| GET | `/api/auth/validate` | Validar token actual |

### CRUD Dinámico
| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/crud?strQuery={sql}` | Ejecutar SELECT |
| POST | `/api/crud/{tabla}` | Insertar registro |
| PUT | `/api/crud/{tabla}/{id}` | Actualizar registro |
| DELETE | `/api/crud/{tabla}?idField={campo}&idValue={valor}` | Eliminar registro |

### Document Intelligence
| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/document-intelligence` | Procesar archivo PDF/JPG/PNG (INE o Tarjeta de Circulación) |

## Configuración

### appsettings.json

```json
{
  "ConnectionStrings": {
    "MySQL": "Server=...;Database=...;Uid=...;Pwd=...;SslMode=Required"
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
  "AzureDocumentIntelligence": {
    "Endpoint": "https://jelapdf.cognitiveservices.azure.com/formrecognizer/documentModels/prebuilt-document:analyze?api-version=2023-07-31",
    "ApiKey": "tu-api-key"
  }
}
```

## Ejecutar en Desarrollo

```bash
cd JELA.API/JELA.API
dotnet run
```

El API estará disponible en:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger UI: http://localhost:5000/

## Compilar para Producción

```bash
dotnet publish -c Release -o ./publish
```

## Desplegar en Azure

Ver la guía completa en [DEPLOY-AZURE.md](./DEPLOY-AZURE.md)

### Despliegue Rápido con Script

1. Copia `azure-config.example.json` a `azure-config.json` y completa los valores
2. Ejecuta el script de despliegue:

```powershell
.\deploy-azure.ps1 -ResourceGroupName "jela-resources" -AppServiceName "jela-api"
```

### Despliegue Manual

1. Crea un App Service en Azure Portal
2. Publica desde Visual Studio (clic derecho → Publicar)
3. Configura las variables de entorno en Azure Portal
4. Actualiza `Web.config` de JelaWeb con la nueva URL

## Seguridad

- **Autenticación JWT**: Todos los endpoints CRUD requieren token válido
- **Rate Limiting**: 100 requests por minuto por IP/usuario
- **Validación de tablas**: Solo se permiten tablas con prefijos autorizados
- **CORS**: Configurado para permitir solo orígenes autorizados

## Migración desde WebService anterior

Este proyecto reemplaza al API anterior (`WebService/WebApplication1`). Los endpoints son compatibles:

| Endpoint Anterior | Endpoint Nuevo |
|-------------------|----------------|
| `GET api/Crud?strQuery=...` | `GET api/crud?strQuery=...` |
| `POST api/Crud?table=...` | `POST api/crud/{tabla}` |
| `PUT api/Crud/{table}/{id}` | `PUT api/crud/{tabla}/{id}` |
| `DELETE api/Crud?table=...&idField=...&idValue=...` | `DELETE api/crud/{tabla}?idField=...&idValue=...` |

### Cambios en JelaWeb

Para usar el nuevo API, actualizar `Web.config` de JelaWeb:

```xml
<appSettings>
  <add key="ApiBaseUrl" value="https://nueva-url-api/api/crud?strQuery=" />
  <add key="APIPost" value="https://nueva-url-api/api/crud" />
</appSettings>
```

## Logs

Los logs se guardan en:
- Consola (desarrollo)
- Archivo: `logs/jela-api-{fecha}.log`

## Health Checks

- `/health/live`: Verifica que la aplicación responde
- `/health/ready`: Verifica conexión a MySQL

## Contacto

JELA BBC - soporte@jelabbc.com
