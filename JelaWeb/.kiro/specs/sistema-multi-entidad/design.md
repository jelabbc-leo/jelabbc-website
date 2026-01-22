# DESIGN - Sistema Multi-Entidad con Selector

**Fecha:** 20 de Enero de 2026  
**Versión:** 1.0  
**Estado:** Listo para Implementación  
**Prioridad:** Alta

---

## 1. RESUMEN EJECUTIVO

### 1.1 Objetivo del Diseño

Diseñar e implementar un sistema multi-entidad que permita a **Administradores de Condominios** gestionar múltiples condominios desde una única cuenta, mientras que **Usuarios Internos** solo accedan a su condominio específico, con filtrado automático de datos por entidad.

### 1.2 Alcance Técnico

**Componentes Afectados:**
- Base de Datos MySQL (2 tablas modificadas, 1 tabla nueva)
- API .NET 8 (2 servicios, 1 modelo)
- Frontend VB.NET (5 helpers, 2 páginas nuevas, 1 master page)
- 10+ páginas existentes (eliminación de campo Entidad)

**Tecnologías:**
- MySQL 8.0
- .NET 8 (API)
- VB.NET 4.8.1 (Frontend)
- JWT Authentication
- ASP.NET WebForms

---

## 2. ARQUITECTURA DEL SISTEMA

### 2.1 Diagrama de Flujo de Autenticación

```
┌─────────────────────────────────────────────────────────────────┐
│                         FLUJO DE LOGIN                          │
└─────────────────────────────────────────────────────────────────┘

Usuario Ingresa Credenciales
         │
         ▼
    Ingreso.aspx.vb
         │
         ▼
    AuthService.vb ──────► JELA.API/AuthEndpoints
         │                        │
         │                        ▼
         │                 JwtAuthService.cs
         │                        │
         │                        ▼
         │                 MySQL: conf_usuarios
         │                        │
         │                        ▼
         │                 Obtener: TipoUsuario, Entidades[]
         │                        │
         ◄────────────────────────┘
         │
         ▼
SessionHelper.InitializeSession()
         │
         ▼
    ¿TipoUsuario = AdministradorCondominios?
         │
    ┌────┴────┐
    │         │
   SÍ        NO
    │         │
    ▼         ▼
SelectorEntidades.aspx    Inicio.aspx
    │                     (IdEntidadActual = su única entidad)
    │
    ▼
Usuario Selecciona Entidad
    │
    ▼
SessionHelper.SetEntidadActual()
    │
    ▼
Inicio.aspx
```

### 2.2 Diagrama de Componentes

```
┌──────────────────────────────────────────────────────────────────┐
│                        CAPA DE DATOS                             │
├──────────────────────────────────────────────────────────────────┤
│  conf_usuarios (modificada)                                      │
│  + TipoUsuario: ENUM                                             │
│  + IdEntidadPrincipal: INT                                       │
│                                                                  │
│  conf_usuario_entidades (nueva)                                  │
│  + IdUsuario, IdEntidad, EsPrincipal                             │
└──────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│                        CAPA API (.NET 8)                         │
├──────────────────────────────────────────────────────────────────┤
│  JwtAuthService.cs                                               │
│  + AuthenticateAsync() → UserInfo con Entidades[]                │
│                                                                  │
│  AuthModels.cs                                                   │
│  + UserInfo: TipoUsuario, Entidades[], IdEntidadPrincipal        │
│  + EntidadInfo: Id, Nombre, Direccion, EsPrincipal               │
└──────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│                    CAPA FRONTEND (VB.NET)                        │
├──────────────────────────────────────────────────────────────────┤
│  SessionHelper.vb                                                │
│  + GetTipoUsuario(), GetEntidades()                              │
│  + GetIdEntidadActual(), SetEntidadActual()                      │
│  + IsAdministradorCondominios()                                  │
│                                                                  │
│  EntidadHelper.vb (nuevo)                                        │
│  + AgregarFiltroEntidad(query)                                   │
│  + AgregarCampoEntidad(campos)                                   │
│  + ValidarPerteneceAEntidadActual(id, tabla)                     │
│                                                                  │
│  ApiConsumerCRUD.vb (modificado)                                 │
│  + ObtenerDatos() → Filtra por IdEntidad automáticamente         │
│  + Insertar() → Agrega IdEntidad automáticamente                 │
│  + Actualizar() → Valida pertenencia a entidad                   │
│  + Eliminar() → Valida pertenencia a entidad                     │
└──────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│                    CAPA PRESENTACIÓN                             │
├──────────────────────────────────────────────────────────────────┤
│  SelectorEntidades.aspx (nueva)                                  │
│  + Tarjetas visuales de entidades                                │
│  + Selección de entidad al login                                 │
│                                                                  │
│  Jela.Master (modificada)                                        │
│  + Dropdown de entidades (solo administradores)                  │
│  + Indicador de entidad activa                                   │
│                                                                  │
│  Páginas existentes (modificadas)                                │
│  - Eliminar campo "Entidad" de formularios                       │
│  - Confiar en filtrado automático                                │
└──────────────────────────────────────────────────────────────────┘
```

