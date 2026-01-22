# Documento de Diseño - Panel Admin JELA

## Resumen

El Panel Admin JELA es una aplicación web ASP.NET WebForms (VB.NET Framework 4.8) que proporciona a los super-administradores de JELA herramientas para gestionar administradores de condominios, controlar licencias, automatizar la configuración de entidades y monitorear el uso del sistema. La aplicación se integra con la API existente JELA.API (.NET 8 C#) y comparte la base de datos MySQL `jela_qa` con JelaWeb.

## Arquitectura

### Arquitectura General

```
┌─────────────────────────────────────────────────────────────┐
│                    Panel Admin JELA                          │
│                  (ASP.NET WebForms VB.NET)                   │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Dashboard  │  │ Administra-  │  │  Entidades   │      │
│  │              │  │   dores      │  │              │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│                                                               │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │  Licencias   │  │  Auditoría   │  │ Configuración│      │
│  │              │  │              │  │              │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│                                                               │
└───────────────────────────┬─────────────────────────────────┘
                            │
                            │ HTTP/REST (Dynamic CRUD API)
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      JELA.API                                │
│                    (.NET 8 C# API)                           │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────────────────────────────────────────────┐   │
│  │        API CRUD Dinámica (YA EXISTE)                 │   │
│  │  - GET /api/crud?strQuery=SELECT...                  │   │
│  │  - POST /api/crud/{tabla}                            │   │
│  │  - PUT /api/crud/{tabla}/{id}                        │   │
│  │  - DELETE /api/crud/{tabla}/{id}                     │   │
│  │                                                       │   │
│  │  Usa CrudDto con Dictionary<string, CampoConTipo>    │   │
│  │  NO requiere DTOs fijos ni controladores específicos │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                               │
│  ┌──────────────────────────────────────────────────────┐   │
│  │     Servicios de Lógica de Negocio (NUEVOS)         │   │
│  │  - ConfiguracionEntidadService                       │   │
│  │  - LicenciaService                                   │   │
│  │  - AuditoriaService                                  │   │
│  │  - AlertasService                                    │   │
│  │                                                       │   │
│  │  Solo para operaciones complejas que requieren      │   │
│  │  transacciones o lógica de negocio                  │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                               │
│  ┌──────────────────────────────────────────────────────┐   │
│  │        MySqlDatabaseService (YA EXISTE)              │   │
│  │  - EjecutarConsultaAsync()                           │   │
│  │  - InsertarAsync()                                   │   │
│  │  - ActualizarAsync()                                 │   │
│  │  - EliminarAsync()                                   │   │
│  └──────────────────────────────────────────────────────┘   │
│                                                               │
└───────────────────────────────┬─────────────────────────────┘
                            │
                            │ MySQL
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   Base de Datos MySQL                        │
│                       jela_qa                                │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Tablas Existentes:          Tablas Nuevas:                 │
│  - conf_usuarios             - conf_licencias                │
│  - conf_usuario_entidades    - conf_auditoria_admin          │
│  - cat_entidades             - conf_sistema                  │
│  - conf_ticket_prompts       - conf_alertas_licencias        │
│  - conf_ticket_sla                                           │
│  - cat_categorias_ticket                                     │
│  - cat_areas_comunes                                         │
│  - cat_conceptos_cuota                                       │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

### Patrón de Arquitectura

El sistema sigue una arquitectura en capas con **API CRUD Dinámica**:

1. **Capa de Presentación (Panel Admin JELA)**
   - Páginas ASP.NET WebForms (.aspx)
   - Code-behind en VB.NET (.aspx.vb)
   - Componentes DevExpress v22.2
   - CSS y JavaScript separados
   - **Genera columnas dinámicamente desde DataTable**

2. **Capa de API (JELA.API) - CRUD Dinámico**
   - **API CRUD Dinámica YA EXISTE** en `/api/crud`
   - **NO se crean DTOs fijos ni controladores específicos**
   - Usa `CrudDto` con `Dictionary<string, CampoConTipo>` para flexibilidad
   - Servicios de negocio SOLO para operaciones complejas (transacciones, configuración automática)
   - Validación y lógica de negocio en servicios cuando sea necesario

3. **Capa de Datos**
   - `MySqlDatabaseService` YA EXISTE con métodos dinámicos
   - Stored procedures MySQL para operaciones complejas
   - Gestión de transacciones
   - Integridad referencial

### Principios de Diseño

1. **API Dinámica**: Usar `/api/crud` existente para todas las operaciones CRUD simples
2. **Sin DTOs Fijos**: Usar `CrudDto` con diccionarios dinámicos en lugar de clases fijas
3. **Columnas Dinámicas**: Los grids generan columnas desde el DataTable, no desde definiciones estáticas
4. **Servicios Solo para Lógica Compleja**: Crear servicios solo cuando se necesite transaccionalidad o lógica de negocio
5. **Transaccionalidad**: Operaciones complejas usan transacciones para garantizar integridad
6. **Auditoría Completa**: Todas las operaciones administrativas se registran
7. **Validación en Capas**: Validación en UI, servicios de negocio y base de datos
8. **Reutilización**: Uso de componentes y helpers compartidos

## Componentes e Interfaces

### Enfoque de API Dinámica

**CRÍTICO**: Este sistema usa la **API CRUD Dinámica existente** en `/api/crud`. **NO se crean DTOs fijos ni controladores específicos** para administradores, entidades o licencias.

### API CRUD Dinámica (YA EXISTE)

La API ya tiene endpoints dinámicos que manejan todas las operaciones CRUD:

#### Endpoints Disponibles

```
GET /api/crud?strQuery=SELECT...
  - Ejecuta cualquier consulta SELECT
  - Retorna List<CrudDto> con campos dinámicos
  - Ejemplo: ?strQuery=SELECT * FROM conf_usuarios WHERE TipoUsuario='AdministradorCondominios'

POST /api/crud/{tabla}
  - Inserta registro en la tabla especificada
  - Body: CrudRequest con Dictionary<string, CampoConTipo>
  - Retorna: { id: number, mensaje: string }
  - Ejemplo: POST /api/crud/conf_usuarios

PUT /api/crud/{tabla}/{id}
  - Actualiza registro por ID
  - Body: CrudRequest con Dictionary<string, CampoConTipo>
  - Retorna: mensaje de éxito
  - Ejemplo: PUT /api/crud/conf_usuarios/5

DELETE /api/crud/{tabla}/{id}
  - Elimina registro por ID
  - Retorna: mensaje de éxito
  - Ejemplo: DELETE /api/crud/conf_usuarios/5
```

#### Modelo Dinámico (YA EXISTE)

```csharp
// CrudDto - Representa cualquier registro de cualquier tabla
public class CrudDto
{
    public Dictionary<string, CampoConTipo> Campos { get; set; } = new();
    
    public object? this[string key]
    {
        get => Campos.TryGetValue(key, out var campo) ? campo.Valor : null;
        set
        {
            var tipo = value?.GetType().FullName ?? "System.Object";
            Campos[key] = new CampoConTipo { Valor = value, Tipo = tipo };
        }
    }
}

// CampoConTipo - Representa un campo con su valor y tipo
public class CampoConTipo
{
    public object? Valor { get; set; }
    public string Tipo { get; set; } = "System.String";
}

// CrudRequest - Para operaciones POST/PUT
public class CrudRequest
{
    public Dictionary<string, CampoConTipo> Campos { get; set; } = new();
}
```

### Componentes de Presentación (WebForms)

Todas las páginas WebForms siguen el mismo patrón:

1. **Cargar datos**: Llamar a `/api/crud?strQuery=SELECT...`
2. **Generar columnas dinámicamente**: Usar `GenerarColumnasDinamicas(grid, dataTable)`
3. **Operaciones CRUD**: Llamar a POST/PUT/DELETE en `/api/crud/{tabla}`
4. **Aplicar estándares**: Usar `FuncionesGridWeb.SUMColumn` en DataBound

#### 1. Dashboard.aspx
**Propósito**: Página principal que muestra métricas del sistema y alertas

**Componentes**:
- Tarjetas de métricas (total administradores, entidades, licencias)
- Grid de alertas activas (ASPxGridView)
- Gráficos de uso (opcional)

**Interacciones**:
- Carga métricas desde `/api/dashboard/metricas`
- Carga alertas desde `/api/alertas/activas`
- Actualización automática cada 30 segundos (opcional)

#### 2. Administradores.aspx
**Propósito**: Gestión CRUD de administradores de condominios

**Componentes**:
- ASPxGridView con columnas dinámicas
- ASPxPopupControl para crear/editar
- Toolbar con acciones (Nuevo, Editar, Eliminar, Exportar)

**Campos del formulario**:
- Usuario (ASPxTextBox, requerido, único)
- Email (ASPxTextBox, requerido, único, validación email)
- Nombre Completo (ASPxTextBox, requerido)
- Licencias Totales (ASPxSpinEdit, requerido, mínimo 1)
- Activo (ASPxCheckBox)

**Interacciones**:
- GET `/api/administradores` - Listar todos
- POST `/api/administradores` - Crear nuevo
- PUT `/api/administradores/{id}` - Actualizar
- DELETE `/api/administradores/{id}` - Eliminar
- GET `/api/administradores/{id}/entidades` - Ver entidades asignadas

#### 3. Entidades.aspx
**Propósito**: Gestión CRUD de entidades (condominios)

**Componentes**:
- ASPxGridView con columnas dinámicas
- ASPxPopupControl para crear/editar
- Toolbar con acciones

**Campos del formulario**:
- Clave (ASPxTextBox, requerido, único, máx 50 caracteres)
- Alias (ASPxTextBox, requerido, máx 100 caracteres)
- Razón Social (ASPxTextBox, requerido, máx 255 caracteres)
- Tipo Condominio (ASPxComboBox: Vertical, Horizontal, Mixto)
- Número de Unidades (ASPxSpinEdit, opcional)
- Fecha Constitución (ASPxDateEdit, opcional)
- Activo (ASPxCheckBox)

**Interacciones**:
- GET `/api/entidades` - Listar todas
- POST `/api/entidades` - Crear nueva (incluye configuración automática)
- PUT `/api/entidades/{id}` - Actualizar
- DELETE `/api/entidades/{id}` - Eliminar
- GET `/api/entidades/{id}/administradores` - Ver administradores asignados

#### 4. AsignacionEntidades.aspx
**Propósito**: Asignar/desasignar entidades a administradores

**Componentes**:
- ASPxComboBox para seleccionar administrador
- ASPxGridView con entidades disponibles
- ASPxGridView con entidades asignadas
- Botones para asignar/desasignar

**Interacciones**:
- GET `/api/administradores/{id}/entidades-disponibles` - Entidades no asignadas
- GET `/api/administradores/{id}/entidades-asignadas` - Entidades asignadas
- POST `/api/administradores/{id}/asignar-entidad` - Asignar
- DELETE `/api/administradores/{id}/desasignar-entidad/{idEntidad}` - Desasignar

#### 5. Licencias.aspx
**Propósito**: Gestión de licencias por administrador

**Componentes**:
- ASPxGridView mostrando administrador, licencias totales, consumidas, disponibles
- ASPxPopupControl para modificar licencias
- Indicadores visuales (badges) para alertas

**Campos del formulario**:
- Administrador (solo lectura)
- Licencias Actuales (solo lectura)
- Licencias Consumidas (solo lectura)
- Nuevas Licencias Totales (ASPxSpinEdit, mínimo = licencias consumidas)

**Interacciones**:
- GET `/api/licencias` - Listar todas
- PUT `/api/licencias/{idAdministrador}` - Actualizar licencias

#### 6. Auditoria.aspx
**Propósito**: Visualización de registro de auditoría

**Componentes**:
- Filtros de fecha (ASPxDateEdit para rango)
- Filtro de tipo de operación (ASPxComboBox)
- ASPxGridView con registros de auditoría
- ASPxPopupControl para ver detalles JSON

**Interacciones**:
- GET `/api/auditoria?fechaInicio={fecha}&fechaFin={fecha}&tipoOperacion={tipo}` - Listar registros

#### 7. Configuracion.aspx
**Propósito**: Configuración global del sistema

**Componentes**:
- ASPxComboBox para seleccionar entidad plantilla
- Botón Guardar

**Interacciones**:
- GET `/api/configuracion` - Obtener configuración actual
- PUT `/api/configuracion` - Actualizar configuración

### Componentes de API (JELA.API)

#### Controladores

##### AdministradoresController
```csharp
[ApiController]
[Route("api/administradores")]
public class AdministradoresController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar()
    
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearAdministradorDto dto)
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarAdministradorDto dto)
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    
    [HttpGet("{id}/entidades")]
    public async Task<IActionResult> ObtenerEntidadesAsignadas(int id)
    
    [HttpGet("{id}/entidades-disponibles")]
    public async Task<IActionResult> ObtenerEntidadesDisponibles(int id)
    
    [HttpPost("{id}/asignar-entidad")]
    public async Task<IActionResult> AsignarEntidad(int id, [FromBody] AsignarEntidadDto dto)
    
    [HttpDelete("{id}/desasignar-entidad/{idEntidad}")]
    public async Task<IActionResult> DesasignarEntidad(int id, int idEntidad)
}
```

##### EntidadesController
```csharp
[ApiController]
[Route("api/entidades")]
public class EntidadesController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar()
    
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    
    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] CrearEntidadDto dto)
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEntidadDto dto)
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(int id)
    
    [HttpGet("{id}/administradores")]
    public async Task<IActionResult> ObtenerAdministradoresAsignados(int id)
}
```

##### LicenciasController
```csharp
[ApiController]
[Route("api/licencias")]
public class LicenciasController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar()
    
    [HttpGet("{idAdministrador}")]
    public async Task<IActionResult> ObtenerPorAdministrador(int idAdministrador)
    
    [HttpPut("{idAdministrador}")]
    public async Task<IActionResult> ActualizarLicencias(int idAdministrador, [FromBody] ActualizarLicenciasDto dto)
}
```

##### DashboardController
```csharp
[ApiController]
[Route("api/dashboard")]
public class DashboardController : ControllerBase
{
    [HttpGet("metricas")]
    public async Task<IActionResult> ObtenerMetricas()
}
```

##### AuditoriaController
```csharp
[ApiController]
[Route("api/auditoria")]
public class AuditoriaController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] DateTime? fechaInicio,
        [FromQuery] DateTime? fechaFin,
        [FromQuery] string tipoOperacion)
}
```

##### AlertasController
```csharp
[ApiController]
[Route("api/alertas")]
public class AlertasController : ControllerBase
{
    [HttpGet("activas")]
    public async Task<IActionResult> ObtenerAlertasActivas()
    
