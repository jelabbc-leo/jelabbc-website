# 🔒 PLAN DE SEGURIDAD PARA CRUD DINÁMICO - SISTEMA COMPLETO

**Fecha**: 19 de enero de 2026  
**Versión**: 3.0 (Actualizada para todo el sistema JELA)  
**Prioridad**: 🔴 CRÍTICA  
**Alcance**: **TODO EL SISTEMA** (no solo tickets)

---

## 📌 CONTEXTO IMPORTANTE

### Alcance del API
El API CRUD dinámico se usa en:
- ✅ **Módulo de Tickets** (op_tickets_v2, op_interacciones, etc.)
- ✅ **Catálogos** (cat_colonias, cat_conceptos, cat_proveedores, etc.)
- ✅ **Configuración** (conf_ticket_prompts, conf_usuarios, etc.)
- ✅ **Operaciones** (op_reservaciones, op_comunicados, op_pagos, etc.)
- ✅ **Logs y auditoría** (log_crud_operations, log_api_requests, etc.)
- ✅ **Apps futuras** (módulos aún no desarrollados)

### Implicación
La solución de seguridad debe ser:
- **Genérica**: Funcionar para cualquier tipo de tabla
- **Escalable**: Soportar nuevos módulos sin modificar código
- **Flexible**: Permitir reglas específicas solo donde se necesiten
- **Mantenible**: Bajo esfuerzo de configuración

---

## 🎯 OBJETIVOS

### 1. Mantener el Dinamismo
```
Agregar campo en BD → API lo detecta automáticamente → Funciona
Agregar tabla nueva → API la acepta (si cumple prefijos) → Funciona
```

### 2. Seguridad Robusta
- ✅ Protección contra SQL Injection
- ✅ Protección contra Mass Assignment
- ✅ Validación automática de tipos y longitudes
- ✅ Detección de campos read-only
- ✅ Rate Limiting
- ✅ Auditoría completa

### 3. Bajo Mantenimiento
- ✅ Cambios en BD no requieren modificar API
- ✅ Reglas de negocio solo en config (opcional)
- ✅ Sin código hardcodeado

---

## 🏗️ ARQUITECTURA: 3 CAPAS DE SEGURIDAD

```
┌─────────────────────────────────────────────────────┐
│  CAPA 1: VALIDACIÓN BÁSICA (SIEMPRE ACTIVA)        │
│  ✓ SQL Injection Prevention                         │
│  ✓ Sanitización de nombres (tabla/campos)          │
│  ✓ Tablas bloqueadas (hardcoded)                   │
│  ✓ Rate Limiting global                            │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│  CAPA 2: SCHEMA DISCOVERY (DINÁMICO)               │
│  ✓ Lee INFORMATION_SCHEMA automáticamente          │
│  ✓ Detecta tipos de columnas (int, varchar, etc.)  │
│  ✓ Valida longitudes máximas                       │
│  ✓ Identifica claves primarias                     │
│  ✓ Detecta campos auto-increment                   │
│  ✓ Caché de schemas (1 hora)                       │
└─────────────────────────────────────────────────────┘
                        ↓
┌─────────────────────────────────────────────────────┐
│  CAPA 3: REGLAS OPCIONALES (CONFIGURABLES)         │
│  ✓ Solo para tablas críticas específicas           │
│  ✓ Operaciones permitidas por tabla                │
│  ✓ Campos prohibidos (blacklist)                   │
│  ✓ Validación de valores enum                      │
│  ✓ Reglas de negocio personalizadas                │
└─────────────────────────────────────────────────────┘
```

---

## 🛠️ COMPONENTES A IMPLEMENTAR

### 1. ISchemaDiscoveryService
**Responsabilidad**: Descubrir y cachear estructura de tablas desde BD

```csharp
public interface ISchemaDiscoveryService
{
    Task<TableSchemaInfo> GetTableSchemaAsync(string tableName);
    Task<bool> FieldExistsAsync(string tableName, string fieldName);
    Task<string?> GetFieldTypeAsync(string tableName, string fieldName);
    void ClearCache(string? tableName = null);
}

public class TableSchemaInfo
{
    public string TableName { get; set; }
    public Dictionary<string, ColumnInfo> Columns { get; set; }
}

public class ColumnInfo
{
    public string Name { get; set; }
    public string DataType { get; set; }
    public int? MaxLength { get; set; }
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public bool IsAutoIncrement { get; set; }
    public bool IsReadOnly { get; set; }
}
```

**Implementación**:
- Consulta `INFORMATION_SCHEMA.COLUMNS` de MySQL
- Caché en memoria (IMemoryCache) por 1 hora
- Detecta automáticamente: tipos, longitudes, PKs, auto-increment

