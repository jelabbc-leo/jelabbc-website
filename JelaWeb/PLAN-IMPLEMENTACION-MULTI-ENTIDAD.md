# 🏢 PLAN DE IMPLEMENTACIÓN: Sistema Multi-Entidad con Selector

**Fecha:** 20 de Enero de 2026  
**Objetivo:** Implementar sistema multi-entidad con selector para administradores  
**Estimación Total:** 8-10 días de desarrollo

---

## 📋 RESUMEN EJECUTIVO

### Requerimiento Principal

Implementar un sistema que permita a **Administradores de Condominios** gestionar múltiples entidades,
mientras que **Usuarios Internos** (Mesa Directiva, Residentes) solo accedan a una entidad.

### Tipos de Usuario

| Tipo | Entidades | Selector al Login | Dropdown en Master | Campo Entidad en Forms |
|------|-----------|-------------------|-------------------|------------------------|
| **Administrador de Condominios** | Múltiples | ✅ Sí | ✅ Sí | ❌ No (automático) |
| **Mesa Directiva** | Una | ❌ No | ❌ No | ❌ No (automático) |
| **Residente** | Una | ❌ No | ❌ No | ❌ No (automático) |
| **Empleado** | Una | ❌ No | ❌ No | ❌ No (automático) |

### Flujos de Usuario

**Administrador de Condominios:**
```
Login → Selector de Entidades (tarjetas) → Elegir Entidad → Sistema
                                              ↓
                                    Guardar IdEntidadActual en sesión
                                              ↓
                                    Dropdown en Master Page (cambiar entidad)
```

**Usuario Interno:**
```
Login → Sistema (directo, sin selector)
          ↓
    Usar su única entidad automáticamente
```

---

## 🗄️ FASE 1: CAMBIOS EN BASE DE DATOS

### 1.1 Tabla: conf_usuarios (Modificar)

**Objetivo:** Agregar campo para identificar tipo de usuario

```sql
-- Agregar campo TipoUsuario
ALTER TABLE conf_usuarios
ADD COLUMN TipoUsuario ENUM('AdministradorCondominios', 'MesaDirectiva', 'Residente', 'Empleado') 
  DEFAULT 'Residente' 
  COMMENT 'Tipo de usuario para determinar acceso multi-entidad',
ADD INDEX idx_usuarios_tipo (TipoUsuario);

-- Agregar campo IdEntidadPrincipal (para usuarios de una sola entidad)
ALTER TABLE conf_usuarios
ADD COLUMN IdEntidadPrincipal INT NULL 
  COMMENT 'Entidad principal para usuarios de una sola entidad',
ADD INDEX idx_usuarios_entidad_principal (IdEntidadPrincipal),
ADD FOREIGN KEY (IdEntidadPrincipal) REFERENCES cat_entidades(Id);
```

**Nota:** El campo `IdEntidad` existente se eliminará, ya que ahora usaremos la tabla de relación.


### 1.2 Tabla: conf_usuario_entidades (Nueva - Relación Muchos a Muchos)

**Objetivo:** Permitir que un usuario tenga acceso a múltiples entidades

```sql
CREATE TABLE conf_usuario_entidades (
  Id INT NOT NULL AUTO_INCREMENT,
  IdUsuario INT NOT NULL COMMENT 'FK a conf_usuarios',
  IdEntidad INT NOT NULL COMMENT 'FK a cat_entidades',
  EsPrincipal BOOLEAN DEFAULT FALSE COMMENT 'Indica si es la entidad principal del usuario',
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
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Relación muchos a muchos entre usuarios y entidades';
```

### 1.3 Script de Migración de Datos

**Objetivo:** Migrar usuarios existentes al nuevo modelo

```sql
-- Migrar usuarios existentes (asumiendo que tienen IdEntidad)
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad, EsPrincipal, Activo)
SELECT Id, COALESCE(IdEntidad, 1), TRUE, 1
FROM conf_usuarios
WHERE Activo = 1;

-- Actualizar IdEntidadPrincipal
UPDATE conf_usuarios u
INNER JOIN conf_usuario_entidades ue ON u.Id = ue.IdUsuario AND ue.EsPrincipal = TRUE
SET u.IdEntidadPrincipal = ue.IdEntidad;

-- Actualizar TipoUsuario (por defecto todos son Residentes, ajustar manualmente después)
UPDATE conf_usuarios SET TipoUsuario = 'Residente' WHERE TipoUsuario IS NULL;
```

**Estimación Fase 1:** 1 día

---

## 🔧 FASE 2: CAMBIOS EN API (.NET 8)

### 2.1 Actualizar AuthModels.cs

**Archivo:** `JELA.API/JELA.API/Models/AuthModels.cs`

```csharp
/// <summary>
/// Información del usuario autenticado (ACTUALIZADO)
/// </summary>
public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int? RolId { get; set; }
    public string? RolNombre { get; set; }
    
    // NUEVO: Tipo de usuario
    public string TipoUsuario { get; set; } = "Residente";
    
    // NUEVO: Entidades asignadas
    public List<EntidadInfo>? Entidades { get; set; }
    
    // NUEVO: Entidad principal (para usuarios de una sola entidad)
    public int? IdEntidadPrincipal { get; set; }
    public string? EntidadPrincipalNombre { get; set; }
}

/// <summary>
/// Información de una entidad (NUEVO)
/// </summary>
public class EntidadInfo
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public bool EsPrincipal { get; set; }
}
```