    [HttpPut("{id}/resolver")]
    public async Task<IActionResult> ResolverAlerta(int id)
}
```

##### ConfiguracionController
```csharp
[ApiController]
[Route("api/configuracion")]
public class ConfiguracionController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ObtenerConfiguracion()
    
    [HttpPut]
    public async Task<IActionResult> ActualizarConfiguracion([FromBody] ActualizarConfiguracionDto dto)
}
```

#### Servicios de Negocio

##### AdministradorService
```csharp
public class AdministradorService
{
    public async Task<List<AdministradorDto>> ListarTodos()
    public async Task<AdministradorDto> ObtenerPorId(int id)
    public async Task<int> Crear(CrearAdministradorDto dto, int idUsuarioCreacion)
    public async Task Actualizar(int id, ActualizarAdministradorDto dto, int idUsuarioModificacion)
    public async Task Eliminar(int id, int idUsuarioEliminacion)
    public async Task<bool> TieneEntidadesAsignadas(int id)
    public async Task<bool> ExisteUsuario(string usuario)
    public async Task<bool> ExisteEmail(string email)
}
```

##### EntidadService
```csharp
public class EntidadService
{
    public async Task<List<EntidadDto>> ListarTodas()
    public async Task<EntidadDto> ObtenerPorId(int id)
    public async Task<int> Crear(CrearEntidadDto dto, int idUsuarioCreacion)
    public async Task Actualizar(int id, ActualizarEntidadDto dto, int idUsuarioModificacion)
    public async Task Eliminar(int id, int idUsuarioEliminacion)
    public async Task<bool> TieneAdministradoresAsignados(int id)
    public async Task<bool> ExisteClave(string clave)
    public async Task ConfigurarEntidadNueva(int idEntidad, int idEntidadPlantilla)
}
```

##### LicenciaService
```csharp
public class LicenciaService
{
    public async Task<List<LicenciaDto>> ListarTodas()
    public async Task<LicenciaDto> ObtenerPorAdministrador(int idAdministrador)
    public async Task CrearLicencia(int idAdministrador, int licenciasTotales)
    public async Task ActualizarLicencias(int idAdministrador, int nuevasLicenciasTotales, int idUsuarioModificacion)
    public async Task IncrementarConsumo(int idAdministrador)
    public async Task DecrementarConsumo(int idAdministrador)
    public async Task<bool> TieneLicenciasDisponibles(int idAdministrador)
    public async Task<int> ObtenerLicenciasDisponibles(int idAdministrador)
    public async Task VerificarYCrearAlertas(int idAdministrador)
}
```

##### AsignacionService
```csharp
public class AsignacionService
{
    public async Task AsignarEntidad(int idAdministrador, int idEntidad, int idUsuarioAsignacion)
    public async Task DesasignarEntidad(int idAdministrador, int idEntidad, int idUsuarioDesasignacion)
    public async Task<bool> ExisteAsignacion(int idAdministrador, int idEntidad)
    public async Task<List<EntidadDto>> ObtenerEntidadesAsignadas(int idAdministrador)
    public async Task<List<EntidadDto>> ObtenerEntidadesDisponibles(int idAdministrador)
}
```

##### AuditoriaService
```csharp
public class AuditoriaService
{
    public async Task RegistrarOperacion(
        int idUsuario,
        string tipoOperacion,
        string descripcion,
        string datosAnteriores = null)
    