---

## 3. MODELO DE DATOS

### 3.1 Tabla: conf_usuarios (MODIFICADA)

**Cambios:**
- Agregar campo `TipoUsuario`
- Agregar campo `IdEntidadPrincipal`
- Agregar índices

```sql
ALTER TABLE conf_usuarios
ADD COLUMN TipoUsuario ENUM('AdministradorCondominios', 'MesaDirectiva', 'Residente', 'Empleado') 
  DEFAULT 'Residente' 
  COMMENT 'Tipo de usuario para determinar acceso multi-entidad',
ADD COLUMN IdEntidadPrincipal INT NULL 
  COMMENT 'Entidad principal para usuarios de una sola entidad',
ADD COLUMN LicenciasDisponibles INT DEFAULT 0
  COMMENT 'Número de licencias disponibles para dar de alta entidades (solo para AdministradorCondominios)',
ADD INDEX idx_usuarios_tipo (TipoUsuario),
ADD INDEX idx_usuarios_entidad_principal (IdEntidadPrincipal),
ADD FOREIGN KEY (IdEntidadPrincipal) REFERENCES cat_entidades(Id);
```

**Campos Resultantes:**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | INT | PK |
| Username | VARCHAR(100) | Usuario único |
| Nombre | VARCHAR(200) | Nombre completo |
| email | VARCHAR(200) | Email |
| PasswordHash | VARCHAR(255) | Hash SHA256 |
| **TipoUsuario** | **ENUM** | **NUEVO: Tipo de usuario** |
| **IdEntidadPrincipal** | **INT** | **NUEVO: FK a cat_entidades** |
| **LicenciasDisponibles** | **INT** | **NUEVO: Licencias para alta de entidades** |
| Activo | TINYINT(1) | Estado |
| FechaCreacion | DATETIME | Auditoría |

### 3.2 Tabla: conf_usuario_entidades (NUEVA)

**Propósito:** Relación muchos a muchos entre usuarios y entidades

```sql
CREATE TABLE conf_usuario_entidades (
  Id INT NOT NULL AUTO_INCREMENT,
  IdUsuario INT NOT NULL COMMENT 'FK a conf_usuarios',
  IdEntidad INT NOT NULL COMMENT 'FK a cat_entidades',
  EsPrincipal BOOLEAN DEFAULT FALSE COMMENT 'Indica si es la entidad principal',
  FechaAsignacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  IdUsuarioCreacion INT DEFAULT 1,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  PRIMARY KEY (Id),
  UNIQUE INDEX uk_usuario_entidad (IdUsuario, IdEntidad),
  INDEX idx_usuario (IdUsuario),
  INDEX idx_entidad (IdEntidad),
  INDEX idx_principal (EsPrincipal),
  FOREIGN KEY (IdUsuario) REFERENCES conf_usuarios(Id) ON DELETE CASCADE,
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

**Campos:**

| Campo | Tipo | Descripción |
|-------|------|-------------|
| Id | INT | PK |
| IdUsuario | INT | FK a conf_usuarios |
| IdEntidad | INT | FK a cat_entidades |
| EsPrincipal | BOOLEAN | Marca entidad principal |
| FechaAsignacion | DATETIME | Fecha de asignación |
| IdUsuarioCreacion | INT | Usuario que asignó |
| FechaCreacion | DATETIME | Auditoría |
| Activo | TINYINT(1) | Estado |

**Índices:**
- PK: Id
- UK: (IdUsuario, IdEntidad) - Evita duplicados
- IDX: IdUsuario - Búsqueda por usuario
- IDX: IdEntidad - Búsqueda por entidad
- IDX: EsPrincipal - Búsqueda de entidad principal

### 3.3 Script de Migración de Datos

```sql
-- Migrar usuarios existentes
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad, EsPrincipal, Activo)
SELECT Id, COALESCE(IdEntidad, 1), TRUE, 1
FROM conf_usuarios
WHERE Activo = 1;

-- Actualizar IdEntidadPrincipal
UPDATE conf_usuarios u
INNER JOIN conf_usuario_entidades ue ON u.Id = ue.IdUsuario AND ue.EsPrincipal = TRUE
SET u.IdEntidadPrincipal = ue.IdEntidad;