### 2.2 Actualizar JwtAuthService.cs

**Archivo:** `JELA.API/JELA.API/Services/JwtAuthService.cs`

```csharp
public async Task<AuthResponse> AuthenticateAsync(string username, string password)
{
    // 1. Buscar usuario (ACTUALIZADO)
    var query = @"
        SELECT u.Id, u.Username, u.Nombre, u.email as Email, u.PasswordHash,
               u.TipoUsuario, u.IdEntidadPrincipal, e.Nombre as EntidadPrincipalNombre
        FROM conf_usuarios u
        LEFT JOIN cat_entidades e ON u.IdEntidadPrincipal = e.Id
        WHERE u.Username = @username AND u.Activo = 1";

    var usuario = (await _database.EjecutarConsultaAsync(query, parametros)).FirstOrDefault();

    // 2. Verificar contraseña
    // ... (código existente)

    // 3. Obtener entidades del usuario (NUEVO)
    var entidades = await ObtenerEntidadesUsuario(Convert.ToInt32(usuario["Id"]));

    // 4. Crear información del usuario (ACTUALIZADO)
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
        Entidades = entidades
    };

    // 5. Generar tokens y retornar
    // ... (código existente)
}

/// <summary>
/// Obtiene las entidades asignadas a un usuario (NUEVO)
/// </summary>
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

**Estimación Fase 2:** 1 día

---

## 🎨 FASE 3: CAMBIOS EN FRONTEND (VB.NET)

### 3.1 Actualizar Constants.vb

**Archivo:** `JelaWeb/Core/Constants.vb`

```vb
' Agregar nuevas constantes de sesión
Public Const SESSION_TIPO_USUARIO As String = "TipoUsuario"
Public Const SESSION_ENTIDADES As String = "Entidades"
Public Const SESSION_ID_ENTIDAD_ACTUAL As String = "IdEntidadActual"
Public Const SESSION_ENTIDAD_ACTUAL_NOMBRE As String = "EntidadActualNombre"

' Agregar ruta de selector de entidades
Public Const ROUTE_SELECTOR_ENTIDADES As String = "~/Views/Auth/SelectorEntidades.aspx"

' Tipos de usuario
Public Const TIPO_USUARIO_ADMIN_CONDOMINIOS As String = "AdministradorCondominios"
Public Const TIPO_USUARIO_MESA_DIRECTIVA As String = "MesaDirectiva"
Public Const TIPO_USUARIO_RESIDENTE As String = "Residente"
Public Const TIPO_USUARIO_EMPLEADO As String = "Empleado"
```


### 3.2 Actualizar SessionHelper.vb

**Archivo:** `JelaWeb/Infrastructure/Helpers/SessionHelper.vb`

```vb
''' <summary>
''' Inicializa la sesión del usuario después del login (ACTUALIZADO)
''' </summary>
Public Shared Sub InitializeSession(userId As Object, nombre As String, opciones As JArray, 
                                    tipoUsuario As String, entidades As JArray, 
                                    Optional idEntidadPrincipal As Integer? = Nothing)
    Dim session = HttpContext.Current.Session

    If session IsNot Nothing Then
        session.Clear()

        ' Datos básicos
        session(Constants.SESSION_USER_ID) = userId
        session(Constants.SESSION_NOMBRE) = nombre
        session(Constants.SESSION_OPCIONES) = opciones
        session(Constants.SESSION_LOGIN_TIME) = DateTime.Now
        session(Constants.SESSION_LAST_ACTIVITY) = DateTime.Now

        ' NUEVO: Tipo de usuario y entidades
        session(Constants.SESSION_TIPO_USUARIO) = tipoUsuario
        session(Constants.SESSION_ENTIDADES) = entidades

        ' NUEVO: Si es usuario de una sola entidad, establecer automáticamente
        If tipoUsuario <> Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS AndAlso idEntidadPrincipal.HasValue Then
            session(Constants.SESSION_ID_ENTIDAD_ACTUAL) = idEntidadPrincipal.Value
            
            ' Obtener nombre de la entidad
            If entidades IsNot Nothing AndAlso entidades.Count > 0 Then
                Dim entidad = entidades(0)
                session(Constants.SESSION_ENTIDAD_ACTUAL_NOMBRE) = entidad("Nombre").ToString()
            End If
        End If

        session.Timeout = Constants.SESSION_TIMEOUT_MINUTES
    End If
End Sub

''' <summary>
''' Obtiene el tipo de usuario actual (NUEVO)
''' </summary>
Public Shared Function GetTipoUsuario() As String
    Dim session = HttpContext.Current.Session
    If session IsNot Nothing AndAlso session(Constants.SESSION_TIPO_USUARIO) IsNot Nothing Then
        Return session(Constants.SESSION_TIPO_USUARIO).ToString()
    End If
    Return Constants.TIPO_USUARIO_RESIDENTE
End Function

