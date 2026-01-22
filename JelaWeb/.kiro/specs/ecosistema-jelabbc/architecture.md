# Arquitectura Tecnológica - Ecosistema JELABBC

## 1. STACK TECNOLÓGICO

### 1.1 BACKEND

**Lenguaje**: Visual Basic .NET (VB.NET)  
**Framework**: ASP.NET Web Application (.NET Framework 4.8.1)  
**Arquitectura**: API REST con controlador CRUD dinámico  
**Patrón**: SqlHelpers + DTO dinámico  
**Proyecto**: WebApplication1

#### Componentes clave:

- **CrudController (VB.NET)**: Controlador REST genérico que maneja operaciones CRUD dinámicas
- **SqlHelpers**: Clase auxiliar para ejecución de consultas MySQL (ExecutarConsulta, EjecutarNoConsulta, EjecutarEscalar)
- **CRUDDTO**: Objeto de transferencia de datos dinámico con diccionario de campos tipados
- **Services**: Servicios transversales (email, SMS, IA)

#### Funcionamiento del API Dinámico:

El backend usa un patrón de API dinámico donde un solo controlador maneja todas las operaciones:

```vb
' GET: Ejecutar SELECT dinámico
GET /api/CRUD?strQuery=SELECT * FROM tabla

' POST: Insertar registro
POST /api/CRUD/{tabla}
Body: { "Campos": { "nombre": { "Valor": "Juan", "Tipo": "System.String" } } }

' PUT: Actualizar registro
PUT /api/CRUD/{tabla}/{id}
Body: { "Campos": { "nombre": { "Valor": "Juan", "Tipo": "System.String" } } }

' DELETE: Eliminar registro
DELETE /api/CRUD?table={tabla}&idField={campo}&idValue={valor}
```

**Respuesta JSON**: Todos los datos se retornan con estructura tipada.

Esta arquitectura permite:
- ✅ Sin necesidad de crear controladores específicos por cada tabla
- ✅ Consultas SQL parametrizadas (prevención de SQL injection)
- ✅ Tipado automático de datos desde MySQL
- ✅ Respuestas JSON estructuradas con metadatos de tipo
- ✅ Flexibilidad total en consultas SELECT con JOINs
- ✅ Un solo punto de mantenimiento en SqlHelpers

### 1.2 BASE DE DATOS

**Motor**: MySQL 8.0+  
**Conector**: MySql.Data.MySqlClient (NuGet)  
**ConnectionString**: ConexionMySQL (en Web.config)  
**Charset**: utf8mb4  
**Collation**: utf8mb4_unicode_ci

**Características**:
- Tablas con prefijos operativos usando guion bajo (ej: `op_tickets`, `conf_usuarios`)
- Stored procedures para lógica compleja
- Índices optimizados para consultas frecuentes
- Respaldos automáticos diarios

### 1.3 FRONTEND WEB

**Tecnologías base del frontend web (MVP)**:
- ASP.NET Web Forms (.NET Framework 4.8.1)
- VB.NET para code-behind
- DevExpress ASP.NET 22.2 (componentes UI)
- Bootstrap 5 (diseño responsivo)
- jQuery + JavaScript modular

**Responsabilidades del frontend web**:
- Renderizado de UI con controles DevExpress
- Validación cliente (JavaScript)
- Llamadas AJAX a API REST
- Manejo de sesión y autenticación
- Dashboards personalizados por rol

**Evolución futura**:
Más adelante, cuando el proyecto escale, este frontend web se podrá migrar a Blazor o React montando un API Gateway delante del backend, pero para el MVP la tecnología oficial es:

**ASP.NET Web Forms (.NET Framework 4.8) + VB.NET + DevExpress ASP.NET 22.2**

### 1.4 APLICACIONES MÓVILES

**Objetivo**: Proveer acceso móvil nativo para residentes, técnicos y productores agrícolas, con capacidades offline y uso intensivo de cámara y notificaciones.

**Tecnología recomendada (MVP)**:
- .NET MAUI (multiplataforma iOS/Android)
- Alternativa: Desarrollo nativo (Swift/Kotlin)

**Módulos móviles prioritarios**:
1. Tickets de soporte (crear, consultar, adjuntar fotos)
2. Órdenes de trabajo para técnicos
3. Consulta de saldo y pagos (residentes)
4. Monitoreo agrícola IoT
5. Modo offline con sincronización