-- Actualizar TipoUsuario (por defecto Residente)
UPDATE conf_usuarios SET TipoUsuario = 'Residente' WHERE TipoUsuario IS NULL;
```

---

## 4. API (.NET 8)

### 4.1 AuthModels.cs (MODIFICADO)

**Archivo:** `JELA.API/JELA.API/Models/AuthModels.cs`

**Cambios:**

```csharp
public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int? RolId { get; set; }
    public string? RolNombre { get; set; }
    
    // NUEVO
    public string TipoUsuario { get; set; } = "Residente";
    public List<EntidadInfo>? Entidades { get; set; }
    public int? IdEntidadPrincipal { get; set; }
    public string? EntidadPrincipalNombre { get; set; }
    public int LicenciasDisponibles { get; set; } = 0;
}

// NUEVO
public class EntidadInfo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public bool EsPrincipal { get; set; }
}
```

### 4.2 JwtAuthService.cs (MODIFICADO)

**Archivo:** `JELA.API/JELA.API/Services/JwtAuthService.cs`

**Método Principal:**

```csharp
public async Task<AuthResponse> AuthenticateAsync(string username, string password)
{
    // 1. Buscar usuario con entidad principal y licencias
    var query = @"
        SELECT u.Id, u.Username, u.Nombre, u.email as Email, u.PasswordHash,
               u.TipoUsuario, u.IdEntidadPrincipal, u.LicenciasDisponibles,
               e.Nombre as EntidadPrincipalNombre
        FROM conf_usuarios u
        LEFT JOIN cat_entidades e ON u.IdEntidadPrincipal = e.Id
        WHERE u.Username = @username AND u.Activo = 1";

    var usuario = (await _database.EjecutarConsultaAsync(query, parametros)).FirstOrDefault();

    // 2. Verificar contraseña
    // ... (código existente)

    // 3. Obtener entidades del usuario
    var entidades = await ObtenerEntidadesUsuario(Convert.ToInt32(usuario["Id"]));

    // 4. Crear UserInfo
    var userInfo = new UserInfo
    {
        Id = Convert.ToInt32(usuario["Id"]),
        Username = usuario["Username"]?.ToString() ?? string.Empty,
        Nombre = usuario["Nombre"]?.ToString() ?? string.Empty,
        Email = usuario["Email"]?.ToString(),
        TipoUsuario = usuario["TipoUsuario"]?.ToString() ?? "Residente",
        IdEntidadPrincipal = usuario["IdEntidadPrincipal"] != null 
            ? Convert.ToInt32(usuario["IdEntidadPrincipal"]) 
            : null,
        EntidadPrincipalNombre = usuario["EntidadPrincipalNombre"]?.ToString(),
        LicenciasDisponibles = usuario["LicenciasDisponibles"] != null
            ? Convert.ToInt32(usuario["LicenciasDisponibles"])
            : 0,
        Entidades = entidades
    };

    // 5. Generar tokens y retornar
    // ... (código existente)
}
```

**Método Auxiliar (NUEVO):**

```csharp
private async Task<List<EntidadInfo>> ObtenerEntidadesUsuario(int userId)
{
    var query = @"
        SELECT e.Id, e.Nombre, e.Direccion, ue.EsPrincipal
        FROM cat_entidades e
        INNER JOIN conf_usuario_entidades ue ON e.Id = ue.IdEntidad
        WHERE ue.IdUsuario = @userId AND ue.Activo = 1 AND e.Activo = 1
        ORDER BY ue.EsPrincipal DESC, e.Nombre";

    var parametros = new Dictionary<string, object> { { "@userId", userId } };
    var resultados = await _database.EjecutarConsultaAsync(query, parametros);

    return resultados.Select(r => new EntidadInfo
    {
        Id = Convert.ToInt32(r["Id"]),
        Nombre = r["Nombre"]?.ToString() ?? string.Empty,
        Direccion = r["Direccion"]?.ToString(),
        EsPrincipal = Convert.ToBoolean(r["EsPrincipal"])
    }).ToList();
}
```

### 4.3 Endpoint ConsumirLicencia (NUEVO)

**Archivo:** `JELA.API/JELA.API/Endpoints/AuthEndpoints.cs`

**Propósito:** Consumir una licencia cuando el usuario crea una nueva entidad

```csharp
// POST /api/usuarios/{id}/consumir-licencia
app.MapPost("/api/usuarios/{id}/consumir-licencia", async (
    int id,
    IDatabaseService database,
    ILogger<Program> logger) =>
{
    try
    {
        // 1. Obtener licencias actuales
        var query = "SELECT LicenciasDisponibles FROM conf_usuarios WHERE Id = @id AND Activo = 1";
        var parametros = new Dictionary<string, object> { { "@id", id } };
        var resultado = (await database.EjecutarConsultaAsync(query, parametros)).FirstOrDefault();

        if (resultado == null)
        {
            return Results.NotFound(new { message = "Usuario no encontrado" });
        }

        var licenciasActuales = Convert.ToInt32(resultado["LicenciasDisponibles"]);

        // 2. Validar que tenga licencias disponibles
        if (licenciasActuales <= 0)
        {
            return Results.BadRequest(new { message = "No tiene licencias disponibles" });
        }

        // 3. Decrementar licencia
        var updateQuery = @"
            UPDATE conf_usuarios 
            SET LicenciasDisponibles = LicenciasDisponibles - 1 
            WHERE Id = @id";
        
        await database.EjecutarNoConsultaAsync(updateQuery, parametros);

        // 4. Log
        logger.LogInformation($"Usuario {id} consumió una licencia. Restantes: {licenciasActuales - 1}");

        // 5. Retornar licencias restantes
        return Results.Ok(new { 
            success = true, 
            licenciasRestantes = licenciasActuales - 1,
            message = "Licencia consumida exitosamente"
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, $"Error al consumir licencia para usuario {id}");
        return Results.Problem("Error al consumir licencia");
    }
})
.RequireAuthorization()
.WithName("ConsumirLicencia")
.WithTags("Auth");
```

---

## 5. FRONTEND (VB.NET)

### 5.1 Constants.vb (MODIFICADO)

**Archivo:** `JelaWeb/Core/Constants.vb`

**Nuevas Constantes:**

```vb
' Sesión
Public Const SESSION_TIPO_USUARIO As String = "TipoUsuario"
Public Const SESSION_ENTIDADES As String = "Entidades"
Public Const SESSION_ID_ENTIDAD_ACTUAL As String = "IdEntidadActual"
Public Const SESSION_ENTIDAD_ACTUAL_NOMBRE As String = "EntidadActualNombre"
Public Const SESSION_LICENCIAS_DISPONIBLES As String = "LicenciasDisponibles"

' Rutas
Public Const ROUTE_SELECTOR_ENTIDADES As String = "~/Views/Auth/SelectorEntidades.aspx"

' Tipos de Usuario
Public Const TIPO_USUARIO_ADMIN_CONDOMINIOS As String = "AdministradorCondominios"
Public Const TIPO_USUARIO_MESA_DIRECTIVA As String = "MesaDirectiva"
Public Const TIPO_USUARIO_RESIDENTE As String = "Residente"
Public Const TIPO_USUARIO_EMPLEADO As String = "Empleado"
```

### 5.2 SessionHelper.vb (MODIFICADO)

**Archivo:** `JelaWeb/Infrastructure/Helpers/SessionHelper.vb`

**Métodos Nuevos:**

```vb
Public Shared Sub InitializeSession(userId As Object, nombre As String, opciones As JArray, 
                                    tipoUsuario As String, entidades As JArray, 
                                    licenciasDisponibles As Integer,
                                    Optional idEntidadPrincipal As Integer? = Nothing)
    ' Inicializar sesión con nuevos campos
    session(Constants.SESSION_TIPO_USUARIO) = tipoUsuario
    session(Constants.SESSION_ENTIDADES) = entidades
    session(Constants.SESSION_LICENCIAS_DISPONIBLES) = licenciasDisponibles
    
    ' Si es usuario de una sola entidad, establecer automáticamente
    If tipoUsuario <> Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS AndAlso idEntidadPrincipal.HasValue Then
        session(Constants.SESSION_ID_ENTIDAD_ACTUAL) = idEntidadPrincipal.Value
        If entidades IsNot Nothing AndAlso entidades.Count > 0 Then
            session(Constants.SESSION_ENTIDAD_ACTUAL_NOMBRE) = entidades(0)("Nombre").ToString()
        End If
    End If
End Sub

Public Shared Function GetTipoUsuario() As String
Public Shared Function GetEntidades() As JArray
Public Shared Function GetIdEntidadActual() As Integer?
Public Shared Function GetEntidadActualNombre() As String
Public Shared Function GetLicenciasDisponibles() As Integer
Public Shared Sub SetEntidadActual(idEntidad As Integer, nombreEntidad As String)
Public Shared Function IsAdministradorCondominios() As Boolean
Public Shared Function TieneMultiplesEntidades() As Boolean
Public Shared Function TieneLicenciasDisponibles() As Boolean
```

### 5.3 EntidadHelper.vb (NUEVO)

**Archivo:** `JelaWeb/Infrastructure/Helpers/EntidadHelper.vb`

**Propósito:** Helper para manejo automático de filtrado por entidad

```vb
Public NotInheritable Class EntidadHelper
    Public Shared Function GetIdEntidadActualOrThrow() As Integer
    Public Shared Function AgregarFiltroEntidad(query As String) As String
    Public Shared Sub AgregarCampoEntidad(ByRef campos As Dictionary(Of String, Object))
    Public Shared Function ValidarPerteneceAEntidadActual(idRegistro As Integer, tabla As String) As Boolean
End Class
```

**Implementación Clave:**

```vb
Public Shared Function AgregarFiltroEntidad(query As String) As String
    Dim idEntidad = GetIdEntidadActualOrThrow()
    
    If query.ToUpper().Contains(" WHERE ") Then
        Return query & $" AND IdEntidad = {idEntidad}"
    Else
        Return query & $" WHERE IdEntidad = {idEntidad}"
    End If
End Function

Public Shared Sub AgregarCampoEntidad(ByRef campos As Dictionary(Of String, Object))
    Dim idEntidad = GetIdEntidadActualOrThrow()
    
    If Not campos.ContainsKey("IdEntidad") Then
        campos.Add("IdEntidad", idEntidad)
    End If
End Sub
```

### 5.4 ApiConsumerCRUD.vb (MODIFICADO)

**Archivo:** `JelaWeb/Services/API/ApiConsumerCRUD.vb`

**Cambios en Métodos:**

```vb
' ObtenerDatos - Agrega filtro de entidad automáticamente
Public Function ObtenerDatos(tabla As String, Optional filtroAdicional As String = "") As List(Of DynamicDto)
    Dim idEntidad = SessionHelper.GetIdEntidadActual()
    If idEntidad.HasValue Then
        Dim filtroEntidad = $"IdEntidad = {idEntidad.Value}"
        If Not String.IsNullOrEmpty(filtroAdicional) Then
            filtroAdicional = filtroEntidad & " AND " & filtroAdicional
        Else
            filtroAdicional = filtroEntidad
        End If
    End If
    ' ... resto del código
End Function

' Insertar - Agrega IdEntidad automáticamente
Public Function Insertar(tabla As String, campos As Dictionary(Of String, Object)) As Integer
    EntidadHelper.AgregarCampoEntidad(campos)
    ' ... resto del código
End Function

' Actualizar - Valida pertenencia a entidad
Public Function Actualizar(tabla As String, id As Integer, campos As Dictionary(Of String, Object)) As Boolean
    If Not EntidadHelper.ValidarPerteneceAEntidadActual(id, tabla) Then
        Throw New UnauthorizedAccessException($"El registro {id} no pertenece a la entidad actual")
    End If
    ' ... resto del código
End Function

' Eliminar - Valida pertenencia a entidad
Public Function Eliminar(tabla As String, id As Integer) As Boolean
    If Not EntidadHelper.ValidarPerteneceAEntidadActual(id, tabla) Then
        Throw New UnauthorizedAccessException($"El registro {id} no pertenece a la entidad actual")
    End If
    ' ... resto del código
End Function
```

---

## 6. PÁGINAS Y UI

### 6.1 SelectorEntidades.aspx (NUEVA)

**Archivo:** `JelaWeb/Views/Auth/SelectorEntidades.aspx`

**Propósito:** Página de selección de entidad para administradores

**Componentes:**
- Logo y bienvenida con nombre de usuario
- **Indicador de licencias disponibles** (ej: "Licencias disponibles: 3")
- Grid de tarjetas con entidades asignadas
- Cada tarjeta muestra: Nombre, Dirección, Badge "Principal"
- Botón "Ingresar" en cada tarjeta
- Botón "Agregar Nuevo Condominio" (habilitado solo si tiene licencias)
- Link "Cerrar Sesión"

**Lógica (VB.NET):**

```vb
Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
    ' Verificar autenticación
    If Not SessionHelper.IsAuthenticated() Then
        Response.Redirect(Constants.ROUTE_LOGIN, True)
    End If
    
    ' Verificar que sea administrador
    If Not SessionHelper.IsAdministradorCondominios() Then
        Response.Redirect(Constants.ROUTE_INICIO, True)
    End If
    
    If Not IsPostBack Then
        CargarEntidades()
        MostrarLicenciasDisponibles()
        
        ' Si viene de Entidades.aspx después de crear, mostrar mensaje
        If Request.QueryString("nueva") = "1" Then
            ShowSuccess("Condominio creado exitosamente.")
        End If
    End If
End Sub

Private Sub MostrarLicenciasDisponibles()
    Dim licencias = SessionHelper.GetLicenciasDisponibles()
    lblLicencias.Text = $"Licencias disponibles: {licencias}"
    btnAgregarEntidad.Enabled = licencias > 0
    
    If licencias = 0 Then
        btnAgregarEntidad.ToolTip = "No tiene licencias disponibles. Contacte al administrador."
    End If
End Sub

Protected Sub btnAgregarEntidad_Click(sender As Object, e As EventArgs)
    ' Validar licencias
    If Not SessionHelper.TieneLicenciasDisponibles() Then
        ShowError("No tiene licencias disponibles. Contacte al administrador del sistema.")
        Return
    End If
    
    ' Redirigir a página de catálogo de entidades con parámetro
    Response.Redirect("~/Views/Catalogos/Entidades.aspx?modo=nuevo&origen=selector", False)
End Sub

Protected Sub rptEntidades_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
    If e.CommandName = "Seleccionar" Then
        Dim idEntidad As Integer = Convert.ToInt32(e.CommandArgument)
        ' Buscar nombre de entidad
        ' Establecer en sesión
        SessionHelper.SetEntidadActual(idEntidad, nombreEntidad)
        ' Redirigir a inicio
        Response.Redirect(Constants.ROUTE_INICIO, False)
    End If
End Sub
```

### 6.2 Entidades.aspx (MODIFICADA)

**Archivo:** `JelaWeb/Views/Catalogos/Entidades.aspx.vb`

**Cambios Requeridos:**

```vb
Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
    If Not IsPostBack Then
        ' ... código existente ...
        
        ' NUEVO: Detectar si viene del selector de entidades
        If Request.QueryString("modo") = "nuevo" AndAlso Request.QueryString("origen") = "selector" Then
            ' Abrir popup automáticamente
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "AbrirPopup", 
                "popupForm.Show();", True)
        End If
    End If
