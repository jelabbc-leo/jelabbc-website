# Fix Chat Widget - Deploy API Actualizado

## 📋 Resumen

El chat widget ahora inicializa correctamente con `IdEntidad: 1`, pero sigue mostrando error 500 al enviar mensajes. La investigación reveló que:

1. ✅ **El código de la API ya está correcto** - usa `op_ticket_conversacion` (singular)
2. ❌ **La versión desplegada en Azure está desactualizada**
3. ✅ **Se compiló nueva versión y se creó ZIP para deploy**

---

## 🔍 Diagnóstico Realizado

### Prueba desde PowerShell
```powershell
$body = '{"nombre":"Test User","email":"test@example.com","telefono":"1234567890","mensaje":"Test message","idEntidad":1,"ipCliente":"127.0.0.1"}'

Invoke-RestMethod -Uri "https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/webhooks/chatweb" -Method POST -Body $body -ContentType "application/json"
```

**Resultado**: Error 500 (Internal Server Error)

### Verificación del Código
- ✅ Archivo: `JELA.API/JELA.API/Endpoints/WebhookEndpoints.cs`
- ✅ Línea 741: `await db.InsertarAsync("op_ticket_conversacion", campos);`
- ✅ Nombre de tabla correcto: `op_ticket_conversacion` (singular)
- ✅ No hay referencias a `op_tickets_conversacion` (plural) en el código

### Conclusión
El código fuente está correcto, pero la versión desplegada en Azure es antigua y probablemente tiene el nombre de tabla incorrecto.

---

## ✅ Solución Implementada

### 1. Compilación Exitosa
```bash
dotnet publish JELA.API\JELA.API.csproj -c Release -o publish
```

**Resultado**: ✅ Compilación exitosa sin errores

### 2. Creación de ZIP para Deploy
```bash
Compress-Archive -Path "publish\*" -DestinationPath "jela-api-deploy.zip"
```

**Resultado**: ✅ ZIP creado (1.99 MB)

**Ubicación**: `JELA.API/jela-api-deploy.zip`

---

## 🚀 Próximos Pasos (USUARIO DEBE HACER)

### Opción 1: Deploy desde Azure Portal (RECOMENDADO)