---

### 2. ICrudSecurityService
**Responsabilidad**: Validar operaciones CRUD en 3 capas

```csharp
public interface ICrudSecurityService
{
    Task<SecurityValidationResult> ValidateOperationAsync(
        string tabla,
        string operacion,
        Dictionary<string, object> campos,
        int idUsuario);
}

public class SecurityValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; }
    public List<string> Warnings { get; set; }
}
```

**Implementación**: `HybridCrudSecurityService`

---

### 3. MySqlDatabaseService (Actualización)
**Agregar**: Método `SanitizeName()` para prevenir SQL Injection

```csharp
private string SanitizeName(string name)
{
    if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("El nombre no puede estar vacío");

    // Solo permitir: letras, números, guiones bajos
    if (!name.All(c => char.IsLetterOrDigit(c) || c == '_'))
        throw new ArgumentException($"Nombre inválido: {name}");

    return name;
}
```

Aplicar en: `InsertarAsync()`, `ActualizarAsync()`, `EliminarAsync()`

---

### 4. CrudEndpoints (Integración)
**Actualizar**: Inyectar `ICrudSecurityService` y validar antes de ejecutar

```csharp
private static async Task<IResult> InsertarRegistro(
    string tabla,
    [FromBody] CrudRequest request,
    IDatabaseService database,
    ICrudSecurityService securityService,  // ← NUEVO
    HttpContext httpContext,                // ← NUEVO
    ...)
{
    // Validar seguridad
    var validationResult = await securityService.ValidateOperationAsync(
        tabla, "INSERT", campos, idUsuario);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(new ErrorResponse 
        { 
            Mensaje = "Validación de seguridad falló",
            Detalles = validationResult.Errors
        });
    }

    // Continuar con INSERT...
}
```

---

## ⚙️ CONFIGURACIÓN (appsettings.json)

### Estructura Completa

```json
{
  "CrudSecurity": {
    "EnableSchemaDiscovery": true,
    "SchemaCacheMinutes": 60,
    
    "BlockedTables": [
      "conf_usuarios",
      "conf_refresh_tokens",
      "conf_api_keys",
      "conf_roles_permisos"
    ],
    
    "TableRules": {
      "op_tickets_v2": {
        "AllowedOperations": ["INSERT", "UPDATE", "SELECT"],
        "ForbiddenFields": ["IdUsuarioCreacion", "FechaCreacion"],
        "EnumFields": {
          "Estado": ["Abierto", "EnProceso", "Cerrado", "Resuelto"],
          "Prioridad": ["Baja", "Media", "Alta", "Urgente"]
        }
      },
      "cat_conceptos": {
        "AllowedOperations": ["INSERT", "UPDATE", "SELECT", "DELETE"],
        "ForbiddenFields": ["FechaCreacion"],
        "EnumFields": {
          "Tipo": ["Ingreso", "Egreso", "Traspaso"]
        }
      },
      "conf_ticket_prompts": {
        "AllowedOperations": ["SELECT", "UPDATE"],
        "ForbiddenFields": ["Id", "FechaCreacion"]
      },
      "op_reservaciones": {
        "AllowedOperations": ["INSERT", "UPDATE", "DELETE"],
        "ForbiddenFields": ["IdUsuarioCreacion"],
        "EnumFields": {
          "Estado": ["Pendiente", "Confirmada", "Cancelada", "Completada"]
        }
      }
    },
    
    "RateLimiting": {
      "MaxRequestsPerMinute": 60,
      "MaxRequestsPerHour": 1000
    }
  }
}
```

### Notas de Configuración

#### BlockedTables (Hardcoded)
Tablas que **NUNCA** deben ser accesibles vía CRUD dinámico:
- `conf_usuarios` - Gestión de usuarios (usar endpoints específicos)
- `conf_refresh_tokens` - Tokens de autenticación
- `conf_api_keys` - Claves de API
- `conf_roles_permisos` - Permisos del sistema

#### TableRules (Opcional)
Solo definir reglas para tablas que requieren validaciones especiales:
- **Operaciones permitidas**: Restringir INSERT/UPDATE/DELETE
- **Campos prohibidos**: Campos que no deben modificarse vía API
- **Valores enum**: Validar valores específicos

**Importante**: Si una tabla NO tiene reglas definidas, solo aplican Capa 1 y Capa 2 (más flexible).

---

## 📊 EJEMPLOS DE VALIDACIÓN POR TIPO DE TABLA

