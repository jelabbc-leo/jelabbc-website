# Changelog - Especificaciones Ecosistema JELABBC

## [1.5.0] - Enero 3, 2026 (Nuevo Sistema de Autenticación - API Directa)

### Implementado

#### AuthService.vb - Nuevo Servicio de Autenticación
- Reemplaza `LoginN8N.vb` para mejor rendimiento (n8n era lento)
- Usa `ApiConsumer` (APIs dinámicas) en lugar de webhooks n8n
- Autenticación directa contra tabla `conf_usuarios`
- Hash de contraseña con SHA256
- Retorna `IdEntidad` para soporte multi-tenant
- Obtiene opciones del menú basadas en roles/permisos del usuario

#### Ingreso.aspx.vb - Actualizado
- Usa `AuthService` en lugar de `LoginN8N`
- Inicializa sesión con `IdEntidad` del usuario autenticado
- Mejor manejo de errores y mensajes

### Archivos Modificados
- `JelaWeb/Services/Auth/AuthService.vb` (NUEVO)
- `JelaWeb/Views/Auth/Ingreso.aspx.vb` (MODIFICADO)
- `JelaWeb/JelaWeb.vbproj` (MODIFICADO - actualizada referencia)

### Archivos Eliminados
- `JelaWeb/Services/Auth/LoginN8N.vb` - Eliminado, reemplazado por AuthService

### Beneficios
- **Rendimiento**: Login directo ~100ms vs n8n ~2-3 segundos
- **Multi-tenant**: Retorna `IdEntidad` automáticamente
- **Seguridad**: Usa `QueryBuilder.EscapeSqlString()` para prevenir SQL injection
- **Mantenibilidad**: Código en el proyecto, no depende de flujos externos

---

## [1.4.1] - Enero 3, 2026 (Implementación Multi-tenant en Sesión y Vistas)

### Implementado

#### SessionHelper - Soporte IdEntidad
- Agregado método `GetIdEntidad()` para obtener el ID de entidad de la sesión
- Actualizado `InitializeSession()` con parámetro opcional `idEntidad As Integer = 0`
- La sesión ahora almacena `IdEntidad` para aislamiento multi-tenant

#### Constants.vb - Nueva Constante
- Agregada constante `SESSION_ID_ENTIDAD = "IdEntidad"` para clave de sesión

#### Ingreso.aspx.vb - Extracción de IdEntidad
- Actualizado para extraer `idEntidad` de la respuesta del login API
- Pasa `idEntidad` a `SessionHelper.InitializeSession()`

#### Services Actualizados con IdEntidad
- `CategoriaTicketService.vb` - `GetCategorias(..., Optional idEntidad As Integer = 0)`
- `ProveedorService.vb` - `GetProveedores(..., Optional idEntidad As Integer = 0)`

#### Vistas Actualizadas para Multi-tenant
Las siguientes vistas ahora pasan `SessionHelper.GetIdEntidad()` a los servicios:
- `Unidades.aspx.vb` - `unidadService.ObtenerTodos(SessionHelper.GetIdEntidad())`
- `Fitosanitarios.aspx.vb` - `fitosanitarioService.ObtenerTodos(SessionHelper.GetIdEntidad())`
- `Parcelas.aspx.vb` - `parcelaService.ObtenerTodos(SessionHelper.GetIdEntidad())`
- `TiposSensor.aspx.vb` - `tipoSensorService.ObtenerTodos(SessionHelper.GetIdEntidad())`
- `CategoriasTicket.aspx.vb` - `categoriaService.ObtenerTodos(SessionHelper.GetIdEntidad())`
- `Proveedores.aspx.vb` - `proveedorService.ObtenerTodos(SessionHelper.GetIdEntidad())`

### Pendiente
1. **Ejecutar migración SQL** en ambiente de pruebas
2. ~~**Actualizar API de login** para retornar `idEntidad` del usuario~~ ✓ Resuelto en v1.5.0 con AuthService
3. **Actualizar vistas restantes** (Conceptos, Roles, Entidades, etc.)
4. **Actualizar operaciones CRUD** para incluir `IdEntidad` en INSERT/UPDATE