    public async Task<List<AuditoriaDto>> ListarRegistros(
        DateTime? fechaInicio,
        DateTime? fechaFin,
        string tipoOperacion)
}
```

##### ConfiguracionService
```csharp
public class ConfiguracionService
{
    public async Task<ConfiguracionDto> ObtenerConfiguracion()
    public async Task ActualizarConfiguracion(ActualizarConfiguracionDto dto, int idUsuarioModificacion)
    public async Task<int> ObtenerIdEntidadPlantilla()
}
```

## Modelos de Datos

### Tablas Existentes (Modificadas)

#### conf_usuarios
```sql
-- Sin cambios en estructura, solo se usa TipoUsuario = 'AdministradorCondominios'
```

#### conf_usuario_entidades
```sql
-- Sin cambios en estructura
-- Relación muchos a muchos entre usuarios y entidades
```

#### cat_entidades
```sql
-- Sin cambios en estructura
-- Almacena información de condominios
```

### Tablas Nuevas

#### conf_licencias
```sql
CREATE TABLE conf_licencias (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdAdministrador INT NOT NULL,
    LicenciasTotales INT NOT NULL DEFAULT 0,
    LicenciasConsumidas INT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FechaModificacion DATETIME NULL,
    FOREIGN KEY (IdAdministrador) REFERENCES conf_usuarios(Id),
    INDEX idx_administrador (IdAdministrador)
);
```

**Campos**:
- `Id`: Identificador único
- `IdAdministrador`: FK a conf_usuarios
- `LicenciasTotales`: Número total de licencias asignadas
- `LicenciasConsumidas`: Número de licencias en uso
- `FechaCreacion`: Timestamp de creación
- `FechaModificacion`: Timestamp de última modificación

**Reglas de Negocio**:
- `LicenciasConsumidas` <= `LicenciasTotales`
- `LicenciasTotales` >= `LicenciasConsumidas` (no se puede reducir por debajo del consumo)
- Licencias disponibles = `LicenciasTotales - LicenciasConsumidas`

#### conf_auditoria_admin
```sql
CREATE TABLE conf_auditoria_admin (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdUsuario INT NOT NULL,
    FechaOperacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TipoOperacion VARCHAR(50) NOT NULL,
    Descripcion TEXT NOT NULL,
    DatosAnteriores JSON NULL,
    FOREIGN KEY (IdUsuario) REFERENCES conf_usuarios(Id),
    INDEX idx_fecha (FechaOperacion),
    INDEX idx_tipo (TipoOperacion),
    INDEX idx_usuario (IdUsuario)
);
```

**Campos**:
- `Id`: Identificador único
- `IdUsuario`: FK a conf_usuarios (super admin que realizó la operación)
- `FechaOperacion`: Timestamp de la operación
- `TipoOperacion`: Tipo de operación (CrearAdministrador, ModificarAdministrador, etc.)
- `Descripcion`: Descripción legible de la operación
- `DatosAnteriores`: JSON con datos antes de la modificación (para rollback)

**Tipos de Operación**:
- `CrearAdministrador`
- `ModificarAdministrador`
- `EliminarAdministrador`
- `CrearEntidad`
- `ModificarEntidad`
- `EliminarEntidad`
- `AsignarEntidad`
- `DesasignarEntidad`
- `ModificarLicencias`
- `ModificarConfiguracion`

#### conf_sistema
```sql
CREATE TABLE conf_sistema (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdEntidadPlantilla INT NOT NULL,
    FechaModificacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IdEntidadPlantilla) REFERENCES cat_entidades(Id),
    INDEX idx_plantilla (IdEntidadPlantilla)
);
```

**Campos**:
- `Id`: Identificador único (solo debe haber 1 registro)
- `IdEntidadPlantilla`: FK a cat_entidades (entidad usada como plantilla)
- `FechaModificacion`: Timestamp de última modificación

**Inicialización**:
```sql
INSERT INTO conf_sistema (IdEntidadPlantilla) VALUES (1);
```

#### conf_alertas_licencias
```sql
CREATE TABLE conf_alertas_licencias (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdAdministrador INT NOT NULL,
    TipoAlerta ENUM('Advertencia', 'Critica') NOT NULL,
    Mensaje TEXT NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Resuelta TINYINT(1) NOT NULL DEFAULT 0,
    FechaResolucion DATETIME NULL,
    FOREIGN KEY (IdAdministrador) REFERENCES conf_usuarios(Id),
    INDEX idx_administrador (IdAdministrador),
    INDEX idx_resuelta (Resuelta),
    INDEX idx_fecha (FechaCreacion)
);
```

**Campos**:
- `Id`: Identificador único
- `IdAdministrador`: FK a conf_usuarios
- `TipoAlerta`: Tipo de alerta (Advertencia o Crítica)
- `Mensaje`: Mensaje descriptivo de la alerta
- `FechaCreacion`: Timestamp de creación
- `Resuelta`: Indica si la alerta fue resuelta (0 = no, 1 = sí)
- `FechaResolucion`: Timestamp de resolución

**Reglas de Creación**:
- Advertencia: Cuando licencias disponibles < 2
- Crítica: Cuando licencias disponibles = 0

### DTOs (Data Transfer Objects)

#### AdministradorDto
```csharp
public class AdministradorDto
{
    public int Id { get; set; }
    public string Usuario { get; set; }
    public string Email { get; set; }
    public string NombreCompleto { get; set; }
    public int LicenciasTotales { get; set; }
    public int LicenciasConsumidas { get; set; }
    public int LicenciasDisponibles { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}
```

#### CrearAdministradorDto
```csharp
public class CrearAdministradorDto
{
    [Required]
    [MaxLength(50)]
    public string Usuario { get; set; }
    
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string NombreCompleto { get; set; }
    
