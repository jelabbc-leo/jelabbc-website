# Instrucciones para Prueba Final del Chat Widget

**Fecha**: 2026-01-21  
**Estado**: ✅ Código compilado, listo para probar

---

## ✅ Cambios Aplicados y Compilados

1. **`UserInfoHandler.ashx.vb`**: Cambiado fallback de `0` a `1`
2. **`chat-widget.js`**: Agregado log de `IdEntidad` recibido
3. **Compilación**: Exitosa sin errores

---

## 🧪 Pasos para Probar

### 1. Refrescar el Navegador (IMPORTANTE)
Debes hacer un **hard refresh** para limpiar el caché:

**Windows**:
- Chrome/Edge: `Ctrl + Shift + R`
- Firefox: `Ctrl + F5`

**O abrir en modo incógnito**:
- `Ctrl + Shift + N` (Chrome/Edge)
- `Ctrl + Shift + P` (Firefox)

### 2. Iniciar Sesión
- Usuario: `usuario5@jelaweb.com`
- Contraseña: (tu contraseña)

### 3. Abrir Consola del Navegador
- Presiona `F12`
- Ve a la pestaña **"Console"**

### 4. Abrir el Chat Widget
- Haz clic en el botón flotante del chat

### 5. Buscar Estos Logs

Deberías ver:

```javascript
[JELA Master] Chat Widget inicializado con IdEntidad: 1  ✅
[JELA Chat Widget] ✓ Usuario autenticado: Administrador de Condominios
[JELA Chat Widget] ✓ IdEntidad recibido: 1  ✅ ← NUEVO LOG
```

**Si ves `IdEntidad recibido: 0`**, significa que el navegador sigue usando caché. Haz hard refresh de nuevo.

### 6. Enviar un Mensaje de Prueba
- Escribe: "Hola, esto es una prueba"
- Haz clic en enviar

### 7. Verificar el Payload

Busca este log:

```javascript
[JELA Chat Widget] Enviando payload: {
  Nombre: "Administrador de Condominios",
  Email: "usuario5@jelaweb.com",
  Mensaje: "Hola, esto es una prueba",
  IPOrigen: "177.249.175.92",
  IdEntidad: 1,  ✅ ← DEBE SER 1, NO 0
  SessionId: null
}
```

### 8. Verificar Respuesta

**ÉXITO** ✅:
```javascript
[JELA Chat Widget] ✓ Respuesta recibida
// Aparece la respuesta de IA en el chat
```

**ERROR** ❌:
```javascript
[JELA Chat Widget] Error: Error en la respuesta del servidor: 500
```

---

## 🎯 Resultados Esperados

### Antes del Fix:
```
IdEntidad recibido: 0  ❌
Enviando payload: {IdEntidad: 0}  ❌
→ Error 500
```

### Después del Fix:
```
IdEntidad recibido: 1  ✅
Enviando payload: {IdEntidad: 1}  ✅
→ Success, ticket creado
```

---

## 🔍 Si Sigue Fallando

### Problema 1: Sigue mostrando `IdEntidad: 0`

**Causa**: Caché del navegador

**Solución**:
1. Cierra completamente el navegador
2. Abre de nuevo
3. O usa modo incógnito

### Problema 2: Error 500 con `IdEntidad: 1`

**Causa**: Problema en el API

**Solución**:
1. Verifica que el API esté actualizado (ya lo republicaste)
2. Revisa los logs del API en Azure Portal
3. Prueba el endpoint directamente con PowerShell:

```powershell
$body = @{
    nombre = "Test"
    email = "test@example.com"
    mensaje = "Test"
    idEntidad = 1
    ipCliente = "127.0.0.1"
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/webhooks/chatweb" -Method POST -Body $body -ContentType "application/json"
```

### Problema 3: No aparece el log `IdEntidad recibido`

**Causa**: El archivo `chat-widget.js` no se actualizó

**Solución**:
1. Verifica que el archivo esté en: `JelaWeb/Scripts/widgets/chat-widget.js`
2. Busca la línea 149: `console.log('[JELA Chat Widget] ✓ IdEntidad recibido:', data.IdEntidad);`
3. Si no está, el archivo no se compiló correctamente

---

## 📊 Verificación en Base de Datos

Si todo funciona, verifica que el ticket se creó:

```sql
-- Ver el último ticket creado
SELECT * FROM op_tickets_v2 
ORDER BY IdTicket DESC 
LIMIT 1;

-- Debe tener:
-- IdEntidad: 1
-- Canal: ChatWeb
-- NombreCompleto: Administrador de Condominios
-- EmailCliente: usuario5@jelaweb.com
-- Estado: Abierto

-- Ver los mensajes de conversación
SELECT * FROM op_ticket_conversacion 
WHERE IdTicket = (SELECT MAX(IdTicket) FROM op_tickets_v2)
ORDER BY Id;

-- Debe tener 2 registros:
-- 1. Mensaje del cliente
-- 2. Respuesta de IA
```

---

## 🎉 Éxito Confirmado

Si ves:
- ✅ `IdEntidad recibido: 1`
- ✅ `Enviando payload: {IdEntidad: 1}`
- ✅ Respuesta de IA aparece en el chat
- ✅ No hay error 500

**¡El problema está resuelto!**

---

## 📝 Resumen de Fixes Aplicados

| Componente | Problema | Solución | Estado |
|------------|----------|----------|--------|
| Master Page | Usaba fallback `1` | ✅ Correcto | OK |
| UserInfoHandler | Usaba fallback `0` | ✅ Cambiado a `1` | FIXED |
| Chat Widget | No mostraba IdEntidad | ✅ Agregado log | FIXED |
| API | Tabla incorrecta | ✅ Republicado | OK |
| JelaWeb | Código desactualizado | ✅ Recompilado | OK |

---

**Siguiente paso**: Prueba el chat widget siguiendo los pasos arriba y confirma que funciona.