''' <summary>
''' Obtiene las entidades asignadas al usuario (NUEVO)
''' </summary>
Public Shared Function GetEntidades() As JArray
    Dim session = HttpContext.Current.Session
    If session IsNot Nothing AndAlso session(Constants.SESSION_ENTIDADES) IsNot Nothing Then
        Return TryCast(session(Constants.SESSION_ENTIDADES), JArray)
    End If
    Return New JArray()
End Function

''' <summary>
''' Obtiene el ID de la entidad actual (ACTUALIZADO)
''' </summary>
Public Shared Function GetIdEntidadActual() As Integer?
    Dim session = HttpContext.Current.Session
    If session IsNot Nothing AndAlso session(Constants.SESSION_ID_ENTIDAD_ACTUAL) IsNot Nothing Then
        Return Convert.ToInt32(session(Constants.SESSION_ID_ENTIDAD_ACTUAL))
    End If
    Return Nothing
End Function

''' <summary>
''' Obtiene el nombre de la entidad actual (NUEVO)
''' </summary>
Public Shared Function GetEntidadActualNombre() As String
    Dim session = HttpContext.Current.Session
    If session IsNot Nothing AndAlso session(Constants.SESSION_ENTIDAD_ACTUAL_NOMBRE) IsNot Nothing Then
        Return session(Constants.SESSION_ENTIDAD_ACTUAL_NOMBRE).ToString()
    End If
    Return String.Empty
End Function

''' <summary>
''' Establece la entidad actual (NUEVO)
''' </summary>
Public Shared Sub SetEntidadActual(idEntidad As Integer, nombreEntidad As String)
    Dim session = HttpContext.Current.Session
    If session IsNot Nothing Then
        session(Constants.SESSION_ID_ENTIDAD_ACTUAL) = idEntidad
        session(Constants.SESSION_ENTIDAD_ACTUAL_NOMBRE) = nombreEntidad
        session(Constants.SESSION_LAST_ACTIVITY) = DateTime.Now
    End If
End Sub

''' <summary>
''' Verifica si el usuario es administrador de condominios (NUEVO)
''' </summary>
Public Shared Function IsAdministradorCondominios() As Boolean
    Return GetTipoUsuario() = Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS
End Function

''' <summary>
''' Verifica si el usuario tiene múltiples entidades (NUEVO)
''' </summary>
Public Shared Function TieneMultiplesEntidades() As Boolean
    Dim entidades = GetEntidades()
    Return entidades IsNot Nothing AndAlso entidades.Count > 1
End Function
```


### 3.3 Actualizar Ingreso.aspx.vb

**Archivo:** `JelaWeb/Views/Auth/Ingreso.aspx.vb`

```vb
Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
    Try
        ' ... (validaciones existentes)

        ' Autenticar usando AuthService
        Dim result As AuthResult = authService.Autenticar(username, password)

        If result.Success Then
            ' ACTUALIZADO: Inicializar sesión con nuevos datos
            SessionHelper.InitializeSession(
                result.UserId, 
                result.Nombre, 
                result.Opciones,
                result.TipoUsuario,
                result.Entidades,
                result.IdEntidadPrincipal
            )

            ' NUEVO: Redirigir según tipo de usuario
            If result.TipoUsuario = Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS Then
                ' Administrador de condominios → Selector de entidades
                Response.Redirect(Constants.ROUTE_SELECTOR_ENTIDADES, False)
            Else
                ' Usuario interno → Directo al inicio
                Response.Redirect(Constants.ROUTE_INICIO, False)
            End If
            
            Response.End()
        Else
            ShowError(result.Message)
        End If

    Catch ex As Exception
        Logger.LogError("Error durante el proceso de login", ex)
        ShowError("Ha ocurrido un error al intentar iniciar sesión.")
    End Try
End Sub
```


### 3.4 Crear SelectorEntidades.aspx (NUEVA PÁGINA)

**Archivo:** `JelaWeb/Views/Auth/SelectorEntidades.aspx`

```aspx
<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="SelectorEntidades.aspx.vb" 
    Inherits="JelaWeb.SelectorEntidades" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Seleccione un Condominio - JELA</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <link href="~/Content/Styles/selector-entidades.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <div class="container-fluid selector-container">
            <div class="row">
                <div class="col-12 text-center header-section">
                    <img src="~/Content/Images/LogoJelaBBC.png" alt="JELA" class="logo" />
                    <h2>Bienvenido, <asp:Label ID="lblNombreUsuario" runat="server" /></h2>
                    <p class="subtitle">Seleccione el condominio con el que desea trabajar</p>
                </div>
            </div>

            <div class="row entidades-grid">
                <asp:Repeater ID="rptEntidades" runat="server" OnItemCommand="rptEntidades_ItemCommand">
                    <ItemTemplate>
                        <div class="col-md-4 col-lg-3 mb-4">
                            <div class="entidad-card">
                                <div class="card-header">
                                    <h4><%# Eval("Nombre") %></h4>
                                    <%# If(Convert.ToBoolean(Eval("EsPrincipal")), 
                                        "<span class='badge badge-primary'>Principal</span>", "") %>
                                </div>
                                <div class="card-body">
                                    <p class="direccion"><%# Eval("Direccion") %></p>
                                    <asp:LinkButton ID="btnSeleccionar" runat="server" 
                                        CssClass="btn btn-primary btn-block"
                                        CommandName="Seleccionar"
                                        CommandArgument='<%# Eval("Id") %>'>
                                        <i class="fa fa-sign-in"></i> Ingresar
                                    </asp:LinkButton>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <div class="row">
                <div class="col-12 text-center footer-section">
                    <asp:LinkButton ID="btnAgregarEntidad" runat="server" 
                        CssClass="btn btn-success" OnClick="btnAgregarEntidad_Click">
                        <i class="fa fa-plus"></i> Agregar Nuevo Condominio
                    </asp:LinkButton>
                    <asp:LinkButton ID="btnCerrarSesion" runat="server" 
                        CssClass="btn btn-link" OnClick="btnCerrarSesion_Click">
                        Cerrar Sesión
                    </asp:LinkButton>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