    [Required]
    [Range(1, int.MaxValue)]
    public int LicenciasTotales { get; set; }
    
    [Required]
    public string Password { get; set; }
}
```

#### ActualizarAdministradorDto
```csharp
public class ActualizarAdministradorDto
{
    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string NombreCompleto { get; set; }
    
    public bool Activo { get; set; }
}
```

#### EntidadDto
```csharp
public class EntidadDto
{
    public int Id { get; set; }
    public string Clave { get; set; }
    public string Alias { get; set; }
    public string RazonSocial { get; set; }
    public string TipoCondominio { get; set; }
    public int? NumeroUnidades { get; set; }
    public DateTime? FechaConstitucion { get; set; }
    public bool Activo { get; set; }
    public int NumeroAdministradores { get; set; }
}
```

#### CrearEntidadDto
```csharp
public class CrearEntidadDto
{
    [Required]
    [MaxLength(50)]
    public string Clave { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string Alias { get; set; }
    
    [Required]
    [MaxLength(255)]
    public string RazonSocial { get; set; }
    
    [Required]
    public string TipoCondominio { get; set; }
    
    public int? NumeroUnidades { get; set; }
    public DateTime? FechaConstitucion { get; set; }
}
```

#### LicenciaDto
```csharp
public class LicenciaDto
{
    public int IdAdministrador { get; set; }
    public string NombreAdministrador { get; set; }
    public int LicenciasTotales { get; set; }
    public int LicenciasConsumidas { get; set; }
    public int LicenciasDisponibles { get; set; }
    public List<string> EntidadesAsignadas { get; set; }
}
```

#### DashboardMetricasDto
```csharp
public class DashboardMetricasDto
{
    public int TotalAdministradores { get; set; }
    public int TotalEntidades { get; set; }
    public int EntidadesActivas { get; set; }
    public int EntidadesInactivas { get; set; }
    public int TotalLicenciasAsignadas { get; set; }
    public int TotalLicenciasConsumidas { get; set; }
    public int TotalLicenciasDisponibles { get; set; }
    public int AlertasActivas { get; set; }
}
```

#### AuditoriaDto
```csharp
public class AuditoriaDto
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; }
    public DateTime FechaOperacion { get; set; }
    public string TipoOperacion { get; set; }
    public string Descripcion { get; set; }
    public string DatosAnteriores { get; set; }
}
```

## Propiedades de Correctitud

*Una propiedad es una característica o comportamiento que debe mantenerse verdadero en todas las ejecuciones válidas de un sistema - esencialmente, una declaración formal sobre lo que el sistema debe hacer. Las propiedades sirven como puente entre especificaciones legibles por humanos y garantías de correctitud verificables por máquina.*

### Propiedad 1: Autenticación de Super Administradores

*Para cualquier* usuario con credenciales válidas y `TipoUsuario = 'SuperAdministrador'`, el sistema debe otorgar acceso al panel admin.

**Valida: Requerimientos 1.2**

### Propiedad 2: Rechazo de Credenciales Inválidas

*Para cualquier* conjunto de credenciales que no coincidan con un usuario existente o tengan contraseña incorrecta, el sistema debe denegar el acceso y mostrar un mensaje de error.

**Valida: Requerimientos 1.3**

### Propiedad 3: Restricción de Acceso por Tipo de Usuario

*Para cualquier* usuario con credenciales válidas pero `TipoUsuario != 'SuperAdministrador'`, el sistema debe denegar el acceso al panel admin.

**Valida: Requerimientos 1.4**

### Propiedad 4: Cálculo Correcto de Métricas del Dashboard

*Para cualquier* estado de la base de datos, las métricas del dashboard deben calcularse correctamente:
- Total administradores = COUNT(conf_usuarios WHERE TipoUsuario = 'AdministradorCondominios')
- Total entidades = COUNT(cat_entidades)
- Licencias disponibles = SUM(LicenciasTotales - LicenciasConsumidas)

**Valida: Requerimientos 2.1, 2.2, 2.3, 2.4, 2.5**

### Propiedad 5: Creación de Administrador con Tipo Correcto

*Para cualquier* administrador creado a través del panel admin, el registro en `conf_usuarios` debe tener `TipoUsuario = 'AdministradorCondominios'`.

**Valida: Requerimientos 3.1**

### Propiedad 6: Validación de Campos Únicos

*Para cualquier* intento de crear un administrador con email o usuario que ya existe en `conf_usuarios`, el sistema debe rechazar la operación y mostrar un mensaje de error indicando qué campo está duplicado.

**Valida: Requerimientos 3.3, 3.4**

### Propiedad 7: Prevención de Eliminación con Entidades Asignadas

*Para cualquier* administrador que tenga al menos un registro en `conf_usuario_entidades`, el sistema debe prevenir su eliminación y mostrar un mensaje de error.

**Valida: Requerimientos 3.8**

### Propiedad 8: Creación Automática de Registro de Licencias

*Para cualquier* administrador creado, debe existir exactamente un registro correspondiente en `conf_licencias` con `IdAdministrador` igual al ID del administrador creado.

**Valida: Requerimientos 4.1**

### Propiedad 9: Incremento de Licencias Consumidas al Asignar

*Para cualquier* asignación de entidad a un administrador, el valor de `LicenciasConsumidas` en `conf_licencias` debe incrementarse en exactamente 1.

**Valida: Requerimientos 4.2**

### Propiedad 10: Decremento de Licencias Consumidas al Desasignar

*Para cualquier* desasignación de entidad de un administrador, el valor de `LicenciasConsumidas` en `conf_licencias` debe decrementarse en exactamente 1.

**Valida: Requerimientos 4.3, 5.7**

### Propiedad 11: Restricción de Reducción de Licencias

*Para cualquier* intento de actualizar `LicenciasTotales` a un valor menor que `LicenciasConsumidas` actual, el sistema debe rechazar la operación y mostrar un mensaje de error.

**Valida: Requerimientos 4.4**

### Propiedad 12: Prevención de Asignación sin Licencias Disponibles

*Para cualquier* administrador donde `LicenciasDisponibles = LicenciasTotales - LicenciasConsumidas <= 0`, el sistema debe prevenir nuevas asignaciones de entidades y mostrar un mensaje de error.

**Valida: Requerimientos 4.5, 5.2, 5.6**

### Propiedad 13: Cálculo Correcto de Licencias Disponibles

*Para cualquier* administrador, el valor mostrado de licencias disponibles debe ser exactamente `LicenciasTotales - LicenciasConsumidas`.

**Valida: Requerimientos 4.6**

### Propiedad 14: Actualización de Timestamp en Modificación de Licencias

*Para cualquier* modificación de `LicenciasTotales` en `conf_licencias`, el campo `FechaModificacion` debe actualizarse al timestamp actual de la operación.

**Valida: Requerimientos 4.7**

### Propiedad 15: Creación de Registro de Asignación

*Para cualquier* asignación de entidad a administrador, debe crearse exactamente un registro en `conf_usuario_entidades` con `IdUsuario` e `IdEntidad` correspondientes.

**Valida: Requerimientos 5.1**

### Propiedad 16: Prevención de Asignaciones Duplicadas

*Para cualquier* par (IdUsuario, IdEntidad) que ya existe en `conf_usuario_entidades`, el sistema debe prevenir crear un segundo registro con el mismo par y mostrar un mensaje de error.

**Valida: Requerimientos 5.3**

### Propiedad 17: Eliminación de Registro de Asignación

*Para cualquier* desasignación de entidad de administrador, el registro correspondiente en `conf_usuario_entidades` debe eliminarse completamente.

**Valida: Requerimientos 5.4**

### Propiedad 18: Configuración Completa de Nueva Entidad

*Para cualquier* entidad creada, el sistema debe copiar automáticamente:
- Todos los prompts de `conf_ticket_prompts` de la entidad plantilla
- Todos los SLAs de `conf_ticket_sla` de la entidad plantilla
- Todas las categorías de `cat_categorias_ticket` de la entidad plantilla

**Valida: Requerimientos 6.1, 6.3, 6.5**

### Propiedad 19: Prompts Mínimos Requeridos Presentes

*Para cualquier* entidad creada, deben existir al menos los siguientes prompts en `conf_ticket_prompts`:
- ChatWebSistema
- AnalisisTicket
- ResolucionTicket
- CategorizacionTicket

**Valida: Requerimientos 6.2**

### Propiedad 20: SLAs para Todas las Prioridades

*Para cualquier* entidad creada, deben existir registros en `conf_ticket_sla` para las 4 prioridades: Baja, Media, Alta, Crítica.

**Valida: Requerimientos 6.4**

### Propiedad 21: Áreas Comunes Predeterminadas Creadas

*Para cualquier* entidad creada, deben existir al menos 3 registros en `cat_areas_comunes` con nombres: Alberca, Salón de Eventos, Gimnasio.

**Valida: Requerimientos 6.6**

### Propiedad 22: Conceptos de Pago Predeterminados Creados

*Para cualquier* entidad creada, deben existir al menos 2 registros en `cat_conceptos_cuota` con nombres: Cuota de Mantenimiento, Agua.

**Valida: Requerimientos 6.7**

### Propiedad 23: Transaccionalidad en Configuración de Entidad

*Para cualquier* fallo durante la configuración de una entidad nueva, no debe quedar ningún dato parcial en las tablas de configuración (prompts, SLAs, categorías, áreas, conceptos). Todo debe revertirse o todo debe completarse.

**Valida: Requerimientos 6.8**

### Propiedad 24: Registro de Auditoría en Configuración de Entidad

*Para cualquier* entidad creada exitosamente, debe existir un registro en `conf_auditoria_admin` con `TipoOperacion = 'CrearEntidad'`.

**Valida: Requerimientos 6.9**

### Propiedad 25: Validación de Clave Única de Entidad

*Para cualquier* intento de crear una entidad con una `Clave` que ya existe en `cat_entidades`, el sistema debe rechazar la operación y mostrar un mensaje de error.

**Valida: Requerimientos 7.3**

### Propiedad 26: Prevención de Eliminación de Entidad con Administradores

*Para cualquier* entidad que tenga al menos un registro en `conf_usuario_entidades`, el sistema debe prevenir su eliminación y mostrar un mensaje de error.

**Valida: Requerimientos 7.8**

### Propiedad 27: Registro de Auditoría para Todas las Operaciones

*Para cualquier* operación administrativa (crear, modificar, eliminar administrador/entidad, asignar/desasignar entidad, modificar licencias), debe crearse un registro en `conf_auditoria_admin` con el `TipoOperacion` correspondiente, `IdUsuario` del super admin, y `FechaOperacion` actual.

**Valida: Requerimientos 8.1, 8.2, 8.3, 8.4, 8.5, 8.6, 8.7, 8.8**

### Propiedad 28: Almacenamiento de Datos Anteriores en Auditoría

*Para cualquier* operación de modificación o eliminación, el registro de auditoría debe incluir en `DatosAnteriores` (JSON) el estado completo del registro antes de la modificación.

**Valida: Requerimientos 8.9**

### Propiedad 29: Generación de Alertas por Licencias Bajas

*Para cualquier* administrador donde `LicenciasDisponibles < 2`, debe existir una alerta activa en `conf_alertas_licencias` con `TipoAlerta = 'Advertencia'` y `Resuelta = 0`.

**Valida: Requerimientos 9.1**

### Propiedad 30: Generación de Alertas Críticas por Licencias Agotadas

*Para cualquier* administrador donde `LicenciasDisponibles = 0`, debe existir una alerta activa en `conf_alertas_licencias` con `TipoAlerta = 'Critica'` y `Resuelta = 0`.

**Valida: Requerimientos 9.2**

### Propiedad 31: Resolución de Alertas al Agregar Licencias

*Para cualquier* alerta activa (`Resuelta = 0`) de un administrador, si se incrementan las licencias de modo que `LicenciasDisponibles >= 2`, la alerta debe marcarse como `Resuelta = 1` y `FechaResolucion` debe establecerse al timestamp actual.

**Valida: Requerimientos 9.4**

### Propiedad 32: Validación de Entidad Plantilla Activa

*Para cualquier* intento de establecer `IdEntidadPlantilla` en `conf_sistema`, la entidad referenciada debe existir en `cat_entidades` y tener `Activo = 1`.

**Valida: Requerimientos 10.4**

### Propiedad 33: Compatibilidad de Datos con JelaWeb

*Para cualquier* modificación en `conf_usuarios` o `conf_usuario_entidades`, los datos deben mantener la estructura y tipos esperados por JelaWeb (campos requeridos presentes, tipos de datos correctos, relaciones íntegras).

**Valida: Requerimientos 11.1, 11.2, 11.3**

### Propiedad 34: Integridad Transaccional en Operaciones de Base de Datos

*Para cualquier* operación que modifique múltiples tablas, si ocurre un error en cualquier paso, todos los cambios deben revertirse (rollback) y la base de datos debe quedar en el estado anterior a la operación.

**Valida: Requerimientos 11.5**

### Propiedad 35: Exportación Completa de Datos Visibles

*Para cualquier* exportación a Excel desde un grid, el archivo exportado debe contener todas las columnas visibles en el grid y todas las filas que pasan los filtros actuales.

**Valida: Requerimientos 13.4, 13.5**

### Propiedad 36: Validación de Formularios Antes de Envío

*Para cualquier* formulario con campos requeridos o con validaciones, el sistema debe prevenir el envío (submit) si alguna validación falla, y debe mostrar mensajes de error específicos para cada campo inválido.

**Valida: Requerimientos 14.2, 14.7**

### Propiedad 37: Mensajes de Error para Restricciones de Unicidad

*Para cualquier* violación de restricción UNIQUE en la base de datos, el sistema debe capturar el error y mostrar un mensaje amigable indicando específicamente qué campo debe ser único.

**Valida: Requerimientos 14.3**

### Propiedad 38: Mensajes de Error para Restricciones de Clave Foránea

*Para cualquier* violación de restricción FOREIGN KEY en la base de datos, el sistema debe capturar el error y mostrar un mensaje amigable explicando la dependencia que impide la operación.

**Valida: Requerimientos 14.4**

## Manejo de Errores

### Estrategia General

El sistema implementa manejo de errores en tres capas:

1. **Capa de Presentación (WebForms)**
   - Validación de entrada usando validadores DevExpress
   - Mensajes de error amigables para el usuario
   - Prevención de envío de formularios inválidos

2. **Capa de API**
   - Validación de DTOs usando Data Annotations
   - Manejo de excepciones con try-catch
   - Retorno de códigos HTTP apropiados (400, 404, 500)
   - Logging de errores detallados

3. **Capa de Datos**
   - Manejo de excepciones de base de datos
   - Rollback automático de transacciones en caso de error
   - Validación de restricciones (UNIQUE, FOREIGN KEY)

### Tipos de Errores y Respuestas

#### Errores de Validación (400 Bad Request)
- Campos requeridos faltantes
- Formato de email inválido
- Valores fuera de rango
- Longitud de texto excedida

**Respuesta**:
```json
{
  "error": "Validación fallida",
  "detalles": {
    "Email": ["El email es requerido", "Formato de email inválido"],
    "LicenciasTotales": ["Debe ser mayor a 0"]
  }
}
```

#### Errores de Reglas de Negocio (400 Bad Request)
- Licencias insuficientes
- Reducción de licencias por debajo del consumo
- Asignación duplicada
- Eliminación con dependencias

**Respuesta**:
```json
{
  "error": "No se puede asignar la entidad",
  "mensaje": "El administrador no tiene licencias disponibles. Licencias disponibles: 0"
}
```

#### Errores de Restricciones de Base de Datos (409 Conflict)
- Violación de UNIQUE constraint
- Violación de FOREIGN KEY constraint

**Respuesta**:
```json
{
  "error": "El email ya está registrado",
  "mensaje": "Ya existe un administrador con el email 'admin@ejemplo.com'"
}
```

#### Errores de Recurso No Encontrado (404 Not Found)
- Administrador no existe
- Entidad no existe
- Configuración no encontrada

**Respuesta**:
```json
{
  "error": "Recurso no encontrado",
  "mensaje": "No se encontró el administrador con ID 123"
}
```

#### Errores Internos del Servidor (500 Internal Server Error)
- Errores de base de datos no manejados
- Excepciones no capturadas
- Fallos de conexión

**Respuesta**:
```json
{
  "error": "Error interno del servidor",
  "mensaje": "Ocurrió un error inesperado. Por favor contacte al administrador.",
  "codigoError": "ERR-2024-001"
}
```

### Logging de Errores

Todos los errores se registran usando el sistema de logging existente:

```vb
Try
    ' Operación
