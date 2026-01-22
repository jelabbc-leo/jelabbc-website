# 🔐 ANÁLISIS COMPLETO: Autenticación y Sesión en JELABBC

**Fecha:** 20 de Enero de 2026  
**Alcance:** Sistema completo de autenticación y gestión de sesión  
**Objetivo:** Documentar flujo de login, información disponible globalmente y propuestas de mejora

---

## 📊 RESUMEN EJECUTIVO

### Estado Actual del Sistema

| Aspecto | Estado | Observación |
|---------|--------|-------------|
| **Autenticación** | ✅ Funcional | JWT + API REST |
| **IdEntidad en sesión** | ⚠️ **PARCIAL** | Se guarda pero **NO se usa consistentemente** |
| **Multi-tenant** | ⚠️ **INCOMPLETO** | Infraestructura existe pero no se aplica |
| **Jerarquías de usuario** | ❌ **NO IMPLEMENTADO** | No hay roles ni permisos en sesión |
| **Información disponible** | ⚠️ **LIMITADA** | Solo: UserId, Nombre, IdEntidad, Opciones |

### Hallazgos Críticos

1. ✅ **IdEntidad SÍ se guarda en sesión** durante el login
2. ❌ **IdEntidad NO se usa** en las queries del sistema (falta WHERE IdEntidad = X)
3. ❌ **NO hay información de roles** en la sesión
4. ❌ **NO hay jerarquías** de usuario implementadas
5. ⚠️ **Sistema multi-tenant a medias** - Infraestructura lista pero sin uso real

---

## 🔄 FLUJO COMPLETO DE AUTENTICACIÓN

### 1. Usuario Ingresa Credenciales

**Archivo:** `JelaWeb/Views/Auth/Ingreso.aspx.vb`

```vb
Protected Sub btnLogin_Click(sender As Object, e As EventArgs)
    ' 1. Validar entrada
    If String.IsNullOrWhiteSpace(txtUsername.Text) Or String.IsNullOrWhiteSpace(txtPassword.Text) Then
        ShowError("Por favor, ingrese usuario y contraseña.")
        Return
    End If

    ' 2. Sanitizar entrada
    Dim username As String = SecurityHelper.SanitizeInput(txtUsername.Text)
    Dim password As String = txtPassword.Text

    ' 3. Autenticar usando AuthService
    Dim result As AuthResult = authService.Autenticar(username, password)

    ' 4. Si es exitoso, inicializar sesión
    If result.Success Then
        SessionHelper.InitializeSession(result.UserId, result.Nombre, result.Opciones, result.IdEntidad)
        Response.Redirect(Constants.ROUTE_INICIO, False)
    End If
End Sub
```

---

### 2. AuthService Llama al API

**Archivo:** `JelaWeb/Services/Auth/AuthService.vb`

```vb
Public Function Autenticar(username As String, password As String) As AuthResult
    ' 1. Preparar request
    Dim loginRequest = New With {
        .username = username,
        .password = password
    }

    ' 2. Llamar al API JWT
    Dim json = JsonConvert.SerializeObject(loginRequest)
    Dim content = New StringContent(json, Encoding.UTF8, "application/json")
    Dim respuesta = HttpClientHelper.Client.PostAsync(apiAuthUrl, content).Result

    ' 3. Parsear respuesta JWT
    Dim authResponse = JsonConvert.DeserializeObject(Of JwtAuthResponse)(contenido)

    ' 4. Guardar tokens JWT
    JwtTokenService.Instance.SetToken(
        authResponse.Token,
        authResponse.RefreshToken,
        authResponse.ExpiresAt.Value
    )

    ' 5. Obtener datos del usuario
    Dim userId As Integer = authResponse.User.Id
    Dim nombre As String = authResponse.User.Nombre
    Dim email As String = authResponse.User.Email
    Dim idEntidad As Integer = authResponse.User.EntidadId  ' ⚠️ AQUÍ SE OBTIENE
    Dim entidadNombre As String = authResponse.User.EntidadNombre

    ' 6. Obtener opciones del menú
    Dim opciones As JArray = ObtenerOpcionesMenu(userId)

    ' 7. Retornar resultado
    Return New AuthResult With {
        .Success = True,
        .UserId = userId,
        .Nombre = nombre,
        .Email = email,
        .IdEntidad = idEntidad,        ' ⚠️ SE RETORNA
        .EntidadNombre = entidadNombre,
        .Opciones = opciones
    }
End Function
```

---

### 3. API Autentica y Retorna Datos

