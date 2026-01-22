# Instrucciones para Desplegar JELA.API Manualmente

## ✅ Compilación Completada

El proyecto se ha compilado exitosamente y está listo para desplegar.

**Archivo generado**: `JELA.API/jela-api-deploy.zip` (1.99 MB)

---

## 🚀 Opción 1: Desplegar desde Azure Portal (MÁS FÁCIL)

### Paso 1: Abrir Azure Portal
1. Ve a [Azure Portal](https://portal.azure.com)
2. Busca tu App Service: **jela-api-ctb8a6ggbpdqbxhg**

### Paso 2: Desplegar el ZIP
1. En el menú lateral, ve a **"Centro de implementación"** (Deployment Center)
2. En la parte superior, haz clic en **"Implementación manual"** o busca la opción **"ZIP Deploy"**
3. Arrastra el archivo `jela-api-deploy.zip` o haz clic para seleccionarlo
4. Espera 1-2 minutos mientras se despliega
5. Verás un mensaje de confirmación cuando termine

### Paso 3: Verificar
Abre en tu navegador:
```
https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/health
```

Deberías ver:
```json
{
  "Status": "Healthy",
  "Timestamp": "...",
  "Checks": { ... }
}
```

---

## 🚀 Opción 2: Desplegar con Kudu (ALTERNATIVA)

### Paso 1: Abrir Kudu Console
1. Ve a tu App Service en Azure Portal
2. En el menú lateral, busca **"Herramientas avanzadas"** (Advanced Tools)
3. Haz clic en **"Ir"** (Go)
4. Se abrirá Kudu en una nueva pestaña

### Paso 2: Usar ZIP Deploy API
1. En Kudu, ve a **"Tools"** → **"Zip Push Deploy"**
2. Arrastra el archivo `jela-api-deploy.zip` a la zona de arrastre
3. Espera a que termine el despliegue

---

## 🚀 Opción 3: Desplegar con PowerShell (REQUIERE AZURE CLI)

Si tienes Azure CLI instalado:

```powershell
# Instalar Azure CLI primero (si no lo tienes)
# Descarga desde: https://aka.ms/installazurecliwindows

# Login a Azure
az login

# Desplegar el ZIP
az webapp deployment source config-zip `
    --resource-group jela-resources `
    --name jela-api-ctb8a6ggbpdqbxhg `
    --src jela-api-deploy.zip
```

---

## 🧪 Probar el Chat Widget Después del Deploy

### 1. Verificar que la API esté funcionando
```powershell
curl https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/health
```

### 2. Probar el endpoint de chat web
```powershell
$body = '{"nombre":"Test User","email":"test@example.com","telefono":"1234567890","mensaje":"Test message","idEntidad":1,"ipCliente":"127.0.0.1"}'

Invoke-RestMethod -Uri "https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/webhooks/chatweb" -Method POST -Body $body -ContentType "application/json"
```

Deberías recibir una respuesta exitosa con:
```json
{
  "success": true,
  "ticketId": 123,
  "mensaje": "Ticket #123 creado exitosamente",
  "respuestaIA": "...",
  "sessionId": null
}
```

### 3. Verificar en la base de datos
Conéctate a MySQL y verifica:

```sql
-- Ver el ticket creado
SELECT * FROM op_tickets_v2 ORDER BY IdTicket DESC LIMIT 1;

-- Ver los mensajes de conversación
SELECT * FROM op_ticket_conversacion ORDER BY IdConversacion DESC LIMIT 5;
```

---

## 🔍 Cambios Incluidos en Este Deploy

### ✅ Tabla Correcta
- **Antes**: El código podría haber tenido `op_tickets_conversacion` (plural)
- **Ahora**: Usa `op_ticket_conversacion` (singular) ✓

### ✅ Logs Detallados
- Se agregaron logs paso a paso en el endpoint de chat web
- Facilita el debugging si hay errores

### ✅ Manejo de Errores Mejorado
- Mensajes de error más descriptivos
- Stack traces en los logs

---

## ❌ Troubleshooting

### Error: "Table 'jela_qa.op_ticket_conversacion' doesn't exist"
**Solución**: Verifica que la tabla exista en la base de datos:
```sql
SHOW TABLES LIKE 'op_ticket%';
```

Si no existe, ejecuta el script:
```sql
-- Ver: JelaWeb/Scripts/SQL/05_CREATE_op_ticket_conversacion.sql
```

### Error: "500 Internal Server Error" después del deploy
**Solución**: 
1. Ve a Azure Portal → App Service → **"Registros de App Service"**
2. Habilita **"Registro de aplicaciones"** nivel **"Información"**
3. Ve a **"Secuencia de registro"** (Log stream) para ver errores en tiempo real

### Error: "No se puede conectar a la base de datos"
**Solución**: Verifica la connection string en Azure Portal:
1. Ve a **"Configuración"** → **"Cadenas de conexión"**
2. Verifica que `MySQL` esté configurado correctamente

---

## 📝 Notas Importantes

1. **El archivo ZIP ya está listo**: `JELA.API/jela-api-deploy.zip`
2. **No necesitas recompilar**: El ZIP contiene todo lo necesario
3. **El deploy toma 1-2 minutos**: Ten paciencia
4. **Después del deploy**: Prueba el chat widget inmediatamente

---

## 🎯 Siguiente Paso

**Elige una de las 3 opciones arriba y despliega el ZIP.**

La más fácil es la **Opción 1** (Azure Portal con drag & drop).

Una vez desplegado, prueba el chat widget y deberías ver que los mensajes se guardan correctamente en `op_ticket_conversacion`.
