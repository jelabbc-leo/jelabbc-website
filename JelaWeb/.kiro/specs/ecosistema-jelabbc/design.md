# Documento de Diseño - Ecosistema JELABBC

## Visión General

El Ecosistema JELABBC es una plataforma integral de gestión empresarial multi-industria que integra IA, IoT y automatización. Este documento describe el diseño técnico del **frontend web** (ASP.NET Web Forms con VB.NET) y las **aplicaciones móviles** (nativas iOS/Android o MAUI), considerando que el backend REST API ya está desarrollado y operativo.

### Contexto Actual

**Backend Existente:**
- API REST operativa en Azure: `https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net`
- Base de datos MySQL
- Endpoints CRUD genéricos disponibles

**Frontend Existente:**
- ASP.NET Web Forms (.NET Framework 4.8.1) con VB.NET
- DevExpress v22.2 para componentes UI
- Bootstrap 5 para diseño responsivo
- Módulos implementados: Login, Entidades, Conceptos, Captura de Documentos básica

**Integraciones Existentes:**
- Flujos N8N para automatizaciones
- Azure OpenAI Service (GPT-4) para IA
- Azure IoT Hub para dispositivos IoT

### Objetivos del Diseño

1. Completar el portal web con todos los módulos faltantes
2. Desarrollar aplicaciones móviles con funcionalidad offline
3. Integrar Agente de Voz IA para atención 24/7
4. Implementar módulo de agricultura inteligente con IoT
5. Garantizar seguridad, rendimiento y accesibilidad

### Estándares de UI

**IMPORTANTE**: Todos los módulos del portal web deben seguir los estándares de UI documentados en `ui-standards.md`. Este documento define reglas críticas para:

- Separación de CSS/JS (NUNCA inline)
- Nomenclatura contextual de botones
- Configuración estándar de ASPxGridView
- Uso de popups modales para captura de datos
- Estructura de páginas y layout
- Accesibilidad y navegación por teclado
- Notificaciones con Toastr
- Seguridad en UI

**Ver**: [Estándares de UI](./ui-standards.md) para detalles completos.

## Arquitectura

### Arquitectura General del Sistema

```
┌─────────────────────────────────────────────────────────────┐
│                    USUARIOS FINALES                          │
│  (Administradores, Residentes, Técnicos, Productores)       │
└────────────┬────────────────────────────┬───────────────────┘
             │                            │
             ▼                            ▼
┌────────────────────────┐   ┌──────────────────────────┐
│    PORTAL WEB          │   │   APPS MÓVILES           │
│  (ASP.NET Web Forms)   │   │  (iOS/Android/MAUI)      │
│  - VB.NET              │   │  - Modo Offline          │
│  - DevExpress          │   │  - Push Notifications    │
│  - Bootstrap 5         │   │  - Biometría             │
└────────────┬───────────┘   └──────────┬───────────────┘
             │                          │
             └──────────┬───────────────┘
                        │
                        ▼
             ┌──────────────────────┐
             │   API REST (Backend) │
             │   - MySQL Database   │
             │   - Azure Hosted     │
             └──────────┬───────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
┌──────────────┐ ┌─────────────┐ ┌────────────┐
│  Azure       │ │   N8N       │ │  Azure     │
│  OpenAI      │ │   Workflows │ │  IoT Hub   │
│  (GPT-4)     │ │             │ │            │
└──────────────┘ └─────────────┘ └────────────┘
```


### Arquitectura del Portal Web

El portal web sigue el patrón **Page Controller** de ASP.NET Web Forms con separación de responsabilidades:

```
┌─────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                    │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  .aspx Pages │  │ Master Pages │  │  User        │  │
│  │  (UI Markup) │  │  (Layout)    │  │  Controls    │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                    CODE-BEHIND LAYER                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  .aspx.vb    │  │  BasePage.vb │  │  Event       │  │
│  │  (Logic)     │  │  (Common)    │  │  Handlers    │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                    SERVICE LAYER                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  ApiService  │  │  AuthHelper  │  │  N8N         │  │
│  │  (API Calls) │  │  (Security)  │  │  Integration │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                    INFRASTRUCTURE LAYER                  │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ HttpClient   │  │  Logger      │  │  Cache       │  │
│  │ Helper       │  │  Helper      │  │  Helper      │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
```

**Componentes Clave:**

- **BasePage.vb**: Clase base para todas las páginas con validación de autenticación y permisos
- **ApiService**: Capa de abstracción para llamadas a la API REST
- **AuthHelper**: Gestión de autenticación y autorización
- **SessionHelper**: Manejo seguro de sesiones
- **Logger**: Registro de eventos y errores
- **CacheHelper**: Optimización de rendimiento con caché

### Arquitectura de Aplicaciones Móviles

Las aplicaciones móviles seguirán el patrón **MVVM (Model-View-ViewModel)** con arquitectura limpia:

```
┌─────────────────────────────────────────────────────────┐
│                         VIEW LAYER                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Pages/      │  │  Components/ │  │  Styles/     │  │
│  │  Screens     │  │  Widgets     │  │  Themes      │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                      VIEWMODEL LAYER                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  ViewModels  │  │  State       │  │  Commands    │  │
│  │              │  │  Management  │  │              │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                       MODEL LAYER                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Domain      │  │  DTOs        │  │  Entities    │  │
│  │  Models      │  │              │  │              │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                      SERVICE LAYER                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  API         │  │  Auth        │  │  Sync        │  │
│  │  Service     │  │  Service     │  │  Service     │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                          │
┌─────────────────────────────────────────────────────────┐
│                   INFRASTRUCTURE LAYER                   │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │  Local DB    │  │  HTTP Client │  │  Push        │  │
│  │  (SQLite)    │  │              │  │  Notifications│ │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
```

**Componentes Clave:**

- **Local Database (SQLite)**: Almacenamiento offline de datos
- **Sync Service**: Sincronización bidireccional con el backend
- **Push Notification Service**: Manejo de notificaciones push
- **Biometric Service**: Integración con APIs nativas de biometría
- **Location Service**: GPS y geolocalización
- **Camera Service**: Captura de fotos y videos


## Componentes e Interfaces

### Portal Web - Componentes Principales

#### 1. Módulo de Autenticación

**Componentes:**
- `Ingreso.aspx`: Página de login con soporte para credenciales y biometría web (WebAuthn)
- `AuthHelper.vb`: Validación de credenciales y gestión de tokens
- `SessionHelper.vb`: Manejo seguro de sesiones
- `SecurityHelper.vb`: Validaciones de seguridad y headers

**Interfaces:**
```vb
Public Interface IAuthService
    Function Authenticate(username As String, password As String) As AuthResult
    Function AuthenticateBiometric(credential As String) As AuthResult
    Function ValidateSession() As Boolean
    Sub Logout()
End Interface

Public Class AuthResult
    Public Property Success As Boolean
    Public Property Token As String
    Public Property User As UserDTO
    Public Property ErrorMessage As String
End Class
```

#### 2. Módulo de Dashboard

**Componentes:**
- `Inicio.aspx`: Dashboard principal con widgets personalizados
- `DashboardService.vb`: Lógica de negocio para métricas
- Widgets DevExpress: ASPxCardView, ASPxChart, ASPxGridView

**Interfaces:**
```vb
Public Interface IDashboardService
    Function GetDashboardData(userId As Integer, roleId As Integer) As DashboardDTO
    Function GetMetrics(entityId As Integer, dateRange As DateRange) As MetricsDTO
End Interface

Public Class DashboardDTO
    Public Property Widgets As List(Of WidgetDTO)
    Public Property Alerts As List(Of AlertDTO)
    Public Property QuickActions As List(Of ActionDTO)
End Class
```

#### 3. Módulo de Entidades

**Componentes:**
- `Entidades.aspx`: CRUD de entidades con grid DevExpress
- `EntidadesService.vb`: Lógica de negocio
- `EntidadDTO.vb`: Modelo de datos

**Interfaces:**
```vb
Public Interface IEntidadService
    Function GetAll() As List(Of EntidadDTO)
    Function GetById(id As Integer) As EntidadDTO
    Function Create(entidad As EntidadDTO) As OperationResult
    Function Update(entidad As EntidadDTO) As OperationResult
    Function Delete(id As Integer) As OperationResult
End Interface
```

#### 4. Módulo de Tickets

**Componentes:**
- `Tickets.aspx`: Gestión de tickets con workflow
- `TicketService.vb`: Lógica de negocio y notificaciones
- `TicketDTO.vb`: Modelo de datos

**Interfaces:**
```vb
Public Interface ITicketService
    Function Create(ticket As TicketDTO) As OperationResult
    Function Update(ticketId As Integer, status As TicketStatus, notes As String) As OperationResult
    Function GetByUser(userId As Integer, filters As TicketFilters) As List(Of TicketDTO)
    Function AssignTechnician(ticketId As Integer, technicianId As Integer) As OperationResult
End Interface

Public Enum TicketStatus
    Nuevo = 1
    Asignado = 2
    EnProceso = 3
    Resuelto = 4
    Cerrado = 5
End Enum
```

#### 5. Módulo de Órdenes de Compra

**Componentes:**
- `OrdenesCompra.aspx`: Gestión de órdenes con validación IA
- `OrdenCompraService.vb`: Lógica de negocio y workflow
- `AIValidationService.vb`: Integración con Azure OpenAI

**Interfaces:**
```vb
Public Interface IOrdenCompraService
    Function Create(orden As OrdenCompraDTO) As OperationResult
    Function ValidateWithAI(orden As OrdenCompraDTO) As AIValidationResult
    Function Approve(ordenId As Integer, approverId As Integer) As OperationResult
    Function GetWorkflowStatus(ordenId As Integer) As WorkflowStatusDTO
End Interface

Public Class AIValidationResult
    Public Property IsValid As Boolean
    Public Property Issues As List(Of ValidationIssue)
    Public Property Suggestions As List(Of String)
    Public Property ComplianceScore As Decimal
End Class
```

#### 6. Módulo de Agricultura IoT

**Componentes:**
- `AgriculturaIoT.aspx`: Dashboard de monitoreo con mapas
- `IoTService.vb`: Integración con Azure IoT Hub
- `RiegoService.vb`: Control de riego automatizado

**Interfaces:**
```vb
Public Interface IIoTService
    Function GetSensorData(sensorId As String) As SensorDataDTO
    Function GetAlerts(parcelaId As Integer) As List(Of IoTAlertDTO)
    Function SendCommand(deviceId As String, command As IoTCommand) As OperationResult
End Interface

Public Interface IRiegoService
    Function CreateSchedule(schedule As RiegoScheduleDTO) As OperationResult
    Function ActivateRiego(parcelaId As Integer, duration As Integer) As OperationResult
    Function GetRiegoHistory(parcelaId As Integer, dateRange As DateRange) As List(Of RiegoHistoryDTO)
End Interface
```

### Aplicaciones Móviles - Componentes Principales

#### 1. Autenticación y Biometría

**Componentes:**
- `LoginPage`: Pantalla de login
- `BiometricService`: Integración con APIs nativas
- `SecureStorageService`: Almacenamiento seguro de tokens

**Interfaces:**
```csharp
public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string username, string password);
    Task<AuthResult> AuthenticateBiometricAsync();
    Task<bool> IsBiometricAvailableAsync();
    Task<bool> EnableBiometricAsync();
}

public interface ISecureStorageService
{
    Task SetAsync(string key, string value);
    Task<string> GetAsync(string key);
    Task RemoveAsync(string key);
}
```

#### 2. Sincronización Offline

**Componentes:**
- `SyncService`: Sincronización bidireccional
- `LocalDatabase`: SQLite para almacenamiento local
- `QueueManager`: Cola de operaciones pendientes

**Interfaces:**
```csharp
public interface ISyncService
{
    Task<SyncResult> SyncAllAsync();
    Task<SyncResult> SyncEntityAsync(string entityType);
    Task QueueOperationAsync(PendingOperation operation);
    Task<List<PendingOperation>> GetPendingOperationsAsync();
}

public class PendingOperation
{
    public int Id { get; set; }
    public string EntityType { get; set; }
    public string Operation { get; set; } // CREATE, UPDATE, DELETE
    public string Data { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RetryCount { get; set; }
}
```

#### 3. Notificaciones Push

**Componentes:**
- `PushNotificationService`: Manejo de notificaciones
- `NotificationHandler`: Procesamiento de notificaciones

**Interfaces:**
```csharp
public interface IPushNotificationService
{
    Task<bool> RegisterDeviceAsync(string deviceToken);
    Task<bool> UnregisterDeviceAsync();
    Task HandleNotificationAsync(NotificationData notification);
}

public class NotificationData
{
    public string Title { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Data { get; set; }
    public NotificationType Type { get; set; }
}
```

#### 4. Gestión de Tickets Móvil

**Componentes:**
- `TicketListPage`: Lista de tickets
- `CreateTicketPage`: Creación con fotos y GPS
- `TicketDetailPage`: Detalle y seguimiento

**Interfaces:**
```csharp
public interface ITicketService
{
    Task<List<TicketDTO>> GetTicketsAsync(TicketFilters filters);
    Task<OperationResult> CreateTicketAsync(TicketDTO ticket, List<byte[]> photos);
    Task<OperationResult> UpdateTicketAsync(int ticketId, TicketStatus status, string notes);
}
```


## Modelos de Datos

### DTOs Principales

#### EntidadDTO
```vb
Public Class EntidadDTO
    Public Property Id As Integer
    Public Property Nombre As String
    Public Property Tipo As TipoEntidad ' Condominio, Agricola, Municipal, Proveedor
    Public Property RFC As String
    Public Property Direccion As String
    Public Property Telefono As String
    Public Property Email As String
    Public Property Logo As String
    Public Property Activo As Boolean
    Public Property FechaCreacion As DateTime
    Public Property SubEntidades As List(Of SubEntidadDTO)
End Class

Public Enum TipoEntidad
    Condominio = 1
    Agricola = 2
    Municipal = 3
    Proveedor = 4
End Enum
```

#### UsuarioDTO
```vb
Public Class UsuarioDTO
    Public Property Id As Integer
    Public Property Nombre As String
    Public Property Email As String
    Public Property Telefono As String
    Public Property EntidadId As Integer
    Public Property RolId As Integer
    Public Property Activo As Boolean
    Public Property BiometriaHabilitada As Boolean
    Public Property UltimoAcceso As DateTime
    Public Property Permisos As List(Of PermisoDTO)
End Class
```

#### TicketDTO
```vb
Public Class TicketDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property Titulo As String
    Public Property Descripcion As String
    Public Property CategoriaId As Integer
    Public Property Prioridad As PrioridadTicket
    Public Property Estado As TicketStatus
    Public Property SolicitanteId As Integer
    Public Property TecnicoAsignadoId As Integer?
    Public Property EntidadId As Integer
    Public Property FechaCreacion As DateTime
    Public Property FechaActualizacion As DateTime
    Public Property FechaCierre As DateTime?
    Public Property Adjuntos As List(Of AdjuntoDTO)
    Public Property Comentarios As List(Of ComentarioDTO)
    Public Property Ubicacion As UbicacionDTO
    Public Property Calificacion As Integer?
End Class

Public Enum PrioridadTicket
    Baja = 1
    Media = 2
    Alta = 3
    Urgente = 4
End Enum
```

#### OrdenCompraDTO
```vb
Public Class OrdenCompraDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property EntidadId As Integer
    Public Property ProveedorId As Integer
    Public Property Descripcion As String
    Public Property MontoTotal As Decimal
    Public Property Estado As EstadoOrdenCompra
    Public Property FechaCreacion As DateTime
    Public Property FechaAprobacion As DateTime?
    Public Property Conceptos As List(Of ConceptoOrdenDTO)
    Public Property Adjuntos As List(Of AdjuntoDTO)
    Public Property WorkflowAprobaciones As List(Of AprobacionDTO)
    Public Property ValidacionIA As AIValidationResult
End Class

Public Enum EstadoOrdenCompra
    Borrador = 1
    PendienteValidacion = 2
    PendienteAprobacion = 3
    Aprobada = 4
    Rechazada = 5
    Cancelada = 6
End Enum
```

#### SensorDataDTO
```vb
Public Class SensorDataDTO
    Public Property SensorId As String
    Public Property ParcelaId As Integer
    Public Property TipoSensor As TipoSensor
    Public Property Valor As Decimal
    Public Property Unidad As String
    Public Property Timestamp As DateTime
    Public Property Ubicacion As UbicacionDTO
End Class

Public Enum TipoSensor
    Humedad = 1
    Temperatura = 2
    pH = 3
    Conductividad = 4
    LuzSolar = 5
End Enum
```

#### RiegoScheduleDTO
```vb
Public Class RiegoScheduleDTO
    Public Property Id As Integer
    Public Property ParcelaId As Integer
    Public Property Nombre As String
    Public Property HoraInicio As TimeSpan
    Public Property Duracion As Integer ' minutos
    Public Property DiasActivo As List(Of DayOfWeek)
    Public Property CondicionesActivacion As RiegoConditionsDTO
    Public Property Activo As Boolean
End Class

Public Class RiegoConditionsDTO
    Public Property HumedadMinima As Decimal?
    Public Property TemperaturaMaxima As Decimal?
    Public Property CancelarSiLluvia As Boolean
End Class
```

### Modelos de Base de Datos Local (Móvil)

Para las aplicaciones móviles, se usará SQLite con los siguientes modelos:

```csharp
public class LocalTicket
{
    [PrimaryKey, AutoIncrement]
    public int LocalId { get; set; }
    
    public int? ServerId { get; set; }
    public string Folio { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public int CategoriaId { get; set; }
    public int Prioridad { get; set; }
    public int Estado { get; set; }
    public int SolicitanteId { get; set; }
    public DateTime FechaCreacion { get; set; }
    public bool IsSynced { get; set; }
    public string JsonData { get; set; } // Datos completos en JSON
}

public class LocalPhoto
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public int LocalTicketId { get; set; }
    public string FilePath { get; set; }
    public DateTime CapturedAt { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsUploaded { get; set; }
}

public class PendingSync
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    
    public string EntityType { get; set; }
    public string Operation { get; set; }
    public string JsonData { get; set; }
    public DateTime CreatedAt { get; set; }
    public int RetryCount { get; set; }
    public string LastError { get; set; }
}
```


## Propiedades de Corrección

*Una propiedad es una característica o comportamiento que debe mantenerse verdadero en todas las ejecuciones válidas de un sistema - esencialmente, una declaración formal sobre lo que el sistema debe hacer. Las propiedades sirven como puente entre las especificaciones legibles por humanos y las garantías de corrección verificables por máquina.*

### Propiedades de Autenticación y Seguridad

**Propiedad 1: Autenticación exitosa crea sesión válida**
*Para cualquier* usuario con credenciales válidas, cuando se autentica mediante la API REST, el sistema debe crear una sesión segura con un token válido que permita acceso a funcionalidades autorizadas.
**Valida: Requerimientos 1.1**

**Propiedad 2: Autenticación biométrica equivale a credenciales**
*Para cualquier* usuario con biometría habilitada en dispositivo compatible, la autenticación biométrica debe producir el mismo resultado (sesión válida) que la autenticación con credenciales.
**Valida: Requerimientos 1.2**

**Propiedad 3: Credenciales inválidas rechazan acceso**
*Para cualquier* conjunto de credenciales inválidas, el sistema debe rechazar la autenticación, mostrar mensaje de error y registrar el intento fallido sin crear sesión.
**Valida: Requerimientos 1.3**

**Propiedad 4: Sesión expirada redirige a login**
*Para cualquier* sesión que exceda el tiempo de inactividad configurado, el sistema debe invalidar la sesión y redirigir al usuario a la página de login con mensaje informativo.
**Valida: Requerimientos 1.4**

**Propiedad 5: Logout limpia datos sensibles**
*Para cualquier* sesión activa, cuando el usuario cierra sesión, el sistema debe invalidar el token y eliminar todos los datos sensibles del almacenamiento local.
**Valida: Requerimientos 1.5**

**Propiedad 6: Operaciones validan permisos**
*Para cualquier* operación solicitada por un usuario con sesión activa, el sistema debe validar que el usuario tiene los permisos necesarios antes de ejecutar la operación.
**Valida: Requerimientos 1.6**

**Propiedad 7: Contraseñas cumplen complejidad**
*Para cualquier* contraseña ingresada, el sistema debe validar que cumple con los requisitos mínimos de complejidad (longitud, caracteres especiales, mayúsculas, números).
**Valida: Requerimientos 21.2**

**Propiedad 8: Intentos fallidos bloquean cuenta**
*Para cualquier* cuenta con múltiples intentos fallidos de login consecutivos (>= umbral configurado), el sistema debe bloquear temporalmente la cuenta y notificar al usuario.
**Valida: Requerimientos 21.3**

### Propiedades de Dashboard y Visualización

**Propiedad 9: Dashboard personalizado por rol**
*Para cualquier* usuario autenticado, el dashboard debe mostrar únicamente widgets y funcionalidades correspondientes a su rol asignado.
**Valida: Requerimientos 2.1**

**Propiedad 10: Dashboard se actualiza sin recarga**
*Para cualquier* cambio en datos relevantes del dashboard, el sistema debe actualizar la visualización automáticamente sin recargar la página completa.
**Valida: Requerimientos 2.5**

### Propiedades de Gestión de Entidades

**Propiedad 11: Entidad válida se persiste**
*Para cualquier* entidad con datos obligatorios completos y válidos, el sistema debe guardarla exitosamente mediante la API REST y asignarle un ID único.
**Valida: Requerimientos 3.1**

**Propiedad 12: Edición de entidad preserva integridad**
*Para cualquier* entidad existente, cuando se edita y guarda, el sistema debe actualizar solo los campos modificados y preservar el resto de datos sin alteración.
**Valida: Requerimientos 3.2**

**Propiedad 13: Eliminación valida dependencias**
*Para cualquier* entidad con dependencias activas (subentidades, usuarios, tickets), el sistema debe rechazar la eliminación y mostrar mensaje explicativo.
**Valida: Requerimientos 3.3**

**Propiedad 14: Lista de entidades soporta paginación**
*Para cualquier* conjunto de entidades mayor al tamaño de página configurado, el sistema debe mostrar controles de paginación funcionales y cargar solo la página solicitada.
**Valida: Requerimientos 3.4**

**Propiedad 15: SubEntidad asociada a padre**
*Para cualquier* subentidad creada, el sistema debe asociarla correctamente a su entidad padre y validar que el padre existe.
**Valida: Requerimientos 3.5**

### Propiedades de Gestión de Tickets

**Propiedad 16: Ticket captura todos los campos requeridos**
*Para cualquier* ticket creado, el sistema debe capturar y persistir descripción, categoría, prioridad y adjuntos opcionales correctamente.
**Valida: Requerimientos 5.1**

**Propiedad 17: Folio de ticket es único**
*Para cualquier* ticket creado, el sistema debe asignar un folio único que no se repita en ningún otro ticket del sistema.
**Valida: Requerimientos 5.2**

**Propiedad 18: Cambio de estado registra auditoría**
*Para cualquier* cambio de estado en un ticket, el sistema debe registrar quién hizo el cambio, cuándo, el estado anterior y el nuevo estado.
**Valida: Requerimientos 5.3**

**Propiedad 19: Filtros de tickets funcionan correctamente**
*Para cualquier* conjunto de filtros aplicados (estado, fecha, categoría), el sistema debe retornar únicamente tickets que cumplan todos los criterios especificados.
**Valida: Requerimientos 5.4**

**Propiedad 20: Ticket cerrado solicita calificación**
*Para cualquier* ticket que cambia a estado "Cerrado", el sistema debe enviar solicitud de calificación al solicitante.
**Valida: Requerimientos 5.5**

### Propiedades de Órdenes de Compra e IA

**Propiedad 21: IA valida cumplimiento normativo**
*Para cualquier* orden de compra enviada a validación, el Agente IA debe revisar cumplimiento normativo y retornar lista de inconsistencias encontradas (vacía si cumple).
**Valida: Requerimientos 6.2**

**Propiedad 22: Historial de orden es completo**
*Para cualquier* orden de compra, el historial debe contener todos los cambios de estado, aprobaciones y modificaciones en orden cronológico.
**Valida: Requerimientos 6.5**

### Propiedades de IoT y Agricultura

**Propiedad 23: Datos de sensor actualizan indicadores**
*Para cualquier* dato recibido de un sensor IoT, el sistema debe actualizar los indicadores correspondientes en tiempo real (< 2 segundos).
**Valida: Requerimientos 9.2**

**Propiedad 24: Umbrales generan alertas**
*Para cualquier* dato de sensor que supere un umbral configurado, el sistema debe generar una alerta automática y notificar a los usuarios responsables.
**Valida: Requerimientos 9.3**

**Propiedad 25: Condiciones de riego activan sistema**
*Para cualquier* programa de riego con condiciones cumplidas, el sistema debe enviar comando de activación al actuador IoT correspondiente.
**Valida: Requerimientos 10.2**

**Propiedad 26: Humedad suficiente cancela riego**
*Para cualquier* riego programado, si la humedad del suelo está por encima del umbral mínimo configurado, el sistema debe cancelar automáticamente el riego.
**Valida: Requerimientos 10.5**

### Propiedades de Aplicación Móvil