**Archivo:** `JELA.API/JELA.API/Services/JwtAuthService.cs`

```csharp
public async Task<AuthResponse> AuthenticateAsync(string username, string password)
{
    // 1. Buscar usuario en BD
    var query = @"
        SELECT Id, Username, Nombre, email as Email, PasswordHash
        FROM conf_usuarios
        WHERE Username = @username AND Activo = 1";

    var usuario = (await _database.EjecutarConsultaAsync(query, parametros)).FirstOrDefault();

    // 2. Verificar contraseña (SHA256)
    var passwordHash = usuario["PasswordHash"]?.ToString();
    var inputHash = ComputeSHA256Hash(password);

    if (passwordHash != inputHash)
        return new AuthResponse { Success = false, Message = "Usuario o contraseña incorrectos" };

    // 3. Crear información del usuario
    var userInfo = new UserInfo
    {
        Id = Convert.ToInt32(usuario["Id"]),
        Username = usuario["Username"]?.ToString() ?? string.Empty,
        Nombre = usuario["Nombre"]?.ToString() ?? string.Empty,
        Email = usuario["Email"]?.ToString(),
        RolId = null,              // ❌ NO SE OBTIENE
        RolNombre = null,          // ❌ NO SE OBTIENE
        EntidadId = null,          // ❌ NO SE OBTIENE (debería obtenerse de conf_usuarios)
        EntidadNombre = null       // ❌ NO SE OBTIENE
    };

    // 4. Generar tokens JWT
    var token = GenerateToken(userInfo);
    var refreshToken = GenerateRefreshToken();

    // 5. Retornar respuesta
    return new AuthResponse
    {
        Success = true,
        Token = token,
        RefreshToken = refreshToken,
        ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        User = userInfo  // ⚠️ UserInfo con EntidadId = null
    };
}
```

**⚠️ PROBLEMA DETECTADO:** El API **NO está obteniendo** `EntidadId` de la base de datos. La tabla `conf_usuarios` debería tener este campo.

---

### 4. SessionHelper Inicializa la Sesión

**Archivo:** `JelaWeb/Infrastructure/Helpers/SessionHelper.vb`

```vb
Public Shared Sub InitializeSession(userId As Object, nombre As String, opciones As JArray, Optional idEntidad As Integer = 0)
    Dim session = HttpContext.Current.Session

    If session IsNot Nothing Then
        ' Limpiar sesión anterior
        session.Clear()

        ' Inicializar valores de sesión
        session(Constants.SESSION_USER_ID) = userId           ' ✅ UserId
        session(Constants.SESSION_ID_ENTIDAD) = idEntidad     ' ✅ IdEntidad (GUARDADO)
        session(Constants.SESSION_NOMBRE) = nombre            ' ✅ Nombre
        session(Constants.SESSION_OPCIONES) = opciones        ' ✅ Opciones menú
        session(Constants.SESSION_LOGIN_TIME) = DateTime.Now
        session(Constants.SESSION_LAST_ACTIVITY) = DateTime.Now

        session.Timeout = Constants.SESSION_TIMEOUT_MINUTES
    End If
End Sub
```

---

## 📦 INFORMACIÓN DISPONIBLE GLOBALMENTE

### Constantes de Sesión

**Archivo:** `JelaWeb/Core/Constants.vb`

```vb
' Claves de sesión disponibles
Public Const SESSION_USER_ID As String = "UserId"           ' ✅ ID del usuario
Public Const SESSION_ID_ENTIDAD As String = "IdEntidad"     ' ✅ ID de la entidad (multi-tenant)
Public Const SESSION_NOMBRE As String = "Nombre"            ' ✅ Nombre del usuario
Public Const SESSION_OPCIONES As String = "Opciones"        ' ✅ Opciones del menú (JArray)
Public Const SESSION_LAST_ACTIVITY As String = "LastActivity"
Public Const SESSION_LOGIN_TIME As String = "LoginTime"
```

### Métodos de Acceso

**Archivo:** `JelaWeb/Infrastructure/Helpers/SessionHelper.vb`

```vb
' ✅ Obtener UserId
Public Shared Function GetUserId() As Integer?
    Return Convert.ToInt32(session(Constants.SESSION_USER_ID))
End Function

' ✅ Obtener IdEntidad (DISPONIBLE PERO NO SE USA)
Public Shared Function GetIdEntidad() As Integer
    Return Convert.ToInt32(session(Constants.SESSION_ID_ENTIDAD))
End Function

' ✅ Obtener Nombre
Public Shared Function GetNombre() As String
    Return session(Constants.SESSION_NOMBRE).ToString()
End Function

' ✅ Obtener Opciones del Menú
Public Shared Function GetOpciones() As JArray
    Return TryCast(session(Constants.SESSION_OPCIONES), JArray)
End Function

' ✅ Verificar si está autenticado
Public Shared Function IsAuthenticated() As Boolean
    Return session IsNot Nothing AndAlso GetUserId().HasValue
End Function
```