```

**Archivo:** `JelaWeb/Views/Auth/SelectorEntidades.aspx.vb`

```vb
Imports Newtonsoft.Json.Linq

Public Class SelectorEntidades
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Verificar que el usuario esté autenticado
        If Not SessionHelper.IsAuthenticated() Then
            Response.Redirect(Constants.ROUTE_LOGIN, True)
            Return
        End If

        ' Verificar que sea administrador de condominios
        If Not SessionHelper.IsAdministradorCondominios() Then
            Response.Redirect(Constants.ROUTE_INICIO, True)
            Return
        End If

        If Not IsPostBack Then
            CargarEntidades()
        End If
    End Sub

    Private Sub CargarEntidades()
        ' Obtener nombre del usuario
        lblNombreUsuario.Text = SessionHelper.GetNombre()

        ' Obtener entidades del usuario
        Dim entidades = SessionHelper.GetEntidades()

        If entidades Is Nothing OrElse entidades.Count = 0 Then
            ' No tiene entidades asignadas
            Response.Write("<script>alert('No tiene condominios asignados. Contacte al administrador.');</script>")
            Return
        End If

        ' Convertir JArray a List para el Repeater
        Dim listaEntidades As New List(Of Object)

        For Each entidad In entidades
            listaEntidades.Add(New With {
                .Id = entidad("Id").ToString(),
                .Nombre = entidad("Nombre").ToString(),
                .Direccion = If(entidad("Direccion") IsNot Nothing, entidad("Direccion").ToString(), ""),
                .EsPrincipal = Convert.ToBoolean(entidad("EsPrincipal"))
            })
        Next

        rptEntidades.DataSource = listaEntidades
        rptEntidades.DataBind()
    End Sub

    Protected Sub rptEntidades_ItemCommand(source As Object, e As RepeaterCommandEventArgs)
        If e.CommandName = "Seleccionar" Then
            Dim idEntidad As Integer = Convert.ToInt32(e.CommandArgument)

            ' Buscar el nombre de la entidad
            Dim entidades = SessionHelper.GetEntidades()
            Dim entidadSeleccionada = entidades.FirstOrDefault(Function(ent) 
                Convert.ToInt32(ent("Id")) = idEntidad)

            If entidadSeleccionada IsNot Nothing Then
                Dim nombreEntidad = entidadSeleccionada("Nombre").ToString()

                ' Establecer la entidad actual en sesión
                SessionHelper.SetEntidadActual(idEntidad, nombreEntidad)

                ' Log de selección
                Logger.LogInfo($"Usuario {SessionHelper.GetNombre()} seleccionó entidad: {nombreEntidad} (ID: {idEntidad})")

                ' Redirigir al inicio
                Response.Redirect(Constants.ROUTE_INICIO, False)
            End If
        End If
    End Sub

    Protected Sub btnAgregarEntidad_Click(sender As Object, e As EventArgs)
        ' TODO: Redirigir a página de registro de nueva entidad
        ' Por ahora, mostrar mensaje
        Response.Write("<script>alert('Funcionalidad en desarrollo. Contacte a soporte para agregar un nuevo condominio.');</script>")
    End Sub

    Protected Sub btnCerrarSesion_Click(sender As Object, e As EventArgs)
        SessionHelper.ClearSession()
        Response.Redirect(Constants.ROUTE_LOGIN, True)
    End Sub
End Class
```

**Archivo:** `JelaWeb/Content/Styles/selector-entidades.css`

```css
.selector-container {
    min-height: 100vh;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    padding: 40px 20px;
}

.header-section {
    margin-bottom: 40px;
    color: white;
}

.header-section .logo {
    max-width: 200px;
    margin-bottom: 20px;
}

.header-section h2 {
    font-size: 2rem;
    font-weight: 300;
    margin-bottom: 10px;
}

.header-section .subtitle {
    font-size: 1.1rem;
    opacity: 0.9;
}

.entidades-grid {
    max-width: 1200px;
    margin: 0 auto;
}

.entidad-card {
    background: white;
    border-radius: 10px;
    box-shadow: 0 4px 6px rgba(0,0,0,0.1);
    transition: transform 0.3s, box-shadow 0.3s;
    height: 100%;
    display: flex;
    flex-direction: column;
}

.entidad-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 8px 15px rgba(0,0,0,0.2);
}

.entidad-card .card-header {
    background: #f8f9fa;
    padding: 20px;
    border-bottom: 1px solid #dee2e6;
    border-radius: 10px 10px 0 0;
}

.entidad-card .card-header h4 {
    margin: 0;
    font-size: 1.3rem;
    color: #333;
}

.entidad-card .card-header .badge {
    margin-left: 10px;
    font-size: 0.75rem;
}