**Propiedad 27: Ticket móvil captura ubicación y fotos**
*Para cualquier* ticket creado desde la app móvil, el sistema debe capturar y asociar correctamente la ubicación GPS y las fotos adjuntas.
**Valida: Requerimientos 14.1**

**Propiedad 28: Ticket offline sincroniza correctamente (Round-trip)**
*Para cualquier* ticket creado en modo offline, cuando la app recupera conexión, el ticket debe sincronizarse al servidor y el servidor debe retornar el mismo ticket con ID asignado.
**Valida: Requerimientos 14.5**

**Propiedad 29: Sincronización procesa datos pendientes**
*Para cualquier* conjunto de operaciones pendientes en la cola de sincronización, cuando la app recupera conexión, todas las operaciones deben procesarse en orden y marcarse como sincronizadas.
**Valida: Requerimientos 18.4**

### Propiedades de Internacionalización

**Propiedad 30: Cambio de idioma actualiza interfaz**
*Para cualquier* idioma soportado seleccionado por el usuario, el sistema debe actualizar todos los textos de la interfaz al idioma seleccionado sin recargar la sesión.
**Valida: Requerimientos 26.2**

**Propiedad 31: Textos sin traducción usan fallback**
*Para cualquier* texto que no tenga traducción en el idioma activo, el sistema debe mostrar el texto en el idioma predeterminado (español) sin generar errores.
**Valida: Requerimientos 26.4**

**Propiedad 32: Preferencia de idioma persiste**
*Para cualquier* usuario con idioma configurado, la preferencia debe mantenerse en todas las páginas, módulos y sesiones futuras.
**Valida: Requerimientos 26.5**

### Propiedades de Seguridad y Visibilidad Multinivel

**Propiedad 33: Visibilidad de montos por nivel**
*Para cualquier* usuario de nivel N, cuando consulta un documento, el sistema debe mostrar únicamente el monto capturado por el nivel N-1 (anterior) sin exponer montos de otros niveles.
**Valida: Requerimientos 27.1, 27.2, 27.3**

**Propiedad 34: Acceso restringido a documentos asignados**
*Para cualquier* usuario de nivel N, la lista de documentos debe contener únicamente documentos asignados a su nivel jerárquico o creados por él.
**Valida: Requerimientos 27.4**

**Propiedad 35: Denegación de acceso a documentos no asignados**
*Para cualquier* intento de acceso a un documento no asignado al nivel del usuario, el sistema debe denegar el acceso y registrar el intento en el log de seguridad.
**Valida: Requerimientos 27.5**

**Propiedad 36: Ocultamiento de columnas por nivel**
*Para cualquier* usuario visualizando partidas de un documento, las columnas de montos de niveles sin acceso deben estar ocultas en la interfaz.
**Valida: Requerimientos 27.6**

**Propiedad 37: Administrador ve toda la cadena**
*Para cualquier* administrador de Entidad (nivel 1), el sistema debe mostrar toda la cadena jerárquica completa y todos los montos de todos los niveles.
**Valida: Requerimientos 27.7**


## Seguridad y Visibilidad en Flujo Multinivel

### Contexto y Propósito

El módulo de **Captura de Documentos** del portal web implementa un flujo multinivel para gestión de órdenes de compra, órdenes de trabajo y dictámenes técnicos. Este flujo permite que diferentes niveles jerárquicos (Entidad → SubEntidad → Proveedor → Colaborador) capturen montos de forma secuencial, manteniendo la privacidad de la información entre niveles.

**Objetivo:** Prevenir manipulación de precios y garantizar que cada nivel trabaje con información justa, sin conocer los montos de otros niveles que podrían influenciar sus decisiones.

### Principios de Seguridad por Nivel

El sistema implementa un modelo de seguridad estricto donde cada nivel jerárquico tiene acceso limitado a la información:

**Regla de Oro**: Cada nivel solo ve el monto capturado por el nivel inmediatamente anterior, nunca el historial completo.

**Flujo de Información:**

```
┌─────────────────────────────────────────────────────────────┐
│ Nivel 1: ENTIDAD (Administrador)                            │
│ ✅ Define: monto_entidad (presupuesto base)                  │
│ ✅ Ve: TODO (para trazabilidad y auditoría)                  │
│ 📊 Puede analizar variaciones entre niveles                  │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│ Nivel 2: SUBENTIDAD (Departamento/División)                 │
│ ✅ Ve: monto_entidad (presupuesto asignado)                  │
│ ✅ Define: monto_subentidad (su propuesta ajustada)          │
│ ❌ NO ve: monto_proveedor, monto_real                        │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│ Nivel 3: PROVEEDOR (Externo)                                │
│ ❌ NO ve: monto_entidad (presupuesto original)               │
│ ✅ Ve: monto_subentidad (presupuesto aprobado)               │
│ ✅ Define: monto_proveedor (su cotización)                   │
│ ❌ NO ve: monto_real                                         │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│ Nivel 4: COLABORADOR (Ejecutor)                             │
│ ❌ NO ve: monto_entidad, monto_subentidad                    │
│ ✅ Ve: monto_proveedor (presupuesto asignado)                │
│ ✅ Define: monto_real (costo real ejecutado)                 │
└─────────────────────────────────────────────────────────────┘
```

### Matriz de Visibilidad de Montos

| Nivel Usuario | monto_entidad | monto_subentidad | monto_proveedor | monto_real | Documentos Visibles |
|---------------|---------------|------------------|-----------------|------------|---------------------|
| **1 - Entidad** | ✅ Ver/Editar | ✅ Ver | ✅ Ver | ✅ Ver | Todos de su entidad |
| **2 - SubEntidad** | ✅ Ver | ✅ Ver/Editar | ❌ Oculto | ❌ Oculto | Solo asignados a su subentidad |
| **3 - Proveedor** | ❌ Oculto | ✅ Ver | ✅ Ver/Editar | ❌ Oculto | Solo asignados a él |
| **4 - Colaborador** | ❌ Oculto | ❌ Oculto | ✅ Ver | ✅ Ver/Editar | Solo asignados a él |

### Implementación de Seguridad

#### 1. Capa de Base de Datos

**Tabla de Auditoría de Seguridad:**

```sql
CREATE TABLE op_documento_security_log (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario_id INT NOT NULL,
    documento_id INT NULL,
    accion VARCHAR(100) NOT NULL COMMENT 'acceso_exitoso, acceso_denegado, edicion, etc.',
    nivel_usuario INT NOT NULL COMMENT '1=Entidad, 2=SubEntidad, 3=Proveedor, 4=Colaborador',
    ip_address VARCHAR(45) NULL,
    user_agent TEXT NULL,
    detalles JSON NULL COMMENT 'Información adicional del evento',
    fecha_evento DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    INDEX idx_usuario (usuario_id),
    INDEX idx_documento (documento_id),
    INDEX idx_fecha (fecha_evento)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

**Stored Procedure con Filtrado por Nivel:**

Este stored procedure implementa la lógica de seguridad a nivel de base de datos, garantizando que incluso accesos directos a la BD respeten las reglas de visibilidad.

```sql
DELIMITER //

DROP PROCEDURE IF EXISTS sp_obtener_partidas_por_nivel//

CREATE PROCEDURE sp_obtener_partidas_por_nivel(
    IN p_documento_id INT,
    IN p_usuario_id INT,
    IN p_nivel_usuario INT,
    IN p_ip_address VARCHAR(45)
)
BEGIN
    DECLARE v_tiene_acceso BOOLEAN DEFAULT FALSE;
    DECLARE v_documento_existe BOOLEAN DEFAULT FALSE;
    
    -- Verificar que el documento existe
    SELECT COUNT(*) > 0 INTO v_documento_existe
    FROM op_documentos
    WHERE id = p_documento_id;
    
    IF NOT v_documento_existe THEN
        -- Registrar intento de acceso a documento inexistente
        INSERT INTO op_documento_security_log (usuario_id, documento_id, accion, nivel_usuario, ip_address, detalles)
        VALUES (p_usuario_id, p_documento_id, 'acceso_denegado', p_nivel_usuario, p_ip_address, 
                JSON_OBJECT('motivo', 'documento_no_existe'));
        
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Documento no encontrado';
    END IF;
    
    -- Verificar que el usuario tiene acceso al documento según su nivel
    SELECT COUNT(*) > 0 INTO v_tiene_acceso
    FROM op_documentos d
    INNER JOIN conf_usuarios u ON u.id = p_usuario_id
    WHERE d.id = p_documento_id
    AND (
        -- Nivel 1 (Entidad): ve todos los documentos de su entidad
        (p_nivel_usuario = 1 AND d.IdEntidad = u.IdEntidad)
        -- Nivel 2 (SubEntidad): solo documentos de su subentidad
        OR (p_nivel_usuario = 2 AND d.IdSubEntidad = u.IdSubEntidad)
        -- Nivel 3 (Proveedor): solo documentos asignados a él
        OR (p_nivel_usuario = 3 AND d.proveedor_id = u.proveedor_id)
        -- Nivel 4 (Colaborador): solo documentos asignados a él
        OR (p_nivel_usuario = 4 AND d.colaborador_id = p_usuario_id)
        -- Creador siempre tiene acceso
        OR d.usuario_creador_id = p_usuario_id
    );
    
    IF NOT v_tiene_acceso THEN
        -- Registrar intento de acceso no autorizado
        INSERT INTO op_documento_security_log (usuario_id, documento_id, accion, nivel_usuario, ip_address, detalles)
        VALUES (p_usuario_id, p_documento_id, 'acceso_denegado', p_nivel_usuario, p_ip_address,
                JSON_OBJECT('motivo', 'documento_no_asignado', 'nivel_usuario', p_nivel_usuario));
        
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Acceso denegado: documento no asignado a este usuario';
    END IF;
    
    -- Registrar acceso exitoso
    INSERT INTO op_documento_security_log (usuario_id, documento_id, accion, nivel_usuario, ip_address)
    VALUES (p_usuario_id, p_documento_id, 'acceso_exitoso', p_nivel_usuario, p_ip_address);
    
    -- Retornar partidas con columnas filtradas según nivel
    -- NULL se usa para ocultar columnas sin acceso
    SELECT 
        dt.id,
        dt.ClaveConcepto,
        dt.Descripcion,
        dt.Cantidad,
        dt.Unidad,
        
        -- Columnas de montos visibles según nivel
        CASE WHEN p_nivel_usuario <= 2 THEN dt.monto_entidad ELSE NULL END AS monto_entidad,
        CASE WHEN p_nivel_usuario IN (1, 2, 3) THEN dt.monto_subentidad ELSE NULL END AS monto_subentidad,
        CASE WHEN p_nivel_usuario IN (1, 3, 4) THEN dt.monto_proveedor ELSE NULL END AS monto_proveedor,
        CASE WHEN p_nivel_usuario IN (1, 4) THEN dt.monto_real ELSE NULL END AS monto_real,
        
        -- Observaciones visibles según nivel
        CASE WHEN p_nivel_usuario IN (1, 2, 3) THEN dt.observaciones_subentidad ELSE NULL END AS observaciones_subentidad,
        CASE WHEN p_nivel_usuario IN (1, 3, 4) THEN dt.observaciones_proveedor ELSE NULL END AS observaciones_proveedor,
        CASE WHEN p_nivel_usuario IN (1, 4) THEN dt.observaciones_colaborador ELSE NULL END AS observaciones_colaborador,
        
        -- Campos comunes siempre visibles
        dt.fecha_creacion,
        dt.fecha_actualizacion
        
    FROM op_documentos_detalle dt
    WHERE dt.IdDocumento = p_documento_id
    ORDER BY dt.ClaveConcepto;
END//

DELIMITER ;
```

**Función de Verificación de Acceso:**

```sql
DELIMITER //

DROP FUNCTION IF EXISTS fn_verificar_acceso_documento//

CREATE FUNCTION fn_verificar_acceso_documento(
    p_documento_id INT,
    p_usuario_id INT,
    p_nivel_usuario INT
) RETURNS BOOLEAN
DETERMINISTIC
READS SQL DATA
BEGIN
    DECLARE v_tiene_acceso BOOLEAN DEFAULT FALSE;
    
    SELECT COUNT(*) > 0 INTO v_tiene_acceso
    FROM op_documentos d
    INNER JOIN conf_usuarios u ON u.id = p_usuario_id
    WHERE d.id = p_documento_id
    AND (
        (p_nivel_usuario = 1 AND d.IdEntidad = u.IdEntidad)
        OR (p_nivel_usuario = 2 AND d.IdSubEntidad = u.IdSubEntidad)
        OR (p_nivel_usuario = 3 AND d.proveedor_id = u.proveedor_id)
        OR (p_nivel_usuario = 4 AND d.colaborador_id = p_usuario_id)
        OR d.usuario_creador_id = p_usuario_id
    );
    
    RETURN v_tiene_acceso;
END//

DELIMITER ;
```

#### 2. Capa de Servicio en Portal Web (VB.NET)

**Servicio de Seguridad de Documentos:**

Este servicio encapsula la lógica de seguridad y se integra con el módulo existente de Captura de Documentos.

```vb
Imports System.Collections.Generic
Imports System.Web