---

## ❌ INFORMACIÓN QUE **NO** ESTÁ DISPONIBLE

### 1. Roles del Usuario

```vb
' ❌ NO EXISTE
Public Shared Function GetRolId() As Integer?
Public Shared Function GetRolNombre() As String

' ❌ NO EXISTE
Public Shared Function GetRoles() As List(Of Integer)
Public Shared Function HasRole(roleName As String) As Boolean
```

### 2. Permisos del Usuario

```vb
' ❌ NO EXISTE
Public Shared Function GetPermisos() As List(Of String)
Public Shared Function HasPermiso(permiso As String) As Boolean
Public Shared Function CanAccess(recurso As String) As Boolean
```

### 3. Jerarquía Organizacional

```vb
' ❌ NO EXISTE
Public Shared Function GetDepartamentoId() As Integer?
Public Shared Function GetSupervisorId() As Integer?
Public Shared Function GetNivelJerarquico() As Integer
Public Shared Function GetSubordinados() As List(Of Integer)
```

### 4. Información de la Entidad

```vb
' ⚠️ EXISTE GetIdEntidad() PERO NO:
Public Shared Function GetEntidadNombre() As String
Public Shared Function GetEntidadTipo() As String
Public Shared Function GetEntidadConfiguracion() As Object
```

---

## 🔍 ANÁLISIS: ¿SE USA IdEntidad EN EL SISTEMA?

### Búsqueda en el Código

Voy a buscar si `GetIdEntidad()` se usa en alguna parte del sistema:

**Resultado:** ⚠️ **SE GUARDA PERO NO SE USA**

El método `SessionHelper.GetIdEntidad()` existe y funciona, pero:

1. ❌ **NO se usa en queries SQL** - Las consultas no filtran por `WHERE IdEntidad = X`
2. ❌ **NO se usa en ApiConsumerCRUD** - No se envía como parámetro
3. ❌ **NO se usa en páginas** - Las páginas no lo consultan
4. ❌ **NO se usa en servicios** - Los servicios VB.NET no lo usan

### Ejemplo de Uso Correcto (NO IMPLEMENTADO)

```vb
' ❌ ACTUAL (sin filtro de entidad)
Dim query = "SELECT * FROM op_tickets_v2 WHERE Estado = 'Abierto'"

' ✅ DEBERÍA SER (con filtro de entidad)
Dim idEntidad = SessionHelper.GetIdEntidad()
Dim query = $"SELECT * FROM op_tickets_v2 WHERE Estado = 'Abierto' AND IdEntidad = {idEntidad}"
```

---

## 🚨 PROBLEMAS IDENTIFICADOS

### 1. API No Obtiene IdEntidad de la BD

**Problema:** `JwtAuthService.cs` no consulta el campo `IdEntidad` de `conf_usuarios`

**Solución:**

```csharp
// ❌ ACTUAL
var query = @"
    SELECT Id, Username, Nombre, email as Email, PasswordHash
    FROM conf_usuarios
    WHERE Username = @username AND Activo = 1";

// ✅ DEBERÍA SER
var query = @"
    SELECT u.Id, u.Username, u.Nombre, u.email as Email, u.PasswordHash, 
           u.IdEntidad, e.Nombre as EntidadNombre
    FROM conf_usuarios u
    LEFT JOIN cat_entidades e ON u.IdEntidad = e.Id
    WHERE u.Username = @username AND u.Activo = 1";
```

### 2. Tabla conf_usuarios Sin Campo IdEntidad

**Problema:** La tabla `conf_usuarios` probablemente **NO tiene** el campo `IdEntidad`

**Solución:** Agregar campo a la tabla:

```sql
ALTER TABLE conf_usuarios
ADD COLUMN IdEntidad INT DEFAULT 1 COMMENT 'FK a cat_entidades (multi-tenant)',
ADD INDEX idx_usuarios_entidad (IdEntidad),
ADD FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id);
```

### 3. Sistema No Filtra por IdEntidad

**Problema:** Todas las queries deberían filtrar por `IdEntidad` pero no lo hacen