---

## [1.4.0] - Enero 3, 2026 (Corrección Crítica de Base de Datos - IdEntidad)

### Problema Identificado
Se detectó un error grave en el diseño de la base de datos: la mayoría de las tablas no tienen relación con `cat_entidades`, lo cual impide el aislamiento multi-tenant correcto. El sistema debe ser piramidal con `cat_entidades` como nivel máximo.

### Análisis Realizado

#### Tablas que YA TIENEN IdEntidad (7 tablas) ✓
- `cat_sub_entidades` - IdEntidad
- `cat_medidor` - IdEntidad
- `cat_unidades` - entidad_id
- `op_documentos` - IdEntidad
- `op_tickets` - entidad_id
- `op_fallo_formulario` - entidad_id
- `cat_documentos` - IdEntidad

#### Tablas que NECESITAN IdEntidad (18 tablas) ✗
1. `conf_usuarios` - **CRÍTICO**: Usuarios deben pertenecer a una entidad
2. `op_tickets_v2` - **CRÍTICO**: Tickets v2 sin aislamiento por entidad
3. `cat_conceptos` - Productos/servicios por entidad
4. `cat_proveedores` - Proveedores por entidad
5. `cat_colaboradores` - Colaboradores por entidad
6. `cat_colonias` - Colonias por entidad
7. `cat_categorias_ticket` - Categorías de tickets por entidad
8. `cat_ticket_categorias` - Categorías v2 por entidad
9. `cat_formularios` - Formularios por entidad
10. `cat_roles` - Roles por entidad
11. `conf_roles` - Configuración de roles por entidad
12. `cat_permisos` - Permisos por entidad
13. `conf_opciones` - Opciones de menú por entidad
14. `conf_ticket_sla` - SLA por entidad
15. `conf_ticket_prompts` - Prompts IA por entidad
16. `cat_plantilla_pdf` - Plantillas PDF por entidad
17. `conf_formulario_config` - Config formularios por entidad
18. `conf_perfiles` - Perfiles por entidad

#### Tablas que NO requieren IdEntidad (Catálogos SAT globales)
- `cat_usos_cfdi`, `cat_regimen_fiscal`, `cat_metodo_de_pago`
- `cat_forma_de_pago`, `cat_unidades_medidas`, `cat_tipo_documentos`
- `cat_conceptos_familias`

### Scripts SQL Generados

#### `/Scripts/SQL/migration_add_identidad.sql`
- Agrega columna `IdEntidad INT NOT NULL DEFAULT 1` a las 18 tablas
- Crea índices para optimizar consultas por entidad
- Agrega foreign keys a `cat_entidades`

#### `/Scripts/SQL/migration_update_identidad_data.sql`
- Actualiza índices únicos para incluir IdEntidad (permite mismos nombres en diferentes entidades)
- Consultas de verificación de datos huérfanos

### Cambios en DTOs (Completado)
Se agregó propiedad `IdEntidad As Integer` a los siguientes DTOs:
- `CategoriaTicketDTO.vb` - Categorías de tickets
- `ConfiguracionSLADTO.vb` - Configuración SLA
- `ConceptosDTO.vb` - Productos/servicios
- `ColoniasDTO.vb` - Colonias
- `FitosanitarioDTO.vb` - Productos fitosanitarios
- `FormularioDTO.vb` - Formularios dinámicos
- `ParcelaDTO.vb` - Parcelas
- `ProveedorDTO.vb` - Proveedores
- `RolDTO.vb` - Roles
- `PermisoDTO.vb` - Permisos
- `TipoSensorDTO.vb` - Tipos de sensor

### Nuevos DTOs Creados
- `TicketV2DTO.vb` - Tickets v2 con IA (incluye IdEntidad)
- `UsuarioDTO.vb` - Usuarios del sistema (incluye IdEntidad)
- `SesionUsuarioDTO.vb` - Información de sesión con IdEntidad