''' <summary>
''' Servicio de seguridad para el flujo multinivel de documentos.
''' Implementa las reglas de visibilidad y acceso según nivel jerárquico.
''' </summary>
Public Class DocumentoSecurityService
    
    ''' <summary>
    ''' Valida si el usuario tiene acceso al documento según su nivel jerárquico
    ''' </summary>
    ''' <param name="documentoId">ID del documento a validar</param>
    ''' <param name="usuarioId">ID del usuario que solicita acceso</param>
    ''' <returns>True si tiene acceso, False en caso contrario</returns>
    Public Shared Function ValidarAccesoDocumento(documentoId As Integer, usuarioId As Integer) As Boolean
        Try
            Dim usuario = SessionHelper.GetCurrentUser()
            If usuario Is Nothing Then
                Logger.LogWarning("ValidarAccesoDocumento: Usuario no encontrado en sesión")
                Return False
            End If
            
            ' Obtener documento desde la API
            Dim documento = DocumentoService.GetById(documentoId)
            If documento Is Nothing Then
                Logger.LogWarning($"ValidarAccesoDocumento: Documento {documentoId} no encontrado")
                Return False
            End If
            
            ' Validar acceso según nivel jerárquico
            Dim tieneAcceso As Boolean = False
            
            Select Case usuario.NivelJerarquico
                Case 1 ' Entidad (Administrador) - ve todos los documentos de su entidad
                    tieneAcceso = (documento.IdEntidad = usuario.IdEntidad)
                    
                Case 2 ' SubEntidad - solo documentos asignados a su subentidad
                    tieneAcceso = (documento.IdSubEntidad = usuario.IdSubEntidad)
                    
                Case 3 ' Proveedor - solo documentos asignados a él
                    tieneAcceso = (documento.ProveedorId = usuario.ProveedorId)
                    
                Case 4 ' Colaborador - solo documentos asignados a él
                    tieneAcceso = (documento.ColaboradorId = usuario.Id)
                    
                Case Else
                    Logger.LogWarning($"ValidarAccesoDocumento: Nivel jerárquico inválido {usuario.NivelJerarquico}")
                    tieneAcceso = False
            End Select
            
            ' El creador siempre tiene acceso
            If documento.UsuarioCreadorId = usuario.Id Then
                tieneAcceso = True
            End If
            
            ' Registrar resultado de validación
            If Not tieneAcceso Then
                SecurityLogger.LogUnauthorizedAccess(usuario.Id, documentoId, "validar_acceso")
            End If
            
            Return tieneAcceso
            
        Catch ex As Exception
            Logger.LogError("Error en ValidarAccesoDocumento", ex)
            Return False
        End Try
    End Function
    
    ''' <summary>
    ''' Obtiene las columnas de montos visibles para el nivel del usuario
    ''' </summary>
    ''' <param name="nivelUsuario">Nivel jerárquico del usuario (1-4)</param>
    ''' <returns>Lista de nombres de columnas visibles</returns>
    Public Shared Function GetColumnasVisibles(nivelUsuario As Integer) As List(Of String)
        Dim columnas As New List(Of String)
        
        Select Case nivelUsuario
            Case 1 ' Entidad - ve todas las columnas para auditoría
                columnas.AddRange({"monto_entidad", "monto_subentidad", "monto_proveedor", "monto_real"})
                
            Case 2 ' SubEntidad - ve presupuesto base y su propuesta
                columnas.AddRange({"monto_entidad", "monto_subentidad"})
                
            Case 3 ' Proveedor - ve presupuesto aprobado y su cotización
                columnas.AddRange({"monto_subentidad", "monto_proveedor"})
                
            Case 4 ' Colaborador - ve presupuesto asignado y costo real
                columnas.AddRange({"monto_proveedor", "monto_real"})
                
            Case Else
                Logger.LogWarning($"GetColumnasVisibles: Nivel jerárquico inválido {nivelUsuario}")
        End Select
        
        Return columnas
    End Function
    
    ''' <summary>
    ''' Filtra partidas según el nivel del usuario, ocultando montos no autorizados
    ''' </summary>
    ''' <param name="partidas">Lista de partidas a filtrar</param>
    ''' <param name="nivelUsuario">Nivel jerárquico del usuario</param>
    ''' <returns>Lista de partidas filtradas</returns>
    ''' <remarks>
    ''' Este método se usa como capa adicional de seguridad en el cliente.
    ''' La seguridad principal se implementa en el stored procedure.
    ''' </remarks>
    Public Shared Function FiltrarPartidas(partidas As List(Of DocumentoDetalleDTO), nivelUsuario As Integer) As List(Of DocumentoDetalleDTO)
        If partidas Is Nothing OrElse partidas.Count = 0 Then
            Return partidas
        End If
        
        For Each partida In partidas
            ' Ocultar montos según nivel jerárquico
            Select Case nivelUsuario
                Case 2 ' SubEntidad - ocultar montos de proveedor y colaborador
                    partida.MontoProveedor = Nothing
                    partida.MontoReal = Nothing
                    partida.ObservacionesProveedor = Nothing
                    partida.ObservacionesColaborador = Nothing
                    
                Case 3 ' Proveedor - ocultar monto de entidad y colaborador
                    partida.MontoEntidad = Nothing
                    partida.MontoReal = Nothing
                    partida.ObservacionesColaborador = Nothing
                    
                Case 4 ' Colaborador - ocultar montos de entidad y subentidad
                    partida.MontoEntidad = Nothing
                    partida.MontoSubEntidad = Nothing
                    partida.ObservacionesSubEntidad = Nothing
                    
                Case 1 ' Entidad - no ocultar nada
                    ' No hacer nada, ve todo
                    
            End Select
        Next
        
        Return partidas
    End Function
    
    ''' <summary>
    ''' Determina si el usuario puede editar un campo específico según su nivel
    ''' </summary>
    ''' <param name="nivelUsuario">Nivel jerárquico del usuario</param>
    ''' <param name="nombreCampo">Nombre del campo a validar</param>
    ''' <returns>True si puede editar, False en caso contrario</returns>
    Public Shared Function PuedeEditarCampo(nivelUsuario As Integer, nombreCampo As String) As Boolean
        Select Case nivelUsuario
            Case 1 ' Entidad puede editar monto_entidad
                Return nombreCampo = "monto_entidad"
            Case 2 ' SubEntidad puede editar monto_subentidad
                Return nombreCampo = "monto_subentidad"
            Case 3 ' Proveedor puede editar monto_proveedor
                Return nombreCampo = "monto_proveedor"
            Case 4 ' Colaborador puede editar monto_real
                Return nombreCampo = "monto_real"
            Case Else
                Return False
        End Select
    End Function
    
End Class
```

**Logger de Seguridad:**

```vb
Imports System.Net

''' <summary>
''' Servicio de logging para eventos de seguridad.
''' Registra intentos de acceso no autorizado y genera alertas.
''' </summary>
Public Class SecurityLogger
    
    Private Const MAX_INTENTOS_PERMITIDOS As Integer = 3
    Private Const VENTANA_TIEMPO_MINUTOS As Integer = 5
    
    ''' <summary>
    ''' Registra un intento de acceso no autorizado
    ''' </summary>
    Public Shared Sub LogUnauthorizedAccess(usuarioId As Integer, documentoId As Integer, accion As String)
        Try
            Dim logEntry As New SecurityLogEntry With {
                .UsuarioId = usuarioId,
                .DocumentoId = documentoId,
                .Accion = $"acceso_denegado_{accion}",
                .Timestamp = DateTime.Now,
                .IpAddress = GetClientIpAddress(),
                .UserAgent = GetUserAgent()
            }
            
            ' Registrar en base de datos
            SecurityLogRepository.Insert(logEntry)
            
            ' Registrar en Application Insights para monitoreo
            ApplicationInsightsHelper.TrackSecurityEvent("UnauthorizedAccess", New Dictionary(Of String, String) From {
                {"usuario_id", usuarioId.ToString()},
                {"documento_id", documentoId.ToString()},
                {"accion", accion},
                {"ip_address", logEntry.IpAddress}
            })
            
            ' Verificar si hay múltiples intentos recientes
            Dim intentosRecientes = SecurityLogRepository.GetRecentAttempts(
                usuarioId, 
                TimeSpan.FromMinutes(VENTANA_TIEMPO_MINUTOS)
            )
            
            If intentosRecientes.Count >= MAX_INTENTOS_PERMITIDOS Then
                ' Alertar a administradores
                Dim mensaje = $"ALERTA DE SEGURIDAD: Usuario {usuarioId} tiene {intentosRecientes.Count} intentos de acceso no autorizado en los últimos {VENTANA_TIEMPO_MINUTOS} minutos"
                AlertService.NotifyAdmins(mensaje, AlertPriority.High)
                
                ' Registrar alerta en Application Insights
                ApplicationInsightsHelper.TrackEvent("SecurityAlert", New Dictionary(Of String, String) From {
                    {"usuario_id", usuarioId.ToString()},
                    {"intentos", intentosRecientes.Count.ToString()},
                    {"ventana_minutos", VENTANA_TIEMPO_MINUTOS.ToString()}
                })
            End If
            
        Catch ex As Exception
            Logger.LogError("Error en LogUnauthorizedAccess", ex)
        End Try
    End Sub
    
    ''' <summary>
    ''' Registra un acceso exitoso para auditoría
    ''' </summary>
    Public Shared Sub LogSuccessfulAccess(usuarioId As Integer, documentoId As Integer, accion As String)
        Try
            Dim logEntry As New SecurityLogEntry With {
                .UsuarioId = usuarioId,
                .DocumentoId = documentoId,
                .Accion = $"acceso_exitoso_{accion}",
                .Timestamp = DateTime.Now,
                .IpAddress = GetClientIpAddress(),
                .UserAgent = GetUserAgent()
            }
            
            SecurityLogRepository.Insert(logEntry)
            
        Catch ex As Exception
            Logger.LogError("Error en LogSuccessfulAccess", ex)
        End Try
    End Sub
    
    Private Shared Function GetClientIpAddress() As String
        Try
            Dim context = HttpContext.Current
            If context IsNot Nothing Then
                ' Intentar obtener IP real detrás de proxy
                Dim ipAddress = context.Request.ServerVariables("HTTP_X_FORWARDED_FOR")
                If String.IsNullOrEmpty(ipAddress) Then
                    ipAddress = context.Request.ServerVariables("REMOTE_ADDR")
                End If
                Return ipAddress
            End If
        Catch ex As Exception
            Logger.LogError("Error obteniendo IP del cliente", ex)
        End Try
        Return "unknown"
    End Function
    
    Private Shared Function GetUserAgent() As String
        Try
            Dim context = HttpContext.Current
            If context IsNot Nothing Then
                Return context.Request.UserAgent
            End If
        Catch ex As Exception
            Logger.LogError("Error obteniendo User Agent", ex)
        End Try
        Return "unknown"
    End Function
    
End Class
```

#### 3. Integración con Módulo de Captura de Documentos

**Actualización de CapturaDocumentos.aspx.vb:**

El módulo existente de Captura de Documentos se actualiza para integrar la seguridad multinivel.

```vb
Partial Class Views_Operacion_CapturaDocumentos
    Inherits BasePage
    
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim documentoId As Integer = Request.QueryString("id")
            If documentoId > 0 Then
                CargarDocumento(documentoId)
            End If
        End If
    End Sub
    
    ''' <summary>
    ''' Carga el documento y sus partidas aplicando seguridad multinivel
    ''' </summary>
    Protected Sub CargarDocumento(documentoId As Integer)
        Try
            Dim usuario = SessionHelper.GetCurrentUser()
            
            ' Validar acceso al documento
            If Not DocumentoSecurityService.ValidarAccesoDocumento(documentoId, usuario.Id) Then
                SecurityLogger.LogUnauthorizedAccess(usuario.Id, documentoId, "cargar_documento")
                Response.Redirect($"~/Views/Error/Error403.aspx?msg=acceso_denegado")
                Return
            End If
            
            ' Cargar documento desde API
            Dim documento = DocumentoService.GetById(documentoId)
            If documento Is Nothing Then
                Response.Redirect("~/Views/Error/Error404.aspx")
                Return
            End If
            
            ' Cargar partidas usando stored procedure con seguridad
            CargarPartidas(documentoId)
            
            ' Registrar acceso exitoso
            SecurityLogger.LogSuccessfulAccess(usuario.Id, documentoId, "cargar_documento")
            
        Catch ex As Exception
            Logger.LogError($"Error cargando documento {documentoId}", ex)
            Response.Redirect("~/Views/Error/Error500.aspx")
        End Try
    End Sub
    
    ''' <summary>
    ''' Carga las partidas del documento con filtrado por nivel
    ''' </summary>
    Protected Sub CargarPartidas(documentoId As Integer)
        Try
            Dim usuario = SessionHelper.GetCurrentUser()
            
            ' Obtener partidas usando stored procedure que aplica seguridad
            Dim partidas = DocumentoService.GetPartidasPorNivel(
                documentoId, 
                usuario.Id, 
                usuario.NivelJerarquico,
                GetClientIpAddress()
            )
            
            ' Configurar grid según nivel del usuario
            ConfigurarGridPartidas()
            
            ' Bind data
            gridPartidas.DataSource = partidas
            gridPartidas.DataBind()
            
        Catch ex As UnauthorizedAccessException
            ' El stored procedure lanzó error de acceso denegado
            SecurityLogger.LogUnauthorizedAccess(usuario.Id, documentoId, "cargar_partidas")
            ShowError("No tiene permisos para ver este documento")
        Catch ex As Exception
            Logger.LogError($"Error cargando partidas del documento {documentoId}", ex)
            ShowError("Error cargando partidas")
        End Try
    End Sub
    
    ''' <summary>
    ''' Configura la visibilidad y permisos de edición del grid según nivel del usuario
    ''' </summary>
    Protected Sub ConfigurarGridPartidas()
        Dim usuario = SessionHelper.GetCurrentUser()
        Dim columnasVisibles = DocumentoSecurityService.GetColumnasVisibles(usuario.NivelJerarquico)
        
        ' Configurar visibilidad de columnas de montos
        If gridPartidas.Columns("monto_entidad") IsNot Nothing Then
            gridPartidas.Columns("monto_entidad").Visible = columnasVisibles.Contains("monto_entidad")
        End If
        
        If gridPartidas.Columns("monto_subentidad") IsNot Nothing Then
            gridPartidas.Columns("monto_subentidad").Visible = columnasVisibles.Contains("monto_subentidad")
        End If
        
        If gridPartidas.Columns("monto_proveedor") IsNot Nothing Then
            gridPartidas.Columns("monto_proveedor").Visible = columnasVisibles.Contains("monto_proveedor")
        End If
        
        If gridPartidas.Columns("monto_real") IsNot Nothing Then
            gridPartidas.Columns("monto_real").Visible = columnasVisibles.Contains("monto_real")
        End If
        
        ' Configurar permisos de edición - solo el campo correspondiente al nivel
        ' Todos los campos empiezan como ReadOnly
        If gridPartidas.Columns("monto_entidad") IsNot Nothing Then
            gridPartidas.Columns("monto_entidad").ReadOnly = True
        End If
        If gridPartidas.Columns("monto_subentidad") IsNot Nothing Then
            gridPartidas.Columns("monto_subentidad").ReadOnly = True
        End If
        If gridPartidas.Columns("monto_proveedor") IsNot Nothing Then
            gridPartidas.Columns("monto_proveedor").ReadOnly = True
        End If
        If gridPartidas.Columns("monto_real") IsNot Nothing Then
            gridPartidas.Columns("monto_real").ReadOnly = True
        End If
        
        ' Habilitar edición solo para el campo correspondiente al nivel
        Select Case usuario.NivelJerarquico
            Case 1 ' Entidad puede editar monto_entidad
                If gridPartidas.Columns("monto_entidad") IsNot Nothing Then
                    gridPartidas.Columns("monto_entidad").ReadOnly = False
                End If
                
            Case 2 ' SubEntidad puede editar monto_subentidad
                If gridPartidas.Columns("monto_subentidad") IsNot Nothing Then
                    gridPartidas.Columns("monto_subentidad").ReadOnly = False
                End If
                
            Case 3 ' Proveedor puede editar monto_proveedor
                If gridPartidas.Columns("monto_proveedor") IsNot Nothing Then
                    gridPartidas.Columns("monto_proveedor").ReadOnly = False
                End If
                
            Case 4 ' Colaborador puede editar monto_real
                If gridPartidas.Columns("monto_real") IsNot Nothing Then
                    gridPartidas.Columns("monto_real").ReadOnly = False
                End If
        End Select
    End Sub
    
    ''' <summary>
    ''' Maneja la actualización de partidas con validación de seguridad
    ''' </summary>
    Protected Sub gridPartidas_RowUpdating(sender As Object, e As ASPxDataUpdatingEventArgs)
        Try
            Dim usuario = SessionHelper.GetCurrentUser()
            Dim partidaId As Integer = CInt(e.Keys("id"))
            
            ' Validar que el usuario solo edite el campo permitido para su nivel
            For Each key As String In e.NewValues.Keys
                If key.StartsWith("monto_") Then
                    If Not DocumentoSecurityService.PuedeEditarCampo(usuario.NivelJerarquico, key) Then
                        SecurityLogger.LogUnauthorizedAccess(usuario.Id, 0, $"editar_{key}")
                        e.Cancel = True
                        ShowError("No tiene permisos para editar este campo")
                        Return
                    End If
                End If
            Next
            
            ' Proceder con la actualización
            DocumentoService.UpdatePartida(partidaId, e.NewValues)
            
            ' Registrar edición exitosa
            SecurityLogger.LogSuccessfulAccess(usuario.Id, 0, "editar_partida")
            
        Catch ex As Exception
            Logger.LogError("Error actualizando partida", ex)
            e.Cancel = True
            ShowError("Error actualizando partida")
        End Try
    End Sub
    
    Private Function GetClientIpAddress() As String
        Try
            Dim ipAddress = Request.ServerVariables("HTTP_X_FORWARDED_FOR")
            If String.IsNullOrEmpty(ipAddress) Then
                ipAddress = Request.ServerVariables("REMOTE_ADDR")
            End If
            Return ipAddress
        Catch
            Return "unknown"
        End Try
    End Function
    
    Private Sub ShowError(message As String)
        ' Implementar según el mecanismo de notificaciones del proyecto
        ' Puede ser un ASPxPopupControl, ASPxLabel, o redirección a página de error
    End Sub
    
End Class
```

### Logging de Intentos de Acceso No Autorizado

```vb
Public Class SecurityLogger
    
    Public Shared Sub LogUnauthorizedAccess(usuarioId As Integer, documentoId As Integer, accion As String)
        Dim logEntry As New SecurityLogEntry With {
            .UsuarioId = usuarioId,
            .DocumentoId = documentoId,
            .Accion = accion,
            .Timestamp = DateTime.Now,
            .IpAddress = HttpContext.Current.Request.UserHostAddress,
            .UserAgent = HttpContext.Current.Request.UserAgent
        }
        
        ' Registrar en base de datos
        SecurityLogRepository.Insert(logEntry)
        
        ' Registrar en Application Insights
        ApplicationInsightsHelper.TrackSecurityEvent("UnauthorizedAccess", logEntry)
        
        ' Si hay múltiples intentos, alertar a administradores
        Dim intentosRecientes = SecurityLogRepository.GetRecentAttempts(usuarioId, TimeSpan.FromMinutes(5))
        If intentosRecientes.Count >= 3 Then
            AlertService.NotifyAdmins($"Usuario {usuarioId} tiene {intentosRecientes.Count} intentos de acceso no autorizado")
        End If
    End Sub
    
End Class
```

#### 4. Aplicación Móvil - Seguridad Multinivel

**Servicio de Seguridad para Apps Móviles:**

La aplicación móvil implementa las mismas reglas de seguridad que el portal web, garantizando consistencia en todo el ecosistema.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JelaApp.Services.Security
{
    /// <summary>
    /// Servicio de seguridad para documentos multinivel en aplicación móvil.
    /// Implementa las mismas reglas que el portal web.
    /// </summary>
    public class DocumentSecurityService
    {
        private readonly IAuthService _authService;
        private readonly IDocumentService _documentService;
        private readonly ISecurityLogger _securityLogger;
        
        public DocumentSecurityService(
            IAuthService authService,
            IDocumentService documentService,
            ISecurityLogger securityLogger)
        {
            _authService = authService;
            _documentService = documentService;
            _securityLogger = securityLogger;
        }
        
        /// <summary>
        /// Valida si el usuario actual tiene acceso al documento
        /// </summary>
        public async Task<bool> ValidateAccessAsync(int documentId, int userId)
        {
            try
            {
                var user = await _authService.GetCurrentUserAsync();
                if (user == null)
                {
                    await _securityLogger.LogWarningAsync("ValidateAccess: Usuario no encontrado");
                    return false;
                }
                
                var document = await _documentService.GetByIdAsync(documentId);
                if (document == null)
                {
                    await _securityLogger.LogWarningAsync($"ValidateAccess: Documento {documentId} no encontrado");
                    return false;
                }
                
                // Validar según nivel jerárquico
                bool hasAccess = user.NivelJerarquico switch
                {
                    1 => document.IdEntidad == user.IdEntidad, // Entidad ve todos de su entidad
                    2 => document.IdSubEntidad == user.IdSubEntidad, // SubEntidad solo los suyos
                    3 => document.ProveedorId == user.ProveedorId, // Proveedor solo los asignados a él
                    4 => document.ColaboradorId == user.Id, // Colaborador solo los asignados a él
                    _ => false
                };
                
                // El creador siempre tiene acceso
                if (document.UsuarioCreadorId == user.Id)
                {
                    hasAccess = true;
                }
                
                // Registrar resultado
                if (!hasAccess)
                {
                    await _securityLogger.LogUnauthorizedAccessAsync(
                        user.Id, 
                        documentId, 
                        "validate_access_mobile"
                    );
                }
                
                return hasAccess;
            }
            catch (Exception ex)
            {
                await _securityLogger.LogErrorAsync("Error en ValidateAccessAsync", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Obtiene las columnas visibles según el nivel del usuario
        /// </summary>
        public List<string> GetVisibleColumns(int nivelUsuario)
        {
            return nivelUsuario switch
            {
                1 => new List<string> { "monto_entidad", "monto_subentidad", "monto_proveedor", "monto_real" },
                2 => new List<string> { "monto_entidad", "monto_subentidad" },
                3 => new List<string> { "monto_subentidad", "monto_proveedor" },
                4 => new List<string> { "monto_proveedor", "monto_real" },
                _ => new List<string>()
            };
        }
        
        /// <summary>
        /// Filtra partidas según el nivel del usuario, ocultando montos no autorizados
        /// </summary>
        public List<DocumentoDetalleDTO> FilterPartidas(List<DocumentoDetalleDTO> partidas, int nivelUsuario)
        {
            if (partidas == null || !partidas.Any())
                return partidas;
            
            foreach (var partida in partidas)
            {
                switch (nivelUsuario)
                {
                    case 2: // SubEntidad - ocultar proveedor y colaborador
                        partida.MontoProveedor = null;
                        partida.MontoReal = null;
                        partida.ObservacionesProveedor = null;
                        partida.ObservacionesColaborador = null;
                        break;
                        
                    case 3: // Proveedor - ocultar entidad y colaborador
                        partida.MontoEntidad = null;
                        partida.MontoReal = null;
                        partida.ObservacionesColaborador = null;
                        break;
                        
                    case 4: // Colaborador - ocultar entidad y subentidad
                        partida.MontoEntidad = null;
                        partida.MontoSubEntidad = null;
                        partida.ObservacionesSubEntidad = null;
                        break;
                        
                    case 1: // Entidad - no ocultar nada
                        // No hacer nada, ve todo
                        break;
                }
            }
            
            return partidas;
        }
        
        /// <summary>
        /// Determina si el usuario puede editar un campo específico
        /// </summary>
        public bool CanEditField(int nivelUsuario, string fieldName)
        {
            return nivelUsuario switch
            {
                1 => fieldName == "monto_entidad",
                2 => fieldName == "monto_subentidad",
                3 => fieldName == "monto_proveedor",
                4 => fieldName == "monto_real",
                _ => false
            };
        }
    }
    
    /// <summary>
    /// Logger de seguridad para aplicación móvil
    /// </summary>
    public class SecurityLogger : ISecurityLogger
    {
        private readonly IApiService _apiService;
        private readonly ILocalDatabase _localDb;
        private readonly ITelemetryService _telemetry;
        
        private const int MAX_INTENTOS_PERMITIDOS = 3;
        private const int VENTANA_TIEMPO_MINUTOS = 5;
        
        public SecurityLogger(
            IApiService apiService,
            ILocalDatabase localDb,
            ITelemetryService telemetry)
        {
            _apiService = apiService;
            _localDb = localDb;
            _telemetry = telemetry;
        }
        
        /// <summary>
        /// Registra un intento de acceso no autorizado
        /// </summary>
        public async Task LogUnauthorizedAccessAsync(int usuarioId, int documentoId, string accion)
        {
            try
            {
                var logEntry = new SecurityLogEntry
                {
                    UsuarioId = usuarioId,
                    DocumentoId = documentoId,
                    Accion = $"acceso_denegado_{accion}",
                    Timestamp = DateTime.Now,
                    Platform = "mobile",
                    DeviceInfo = GetDeviceInfo()
                };
                
                // Intentar enviar al servidor
                if (ConnectivityService.IsConnected)
                {
                    await _apiService.PostAsync("/api/security/log", logEntry);
                }
                else
                {
                    // Guardar localmente para sincronizar después
                    await _localDb.InsertAsync(logEntry);
                }
                
                // Registrar en telemetría
                await _telemetry.TrackEventAsync("UnauthorizedAccess", new Dictionary<string, string>
                {
                    { "usuario_id", usuarioId.ToString() },
                    { "documento_id", documentoId.ToString() },
                    { "accion", accion },
                    { "platform", "mobile" }
                });
                
                // Verificar múltiples intentos
                var intentosRecientes = await GetRecentAttemptsAsync(usuarioId, TimeSpan.FromMinutes(VENTANA_TIEMPO_MINUTOS));
                if (intentosRecientes >= MAX_INTENTOS_PERMITIDOS)
                {
                    await _telemetry.TrackEventAsync("SecurityAlert", new Dictionary<string, string>
                    {
                        { "usuario_id", usuarioId.ToString() },
                        { "intentos", intentosRecientes.ToString() },
                        { "ventana_minutos", VENTANA_TIEMPO_MINUTOS.ToString() }
                    });
                }
            }
            catch (Exception ex)
            {
                await LogErrorAsync("Error en LogUnauthorizedAccessAsync", ex);
            }
        }
        
        /// <summary>
        /// Registra un acceso exitoso para auditoría
        /// </summary>
        public async Task LogSuccessfulAccessAsync(int usuarioId, int documentoId, string accion)
        {
            try
            {
                var logEntry = new SecurityLogEntry
                {
                    UsuarioId = usuarioId,
                    DocumentoId = documentoId,
                    Accion = $"acceso_exitoso_{accion}",
                    Timestamp = DateTime.Now,
                    Platform = "mobile",
                    DeviceInfo = GetDeviceInfo()
                };
                
                if (ConnectivityService.IsConnected)
                {
                    await _apiService.PostAsync("/api/security/log", logEntry);
                }
                else
                {
                    await _localDb.InsertAsync(logEntry);
                }
            }
            catch (Exception ex)
            {
                await LogErrorAsync("Error en LogSuccessfulAccessAsync", ex);
            }
        }
        
        private async Task<int> GetRecentAttemptsAsync(int usuarioId, TimeSpan timeWindow)
        {
            var cutoffTime = DateTime.Now.Subtract(timeWindow);
            return await _localDb.Table<SecurityLogEntry>()
                .Where(x => x.UsuarioId == usuarioId 
                    && x.Accion.StartsWith("acceso_denegado")
                    && x.Timestamp >= cutoffTime)
                .CountAsync();
        }
        
        private string GetDeviceInfo()
        {
            return $"{DeviceInfo.Platform} {DeviceInfo.Version} - {DeviceInfo.Model}";
        }
        
        public async Task LogWarningAsync(string message)
        {
            await _telemetry.TrackEventAsync("SecurityWarning", new Dictionary<string, string>
            {
                { "message", message }
            });
        }
        
        public async Task LogErrorAsync(string message, Exception ex)
        {
            await _telemetry.TrackExceptionAsync(ex, new Dictionary<string, string>
            {
                { "context", message }
            });
        }
    }
}
```

#### 5. Testing de Seguridad Multinivel

**Property-Based Tests para Seguridad:**

Los tests basados en propiedades verifican que las reglas de seguridad se cumplan para todos los casos posibles.

```vb
Imports FsCheck
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass>
Public Class DocumentoSecurityTests
    
    ' **Feature: ecosistema-jelabbc, Property 33: Visibilidad de montos por nivel**
    <TestMethod>
    <Property(MaxTest:=100)>
    Public Sub PropiedadVisibilidadMontosPorNivel()
        ' Para cualquier usuario de nivel N,
        ' cuando consulta un documento,
        ' el sistema debe mostrar únicamente el monto del nivel N-1
        
        Dim usuario = GenerarUsuarioAleatorio()
        Dim documento = GenerarDocumentoAleatorio()
        
        ' Filtrar partidas según nivel del usuario
        Dim partidas = DocumentoSecurityService.FiltrarPartidas(
            documento.Partidas, 
            usuario.NivelJerarquico
        )
        
        ' Verificar que cada partida cumple las reglas de visibilidad
        For Each partida In partidas
            Select Case usuario.NivelJerarquico
                Case 1 ' Entidad - ve todo
                    Assert.IsNotNull(partida.MontoEntidad, "Entidad debe ver monto_entidad")
                    Assert.IsNotNull(partida.MontoSubEntidad, "Entidad debe ver monto_subentidad")
                    Assert.IsNotNull(partida.MontoProveedor, "Entidad debe ver monto_proveedor")
                    Assert.IsNotNull(partida.MontoReal, "Entidad debe ver monto_real")
                    
                Case 2 ' SubEntidad - ve entidad y subentidad
                    Assert.IsNotNull(partida.MontoEntidad, "SubEntidad debe ver monto_entidad")
                    Assert.IsNotNull(partida.MontoSubEntidad, "SubEntidad debe ver monto_subentidad")
                    Assert.IsNull(partida.MontoProveedor, "SubEntidad NO debe ver monto_proveedor")
                    Assert.IsNull(partida.MontoReal, "SubEntidad NO debe ver monto_real")
                    
                Case 3 ' Proveedor - ve subentidad y proveedor
                    Assert.IsNull(partida.MontoEntidad, "Proveedor NO debe ver monto_entidad")
                    Assert.IsNotNull(partida.MontoSubEntidad, "Proveedor debe ver monto_subentidad")
                    Assert.IsNotNull(partida.MontoProveedor, "Proveedor debe ver monto_proveedor")
                    Assert.IsNull(partida.MontoReal, "Proveedor NO debe ver monto_real")
                    
                Case 4 ' Colaborador - ve proveedor y real
                    Assert.IsNull(partida.MontoEntidad, "Colaborador NO debe ver monto_entidad")
                    Assert.IsNull(partida.MontoSubEntidad, "Colaborador NO debe ver monto_subentidad")
                    Assert.IsNotNull(partida.MontoProveedor, "Colaborador debe ver monto_proveedor")
                    Assert.IsNotNull(partida.MontoReal, "Colaborador debe ver monto_real")
            End Select
        Next
    End Sub
    
    ' **Feature: ecosistema-jelabbc, Property 34: Acceso restringido a documentos asignados**
    <TestMethod>
    <Property(MaxTest:=100)>
    Public Sub PropiedadAccesoRestringido()
        ' Para cualquier usuario de nivel N,
        ' la lista de documentos debe contener únicamente
        ' documentos asignados a su nivel jerárquico
        
        Dim usuario = GenerarUsuarioAleatorio()
        Dim documentos = GenerarListaDocumentosAleatorios(20)
        
        ' Filtrar documentos a los que el usuario tiene acceso
        Dim documentosAccesibles = documentos.Where(Function(d) 
            DocumentoSecurityService.ValidarAccesoDocumento(d.Id, usuario.Id)
        ).ToList()
        
        ' Verificar que todos los documentos accesibles cumplen las reglas
        For Each doc In documentosAccesibles
            Dim tieneAcceso As Boolean = False
            
            Select Case usuario.NivelJerarquico
                Case 1 ' Entidad
                    tieneAcceso = (doc.IdEntidad = usuario.IdEntidad)
                Case 2 ' SubEntidad
                    tieneAcceso = (doc.IdSubEntidad = usuario.IdSubEntidad)
                Case 3 ' Proveedor
                    tieneAcceso = (doc.ProveedorId = usuario.ProveedorId)
                Case 4 ' Colaborador
                    tieneAcceso = (doc.ColaboradorId = usuario.Id)
            End Select
            
            ' El creador siempre tiene acceso
            If doc.UsuarioCreadorId = usuario.Id Then
                tieneAcceso = True
            End If
            
            Assert.IsTrue(tieneAcceso, $"Usuario nivel {usuario.NivelJerarquico} no debería tener acceso a documento {doc.Id}")
        Next
    End Sub
    
    ' **Feature: ecosistema-jelabbc, Property 35: Denegación de acceso a documentos no asignados**
    <TestMethod>
    <Property(MaxTest:=100)>
    Public Sub PropiedadDenegacionAcceso()
        ' Para cualquier intento de acceso a un documento no asignado,
        ' el sistema debe denegar el acceso y registrar el intento
        
        Dim usuario = GenerarUsuarioAleatorio()
        Dim documentoNoAsignado = GenerarDocumentoNoAsignadoA(usuario)
        
        ' Intentar acceder al documento
        Dim tieneAcceso = DocumentoSecurityService.ValidarAccesoDocumento(
            documentoNoAsignado.Id, 
            usuario.Id
        )
        
        ' Verificar que el acceso fue denegado
        Assert.IsFalse(tieneAcceso, "El acceso debería ser denegado para documento no asignado")
        
        ' Verificar que se registró el intento en el log
        Dim logs = SecurityLogRepository.GetRecentAttempts(usuario.Id, TimeSpan.FromMinutes(1))
        Assert.IsTrue(logs.Count > 0, "Debe existir un log del intento de acceso denegado")
        
        Dim ultimoLog = logs.OrderByDescending(Function(l) l.Timestamp).First()
        Assert.AreEqual("acceso_denegado", ultimoLog.Accion.Substring(0, 15), "El log debe indicar acceso denegado")
    End Sub
    
    ' **Feature: ecosistema-jelabbc, Property 36: Ocultamiento de columnas por nivel**
    <TestMethod>
    <Property(MaxTest:=100)>
    Public Sub PropiedadOcultamientoColumnas()
        ' Para cualquier usuario visualizando partidas de un documento,
        ' las columnas de montos de niveles sin acceso deben estar ocultas
        
        Dim usuario = GenerarUsuarioAleatorio()
        Dim columnasVisibles = DocumentoSecurityService.GetColumnasVisibles(usuario.NivelJerarquico)
        
        ' Verificar que las columnas visibles son las correctas según nivel
        Select Case usuario.NivelJerarquico
            Case 1 ' Entidad - ve todas
                Assert.IsTrue(columnasVisibles.Contains("monto_entidad"))
                Assert.IsTrue(columnasVisibles.Contains("monto_subentidad"))
                Assert.IsTrue(columnasVisibles.Contains("monto_proveedor"))
                Assert.IsTrue(columnasVisibles.Contains("monto_real"))
                
            Case 2 ' SubEntidad
                Assert.IsTrue(columnasVisibles.Contains("monto_entidad"))
                Assert.IsTrue(columnasVisibles.Contains("monto_subentidad"))
                Assert.IsFalse(columnasVisibles.Contains("monto_proveedor"))
                Assert.IsFalse(columnasVisibles.Contains("monto_real"))
                
            Case 3 ' Proveedor
                Assert.IsFalse(columnasVisibles.Contains("monto_entidad"))
                Assert.IsTrue(columnasVisibles.Contains("monto_subentidad"))
                Assert.IsTrue(columnasVisibles.Contains("monto_proveedor"))
                Assert.IsFalse(columnasVisibles.Contains("monto_real"))
                
            Case 4 ' Colaborador
                Assert.IsFalse(columnasVisibles.Contains("monto_entidad"))
                Assert.IsFalse(columnasVisibles.Contains("monto_subentidad"))
                Assert.IsTrue(columnasVisibles.Contains("monto_proveedor"))
                Assert.IsTrue(columnasVisibles.Contains("monto_real"))
        End Select
    End Sub
    
    ' **Feature: ecosistema-jelabbc, Property 37: Administrador ve toda la cadena**
    <TestMethod>
    <Property(MaxTest:=100)>
    Public Sub PropiedadAdministradorVeTodo()
        ' Para cualquier administrador de Entidad (nivel 1),
        ' el sistema debe mostrar toda la cadena jerárquica completa
        ' y todos los montos de todos los niveles
        
        Dim adminEntidad = GenerarUsuarioNivel1()
        Dim documento = GenerarDocumentoAleatorio()
        documento.IdEntidad = adminEntidad.IdEntidad ' Asegurar que pertenece a su entidad
        
        ' Verificar acceso
        Dim tieneAcceso = DocumentoSecurityService.ValidarAccesoDocumento(documento.Id, adminEntidad.Id)
        Assert.IsTrue(tieneAcceso, "Administrador debe tener acceso a documentos de su entidad")
        
        ' Verificar columnas visibles
        Dim columnasVisibles = DocumentoSecurityService.GetColumnasVisibles(adminEntidad.NivelJerarquico)
        Assert.AreEqual(4, columnasVisibles.Count, "Administrador debe ver las 4 columnas de montos")
        
        ' Verificar que las partidas no están filtradas
        Dim partidas = DocumentoSecurityService.FiltrarPartidas(documento.Partidas, adminEntidad.NivelJerarquico)
        For Each partida In partidas
            Assert.IsNotNull(partida.MontoEntidad, "Admin debe ver monto_entidad")
            Assert.IsNotNull(partida.MontoSubEntidad, "Admin debe ver monto_subentidad")
            Assert.IsNotNull(partida.MontoProveedor, "Admin debe ver monto_proveedor")
            Assert.IsNotNull(partida.MontoReal, "Admin debe ver monto_real")
        Next
    End Sub
    
    ' Generadores de datos aleatorios para property tests
    Private Function GenerarUsuarioAleatorio() As UsuarioDTO
        Dim random As New Random()
        Return New UsuarioDTO With {
            .Id = random.Next(1, 1000),
            .NivelJerarquico = random.Next(1, 5), ' 1-4
            .IdEntidad = random.Next(1, 10),
            .IdSubEntidad = random.Next(1, 20),
            .ProveedorId = random.Next(1, 50)
        }
    End Function
    
    Private Function GenerarUsuarioNivel1() As UsuarioDTO
        Dim random As New Random()
        Return New UsuarioDTO With {
            .Id = random.Next(1, 1000),
            .NivelJerarquico = 1,
            .IdEntidad = random.Next(1, 10)
        }
    End Function
    
    Private Function GenerarDocumentoAleatorio() As DocumentoDTO
        Dim random As New Random()
        Dim doc As New DocumentoDTO With {
            .Id = random.Next(1, 10000),
            .IdEntidad = random.Next(1, 10),
            .IdSubEntidad = random.Next(1, 20),
            .ProveedorId = random.Next(1, 50),
            .ColaboradorId = random.Next(1, 100),
            .UsuarioCreadorId = random.Next(1, 1000),
            .Partidas = New List(Of DocumentoDetalleDTO)()
        }
        
        ' Generar partidas aleatorias
        For i As Integer = 1 To random.Next(3, 10)
            doc.Partidas.Add(New DocumentoDetalleDTO With {
                .Id = i,
                .MontoEntidad = random.Next(1000, 10000),
                .MontoSubEntidad = random.Next(900, 9500),
                .MontoProveedor = random.Next(800, 9000),
                .MontoReal = random.Next(700, 8500)
            })
        Next
        
        Return doc
    End Function
    
    Private Function GenerarListaDocumentosAleatorios(cantidad As Integer) As List(Of DocumentoDTO)
        Dim documentos As New List(Of DocumentoDTO)()
        For i As Integer = 1 To cantidad
            documentos.Add(GenerarDocumentoAleatorio())
        Next
        Return documentos
    End Function
    
    Private Function GenerarDocumentoNoAsignadoA(usuario As UsuarioDTO) As DocumentoDTO
        Dim doc = GenerarDocumentoAleatorio()
        
        ' Asegurar que el documento NO está asignado al usuario
        doc.IdEntidad = usuario.IdEntidad + 1000
        doc.IdSubEntidad = usuario.IdSubEntidad + 1000
        doc.ProveedorId = usuario.ProveedorId + 1000
        doc.ColaboradorId = usuario.Id + 1000
        doc.UsuarioCreadorId = usuario.Id + 1000
        
        Return doc
    End Function
    
End Class
```

**Unit Tests Complementarios:**

```vb
<TestClass>
Public Class DocumentoSecurityUnitTests
    
    <TestMethod>
    Public Sub SubEntidad_SoloVeSuInformacion()
        ' Arrange
        Dim usuario As New UsuarioDTO With {
            .Id = 1,
            .NivelJerarquico = 2,
            .IdSubEntidad = 5
        }
        
        Dim partida As New DocumentoDetalleDTO With {
            .MontoEntidad = 10000,
            .MontoSubEntidad = 9500,
            .MontoProveedor = 9200,
            .MontoReal = 9100
        }
        
        ' Act
        Dim partidas = DocumentoSecurityService.FiltrarPartidas(
            New List(Of DocumentoDetalleDTO) From {partida},
            usuario.NivelJerarquico
        )
        
        ' Assert
        Assert.IsNotNull(partidas(0).MontoEntidad)
        Assert.IsNotNull(partidas(0).MontoSubEntidad)
        Assert.IsNull(partidas(0).MontoProveedor)
        Assert.IsNull(partidas(0).MontoReal)
    End Sub
    
    <TestMethod>
    Public Sub Proveedor_NoVeMontoEntidad()
        ' Arrange
        Dim usuario As New UsuarioDTO With {
            .Id = 2,
            .NivelJerarquico = 3,
            .ProveedorId = 10
        }
        
        Dim partida As New DocumentoDetalleDTO With {
            .MontoEntidad = 10000,
            .MontoSubEntidad = 9500,
            .MontoProveedor = 9200,
            .MontoReal = 9100
        }
        
        ' Act
        Dim partidas = DocumentoSecurityService.FiltrarPartidas(
            New List(Of DocumentoDetalleDTO) From {partida},
            usuario.NivelJerarquico
        )
        
        ' Assert
        Assert.IsNull(partidas(0).MontoEntidad)
        Assert.IsNotNull(partidas(0).MontoSubEntidad)
        Assert.IsNotNull(partidas(0).MontoProveedor)
        Assert.IsNull(partidas(0).MontoReal)
    End Sub
    
    <TestMethod>
    Public Sub Colaborador_NoVeMontosAnteriores()
        ' Arrange
        Dim usuario As New UsuarioDTO With {
            .Id = 3,
            .NivelJerarquico = 4
        }
        
        Dim partida As New DocumentoDetalleDTO With {
            .MontoEntidad = 10000,
            .MontoSubEntidad = 9500,
            .MontoProveedor = 9200,
            .MontoReal = 9100
        }
        
        ' Act
        Dim partidas = DocumentoSecurityService.FiltrarPartidas(
            New List(Of DocumentoDetalleDTO) From {partida},
            usuario.NivelJerarquico
        )
        
        ' Assert
        Assert.IsNull(partidas(0).MontoEntidad)
        Assert.IsNull(partidas(0).MontoSubEntidad)
        Assert.IsNotNull(partidas(0).MontoProveedor)
        Assert.IsNotNull(partidas(0).MontoReal)
    End Sub
    
    <TestMethod>
    Public Sub Administrador_VeTodo()
        ' Arrange
        Dim usuario As New UsuarioDTO With {
            .Id = 4,
            .NivelJerarquico = 1,
            .IdEntidad = 1
        }
        
        Dim partida As New DocumentoDetalleDTO With {
            .MontoEntidad = 10000,
            .MontoSubEntidad = 9500,
            .MontoProveedor = 9200,
            .MontoReal = 9100
        }
        
        ' Act
        Dim partidas = DocumentoSecurityService.FiltrarPartidas(
            New List(Of DocumentoDetalleDTO) From {partida},
            usuario.NivelJerarquico
        )
        
        ' Assert
        Assert.IsNotNull(partidas(0).MontoEntidad)
        Assert.IsNotNull(partidas(0).MontoSubEntidad)
        Assert.IsNotNull(partidas(0).MontoProveedor)
        Assert.IsNotNull(partidas(0).MontoReal)
    End Sub
    
End Class
```

### Integración con Otros Módulos del Sistema

El flujo de seguridad multinivel se integra con los siguientes módulos existentes del Ecosistema JELABBC:

#### Integración con Módulo de Órdenes de Compra (Requerimiento 6)

Las órdenes de compra utilizan el flujo multinivel para:
- **Nivel 1 (Entidad)**: Define presupuesto base y envía a validación de IA
- **Nivel 2 (SubEntidad)**: Ajusta presupuesto según disponibilidad departamental
- **Nivel 3 (Proveedor)**: Cotiza basándose en presupuesto aprobado
- **Nivel 4 (Colaborador)**: Registra costos reales de ejecución

El Agente IA valida cumplimiento normativo en cada nivel sin acceder a montos de otros niveles.

#### Integración con Módulo de Órdenes de Trabajo (Requerimiento 15)

Las órdenes de trabajo para técnicos siguen el mismo flujo:
- **Nivel 1**: Define presupuesto para el trabajo
- **Nivel 2**: Asigna recursos departamentales
- **Nivel 3**: Proveedor externo cotiza materiales
- **Nivel 4**: Técnico registra materiales usados y tiempo real

La app móvil permite a técnicos (nivel 4) ver solo el presupuesto asignado y capturar costos reales.

#### Integración con Módulo de Dictámenes Técnicos (Requerimiento 7)

Los dictámenes técnicos municipales utilizan el flujo para:
- **Nivel 1**: Define criterios de evaluación y presupuesto máximo
- **Nivel 2**: Departamento técnico evalúa propuestas
- **Nivel 3**: Proveedores presentan cotizaciones sin ver presupuesto original
- **Nivel 4**: Inspector valida ejecución contra cotización aprobada

#### Integración con Sistema de Auditoría y Reportes (Requerimiento 23)

Los administradores de nivel 1 pueden generar reportes que muestran:
- Variaciones entre niveles (presupuesto vs cotización vs real)
- Análisis de eficiencia por proveedor
- Detección de anomalías en precios
- Trazabilidad completa del flujo

Los logs de seguridad alimentan reportes de:
- Intentos de acceso no autorizado
- Patrones de uso por nivel
- Auditoría de cambios en montos

#### Integración con Notificaciones N8N (Requerimiento 22)

Los flujos N8N se activan en cada transición de nivel:
- Notificar al siguiente nivel cuando se completa captura
- Alertar a administradores sobre variaciones significativas
- Enviar recordatorios de documentos pendientes
- Notificar intentos de acceso no autorizado

#### Integración con Agente de Voz IA (Requerimiento 11)

El Agente de Voz respeta las mismas reglas de seguridad:
- Usuarios pueden consultar sus documentos asignados por voz
- El agente solo proporciona información del nivel correspondiente
- No revela montos de otros niveles
- Registra consultas en el log de auditoría

### Beneficios de la Integración

1. **Consistencia**: Las mismas reglas de seguridad aplican en portal web, app móvil y agente de voz
2. **Trazabilidad**: Todos los accesos se registran independientemente del canal
3. **Escalabilidad**: Fácil agregar nuevos niveles o tipos de documentos
4. **Auditoría**: Vista completa del flujo para administradores
5. **Prevención de Fraude**: Imposible manipular precios viendo presupuestos originales


## Internacionalización (i18n)

### Estrategia de Localización

El sistema soportará múltiples idiomas usando archivos de recursos (.resx para .NET) y archivos JSON para aplicaciones móviles.

**Idiomas Soportados (Fase 1):**
- Español (es-MX) - Idioma predeterminado
- Inglés (en-US)

**Idiomas Futuros:**
- Francés (fr-FR)
- Portugués (pt-BR)

### Portal Web - Implementación

#### Archivos de Recursos
```
/App_GlobalResources/
  ├── Resources.es-MX.resx  (Español - predeterminado)
  ├── Resources.en-US.resx  (Inglés)
  ├── Errors.es-MX.resx
  ├── Errors.en-US.resx
  ├── Validation.es-MX.resx
  └── Validation.en-US.resx
```

#### Uso en Código VB.NET
```vb
' En código behind
lblWelcome.Text = Resources.Welcome

' En ASPX con expresiones
<asp:Label runat="server" Text="<%$ Resources:Resources, Welcome %>" />

' Con DevExpress
<dx:ASPxButton runat="server" Text="<%$ Resources:Resources, Save %>" />
```

#### Servicio de Localización
```vb
Public Class LocalizationService
    Public Shared Function GetCurrentCulture() As CultureInfo
        ' Obtener de sesión o detectar del navegador
        Dim cultureName As String = SessionHelper.GetCulture()
        If String.IsNullOrEmpty(cultureName) Then
            cultureName = Request.UserLanguages(0)
        End If
        Return New CultureInfo(cultureName)
    End Function
    
    Public Shared Sub SetCulture(cultureName As String)
        Thread.CurrentThread.CurrentCulture = New CultureInfo(cultureName)
        Thread.CurrentThread.CurrentUICulture = New CultureInfo(cultureName)
        SessionHelper.SetCulture(cultureName)
    End Sub
    
    Public Shared Function GetString(key As String) As String
        Return Resources.ResourceManager.GetString(key, GetCurrentCulture())
    End Function
End Class
```

#### Selector de Idioma
```vb
' Componente en Master Page
<dx:ASPxComboBox ID="cmbLanguage" runat="server" 
                 OnSelectedIndexChanged="cmbLanguage_SelectedIndexChanged">
    <Items>
        <dx:ListEditItem Text="Español" Value="es-MX" ImageUrl="~/Content/Images/Flags/mx.png" />
        <dx:ListEditItem Text="English" Value="en-US" ImageUrl="~/Content/Images/Flags/us.png" />
    </Items>
</dx:ASPxComboBox>
```

### Aplicación Móvil - Implementación

#### Archivos de Traducción
```
/Resources/Localization/
  ├── es-MX.json
  ├── en-US.json
  └── fr-FR.json
```

#### Estructura de Archivo JSON
```json
{
  "common": {
    "save": "Guardar",
    "cancel": "Cancelar",
    "delete": "Eliminar",
    "edit": "Editar"
  },
  "auth": {
    "login": "Iniciar Sesión",
    "username": "Usuario",
    "password": "Contraseña",
    "forgotPassword": "¿Olvidaste tu contraseña?"
  },
  "tickets": {
    "title": "Tickets",
    "create": "Crear Ticket",
    "priority": "Prioridad",
    "status": "Estado"
  }
}
```

#### Servicio de Localización Móvil
```csharp
public class LocalizationService
{
    private static Dictionary<string, Dictionary<string, object>> _translations;
    private static string _currentLanguage = "es-MX";
    
    public static async Task InitializeAsync()
    {
        _translations = new Dictionary<string, Dictionary<string, object>>();
        
        // Cargar todos los idiomas
        await LoadLanguageAsync("es-MX");
        await LoadLanguageAsync("en-US");
        
        // Detectar idioma del dispositivo
        var deviceLanguage = CultureInfo.CurrentCulture.Name;
        if (_translations.ContainsKey(deviceLanguage))
        {
            _currentLanguage = deviceLanguage;
        }
    }
    
    private static async Task LoadLanguageAsync(string language)
    {
        var json = await File.ReadAllTextAsync($"Resources/Localization/{language}.json");
        _translations[language] = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
    }
    
    public static string GetString(string key)
    {
        var keys = key.Split('.');
        var current = _translations[_currentLanguage];
        
        foreach (var k in keys)
        {
            if (current.ContainsKey(k))
            {
                if (current[k] is string str)
                    return str;
                current = (Dictionary<string, object>)current[k];
            }
            else
            {
                // Fallback a español
                return GetStringFromLanguage(key, "es-MX");
            }
        }
        
        return key; // Si no se encuentra, retornar la key
    }
    
    public static void SetLanguage(string language)
    {
        if (_translations.ContainsKey(language))
        {
            _currentLanguage = language;
            Preferences.Set("app_language", language);
        }
    }
}
```

#### Uso en XAML
```xml
<!-- Con binding -->
<Label Text="{Binding Source={x:Static loc:LocalizationService}, Path=[tickets.title]}" />

<!-- Con extensión de marcado personalizada -->
<Label Text="{loc:Translate tickets.create}" />
```

### Agente de Voz IA - Multi-Idioma

El Agente de Voz debe detectar el idioma automáticamente:

```vb
Public Class VoiceAgentService
    Public Function ProcessCall(phoneNumber As String, audioStream As Stream) As VoiceResponse
        ' Detectar idioma del audio
        Dim detectedLanguage As String = AzureSpeechService.DetectLanguage(audioStream)
        
        ' Configurar el agente para responder en ese idioma
        Dim agent As New OpenAIAgent(detectedLanguage)
        
        ' Procesar la solicitud
        Dim transcription As String = AzureSpeechService.TranscribeAudio(audioStream, detectedLanguage)
        Dim response As String = agent.ProcessRequest(transcription)
        
        ' Generar audio de respuesta en el idioma detectado
        Dim responseAudio As Stream = AzureSpeechService.TextToSpeech(response, detectedLanguage)
        
        Return New VoiceResponse With {
            .Language = detectedLanguage,
            .Transcription = transcription,
            .ResponseText = response,
            .ResponseAudio = responseAudio
        }
    End Function
End Class
```

### Contenido Dinámico Multi-Idioma

Para contenido generado por usuarios (tickets, comentarios, etc.), el sistema NO traducirá automáticamente, pero sí traducirá:

- Etiquetas de interfaz
- Mensajes del sistema
- Notificaciones automáticas
- Reportes generados
- Emails y SMS del sistema

### Formato de Fechas, Números y Moneda

El sistema debe respetar las convenciones locales:

```vb
' Portal Web
Public Class FormatHelper
    Public Shared Function FormatDate(fecha As DateTime) As String
        Return fecha.ToString("d", LocalizationService.GetCurrentCulture())
    End Function
    
    Public Shared Function FormatCurrency(monto As Decimal) As String
        Return monto.ToString("C", LocalizationService.GetCurrentCulture())
    End Function
End Class
```

```csharp
// App Móvil
public class FormatHelper
{
    public static string FormatDate(DateTime date)
    {
        return date.ToString("d", CultureInfo.CurrentCulture);
    }
    
    public static string FormatCurrency(decimal amount)
    {
        return amount.ToString("C", CultureInfo.CurrentCulture);
    }
}
```

## Manejo de Errores

### Estrategia General

El sistema implementará una estrategia de manejo de errores en capas:

1. **Capa de Presentación**: Captura errores de UI y muestra mensajes amigables
2. **Capa de Servicio**: Captura errores de lógica de negocio y los transforma en respuestas estructuradas
3. **Capa de Infraestructura**: Captura errores de red, base de datos y servicios externos

### Portal Web - Manejo de Errores

#### Errores de Autenticación
```vb
Public Class AuthenticationException
    Inherits Exception
    Public Property ErrorCode As AuthErrorCode
    Public Property RetryAllowed As Boolean
End Class

Public Enum AuthErrorCode
    InvalidCredentials = 1
    AccountLocked = 2
    SessionExpired = 3
    BiometricFailed = 4
    NetworkError = 5
End Enum
```

**Manejo:**
- Mostrar mensaje específico según el código de error
- Registrar intento en log de seguridad
- Implementar rate limiting para prevenir ataques de fuerza bruta

#### Errores de API
```vb
Public Class ApiException
    Inherits Exception
    Public Property StatusCode As Integer
    Public Property ErrorDetails As ApiErrorDetails
End Class

Public Class ApiErrorDetails
    Public Property Code As String
    Public Property Message As String
    Public Property ValidationErrors As Dictionary(Of String, List(Of String))
End Class
```

**Manejo:**
- Reintentar automáticamente para errores 5xx (máximo 3 intentos con backoff exponencial)
- Mostrar errores de validación junto a los campos correspondientes
- Redirigir a página de error para errores críticos

#### Errores de Validación
```vb
Public Class ValidationException
    Inherits Exception
    Public Property Errors As Dictionary(Of String, List(Of String))
End Class
```

**Manejo:**
- Resaltar campos con error en rojo
- Mostrar mensajes de error específicos junto a cada campo
- Prevenir envío del formulario hasta corregir todos los errores

### Aplicación Móvil - Manejo de Errores

#### Errores de Red
```csharp
public class NetworkException : Exception
{
    public NetworkErrorType ErrorType { get; set; }
    public bool IsRetryable { get; set; }
}

public enum NetworkErrorType
{
    NoConnection,
    Timeout,
    ServerError,
    Unauthorized
}
```

**Manejo:**
- Mostrar banner de "Sin conexión" cuando no hay red
- Reintentar automáticamente operaciones críticas
- Guardar operaciones en cola para sincronización posterior

#### Errores de Sincronización
```csharp
public class SyncException : Exception
{
    public string EntityType { get; set; }
    public int LocalId { get; set; }
    public SyncErrorType ErrorType { get; set; }
}

public enum SyncErrorType
{
    Conflict,
    ValidationError,
    NotFound,
    NetworkError
}
```

**Manejo:**
- Para conflictos: mostrar diálogo de resolución (mantener local, usar servidor, fusionar)
- Para errores de validación: permitir editar y reintentar
- Para errores de red: reintentar automáticamente más tarde

#### Errores de Almacenamiento Local
```csharp
public class StorageException : Exception
{
    public StorageErrorType ErrorType { get; set; }
}

public enum StorageErrorType
{
    DiskFull,
    PermissionDenied,
    CorruptedData
}
```

**Manejo:**
- Para disco lleno: ofrecer limpiar caché y datos antiguos
- Para permisos: solicitar permisos al usuario
- Para datos corruptos: intentar recuperar o reinicializar base de datos local

### Logging y Monitoreo

#### Portal Web
```vb
Public Class Logger
    Public Shared Sub LogError(message As String, ex As Exception, Optional userId As String = Nothing)
        ' Registrar en archivo local
        WriteToFile(message, ex)
        
        ' Enviar a Application Insights si está configurado
        ApplicationInsightsHelper.TrackException(ex)
        
        ' Para errores críticos, enviar alerta
        If IsCriticalError(ex) Then
            SendAlertToAdmins(message, ex)
        End If
    End Sub
End Class
```

#### Aplicación Móvil
```csharp
public class Logger
{
    public static async Task LogErrorAsync(string message, Exception ex, string userId = null)
    {
        // Registrar localmente
        await WriteToLocalLogAsync(message, ex);
        
        // Enviar a servicio de telemetría cuando haya conexión
        if (ConnectivityService.IsConnected)
        {
            await TelemetryService.TrackExceptionAsync(ex);
        }
        else
        {
            // Guardar para enviar después
            await QueueTelemetryAsync(message, ex);
        }
    }
}
```

### Páginas de Error Personalizadas

El portal web ya tiene páginas de error implementadas:
- `Error404.aspx`: Recurso no encontrado
- `Error500.aspx`: Error del servidor
- `Error.aspx`: Error general con parámetros

Estas páginas deben:
- Mostrar mensaje amigable al usuario
- Ofrecer acciones útiles (volver al inicio, contactar soporte)
- Registrar el error para análisis posterior
- No exponer información sensible del sistema


## Estrategia de Testing

### Enfoque Dual: Unit Tests + Property-Based Tests

El sistema utilizará un enfoque dual de testing que combina pruebas unitarias tradicionales con pruebas basadas en propiedades (Property-Based Testing - PBT) para garantizar corrección y robustez.

**Justificación:**
- **Unit Tests**: Verifican ejemplos específicos, casos edge y condiciones de error conocidas
- **Property Tests**: Verifican propiedades universales que deben cumplirse para todos los inputs válidos
- **Complementariedad**: Los unit tests capturan bugs concretos, los property tests verifican corrección general

### Frameworks de Testing

#### Portal Web (VB.NET)
- **Unit Testing**: MSTest o NUnit
- **Property-Based Testing**: FsCheck.NET
- **Mocking**: Moq
- **Cobertura**: OpenCover + ReportGenerator

#### Aplicación Móvil (C#)
- **Unit Testing**: xUnit
- **Property-Based Testing**: FsCheck
- **UI Testing**: Xamarin.UITest o Appium
- **Mocking**: Moq

### Configuración de Property-Based Tests

Cada property test debe configurarse para ejecutar **mínimo 100 iteraciones** con datos aleatorios generados, ya que el proceso es probabilístico y necesita suficientes casos para detectar bugs.

```vb
' Ejemplo de configuración en VB.NET con FsCheck
<Property(MaxTest:=100)>
Public Sub PropiedadAutenticacionExitosa(username As NonEmptyString, password As NonEmptyString)
    ' Test implementation
End Sub
```

```csharp
// Ejemplo de configuración en C# con FsCheck
[Property(MaxTest = 100)]
public Property PropiedadTicketOfflineSync(TicketDTO ticket)
{
    // Test implementation
}
```

### Etiquetado de Property Tests

Cada property test DEBE incluir un comentario que referencie explícitamente la propiedad de corrección del documento de diseño:

**Formato obligatorio:**
```
' **Feature: ecosistema-jelabbc, Property {número}: {texto de la propiedad}**
```

**Ejemplo:**
```vb
' **Feature: ecosistema-jelabbc, Property 1: Autenticación exitosa crea sesión válida**
<Property(MaxTest:=100)>
Public Sub PropiedadAutenticacionExitosa()
    ' Para cualquier usuario con credenciales válidas,
    ' cuando se autentica mediante la API REST,
    ' el sistema debe crear una sesión segura con un token válido
End Sub
```

### Estrategia por Módulo

#### 1. Módulo de Autenticación

**Unit Tests:**
- Login con credenciales válidas específicas
- Login con credenciales inválidas específicas
- Recuperación de contraseña con email válido
- Bloqueo de cuenta después de 5 intentos fallidos
- Expiración de sesión después de 30 minutos

**Property Tests:**
- **Propiedad 1**: Para cualquier usuario válido, autenticación exitosa crea sesión
- **Propiedad 3**: Para cualquier credencial inválida, autenticación falla sin crear sesión
- **Propiedad 5**: Para cualquier sesión activa, logout limpia datos sensibles
- **Propiedad 7**: Para cualquier contraseña, validación de complejidad es consistente

#### 2. Módulo de Entidades

**Unit Tests:**
- Crear entidad de tipo Condominio con datos mínimos
- Editar nombre de entidad existente
- Eliminar entidad sin dependencias
- Intentar eliminar entidad con subentidades (debe fallar)

**Property Tests:**
- **Propiedad 11**: Para cualquier entidad válida, se persiste correctamente
- **Propiedad 12**: Para cualquier entidad, edición preserva datos no modificados
- **Propiedad 13**: Para cualquier entidad con dependencias, eliminación falla
- **Propiedad 15**: Para cualquier subentidad, asociación a padre es correcta

#### 3. Módulo de Tickets

**Unit Tests:**
- Crear ticket con prioridad Alta
- Asignar técnico a ticket específico
- Cerrar ticket y verificar solicitud de calificación
- Filtrar tickets por estado "Abierto"

**Property Tests:**
- **Propiedad 16**: Para cualquier ticket, todos los campos requeridos se capturan
- **Propiedad 17**: Para cualquier ticket, el folio es único
- **Propiedad 18**: Para cualquier cambio de estado, se registra auditoría completa
- **Propiedad 19**: Para cualquier conjunto de filtros, resultados cumplen criterios

#### 4. Módulo de IoT y Agricultura

**Unit Tests:**
- Sensor de humedad envía valor 45% y actualiza dashboard
- Temperatura supera 35°C y genera alerta
- Activar riego manualmente en parcela específica
- Cancelar riego cuando humedad es 80%

**Property Tests:**
- **Propiedad 23**: Para cualquier dato de sensor, indicadores se actualizan en < 2s
- **Propiedad 24**: Para cualquier dato que supera umbral, se genera alerta
- **Propiedad 25**: Para cualquier condición cumplida, riego se activa
- **Propiedad 26**: Para cualquier humedad suficiente, riego se cancela

#### 5. Aplicación Móvil - Sincronización Offline

**Unit Tests:**
- Crear ticket offline y verificar almacenamiento local
- Sincronizar 1 ticket pendiente al recuperar conexión
- Resolver conflicto eligiendo versión del servidor
- Limpiar datos sincronizados de base de datos local

**Property Tests:**
- **Propiedad 28**: Para cualquier ticket offline, round-trip de sincronización preserva datos
- **Propiedad 29**: Para cualquier conjunto de operaciones pendientes, todas se procesan

### Generadores Inteligentes para Property Tests

Los property tests requieren generadores que produzcan datos válidos y representativos:

```vb
' Generador de usuarios válidos
Public Class ValidUserGenerator
    Implements IArbitrary(Of UserDTO)
    
    Public Function Generate() As Gen(Of UserDTO)
        Return From nombre In Arb.Generate(Of NonEmptyString)()
               From email In GenerateValidEmail()
               From rol In Gen.Elements(1, 2, 3, 4)
               Select New UserDTO With {
                   .Nombre = nombre.Get,
                   .Email = email,
                   .RolId = rol,
                   .Activo = True
               }
    End Function
End Class
```

```csharp
// Generador de tickets válidos
public class ValidTicketGenerator : Arbitrary<TicketDTO>
{
    public override Gen<TicketDTO> Generator =>
        from titulo in Arb.Generate<NonEmptyString>()
        from descripcion in Arb.Generate<NonEmptyString>()
        from prioridad in Gen.Elements(1, 2, 3, 4)
        from categoria in Gen.Choose(1, 10)
        select new TicketDTO
        {
            Titulo = titulo.Get,
            Descripcion = descripcion.Get,
            Prioridad = (PrioridadTicket)prioridad,
            CategoriaId = categoria,
            FechaCreacion = DateTime.Now
        };
}
```

### Cobertura de Código

**Objetivos:**
- **Código crítico** (autenticación, pagos, seguridad): 90%+ cobertura
- **Lógica de negocio**: 80%+ cobertura
- **UI y presentación**: 60%+ cobertura

**Herramientas:**
- Portal Web: OpenCover + ReportGenerator
- App Móvil: Coverlet + ReportGenerator

### Integración Continua

Los tests deben ejecutarse automáticamente en cada commit:

1. **Pre-commit**: Linters y formatters
2. **CI Pipeline**:
   - Compilación
   - Unit tests (rápidos, < 5 min)
   - Property tests (más lentos, < 15 min)
   - Análisis de cobertura
   - Análisis estático de código

3. **Nightly Build**:
   - Tests de integración completos
   - Tests de UI
   - Tests de rendimiento
   - Análisis de seguridad

### Estrategia de Mocking

**Principio**: Minimizar mocks para mantener tests simples y cercanos a la realidad.

**Cuándo usar mocks:**
- Servicios externos (APIs de terceros, pasarelas de pago)
- Operaciones costosas (envío de emails, SMS)
- Dependencias no determinísticas (fecha/hora actual, generadores aleatorios)

**Cuándo NO usar mocks:**
- Lógica de negocio interna
- Validaciones
- Transformaciones de datos
- Operaciones de base de datos (usar base de datos de test)

### Tests de Regresión

Cuando se encuentra un bug en producción:

1. Crear un unit test que reproduzca el bug
2. Verificar que el test falla
3. Corregir el bug
4. Verificar que el test pasa
5. Considerar si se necesita un property test adicional para prevenir bugs similares



## Módulo de Servicios Municipales

### Visión General

El Módulo de Servicios Municipales gestiona el proceso completo de licitaciones, fallos, órdenes de compra y ejecución en campo mediante un flujo multinivel que involucra cuatro actores jerárquicos: Entidad → SubEntidad → Proveedor → Colaborador.

**Objetivos del Módulo:**
- Gestionar licitaciones y fallos con captura de secciones (colonias) y conceptos
- Implementar flujo multinivel de documentos con seguridad estricta
- Controlar órdenes de compra con seguimiento de tiempos y KPIs
- Gestionar dictámenes técnicos con aprobación/rechazo
- Proporcionar alertas configurables vía WhatsApp
- Habilitar comunicación entre partes mediante chat integrado

### Arquitectura del Módulo

```
┌─────────────────────────────────────────────────────────────┐
│                    NIVEL 1: ENTIDAD                          │
│  (Funcionario Municipal / Administrador)                     │
│                                                              │
│  ✅ Crea documento de fallo                                  │
│  ✅ Captura secciones (colonias)                             │
│  ✅ Captura conceptos                                        │
│  ✅ Define monto_entidad (presupuesto base)                  │
│  ✅ Asigna a SubEntidad                                      │
│  ✅ Aprueba/Rechaza dictámenes                               │
│  ✅ Gestiona OCs (Nueva → En Proceso → Pagada)              │
│  ✅ Visualiza KPIs completos                                 │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                  NIVEL 2: SUBENTIDAD                         │
│  (Departamento / División)                                   │
│                                                              │
│  ✅ Recibe documento asignado                                │
│  ✅ Ve monto_entidad (presupuesto asignado)                  │
│  ✅ Captura monto_subentidad (propuesta ajustada)            │
│  ✅ Asigna a Proveedor                                       │
│  ✅ Crea dictámenes técnicos                                 │
│  ❌ NO ve monto_proveedor ni monto_real                      │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   NIVEL 3: PROVEEDOR                         │
│  (Empresa Externa)                                           │
│                                                              │
│  ✅ Recibe documento asignado                                │
│  ❌ NO ve monto_entidad                                      │
│  ✅ Ve monto_subentidad (presupuesto aprobado)               │
│  ✅ Captura monto_proveedor (cotización)                     │
│  ✅ Asigna a Colaboradores                                   │
│  ❌ NO ve monto_real                                         │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                  NIVEL 4: COLABORADOR                        │
│  (Técnico en Campo)                                          │
│                                                              │
│  ✅ Recibe documento asignado (App Móvil)                    │
│  ❌ NO ve monto_entidad ni monto_subentidad                  │
│  ✅ Ve monto_proveedor (presupuesto asignado)                │
│  ✅ Captura monto_real (costo real ejecutado)                │
│  ✅ Registra evidencias (fotos, GPS)                         │
└─────────────────────────────────────────────────────────────┘
```

### Componentes del Módulo

#### 1. Gestión de Licitaciones y Fallos

**Componentes:**
- `ServiciosMunicipales.aspx`: Página principal del módulo
- `FalloLicitacion.aspx`: Captura de fallos con secciones y conceptos
- `ServiciosMunicipalesService.vb`: Lógica de negocio
- `FalloDTO.vb`: Modelo de datos

**Interfaces:**
```vb
Public Interface IServiciosMunicipalesService
    Function CrearFallo(fallo As FalloDTO) As OperationResult
    Function AgregarSeccion(falloId As Integer, seccion As SeccionDTO) As OperationResult
    Function AgregarConcepto(falloId As Integer, concepto As ConceptoFalloDTO) As OperationResult
    Function AsignarASubEntidad(falloId As Integer, subEntidadId As Integer) As OperationResult
End Interface

Public Class FalloDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property Descripcion As String
    Public Property FechaFallo As DateTime
    Public Property EntidadId As Integer
    Public Property Secciones As List(Of SeccionDTO)
    Public Property Conceptos As List(Of ConceptoFalloDTO)
    Public Property Estado As EstadoFallo
End Class

Public Class SeccionDTO
    Public Property Id As Integer
    Public Property NumPartida As Integer
    Public Property ClaveSeccion As String
    Public Property NombreSeccion As String
    Public Property MontoMinEntidad As Decimal
    Public Property MontoMaxEntidad As Decimal
    Public Property MontoMinSubEntidad As Decimal?
    Public Property MontoMaxSubEntidad As Decimal?
    Public Property MontoMinProveedor As Decimal?
    Public Property MontoMaxProveedor As Decimal?
End Class

Public Enum EstadoFallo
    Borrador = 1
    AsignadoSubEntidad = 2
    AsignadoProveedor = 3
    AsignadoColaborador = 4
    EnEjecucion = 5
    Completado = 6
End Enum
```

#### 2. Gestión de Órdenes de Compra

**Componentes:**
- `OrdenesCompraMunicipal.aspx`: Gestión de OCs con estados
- `OCMunicipalService.vb`: Lógica de negocio y workflow
- `OCMunicipalDTO.vb`: Modelo de datos

**Interfaces:**
```vb
Public Interface IOCMunicipalService
    Function CrearOC(oc As OCMunicipalDTO) As OperationResult
    Function CambiarEstado(ocId As Integer, nuevoEstado As EstadoOC) As OperationResult
    Function GetKPIs(entidadId As Integer, fechaInicio As DateTime, fechaFin As DateTime) As KPIsDTO
    Function ConfigurarAlerta(alerta As AlertaOCDTO) As OperationResult
End Interface

Public Class OCMunicipalDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property FalloId As Integer
    Public Property SubEntidadId As Integer
    Public Property FechaInicio As DateTime
    Public Property FechaFin As DateTime
    Public Property Estado As EstadoOC
    Public Property TiempoEstimadoDias As Integer
    Public Property TiempoRealDias As Integer?
    Public Property PorcentajeCumplimiento As Decimal?
    Public Property Dictamenes As List(Of DictamenDTO)
End Class

Public Enum EstadoOC
    OCNueva = 1
    EnProcesoPago = 2
    OCPagada = 3
End Enum

Public Class KPIsDTO
    Public Property TotalOCs As Integer
    Public Property OCsEnTiempo As Integer
    Public Property OCsRetrasadas As Integer
    Public Property TiempoPromedioOCNueva As Decimal
    Public Property TiempoPromedioEnProceso As Decimal
    Public Property TiempoPromedioTotal As Decimal
    Public Property PorcentajeCumplimientoPromedio As Decimal
    Public Property OCsEnRiesgo As List(Of OCMunicipalDTO)
End Class
```

#### 3. Gestión de Dictámenes

**Componentes:**
- `DictamenesMunicipales.aspx`: Gestión de dictámenes técnicos
- `DictamenService.vb`: Lógica de negocio
- `DictamenDTO.vb`: Modelo de datos

**Interfaces:**
```vb
Public Interface IDictamenService
    Function CrearDictamen(dictamen As DictamenDTO) As OperationResult
    Function AprobarDictamen(dictamenId As Integer, folioPago As String) As OperationResult
    Function RechazarDictamen(dictamenId As Integer, comentarios As String) As OperationResult
    Function GetDictamenesPorOC(ocId As Integer) As List(Of DictamenDTO)
End Interface

Public Class DictamenDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property OCId As Integer
    Public Property SubEntidadId As Integer
    Public Property Descripcion As String
    Public Property MontoTotal As Decimal
    Public Property Estado As EstadoDictamen
    Public Property FolioPago As String
    Public Property ComentariosRechazo As String
    Public Property Adjuntos As List(Of AdjuntoDTO)
    Public Property FechaCreacion As DateTime
    Public Property FechaAprobacion As DateTime?
End Class

Public Enum EstadoDictamen
    Nuevo = 1
    PendienteAprobacion = 2
    Aprobado = 3
    Rechazado = 4
End Enum
```

#### 4. Sistema de Alertas

**Componentes:**
- `AlertasOC.aspx`: Configuración de alertas
- `AlertaService.vb`: Lógica de envío de alertas
- `WhatsAppService.vb`: Integración con Twilio

**Interfaces:**
```vb
Public Interface IAlertaService
    Function ConfigurarAlerta(alerta As AlertaOCDTO) As OperationResult
    Function EvaluarAlertas() As List(Of AlertaEjecutadaDTO)
    Function EnviarAlertaWhatsApp(telefonos As List(Of String), mensaje As String) As OperationResult
End Interface

Public Class AlertaOCDTO
    Public Property Id As Integer
    Public Property OCId As Integer
    Public Property MensajeAlerta As String
    Public Property DiasParaAlerta As List(Of Integer) ' Ej: [3, 6, 9]
    Public Property TelefonosDestino As List(Of String)
    Public Property Activa As Boolean
End Class
```

#### 5. Chat Integrado en Documentos

**Componentes:**
- `ChatDocumento.ascx`: User Control para chat
- `ChatService.vb`: Lógica de mensajería
- `MensajeChatDTO.vb`: Modelo de datos

**Interfaces:**
```vb
Public Interface IChatService
    Function EnviarMensaje(mensaje As MensajeChatDTO) As OperationResult
    Function GetMensajes(documentoId As Integer) As List(Of MensajeChatDTO)
    Function EditarMensaje(mensajeId As Integer, nuevoTexto As String) As OperationResult
    Function NotificarPartes(documentoId As Integer, mensaje As String) As OperationResult
End Interface

Public Class MensajeChatDTO
    Public Property Id As Integer
    Public Property DocumentoId As Integer
    Public Property UsuarioId As Integer
    Public Property NombreUsuario As String
    Public Property Mensaje As String
    Public Property FechaEnvio As DateTime
    Public Property FechaEdicion As DateTime?
    Public Property Editado As Boolean
End Class
```

### Modelos de Datos

#### Tablas de Base de Datos

**Tabla: op_documentos (existente, se reutiliza)**
```sql
-- Campos adicionales para Servicios Municipales
ALTER TABLE op_documentos
ADD COLUMN IF NOT EXISTS tipo_documento VARCHAR(50) COMMENT 'Fallo, OC, Dictamen',
ADD COLUMN IF NOT EXISTS fallo_id INT COMMENT 'Referencia al fallo original',
ADD COLUMN IF NOT EXISTS fecha_inicio DATETIME COMMENT 'Fecha inicio de OC',
ADD COLUMN IF NOT EXISTS fecha_fin DATETIME COMMENT 'Fecha fin de OC',
ADD COLUMN IF NOT EXISTS tiempo_estimado_dias INT COMMENT 'Tiempo estimado en días',
ADD COLUMN IF NOT EXISTS tiempo_real_dias INT COMMENT 'Tiempo real ejecutado',
ADD COLUMN IF NOT EXISTS porcentaje_cumplimiento DECIMAL(5,2) COMMENT 'KPI de cumplimiento',
ADD COLUMN IF NOT EXISTS folio_pago VARCHAR(100) COMMENT 'Folio de pago asignado';
```

**Tabla: op_documentos_secciones (nueva)**
```sql
CREATE TABLE IF NOT EXISTS op_documentos_secciones (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_documento INT NOT NULL,
    num_partida INT NOT NULL,
    clave_seccion VARCHAR(50) NOT NULL,
    nombre_seccion VARCHAR(255) NOT NULL,
    monto_min_entidad DECIMAL(18,2),
    monto_max_entidad DECIMAL(18,2),
    monto_min_subentidad DECIMAL(18,2),
    monto_max_subentidad DECIMAL(18,2),
    monto_min_proveedor DECIMAL(18,2),
    monto_max_proveedor DECIMAL(18,2),
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    fecha_actualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    
    FOREIGN KEY (id_documento) REFERENCES op_documentos(id) ON DELETE CASCADE,
    INDEX idx_documento (id_documento),
    INDEX idx_partida (num_partida)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

**Tabla: op_alertas_oc (nueva)**
```sql
CREATE TABLE IF NOT EXISTS op_alertas_oc (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_oc INT NOT NULL,
    mensaje_alerta TEXT NOT NULL,
    dias_para_alerta JSON NOT NULL COMMENT 'Array de días: [3, 6, 9]',
    telefonos_destino JSON NOT NULL COMMENT 'Array de teléfonos',
    activa BOOLEAN DEFAULT TRUE,
    fecha_creacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    FOREIGN KEY (id_oc) REFERENCES op_documentos(id) ON DELETE CASCADE,
    INDEX idx_oc (id_oc),
    INDEX idx_activa (activa)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

**Tabla: op_alertas_ejecutadas (nueva)**
```sql
CREATE TABLE IF NOT EXISTS op_alertas_ejecutadas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_alerta INT NOT NULL,
    id_oc INT NOT NULL,
    dia_alerta INT NOT NULL,
    mensaje_enviado TEXT NOT NULL,
    telefonos_enviados JSON NOT NULL,
    fecha_ejecucion DATETIME DEFAULT CURRENT_TIMESTAMP,
    resultado VARCHAR(50) COMMENT 'exitoso, fallido',
    
    FOREIGN KEY (id_alerta) REFERENCES op_alertas_oc(id) ON DELETE CASCADE,
    FOREIGN KEY (id_oc) REFERENCES op_documentos(id) ON DELETE CASCADE,
    INDEX idx_alerta (id_alerta),
    INDEX idx_oc (id_oc),
    INDEX idx_fecha (fecha_ejecucion)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

**Tabla: op_chat_documentos (nueva)**
```sql
CREATE TABLE IF NOT EXISTS op_chat_documentos (
    id INT AUTO_INCREMENT PRIMARY KEY,
    id_documento INT NOT NULL,
    id_usuario INT NOT NULL,
    mensaje TEXT NOT NULL,
    fecha_envio DATETIME DEFAULT CURRENT_TIMESTAMP,
    fecha_edicion DATETIME NULL,
    editado BOOLEAN DEFAULT FALSE,
    
    FOREIGN KEY (id_documento) REFERENCES op_documentos(id) ON DELETE CASCADE,
    FOREIGN KEY (id_usuario) REFERENCES conf_usuarios(id),
    INDEX idx_documento (id_documento),
    INDEX idx_fecha (fecha_envio)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### Propiedades de Corrección - Servicios Municipales

**Propiedad 38: Fallo captura secciones y conceptos**
*Para cualquier* documento de fallo creado, el sistema debe permitir agregar secciones (colonias) y conceptos con montos mínimos/máximos por nivel.
**Valida: Requerimientos 28.1**

**Propiedad 39: Asignación cambia estado y notifica**
*Para cualquier* documento asignado a un nivel inferior, el sistema debe cambiar el estado correspondiente y enviar notificación al responsable.
**Valida: Requerimientos 28.2, 28.4, 28.6**

**Propiedad 40: SubEntidad ve solo monto de Entidad**
*Para cualquier* documento recibido por SubEntidad, el sistema debe mostrar únicamente el monto_entidad y ocultar monto_proveedor y monto_real.
**Valida: Requerimientos 28.3**

**Propiedad 41: Proveedor ve solo monto de SubEntidad**
*Para cualquier* documento recibido por Proveedor, el sistema debe mostrar únicamente el monto_subentidad y ocultar monto_entidad y monto_real.
**Valida: Requerimientos 28.5**

**Propiedad 42: Colaborador ve solo monto de Proveedor**
*Para cualquier* documento recibido por Colaborador, el sistema debe mostrar únicamente el monto_proveedor y ocultar montos de niveles anteriores.
**Valida: Requerimientos 28.7**

**Propiedad 43: OC inicia con fechas y estado correcto**
*Para cualquier* OC generada, el sistema debe crear el registro con fecha_inicio, fecha_fin, tiempo_estimado_dias y estado "OC Nueva".
**Valida: Requerimientos 29.1, 29.2**

**Propiedad 44: Alertas se envían según configuración**
*Para cualquier* OC con alertas configuradas, cuando se cumple la condición de tiempo, el sistema debe enviar notificación vía WhatsApp a los teléfonos configurados.
**Valida: Requerimientos 29.3**

**Propiedad 45: Dictamen se vincula a OC**
*Para cualquier* dictamen creado por SubEntidad, el sistema debe vincularlo correctamente a la OC correspondiente y notificar a la Entidad.
**Valida: Requerimientos 29.4**

**Propiedad 46: Aprobación cambia estado a En Proceso**
*Para cualquier* dictamen aprobado, el sistema debe cambiar el estado de la OC a "En Proceso de Pago" y asignar folio de pago.
**Valida: Requerimientos 29.5**

**Propiedad 47: Rechazo devuelve con comentarios**
*Para cualquier* dictamen rechazado, el sistema debe devolverlo a SubEntidad con estado "Rechazado" y comentarios de rechazo visibles.
**Valida: Requerimientos 29.6**

**Propiedad 48: Pago registra tiempo total (Round-trip)**
*Para cualquier* OC pagada, el sistema debe calcular el tiempo_real_dias desde fecha_inicio hasta fecha de pago y registrar el porcentaje_cumplimiento.
**Valida: Requerimientos 29.7**

**Propiedad 49: KPIs calculan tiempos por fase**
*Para cualquier* conjunto de OCs en un rango de fechas, el sistema debe calcular correctamente los tiempos promedio por fase (OC Nueva, En Proceso, Pagada).
**Valida: Requerimientos 30.1**

**Propiedad 50: Cumplimiento compara real vs estimado**
*Para cualquier* OC completada, el porcentaje de cumplimiento debe ser igual a (tiempo_estimado_dias / tiempo_real_dias) * 100.
**Valida: Requerimientos 30.2**

**Propiedad 51: Retraso identifica fase problemática**
*Para cualquier* OC con cumplimiento < 100%, el sistema debe identificar en qué fase (OC Nueva, En Proceso, Pagada) ocurrió el mayor retraso.
**Valida: Requerimientos 30.3**

**Propiedad 52: Configuración de alerta persiste**
*Para cualquier* alerta configurada, el sistema debe guardar mensaje, destinatarios y momentos de envío correctamente.
**Valida: Requerimientos 30.4**

**Propiedad 53: Alerta se envía en momento correcto**
*Para cualquier* alerta activa, cuando el tiempo transcurrido coincide con uno de los días configurados, el sistema debe enviar la notificación vía WhatsApp.
**Valida: Requerimientos 30.5**

**Propiedad 54: Dashboard muestra KPIs correctos**
*Para cualquier* entidad, el dashboard de KPIs debe mostrar gráficas con datos correctos de cumplimiento, tiempos promedio y OCs en riesgo.
**Valida: Requerimientos 30.6**

**Propiedad 55: Chat guarda mensajes con timestamp**
*Para cualquier* mensaje enviado en el chat de un documento, el sistema debe guardarlo con timestamp, nombre del remitente y vincularlo al documento.
**Valida: Requerimientos 31.2**

**Propiedad 56: Chat notifica a todas las partes**
*Para cualquier* mensaje enviado en el chat, el sistema debe notificar a todos los usuarios involucrados en el documento (Entidad, SubEntidad, Proveedor, Colaborador).
**Valida: Requerimientos 31.3**

**Propiedad 57: Edición de mensaje registra timestamp**
*Para cualquier* mensaje editado en el chat, el sistema debe registrar la fecha_edicion y marcar el campo editado como true.
**Valida: Requerimientos 31.4**

**Propiedad 58: Historial de chat se mantiene completo**
*Para cualquier* documento activo, el sistema debe mantener el historial completo del chat en orden cronológico sin pérdida de mensajes.
**Valida: Requerimientos 31.5**

### Estrategia de Testing - Servicios Municipales

#### Property-Based Tests

Los siguientes property tests deben implementarse para el módulo de Servicios Municipales:

1. **Test de Visibilidad de Montos por Nivel**
   - Generar usuarios aleatorios de niveles 1-4
   - Generar documentos con secciones y montos
   - Verificar que cada nivel ve solo los montos permitidos
   - Ejecutar 100+ iteraciones

2. **Test de Flujo de Estados de OC**
   - Generar OCs aleatorias
   - Simular transiciones de estado (Nueva → En Proceso → Pagada)
   - Verificar que cada transición es válida y registra timestamps
   - Ejecutar 100+ iteraciones

3. **Test de Cálculo de KPIs**
   - Generar conjuntos aleatorios de OCs con tiempos variados
   - Calcular KPIs manualmente
   - Verificar que el sistema calcula los mismos valores
   - Ejecutar 100+ iteraciones

4. **Test de Alertas**
   - Generar OCs con diferentes configuraciones de alertas
   - Simular paso del tiempo
   - Verificar que las alertas se envían en los momentos correctos
   - Ejecutar 100+ iteraciones

5. **Test de Chat**
   - Generar mensajes aleatorios en documentos
   - Verificar que se guardan correctamente
   - Verificar que las notificaciones se envían a todas las partes
   - Ejecutar 100+ iteraciones

#### Unit Tests

Los siguientes unit tests deben implementarse:

1. **Tests de Seguridad Multinivel**
   - SubEntidad no ve monto_proveedor
   - Proveedor no ve monto_entidad
   - Colaborador no ve montos anteriores
   - Administrador ve todo

2. **Tests de Workflow de OC**
   - Creación de OC con fechas correctas
   - Cambio de estado válido
   - Cambio de estado inválido (debe fallar)
   - Cálculo de tiempo real

3. **Tests de Dictámenes**
   - Creación de dictamen vinculado a OC
   - Aprobación asigna folio de pago
   - Rechazo devuelve con comentarios
   - Dictamen sin OC (debe fallar)

4. **Tests de Alertas**
   - Configuración de alerta válida
   - Envío de alerta en momento correcto
   - Alerta desactivada no se envía
   - Registro de alerta ejecutada

5. **Tests de Chat**
   - Envío de mensaje
   - Edición de mensaje
   - Notificación a partes involucradas
   - Historial completo

### Integración con Módulos Existentes

El módulo de Servicios Municipales se integra con:

1. **Módulo de Captura de Documentos (existente)**
   - Reutiliza tabla op_documentos
   - Extiende con campos específicos de Servicios Municipales
   - Aplica misma seguridad multinivel

2. **Módulo de Entidades y SubEntidades (existente)**
   - Usa catentidades y catsubentidades
   - Valida permisos según jerarquía

3. **Módulo de Proveedores (existente)**
   - Usa catproveedores
   - Asigna documentos a proveedores

4. **Módulo de Usuarios (existente)**
   - Usa confusuarios con campo nivel_jerarquico
   - Valida accesos según nivel

5. **Sistema de Notificaciones (existente)**
   - Integra con WhatsAppService para alertas
   - Usa NotificationService para notificaciones en app

### Consideraciones de Rendimiento

1. **Índices de Base de Datos**
   - Índice en op_documentos(tipo_documento, estado)
   - Índice en op_documentos_secciones(id_documento)
   - Índice en op_alertas_oc(activa, id_oc)
   - Índice en op_chat_documentos(id_documento, fecha_envio)

2. **Caché**
   - Cachear KPIs por 5 minutos
   - Cachear configuración de alertas por 10 minutos
   - Invalidar caché al cambiar estado de OC

3. **Consultas Optimizadas**
   - Usar stored procedures para cálculo de KPIs
   - Paginación en listado de OCs
   - Lazy loading de chat (últimos 50 mensajes)

### Seguridad Adicional

1. **Validación de Permisos**
   - Validar nivel jerárquico en cada operación
   - Registrar intentos de acceso no autorizado
   - Alertar a administradores por múltiples intentos

2. **Auditoría**
   - Registrar todos los cambios de estado
   - Registrar todas las asignaciones
   - Registrar todas las aprobaciones/rechazos

3. **Cifrado**
   - Cifrar mensajes del chat en tránsito (HTTPS)
   - Cifrar números de teléfono en base de datos
   - Cifrar folios de pago


## Módulo 07.5: Formularios Dinámicos y Gestión de Capturas

### Descripción General

Este módulo proporciona un sistema integral de formularios dinámicos que permite la creación, gestión y captura de información mediante formularios adaptables a diferentes plataformas (Web y Móvil). El sistema utiliza Azure Document Intelligence para extraer campos de PDFs de plantilla, y genera PDFs finales con el formato original llenos con los datos capturados.

**Conceptos Clave:**
- **Definición Abstracta**: Los formularios se definen en BD (cat_formularios + cat_campos_formulario) como fuente única de verdad
- **Renderizado Multiplataforma**: Cada plataforma interpreta la definición y renderiza con sus controles nativos
- **Plantilla PDF**: Template HTML/CSS en cat_plantilla_pdf para generar el PDF final con formato similar al original

### Arquitectura del Módulo

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                    DEFINICIÓN ABSTRACTA (Base de Datos MySQL)               │
│  cat_formularios + cat_campos_formulario + cat_opciones_campo               │
│  (Estructura, tipos, orden, secciones, validaciones)                        │
│  cat_plantilla_pdf (Template HTML para generación de PDF)                   │
└─────────────────────────────────────────────────────────────────────────────┘
                    ↓                    ↓                    ↓
         ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐
         │   PORTAL WEB    │  │   APP MÓVIL     │  │   PDF FINAL     │
         │   (ASP.NET)     │  │   (MAUI)        │  │   (Servidor)    │
         │                 │  │                 │  │                 │
         │ DevExpress      │  │ StackLayout +   │  │ HTML Template   │
         │ ASPxFormLayout  │  │ Controles MAUI  │  │ + SelectPdf/    │
         │ + controles     │  │ nativos         │  │ iTextSharp      │
         └────────┬────────┘  └────────┬────────┘  └────────┬────────┘
                  │                    │                    │
                  └────────────────────┼────────────────────┘
                                       ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│                         API REST (Backend Existente)                        │
│  CrudController dinámico + Endpoints específicos para sincronización        │
└─────────────────────────────────────────────────────────────────────────────┘
                                       ↓
         ┌─────────────────┬───────────────────┬─────────────────┐
         ↓                 ↓                   ↓                 ↓
    MySQL DB         Azure Blob          Azure Document    SelectPdf/
   (jela_qa)         Storage             Intelligence      iTextSharp
                     - formularios-fotos  (Extracción)     (Generación PDF)
                     - formularios-firmas
                     - formularios-pdf
```

### Flujo Completo del Módulo

```
┌─────────────────────────────────────────────────────────────────────────────┐
│  FASE 1: Creación del Formulario (Portal Web - Admin)                       │
├─────────────────────────────────────────────────────────────────────────────┤
│  OPCIÓN A: Desde PDF                                                        │
│  Admin sube PDF → Azure Document Intelligence extrae campos →               │
│  Admin revisa/ajusta campos detectados → Guarda en cat_formularios +        │
│  cat_campos_formulario → Admin crea/edita plantilla HTML en cat_plantilla_pdf│
│  (PDF original NO se almacena, solo se procesa para extracción)             │
│                                                                             │
│  OPCIÓN B: Manual                                                           │
│  Admin crea formulario → Define campos uno a uno → Configura secciones →    │
│  Crea plantilla HTML para PDF                                               │
└─────────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│  FASE 2: Asignación (Portal Web - Funcionario)                              │
├─────────────────────────────────────────────────────────────────────────────┤
│  Funcionario crea fallo → Selecciona formulario → Sistema crea registro     │
│  en op_fallo_formulario vinculando fallo + formulario + usuario asignado    │
└─────────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│  FASE 3: Captura en Campo (App Móvil o Web - Colaborador)                   │
├─────────────────────────────────────────────────────────────────────────────┤
│  Colaborador abre fallo → Sistema carga definición de cat_campos_formulario │
│  → Renderiza formulario con controles nativos (MAUI o DevExpress) →         │
│  Colaborador llena campos, toma fotos, captura firma →                      │
│  App guarda localmente en SQLite (móvil) o envía directo (web)              │
└─────────────────────────────────────────────────────────────────────────────┘
                                    ↓
┌─────────────────────────────────────────────────────────────────────────────┐
│  FASE 4: Sincronización y Generación de PDF (Automático al completar)       │
├─────────────────────────────────────────────────────────────────────────────┤
│  1. App sincroniza → Guarda en op_respuesta_formulario + op_respuesta_campo │
│  2. Sube fotos a Azure Blob Storage (contenedor: formularios-fotos)         │
│  3. Sube firma a Azure Blob Storage (contenedor: formularios-firmas)        │
│  4. Servidor genera PDF:                                                    │
│     - Obtiene plantilla HTML de cat_plantilla_pdf                           │
│     - Reemplaza placeholders con datos de op_respuesta_campo                │
│     - Inserta imágenes de fotos y firma                                     │
│     - Convierte HTML a PDF con SelectPdf/iTextSharp                         │
│  5. Sube PDF a Azure Blob Storage (contenedor: formularios-pdf)             │
│  6. Registra en op_documento_formulario_pdf con url_pdf_azure               │
│  7. Vincula PDF al fallo como documento hijo                                │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Modelo de Datos

**Nota:** Las tablas usan la nomenclatura correcta:
- `cat_` = Catálogos (datos maestros/referencia)
- `op_` = Operaciones/Transacciones (datos transaccionales)
- `conf_` = Configuración (parámetros del sistema)

#### Tabla: cat_formularios
Catálogo maestro de formularios disponibles.

```sql
CREATE TABLE cat_formularios (
    formulario_id INT PRIMARY KEY AUTO_INCREMENT,
    nombre_formulario VARCHAR(255) NOT NULL UNIQUE,
    descripcion TEXT,
    plataformas VARCHAR(100) NOT NULL DEFAULT 'web,movil',
    estado ENUM('activo', 'inactivo', 'borrador') DEFAULT 'borrador',
    version INT DEFAULT 1,
    icono VARCHAR(50) DEFAULT 'fa-file-alt',
    color_tema VARCHAR(7) DEFAULT '#007bff',
    requiere_firma BOOLEAN DEFAULT FALSE,
    requiere_foto BOOLEAN DEFAULT FALSE,
    tiempo_estimado_minutos INT DEFAULT 15,
    instrucciones TEXT,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    creado_por INT NOT NULL,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modificado_por INT,
    INDEX idx_estado (estado),
    INDEX idx_plataformas (plataformas)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### Tabla: cat_campos_formulario
Define los campos individuales de cada formulario con metadatos de layout para renderizado multiplataforma.

```sql
CREATE TABLE cat_campos_formulario (
    campo_id INT PRIMARY KEY AUTO_INCREMENT,
    formulario_id INT NOT NULL,
    nombre_campo VARCHAR(255) NOT NULL COMMENT 'Nombre interno del campo',
    etiqueta_campo VARCHAR(255) NOT NULL COMMENT 'Etiqueta visible para el usuario',
    tipo_campo ENUM(
        'texto', 'numero', 'decimal', 'fecha', 'fecha_hora', 'hora',
        'email', 'telefono', 'foto', 'firma', 'dropdown', 'checkbox',
        'radio', 'textarea', 'archivo', 'ubicacion', 'codigo_barras',
        'separador', 'titulo_seccion'
    ) NOT NULL,
    es_requerido BOOLEAN DEFAULT FALSE,
    es_visible BOOLEAN DEFAULT TRUE,
    es_editable BOOLEAN DEFAULT TRUE,
    posicion_orden INT DEFAULT 0 COMMENT 'Orden de renderizado',
    seccion VARCHAR(100) COMMENT 'Agrupación visual de campos',
    ancho_columna INT DEFAULT 12 COMMENT 'Grid Bootstrap 1-12 para web',
    valor_por_defecto VARCHAR(500),
    ayuda_campo TEXT,
    placeholder VARCHAR(255),
    longitud_maxima INT,
    longitud_minima INT,
    patron_validacion VARCHAR(500),
    mensaje_error_patron VARCHAR(255),
    depende_de_campo INT COMMENT 'ID del campo del que depende visibilidad',
    depende_de_valor VARCHAR(255),
    acepta_multiples BOOLEAN DEFAULT FALSE COMMENT 'Para fotos/archivos múltiples',
    max_archivos INT DEFAULT 1,
    tipos_archivo_permitidos VARCHAR(255) DEFAULT '.jpg,.jpeg,.png,.pdf',
    tamaño_max_mb DECIMAL(5,2) DEFAULT 5.00,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_campo_formulario FOREIGN KEY (formulario_id) 
        REFERENCES cat_formularios(formulario_id) ON DELETE CASCADE,
    INDEX idx_formulario (formulario_id),
    INDEX idx_orden (formulario_id, posicion_orden),
    INDEX idx_seccion (formulario_id, seccion)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### Tabla: cat_opciones_campo
Opciones disponibles para campos dropdown, radio y checkbox.

```sql
CREATE TABLE cat_opciones_campo (
    opcion_id INT PRIMARY KEY AUTO_INCREMENT,
    campo_id INT NOT NULL,
    valor_opcion VARCHAR(255) NOT NULL COMMENT 'Valor que se guarda',
    etiqueta_opcion VARCHAR(255) NOT NULL COMMENT 'Texto visible',
    descripcion_opcion TEXT,
    icono VARCHAR(50),
    color VARCHAR(7),
    posicion_orden INT DEFAULT 0,
    es_default BOOLEAN DEFAULT FALSE,
    activo BOOLEAN DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_opcion_campo FOREIGN KEY (campo_id) 
        REFERENCES cat_campos_formulario(campo_id) ON DELETE CASCADE,
    INDEX idx_campo (campo_id),
    INDEX idx_orden (campo_id, posicion_orden)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### Tabla: cat_plantilla_pdf
Plantillas HTML/CSS para generación de PDFs con formato similar al original.

```sql
CREATE TABLE cat_plantilla_pdf (
    plantilla_id INT PRIMARY KEY AUTO_INCREMENT,
    nombre_plantilla VARCHAR(100) NOT NULL,
    descripcion TEXT,
    formulario_id INT COMMENT 'NULL = plantilla genérica',
    contenido_html LONGTEXT COMMENT 'Template HTML con placeholders {{campo}}',
    estilos_css TEXT COMMENT 'CSS para formato del PDF',
    encabezado_html TEXT,
    pie_pagina_html TEXT,
    logo_url VARCHAR(500),
    orientacion ENUM('portrait', 'landscape') DEFAULT 'portrait',
    tamaño_pagina ENUM('letter', 'legal', 'a4') DEFAULT 'letter',
    margenes_json VARCHAR(200) DEFAULT '{"top":20,"right":15,"bottom":20,"left":15}',
    activo BOOLEAN DEFAULT TRUE,
    es_default BOOLEAN DEFAULT FALSE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    creado_por INT NOT NULL,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modificado_por INT,
    CONSTRAINT fk_plantilla_formulario FOREIGN KEY (formulario_id) 
        REFERENCES cat_formularios(formulario_id) ON DELETE SET NULL,
    UNIQUE KEY uk_nombre_plantilla (nombre_plantilla),
    INDEX idx_formulario (formulario_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### Tabla: op_fallo_formulario
Asignación de formularios a fallos (relación parent-child) - Tabla OPERACIONAL.

```sql
CREATE TABLE op_fallo_formulario (
    fallo_formulario_id INT PRIMARY KEY AUTO_INCREMENT,
    fallo_id INT NOT NULL COMMENT 'ID del documento de fallo en op_documentos',
    formulario_id INT NOT NULL,
    entidad_id INT NOT NULL,
    sub_entidad_id INT,
    proveedor_id INT,
    colaborador_id INT,
    usuario_asignado INT COMMENT 'Usuario responsable de llenar',
    fecha_asignacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    asignado_por INT NOT NULL,
    fecha_limite DATE,
    prioridad ENUM('baja', 'media', 'alta', 'urgente') DEFAULT 'media',
    estado ENUM('pendiente', 'en_proceso', 'completado', 'cancelado') DEFAULT 'pendiente',
    notas_asignacion TEXT,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    creado_por INT NOT NULL,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modificado_por INT,
    CONSTRAINT fk_fallo_doc FOREIGN KEY (fallo_id) 
        REFERENCES op_documentos(Id) ON DELETE CASCADE,
    CONSTRAINT fk_fallo_form FOREIGN KEY (formulario_id) 
        REFERENCES cat_formularios(formulario_id) ON DELETE RESTRICT,
    UNIQUE KEY uk_fallo_formulario (fallo_id, formulario_id),
    INDEX idx_fallo (fallo_id),
    INDEX idx_formulario (formulario_id),
    INDEX idx_estado (estado)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### Tabla: op_respuesta_formulario
Instancia de captura/respuesta de un formulario - Tabla OPERACIONAL.

```sql
CREATE TABLE op_respuesta_formulario (
    respuesta_id INT PRIMARY KEY AUTO_INCREMENT,
    fallo_formulario_id INT NOT NULL,
    usuario_responsable INT NOT NULL,
    estado_respuesta ENUM(
        'nuevo', 'en_proceso', 'pausado', 'en_revision',
        'completado', 'rechazado', 'archivado'
    ) DEFAULT 'nuevo',
    porcentaje_completado DECIMAL(5,2) DEFAULT 0.00,
    campos_completados INT DEFAULT 0,
    campos_totales INT DEFAULT 0,
    campos_requeridos_completados INT DEFAULT 0,
    campos_requeridos_totales INT DEFAULT 0,
    fecha_inicio_llenado TIMESTAMP NULL,
    fecha_ultimo_guardado TIMESTAMP NULL,
    fecha_fin_llenado TIMESTAMP NULL,
    tiempo_llenado_segundos INT DEFAULT 0,
    tipo_dispositivo ENUM('web', 'movil', 'escritorio', 'tablet') DEFAULT 'web',
    sistema_operativo VARCHAR(50),
    version_app VARCHAR(20),
    ip_address VARCHAR(45),
    ubicacion_latitud DECIMAL(10,8),
    ubicacion_longitud DECIMAL(11,8),
    sincronizado BOOLEAN DEFAULT TRUE,
    fecha_sincronizacion TIMESTAMP NULL,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    creado_por INT NOT NULL,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modificado_por INT,
    CONSTRAINT fk_respuesta_fallo_form FOREIGN KEY (fallo_formulario_id) 
        REFERENCES op_fallo_formulario(fallo_formulario_id) ON DELETE CASCADE,
    INDEX idx_fallo_formulario (fallo_formulario_id),
    INDEX idx_estado (estado_respuesta),
    INDEX idx_usuario (usuario_responsable)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### Tabla: op_respuesta_campo
Valores de respuesta para cada campo del formulario - Tabla OPERACIONAL.

```sql
CREATE TABLE op_respuesta_campo (
    respuesta_campo_id INT PRIMARY KEY AUTO_INCREMENT,
    respuesta_id INT NOT NULL,
    campo_id INT NOT NULL,
    valor_texto TEXT COMMENT 'Valor para campos de texto',
    valor_numero DECIMAL(18,4) COMMENT 'Valor para campos numéricos',
    valor_fecha DATETIME COMMENT 'Valor para campos de fecha/hora',
    valor_booleano BOOLEAN COMMENT 'Valor para checkboxes',
    valor_json TEXT COMMENT 'Valor para campos complejos',
    valor_archivo_nombre VARCHAR(255),
    valor_archivo_ruta_local VARCHAR(500) COMMENT 'Ruta local en dispositivo móvil',
    valor_archivo_url_azure VARCHAR(500) COMMENT 'URL en Azure Blob Storage',
    valor_archivo_contenedor VARCHAR(100),
    valor_archivo_blob_name VARCHAR(255),
    valor_archivo_tamaño_bytes BIGINT,
    valor_archivo_tipo_mime VARCHAR(100),
    valor_firma_ruta_local VARCHAR(500),
    valor_firma_url_azure VARCHAR(500),
    ubicacion_latitud DECIMAL(10,8),
    ubicacion_longitud DECIMAL(11,8),
    ubicacion_direccion VARCHAR(500),
    es_valido BOOLEAN DEFAULT TRUE,
    mensaje_validacion VARCHAR(500),
    capturado_offline BOOLEAN DEFAULT FALSE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT fk_resp_campo_respuesta FOREIGN KEY (respuesta_id) 
        REFERENCES op_respuesta_formulario(respuesta_id) ON DELETE CASCADE,
    CONSTRAINT fk_resp_campo_campo FOREIGN KEY (campo_id) 
        REFERENCES cat_campos_formulario(campo_id) ON DELETE RESTRICT,
    UNIQUE KEY uk_respuesta_campo (respuesta_id, campo_id),
    INDEX idx_respuesta (respuesta_id),
    INDEX idx_campo (campo_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### Tabla: op_documento_formulario_pdf
PDFs generados de formularios completados - Tabla OPERACIONAL.

```sql
CREATE TABLE op_documento_formulario_pdf (
    documento_pdf_id INT PRIMARY KEY AUTO_INCREMENT,
    respuesta_id INT NOT NULL,
    fallo_id INT NOT NULL COMMENT 'Referencia al documento padre',
    nombre_archivo VARCHAR(255) NOT NULL,
    ruta_local_pdf VARCHAR(500),
    url_pdf_azure VARCHAR(500),
    contenedor_azure VARCHAR(100) DEFAULT 'formularios-pdf',
    blob_name_azure VARCHAR(255),
    tamaño_bytes BIGINT,
    numero_paginas INT DEFAULT 1,
    hash_md5 VARCHAR(32),
    estado_almacenamiento ENUM(
        'pendiente', 'generando', 'subiendo', 'completado', 'error'
    ) DEFAULT 'pendiente',
    intentos_generacion INT DEFAULT 0,
    error_generacion TEXT,
    fecha_generacion TIMESTAMP NULL,
    fecha_almacenamiento TIMESTAMP NULL,
    generado_por INT,
    plantilla_pdf VARCHAR(100) COMMENT 'Nombre de la plantilla usada',
    incluye_fotos BOOLEAN DEFAULT TRUE,
    incluye_firma BOOLEAN DEFAULT TRUE,
    fecha_creacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    creado_por INT NOT NULL,
    fecha_modificacion TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    modificado_por INT,
    CONSTRAINT fk_pdf_respuesta FOREIGN KEY (respuesta_id) 
        REFERENCES op_respuesta_formulario(respuesta_id) ON DELETE CASCADE,
    CONSTRAINT fk_pdf_fallo FOREIGN KEY (fallo_id) 
        REFERENCES op_documentos(Id) ON DELETE CASCADE,
    UNIQUE KEY uk_respuesta_pdf (respuesta_id),
    INDEX idx_fallo (fallo_id),
    INDEX idx_estado (estado_almacenamiento)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

### Especificaciones API (CrudController Dinámico)

#### GET - Obtener Formularios Activos para Móvil

```
GET /api/Crud/GetRegistros?strQuery=SELECT * FROM op_formularios WHERE estado='activo' AND plataformas LIKE '%movil%' LIMIT 20
```

**Respuesta:**
```json
[
  {
    "Campos": {
      "formulario_id": {"Valor": 1, "Tipo": "Int32"},
      "nombre_formulario": {"Valor": "Formulario de Fallo", "Tipo": "String"},
      "descripcion": {"Valor": "Captura de información de fallos", "Tipo": "String"},
      "plataformas": {"Valor": "movil,web", "Tipo": "String"},
      "estado": {"Valor": "activo", "Tipo": "String"},
      "version": {"Valor": 1, "Tipo": "Int32"}
    }
  }
]
```

#### GET - Obtener Campos de un Formulario

```
GET /api/Crud/GetRegistros?strQuery=SELECT * FROM op_campos_formulario WHERE formulario_id=1 ORDER BY posicion_orden ASC
```

#### POST - Crear Formulario

```
POST /api/Crud/Post?table=op_formularios
Content-Type: application/json

{
  "Campos": {
    "nombre_formulario": {"Valor": "Formulario de Captura", "Tipo": "string"},
    "descripcion": {"Valor": "Captura de información de campo", "Tipo": "string"},
    "plataformas": {"Valor": "movil", "Tipo": "string"},
    "estado": {"Valor": "activo", "Tipo": "string"},
    "version": {"Valor": 1, "Tipo": "int32"},
    "creado_por": {"Valor": 1, "Tipo": "int32"}
  }
}
```

#### POST - Crear Respuesta de Formulario

```
POST /api/Crud/Post?table=op_respuesta_formulario
Content-Type: application/json

{
  "Campos": {
    "fallo_formulario_id": {"Valor": 1, "Tipo": "int32"},
    "usuario_responsable": {"Valor": 10, "Tipo": "int32"},
    "estado_respuesta": {"Valor": "nuevo", "Tipo": "string"},
    "tipo_dispositivo": {"Valor": "movil", "Tipo": "string"},
    "creado_por": {"Valor": 10, "Tipo": "int32"}
  }
}
```

#### PUT - Actualizar Estado de Respuesta

```
PUT /api/Crud/op_respuesta_formulario/100
Content-Type: application/json

{
  "Campos": {
    "estado_respuesta": {"Valor": "completado", "Tipo": "string"},
    "fecha_fin_llenado": {"Valor": "2025-12-30T15:30:00Z", "Tipo": "datetime"},
    "tiempo_llenado_segundos": {"Valor": 300, "Tipo": "int32"},
    "porcentaje_completado": {"Valor": 100, "Tipo": "int32"}
  }
}
```

### Componentes del Portal Web

#### FormulariosDinamicos.aspx
Página para gestión de formularios dinámicos.

**Componentes DevExpress:**
- ASPxGridView para listado de formularios
- ASPxPopupControl para crear/editar formularios
- ASPxPageControl con pestañas: Campos, Opciones, Plantilla PDF
- ASPxGridView anidado para campos del formulario
- ASPxUploadControl para subir PDF de plantilla

**Funcionalidades:**
- CRUD de formularios
- Extracción de campos desde PDF con Azure Document Intelligence
- Configuración de campos con drag & drop para ordenar
- Configuración de secciones para agrupación visual
- Editor de plantilla HTML para generación de PDF
- Vista previa del formulario

#### DocumentIntelligenceService.vb
Servicio para extracción de campos desde PDF.

```vb
Imports Azure.AI.FormRecognizer.DocumentAnalysis

Public Class DocumentIntelligenceService
    Private _client As DocumentAnalysisClient
    
    Public Sub New()
        Dim endpoint = ConfigurationManager.AppSettings("AzureDocIntelEndpoint")
        Dim apiKey = ConfigurationManager.AppSettings("AzureDocIntelKey")
        _client = New DocumentAnalysisClient(New Uri(endpoint), New AzureKeyCredential(apiKey))
    End Sub
    
    ''' <summary>
    ''' Extrae campos de un PDF sin almacenarlo en Blob Storage
    ''' </summary>
    Public Async Function ExtraerCamposDePDF(archivo As Stream) As Task(Of List(Of CampoDetectadoDTO))
        Dim campos As New List(Of CampoDetectadoDTO)
        
        ' Analizar documento con Document Intelligence
        Dim operation = Await _client.AnalyzeDocumentAsync(
            WaitUntil.Completed,
            "prebuilt-document",
            archivo
        )
        
        Dim result = operation.Value
        Dim orden As Integer = 1
        
        ' Extraer campos detectados (key-value pairs)
        For Each kvp In result.KeyValuePairs
            If kvp.Key IsNot Nothing Then
                campos.Add(New CampoDetectadoDTO With {
                    .NombreCampo = LimpiarNombreCampo(kvp.Key.Content),
                    .EtiquetaCampo = kvp.Key.Content,
                    .TipoSugerido = InferirTipoCampo(kvp.Value?.Content),
                    .ValorEjemplo = kvp.Value?.Content,
                    .PosicionOrden = orden,
                    .Confianza = kvp.Confidence
                })
                orden += 1
            End If
        Next
        
        ' Extraer campos de tablas si existen
        For Each tabla In result.Tables
            For Each celda In tabla.Cells.Where(Function(c) c.Kind = "columnHeader")
                campos.Add(New CampoDetectadoDTO With {
                    .NombreCampo = LimpiarNombreCampo(celda.Content),
                    .EtiquetaCampo = celda.Content,
                    .TipoSugerido = "texto",
                    .PosicionOrden = orden,
                    .Seccion = "Tabla"
                })
                orden += 1
            Next
        Next
        
        Return campos
    End Function
    
    Private Function InferirTipoCampo(valor As String) As String
        If String.IsNullOrEmpty(valor) Then Return "texto"
        
        ' Detectar tipo por contenido
        If Decimal.TryParse(valor, Nothing) Then Return "numero"
        If DateTime.TryParse(valor, Nothing) Then Return "fecha"
        If valor.Contains("@") Then Return "email"
        If Regex.IsMatch(valor, "^\d{10}$") Then Return "telefono"
        
        Return "texto"
    End Function
    
    Private Function LimpiarNombreCampo(nombre As String) As String
        ' Convertir a snake_case sin caracteres especiales
        Return Regex.Replace(nombre.ToLower().Trim(), "[^a-z0-9]+", "_").Trim("_"c)
    End Function