End Sub

Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)
    Try
        ' ... código existente de guardar entidad ...
        
        ' NUEVO: Si viene del selector, asignar entidad al usuario y consumir licencia
        If Request.QueryString("origen") = "selector" Then
            Dim userId = SessionHelper.GetUserId()
            Dim idNuevaEntidad = ' ... ID de la entidad recién creada
            
            ' 1. Asignar entidad al usuario
            Dim camposRelacion As New Dictionary(Of String, Object) From {
                {"IdUsuario", userId},
                {"IdEntidad", idNuevaEntidad},
                {"EsPrincipal", False},
                {"Activo", 1}
            }
            Dim apiConsumer As New ApiConsumerCRUD()
            apiConsumer.Insertar("conf_usuario_entidades", camposRelacion)
            
            ' 2. Consumir licencia
            Dim authService As New AuthService()
            authService.ConsumirLicencia(userId.Value)
            
            ' 3. Actualizar sesión con nuevas licencias
            Dim licenciasActuales = SessionHelper.GetLicenciasDisponibles()
            Session(Constants.SESSION_LICENCIAS_DISPONIBLES) = licenciasActuales - 1
            
            ' 4. Log
            Logger.LogInfo($"Usuario {userId} creó entidad {idNuevaEntidad} y consumió licencia")
            
            ' 5. Redirigir de vuelta al selector
            Response.Redirect("~/Views/Auth/SelectorEntidades.aspx?nueva=1", False)
        Else
            ' Flujo normal del catálogo
            ShowSuccess("Entidad guardada exitosamente")
            CargarGrid()
        End If
        
    Catch ex As Exception
        Logger.LogError("Error al guardar entidad", ex)
        ShowError("Error al guardar la entidad")
    End Try