.entidad-card .card-body {
    padding: 20px;
    flex-grow: 1;
    display: flex;
    flex-direction: column;
    justify-content: space-between;
}

.entidad-card .direccion {
    color: #666;
    font-size: 0.95rem;
    margin-bottom: 15px;
    min-height: 40px;
}

.footer-section {
    margin-top: 40px;
}

.footer-section .btn {
    margin: 0 10px;
}

@media (max-width: 768px) {
    .entidad-card {
        margin-bottom: 20px;
    }
}
```

**Estimación Fase 3:** 2-3 días

---

## 🎯 FASE 4: DROPDOWN DE ENTIDADES EN MASTER PAGE

### 4.1 Actualizar Jela.Master

**Archivo:** `JelaWeb/MasterPages/Jela.Master`

Agregar dropdown en el status bar (después del nombre del usuario):

```aspx
<!-- Status Bar (actualizar) -->
<div class="status-bar">
    <span class="user-info">
        <i class="fa fa-user"></i>
        <asp:Label ID="lblUsuario" runat="server" />
    </span>

    <!-- NUEVO: Dropdown de entidades (solo para administradores) -->
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


### 4.2 Actualizar Jela.Master.vb

**Archivo:** `JelaWeb/MasterPages/Jela.Master.vb`

```vb
Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    If Not IsPostBack Then
        ' ... (código existente)

        ' NUEVO: Cargar dropdown de entidades si es administrador
        If SessionHelper.IsAdministradorCondominios() AndAlso SessionHelper.TieneMultiplesEntidades() Then
            CargarDropdownEntidades()
            pnlSelectorEntidades.Visible = True
        Else
            pnlSelectorEntidades.Visible = False
        End If
    End If
End Sub

''' <summary>
''' Carga el dropdown de entidades (NUEVO)
''' </summary>
Private Sub CargarDropdownEntidades()
    Try
        Dim entidades = SessionHelper.GetEntidades()
        Dim idEntidadActual = SessionHelper.GetIdEntidadActual()

        If entidades IsNot Nothing AndAlso entidades.Count > 0 Then
            ddlEntidades.Items.Clear()

            For Each entidad In entidades
                Dim item As New ListItem()
                item.Text = entidad("Nombre").ToString()
                item.Value = entidad("Id").ToString()

                ' Marcar como seleccionado si es la entidad actual
                If idEntidadActual.HasValue AndAlso 
                   Convert.ToInt32(entidad("Id")) = idEntidadActual.Value Then
                    item.Selected = True
                End If

                ddlEntidades.Items.Add(item)
            Next
        End If

    Catch ex As Exception
        Logger.LogError("Error al cargar dropdown de entidades", ex)
    End Try
End Sub

''' <summary>
''' Maneja el cambio de entidad en el dropdown (NUEVO)
''' </summary>
Protected Sub ddlEntidades_SelectedIndexChanged(sender As Object, e As EventArgs)
    Try
        Dim idEntidad As Integer = Convert.ToInt32(ddlEntidades.SelectedValue)
        Dim nombreEntidad As String = ddlEntidades.SelectedItem.Text

        ' Cambiar la entidad actual en sesión
        SessionHelper.SetEntidadActual(idEntidad, nombreEntidad)

        ' Log del cambio
        Logger.LogInfo($"Usuario {SessionHelper.GetNombre()} cambió a entidad: {nombreEntidad} (ID: {idEntidad})")

        ' Recargar la página actual para reflejar los cambios
        Response.Redirect(Request.RawUrl, False)

    Catch ex As Exception
        Logger.LogError("Error al cambiar de entidad", ex)
        ' Mostrar mensaje de error al usuario
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ErrorCambioEntidad", 
            "alert('Error al cambiar de condominio. Por favor, intente nuevamente.');", True)
    End Try
End Sub
```

### 4.3 Actualizar Site.css

**Archivo:** `JelaWeb/Content/Styles/site.css`

```css
/* Dropdown de entidades en status bar */
.status-bar .entidad-selector {
    display: inline-block;
    margin: 0 15px;
    padding: 5px 10px;
    background: rgba(255, 255, 255, 0.1);
    border-radius: 5px;
}

.status-bar .entidad-selector i {
    margin-right: 8px;
    color: #ffc107;
}

.status-bar .entidad-selector .form-control {
    display: inline-block;
    width: auto;
    min-width: 200px;
    background: white;
    border: 1px solid #ddd;
    color: #333;
    font-size: 0.9rem;
    padding: 5px 10px;
}

.status-bar .entidad-selector .form-control:focus {
    border-color: #667eea;
    box-shadow: 0 0 0 0.2rem rgba(102, 126, 234, 0.25);
}
```

**Estimación Fase 4:** 1 día

---

## 🔄 FASE 5: FILTRADO AUTOMÁTICO POR ENTIDAD

### 5.1 Crear EntidadHelper.vb (NUEVO)

**Archivo:** `JelaWeb/Infrastructure/Helpers/EntidadHelper.vb`