Catch ex As Exception
    Logger.LogError("NombreMetodo", ex)
    ' Mostrar mensaje amigable al usuario
    MostrarError("Ocurrió un error al procesar la solicitud")
End Try
```

## Estrategia de Pruebas

### Enfoque Dual de Pruebas

El sistema requiere dos tipos complementarios de pruebas:

1. **Pruebas Unitarias**: Verifican ejemplos específicos, casos borde y condiciones de error
2. **Pruebas Basadas en Propiedades**: Verifican propiedades universales a través de todas las entradas

Ambos tipos son necesarios para cobertura completa. Las pruebas unitarias capturan bugs concretos, las pruebas de propiedades verifican correctitud general.

### Balance de Pruebas Unitarias

- Las pruebas unitarias son útiles para ejemplos específicos y casos borde
- Evitar escribir demasiadas pruebas unitarias - las pruebas de propiedades manejan la cobertura de muchas entradas
- Las pruebas unitarias deben enfocarse en:
  - Ejemplos específicos que demuestran comportamiento correcto
  - Puntos de integración entre componentes
  - Casos borde y condiciones de error
- Las pruebas de propiedades deben enfocarse en:
  - Propiedades universales que se mantienen para todas las entradas
  - Cobertura comprehensiva de entradas a través de aleatorización

### Configuración de Pruebas Basadas en Propiedades

**Biblioteca**: Para C# se usará **FsCheck** (port de QuickCheck para .NET)

**Configuración**:
- Mínimo 100 iteraciones por prueba de propiedad (debido a aleatorización)
- Cada prueba de propiedad debe referenciar su propiedad del documento de diseño
- Formato de etiqueta: `// Feature: panel-admin-jela, Property {número}: {texto de propiedad}`
- Cada propiedad de correctitud DEBE ser implementada por UNA SOLA prueba basada en propiedad