End Sub
```

### 6.2 Jela.Master (MODIFICADA)

**Archivo:** `JelaWeb/MasterPages/Jela.Master`

**Cambios en Status Bar:**

```aspx
<div class="status-bar">
    <span class="user-info">
        <i class="fa fa-user"></i>
        <asp:Label ID="lblUsuario" runat="server" />
    </span>

    <!-- NUEVO: Dropdown de entidades -->
    <asp:Panel ID="pnlSelectorEntidades" runat="server" Visible="false" CssClass="entidad-selector">
        <i class="fa fa-building"></i>
        <asp:DropDownList ID="ddlEntidades" runat="server" 
            AutoPostBack="true" 
            OnSelectedIndexChanged="ddlEntidades_SelectedIndexChanged"
            CssClass="form-control form-control-sm">
        </asp:DropDownList>
    </asp:Panel>

    <span class="datetime">
        <i class="fa fa-clock-o"></i>
        <asp:Label ID="lblFechaHora" runat="server" />
    </span>
</div>
```

**Lógica (VB.NET):**

```vb
Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs)
    If Not IsPostBack Then
        ' Cargar dropdown si es administrador con múltiples entidades
        If SessionHelper.IsAdministradorCondominios() AndAlso SessionHelper.TieneMultiplesEntidades() Then
            CargarDropdownEntidades()
            pnlSelectorEntidades.Visible = True
        Else
            pnlSelectorEntidades.Visible = False
        End If
    End If