```vb
''' <summary>
''' Helper para manejo automático de filtrado por entidad
''' </summary>
Public NotInheritable Class EntidadHelper
    Private Sub New()
        ' Clase estática
    End Sub

    ''' <summary>
    ''' Obtiene el ID de la entidad actual o lanza excepción si no está definida
    ''' </summary>
    Public Shared Function GetIdEntidadActualOrThrow() As Integer
        Dim idEntidad = SessionHelper.GetIdEntidadActual()

        If Not idEntidad.HasValue Then
            Throw New InvalidOperationException("No hay una entidad seleccionada. El usuario debe seleccionar un condominio.")
        End If

        Return idEntidad.Value
    End Function

    ''' <summary>
    ''' Agrega filtro de entidad a una query SQL
    ''' </summary>
    Public Shared Function AgregarFiltroEntidad(query As String) As String
        Dim idEntidad = GetIdEntidadActualOrThrow()

        ' Detectar si ya tiene WHERE
        If query.ToUpper().Contains(" WHERE ") Then
            Return query & $" AND IdEntidad = {idEntidad}"
        Else
            Return query & $" WHERE IdEntidad = {idEntidad}"
        End If
    End Function

    ''' <summary>
    ''' Agrega el campo IdEntidad a un diccionario de campos para INSERT/UPDATE
    ''' </summary>
    Public Shared Sub AgregarCampoEntidad(ByRef campos As Dictionary(Of String, Object))
        Dim idEntidad = GetIdEntidadActualOrThrow()

        If Not campos.ContainsKey("IdEntidad") Then
            campos.Add("IdEntidad", idEntidad)
        End If
    End Sub

    ''' <summary>
    ''' Valida que un registro pertenezca a la entidad actual
    ''' </summary>
    Public Shared Function ValidarPerteneceAEntidadActual(idRegistro As Integer, tabla As String) As Boolean
        Try
            Dim idEntidad = GetIdEntidadActualOrThrow()
            Dim apiConsumer As New ApiConsumerCRUD()

            Dim query = $"SELECT COUNT(*) as Total FROM {tabla} WHERE Id = {idRegistro} AND IdEntidad = {idEntidad}"
            Dim resultado = apiConsumer.EjecutarConsulta(query)

            If resultado IsNot Nothing AndAlso resultado.Count > 0 Then
                Dim total = Convert.ToInt32(resultado(0).Campos("Total").Valor)
                Return total > 0
            End If

            Return False

        Catch ex As Exception
            Logger.LogError($"Error al validar pertenencia a entidad: {tabla}, ID: {idRegistro}", ex)
            Return False
        End Try
    End Function
End Class
```


### 5.2 Actualizar ApiConsumerCRUD.vb

**Archivo:** `JelaWeb/Services/API/ApiConsumerCRUD.vb`

```vb
''' <summary>
''' Obtiene datos de una tabla (ACTUALIZADO - con filtro automático de entidad)
''' </summary>
Public Function ObtenerDatos(tabla As String, Optional filtroAdicional As String = "") As List(Of DynamicDto)
    Try
        ' NUEVO: Agregar filtro de entidad automáticamente
        Dim idEntidad = SessionHelper.GetIdEntidadActual()

        If idEntidad.HasValue Then
            Dim filtroEntidad = $"IdEntidad = {idEntidad.Value}"

            If Not String.IsNullOrEmpty(filtroAdicional) Then
                filtroAdicional = filtroEntidad & " AND " & filtroAdicional
            Else
                filtroAdicional = filtroEntidad
            End If
        End If

        Dim query = $"SELECT * FROM {tabla}"

        If Not String.IsNullOrEmpty(filtroAdicional) Then
            query &= $" WHERE {filtroAdicional}"
        End If

        Return EjecutarConsulta(query)

    Catch ex As Exception
        Logger.LogError($"Error en ObtenerDatos para tabla {tabla}", ex)
        Throw
    End Try
End Function

''' <summary>
''' Inserta un registro (ACTUALIZADO - agrega IdEntidad automáticamente)
''' </summary>
Public Function Insertar(tabla As String, campos As Dictionary(Of String, Object)) As Integer
    Try
        ' NUEVO: Agregar IdEntidad automáticamente si no existe
        EntidadHelper.AgregarCampoEntidad(campos)

        ' Agregar campos de auditoría
        If Not campos.ContainsKey("IdUsuarioCreacion") Then
            Dim userId = SessionHelper.GetUserId()
            If userId.HasValue Then
                campos.Add("IdUsuarioCreacion", userId.Value)
            End If
        End If

        If Not campos.ContainsKey("FechaCreacion") Then
            campos.Add("FechaCreacion", DateTime.Now)
        End If

        ' Construir query INSERT
        Dim columnas = String.Join(", ", campos.Keys)
        Dim valores = String.Join(", ", campos.Keys.Select(Function(k) $"@{k}"))
        Dim query = $"INSERT INTO {tabla} ({columnas}) VALUES ({valores}); SELECT LAST_INSERT_ID();"

        ' Ejecutar y retornar ID
        Dim resultado = EjecutarConsulta(query, campos)

        If resultado IsNot Nothing AndAlso resultado.Count > 0 Then
            Return Convert.ToInt32(resultado(0).Campos.First().Value.Valor)
        End If

        Return 0

    Catch ex As Exception
        Logger.LogError($"Error en Insertar para tabla {tabla}", ex)
        Throw
    End Try
End Function

''' <summary>
''' Actualiza un registro (ACTUALIZADO - valida que pertenezca a la entidad)
''' </summary>
Public Function Actualizar(tabla As String, id As Integer, campos As Dictionary(Of String, Object)) As Boolean
    Try
        ' NUEVO: Validar que el registro pertenezca a la entidad actual
        If Not EntidadHelper.ValidarPerteneceAEntidadActual(id, tabla) Then
            Throw New UnauthorizedAccessException($"El registro {id} no pertenece a la entidad actual")
        End If

        ' Agregar campos de auditoría
        If Not campos.ContainsKey("FechaUltimaActualizacion") Then
            campos.Add("FechaUltimaActualizacion", DateTime.Now)
        End If

        ' Construir query UPDATE
        Dim sets = String.Join(", ", campos.Keys.Select(Function(k) $"{k} = @{k}"))
        Dim idEntidad = SessionHelper.GetIdEntidadActual()
        Dim query = $"UPDATE {tabla} SET {sets} WHERE Id = @id AND IdEntidad = {idEntidad.Value}"

        campos.Add("id", id)

        ' Ejecutar
        Dim resultado = EjecutarNoConsulta(query, campos)
        Return resultado > 0

    Catch ex As Exception
        Logger.LogError($"Error en Actualizar para tabla {tabla}, ID {id}", ex)
        Throw
    End Try
End Function

''' <summary>
''' Elimina un registro (ACTUALIZADO - valida que pertenezca a la entidad)
''' </summary>
Public Function Eliminar(tabla As String, id As Integer) As Boolean
    Try
        ' NUEVO: Validar que el registro pertenezca a la entidad actual
        If Not EntidadHelper.ValidarPerteneceAEntidadActual(id, tabla) Then
            Throw New UnauthorizedAccessException($"El registro {id} no pertenece a la entidad actual")
        End If

        Dim idEntidad = SessionHelper.GetIdEntidadActual()
        Dim query = $"DELETE FROM {tabla} WHERE Id = @id AND IdEntidad = {idEntidad.Value}"
        Dim parametros As New Dictionary(Of String, Object) From {{"@id", id}}

        Dim resultado = EjecutarNoConsulta(query, parametros)
        Return resultado > 0

    Catch ex As Exception
        Logger.LogError($"Error en Eliminar para tabla {tabla}, ID {id}", ex)
        Throw
    End Try
End Function
```