### Estructura de Pruebas

#### Pruebas de API (C# con xUnit y FsCheck)

```csharp
public class AdministradorServiceTests
{
    // Prueba unitaria - ejemplo específico
    [Fact]
    public void Crear_ConDatosValidos_CreaAdministrador()
    {
        // Arrange
        var dto = new CrearAdministradorDto
        {
            Usuario = "admin1",
            Email = "admin1@test.com",
            NombreCompleto = "Admin Test",
            LicenciasTotales = 5,
            Password = "Pass123!"
        };
        
        // Act
        var id = _service.Crear(dto, 1).Result;
        
        // Assert
        Assert.True(id > 0);
    }
    
    // Prueba basada en propiedad
    // Feature: panel-admin-jela, Property 5: Creación de Administrador con Tipo Correcto
    [Property]
    public Property Crear_SiempreAsignaTipoAdministradorCondominios()
    {
        return Prop.ForAll(
            GeneradorAdministrador.Valido(),
            dto =>
            {
                // Arrange & Act
                var id = _service.Crear(dto, 1).Result;
                var admin = _repository.ObtenerPorId(id).Result;
                
                // Assert
                return admin.TipoUsuario == "AdministradorCondominios";
            });
    }
    
    // Feature: panel-admin-jela, Property 6: Validación de Campos Únicos
    [Property]
    public Property Crear_ConEmailDuplicado_RechazaOperacion()
    {
        return Prop.ForAll(
            GeneradorAdministrador.Valido(),
            dto =>
            {
                // Arrange - crear primer administrador
                _service.Crear(dto, 1).Wait();
                
                // Act - intentar crear segundo con mismo email
                var dto2 = new CrearAdministradorDto
                {
                    Usuario = "otro_usuario",
                    Email = dto.Email, // Email duplicado
                    NombreCompleto = "Otro Admin",
                    LicenciasTotales = 3,
                    Password = "Pass123!"
                };
                
                // Assert
                return Throws<ValidationException>(() => _service.Crear(dto2, 1).Wait());
            });
    }
}
```