### Cambios en Services (Completado)
Se actualizaron los Services para filtrar por `IdEntidad`:
- `UnidadService.vb` - `ObtenerTodos(Optional idEntidad As Integer = 0)`
- `FitosanitarioService.vb` - `ObtenerTodos(Optional idEntidad As Integer = 0)`
- `ParcelaService.vb` - `ObtenerTodos(Optional idEntidad As Integer = 0)`
- `TipoSensorService.vb` - `ObtenerTodos(Optional idEntidad As Integer = 0)`
- `FormularioService.vb` - `GetFormularios(..., Optional idEntidad As Integer = 0)`
- `TicketService.vb` - `GetTickets(..., Optional idEntidad As Integer = 0)`

### Próximos Pasos (Pendientes)
1. **Ejecutar migración SQL en ambiente de pruebas**
2. **Actualizar páginas ASPX**: Obtener `IdEntidad` de sesión y pasarlo a los Services
3. **Actualizar Login**: Almacenar `IdEntidad` del usuario en sesión al autenticar

### Impacto en el Código
- Todos los Services deben incluir `WHERE IdEntidad = @IdEntidad`
- Todos los INSERT deben incluir el `IdEntidad` del usuario
- La sesión debe contener el `IdEntidad` del usuario logueado
- Los grids deben filtrar automáticamente por entidad

---

## [1.3.1] - Enero 3, 2026 (Reorganización y Limpieza de Carpetas)

### Reorganizado

#### Models/DTOs - COMPLETADO ✓
Todos los DTOs consolidados en `/Models/DTOs/`:
- `UnidadDTO.vb`, `FitosanitarioDTO.vb`, `CategoriaTicketDTO.vb`
- `RolDTO.vb`, `ProveedorDTO.vb`, `TipoSensorDTO.vb`, `ParcelaDTO.vb`

#### Content/CSS - COMPLETADO ✓
CSS de módulos consolidados en `/Content/CSS/`:
- `capturadocumentos.css`, `entidades.css`, `gestiondocumentos.css`, `gestiondocumentosmenu.css`
- `iot.css`, `tickets.css`
- Carpeta `/Styles/` ahora solo contiene archivos base: `site.css`, `login.css`, `error.css`, `error-pages.css`

#### Scripts/app - COMPLETADO ✓
JavaScript consolidado en `/Scripts/app/`:
- `/app/Catalogos/`: `Cat_Conceptos.js`, `entidades.js`, `formulario-*.js`
- `/app/IOT/`: `iot.js`
- `/app/Operacion/`: `capturadocumentos.js`, `gestiondocumentosmenu.js`
- `/app/Operacion/Tickets/`: `tickets.js`
- `/app/shared/`: `catalogos.js`

### Eliminado (Archivos sin usar)

#### Scripts eliminados
- `tabs-manager.js` - Sistema MDI con pestañas fue eliminado del proyecto
- `status-bar.js` - Control lblFechaHora fue eliminado del Master Page
- `entidades-init.js` - No referenciado en ninguna página
- `gestiondocumentos.js` - Duplicado (solo se usa gestiondocumentosmenu.js)

#### Documentación eliminada
- `README-CATALOGO-ROLES.md` y `README-CATALOGOS-COMPLETOS.md` de Views/Catalogos
- 14 archivos de documentación obsoleta de la raíz (`SOLUCION-*.md`, `*-MDI.md`, `DEBUG-*.md`)

#### Referencias actualizadas
- `Jela.Master.vb` - Eliminada carga de `status-bar.js`

#### Carpetas Vacías Eliminadas
- `/Scripts/IOT/`, `/Scripts/Catalogos/`, `/Scripts/lib/`, `/Scripts/Operacion/`

---

## [1.3.0] - Enero 3, 2026 (Implementación de Módulos y Estándares)

### Agregado