**Integración con otras capas**:
- Consume API REST existente (CrudController + AuthController)
- No reimplementa lógica de negocio en el cliente
- Almacenamiento local con SQLite
- Sincronización bidireccional

**Estructura sugerida para el proyecto móvil**:
```
JELABBC.Mobile/
├── JELABBC.Core/          # Lógica compartida
│   ├── Models/
│   ├── Services/
│   ├── ViewModels/
│   └── Helpers/
├── JELABBC.iOS/           # Proyecto iOS
├── JELABBC.Android/       # Proyecto Android
└── JELABBC.Shared/        # Recursos compartidos
```


### 1.5 INTELIGENCIA ARTIFICIAL

**Proveedor**: Azure OpenAI Service  
**Modelo**: GPT-4  

**Funcionalidades**:
- Análisis de documentos
- Validación de órdenes de compra
- Clasificación automática de tickets
- Agente de voz conversacional 24/7
- Generación de reportes
- Análisis de sentimiento
- Resumen automático de conversaciones

### 1.6 IoT

**Plataforma**: Azure IoT Hub  
**Protocolos**: MQTT, HTTPS  
**Dispositivos**: Sensores de humedad, temperatura, pH  
**Gateways**: Raspberry Pi / ESP32

**Funcionalidades**:
- Monitoreo en tiempo real de cultivos
- Alertas automáticas por umbrales
- Control de riego automatizado
- Historial de lecturas
- Dashboards de visualización

### 1.7 CLOUD & INFRAESTRUCTURA

**Proveedor**: Microsoft Azure  
**Region**: East US / West US (redundancia)

**Servicios principales**:
- Azure App Service (hosting web)
- Azure Database for MySQL
- Azure Storage (blobs, archivos)
- Azure Key Vault (secretos)
- Azure Application Insights (monitoreo)
- Azure DevOps (CI/CD)
- Azure IoT Hub (dispositivos IoT)
- Azure OpenAI Service (IA)

## 2. ARQUITECTURA DE CAPAS

### 2.1 CAPA DE PRESENTACIÓN

**Portal Web (ASP.NET Web Forms)**
- Directorio: `/Views/`
- Tecnología: .aspx + VB.NET code-behind
- Responsabilidades:
  - Renderizado de UI
  - Validación cliente (JavaScript)
  - Llamadas AJAX a API
  - Manejo de sesión

**Apps Móviles**
- iOS App: JELABBC.iOS
- Android App: JELABBC.Android
- Arquitectura: MVVM
- Comunicación: REST API + JWT

### 2.2 CAPA DE SERVICIOS (API REST)

**Controllers (VB.NET)**
- CrudController.vb (CRUD dinámico)
- AuthController.vb (autenticación)
- AgentIAController.vb (IA)

**Rutas API principales**:
```
/api/CRUD                   - CRUD dinámico
/api/auth                   - Autenticación
/api/entidades              - Gestión de organizaciones
/api/usuarios               - Gestión de usuarios
/api/tickets                - Sistema de tickets
/api/ordenes                - Órdenes de compra
/api/facturacion            - Facturación y tarifas
/api/agricultura            - Módulo agrícola
/api/iot                    - Datos de sensores IoT
/api/agente                 - Agente IA
```

### 2.3 CAPA DE NEGOCIO

**Business Logic Layer**
- Directorio: `/Business/`
- Responsabilidades:
  - Reglas de negocio
  - Validaciones complejas
  - Orquestación de operaciones
  - Llamadas a servicios externos

**Clases principales**:
- EntidadesBusiness.vb
- UsuariosBusiness.vb
- TicketsBusiness.vb
- FacturacionBusiness.vb
- AgriculturaBusiness.vb
- IAAgentBusiness.vb

### 2.4 CAPA DE ACCESO A DATOS

**Data Access Layer (DAL)**
- Directorio: `/DataAccess/`
- Tecnología: ADO.NET + MySql.Data.MySqlClient
- Patrón: Repository Pattern

**Clase base de conexión**: SqlHelpers.vb

### 2.5 CAPA DE SERVICIOS EXTERNOS

**External Services Layer**
- Directorio: `/ExternalServices/`

**Servicios**:
- EmailService.vb (SMTP)
- WhatsAppService.vb (Twilio/WhatsApp API)
- SMSService.vb (Twilio)
- OpenAIService.vb (Azure OpenAI)
- IoTHubService.vb (Azure IoT Hub)
- StorageService.vb (Azure Blob Storage)

### 2.6 CAPA DE ORQUESTACIÓN (n8n / WORKFLOWS)