#### Generadores de Datos Aleatorios

```csharp
public static class GeneradorAdministrador
{
    public static Arbitrary<CrearAdministradorDto> Valido()
    {
        return Arb.From(
            from usuario in Gen.Elements("admin", "user", "manager")
                .SelectMany(prefix => Gen.Choose(1, 9999).Select(n => $"{prefix}{n}"))
            from nombre in Gen.Elements("Juan", "María", "Pedro", "Ana")
            from apellido in Gen.Elements("García", "López", "Martínez", "Rodríguez")
            from dominio in Gen.Elements("test.com", "ejemplo.com", "demo.com")
            from licencias in Gen.Choose(1, 50)
            select new CrearAdministradorDto
            {
                Usuario = usuario,
                Email = $"{usuario}@{dominio}",
                NombreCompleto = $"{nombre} {apellido}",
                LicenciasTotales = licencias,
                Password = "Pass123!"
            });
    }
}
```

### Pruebas de Integración

Las pruebas de integración verifican la interacción entre componentes:

1. **WebForms → API**: Verificar que las páginas llamen correctamente a los endpoints
2. **API → Base de Datos**: Verificar que las operaciones de BD se ejecuten correctamente
3. **Transacciones**: Verificar rollback en caso de error
4. **Auditoría**: Verificar que se registren todas las operaciones