**Estimación Fase 5:** 2 días

---

## 📝 FASE 6: ACTUALIZAR PÁGINAS EXISTENTES

### 6.1 Ejemplo: Actualizar Cuotas.aspx

**Archivo:** `JelaWeb/Views/Operacion/Condominios/Cuotas.aspx`

**CAMBIOS:**

1. **ELIMINAR** el campo "Entidad" del popup de Nueva Cuota
2. **ELIMINAR** el campo "Entidad" del grid (si existe como columna)
3. El sistema usará automáticamente `SessionHelper.GetIdEntidadActual()`

**Antes (ASPX):**
```aspx
<!-- ❌ ELIMINAR este campo -->
<div class="form-group">
    <label>Entidad:*</label>
    <dx:ASPxComboBox ID="cmbEntidad" runat="server" />
</div>
```

**Después (ASPX):**
```aspx
<!-- ✅ Campo eliminado - se usa automáticamente de sesión -->
```

**Antes (VB.NET):**
```vb
' ❌ ELIMINAR este código
Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)
    Dim idEntidad = Convert.ToInt32(cmbEntidad.Value)
    ' ...
End Sub
```

**Después (VB.NET):**
```vb
' ✅ No se necesita obtener IdEntidad - ApiConsumerCRUD lo agrega automáticamente
Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)
    Dim campos As New Dictionary(Of String, Object) From {
        {"ConceptoCuotaId", cmbConcepto.Value},
        {"Periodo", txtPeriodo.Text},
        {"Monto", txtMonto.Value}
        ' IdEntidad se agrega automáticamente en ApiConsumerCRUD.Insertar()
    }

    Dim resultado = apiConsumer.Insertar("op_cuotas", campos)
    ' ...
End Sub
```

### 6.2 Lista de Páginas a Actualizar

**Páginas que requieren actualización:**

1. ✅ Cuotas.aspx - Eliminar campo Entidad
2. ✅ Unidades.aspx - Eliminar campo Entidad
3. ✅ Residentes.aspx - Eliminar campo Entidad
4. ✅ Conceptos.aspx - Eliminar campo Entidad
5. ✅ AreasComunes.aspx - Eliminar campo Entidad
6. ✅ Tickets.aspx - Eliminar campo Entidad
7. ✅ Comunicados.aspx - Eliminar campo Entidad
8. ✅ Reservaciones.aspx - Eliminar campo Entidad
9. ✅ Pagos.aspx - Eliminar campo Entidad
10. ✅ EstadoCuenta.aspx - Eliminar campo Entidad

**Patrón de actualización:**
- Eliminar combo/dropdown de Entidad del ASPX
- Eliminar código que obtiene IdEntidad del combo
- Confiar en ApiConsumerCRUD para agregar IdEntidad automáticamente
- Confiar en ApiConsumerCRUD para filtrar por IdEntidad automáticamente

**Estimación Fase 6:** 2-3 días (dependiendo del número de páginas)

---

## 🧪 FASE 7: TESTING Y VALIDACIÓN

### 7.1 Casos de Prueba

**Test 1: Login como Administrador de Condominios**
- Usuario: admin@condominios.com
- Tipo: AdministradorCondominios
- Entidades: 3 condominios
- Resultado esperado: Mostrar selector de entidades