End Sub

Protected Sub ddlEntidades_SelectedIndexChanged(sender As Object, e As EventArgs)
    Dim idEntidad As Integer = Convert.ToInt32(ddlEntidades.SelectedValue)
    Dim nombreEntidad As String = ddlEntidades.SelectedItem.Text
    
    SessionHelper.SetEntidadActual(idEntidad, nombreEntidad)
    Logger.LogInfo($"Usuario cambió a entidad: {nombreEntidad}")
    
    ' Recargar página actual
    Response.Redirect(Request.RawUrl, False)
End Sub
```

### 6.3 Ingreso.aspx.vb (MODIFICADO)

**Archivo:** `JelaWeb/Views/Auth/Ingreso.aspx.vb`

**Cambios en btnLogin_Click:**

```vb
Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
    ' ... validaciones y autenticación ...
    
    If result.Success Then
        ' Inicializar sesión con nuevos datos
        SessionHelper.InitializeSession(
            result.UserId, 
            result.Nombre, 
            result.Opciones,
            result.TipoUsuario,
            result.Entidades,
            result.IdEntidadPrincipal
        )

        ' Redirigir según tipo de usuario
        If result.TipoUsuario = Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS Then
            Response.Redirect(Constants.ROUTE_SELECTOR_ENTIDADES, False)
        Else
            Response.Redirect(Constants.ROUTE_INICIO, False)
        End If
    End If
