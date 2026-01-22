# FIX: Errores de Compilación que Causan Pérdida de Sesión

## Problema Identificado

El log muestra que hay **DOS errores críticos** que están causando que las páginas no compilen correctamente, lo que resulta en pérdida de sesión y redirección a login:

### Error 1: Comentario HTML en Control DevExpress
```
El contenido literal ('<!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->') 
no está permitido en 'DevExpress.Web.LayoutItemCollection'.
```

**Archivo:** `JelaWeb/Views/Catalogos/Unidades.aspx`

**Causa:** Un comentario HTML está dentro de un control DevExpress que no permite contenido literal.

### Error 2: LoadScriptsAndStyles en MasterPage
```
La colección de controles no puede modificarse porque el control contiene bloques de código (por ej. <% ... %>).
```

**Archivo:** `JelaWeb/MasterPages/Jela.Master.vb` - Línea 447

**Causa:** El MasterPage tiene bloques `<% %>` en el markup y el método `LoadScriptsAndStyles()` intenta agregar controles dinámicamente.

## Secuencia de Eventos (según el log)

1. ✅ Login exitoso - Sesión inicializada correctamente
2. ✅ `Server.Transfer` a Inicio.aspx
3. ❌ Error en `LoadScriptsAndStyles()` - No puede agregar controles
4. ❌ `ThreadAbortException` en login (normal después de Transfer)
5. ❌ Usuario hace clic en ribbon → intenta cargar Unidades.aspx
6. ❌ Error de compilación en Unidades.aspx (comentario HTML)
7. ❌ La página no compila → sesión se pierde
8. ⚠️ "Intento de acceso no autorizado" porque la sesión ya no existe

## Soluciones

### Solución 1: Eliminar Comentario de Unidades.aspx

Busca en `JelaWeb/Views/Catalogos/Unidades.aspx` el comentario:
```html
<!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
```

**Opciones:**
1. **Eliminar completamente el comentario**
2. **Moverlo fuera del control DevExpress** (antes o después del `<dx:LayoutItemCollection>`)
3. **Convertirlo en comentario de servidor:** `<%-- Campo Entidad eliminado --%>`

### Solución 2: Comentar LoadScriptsAndStyles()

El método `LoadScriptsAndStyles()` está causando problemas porque el MasterPage tiene bloques `<% %>`.

**Archivo:** `JelaWeb/MasterPages/Jela.Master.vb`

**Opción A - Comentar todo el método:**
```vb
Protected Overrides Sub OnPreRender(e As EventArgs)
    MyBase.OnPreRender(e)

    ' COMENTADO: Causa error cuando hay bloques <% %> en el markup
    ' LoadScriptsAndStyles()

    ' Poblar la barra de estado
    PoblarBarraEstado()

    ' Configurar el nombre de usuario en el menú popup
    If lblUsuarioMenu IsNot Nothing Then
        ' ... resto del código ...
    End If
End Sub
```

**Opción B - Mover los scripts/estilos al markup:**
Agregar los links directamente en el archivo `.master` en lugar de agregarlos dinámicamente.

## Archivos a Revisar

### 1. JelaWeb/Views/Catalogos/Unidades.aspx
Busca y elimina/mueve el comentario HTML problemático.

### 2. JelaWeb/MasterPages/Jela.Master.vb
Comenta la llamada a `LoadScriptsAndStyles()` en `OnPreRender`.

### 3. JelaWeb/MasterPages/Jela.Master (markup)
Verifica si hay bloques `<% %>` que estén causando el problema.

## Verificación

Después de aplicar los fixes:

1. Limpia y recompila el proyecto (Clean + Rebuild)
2. Reinicia IIS Express
3. Haz login
4. Intenta navegar a Unidades.aspx desde el ribbon
5. Verifica que no haya errores en el log

## Log de Referencia

```
2026-01-21 12:01:21 [INFO] Usuario admin es Residente, redirigiendo a inicio
2026-01-21 12:01:23 [ERROR] Error al cargar scripts y estilos en Master Page
2026-01-21 12:01:26 [ERROR] Error durante el proceso de login (ThreadAbortException - normal)
2026-01-21 12:01:30 [ERROR] Error no controlado - Comentario HTML en Unidades.aspx
2026-01-21 12:05:53 [WARNING] Intento de acceso no autorizado a: /Views/Catalogos/Unidades.aspx
```

La sesión se pierde porque la página no compila correctamente.


---

## ✅ FIXES APLICADOS

### Fix 1: Comentarios HTML Eliminados

Se eliminaron los comentarios HTML problemáticos de los siguientes archivos:

1. ✅ `JelaWeb/Views/Catalogos/AreasComunes.aspx`
2. ✅ `JelaWeb/Views/Catalogos/Residentes.aspx`
3. ✅ `JelaWeb/Views/Catalogos/Unidades.aspx`
4. ✅ `JelaWeb/Views/Operacion/Condominios/Comunicados.aspx`
5. ✅ `JelaWeb/Views/Operacion/Condominios/EstadoCuenta.aspx`
6. ✅ `JelaWeb/Views/Operacion/Condominios/Reservaciones.aspx`
7. ✅ `JelaWeb/Views/Operacion/Condominios/Cuotas.aspx` (2 ocurrencias)
8. ✅ `JelaWeb/Views/Operacion/Condominios/Pagos.aspx`

**Total:** 9 comentarios eliminados de 8 archivos

### Fix 2: LoadScriptsAndStyles() Comentado

Se comentó la llamada a `LoadScriptsAndStyles()` en `JelaWeb/MasterPages/Jela.Master.vb` línea ~370 (método `OnPreRender`).

**Razón:** El método intentaba agregar controles dinámicamente al `Page.Header`, pero el MasterPage contiene bloques `<% %>` en el markup, lo cual no permite modificar la colección de controles.

**Solución:** Los scripts y estilos deben agregarse directamente en el markup del archivo `.master` en lugar de hacerlo dinámicamente.

## 🎯 Próximos Pasos

1. **Clean + Rebuild** del proyecto en Visual Studio
2. **Reiniciar IIS Express**
3. **Probar el flujo completo:**
   - Login
   - Navegación a Inicio
   - Click en botones del ribbon
   - Verificar que las páginas carguen correctamente
4. **Revisar el log** para confirmar que no hay más errores

## 📊 Resultado Esperado

Después de estos fixes:
- ✅ Las páginas deben compilar correctamente
- ✅ La sesión debe mantenerse después del login
- ✅ La navegación del ribbon debe funcionar
- ✅ No debe haber errores de "acceso no autorizado"
- ✅ No debe haber errores de compilación en el log

## 🔍 Verificación en el Log

Busca estas líneas en el log después del fix:

```
[INFO] Login exitoso para usuario: admin
[INFO] Usuario admin es Residente, redirigiendo a inicio
(NO debe haber errores de LoadScriptsAndStyles)
(NO debe haber errores de compilación de Unidades.aspx)
(NO debe haber "Intento de acceso no autorizado")
```

## ⚠️ Nota Importante

Si después de estos fixes aún hay problemas de navegación, el siguiente paso es verificar:
1. Que el `Server.Transfer` en `Ingreso.aspx.vb` esté funcionando correctamente
2. Que la sesión se esté guardando correctamente en `InitializeSession`
3. Que `Global.asax.Application_AcquireRequestState` no esté redirigiendo incorrectamente