End Class

Public Class CampoDetectadoDTO
    Public Property NombreCampo As String
    Public Property EtiquetaCampo As String
    Public Property TipoSugerido As String
    Public Property ValorEjemplo As String
    Public Property PosicionOrden As Integer
    Public Property Seccion As String
    Public Property Confianza As Single?
End Class
```

#### FormularioService.vb
Servicio para gestión de formularios.

```vb
Public Interface IFormularioService
    Function GetFormulariosActivos(plataforma As String) As List(Of FormularioDTO)
    Function GetFormularioById(formularioId As Integer) As FormularioDTO
    Function GetCamposFormulario(formularioId As Integer) As List(Of CampoFormularioDTO)
    Function GetOpcionesCampo(campoId As Integer) As List(Of OpcionCampoDTO)
    Function CrearFormularioDesdeExtraccion(nombre As String, campos As List(Of CampoDetectadoDTO)) As Integer
    Function GuardarPlantillaPdf(formularioId As Integer, htmlTemplate As String, cssStyles As String) As Boolean
End Interface
```

#### FormularioRenderService.vb
Servicio para renderizado dinámico de formularios en Web.

```vb
Public Class FormularioRenderService
    
    ''' <summary>
    ''' Renderiza un formulario dinámicamente usando DevExpress ASPxFormLayout
    ''' </summary>
    Public Sub RenderizarFormulario(container As Control, formularioId As Integer)
        Dim campos = _formularioService.GetCamposFormulario(formularioId)
        Dim formLayout As New ASPxFormLayout()
        formLayout.ID = "formDinamico"
        formLayout.ColCount = 2
        
        Dim seccionActual As String = ""
        Dim grupoActual As LayoutGroup = Nothing
        
        For Each campo In campos.OrderBy(Function(c) c.PosicionOrden)
            ' Crear nuevo grupo si cambió la sección
            If campo.Seccion <> seccionActual Then
                seccionActual = campo.Seccion
                grupoActual = New LayoutGroup()
                grupoActual.Caption = If(String.IsNullOrEmpty(seccionActual), "General", seccionActual)
                grupoActual.ColCount = 2
                formLayout.Items.Add(grupoActual)
            End If
            
            ' Crear item según tipo de campo
            Dim item As New LayoutItem()
            item.Caption = campo.EtiquetaCampo
            item.ColSpan = If(campo.AnchoColumna >= 12, 2, 1)
            
            Select Case campo.TipoCampo
                Case "texto"
                    item.NestedControlID = CrearASPxTextBox(container, campo)
                Case "numero", "decimal"
                    item.NestedControlID = CrearASPxSpinEdit(container, campo)
                Case "fecha"
                    item.NestedControlID = CrearASPxDateEdit(container, campo)
                Case "fecha_hora"
                    item.NestedControlID = CrearASPxDateEdit(container, campo, True)
                Case "dropdown"
                    item.NestedControlID = CrearASPxComboBox(container, campo)
                Case "checkbox"
                    item.NestedControlID = CrearASPxCheckBox(container, campo)
                Case "radio"
                    item.NestedControlID = CrearASPxRadioButtonList(container, campo)
                Case "textarea"
                    item.NestedControlID = CrearASPxMemo(container, campo)
                Case "foto"
                    item.NestedControlID = CrearASPxUploadControl(container, campo)
                Case "firma"
                    item.NestedControlID = CrearControlFirma(container, campo)
                Case "separador"
                    ' Solo agregar separador visual
                    Continue For
                Case "titulo_seccion"
                    ' Ya manejado por secciones
                    Continue For
            End Select
            
            If grupoActual IsNot Nothing Then
                grupoActual.Items.Add(item)
            Else
                formLayout.Items.Add(item)
            End If
        Next
        
        container.Controls.Add(formLayout)
    End Sub
    
    Private Function CrearASPxTextBox(container As Control, campo As CampoFormularioDTO) As String
        Dim txt As New ASPxTextBox()
        txt.ID = "txt_" & campo.NombreCampo
        txt.Width = Unit.Percentage(100)
        txt.NullText = campo.Placeholder
        txt.MaxLength = If(campo.LongitudMaxima, 500)
        
        If campo.EsRequerido Then
            txt.ValidationSettings.RequiredField.IsRequired = True
            txt.ValidationSettings.RequiredField.ErrorText = "Campo requerido"
        End If
        
        container.Controls.Add(txt)
        Return txt.ID
    End Function
    
    ' ... otros métodos CrearASPx* similares
End Class
```

#### PdfGeneratorService.vb
Servicio para generación de PDFs desde formularios completados.

```vb
Imports SelectPdf

Public Class PdfGeneratorService
    
    ''' <summary>
    ''' Genera PDF usando plantilla HTML + datos capturados
    ''' </summary>
    Public Function GenerarPdfFormulario(respuestaId As Integer) As Byte()
        ' 1. Obtener datos de la respuesta
        Dim respuesta = ObtenerRespuestaCompleta(respuestaId)
        Dim plantilla = ObtenerPlantillaPdf(respuesta.FormularioId)
        
        ' 2. Construir HTML con datos
        Dim html As String = plantilla.ContenidoHtml
        
        ' Reemplazar placeholders generales
        html = html.Replace("{{formulario_nombre}}", respuesta.NombreFormulario)
        html = html.Replace("{{fecha_generacion}}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
        html = html.Replace("{{folio}}", respuesta.Folio)
        html = html.Replace("{{usuario}}", respuesta.NombreUsuario)
        
        ' Reemplazar valores de campos
        For Each campo In respuesta.Campos
            Dim placeholder = "{{" & campo.NombreCampo & "}}"
            Dim valor As String = ""
            
            Select Case campo.TipoCampo
                Case "foto"
                    If Not String.IsNullOrEmpty(campo.UrlAzure) Then
                        valor = $"<img src='{campo.UrlAzure}' style='max-width:300px; max-height:200px;'/>"
                    End If
                Case "firma"
                    If Not String.IsNullOrEmpty(campo.UrlFirmaAzure) Then
                        valor = $"<img src='{campo.UrlFirmaAzure}' style='max-width:400px; border:1px solid #000; padding:5px;'/>"
                    End If
                Case "fecha"
                    If campo.ValorFecha.HasValue Then
                        valor = campo.ValorFecha.Value.ToString("dd/MM/yyyy")
                    End If
                Case "fecha_hora"
                    If campo.ValorFecha.HasValue Then
                        valor = campo.ValorFecha.Value.ToString("dd/MM/yyyy HH:mm")
                    End If
                Case "checkbox"
                    valor = If(campo.ValorBooleano = True, "☑ Sí", "☐ No")
                Case Else
                    valor = If(campo.ValorTexto, campo.ValorNumero?.ToString(), "")
            End Select
            
            html = html.Replace(placeholder, valor)
        Next
        
        ' 3. Agregar estilos CSS
        If Not String.IsNullOrEmpty(plantilla.EstilosCss) Then
            html = html.Replace("</head>", $"<style>{plantilla.EstilosCss}</style></head>")
        End If
        
        ' 4. Convertir HTML a PDF
        Dim converter As New HtmlToPdf()
        converter.Options.PdfPageSize = PdfPageSize.Letter
        converter.Options.PdfPageOrientation = If(plantilla.Orientacion = "landscape", 
            PdfPageOrientation.Landscape, PdfPageOrientation.Portrait)
        converter.Options.MarginTop = 20
        converter.Options.MarginBottom = 20
        converter.Options.MarginLeft = 15
        converter.Options.MarginRight = 15
        
        ' Encabezado y pie de página
        If Not String.IsNullOrEmpty(plantilla.EncabezadoHtml) Then
            converter.Options.DisplayHeader = True
            converter.Header.Height = 30
            converter.Header.DisplayOnFirstPage = True
            converter.Header.DisplayOnOddPages = True
            converter.Header.DisplayOnEvenPages = True
            
            Dim headerHtml As New PdfHtmlSection(plantilla.EncabezadoHtml, "")
            converter.Header.Add(headerHtml)
        End If
        
        If Not String.IsNullOrEmpty(plantilla.PiePaginaHtml) Then
            converter.Options.DisplayFooter = True
            converter.Footer.Height = 30
            
            Dim footerHtml As New PdfHtmlSection(
                plantilla.PiePaginaHtml.Replace("{{page}}", "{page_number}").Replace("{{pages}}", "{total_pages}"), "")
            converter.Footer.Add(footerHtml)
        End If
        
        Dim doc = converter.ConvertHtmlString(html)
        Dim pdfBytes = doc.Save()
        doc.Close()
        
        Return pdfBytes
    End Function
End Class
```

### Componentes App Móvil (MAUI)

#### FormularioDinamicoPage.xaml.cs
Página para renderizado dinámico de formularios en MAUI.

```csharp
public partial class FormularioDinamicoPage : ContentPage
{
    private readonly IFormularioService _formularioService;
    private readonly IRespuestaService _respuestaService;
    private readonly Dictionary<int, View> _controlesCaptura = new();
    private int _respuestaId;
    
    public FormularioDinamicoPage(int falloFormularioId)
    {
        InitializeComponent();
        CargarFormulario(falloFormularioId);
    }
    
    private async void CargarFormulario(int falloFormularioId)
    {
        var campos = await _formularioService.GetCamposFormularioAsync(falloFormularioId);
        RenderizarFormulario(campos);
    }
    
    /// <summary>
    /// Renderiza el formulario dinámicamente usando controles nativos MAUI
    /// </summary>
    private void RenderizarFormulario(List<CampoFormularioDTO> campos)
    {
        var contenedor = new VerticalStackLayout { Spacing = 10, Padding = 15 };
        string seccionActual = "";
        
        foreach (var campo in campos.OrderBy(c => c.PosicionOrden))
        {
            // Crear nueva sección si cambió
            if (!string.IsNullOrEmpty(campo.Seccion) && campo.Seccion != seccionActual)
            {
                seccionActual = campo.Seccion;
                contenedor.Children.Add(new Label 
                { 
                    Text = seccionActual, 
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 18,
                    Margin = new Thickness(0, 15, 0, 5)
                });
                contenedor.Children.Add(new BoxView 
                { 
                    HeightRequest = 1, 
                    Color = Colors.Gray,
                    Margin = new Thickness(0, 0, 0, 10)
                });
            }
            
            // Agregar etiqueta del campo
            var etiqueta = new Label 
            { 
                Text = campo.EtiquetaCampo + (campo.EsRequerido ? " *" : ""),
                FontSize = 14,
                TextColor = Colors.DarkGray
            };
            contenedor.Children.Add(etiqueta);
            
            // Crear control según tipo
            View control = campo.TipoCampo switch
            {
                "texto" => CrearEntryTexto(campo),
                "numero" => CrearEntryNumerico(campo),
                "decimal" => CrearEntryDecimal(campo),
                "fecha" => CrearDatePicker(campo),
                "fecha_hora" => CrearDateTimePicker(campo),
                "hora" => CrearTimePicker(campo),
                "dropdown" => CrearPicker(campo),
                "radio" => CrearRadioGroup(campo),
                "checkbox" => CrearCheckBox(campo),
                "textarea" => CrearEditor(campo),
                "foto" => CrearBotonFoto(campo),
                "firma" => CrearPadFirma(campo),
                "email" => CrearEntryEmail(campo),
                "telefono" => CrearEntryTelefono(campo),
                "ubicacion" => CrearBotonUbicacion(campo),
                _ => new Label { Text = $"Tipo no soportado: {campo.TipoCampo}" }
            };
            
            _controlesCaptura[campo.CampoId] = control;
            contenedor.Children.Add(control);
        }
        
        // Botón de guardar
        var btnGuardar = new Button
        {
            Text = "Guardar y Sincronizar",
            BackgroundColor = Colors.Green,
            TextColor = Colors.White,
            Margin = new Thickness(0, 20, 0, 0)
        };
        btnGuardar.Clicked += async (s, e) => await GuardarYSincronizar();
        contenedor.Children.Add(btnGuardar);
        
        FormularioContainer.Content = new ScrollView { Content = contenedor };
    }
    
    private Entry CrearEntryTexto(CampoFormularioDTO campo)
    {
        return new Entry
        {
            Placeholder = campo.Placeholder ?? "Ingrese texto...",
            MaxLength = campo.LongitudMaxima ?? 500,
            Keyboard = Keyboard.Default
        };
    }
    
    private Entry CrearEntryNumerico(CampoFormularioDTO campo)
    {
        return new Entry
        {
            Placeholder = campo.Placeholder ?? "0",
            Keyboard = Keyboard.Numeric
        };
    }
    
    private DatePicker CrearDatePicker(CampoFormularioDTO campo)
    {
        return new DatePicker
        {
            Format = "dd/MM/yyyy",
            MinimumDate = DateTime.Now.AddYears(-10),
            MaximumDate = DateTime.Now.AddYears(1)
        };
    }
    
    private Picker CrearPicker(CampoFormularioDTO campo)
    {
        var picker = new Picker { Title = "Seleccione..." };
        foreach (var opcion in campo.Opciones ?? new List<OpcionCampoDTO>())
        {
            picker.Items.Add(opcion.EtiquetaOpcion);
        }
        return picker;
    }
    
    private View CrearBotonFoto(CampoFormularioDTO campo)
    {
        var stack = new VerticalStackLayout { Spacing = 5 };
        var imagen = new Image { HeightRequest = 150, Aspect = Aspect.AspectFit };
        var btnCapturar = new Button 
        { 
            Text = "📷 Tomar Foto",
            BackgroundColor = Colors.Blue,
            TextColor = Colors.White
        };
        
        btnCapturar.Clicked += async (s, e) =>
        {
            var foto = await MediaPicker.CapturePhotoAsync();
            if (foto != null)
            {
                // Comprimir y guardar localmente
                var stream = await foto.OpenReadAsync();
                var rutaLocal = await GuardarFotoLocal(campo.CampoId, stream);
                imagen.Source = ImageSource.FromFile(rutaLocal);
            }
        };
        
        stack.Children.Add(btnCapturar);
        stack.Children.Add(imagen);
        return stack;
    }
    
    private View CrearPadFirma(CampoFormularioDTO campo)
    {
        var stack = new VerticalStackLayout { Spacing = 5 };
        
        // Usar SignaturePad de la comunidad MAUI o implementación custom
        var signaturePad = new GraphicsView
        {
            HeightRequest = 150,
            BackgroundColor = Colors.White
        };
        
        var btnLimpiar = new Button { Text = "Limpiar Firma" };
        
        stack.Children.Add(new Label { Text = "Firme aquí:", FontSize = 12 });
        stack.Children.Add(new Border 
        { 
            Stroke = Colors.Gray,
            StrokeThickness = 1,
            Content = signaturePad
        });
        stack.Children.Add(btnLimpiar);
        
        return stack;
    }
    
    private async Task GuardarYSincronizar()
    {
        try
        {
            // 1. Recopilar valores de todos los controles
            var respuestas = new List<RespuestaCampoDTO>();
            foreach (var kvp in _controlesCaptura)
            {
                var valor = ObtenerValorControl(kvp.Value);
                respuestas.Add(new RespuestaCampoDTO
                {
                    CampoId = kvp.Key,
                    ValorTexto = valor
                });
            }
            
            // 2. Guardar en SQLite local
            await _respuestaService.GuardarRespuestasLocalAsync(_respuestaId, respuestas);
            
            // 3. Intentar sincronizar si hay conexión
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                await _respuestaService.SincronizarAsync(_respuestaId);
                await DisplayAlert("Éxito", "Formulario guardado y sincronizado", "OK");
            }
            else
            {
                await DisplayAlert("Guardado", "Formulario guardado localmente. Se sincronizará cuando haya conexión.", "OK");
            }
            
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
    
    private string ObtenerValorControl(View control)
    {
        return control switch
        {
            Entry entry => entry.Text,
            Editor editor => editor.Text,
            DatePicker datePicker => datePicker.Date.ToString("yyyy-MM-dd"),
            TimePicker timePicker => timePicker.Time.ToString(),
            Picker picker => picker.SelectedItem?.ToString(),
            CheckBox checkBox => checkBox.IsChecked.ToString(),
            _ => ""
        };
    }
}
```

#### FormularioSyncService.cs
Servicio para sincronización de formularios con el servidor.

```csharp
public class FormularioSyncService : IFormularioSyncService
{
    private readonly IApiService _apiService;
    private readonly IBlobStorageService _blobService;
    private readonly ILocalDatabase _localDb;
    
    /// <summary>
    /// Sincroniza una respuesta de formulario completa con el servidor
    /// </summary>
    public async Task<SyncResult> SincronizarRespuestaAsync(int respuestaIdLocal)
    {
        var result = new SyncResult();
        
        try
        {
            // 1. Obtener respuesta local
            var respuestaLocal = await _localDb.GetRespuestaAsync(respuestaIdLocal);
            var camposLocales = await _localDb.GetRespuestasCampoAsync(respuestaIdLocal);
            
            // 2. Crear respuesta en servidor si no existe
            if (!respuestaLocal.ServerId.HasValue)
            {
                var serverResponse = await _apiService.PostAsync<RespuestaFormularioDTO>(
                    "op_respuesta_formulario", respuestaLocal);
                respuestaLocal.ServerId = serverResponse.RespuestaId;
                await _localDb.UpdateRespuestaAsync(respuestaLocal);
            }
            
            // 3. Subir fotos a Azure Blob Storage
            foreach (var campo in camposLocales.Where(c => !string.IsNullOrEmpty(c.RutaArchivoLocal)))
            {
                if (string.IsNullOrEmpty(campo.UrlAzure))
                {
                    var blobUrl = await _blobService.UploadFileAsync(
                        "formularios-fotos",
                        campo.RutaArchivoLocal,
                        $"respuesta_{respuestaLocal.ServerId}/campo_{campo.CampoId}.jpg"
                    );
                    campo.UrlAzure = blobUrl;
                }
            }
            
            // 4. Subir firma a Azure Blob Storage
            foreach (var campo in camposLocales.Where(c => !string.IsNullOrEmpty(c.RutaFirmaLocal)))
            {
                if (string.IsNullOrEmpty(campo.UrlFirmaAzure))
                {
                    var blobUrl = await _blobService.UploadFileAsync(
                        "formularios-firmas",
                        campo.RutaFirmaLocal,
                        $"respuesta_{respuestaLocal.ServerId}/firma.png"
                    );
                    campo.UrlFirmaAzure = blobUrl;
                }
            }
            
            // 5. Guardar respuestas de campos en servidor
            foreach (var campo in camposLocales)
            {
                await _apiService.PostAsync<RespuestaCampoDTO>(
                    "op_respuesta_campo", 
                    new {
                        respuesta_id = respuestaLocal.ServerId,
                        campo_id = campo.CampoId,
                        valor_texto = campo.ValorTexto,
                        valor_numero = campo.ValorNumero,
                        valor_fecha = campo.ValorFecha,
                        valor_booleano = campo.ValorBooleano,
                        valor_archivo_url_azure = campo.UrlAzure,
                        valor_firma_url_azure = campo.UrlFirmaAzure
                    });
            }
            
            // 6. Marcar como completado y solicitar generación de PDF
            await _apiService.PutAsync($"op_respuesta_formulario/{respuestaLocal.ServerId}", 
                new { estado_respuesta = "completado" });
            
            // 7. Invocar generación de PDF en servidor
            await _apiService.PostAsync("formularios/generar-pdf", 
                new { respuesta_id = respuestaLocal.ServerId });
            
            // 8. Marcar como sincronizado localmente
            respuestaLocal.Sincronizado = true;
            respuestaLocal.FechaSincronizacion = DateTime.Now;
            await _localDb.UpdateRespuestaAsync(respuestaLocal);
            
            result.Success = true;
            result.Message = "Sincronización completada";
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            result.Error = ex;
        }
        
        return result;
    }
}
```

### Especificaciones API (CrudController Dinámico)
    Function CreateFormulario(formulario As FormularioDTO) As OperationResult
    Function UpdateFormulario(formulario As FormularioDTO) As OperationResult
    Function DeleteFormulario(formularioId As Integer) As OperationResult
    Function AsignarFormularioAFallo(falloId As Integer, formularioId As Integer, usuarioAsignado As Integer) As OperationResult
End Interface
```

#### RespuestaFormularioService.vb
Servicio para gestión de respuestas.

```vb
Public Interface IRespuestaFormularioService
    Function IniciarRespuesta(falloFormularioId As Integer, usuarioId As Integer, tipoDispositivo As String) As OperationResult
    Function GuardarRespuestaCampo(respuestaId As Integer, campoId As Integer, valor As String) As OperationResult
    Function GuardarFoto(respuestaId As Integer, campoId As Integer, fotoBytes As Byte()) As OperationResult
    Function GuardarFirma(respuestaId As Integer, campoId As Integer, firmaBytes As Byte()) As OperationResult
    Function CompletarRespuesta(respuestaId As Integer) As OperationResult
    Function GenerarPDF(respuestaId As Integer) As OperationResult
End Interface
```

### Componentes de App Móvil

#### Arquitectura SQLite Offline

```csharp
// Modelo local para formularios
public class LocalFormulario
{
    [PrimaryKey]
    public int FormularioId { get; set; }
    public string NombreFormulario { get; set; }
    public string Descripcion { get; set; }
    public string Plataformas { get; set; }
    public int Version { get; set; }
    public DateTime FechaSincronizacion { get; set; }
}

// Modelo local para campos
public class LocalCampoFormulario
{
    [PrimaryKey]
    public int CampoId { get; set; }
    public int FormularioId { get; set; }
    public string NombreCampo { get; set; }
    public string EtiquetaCampo { get; set; }
    public string TipoCampo { get; set; }
    public bool EsRequerido { get; set; }
    public int PosicionOrden { get; set; }
    public string ValorPorDefecto { get; set; }
    public string ValidacionesJson { get; set; }
}

// Modelo local para respuestas pendientes
public class LocalRespuestaPendiente
{
    [PrimaryKey, AutoIncrement]
    public int LocalId { get; set; }
    public int? ServerId { get; set; }
    public int FalloFormularioId { get; set; }
    public string EstadoRespuesta { get; set; }
    public string RespuestasJson { get; set; }
    public string FotosJson { get; set; }
    public string FirmaPath { get; set; }
    public bool IsSynced { get; set; }
    public string HashControl { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

#### FormularioSyncService.cs
Servicio de sincronización para formularios.

```csharp
public interface IFormularioSyncService
{
    Task<SyncResult> SyncFormulariosAsync();
    Task<SyncResult> SyncRespuestasPendientesAsync();
    Task QueueRespuestaAsync(LocalRespuestaPendiente respuesta);
    Task<List<LocalRespuestaPendiente>> GetPendingResponsesAsync();
    Task<bool> ValidateHashAsync(LocalRespuestaPendiente respuesta);
}
```

#### DynamicFormRenderer.cs
Componente para renderizado dinámico de formularios.

```csharp
public interface IDynamicFormRenderer
{
    View RenderForm(LocalFormulario formulario, List<LocalCampoFormulario> campos);
    View RenderField(LocalCampoFormulario campo);
    Task<byte[]> CapturePhotoAsync();
    Task<byte[]> CaptureSignatureAsync();
    Dictionary<int, string> GetFormValues();
    bool ValidateForm();
}
```

### Flujo N8N - Extracción de Campos desde PDF

**Trigger:** POST /webhook/formulario-pdf-upload

**Pasos del Flujo:**
1. Recibir archivo PDF
2. Validar que sea PDF válido
3. Enviar a Azure Form Recognizer
4. Procesar respuesta JSON con campos detectados
5. Crear formulario en op_formularios via API
6. Crear campos en op_campos_formulario via API
7. Retornar estructura del formulario creado

**Configuración Azure Form Recognizer:**
- Endpoint: `https://{resource}.cognitiveservices.azure.com/`
- API Key: Almacenada en Azure Key Vault
- Modelo: prebuilt-document o custom model

### Integración con Módulo 08 (Servicios Municipales)

#### Flujo de Asignación de Formulario a Fallo

```
1. Entidad crea fallo en op_documentos
2. Sistema muestra formularios disponibles (op_formularios WHERE estado='activo')
3. Entidad selecciona formulario
4. Sistema crea registro en op_fallo_formulario
5. Sistema notifica a SubEntidad/Proveedor/Colaborador asignado
```

#### Flujo de Captura en App Móvil

```
1. Colaborador abre fallo asignado
2. App descarga formulario y campos (si no están en SQLite)
3. App renderiza formulario dinámicamente
4. Colaborador captura datos, fotos, firma
5. App guarda en SQLite local
6. Al tener conexión, App sincroniza con servidor
7. Servidor genera PDF y almacena en Azure Blob
8. Servidor vincula PDF al fallo como documento hijo
```

### Relación Parent-Child de Documentos

```
Fallo (Padre)
  └─ op_fallo_formulario
       └─ op_respuesta_formulario (Hijo)
            └─ op_respuesta_campo (Nieto)
            └─ op_documento_formulario_pdf (Documento Final)
```

### Métricas y Dashboards

**Métricas de Llenado:**
- Formularios completados por día/semana/mes
- Tiempo promedio de llenado por formulario
- Porcentaje de campos completados vs requeridos
- Formularios pendientes de sincronización

**Métricas de Interacción:**
- Campos con más errores de validación
- Tiempo promedio por tipo de campo
- Uso de fotos vs firmas

**Métricas de Almacenamiento:**
- PDFs generados por período
- Espacio utilizado en Azure Blob
- Tasa de éxito de almacenamiento

### Consideraciones de Seguridad

1. **Autenticación:** Bearer Token JWT en todas las llamadas API
2. **Autorización:** Validar que usuario tiene acceso al fallo/formulario
3. **Encriptación:** HTTPS para transmisión, SQLite encriptado en móvil
4. **Auditoría:** Registrar todas las operaciones de captura y sincronización



---

## Módulo de Condominios

### Visión General

El módulo de condominios proporciona un sistema completo de administración para condominios residenciales, incluyendo gestión de residentes, cuotas, pagos, reservaciones de áreas comunes, control de visitantes y comunicados.

### Arquitectura del Módulo

```
┌─────────────────────────────────────────────────────────────┐
│                    MÓDULO CONDOMINIOS                        │
├─────────────────────────────────────────────────────────────┤
│  JERARQUÍA:                                                  │
│  cat_entidades (Nivel 1) = Condominio/Fraccionamiento       │
│  cat_subentidades (Nivel 2) = Secciones/Torres (YA EXISTE)  │
│  cat_areas_comunes (Nivel 2/3) = Áreas comunes reservables  │
│  cat_unidades = Unidades privativas por SubEntidad          │
├─────────────────────────────────────────────────────────────┤
│  CATÁLOGOS                    │  OPERACIONES                │
│  ┌─────────────────────────┐  │  ┌─────────────────────────┐│
│  │ cat_areas_comunes       │  │  │ op_cuotas               ││
│  │ cat_residentes          │  │  │ op_pagos                ││
│  │ cat_conceptos_cuota     │  │  │ op_pagos_detalle        ││
│  │ cat_unidades (mejorada) │  │  │ op_reservaciones        ││
│  └─────────────────────────┘  │  │ op_visitantes           ││
│                               │  │ op_comunicados          ││
│  NOTA: Secciones/Torres se    │  │ op_comunicados_lecturas ││
│  manejan en cat_subentidades  │  └─────────────────────────┘│
├─────────────────────────────────────────────────────────────┤
│  VISTAS                       │  STORED PROCEDURES          │
│  ┌─────────────────────────┐  │  ┌─────────────────────────┐│
│  │ vw_estado_cuenta        │  │  │ sp_GenerarCuotasMensuales│
│  │ vw_resumen_morosidad    │  │  │ sp_AplicarRecargosMora  ││
│  │ vw_calendario_reserv    │  │  │ fn_GenerarFolio         ││
│  │ vw_visitantes_activos   │  │  └─────────────────────────┘│
│  └─────────────────────────┘  │                             │
└─────────────────────────────────────────────────────────────┘
```

### Componentes e Interfaces

#### 1. Catálogo de Áreas Comunes

**Página:** `Views/Catalogos/AreasComunes.aspx`
**Tabla:** `cat_areas_comunes`

Este catálogo maneja las áreas comunes reservables del condominio. Pueden pertenecer a:
- **SubEntidadId = NULL**: Área compartida por todo el condominio
- **SubEntidadId = ID**: Área exclusiva de una torre/sección específica

**NOTA:** Las secciones/torres se manejan en `cat_subentidades` (ya existente). NO se necesita crear `cat_secciones`.

```vb
Public Class AreaComunDTO
    Public Property Id As Integer
    Public Property EntidadId As Integer
    Public Property SubEntidadId As Integer? ' NULL=compartida, ID=exclusiva de torre
    Public Property Clave As String
    Public Property Nombre As String
    Public Property TipoArea As String ' Salon, Alberca, Gimnasio, Jardin, Terraza, Asador, Ludoteca, SalaJuntas, etc.
    Public Property Descripcion As String
    Public Property Ubicacion As String
    Public Property Capacidad As Integer?
    Public Property CostoReservacion As Decimal
    Public Property DepositoRequerido As Decimal
    Public Property RequiereReservacion As Boolean
    Public Property HoraApertura As TimeSpan?
    Public Property HoraCierre As TimeSpan?
    Public Property DiasDisponibles As String
    Public Property DuracionMinimaHoras As Integer?
    Public Property DuracionMaximaHoras As Integer?
    Public Property AnticipacionMinimaDias As Integer?
    Public Property AnticipacionMaximaDias As Integer?
    Public Property Reglamento As String
    Public Property ImagenPath As String
    Public Property Activo As Boolean
    ' Campos de navegación
    Public Property SubEntidadNombre As String ' Nombre de la torre/sección (si aplica)
End Class
```

#### 2. Catálogo de Residentes

**Página:** `Views/Catalogos/Residentes.aspx`
**Tabla:** `cat_residentes`

```vb
Public Class ResidenteDTO
    Public Property Id As Integer
    Public Property EntidadId As Integer
    Public Property UnidadId As Integer?
    Public Property TipoResidente As String ' Propietario, Inquilino, Familiar, Empleado
    Public Property Nombre As String
    Public Property ApellidoPaterno As String
    Public Property ApellidoMaterno As String
    Public Property NombreCompleto As String ' Campo calculado
    Public Property Email As String
    Public Property Telefono As String
    Public Property TelefonoCelular As String
    Public Property CURP As String
    Public Property RFC As String
    Public Property FechaNacimiento As Date?
    Public Property Genero As String
    Public Property DireccionAlterna As String
    Public Property ContactoEmergencia As String
    Public Property TelefonoEmergencia As String
    Public Property FechaIngreso As Date?
    Public Property FechaSalida As Date?
    Public Property TelegramChatId As Long?
    Public Property FotoPath As String
    Public Property Observaciones As String
    Public Property Activo As Boolean
End Class
```

#### 3. Catálogo de Conceptos de Cuota

**Página:** `Views/Catalogos/ConceptosCuota.aspx`
**Tabla:** `cat_conceptos_cuota`

```vb
Public Class ConceptoCuotaDTO
    Public Property Id As Integer
    Public Property EntidadId As Integer
    Public Property Clave As String
    Public Property Nombre As String
    Public Property Descripcion As String
    Public Property TipoCuota As String ' Ordinaria, Extraordinaria, Servicio, Multa, Recargo
    Public Property MontoDefault As Decimal
    Public Property EsRecurrente As Boolean
    Public Property DiaVencimiento As Integer
    Public Property DiasGracia As Integer
    Public Property PorcentajeRecargo As Decimal
    Public Property CuentaContable As String
    Public Property Activo As Boolean
End Class
```

#### 4. Gestión de Cuotas

**Página:** `Views/Operacion/Condominios/Cuotas.aspx`
**Tabla:** `op_cuotas`

```vb
Public Class CuotaDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property EntidadId As Integer
    Public Property UnidadId As Integer
    Public Property ConceptoCuotaId As Integer
    Public Property ResidenteId As Integer?
    Public Property Periodo As String ' YYYY-MM
    Public Property Anio As Integer
    Public Property Mes As Integer
    Public Property Monto As Decimal
    Public Property Descuento As Decimal
    Public Property Recargo As Decimal
    Public Property MontoTotal As Decimal ' Calculado
    Public Property MontoPagado As Decimal
    Public Property Saldo As Decimal ' Calculado
    Public Property FechaEmision As Date
    Public Property FechaVencimiento As Date
    Public Property FechaPago As Date?
    Public Property Estado As String ' Pendiente, Parcial, Pagada, Vencida, Cancelada
    Public Property Observaciones As String
    Public Property CreadoPor As Integer?
    ' Campos de navegación
    Public Property UnidadCodigo As String
    Public Property UnidadNombre As String
    Public Property ConceptoNombre As String
    Public Property ResidenteNombre As String
End Class
```

#### 5. Gestión de Pagos

**Página:** `Views/Operacion/Condominios/Pagos.aspx`
**Tabla:** `op_pagos`, `op_pagos_detalle`

```vb
Public Class PagoDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property EntidadId As Integer
    Public Property UnidadId As Integer
    Public Property ResidenteId As Integer?
    Public Property Monto As Decimal
    Public Property FormaPago As String ' Efectivo, Transferencia, Tarjeta, Cheque, Deposito, SPEI
    Public Property Referencia As String
    Public Property Banco As String
    Public Property NumeroAutorizacion As String
    Public Property FechaPago As Date
    Public Property ComprobantePath As String
    Public Property Estado As String ' Aplicado, Pendiente, Rechazado, Cancelado
    Public Property MotivoRechazo As String
    Public Property Observaciones As String
    Public Property RegistradoPor As Integer?
    Public Property AprobadoPor As Integer?
    Public Property FechaAprobacion As DateTime?
    ' Detalle de aplicación
    Public Property Detalles As List(Of PagoDetalleDTO)
End Class

Public Class PagoDetalleDTO
    Public Property Id As Integer
    Public Property PagoId As Integer
    Public Property CuotaId As Integer
    Public Property MontoAplicado As Decimal
    ' Campos de navegación
    Public Property CuotaFolio As String
    Public Property CuotaPeriodo As String
    Public Property CuotaConcepto As String
End Class
```

#### 6. Reservaciones de Áreas Comunes

**Página:** `Views/Operacion/Condominios/Reservaciones.aspx`
**Tabla:** `op_reservaciones`

**Reglas de Acceso a Áreas Comunes:**

Las áreas comunes tienen dos niveles de pertenencia que determinan quién puede reservarlas:

1. **Áreas de la Entidad (SubEntidadId = NULL)**: Son compartidas por todo el condominio. Cualquier unidad de cualquier sub-entidad puede reservarlas.
   - Ejemplo: Alberca principal, Salón de fiestas del condominio

2. **Áreas de una SubEntidad (SubEntidadId = ID)**: Son exclusivas de una torre/sección específica. Solo las unidades que pertenecen a esa sub-entidad pueden reservarlas.
   - Ejemplo: Sala de juntas de Torre A, Terraza de Edificio B

**Validación al crear reservación:**
```vb
' Pseudocódigo de validación
Function PuedeReservarArea(unidadId As Integer, areaComunId As Integer) As Boolean
    Dim area = GetAreaComun(areaComunId)
    Dim unidad = GetUnidad(unidadId)
    
    ' Si el área es de la Entidad (compartida), cualquier unidad puede reservar
    If area.SubEntidadId Is Nothing Then
        Return True
    End If
    
    ' Si el área es exclusiva de una SubEntidad, validar que la unidad pertenezca a ella
    Return unidad.SubEntidadId = area.SubEntidadId
End Function
```

**Filtrado de áreas disponibles para reservar:**
```sql
-- Vista para obtener áreas disponibles según la sub-entidad del residente
SELECT ac.* 
FROM cat_areas_comunes ac
WHERE ac.EntidadId = @EntidadId
  AND ac.Activo = 1
  AND ac.RequiereReservacion = 1
  AND (
    ac.SubEntidadId IS NULL  -- Áreas compartidas de la Entidad
    OR ac.SubEntidadId = @SubEntidadIdDelResidente  -- Áreas exclusivas de su torre
  )
```

```vb
Public Class ReservacionDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property EntidadId As Integer
    Public Property AreaComunId As Integer ' FK a cat_areas_comunes
    Public Property UnidadId As Integer
    Public Property ResidenteId As Integer?
    Public Property FechaReservacion As Date
    Public Property HoraInicio As TimeSpan
    Public Property HoraFin As TimeSpan
    Public Property DuracionHoras As Decimal ' Calculado
    Public Property NumeroInvitados As Integer
    Public Property Motivo As String
    Public Property TipoEvento As String ' Reunion, Fiesta, Cumpleanos, Boda, Bautizo, Otro
    Public Property CostoReservacion As Decimal
    Public Property DepositoRequerido As Decimal
    Public Property DepositoPagado As Decimal
    Public Property DepositoDevuelto As Decimal
    Public Property MontoDescuento As Decimal
    Public Property Estado As String ' Pendiente, Confirmada, EnUso, Completada, Cancelada, NoShow
    Public Property MotivoRechazo As String
    Public Property MotivoCancelacion As String
    Public Property Observaciones As String
    Public Property ReglamentoAceptado As Boolean
    Public Property SolicitadoPor As Integer?
    Public Property AprobadoPor As Integer?
    Public Property FechaAprobacion As DateTime?
    ' Campos de navegación
    Public Property AreaNombre As String
    Public Property UnidadCodigo As String
    Public Property ResidenteNombre As String
End Class
```

#### 7. Control de Visitantes

**Página:** `Views/Operacion/Condominios/Visitantes.aspx`
**Tabla:** `op_visitantes`

```vb
Public Class VisitanteDTO
    Public Property Id As Integer
    Public Property Folio As String
    Public Property EntidadId As Integer
    Public Property UnidadId As Integer
    Public Property ResidenteId As Integer?
    Public Property NombreVisitante As String
    Public Property TipoIdentificacion As String ' INE, Pasaporte, Licencia, Cedula, Otro
    Public Property NumeroIdentificacion As String
    Public Property TieneVehiculo As Boolean
    Public Property PlacasVehiculo As String
    Public Property ColorVehiculo As String
    Public Property MarcaVehiculo As String
    Public Property ModeloVehiculo As String
    Public Property TipoVisita As String ' Personal, Proveedor, Delivery, Emergencia, Trabajador, Invitado
    Public Property MotivoVisita As String
    Public Property EmpresaProveedor As String
    Public Property FechaEntrada As DateTime
    Public Property FechaSalida As DateTime?
    Public Property TiempoEstancia As Integer? ' Minutos, calculado
    Public Property Estado As String ' EnCondominio, Salida, Rechazado
    Public Property MotivoRechazo As String
    Public Property AutorizadoPor As String
    Public Property TelefonoAutorizacion As String
    Public Property FotoVisitantePath As String
    Public Property FotoIdentificacionPath As String
    Public Property Observaciones As String
    Public Property RegistradoPor As Integer?
    Public Property RegistroSalidaPor As Integer?
    ' Campos de navegación
    Public Property UnidadCodigo As String
    Public Property UnidadNombre As String
End Class
```

#### 8. Comunicados

**Página:** `Views/Operacion/Condominios/Comunicados.aspx`
**Tabla:** `op_comunicados`, `op_comunicados_lecturas`

```vb
Public Class ComunicadoDTO
    Public Property Id As Integer
    Public Property EntidadId As Integer
    Public Property SubEntidadId As Integer? ' FK a cat_subentidades (si Destinatarios=Seccion)
    Public Property Titulo As String
    Public Property Contenido As String
    Public Property Resumen As String
    Public Property TipoComunicado As String ' Aviso, Urgente, Mantenimiento, Evento, Asamblea, Cobranza, General
    Public Property Prioridad As String ' Baja, Normal, Alta, Urgente
    Public Property FechaPublicacion As DateTime
    Public Property FechaExpiracion As DateTime?
    Public Property EnviarEmail As Boolean
    Public Property EnviarTelegram As Boolean
    Public Property EnviarPush As Boolean
    Public Property EnviarSMS As Boolean
    Public Property Destinatarios As String ' Todos, Propietarios, Inquilinos, Morosos, Seccion, Unidades
    Public Property UnidadesDestino As String ' IDs separados por coma
    Public Property ArchivoAdjuntoPath As String
    Public Property ImagenPath As String
    Public Property Estado As String ' Borrador, Programado, Publicado, Expirado, Cancelado
    Public Property TotalDestinatarios As Integer
    Public Property TotalLeidos As Integer
    Public Property TotalEmailEnviados As Integer
    Public Property TotalTelegramEnviados As Integer
    Public Property CreadoPor As Integer?
    Public Property PublicadoPor As Integer?
    ' Campos de navegación
    Public Property SubEntidadNombre As String ' Nombre de la torre/sección (si aplica)
End Class
```

### Flujos de Proceso

#### Flujo de Generación de Cuotas Mensuales

```
1. Administrador selecciona periodo (YYYY-MM) y entidad
2. Sistema ejecuta sp_GenerarCuotasMensuales
3. SP obtiene unidades activas de la entidad
4. SP obtiene conceptos recurrentes de la entidad
5. Para cada unidad × concepto:
   a. Verifica si ya existe cuota para el periodo
   b. Si no existe, crea registro en op_cuotas
   c. Asigna folio único CUO-YYYYMM-XXXX
   d. Calcula monto (CuotaOrdinaria de unidad o MontoDefault de concepto)
   e. Calcula fecha de vencimiento según DiaVencimiento
6. Sistema retorna cantidad de cuotas generadas
```

#### Flujo de Registro de Pago

```
1. Administrador selecciona unidad
2. Sistema muestra cuotas pendientes ordenadas por periodo (FIFO)
3. Administrador ingresa monto y forma de pago
4. Sistema crea registro en op_pagos con folio PAG-YYYYMMDD-XXXX
5. Sistema aplica pago a cuotas pendientes:
   a. Crea registros en op_pagos_detalle
   b. Trigger actualiza MontoPagado en op_cuotas
   c. Trigger actualiza Estado (Pendiente→Parcial→Pagada)
6. Si hay excedente, se registra como saldo a favor
```

#### Flujo de Reservación de Área Común

```
1. Residente selecciona área común
2. Sistema muestra calendario con disponibilidad
3. Residente selecciona fecha y horario
4. Sistema valida:
   a. Disponibilidad (no conflicto con otras reservaciones)
   b. Anticipación mínima/máxima
   c. Duración mínima/máxima
   d. Horario dentro de apertura/cierre
5. Sistema calcula costo y depósito
6. Residente confirma y acepta reglamento
7. Sistema crea reservación con estado Pendiente
8. Administrador aprueba → Estado = Confirmada
9. Sistema notifica al residente
```

#### Flujo de Control de Visitantes

```
1. Vigilante registra entrada:
   a. Captura datos del visitante
   b. Captura datos del vehículo (si aplica)
   c. Registra unidad destino y autorización
   d. Sistema asigna folio VIS-YYYYMMDD-XXXX
   e. Estado = EnCondominio
2. Visitante permanece en condominio
3. Vigilante registra salida:
   a. Busca visitante por folio o nombre
   b. Registra FechaSalida
   c. Sistema calcula TiempoEstancia
   d. Estado = Salida
```

### Propiedades de Corrección - Módulo Condominios

**Propiedad 33: Cuota generada tiene folio único**
*Para cualquier* cuota generada, el sistema debe asignar un folio único que no se repita en ninguna otra cuota del sistema.
**Valida: Requerimientos 40.2**

**Propiedad 34: Pago actualiza saldo automáticamente**
*Para cualquier* pago registrado, el sistema debe actualizar automáticamente el MontoPagado y Saldo de las cuotas afectadas mediante trigger.
**Valida: Requerimientos 41.3**

**Propiedad 35: Reservación valida disponibilidad**
*Para cualquier* reservación solicitada, el sistema debe verificar que no exista conflicto de horario con otras reservaciones del mismo área.
**Valida: Requerimientos 43.2, 43.3**

**Propiedad 36: Visitante activo aparece en lista**
*Para cualquier* visitante con Estado='EnCondominio', el sistema debe mostrarlo en la vista de visitantes activos con tiempo de estancia calculado.
**Valida: Requerimientos 45.4, 45.5**

**Propiedad 37: Comunicado notifica por canales seleccionados**
*Para cualquier* comunicado publicado, el sistema debe enviar notificaciones únicamente por los canales marcados como activos (Email, Telegram, Push, SMS).
**Valida: Requerimientos 46.2, 46.3**

**Propiedad 38: Estado de cuenta muestra saldo correcto**
*Para cualquier* unidad consultada, el estado de cuenta debe mostrar el saldo calculado correctamente como (Monto - Descuento + Recargo - MontoPagado).
**Valida: Requerimientos 42.2**

**Propiedad 39: Morosidad ordena por saldo descendente**
*Para cualquier* consulta de morosidad, el sistema debe retornar las unidades ordenadas por SaldoTotal de mayor a menor.
**Valida: Requerimientos 47.1**

**Propiedad 40: Recargo se aplica después de días de gracia**
*Para cualquier* cuota vencida, el sistema debe aplicar recargo únicamente si han transcurrido más días que DiasGracia configurado en el concepto.
**Valida: Requerimientos 39.3**

### Estructura de Páginas Web

```
Views/
├── Catalogos/
│   ├── AreasComunes.aspx        # CRUD de áreas comunes reservables
│   ├── Residentes.aspx          # CRUD de residentes
│   ├── ConceptosCuota.aspx      # CRUD de conceptos de cuota
│   └── Unidades.aspx            # Mejorar existente con nuevos campos
│
└── Operacion/
    └── Condominios/
        ├── Cuotas.aspx              # Generación y gestión de cuotas
        ├── Pagos.aspx               # Registro de pagos
        ├── EstadoCuenta.aspx        # Consulta por unidad
        ├── Reservaciones.aspx       # Gestión de reservaciones
        ├── CalendarioReservaciones.aspx  # Vista calendario
        ├── Visitantes.aspx          # Control de acceso
        └── Comunicados.aspx         # Gestión de avisos
```

**NOTA:** Las secciones/torres se gestionan en SubEntidades (ya existente).

### Scripts SQL

Ubicación: `.kiro/specs/ecosistema-jelabbc/sql/condominios/`

| Archivo | Descripción |
|---------|-------------|
| `00_ejecutar_todos.sql` | Script maestro de ejecución |
| `01_catalogos_base.sql` | Tablas de catálogos (cat_areas_comunes, cat_residentes, cat_conceptos_cuota) |
| `02_cuotas_pagos.sql` | Tablas de cuotas y pagos + triggers + vistas |
| `03_reservaciones_visitantes.sql` | Tablas de reservaciones y visitantes + vistas |
| `04_datos_iniciales.sql` | Datos iniciales + stored procedures + menú |

### Integración con Otros Módulos

- **conf_residentes_telegram**: `cat_residentes.TelegramChatId` vincula para notificaciones
- **op_tickets**: Puede usarse para tickets de mantenimiento del condominio
- **cat_entidades**: Cada condominio es una entidad con `TipoEntidad='Condominio'`
- **cat_subentidades**: Las torres/secciones del condominio (ya existente)
- **n8n**: Workflows para generación automática de cuotas y notificaciones

