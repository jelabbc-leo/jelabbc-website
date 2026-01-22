# Guía de Despliegue de JELA.API en Azure

## Requisitos Previos
- Cuenta de Azure con suscripción activa
- Azure CLI instalado (opcional, pero recomendado)
- Visual Studio 2022 o .NET SDK 8.0
- Acceso a Azure Portal

## Opción 1: Despliegue desde Visual Studio (Recomendado)

### Paso 1: Preparar el Proyecto
1. Abre la solución `JELA.API.sln` en Visual Studio
2. Asegúrate de que el proyecto esté configurado para **Release**
3. Verifica que todas las dependencias estén instaladas

### Paso 2: Crear App Service en Azure Portal
1. Ve a [Azure Portal](https://portal.azure.com)
2. Clic en **"Crear un recurso"**
3. Busca **"App Service"** y selecciona
4. Configura:
   - **Suscripción**: Tu suscripción
   - **Grupo de recursos**: Crea uno nuevo o usa existente (ej: `jela-resources`)
   - **Nombre**: `jela-api` (o el que prefieras, debe ser único)
   - **Publicar**: Código
   - **Pila en tiempo de ejecución**: .NET 8
   - **Sistema operativo**: Linux (recomendado) o Windows
   - **Región**: La más cercana a tus usuarios
   - **Plan de App Service**: 
     - **Crear nuevo** o usar existente
     - **SKU y tamaño**: B1 (Basic) para desarrollo, S1 (Standard) para producción
5. Clic en **"Revisar y crear"** → **"Crear"**

### Paso 3: Publicar desde Visual Studio
1. En Visual Studio, clic derecho en el proyecto `JELA.API`
2. Selecciona **"Publicar"**
3. Selecciona **"Azure"** → **"Azure App Service (Linux)"** o **"Azure App Service (Windows)"**
4. Selecciona tu suscripción y el App Service creado
5. Clic en **"Finalizar"**
6. En la página de publicación:
   - **Configuración**: Release
   - **Implementación**: Framework dependiente (más rápido) o Autocontenida
7. Clic en **"Publicar"**

## Opción 2: Despliegue con Azure CLI

### Paso 1: Instalar Azure CLI
```powershell
# Descargar desde: https://aka.ms/installazurecliwindows
# O usar winget:
winget install -e --id Microsoft.AzureCLI
```

### Paso 2: Login y Configuración
```powershell
# Login a Azure
az login

# Seleccionar suscripción
az account set --subscription "TU-SUSCRIPCION-ID"

# Crear grupo de recursos (si no existe)
az group create --name jela-resources --location eastus

# Crear App Service Plan
az appservice plan create \
  --name jela-api-plan \
  --resource-group jela-resources \
  --sku B1 \
  --is-linux

# Crear Web App
az webapp create \
  --name jela-api \
  --resource-group jela-resources \
  --plan jela-api-plan \
  --runtime "DOTNET|8.0"
```

### Paso 3: Publicar desde Terminal
```powershell
cd d:\DesarrolloWEB\JelaAzure\JelaWeb\JELA.API\JELA.API

# Compilar en modo Release
dotnet publish -c Release -o ./publish

# Publicar a Azure
az webapp deployment source config-zip \
  --resource-group jela-resources \
  --name jela-api \
  --src ./publish.zip
```

## Configuración Post-Despliegue

### Paso 1: Configurar Connection Strings y App Settings

En Azure Portal:
1. Ve a tu App Service `jela-api`
2. En el menú lateral, ve a **"Configuración"** → **"Configuración de la aplicación"**
3. Agrega las siguientes **Configuración de la aplicación**:

#### Connection Strings
```
Name: MySQL
Value: Server=jela.mysql.database.azure.com;Database=jela_qa;Uid=jlsg;Pwd=Jela@2025;SslMode=Required
Type: Custom
```

#### Application Settings
```json
{
  "Jwt:SecretKey": "TU_CLAVE_SECRETA_SUPER_SEGURA_DE_AL_MENOS_32_CARACTERES_2025!",
  "Jwt:Issuer": "JELA.API",
  "Jwt:Audience": "JelaWeb",
  "Jwt:ExpirationMinutes": "60",
  "Jwt:RefreshTokenExpirationDays": "7",
  
  "AzureDocumentIntelligence:Endpoint": "https://jelapdf.cognitiveservices.azure.com/formrecognizer/documentModels/prebuilt-document:analyze?api-version=2023-07-31",
  "AzureDocumentIntelligence:ApiKey": "TU_API_KEY_DE_AZURE_DOCUMENT_INTELLIGENCE",
  
  "AllowedTables:AllowedPrefixes__0": "cat_",
  "AllowedTables:AllowedPrefixes__1": "conf_",
  "AllowedTables:AllowedPrefixes__2": "op_",
  "AllowedTables:AllowedPrefixes__3": "log_",
  "AllowedTables:AllowedPrefixes__4": "vw_",
  "AllowedTables:BlockedTables__0": "conf_refresh_tokens",
  
  "RateLimiting:PermitLimit": "100",
  "RateLimiting:WindowMinutes": "1",
  
  "ASPNETCORE_ENVIRONMENT": "Production",
  "ASPNETCORE_URLS": "http://+:80"
}
```

**Nota**: Para arrays en Azure App Settings, usa la sintaxis `__0`, `__1`, etc.

### Paso 2: Configurar Connection String (Alternativa)
Si prefieres usar Connection Strings en lugar de App Settings:

1. Ve a **"Configuración"** → **"Cadenas de conexión"**
2. Agrega:
   - **Nombre**: `MySQL`
   - **Valor**: `Server=jela.mysql.database.azure.com;Database=jela_qa;Uid=jlsg;Pwd=Jela@2025;SslMode=Required`
   - **Tipo**: Personalizada

### Paso 3: Actualizar appsettings.json (Opcional)
Si prefieres mantener configuración en `appsettings.json`, actualiza `appsettings.Production.json`:

```json
{
  "ConnectionStrings": {
    "MySQL": "Server=jela.mysql.database.azure.com;Database=jela_qa;Uid=jlsg;Pwd=Jela@2025;SslMode=Required"
  },
  "Jwt": {
    "SecretKey": "TU_CLAVE_SECRETA_EN_PRODUCTION",
    "Issuer": "JELA.API",
    "Audience": "JelaWeb",
    "ExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "AzureDocumentIntelligence": {
    "Endpoint": "https://jelapdf.cognitiveservices.azure.com/formrecognizer/documentModels/prebuilt-document:analyze?api-version=2023-07-31",
    "ApiKey": "TU_API_KEY"
  }
}
```

## Configuración de CORS (Si es necesario)

Si JelaWeb está en un dominio diferente:

1. En Azure Portal, ve a **"API"** → **"CORS"**
2. Agrega los orígenes permitidos:
   - `https://tu-dominio-jelaweb.azurewebsites.net`
   - O `*` para desarrollo (no recomendado en producción)

O configura en `Program.cs`:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowJelaWeb", policy =>
    {
        policy.WithOrigins("https://tu-dominio-jelaweb.azurewebsites.net")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

## Actualizar JelaWeb para usar la nueva API

### Paso 1: Actualizar Web.config
En `JelaWeb/Web.config`, actualiza las URLs:

```xml
<!-- API de producción (Azure) -->
<add key="ApiBaseUrl" value="https://jela-api.azurewebsites.net/api/CRUD?strQuery=" />
<add key="APIPost" value="https://jela-api.azurewebsites.net/api/CRUD?table=" />
```

### Paso 2: Verificar Configuración
Asegúrate de que:
- La URL base de la API esté correcta
- El proxy de Document Intelligence use la URL correcta
- Las credenciales JWT sean las mismas en ambos proyectos

## Verificación Post-Despliegue

### 1. Verificar Health Endpoint
```powershell
curl https://jela-api.azurewebsites.net/api/health
```

Debería retornar:
```json
{
  "Status": "Healthy",
  "Timestamp": "...",
  "Checks": {
    "database": { "Status": "Healthy" },
    "api": { "Status": "Healthy" }
  }
}
```

### 2. Verificar Swagger
Abre en el navegador:
```
https://jela-api.azurewebsites.net/swagger
```

### 3. Probar Login
```powershell
curl -X POST https://jela-api.azurewebsites.net/api/auth/login `
  -H "Content-Type: application/json" `
  -d '{"username":"admin","password":"admin123"}'
```

### 4. Verificar Logs
En Azure Portal:
1. Ve a tu App Service
2. **"Supervisión"** → **"Registros de App Service"**
3. Habilita **"Registro de aplicaciones (Filesystem)"**
4. Nivel: **"Información"** o **"Advertencia"**
5. Guarda y revisa los logs

## Troubleshooting

### Error: "No se puede conectar a la base de datos"
- Verifica que la Connection String esté correcta
- Verifica que el firewall de Azure MySQL permita conexiones desde App Service
- En Azure MySQL, ve a **"Seguridad de conexión"** y agrega la IP del App Service

### Error: "401 Unauthorized"
- Verifica que `Jwt:SecretKey` sea la misma en JelaWeb y JELA.API
- Verifica que `Jwt:Issuer` y `Jwt:Audience` coincidan

### Error: "404 Not Found" en endpoints
- Verifica que el despliegue se haya completado correctamente
- Revisa los logs de App Service
- Verifica que `ASPNETCORE_ENVIRONMENT` esté configurado

### Error: "500 Internal Server Error"
- Revisa los logs de App Service
- Verifica que todas las configuraciones estén correctas
- Verifica que Azure Document Intelligence esté configurado

## Comandos Útiles

### Ver logs en tiempo real
```powershell
az webapp log tail --name jela-api --resource-group jela-resources
```

### Reiniciar App Service
```powershell
az webapp restart --name jela-api --resource-group jela-resources
```

### Ver configuración actual
```powershell
az webapp config appsettings list --name jela-api --resource-group jela-resources
```

### Actualizar configuración
```powershell
az webapp config appsettings set \
  --name jela-api \
  --resource-group jela-resources \
  --settings "Jwt:SecretKey=NuevaClave"
```

## Seguridad Adicional

### 1. Habilitar HTTPS
- Azure App Service ya incluye HTTPS por defecto
- Puedes configurar un dominio personalizado y certificado SSL

### 2. Configurar Authentication/Authorization
En Azure Portal:
1. Ve a **"Autenticación"**
2. Puedes habilitar Azure AD, Google, etc. (opcional)

### 3. Configurar IP Restrictions
1. Ve a **"Redes"** → **"Restricciones de acceso"**
2. Agrega reglas para permitir solo IPs específicas

### 4. Usar Key Vault para Secrets
Para mayor seguridad, almacena secrets en Azure Key Vault:
```powershell
# Crear Key Vault
az keyvault create --name jela-keyvault --resource-group jela-resources

# Agregar secretos
az keyvault secret set --vault-name jela-keyvault --name "JwtSecretKey" --value "TU_CLAVE"

# Configurar App Service para usar Key Vault
az webapp config appsettings set \
  --name jela-api \
  --resource-group jela-resources \
  --settings "@Microsoft.KeyVault(SecretUri=https://jela-keyvault.vault.azure.net/secrets/JwtSecretKey/)"
```

## Costos Estimados

- **App Service Plan B1 (Basic)**: ~$13/mes
- **App Service Plan S1 (Standard)**: ~$70/mes
- **Azure Document Intelligence**: Pay-as-you-go (varía según uso)

## Próximos Pasos

1. ✅ Desplegar API en Azure
2. ✅ Configurar variables de entorno
3. ✅ Actualizar JelaWeb para usar la nueva URL
4. ✅ Probar endpoints desde producción
5. ✅ Configurar monitoreo y alertas
6. ✅ Configurar CI/CD (Azure DevOps o GitHub Actions)
