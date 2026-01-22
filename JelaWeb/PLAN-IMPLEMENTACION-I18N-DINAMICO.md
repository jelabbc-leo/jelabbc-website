# Plan de implementación: Internacionalización dinámica (sin hardcode)

## Objetivo
Centralizar traducciones y formato cultural para que **todo el ecosistema** (WebForms, futuros front-ends y MAUI) use un **mismo origen de verdad** vía la **API**. El sistema debe evitar textos hardcodeados y permitir **cambios dinámicos** por idioma/entidad.

## Alcance
- JelaWeb (ASP.NET WebForms, VB.NET)
- JELA.API (.NET 8)
- Futuros front-ends (MAUI, web adicional)

## Principios
- **Nada hardcodeado** en front (textos, labels, captions, menús).
- **API como fuente única** de traducciones.
- **Front-end 100% dinámico** (UI y textos configurables).
- Cumplimiento de ui-standards.md (separación CSS/JS, toolbar, etc.).

---

## Fase 1: Modelo de datos y API de traducciones

### 1.1 Modelo de datos
Crear tablas en API (no en front):
- `conf_i18n_resources`
  - `Id` (PK)
  - `ResourceKey` (string, único)
  - `Descripcion` (string, opcional)
- `conf_i18n_translations`
  - `Id` (PK)
  - `ResourceId` (FK a resources)
  - `Culture` (string, ej: es-MX, en-US)
  - `Text` (string)
  - `EntidadId` (nullable, para overrides por entidad)
  - `Activo` (bool)

### 1.2 Endpoints en JELA.API
Crear endpoints versionados y cacheables:
- `GET /api/i18n/resources?culture=es-MX&entidadId=1&scope=web`
- `GET /api/i18n/resources/{resourceKey}?culture=...`
- `POST /api/i18n/resources` (admin)
- `POST /api/i18n/translations` (admin)

Reglas:
- Si existe traducción por EntidadId, usarla; si no, usar global.
- Devolver diccionario `ResourceKey -> Text`.
- Incluir `ETag` y cache control.

### 1.3 Caching en API
- Cache por (culture, entidadId, scope) con expiración.
- Invalidación al crear/editar traducciones.

---

## Fase 2: Proveedor de recursos dinámicos en JelaWeb

### 2.1 ResourceProvider personalizado
Implementar `IResourceProvider`/`IResourceProviderFactory` en JelaWeb:
- Consulta a JELA.API
- Cache local (MemoryCache) por cultura/entidad
- Exponer métodos:
  - `GetString(key)`
  - `GetDictionary(scope)`

### 2.2 LocalizationHelper
Reemplazar `App_GlobalResources` por el provider dinámico.
- Mantener `GetString(key)`
- Eliminar dependencias a `.resx` en código.
- Controlar `CurrentCulture` y `CurrentUICulture` con selección del usuario.

### 2.3 Cambio de idioma
- Dropdown de idioma en MasterPage seguirá guardando cultura en sesión.
- Al cambiar idioma: limpia cache de recursos y recarga.

---

## Fase 3: Estandarización de claves (ResourceKey)

### 3.1 Convención de claves
Ejemplos:
- `Common.Save`, `Common.Cancel`, `Common.Delete`
- `Scheduler.View.Day`, `Scheduler.View.Week`
- `Reservaciones.Title`, `Reservaciones.New`

### 3.2 Inventario inicial
- Exportar textos visibles actuales a un CSV base.
- Crear script para poblar `conf_i18n_resources` y `conf_i18n_translations`.

---

## Fase 4: Migración progresiva en WebForms

### 4.1 Reemplazar textos hardcodeados
- Botones, labels, captions, menús, tabs, tooltips.
- En VB/ASPX usar `LocalizationHelper.GetString("...")`.

### 4.2 Scheduler / componentes DevExpress
- Deshabilitar textos fijos y usar recursos dinámicos.
- Tabs y textos de vistas: setear con `GetString()`.
- Context menus personalizados: textos desde recursos.

### 4.3 UI Standards
- Mantener separación CSS/JS.
- No usar inline strings en JS: usar diccionario cargado desde API.

---

## Fase 5: Front-end dinámico (JS)

### 5.1 Carga inicial de diccionario
- Al cargar página, pedir a API `i18n/resources` por scope.
- Guardar en `window.i18n`.

### 5.2 Helper JS
- `t(key)` que retorna traducción.
- Reemplazar textos del DOM dinámicamente cuando aplique.

---

## Fase 6: Backoffice de traducciones

### 6.1 Módulo admin en JelaWeb
- CRUD de `ResourceKey` y traducciones.
- Filtros por cultura, entidad.
- Exportación CSV.

### 6.2 Seguridad
- Roles y permisos (solo admins).

---

## Fase 7: MAUI (futuro)

### 7.1 Cliente MAUI
- Consumir `/api/i18n/resources`.
- Cache local en SQLite o MemoryCache.
- Al cambiar idioma, refrescar diccionario.

### 7.2 UI dinámica
- Binding a diccionario local (ResourceDictionary dinámico).
- Soporte a overrides por entidad.

---

## Fase 8: Observabilidad y calidad

### 8.1 Logging
- Registrar claves faltantes.
- Endpoint en API para reportar faltantes.

### 8.2 Validación
- Script de validación: detectar claves sin traducción por cultura.

---

## Entregables
1. Tablas `conf_i18n_resources` y `conf_i18n_translations`.
2. Endpoints i18n en JELA.API.
3. Provider de recursos dinámico en JelaWeb.
4. Helper JS `t(key)`.
5. Módulo admin de traducciones.
6. Documentación de claves y convención.

---

## Roadmap sugerido
- **Semana 1**: Modelo + API endpoints + cache.
- **Semana 2**: Provider dinámico + integración WebForms.
- **Semana 3**: Migración de módulos críticos (Reservaciones, Tickets, Catálogos).
- **Semana 4**: Admin de traducciones + pruebas.
- **Semana 5**: Preparación MAUI + SDK de i18n compartido.

---

## Notas
- Todas las traducciones deben ir **por API**, nunca locales.
- El sistema debe permitir overrides por **EntidadId**.
- Se debe mantener coherencia con **ui-standards.md**.
