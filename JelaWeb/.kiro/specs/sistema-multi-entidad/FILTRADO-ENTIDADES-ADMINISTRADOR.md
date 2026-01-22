# Filtrado de Entidades para Administradores

## Fecha
2026-01-20

## Problema Identificado
Cuando un Administrador de Condominios accedía a la página `Entidades.aspx` (desde el selector o desde el menú), se mostraban TODAS las entidades del sistema, no solo las que pertenecen a ese administrador.

Esto representa:
- **Problema de seguridad**: El administrador puede ver entidades que no le pertenecen
- **Problema de usabilidad**: El grid muestra información irrelevante
- **Inconsistencia**: El selector muestra solo sus entidades, pero el catálogo muestra todas

## Solución Implementada

### 1. Modificación en `ApiService.ListarEntidades()`

**Archivo**: `JelaWeb/Services/API/ApiService.vb`

Se modificó el método `ListarEntidades()` para aplicar filtrado automático basado en el tipo de usuario:

```vb
Public Function ListarEntidades() As DataTable
    Try
        Dim query As String
        Dim userId = SessionHelper.GetUserId()
        Dim tipoUsuario = SessionHelper.GetTipoUsuario()
        
        ' Si es Administrador de Condominios, filtrar por entidades asignadas
        If tipoUsuario = Constants.TIPO_USUARIO_ADMIN_CONDOMINIOS AndAlso userId.HasValue Then
            query = "SELECT DISTINCT e.Id,e.Clave,e.RazonSocial,e.CP,e.NombreVialidad,e.NoExterior,e.NoInterior,e.Colonia,e.Localidad,e.EntidadFederativa,e.RFC,e.FechaAlta " &
                    "FROM cat_entidades e " &
                    "INNER JOIN conf_usuario_entidades ue ON e.Id = ue.IdEntidad " &
                    "WHERE ue.IdUsuario = " & QueryBuilder.EscapeSqlInteger(userId.Value) & " AND ue.Activo = 1"
            
            Logger.LogInfo($"ListarEntidades - Filtrando para Administrador UserId={userId.Value}")
        Else
            ' Para otros tipos de usuario, mostrar todas las entidades (comportamiento original)
            query = "SELECT e.Id,e.Clave,e.RazonSocial,e.CP,e.NombreVialidad,e.NoExterior,e.NoInterior,e.Colonia,e.Localidad,e.EntidadFederativa,e.RFC,e.FechaAlta FROM cat_entidades e"
            
            Logger.LogInfo($"ListarEntidades - Sin filtro para TipoUsuario={tipoUsuario}")
        End If
        
        Dim url As String = baseUrl & query
        Dim datos = api.ObtenerDatos(url)

        Return api.ConvertirADatatable(datos)

    Catch ex As Exception
        Logger.LogError("ApiService.ListarEntidades", ex)
        Throw New ApplicationException("Error al obtener los registros desde el API.", ex)
    End Try
End Function
```

### 2. Lógica de Filtrado

**Para Administradores de Condominios** (`TipoUsuario = 'AdministradorCondominios'`):
- Se hace JOIN con la tabla `conf_usuario_entidades`
- Solo se muestran entidades donde:
  - `ue.IdUsuario = [UserId actual]`
  - `ue.Activo = 1`
- Se usa `DISTINCT` para evitar duplicados si hay múltiples registros

**Para otros tipos de usuario** (MesaDirectiva, Residente, Empleado):
- Se mantiene el comportamiento original
- Se muestran todas las entidades sin filtro
- Esto permite que usuarios internos vean todas las entidades del sistema

### 3. Logging Agregado

Se agregaron logs para facilitar el debugging:
- Log cuando se aplica filtro para administrador
- Log cuando NO se aplica filtro (otros usuarios)
- Log de errores con contexto completo

## Beneficios

1. **Seguridad**: Los administradores solo ven sus propias entidades
2. **Consistencia**: El catálogo de entidades ahora es consistente con el selector
3. **Usabilidad**: Grid más limpio y relevante para cada usuario
4. **Mantenibilidad**: Filtrado centralizado en un solo lugar
5. **Auditoría**: Logs permiten rastrear qué usuarios acceden a qué entidades

## Casos de Uso Cubiertos

### Caso 1: Administrador accede desde el selector
1. Usuario hace login como Administrador
2. Ve selector con sus entidades
3. Click en "Agregar Nuevo Condominio"
4. Se abre `Entidades.aspx?modo=nuevo&origen=selector`
5. **Grid muestra solo sus entidades** ✅

### Caso 2: Administrador accede desde el menú
1. Usuario hace login como Administrador
2. Selecciona una entidad del selector
3. Accede al sistema
4. Navega a Catálogos > Entidades
5. **Grid muestra solo sus entidades** ✅

### Caso 3: Usuario interno accede al catálogo
1. Usuario hace login como MesaDirectiva/Empleado
2. Accede al sistema
3. Navega a Catálogos > Entidades
4. **Grid muestra todas las entidades** ✅ (comportamiento original)

## Tablas Involucradas

- `cat_entidades`: Tabla principal de entidades
- `conf_usuario_entidades`: Relación usuario-entidad con campo `Activo`

## Pruebas Recomendadas

1. **Login como Administrador**:
   - Verificar que solo ve sus entidades en el grid
   - Crear nueva entidad y verificar que aparece en el grid
   - Verificar logs en `JelaWeb_[fecha].log`

2. **Login como MesaDirectiva**:
   - Verificar que ve todas las entidades (comportamiento original)

3. **Verificar SQL generado**:
   - Revisar logs para confirmar que el query incluye el JOIN correcto
   - Verificar que `QueryBuilder.EscapeSqlInteger()` sanitiza correctamente el UserId

## Archivos Modificados

- `JelaWeb/Services/API/ApiService.vb`

## Próximos Pasos

- [ ] Probar con usuario administrador real
- [ ] Verificar que el filtrado funciona correctamente
- [ ] Confirmar que usuarios internos siguen viendo todas las entidades
- [ ] Revisar logs para detectar posibles errores
- [ ] Considerar agregar filtrado similar en otros catálogos si es necesario
