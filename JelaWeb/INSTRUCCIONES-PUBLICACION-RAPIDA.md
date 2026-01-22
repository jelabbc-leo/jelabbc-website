# 🚀 INSTRUCCIONES DE PUBLICACIÓN RÁPIDA

**Sistema:** JELA API - Sistema 100% Dinámico  
**Fecha:** 19 de Enero de 2026  
**Tiempo estimado:** 15-20 minutos

---

## ⚠️ ADVERTENCIA CRÍTICA

**EL SISTEMA ES 100% DINÁMICO Y NO TIENE PROMPTS HARDCODEADOS**

Si no ejecutas el script SQL ANTES de publicar el API, el sistema lanzará excepciones:
```
InvalidOperationException: "Prompt 'XXX' no encontrado en conf_ticket_prompts"
```

**Esto es INTENCIONAL** para forzar la configuración correcta de la BD.

---

## 📋 PASOS DE PUBLICACIÓN (EN ORDEN)

### ✅ PASO 1: Ejecutar Script SQL (OBLIGATORIO)

```bash
# Opción A: Script seguro (RECOMENDADO - maneja duplicados)
mysql -h jela-db-server.mysql.database.azure.com -u admin@jela-db-server -p jela_qa < JELA.API/insert-prompts-iniciales-safe.sql

# Opción B: Script original (solo si BD está limpia)
mysql -h jela-db-server.mysql.database.azure.com -u admin@jela-db-server -p jela_qa < JELA.API/insert-prompts-iniciales.sql
```

**Verificar que funcionó:**
```sql
SELECT COUNT(*) AS Total FROM conf_ticket_prompts WHERE IdEntidad = 1 AND Activo = 1;
-- Debe retornar: Total = 8 (o más si ya existían otros)
```

---

### ✅ PASO 2: Compilar Localmente

```bash
cd JELA.API/JELA.API
dotnet build --configuration Release
```

**Verificar:**
- ✅ Build succeeded
- ✅ 0 Error(s)

---

### ✅ PASO 3: Publicar a Azure

```powershell
# Desde la carpeta JELA.API
.\republish-quick.ps1
```

**O manualmente:**
```bash
cd JELA.API/JELA.API
dotnet publish --configuration Release
# Luego subir a Azure App Service
```

---

### ✅ PASO 4: Reiniciar App Service

```bash
# Desde Azure Portal
# App Service > jela-api-ctb8a6ggbpdqbxhg > Restart

# O desde Azure CLI
az webapp restart --name jela-api-ctb8a6ggbpdqbxhg --resource-group jela-rg
```

---

### ✅ PASO 5: Verificar que Funciona

```bash
# 1. Health check
curl https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/health

# 2. Probar ChatWeb
.\test-widget-azure.ps1
```

**Revisar logs en Azure:**
- ✅ Debe aparecer: "Prompt encontrado: ChatWebSistema"
- ✅ Debe aparecer: "Prompt encontrado: ChatWebUsuario"
- ❌ NO debe aparecer: "Prompt no encontrado"

---

## 🔍 VERIFICACIÓN RÁPIDA

### Checklist Post-Publicación

- [ ] Script SQL ejecutado ✅
- [ ] 8 prompts en BD ✅
- [ ] API compilada sin errores ✅
- [ ] API publicada a Azure ✅
- [ ] App Service reiniciado ✅
- [ ] Health check OK ✅
- [ ] ChatWeb funciona ✅
- [ ] Sin errores en logs ✅

---

## 🚨 SOLUCIÓN DE PROBLEMAS

### Error: "Prompt 'XXX' no encontrado"

**Causa:** No ejecutaste el script SQL

**Solución:**
```bash
# Ejecutar script SQL inmediatamente
mysql -h jela-db-server.mysql.database.azure.com -u admin@jela-db-server -p jela_qa < JELA.API/insert-prompts-iniciales-safe.sql

# Reiniciar API
az webapp restart --name jela-api-ctb8a6ggbpdqbxhg --resource-group jela-rg
```

---

### Error: "Duplicate entry '1-ChatWebSistema'"

**Causa:** Los prompts ya existen en BD

**Solución:**
```bash
# Usar el script seguro que maneja duplicados
mysql -h jela-db-server.mysql.database.azure.com -u admin@jela-db-server -p jela_qa < JELA.API/insert-prompts-iniciales-safe.sql
```

---

### Error 500 en ChatWeb

**Causa:** Prompts no existen o conexión a BD falla

**Solución:**
```sql
-- Verificar prompts en BD
SELECT NombrePrompt, Activo FROM conf_ticket_prompts WHERE IdEntidad = 1;

-- Si no hay prompts, ejecutar script
source JELA.API/insert-prompts-iniciales-safe.sql;
```

---

## 📊 COMANDOS ÚTILES

### Verificar Prompts en BD
```sql
SELECT 
    NombrePrompt,
    Canal,
    LENGTH(ContenidoPrompt) AS Longitud,
    Activo,
    FechaCreacion
FROM conf_ticket_prompts
WHERE IdEntidad = 1
ORDER BY NombrePrompt;
```

### Ver Logs en Tiempo Real
```bash
# Azure Portal > App Service > Log Stream
# O desde CLI:
az webapp log tail --name jela-api-ctb8a6ggbpdqbxhg --resource-group jela-rg
```

### Verificar Tickets Creados
```sql
SELECT 
    Id,
    Canal,
    AsuntoCorto,
    RespuestaIA IS NOT NULL AS TieneRespuestaIA,
    FechaCreacion
FROM op_tickets_v2
WHERE FechaCreacion >= NOW() - INTERVAL 1 HOUR
ORDER BY FechaCreacion DESC
LIMIT 10;
```

---

## 🎯 RESUMEN DE 1 MINUTO

```bash
# 1. SQL (OBLIGATORIO)
mysql -h jela-db-server.mysql.database.azure.com -u admin@jela-db-server -p jela_qa < JELA.API/insert-prompts-iniciales-safe.sql

# 2. Compilar
cd JELA.API/JELA.API && dotnet build --configuration Release

# 3. Publicar
.\republish-quick.ps1

# 4. Reiniciar
az webapp restart --name jela-api-ctb8a6ggbpdqbxhg --resource-group jela-rg

# 5. Verificar
curl https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/health
.\test-widget-azure.ps1
```

---

## 📚 DOCUMENTACIÓN COMPLETA

Para más detalles, consulta:
- [RESUMEN-FINAL-SISTEMA-DINAMICO.md](./RESUMEN-FINAL-SISTEMA-DINAMICO.md)
- [VALIDACION-SISTEMA-100-DINAMICO.md](./VALIDACION-SISTEMA-100-DINAMICO.md)
- [CHECKLIST-REFACTORIZACION-PROMPTS.md](./CHECKLIST-REFACTORIZACION-PROMPTS.md)

---

**¡Listo! El sistema está 100% dinámico y listo para producción.**