**Impacto:** 
- ❌ Usuario de Entidad A puede ver datos de Entidad B
- ❌ No hay aislamiento de datos entre entidades
- ❌ Sistema multi-tenant no funciona

**Solución:** Agregar filtro en **TODAS** las queries:

```vb
' En ApiConsumerCRUD.vb o en cada página
Dim idEntidad = SessionHelper.GetIdEntidad()
Dim query = $"SELECT * FROM tabla WHERE IdEntidad = {idEntidad} AND ..."
```

### 4. No Hay Roles ni Permisos en Sesión

**Problema:** El sistema tiene tablas de roles (`conf_usuarioroles`, `conf_rolopciones`) pero no se cargan en sesión

**Impacto:**
- ❌ No se puede validar permisos en tiempo real
- ❌ No se puede ocultar opciones según rol
- ❌ No se puede restringir acciones según permisos

**Solución:** Cargar roles y permisos durante el login

---

## 💡 PROPUESTA DE MEJORA

### Fase 1: Completar Multi-Tenant (URGENTE)

**Objetivo:** Hacer que el sistema realmente filtre por `IdEntidad`

**Tareas:**

1. **Agregar campo IdEntidad a conf_usuarios**
   ```sql
   ALTER TABLE conf_usuarios ADD COLUMN IdEntidad INT DEFAULT 1;
   ```

2. **Actualizar JwtAuthService para obtener IdEntidad**
   ```csharp
   var query = @"
       SELECT u.Id, u.Username, u.Nombre, u.email, u.PasswordHash, 
              u.IdEntidad, e.Nombre as EntidadNombre
       FROM conf_usuarios u
       LEFT JOIN cat_entidades e ON u.IdEntidad = e.Id
       WHERE u.Username = @username AND u.Activo = 1";
   ```

3. **Crear helper para agregar filtro de entidad automáticamente**
   ```vb
   Public Class QueryHelper
       Public Shared Function AddEntidadFilter(query As String) As String
           Dim idEntidad = SessionHelper.GetIdEntidad()
           If query.ToUpper().Contains("WHERE") Then
               Return query & $" AND IdEntidad = {idEntidad}"
           Else
               Return query & $" WHERE IdEntidad = {idEntidad}"
           End If
       End Function
   End Class
   ```

4. **Actualizar ApiConsumerCRUD para agregar filtro automático**
   ```vb
   Public Function ObtenerDatos(tabla As String) As List(Of DynamicDto)
       Dim idEntidad = SessionHelper.GetIdEntidad()
       Dim query = $"SELECT * FROM {tabla} WHERE IdEntidad = {idEntidad}"
       ' ...
   End Function
   ```

**Estimación:** 2-3 días

---

### Fase 2: Implementar Roles y Permisos (IMPORTANTE)

**Objetivo:** Cargar roles y permisos del usuario en sesión

**Tareas:**

1. **Agregar constantes de sesión**
   ```vb
   Public Const SESSION_ROLES As String = "Roles"
   Public Const SESSION_PERMISOS As String = "Permisos"
   ```

2. **Actualizar AuthService para obtener roles**
   ```vb
   Private Function ObtenerRolesUsuario(userId As Integer) As JArray
       Dim query = "SELECT r.Id, r.Nombre FROM conf_roles r " &
                   "INNER JOIN conf_usuarioroles ur ON r.Id = ur.RolId " &
                   "WHERE ur.UsuarioId = @userId AND r.Activo = 1"
       ' ...
   End Function
   ```

3. **Actualizar SessionHelper**
   ```vb
   Public Shared Sub InitializeSession(userId, nombre, opciones, idEntidad, roles, permisos)
       session(Constants.SESSION_ROLES) = roles
       session(Constants.SESSION_PERMISOS) = permisos
       ' ...
   End Sub

   Public Shared Function GetRoles() As JArray
   Public Shared Function HasRole(roleName As String) As Boolean
   Public Shared Function HasPermiso(permiso As String) As Boolean
   ```

4. **Crear AuthorizationHelper**
   ```vb
   Public Class AuthorizationHelper
       Public Shared Function CanAccess(recurso As String) As Boolean
       Public Shared Function CanEdit(recurso As String) As Boolean
       Public Shared Function CanDelete(recurso As String) As Boolean
   End Class
   ```

**Estimación:** 3-4 días

---

### Fase 3: Implementar Jerarquías (OPCIONAL)

**Objetivo:** Agregar jerarquías organizacionales (supervisor, departamento, etc.)

**Tareas:**