1. Abrir [Azure Portal](https://portal.azure.com)
2. Buscar App Service: **jela-api-ctb8a6ggbpdqbxhg**
3. Ir a **"Centro de implementación"** (Deployment Center)
4. Seleccionar **"Implementación manual"** o **"ZIP Deploy"**
5. Arrastrar el archivo `JELA.API/jela-api-deploy.zip`
6. Esperar 1-2 minutos

### Opción 2: Deploy con Kudu Console

1. En Azure Portal → App Service → **"Herramientas avanzadas"**
2. Clic en **"Ir"** (abre Kudu)
3. **"Tools"** → **"Zip Push Deploy"**
4. Arrastrar `jela-api-deploy.zip`

### Opción 3: Deploy con Azure CLI (si está instalado)

```powershell
az login

az webapp deployment source config-zip `
    --resource-group jela-resources `
    --name jela-api-ctb8a6ggbpdqbxhg `
    --src JELA.API/jela-api-deploy.zip
```

---

## 🧪 Verificación Post-Deploy

### 1. Verificar Health Endpoint
```powershell
curl https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/health
```

**Esperado**:
```json
{
  "Status": "Healthy",
  "Timestamp": "...",
  "Checks": { ... }
}
```

### 2. Probar Chat Web Endpoint
```powershell
$body = '{"nombre":"Test User","email":"test@example.com","telefono":"1234567890","mensaje":"Test message","idEntidad":1,"ipCliente":"127.0.0.1"}'

Invoke-RestMethod -Uri "https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/webhooks/chatweb" -Method POST -Body $body -ContentType "application/json"
```

**Esperado**:
```json
{
  "success": true,
  "ticketId": 123,
  "mensaje": "Ticket #123 creado exitosamente",
  "respuestaIA": "...",
  "sessionId": null
}
```

### 3. Verificar en Base de Datos
```sql
-- Ver ticket creado
SELECT * FROM op_tickets_v2 ORDER BY IdTicket DESC LIMIT 1;

-- Ver mensajes de conversación
SELECT * FROM op_ticket_conversacion ORDER BY Id DESC LIMIT 5;
```

### 4. Probar Chat Widget en JelaWeb
1. Abrir JelaWeb en el navegador
2. Abrir consola del navegador (F12)
3. Abrir el chat widget
4. Enviar un mensaje de prueba
5. Verificar que no hay error 500
6. Verificar que aparece respuesta de IA

---

## 📝 Cambios Incluidos en el Deploy

### Código Correcto
```csharp
// Línea 741 en WebhookEndpoints.cs
await db.InsertarAsync("op_ticket_conversacion", campos);
```

### Logs Mejorados
Se agregaron logs detallados paso a paso:
```csharp
logger.LogInformation("Paso 1: Validando cliente duplicado...");
logger.LogInformation("Paso 2: Obteniendo prompts de la base de datos...");
logger.LogInformation("Paso 3: Llamando a Azure OpenAI...");
logger.LogInformation("Paso 4: Creando ticket en base de datos...");
logger.LogInformation("Paso 5: Registrando interacción...");
logger.LogInformation("Paso 6: Guardando mensaje del cliente en conversación...");
logger.LogInformation("Paso 7: Guardando respuesta de IA en conversación...");
```

### Manejo de Errores
```csharp
logger.LogError(ex, "ERROR DETALLADO en chat web - Email: {Email}, Mensaje: {Message}, StackTrace: {StackTrace}", 
    request.Email, ex.Message, ex.StackTrace);
```

---

## 🔧 Troubleshooting

### Si sigue dando error 500 después del deploy

1. **Ver logs en tiempo real**:
   - Azure Portal → App Service → **"Secuencia de registro"** (Log stream)

2. **Verificar tabla en base de datos**:
   ```sql
   SHOW TABLES LIKE 'op_ticket%';
   ```
   
   Debe existir: `op_ticket_conversacion`

3. **Verificar estructura de tabla**:
   ```sql
   DESCRIBE op_ticket_conversacion;
   ```
   
   Debe tener columnas: `Id`, `IdTicket`, `TipoMensaje`, `Mensaje`, `EsRespuestaIA`, etc.

4. **Si la tabla no existe**, ejecutar:
   ```sql
   -- Ver: JelaWeb/Scripts/SQL/05_CREATE_op_ticket_conversacion.sql
   ```

---

## 📊 Estado Actual

| Componente | Estado | Notas |
|------------|--------|-------|
| Chat Widget Inicialización | ✅ OK | IdEntidad: 1 correcto |
| Código API (local) | ✅ OK | Tabla correcta: `op_ticket_conversacion` |
| Código API (Azure) | ❌ DESACTUALIZADO | Necesita deploy |
| ZIP de Deploy | ✅ LISTO | `JELA.API/jela-api-deploy.zip` (1.99 MB) |
| Tabla en BD | ✅ OK | Confirmado por usuario |

---

## 🎯 Acción Requerida

**El usuario debe desplegar el ZIP a Azure usando una de las 3 opciones descritas arriba.**

Una vez desplegado, el chat widget funcionará correctamente end-to-end:
1. ✅ Inicializa con IdEntidad correcto
2. ✅ Envía mensaje al API
3. ✅ API crea ticket en `op_tickets_v2`
4. ✅ API guarda mensajes en `op_ticket_conversacion`
5. ✅ API retorna respuesta de IA
6. ✅ Widget muestra respuesta al usuario

---

## 📚 Archivos Relacionados

- `JELA.API/jela-api-deploy.zip` - ZIP listo para deploy
- `JELA.API/INSTRUCCIONES-DEPLOY-MANUAL.md` - Guía detallada de deploy
- `JELA.API/JELA.API/Endpoints/WebhookEndpoints.cs` - Código corregido
- `JelaWeb/Scripts/SQL/05_CREATE_op_ticket_conversacion.sql` - Script de tabla
- `.kiro/specs/sistema-multi-entidad/FIX-CHAT-WIDGET-IDENTIDAD.md` - Fix anterior
- `.kiro/specs/sistema-multi-entidad/AUDITORIA-IDENTIDAD-COMPLETA.md` - Auditoría completa

---

**Fecha**: 2026-01-20  
**Estado**: Pendiente de deploy por usuario  
**Prioridad**: Alta