#### Nueva Carpeta de Configuración
- **Views/Config/**: Nueva carpeta para páginas de configuración del sistema
  - `FormulariosDinamicos.aspx` - Lista de formularios dinámicos
  - `FormularioDisenador.aspx` - Diseñador visual de formularios (actualizado)
  - `FormularioVistaPrevia.aspx` - Vista previa de formularios

#### Diseñador de Formularios Mejorado
- **Nuevos tipos de campo**: 15 tipos de campo soportados
  - Campos de entrada: texto, numero, decimal, fecha, fecha_hora, hora, dropdown, radio, checkbox, textarea, foto, archivo, firma
  - Botones de acción: boton_guardar, boton_cancelar
- **Panel de propiedades mejorado**: 
  - Slider para ancho de columnas (1-12)
  - Slider para altura de textarea (80-400px)
  - Placeholder configurable
- **Toolbar de canvas**: Botones para mover arriba/abajo y eliminar campos
- **Estados de formulario**: borrador, activo, inactivo

#### Nuevas Páginas de Catálogos
- **FormulariosDinamicos.aspx**: Grid de formularios dinámicos con toolbar estándar
- **FormularioDisenador.aspx**: Diseñador visual estilo VS con 3 paneles (Toolbox, Canvas, Properties)
- **FormularioVistaPrevia.aspx**: Vista previa de formularios dinámicos
- **Fitosanitarios.aspx**: Catálogo de productos fitosanitarios
- **Parcelas.aspx**: Catálogo de parcelas agrícolas
- **TiposSensor.aspx**: Catálogo de tipos de sensores IoT
- **Unidades.aspx**: Catálogo de unidades/departamentos
- **Proveedores.aspx**: Catálogo de proveedores
- **CategoriasTicket.aspx**: Catálogo de categorías de tickets con SLA
- **Roles.aspx**: Catálogo de roles del sistema
- **Conceptos.aspx**: Catálogo de conceptos/productos
- **Entidades.aspx**: Catálogo de entidades/organizaciones

#### Nuevos Servicios
- **FormularioService.vb**: CRUD para formularios dinámicos
- **FormularioRenderService.vb**: Renderizado de formularios con DevExpress
- **FitosanitarioService.vb**: CRUD para productos fitosanitarios
- **ParcelaService.vb**: CRUD para parcelas
- **TipoSensorService.vb**: CRUD para tipos de sensores
- **UnidadService.vb**: CRUD para unidades
- **ProveedorService.vb**: CRUD para proveedores
- **CategoriaTicketService.vb**: CRUD para categorías de tickets
- **RolService.vb**: CRUD para roles
- **TicketService.vb**: CRUD para tickets

#### Nuevos DTOs
- **FormularioDTO.vb**: DTO para formularios dinámicos (incluye campos y secciones)

#### Nuevos Assets
- **formulario-disenador.css**: Estilos para el diseñador de formularios
- **formulario-disenador.js**: JavaScript para drag & drop del diseñador

### Modificado

#### Estándares de UI Actualizados
- **Regla 5 (Paginación)**: Marcada como **CRÍTICA** - Todos los grids DEBEN usar `Mode="ShowAllRecords"`
- **Regla 8 (FuncionesGridWeb)**: Agregada implementación obligatoria del evento `DataBound` con `FuncionesGridWeb.SUMColumn`
- **Checklist**: Agregado nuevo ítem para verificar evento DataBound

#### Páginas Actualizadas con Estándares
Las siguientes páginas fueron actualizadas para cumplir con los estándares:
- Unidades.aspx.vb - Agregado `gridUnidades_DataBound`, `Mode="ShowAllRecords"`
- TiposSensor.aspx.vb - Agregado `gridTipos_DataBound`, `Mode="ShowAllRecords"`
- Roles.aspx.vb - Agregado `gridRoles_DataBound`, `Mode="ShowAllRecords"`
- Proveedores.aspx.vb - Agregado `gridProveedores_DataBound`, `Mode="ShowAllRecords"`
- Parcelas.aspx.vb - Agregado `gridParcelas_DataBound`, `Mode="ShowAllRecords"`
- Fitosanitarios.aspx.vb - Agregado `gridFito_DataBound`, `Mode="ShowAllRecords"`
- CategoriasTicket.aspx.vb - Agregado `gridCategorias_DataBound`, `Mode="ShowAllRecords"`
- FormulariosDinamicos.aspx.vb - Agregado `gridFormularios_DataBound`, `Mode="ShowAllRecords"`

### Corregido
- **Logger.LogError**: Corregida firma incorrecta en 6 archivos (se usaba 3 parámetros, la firma correcta es 2)

### Estructura Actual del Proyecto (Reorganizada v1.3.1)

```
JelaWeb/
├── Views/
│   ├── Auth/                          # Autenticación
│   │   ├── Ingreso.aspx
│   │   └── Logout.aspx
│   ├── Catalogos/                     # Catálogos del sistema
│   │   ├── CategoriasTicket.aspx
│   │   ├── Conceptos.aspx
│   │   ├── Entidades.aspx
│   │   ├── Fitosanitarios.aspx
│   │   ├── Parcelas.aspx
│   │   ├── Proveedores.aspx
│   │   ├── Roles.aspx
│   │   ├── TiposSensor.aspx
│   │   └── Unidades.aspx
│   ├── Config/                        # Configuración del sistema
│   │   ├── FormularioDisenador.aspx
│   │   ├── FormulariosDinamicos.aspx
│   │   └── FormularioVistaPrevia.aspx
│   ├── Error/
│   │   └── Error.aspx
│   ├── IOT/                           # Dashboard IoT
│   │   └── IOT.aspx
│   ├── Operacion/                     # Módulos operativos
│   │   ├── GestionDocumentos/
│   │   │   └── GestionDocumentosMenu.aspx
│   │   ├── Tickets/
│   │   │   └── Tickets.aspx
│   │   └── CapturaDocumentos.aspx
│   └── Inicio.aspx
│
├── Services/                          # Servicios de negocio
│   ├── API/                           # Consumo de API
│   │   ├── ApiConsumer.vb
│   │   ├── ApiConsumerCRUD.vb
│   │   ├── ApiService.vb
│   │   └── DynamicDto.vb
│   ├── Auth/
│   │   └── AuthService.vb
│   ├── CategoriaTicketService.vb
│   ├── DocumentIntelligenceService.vb
│   ├── FitosanitarioService.vb
│   ├── FormularioRenderService.vb
│   ├── FormularioService.vb
│   ├── ParcelaService.vb
│   ├── ProveedorService.vb
│   ├── RolService.vb
│   ├── TicketService.vb
│   ├── TipoSensorService.vb
│   └── UnidadService.vb
│
├── Models/                            # ✓ REORGANIZADO
│   └── DTOs/                          # Todos los DTOs aquí
│       ├── CategoriaTicketDTO.vb
│       ├── ChartDataPointDTO.vb
│       ├── ColoniasDTO.vb
│       ├── ConceptosDTO.vb
│       ├── DashboardMetricsDTO.vb
│       ├── FitosanitarioDTO.vb
│       ├── FormularioDTO.vb
│       ├── ParcelaDTO.vb
│       ├── ProveedorDTO.vb
│       ├── RolDTO.vb
│       ├── TicketDTO.vb
│       ├── TipoSensorDTO.vb
│       └── UnidadDTO.vb
│
├── Content/                           # ✓ REORGANIZADO
│   ├── CSS/                           # CSS de módulos (usar esta)
│   │   ├── capturadocumentos.css
│   │   ├── catalogos.css
│   │   ├── entidades.css
│   │   ├── formulario-disenador.css
│   │   ├── formulario-vistaprevia.css
│   │   ├── formularios-dinamicos.css
│   │   ├── gestiondocumentos.css
│   │   ├── iot.css
│   │   └── tickets.css
│   ├── Styles/                        # Solo archivos base
│   │   ├── error-pages.css
│   │   ├── error.css
│   │   ├── login.css
│   │   └── site.css
│   ├── Images/
│   │   └── Iconos/
│   ├── bootstrap.min.css
│   └── Site.css
│
├── Scripts/                           # ✓ REORGANIZADO
│   ├── app/                           # JavaScript de módulos (usar esta)
│   │   ├── Catalogos/
│   │   │   ├── Cat_Conceptos.js
│   │   │   ├── entidades-init.js
│   │   │   ├── entidades.js
│   │   │   ├── formulario-disenador.js
│   │   │   ├── formulario-vistaprevia.js
│   │   │   └── formularios-dinamicos.js
│   │   ├── IOT/
│   │   │   └── iot.js
│   │   ├── Operacion/
│   │   │   ├── Tickets/
│   │   │   │   └── tickets.js
│   │   │   ├── capturadocumentos.js
│   │   │   ├── gestiondocumentos.js
│   │   │   └── gestiondocumentosmenu.js
│   │   └── shared/
│   │       ├── catalogos.js
│   │       ├── status-bar.js
│   │       └── tabs-manager.js
│   ├── SQL/                           # Scripts SQL
│   ├── lib/                           # Librerías externas
│   ├── bootstrap.min.js
│   ├── jquery.min.js
│   └── popper.min.js
│
└── Utilities/
    └── FuncionesGridWeb.vb
```

### Convenciones de Carpetas

| Tipo | Ubicación | Notas |
|------|-----------|-------|
| DTOs | `/Models/DTOs/` | Todos los DTOs van aquí |
| CSS módulos | `/Content/CSS/` | CSS específico de páginas |
| CSS base | `/Content/Styles/` | Solo site.css, login.css, error.css |
| JS módulos | `/Scripts/app/{Modulo}/` | Organizado por módulo |
| JS compartido | `/Scripts/app/shared/` | Funciones reutilizables |

---

## [1.2.0] - Diciembre 26, 2024 (Actualización Completa del Gist)

### Agregado

#### Nuevos Documentos Principales
- **architecture.md**: Arquitectura tecnológica completa del ecosistema
  - Stack tecnológico (Backend VB.NET, Frontend Web Forms, Apps Móviles)
  - Arquitectura de capas (Presentación, Servicios, Negocio, Datos, Externos, Orquestación)
  - Estructura del proyecto completa
  - Configuración y dependencias
  - Patrones de diseño
  - Seguridad y monitoreo
  - Infraestructura Azure
  - CI/CD con Azure DevOps

- **modules.md**: Especificaciones detalladas de módulos
  - Módulo 1: Entidades y Usuarios
  - Módulo 2: Conceptos y Productos
  - Módulo 3: Documentos Operativos
  - Módulo 4: Medidores IoT
  - Módulo 5: Proveedores y Colaboradores
  - Ejemplos de uso del API CRUD dinámico
  - Interfaces web y móvil por módulo
  - Automatizaciones n8n

- **tickets-klarna.md**: Sistema de tickets tipo Klarna con IA
  - Modelo inspirado en Klarna (66% automatización)
  - Estructura completa del ticket
  - Modelo de base de datos (op_tickets_v2)
  - Flujo de trabajo con IA
  - Interfaz de usuario
  - Código VB.NET de ejemplo
  - Métricas y KPIs
  - Plan de migración

- **servicios-municipales.md**: Módulo de servicios municipales
  - Gestión de licitaciones y fallos
  - Flujo multinivel (Entidad → SubEntidad → Proveedor → Colaborador)
  - Órdenes de compra con KPIs
  - Dictámenes técnicos
  - Chat integrado en documentos
  - Modelo de datos usando tablas existentes

### Modificado

- **design.md**: Agregada referencia completa a arquitectura y módulos
- **requirements.md**: Agregada referencia a arquitectura y módulos
- **ui-standards.md**: Ya existía, sin cambios adicionales

### Información Clave del Gist

#### Backend - API CRUD Dinámico
- Un solo controlador (CrudController) maneja TODAS las operaciones CRUD
- Consultas SQL parametrizadas para prevenir SQL injection
- Tipado automático de datos desde MySQL
- Respuestas JSON estructuradas con metadatos de tipo
- Flexibilidad total en consultas SELECT con JOINs

#### Frontend Web
- ASP.NET Web Forms (.NET Framework 4.8.1) + VB.NET
- DevExpress ASP.NET 22.2 (componentes UI oficiales)
- Bootstrap 5 para diseño responsivo
- jQuery + JavaScript modular
- Evolución futura: Blazor o React (no para MVP)

#### Apps Móviles
- .NET MAUI recomendado (multiplataforma)
- Alternativa: Desarrollo nativo (Swift/Kotlin)
- Modo offline con SQLite
- Sincronización bidireccional
- Consume API REST existente

#### Inteligencia Artificial
- Azure OpenAI Service (GPT-4)
- Análisis y clasificación automática de tickets
- Resumen automático de conversaciones
- Análisis de sentimiento
- Agente de voz conversacional 24/7

#### IoT
- Azure IoT Hub
- Sensores de humedad, temperatura, pH
- Alertas automáticas por umbrales
- Control de riego automatizado
- Dashboards de visualización

#### Orquestación con n8n
- Automatización de notificaciones
- Workflows de aprobación
- Integración con servicios externos
- Procesamiento de alertas IoT
- Workflows de IA

### Módulos Documentados

1. **Entidades y Usuarios**: Gestión multi-organización con roles y permisos
2. **Conceptos y Productos**: Catálogo de productos y servicios
3. **Documentos Operativos**: Facturas, órdenes, presupuestos
4. **Medidores IoT**: Sistema de sensores para agricultura
5. **Proveedores y Colaboradores**: Gestión de proveedores externos
6. **Tickets Klarna**: Sistema de atención con IA (66% automatización)
7. **Servicios Municipales**: Licitaciones, fallos, OC multinivel

### Reglas Clave Confirmadas

1. **Nomenclatura de Tablas**: Prefijos con guion bajo (ej: `op_tickets`, `conf_usuarios`)
2. **API CRUD Dinámico**: Un solo controlador para todas las tablas
3. **Separación CSS/JS**: SIEMPRE en archivos externos, NUNCA inline
4. **Toolbar del Grid**: Acciones CRUD SOLO en toolbar
5. **Paginación**: Mostrar TODOS los registros (`PageSize="-1"`)
6. **Filtros**: Solo fechas arriba, otros filtros nativos del grid
7. **Popups**: Toda captura mediante ASPxPopupControl
8. **Clase Helper**: FuncionesGridWeb.vb para estandarizar

### Infraestructura Azure

- App Service S2 (2 cores, 3.5GB RAM)
- MySQL (General Purpose 2vCore)
- Azure Storage, Key Vault, Application Insights
- Azure IoT Hub (S1 tier)
- Azure OpenAI Service
- Costo estimado: ~$366/mes

### Próximos Pasos

1. Implementar módulos faltantes según especificaciones
2. Crear workflows n8n para automatizaciones
3. Desarrollar apps móviles con .NET MAUI
4. Integrar Azure OpenAI para IA
5. Configurar Azure IoT Hub para sensores
6. Implementar sistema de tickets tipo Klarna
7. Desarrollar módulo de servicios municipales

---

## [1.1.0] - Diciembre 26, 2024 (Primera Actualización)

### Agregado
- **Documento de Estándares de UI** (`ui-standards.md`): Documento completo con reglas de interfaz de usuario basadas en el Gist oficial del proyecto

### Modificado
- **design.md**: Agregada sección de referencia a estándares de UI en "Visión General"
- **requirements.md**: Agregada sección de referencia a estándares de UI en "Introducción"