End Sub
```

### 6.4 Páginas Existentes (MODIFICADAS)

**Páginas a Actualizar:**
- Cuotas.aspx
- Unidades.aspx
- Residentes.aspx
- Conceptos.aspx
- AreasComunes.aspx
- Tickets.aspx
- Comunicados.aspx
- Reservaciones.aspx
- Pagos.aspx
- EstadoCuenta.aspx

**Cambios Requeridos:**

1. **ELIMINAR** campo "Entidad" del ASPX:
```aspx
<!-- ❌ ELIMINAR -->
<dx:ASPxComboBox ID="cmbEntidad" runat="server" />
```

2. **ELIMINAR** código que obtiene IdEntidad:
```vb
' ❌ ELIMINAR
Dim idEntidad = Convert.ToInt32(cmbEntidad.Value)
```

3. **CONFIAR** en ApiConsumerCRUD:
```vb
' ✅ IdEntidad se agrega automáticamente
Dim campos As New Dictionary(Of String, Object) From {
    {"ConceptoCuotaId", cmbConcepto.Value},
    {"Monto", txtMonto.Value}
}
Dim resultado = apiConsumer.Insertar("op_cuotas", campos)
```

---

## 7. ESTILOS CSS

### 7.1 selector-entidades.css (NUEVO)

**Archivo:** `JelaWeb/Content/Styles/selector-entidades.css`

```css
.selector-container {
    min-height: 100vh;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    padding: 40px 20px;
}

.entidad-card {
    background: white;
    border-radius: 10px;
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    transition: transform 0.3s, box-shadow 0.3s;
}

.entidad-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 8px 15px rgba(0,0,0,0.2);
}
```

### 7.2 site.css (MODIFICADO)

**Archivo:** `JelaWeb/Content/Styles/site.css`

```css
.status-bar .entidad-selector {
    display: inline-block;
    margin: 0 15px;
    padding: 5px 10px;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 5px;
}

.status-bar .entidad-selector .form-control {
    display: inline-block;
    width: auto;
    min-width: 200px;
}
```

---

## 8. SEGURIDAD

### 8.1 Validación de Pertenencia

**Regla:** Antes de actualizar o eliminar, validar que el registro pertenezca a la entidad actual

```vb
If Not EntidadHelper.ValidarPerteneceAEntidadActual(id, tabla) Then
    Throw New UnauthorizedAccessException("Acceso denegado")
End If
```

### 8.2 Logs de Auditoría

**Eventos a Registrar:**
- Cambio de entidad (usuario, entidad origen, entidad destino, timestamp)
- Intentos de acceso no autorizado
- Selección de entidad al login

```vb
Logger.LogInfo($"Usuario {nombre} cambió a entidad: {entidad} (ID: {id})")
Logger.LogWarning($"Intento de acceso no autorizado: Usuario {userId}, Registro {id}, Tabla {tabla}")
```

### 8.3 Prevención de SQL Injection

**Regla:** Usar parámetros en todas las queries

```vb
' ❌ MAL
Dim query = $"SELECT * FROM tabla WHERE Id = {id}"

' ✅ BIEN
Dim query = "SELECT * FROM tabla WHERE Id = @id"
Dim parametros = New Dictionary(Of String, Object) From {{"@id", id}}
```

---

## 9. PERFORMANCE

### 9.1 Índices en Base de Datos

**Índices Requeridos:**

```sql
-- conf_usuarios
CREATE INDEX idx_usuarios_tipo ON conf_usuarios(TipoUsuario);
CREATE INDEX idx_usuarios_entidad_principal ON conf_usuarios(IdEntidadPrincipal);