Para orquestar procesos complejos entre módulos (tickets, órdenes, IA, notificaciones), el ecosistema utiliza **n8n** como motor de workflows.

**Responsabilidades de la capa n8n**:
- Automatización de notificaciones (email, SMS, WhatsApp)
- Orquestación de flujos de aprobación
- Integración con servicios externos
- Procesamiento de alertas IoT
- Workflows de IA (análisis, clasificación)

**Workflows principales**:
1. Tickets de soporte (notificaciones automáticas)
2. Órdenes de compra (workflow de aprobación)
3. Alertas IoT (procesamiento y notificación)
4. Agente de voz IA (transcripción y acciones)

## 3. ESTRUCTURA DEL PROYECTO

```
WebApplication1/
├── App_Data/
│   └── (archivos temporales)
├── Business/
│   ├── EntidadesBusiness.vb
│   ├── UsuariosBusiness.vb
│   ├── TicketsBusiness.vb
│   ├── FacturacionBusiness.vb
│   ├── AgriculturaBusiness.vb
│   └── IAAgentBusiness.vb
├── Controllers/
│   ├── CrudController.vb
│   ├── AuthController.vb
│   ├── EntidadesController.vb
│   ├── UsuariosController.vb
│   ├── TicketsController.vb
│   ├── OrdenesController.vb
│   ├── FacturacionController.vb
│   ├── AgriculturaController.vb
│   ├── IoTController.vb
│   └── AgentIAController.vb
├── DataAccess/
│   ├── DataAccess.vb (clase base)
│   ├── SqlHelpers.vb
│   ├── EntidadesDA.vb
│   ├── UsuariosDA.vb
│   ├── TicketsDA.vb
│   └── etc...
├── ExternalServices/
│   ├── EmailService.vb
│   ├── WhatsAppService.vb
│   ├── SMSService.vb
│   ├── OpenAIService.vb
│   └── IoTHubService.vb
├── Models/
│   ├── CRUDDTO.vb
│   ├── Entidad.vb
│   ├── Usuario.vb
│   ├── Ticket.vb
│   └── etc...
├── Views/
│   ├── Dashboard.aspx           (resumen general por rol)
│   ├── Entidades.aspx           (catentidades)
│   ├── Usuarios.aspx            (confusuarios)
│   ├── Tickets.aspx             (op_tickets + comentarios/archivos)
│   ├── Ordenes.aspx             (op_documentos + detalle)
│   ├── Facturacion.aspx         (cattarifas, op_facturas, op_pagos)
│   └── Medidores.aspx           (catmedidor + lecturas IoT)
├── Scripts/
│   ├── app/
│   │   ├── shared/
│   │   │   ├── common.js
│   │   │   └── validation.js
│   │   └── modules/
│   │       ├── entidades.js
│   │       ├── tickets.js
│   │       ├── ordenes.js
│   │       └── facturacion.js
├── Content/
│   ├── css/
│   │   ├── modules/
│   │   │   ├── entidades.css
│   │   │   ├── tickets.css
│   │   │   └── ordenes.css
│   │   └── site.css
│   └── images/
├── Helpers/
│   └── FuncionesGridWeb.vb
├── Global.asax
└── Web.config
```

Cada página .aspx tendrá su correspondiente code-behind en VB.NET (ejemplo: Entidades.aspx.vb, Usuarios.aspx.vb) y usará controles DevExpress ASP.NET 22.2 (ASPxGridView, ASPxFormLayout, ASPxPopupControl, etc.) para construir los formularios, listados y dashboards.


## 4. CONFIGURACIÓN Y DEPENDENCIAS

### 4.1 WEB.CONFIG

```xml
<configuration>
  <connectionStrings>
    <add name="ConexionMySQL" 
         connectionString="Server=jelabbc-mysql.mysql.database.azure.com;Database=jela_qa;Uid=admin;Pwd=***;SslMode=Required;" 
         providerName="MySql.Data.MySqlClient" />
  </connectionStrings>
  
  <appSettings>
    <add key="AzureOpenAI_Endpoint" value="https://jelabbc-openai.openai.azure.com/" />
    <add key="AzureOpenAI_Key" value="***" />
    <add key="AzureIoTHub_ConnectionString" value="***" />
    <add key="Twilio_AccountSid" value="***" />
    <add key="Twilio_AuthToken" value="***" />
    <add key="SMTP_Host" value="smtp.office365.com" />
    <add key="SMTP_Port" value="587" />
  </appSettings>
</configuration>
```