### Catálogos (cat_*)
```json
"cat_colonias": {
  "AllowedOperations": ["INSERT", "UPDATE", "SELECT", "DELETE"],
  "ForbiddenFields": ["Id", "FechaCreacion"],
  "EnumFields": {
    "Activo": ["0", "1"]
  }
}
```

**Validaciones automáticas (Capa 2)**:
- ✅ Longitud de `Nombre` (desde schema)
- ✅ Tipo de `CodigoPostal` (int)
- ✅ Campo `Id` es auto-increment (no especificar en INSERT)

---

### Operaciones (op_*)
```json
"op_comunicados": {
  "AllowedOperations": ["INSERT", "UPDATE", "SELECT"],
  "ForbiddenFields": ["IdUsuarioCreacion", "FechaCreacion"],
  "EnumFields": {
    "Tipo": ["Informativo", "Urgente", "Mantenimiento"],
    "Estado": ["Borrador", "Publicado", "Archivado"]
  }
}
```

**Validaciones automáticas (Capa 2)**:
- ✅ Longitud de `Titulo` y `Contenido`
- ✅ Tipo de `FechaPublicacion` (datetime)
- ✅ Campo `IdUsuarioCreacion` es read-only

---

### Configuración (conf_*)
```json
"conf_parametros_sistema": {
  "AllowedOperations": ["SELECT", "UPDATE"],
  "ForbiddenFields": ["Id", "Clave"],
  "EnumFields": {}
}
```

**Validaciones automáticas (Capa 2)**:
- ✅ Solo UPDATE permitido (no INSERT/DELETE)
- ✅ Campo `Clave` no puede modificarse
- ✅ Longitud de `Valor` (desde schema)

---

### Logs (log_*)
```json
"log_api_requests": {
  "AllowedOperations": ["INSERT", "SELECT"],
  "ForbiddenFields": [],
  "EnumFields": {}
}
```

**Validaciones automáticas (Capa 2)**:
- ✅ Solo INSERT y SELECT (no UPDATE/DELETE)
- ✅ Todos los campos validados por schema

---

### Tablas sin reglas específicas
Si una tabla NO está en `TableRules`, solo aplican:
- ✅ Capa 1: SQL Injection prevention
- ✅ Capa 2: Validación automática desde schema
- ❌ Capa 3: No aplica

**Ejemplo**: `cat_proveedores` (sin reglas)
- Permite todas las operaciones (INSERT/UPDATE/DELETE)
- Valida tipos y longitudes automáticamente
- No restringe campos específicos

---

## 🚀 FLUJO DE VALIDACIÓN COMPLETO

```
┌─────────────────────────────────────────┐
│  1. Request llega al API                │
│     POST /api/crud/cat_conceptos        │
│     { "Nombre": "...", "Tipo": "..." }  │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│  2. CAPA 1: Validación Básica           │
│     ✓ Nombre de tabla válido?           │
│       → Solo letras, números, _         │
│     ✓ Tabla no bloqueada?               │
│       → No está en BlockedTables        │
│     ✓ Nombres de campos válidos?        │
│       → Solo letras, números, _         │
│     ✓ Rate limiting OK?                 │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│  3. CAPA 2: Schema Discovery            │
│     ✓ Consulta INFORMATION_SCHEMA       │
│       → Obtiene estructura de tabla     │
│     ✓ Caché de schema (1 hora)          │
│     ✓ Valida campos existen             │
│       → "Nombre" existe en tabla        │
│       → "Tipo" existe en tabla          │
│     ✓ Valida tipos de datos             │
│       → "Nombre" es varchar(100)        │
│       → "Tipo" es varchar(50)           │
│     ✓ Valida longitudes                 │
│       → "Nombre" no excede 100 chars    │
│     ✓ Detecta campos read-only          │
│       → "Id" es auto-increment          │
│       → "FechaCreacion" es PK           │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│  4. CAPA 3: Reglas Opcionales           │
│     ✓ Busca reglas para "cat_conceptos" │
│     ✓ Valida operación permitida        │
│       → INSERT está en AllowedOperations│
│     ✓ Valida campos prohibidos          │
│       → "FechaCreacion" no está en body │
│     ✓ Valida valores enum               │
│       → "Tipo" = "Ingreso" es válido    │
└─────────────────────────────────────────┘
                  ↓
┌─────────────────────────────────────────┐
│  5. Ejecutar operación CRUD             │
│     ✓ INSERT en cat_conceptos           │
│     ✓ Auditar operación                 │
│     ✓ Retornar ID generado              │
└─────────────────────────────────────────┘
```

---

## ⚡ RENDIMIENTO