-- conf_usuario_entidades
CREATE INDEX idx_usuario ON conf_usuario_entidades(IdUsuario);
CREATE INDEX idx_entidad ON conf_usuario_entidades(IdEntidad);
CREATE INDEX idx_principal ON conf_usuario_entidades(EsPrincipal);

-- Todas las tablas operativas
CREATE INDEX idx_entidad ON op_tickets_v2(IdEntidad);
CREATE INDEX idx_entidad ON op_cuotas(IdEntidad);
-- ... etc
```

### 9.2 Caché de Sesión

**Estrategia:**
- Lista de entidades se guarda en sesión (no consultar en cada request)
- Invalidar caché al asignar/remover entidades
- Timeout de sesión: 30 minutos

---

## 10. TESTING

### 10.1 Casos de Prueba

**Test 1: Login Administrador**
- Input: Usuario tipo AdministradorCondominios con 3 entidades
- Expected: Mostrar SelectorEntidades.aspx con 3 tarjetas

**Test 2: Login Usuario Interno**
- Input: Usuario tipo Residente con 1 entidad
- Expected: Ir directo a Inicio.aspx, IdEntidadActual establecido

**Test 3: Cambio de Entidad**
- Input: Seleccionar Entidad B desde dropdown
- Expected: Sesión actualizada, página recargada con datos de Entidad B

**Test 4: Aislamiento de Datos**
- Input: Usuario A en Entidad 1 intenta ver datos
- Expected: Solo ve datos de Entidad 1, no de Entidad 2

**Test 5: Validación de Pertenencia**
- Input: Intentar actualizar registro de otra entidad
- Expected: UnauthorizedAccessException

### 10.2 Checklist de Validación

- [ ] Login de administrador muestra selector
- [ ] Login de usuario interno va directo al sistema
- [ ] Dropdown visible solo para administradores
- [ ] Cambio de entidad funciona correctamente
- [ ] Todas las queries filtran por IdEntidad
- [ ] Todos los INSERT agregan IdEntidad
- [ ] Validación de pertenencia funciona
- [ ] Páginas no muestran campo "Entidad"
- [ ] Logs registran cambios de entidad
- [ ] Sesión mantiene entidad entre páginas

---

## 11. MIGRACIÓN Y ROLLBACK

### 11.1 Plan de Migración

**Paso 1:** Backup completo de base de datos

```bash
mysqldump -u root -p jelabbc > backup_pre_multi_entidad.sql
```

**Paso 2:** Ejecutar scripts de BD

```sql
-- 1. Agregar campos a conf_usuarios
ALTER TABLE conf_usuarios ADD COLUMN TipoUsuario...

-- 2. Crear tabla conf_usuario_entidades
CREATE TABLE conf_usuario_entidades...

-- 3. Migrar datos existentes
INSERT INTO conf_usuario_entidades...
```

**Paso 3:** Desplegar cambios en API

**Paso 4:** Desplegar cambios en Frontend

**Paso 5:** Validar funcionamiento

### 11.2 Plan de Rollback

**Si falla la migración:**

```sql
-- Restaurar backup
mysql -u root -p jelabbc < backup_pre_multi_entidad.sql

-- Revertir cambios en código
git revert <commit_hash>
```

---

## 12. ESTIMACIONES

| Componente | Estimación | Prioridad |
|------------|------------|-----------|
| Base de Datos | 1 día | Alta |
| API (.NET 8) | 1.5 días | Alta |
| SessionHelper + Constants | 0.5 días | Alta |
| EntidadHelper | 0.5 días | Alta |
| ApiConsumerCRUD | 1 día | Alta |
| SelectorEntidades.aspx + Licencias | 1.5 días | Alta |
| Endpoint ConsumirLicencia | 0.5 días | Alta |
| Jela.Master | 0.5 días | Alta |
| Ingreso.aspx | 0.5 días | Alta |
| Páginas existentes (10) | 2-3 días | Media |
| CSS y estilos | 0.5 días | Media |
| Testing | 1-2 días | Alta |

**TOTAL:** 11-14 días

---

## 13. DEPENDENCIAS

### 13.1 Dependencias Técnicas

- MySQL 8.0
- .NET 8 SDK
- VB.NET 4.8.1
- Newtonsoft.Json
- DevExpress WebForms

### 13.2 Dependencias de Negocio

- Definición de tipos de usuario
- Proceso de alta de nuevas entidades
- Modelo de cobro por entidad

---

## 14. RIESGOS Y MITIGACIÓN

| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|--------------|---------|------------|
| Migración de datos falla | Media | Alto | Backup + rollback plan |
| Performance degradada | Baja | Medio | Índices + testing de carga |
| Usuarios confundidos | Media | Medio | Capacitación + documentación |
| Bugs en producción | Media | Alto | Testing exhaustivo |

---

**Última Actualización:** 20 de Enero de 2026  
**Estado:** ✅ Listo para Implementación