1. **Agregar campos a conf_usuarios**
   ```sql
   ALTER TABLE conf_usuarios
   ADD COLUMN IdSupervisor INT NULL,
   ADD COLUMN IdDepartamento INT NULL,
   ADD COLUMN NivelJerarquico INT DEFAULT 1;
   ```

2. **Cargar jerarquía en sesión**
   ```vb
   Public Const SESSION_SUPERVISOR_ID As String = "SupervisorId"
   Public Const SESSION_DEPARTAMENTO_ID As String = "DepartamentoId"
   Public Const SESSION_NIVEL_JERARQUICO As String = "NivelJerarquico"
   ```

3. **Crear HierarchyHelper**
   ```vb
   Public Class HierarchyHelper
       Public Shared Function GetSubordinados() As List(Of Integer)
       Public Shared Function IsSupervisorOf(userId As Integer) As Boolean
       Public Shared Function CanApprove(monto As Decimal) As Boolean
   End Class
   ```

**Estimación:** 4-5 días

---

## 📊 COMPARACIÓN: ANTES vs DESPUÉS

### Información en Sesión

| Dato | Antes | Después Fase 1 | Después Fase 2 | Después Fase 3 |
|------|-------|----------------|----------------|----------------|
| UserId | ✅ | ✅ | ✅ | ✅ |
| Nombre | ✅ | ✅ | ✅ | ✅ |
| Email | ❌ | ✅ | ✅ | ✅ |
| IdEntidad | ⚠️ (guardado pero no usado) | ✅ (usado) | ✅ | ✅ |
| EntidadNombre | ❌ | ✅ | ✅ | ✅ |
| Roles | ❌ | ❌ | ✅ | ✅ |
| Permisos | ❌ | ❌ | ✅ | ✅ |
| SupervisorId | ❌ | ❌ | ❌ | ✅ |
| DepartamentoId | ❌ | ❌ | ❌ | ✅ |
| NivelJerarquico | ❌ | ❌ | ❌ | ✅ |

### Capacidades del Sistema

| Capacidad | Antes | Después Fase 1 | Después Fase 2 | Después Fase 3 |
|-----------|-------|----------------|----------------|----------------|
| Multi-tenant | ❌ | ✅ | ✅ | ✅ |
| Aislamiento de datos | ❌ | ✅ | ✅ | ✅ |
| Control de acceso por rol | ❌ | ❌ | ✅ | ✅ |
| Permisos granulares | ❌ | ❌ | ✅ | ✅ |
| Aprobaciones jerárquicas | ❌ | ❌ | ❌ | ✅ |
| Delegación de autoridad | ❌ | ❌ | ❌ | ✅ |

---

## 🎯 RECOMENDACIÓN FINAL

### Prioridad ALTA (Implementar YA)

**Fase 1: Completar Multi-Tenant**

**Razón:** El sistema actualmente **NO aísla datos entre entidades**. Esto es un **riesgo de seguridad crítico**.

**Impacto:**
- ✅ Aislamiento real de datos entre entidades
- ✅ Sistema multi-tenant funcional
- ✅ Seguridad mejorada
- ✅ Base para futuras mejoras

**Estimación:** 2-3 días

### Prioridad MEDIA (Implementar Pronto)

**Fase 2: Roles y Permisos**

**Razón:** Necesario para control de acceso granular y seguridad.

**Estimación:** 3-4 días

### Prioridad BAJA (Implementar Después)

**Fase 3: Jerarquías**

**Razón:** Nice to have, pero no crítico para operación básica.

**Estimación:** 4-5 días

---

## 📝 CONCLUSIÓN

### Estado Actual

El sistema de autenticación de JELABBC:

✅ **Funciona correctamente** para login básico  
✅ **Guarda IdEntidad** en sesión  
❌ **NO usa IdEntidad** para filtrar datos  
❌ **NO implementa** multi-tenant real  
❌ **NO carga roles** ni permisos en sesión  
❌ **NO tiene jerarquías** organizacionales

### Acción Requerida

**URGENTE:** Implementar Fase 1 (Multi-Tenant) para:
1. Agregar campo `IdEntidad` a `conf_usuarios`
2. Actualizar API para obtener `IdEntidad`
3. Filtrar **TODAS** las queries por `IdEntidad`
4. Validar aislamiento de datos

**Tiempo estimado:** 2-3 días  
**Impacto:** CRÍTICO para seguridad

---

**Creado por:** Kiro AI  
**Fecha:** 20 de Enero de 2026  
**Estado:** ⏳ PENDIENTE DE REVISIÓN