**Test 2: Login como Usuario Interno**
- Usuario: mesa.directiva@condominio1.com
- Tipo: MesaDirectiva
- Entidades: 1 condominio
- Resultado esperado: Ir directo al sistema

**Test 3: Cambio de Entidad con Dropdown**
- Seleccionar Condominio A
- Crear cuota
- Cambiar a Condominio B con dropdown
- Verificar que solo se vean cuotas de Condominio B

**Test 4: Aislamiento de Datos**
- Usuario A en Condominio 1
- Usuario B en Condominio 2
- Verificar que Usuario A NO vea datos de Condominio 2

**Test 5: Validación de Pertenencia**
- Intentar actualizar registro de otra entidad
- Resultado esperado: Error de autorización

### 7.2 Checklist de Validación

- [ ] Login de administrador muestra selector
- [ ] Login de usuario interno va directo al sistema
- [ ] Dropdown de entidades solo visible para administradores
- [ ] Cambio de entidad recarga datos correctamente
- [ ] Todas las queries filtran por IdEntidad
- [ ] Todos los INSERT agregan IdEntidad automáticamente
- [ ] No se puede actualizar/eliminar registros de otra entidad
- [ ] Páginas no muestran campo "Entidad" en formularios
- [ ] Logs registran cambios de entidad
- [ ] Sesión mantiene entidad seleccionada entre páginas

**Estimación Fase 7:** 1-2 días

---

## 📊 RESUMEN DE ESTIMACIONES

| Fase | Descripción | Estimación | Prioridad |
|------|-------------|------------|-----------|
| **Fase 1** | Cambios en Base de Datos | 1 día | 🔴 ALTA |
| **Fase 2** | Cambios en API (.NET 8) | 1 día | 🔴 ALTA |
| **Fase 3** | Cambios en Frontend (SessionHelper, Login) | 2-3 días | 🔴 ALTA |
| **Fase 4** | Dropdown en Master Page | 1 día | 🔴 ALTA |
| **Fase 5** | Filtrado Automático | 2 días | 🔴 ALTA |
| **Fase 6** | Actualizar Páginas Existentes | 2-3 días | 🟡 MEDIA |
| **Fase 7** | Testing y Validación | 1-2 días | 🟡 MEDIA |

**TOTAL:** 10-13 días de desarrollo

---

## 🎯 ORDEN DE IMPLEMENTACIÓN RECOMENDADO

### Sprint 1 (Días 1-3): Fundamentos
1. Fase 1: Base de Datos
2. Fase 2: API
3. Fase 3: Frontend básico (SessionHelper, Constants)

### Sprint 2 (Días 4-6): Selector y Dropdown
1. Fase 3: Página SelectorEntidades
2. Fase 4: Dropdown en Master Page
3. Fase 5: Filtrado automático

### Sprint 3 (Días 7-10): Actualización de Páginas
1. Fase 6: Actualizar páginas existentes (priorizar las más usadas)
2. Fase 7: Testing inicial

### Sprint 4 (Días 11-13): Testing y Ajustes
1. Fase 7: Testing completo
2. Corrección de bugs
3. Documentación

---

## ⚠️ CONSIDERACIONES IMPORTANTES

### Seguridad

1. **Validación de Pertenencia:** SIEMPRE validar que el registro pertenezca a la entidad actual
2. **Logs de Auditoría:** Registrar todos los cambios de entidad
3. **Prevención de SQL Injection:** Usar parámetros en todas las queries
4. **Autorización:** Validar permisos antes de permitir cambio de entidad

### Performance

1. **Caché de Entidades:** Guardar lista de entidades en sesión (ya implementado)
2. **Índices en BD:** Agregar índices en campos IdEntidad de todas las tablas
3. **Lazy Loading:** Cargar opciones del menú solo cuando sea necesario

### UX/UI

1. **Indicador Visual:** Mostrar claramente qué entidad está activa
2. **Confirmación de Cambio:** Opcional - preguntar antes de cambiar entidad
3. **Breadcrumbs:** Incluir nombre de entidad en breadcrumbs
4. **Mensajes Claros:** Informar al usuario cuando no tiene entidad seleccionada

### Migración de Datos

1. **Backup:** Hacer backup completo antes de ejecutar scripts
2. **Usuarios Existentes:** Asignar tipo "Residente" por defecto
3. **Entidades Existentes:** Migrar relaciones usuario-entidad
4. **Validación:** Verificar que todos los usuarios tengan al menos una entidad

---

## 📝 CONCLUSIÓN

Este plan implementa un sistema robusto de multi-entidad que:

✅ Permite a administradores gestionar múltiples condominios  
✅ Mantiene usuarios internos en una sola entidad  
✅ Filtra automáticamente todos los datos por entidad  
✅ Elimina la necesidad de seleccionar entidad en formularios  
✅ Proporciona cambio rápido de entidad vía dropdown  
✅ Mantiene aislamiento completo de datos entre entidades  
✅ Es escalable y mantenible

**Tiempo total estimado:** 10-13 días de desarrollo

---

**Creado por:** Kiro AI  
**Fecha:** 20 de Enero de 2026  
**Estado:** ⏳ LISTO PARA REVISIÓN Y APROBACIÓN