### Overhead por Request
- **Capa 1** (Validación básica): ~1-2ms
- **Capa 2** (Schema discovery con caché): ~2-5ms
- **Capa 3** (Reglas opcionales): ~1-2ms
- **Total**: ~4-9ms (aceptable)

### Optimizaciones
- ✅ Caché de schemas (1 hora) - reduce consultas a INFORMATION_SCHEMA
- ✅ Lazy loading de reglas - solo carga si existen
- ✅ Validación paralela de campos - usa LINQ
- ✅ Rate limiting global - previene abuso

### Escalabilidad
- ✅ Soporta cientos de tablas sin degradación
- ✅ Caché por tabla (no global)
- ✅ Sin impacto en tablas sin reglas específicas

---

## 📋 PLAN DE IMPLEMENTACIÓN

### Fase 1: Schema Discovery (4-5 horas)
1. ✅ Crear `ISchemaDiscoveryService` y `SchemaDiscoveryService`
2. ✅ Implementar consulta a `INFORMATION_SCHEMA.COLUMNS`
3. ✅ Agregar caché con `IMemoryCache`
4. ✅ Probar con tablas de diferentes tipos (cat_, op_, conf_)
5. ✅ Validar detección de: tipos, longitudes, PKs, auto-increment

**Archivos**:
- `JELA.API/Services/ISchemaDiscoveryService.cs`
- `JELA.API/Services/SchemaDiscoveryService.cs`

---

### Fase 2: Servicio de Seguridad Híbrido (3-4 horas)
1. ✅ Crear `ICrudSecurityService` y `HybridCrudSecurityService`
2. ✅ Implementar Capa 1: Validación básica
   - Sanitización de nombres
   - Verificación de tablas bloqueadas
3. ✅ Implementar Capa 2: Schema Discovery
   - Validar campos existen
   - Validar tipos de datos
   - Validar longitudes
   - Detectar campos read-only
4. ✅ Implementar Capa 3: Reglas opcionales
   - Operaciones permitidas
   - Campos prohibidos
   - Valores enum
5. ✅ Probar con diferentes tipos de tablas

**Archivos**:
- `JELA.API/Services/ICrudSecurityService.cs`
- `JELA.API/Services/HybridCrudSecurityService.cs`

---

### Fase 3: Integración en CRUD Endpoints (2-3 horas)
1. ✅ Actualizar `MySqlDatabaseService`
   - Agregar método `SanitizeName()`
   - Aplicar en `InsertarAsync()`, `ActualizarAsync()`, `EliminarAsync()`
2. ✅ Actualizar `CrudEndpoints`
   - Inyectar `ICrudSecurityService`
   - Validar antes de INSERT/UPDATE
   - Retornar errores detallados
3. ✅ Actualizar `ErrorResponse` model
   - Agregar campo `Detalles` (List<string>)
4. ✅ Probar endpoints con Swagger

**Archivos**:
- `JELA.API/Services/MySqlDatabaseService.cs`
- `JELA.API/Endpoints/CrudEndpoints.cs`
- `JELA.API/Models/CrudModels.cs`

---

### Fase 4: Configuración y Registro (1-2 horas)
1. ✅ Actualizar `appsettings.json`
   - Agregar sección `CrudSecurity`
   - Definir `BlockedTables`
   - Definir `TableRules` para tablas críticas
2. ✅ Actualizar `Program.cs`
   - Registrar `ISchemaDiscoveryService`
   - Registrar `ICrudSecurityService`
   - Agregar `IMemoryCache` (si no existe)
3. ✅ Documentar configuración

**Archivos**:
- `JELA.API/appsettings.json`
- `JELA.API/Program.cs`

---

### Fase 5: Pruebas y Validación (2-3 horas)
1. ✅ Probar con tablas de catálogos (cat_*)
2. ✅ Probar con tablas de operaciones (op_*)
3. ✅ Probar con tablas de configuración (conf_*)
4. ✅ Probar con tablas sin reglas específicas
5. ✅ Validar intentos de SQL Injection
6. ✅ Validar Mass Assignment
7. ✅ Validar campos read-only
8. ✅ Validar valores enum
9. ✅ Validar rate limiting
10. ✅ Documentar casos de prueba

**Casos de prueba**:
- ✅ INSERT con campo auto-increment (debe fallar)
- ✅ UPDATE de campo read-only (debe fallar)
- ✅ INSERT con longitud excedida (debe fallar)
- ✅ INSERT con valor enum inválido (debe fallar)
- ✅ INSERT en tabla bloqueada (debe fallar)
- ✅ INSERT con SQL Injection (debe fallar)
- ✅ INSERT válido en tabla sin reglas (debe funcionar)
- ✅ UPDATE válido en tabla con reglas (debe funcionar)

