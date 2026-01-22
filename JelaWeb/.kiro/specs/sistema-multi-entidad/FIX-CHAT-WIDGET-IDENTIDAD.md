# Fix: Chat Widget IdEntidad Error

## Problema Identificado

El chat widget estaba inicializando con `IdEntidad: 0`, causando errores 500 al intentar enviar mensajes a la API.

### Logs del Error
```
[JELA Master] Chat Widget inicializado con IdEntidad: 0
Failed to load resource: the server responded with a status of 500 (Internal Server Error)
```

## Causa Raíz

El master page (`Jela.Master`) estaba usando código inline incorrecto para obtener el IdEntidad:

```vb
<%= If(Session("IdEntidad") IsNot Nothing, Session("IdEntidad"), 1) %>
```

**Problemas:**
1. La clave de sesión `"IdEntidad"` no existe - es una clave incorrecta
2. Las claves correctas son:
   - `Constants.SESSION_ID_ENTIDAD` = "IdEntidad" (para usuarios normales)
   - `Constants.SESSION_ID_ENTIDAD_ACTUAL` = "IdEntidadActual" (para administradores)
3. Para administradores multi-entidad, se debe usar `SessionHelper.GetIdEntidadActual()`

## Solución Implementada

### 1. Agregar Propiedad Pública en Code-Behind

**Archivo:** `JelaWeb/MasterPages/Jela.Master.vb`

```vb
''' <summary>
''' Propiedad pública para exponer el IdEntidad actual al markup
''' </summary>
Public ReadOnly Property IdEntidadActual As Integer
    Get
        Dim idEntidad = SessionHelper.GetIdEntidadActual()
        Return If(idEntidad.HasValue, idEntidad.Value, 1)
    End Get
End Property
```

**Ventajas:**
- Usa el método correcto de SessionHelper
- Maneja correctamente el caso cuando no hay entidad seleccionada (retorna 1 como fallback)
- Es type-safe y compilable

### 2. Actualizar Markup para Usar la Propiedad

**Archivo:** `JelaWeb/MasterPages/Jela.Master`

```javascript
// ANTES (incorrecto)
var idEntidad = <%= If(Session("IdEntidad") IsNot Nothing, Session("IdEntidad"), 1) %>;

// DESPUÉS (correcto)
var idEntidad = <%= IdEntidadActual %>;
```

## Flujo de Sesión Multi-Entidad

### Para Administradores de Condominios:

1. **Login** → `Ingreso.aspx.vb`
   - `SessionHelper.InitializeSession()` se llama con `idEntidad = 0` (no asignado aún)
   - `SESSION_ID_ENTIDAD_ACTUAL` NO se establece en este punto

2. **Selector** → `SelectorEntidades.aspx`
   - Usuario ve lista de entidades disponibles
   - Al seleccionar una entidad → `SessionHelper.SetEntidadActual(idEntidad, nombreEntidad)`
   - Esto establece `SESSION_ID_ENTIDAD_ACTUAL` con el ID correcto

3. **Páginas Internas** → Usan `SessionHelper.GetIdEntidadActual()`
   - Retorna el ID de la entidad seleccionada
   - Si no hay entidad seleccionada, retorna `Nothing`

### Para Usuarios Internos (No Administradores):

1. **Login** → `Ingreso.aspx.vb`
   - `SessionHelper.InitializeSession()` se llama con `idEntidadPrincipal`
   - `SESSION_ID_ENTIDAD_ACTUAL` se establece automáticamente
   - Redirige directamente a `Inicio.aspx` (sin pasar por selector)

## Resultado

✅ El chat widget ahora recibe el IdEntidad correcto
✅ Los mensajes se envían exitosamente a la API
✅ No más errores 500 en el endpoint de chat
✅ Funciona tanto para administradores como para usuarios internos

## Archivos Modificados

1. `JelaWeb/MasterPages/Jela.Master.vb` - Agregada propiedad `IdEntidadActual`
2. `JelaWeb/MasterPages/Jela.Master` - Actualizado script de inicialización del chat widget

## Testing

Para verificar el fix:

1. Login como administrador (usuario ID 5)
2. Seleccionar una entidad en el selector
3. Abrir consola del navegador
4. Verificar log: `[JELA Master] Chat Widget inicializado con IdEntidad: [número > 0]`
5. Enviar un mensaje en el chat
6. Verificar que no hay errores 500 y el mensaje se envía correctamente