### Pruebas de UI (Opcional)

Para verificar cumplimiento de estándares de UI:

1. **Estructura de archivos**: Verificar que CSS y JS estén en archivos separados
2. **Configuración de grids**: Verificar que todos los grids usen `ShowAllRecords`
3. **Filtros**: Verificar que solo fechas estén arriba de grids
4. **Columnas dinámicas**: Verificar que las columnas se generen desde el DataTable

### Cobertura de Pruebas

**Objetivo de cobertura**:
- Servicios de negocio: 90%+
- Repositorios: 80%+
- Controladores: 80%+
- Validaciones: 100%

**Métricas**:
- Líneas de código cubiertas
- Ramas cubiertas
- Propiedades de correctitud implementadas (38 propiedades)

### Ejecución de Pruebas

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar solo pruebas unitarias
dotnet test --filter Category=Unit

# Ejecutar solo pruebas de propiedades
dotnet test --filter Category=Property

# Ejecutar con cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Casos de Prueba Críticos

Los siguientes escenarios DEBEN tener pruebas específicas:

1. **Transaccionalidad**: Crear entidad con fallo en medio de configuración → verificar rollback completo
2. **Licencias**: Asignar entidad sin licencias disponibles → verificar rechazo
3. **Auditoría**: Realizar cualquier operación → verificar registro en auditoría
4. **Alertas**: Reducir licencias disponibles a 1 → verificar creación de alerta
5. **Unicidad**: Crear administrador con email duplicado → verificar rechazo
6. **Integridad referencial**: Eliminar administrador con entidades → verificar rechazo
7. **Configuración automática**: Crear entidad → verificar que tenga todos los prompts, SLAs, categorías
8. **Cálculos**: Verificar que licencias disponibles = totales - consumidas en todos los casos

## Consideraciones de Implementación

### Orden de Implementación Recomendado

1. **Fase 1: Infraestructura**
   - Crear tablas nuevas en base de datos
   - Configurar proyecto WebForms
   - Configurar endpoints en API
   - Implementar servicios base

2. **Fase 2: Funcionalidad Core**
   - Autenticación de super administradores
   - CRUD de administradores
   - Sistema de licencias
   - Asignación de entidades

3. **Fase 3: Configuración Automática**
   - Configuración de entidades nuevas
   - Copia de prompts, SLAs, catálogos
   - Transaccionalidad

4. **Fase 4: Auditoría y Monitoreo**
   - Sistema de auditoría
   - Dashboard con métricas
   - Sistema de alertas

5. **Fase 5: UI y Refinamiento**
   - Aplicar estándares de UI
   - Exportación a Excel
   - Validaciones y mensajes de error
   - Pruebas de integración

### Dependencias Externas

- **DevExpress v22.2**: Componentes de UI
- **MySQL Connector**: Conexión a base de datos
- **Dapper**: ORM para acceso a datos
- **FsCheck**: Pruebas basadas en propiedades
- **xUnit**: Framework de pruebas

### Configuración de Seguridad

1. **Autenticación**: Solo super administradores pueden acceder
2. **Autorización**: Verificar rol en cada operación
3. **Validación de entrada**: Sanitizar todos los inputs
4. **SQL Injection**: Usar parámetros en todas las consultas
5. **XSS**: Escapar outputs en UI
6. **CSRF**: Usar tokens anti-falsificación en formularios

### Optimizaciones de Rendimiento

1. **Índices de base de datos**: Crear índices en columnas de búsqueda frecuente
2. **Caché**: Cachear configuración del sistema y entidad plantilla
3. **Paginación**: Aunque los grids muestran todos los registros, limitar a 1000 en backend
4. **Lazy loading**: Cargar datos relacionados solo cuando se necesiten
5. **Batch operations**: Usar inserciones por lotes para configuración de entidades

### Monitoreo y Mantenimiento

1. **Logs**: Registrar todas las operaciones y errores
2. **Métricas**: Monitorear tiempos de respuesta y uso de recursos
3. **Alertas**: Notificar a administradores sobre errores críticos
4. **Backups**: Respaldar base de datos regularmente
5. **Auditoría**: Revisar logs de auditoría periódicamente