---

### Fase 6: Auditoría (Opcional - 2-3 horas)
1. ✅ Crear tabla `log_crud_operations`
2. ✅ Implementar `ICrudAuditService`
3. ✅ Registrar todas las operaciones CRUD
4. ✅ Incluir: usuario, tabla, operación, campos, timestamp
5. ✅ Integrar en endpoints

**Archivos**:
- `JELA.API/Services/ICrudAuditService.cs`
- `JELA.API/Services/CrudAuditService.cs`
- SQL: `CREATE TABLE log_crud_operations`

---

## ⏱️ ESTIMACIÓN TOTAL

| Fase | Tiempo | Prioridad |
|------|--------|-----------|
| Fase 1: Schema Discovery | 4-5 horas | 🔴 Alta |
| Fase 2: Servicio de Seguridad | 3-4 horas | 🔴 Alta |
| Fase 3: Integración Endpoints | 2-3 horas | 🔴 Alta |
| Fase 4: Configuración | 1-2 horas | 🔴 Alta |
| Fase 5: Pruebas | 2-3 horas | 🔴 Alta |
| Fase 6: Auditoría | 2-3 horas | 🟡 Media |
| **TOTAL** | **14-20 horas** | |

---

## ✅ VENTAJAS DEL ENFOQUE HÍBRIDO

### 1. Mantiene el Dinamismo
```
✅ Agregar campo en BD → API lo detecta automáticamente
✅ Agregar tabla nueva → API la acepta (si cumple prefijos)
✅ Modificar tipo de campo → API lo valida automáticamente
```

### 2. Seguridad Robusta
- ✅ **Capa 1**: Protección contra SQL Injection (siempre activa)
- ✅ **Capa 2**: Validación automática desde schema de BD
- ✅ **Capa 3**: Reglas de negocio opcionales

### 3. Genérico y Escalable
- ✅ Funciona para **cualquier tipo de tabla** (cat_, op_, conf_, log_)
- ✅ Soporta **nuevos módulos** sin modificar código
- ✅ Reglas específicas solo donde se necesitan

### 4. Bajo Mantenimiento
- ✅ Agregar campo: Solo en BD
- ✅ Agregar tabla: Solo en BD (y opcionalmente en config)
- ✅ Reglas de negocio: Solo en config (opcional)

### 5. Flexible
- ✅ Tablas sin reglas: Máxima flexibilidad (solo Capa 1 + 2)
- ✅ Tablas críticas: Reglas estrictas (Capa 1 + 2 + 3)
- ✅ Configuración por tabla (no global)

---

## 🎯 CASOS DE USO POR MÓDULO

### Módulo de Tickets
- `op_tickets_v2`: Reglas estrictas (enum de estados, campos read-only)
- `op_interacciones`: Reglas medias (campos read-only)
- `op_telegram_logs`: Sin reglas (solo validación automática)

### Módulo de Catálogos
- `cat_conceptos`: Reglas medias (enum de tipos)
- `cat_colonias`: Sin reglas (solo validación automática)
- `cat_proveedores`: Sin reglas (solo validación automática)

### Módulo de Operaciones
- `op_reservaciones`: Reglas medias (enum de estados)
- `op_comunicados`: Reglas medias (enum de tipos)
- `op_pagos`: Reglas estrictas (campos read-only, validaciones)

### Módulo de Configuración
- `conf_ticket_prompts`: Reglas estrictas (solo UPDATE)
- `conf_parametros_sistema`: Reglas estrictas (solo UPDATE)
- `conf_usuarios`: **BLOQUEADA** (usar endpoints específicos)

### Módulo de Logs
- `log_crud_operations`: Reglas estrictas (solo INSERT/SELECT)
- `log_api_requests`: Reglas estrictas (solo INSERT/SELECT)

---

## 📝 NOTAS FINALES

### Importante
- Esta propuesta es para **TODO EL SISTEMA JELA**, no solo tickets
- El enfoque híbrido permite **máxima flexibilidad** para tablas simples
- Las reglas específicas solo se aplican donde realmente se necesitan
- El sistema es **100% dinámico** - agregar campos/tablas no requiere modificar código

### Próximos Pasos
1. Revisar y aprobar esta propuesta
2. Implementar en el orden de las fases
3. Probar exhaustivamente con diferentes tipos de tablas
4. Documentar casos de uso específicos por módulo
5. Capacitar al equipo en configuración de reglas

---

**Autor**: Kiro AI  
**Fecha**: 19 de enero de 2026  
**Versión**: 3.0 (Sistema Completo)  
**Estado**: ✅ Propuesta lista para implementación