### 4.2 PAQUETES NUGET

```
Microsoft.AspNet.WebApi.Core (5.2.9)
Microsoft.AspNet.WebApi.WebHost (5.2.9)
MySql.Data (8.0.33)
Newtonsoft.Json (13.0.3)
Azure.AI.OpenAI (1.0.0-beta.8)
Microsoft.Azure.Devices (1.39.0)
Microsoft.ApplicationInsights (2.21.0)
Twilio (6.10.0)
MailKit (4.1.0)
System.IdentityModel.Tokens.Jwt (6.32.0)
DevExpress.Web (22.2.x)
```

## 5. PATRONES DE DISEÑO

### 5.1 REPOSITORY PATTERN
Separación entre lógica de negocio y acceso a datos

### 5.2 DEPENDENCY INJECTION
Inyección manual en constructores (VB.NET no tiene DI nativo en .NET 4.8)

### 5.3 DTO PATTERN
Data Transfer Objects para comunicación entre capas

### 5.4 SINGLETON PATTERN
Para servicios externos (EmailService, WhatsAppService, etc.)

## 6. SEGURIDAD

### 6.1 AUTENTICACIÓN

- JWT tokens para API REST
- Sesiones ASP.NET para portal web
- Biometría en apps móviles (Touch ID, Face ID)
- Autenticación de dos factores (2FA) opcional

### 6.2 AUTORIZACIÓN

- Roles y permisos granulares
- Validación en cada endpoint API
- Filtrado de datos por entidad/usuario
- Auditoría de accesos

### 6.3 PROTECCIÓN DE DATOS

- HTTPS/TLS obligatorio
- Cifrado de datos sensibles en BD
- Azure Key Vault para secretos
- Cumplimiento GDPR/LFPDPPP

## 7. MONITOREO Y LOGS

### 7.1 APPLICATION INSIGHTS

- Telemetría de aplicación
- Logs de errores y excepciones
- Métricas de rendimiento
- Alertas automáticas

### 7.2 MÉTRICAS CLAVE

- Tiempo de respuesta API
- Tasa de errores
- Uso de recursos (CPU, memoria)
- Disponibilidad del sistema
- Satisfacción del usuario (CSAT)

## 8. INFRAESTRUCTURA AZURE

### 8.1 RECURSOS NECESARIOS

```
Resource Group: rg-jelabbc-prod
App Service Plan: asp-jelabbc-prod (S2: 2 cores, 3.5GB RAM)
App Service: app-jelabbc-prod
MySQL Server: jelabbc-mysql.mysql.database.azure.com
Storage Account: stjelabbcprod
Key Vault: kv-jelabbc-prod
Application Insights: ai-jelabbc-prod
IoT Hub: jelabbc-iot (S1 tier)
CDN Profile: cdn-jelabbc-prod
```

### 8.2 ESTIMACIÓN DE COSTOS MENSUALES (USD)

```
App Service S2: $146/mes
MySQL (General Purpose 2vCore): $150/mes
Storage (100GB): $5/mes
Application Insights: $25/mes
IoT Hub S1: $25/mes
CDN: $10/mes
Key Vault: $5/mes

Total estimado: ~$366/mes
```

## 9. CI/CD CON AZURE DEVOPS

### 9.1 PIPELINE DE BUILD

1. Checkout código
2. Restaurar paquetes NuGet
3. Compilar solución
4. Ejecutar pruebas unitarias
5. Generar artefactos
6. Publicar artefactos

### 9.2 PIPELINE DE RELEASE

1. Staging → Aprobación → Production
2. Deploy to Azure App Service
3. Run smoke tests
4. Notify team

**Nota**: Los workflows n8n se versionan exportando los JSON de n8n al repositorio y desplegándolos como parte del pipeline.

## RESUMEN ARQUITECTÓNICO

La arquitectura del Ecosistema JELABBC está diseñada para:

✅ **Escalabilidad**: Soportar crecimiento de usuarios y datos  
✅ **Seguridad**: Protección de datos sensibles  
✅ **Mantenibilidad**: Código organizado y bien estructurado  
✅ **Disponibilidad**: Alta disponibilidad (99.9%)  
✅ **Rendimiento**: Tiempos de respuesta óptimos  
✅ **Observabilidad**: Monitoreo completo del sistema

---

**Última actualización**: Diciembre 2024  
**Versión**: 1.0  
**Mantenido por**: Equipo de Arquitectura JELABBC

