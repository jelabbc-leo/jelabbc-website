# Plan de Implementación - Ecosistema JELABBC

## 📊 Estado Actual del Proyecto (Actualizado: Diciembre 2024)

### ✅ Módulos Completados por Cursor

**Infraestructura Base:**
- ✅ Helpers de infraestructura (AuthHelper, SessionHelper, Logger, QueryBuilder, SecurityHelper, etc.)
- ✅ FuncionesGridWeb.vb para formateo automático de grids DevExpress
- ✅ ApiConsumer y ApiConsumerCRUD para integración con backend REST
- ✅ **Sistema de Pestañas MDI** - Implementado con DevExpress ASPxPageControl
  - Pestañas dinámicas con iframes
  - Validación de duplicados
  - Botones de cierre en cada pestaña
  - Integración completa con Ribbon
  - Detección de carga directa
  - Ver documentación: `MDI-TABS-SYSTEM.md`

**Catálogos Implementados (8 de 8):**
1. ✅ **Roles** - CRUD completo con asignación de permisos por módulo
2. ✅ **Categorías de Tickets** - CRUD con jerarquía y configuración SLA
3. ✅ **Proveedores** - CRUD con datos fiscales y evaluación
4. ✅ **Conceptos** - CRUD con categorización e impuestos
5. ✅ **Tipos de Sensor IoT** - CRUD con unidades de medida y umbrales
6. ✅ **Parcelas Agrícolas** - CRUD con coordenadas GPS
7. ✅ **Fitosanitarios** - CRUD con datos de seguridad y stock
8. ✅ **Unidades/Departamentos** - CRUD con jerarquía torre/edificio/unidad

**Módulos Operacionales:**
- ✅ **Entidades** - CRUD completo con SubEntidades
- ✅ **Tickets** - Sistema completo con:
  - Creación con procesamiento IA automático
  - Categorización y priorización automática
  - Detección de sentimiento
  - Resolución automática con IA
  - Sistema de conversación integrado
  - Adjuntos y ubicación GPS
  - Workflow de estados
  - Calificación de servicio

**Servicios Implementados:**
- ✅ RolService.vb
- ✅ CategoriaTicketService.vb
- ✅ ProveedorService.vb
- ✅ TicketService.vb
- ✅ TipoSensorService.vb
- ✅ ParcelaService.vb
- ✅ FitosanitarioService.vb
- ✅ UnidadService.vb

**Páginas ASPX Implementadas:**
- ✅ 9 páginas de catálogos en `/Views/Catalogos/`
- ✅ 1 página de tickets en `/Views/Operacion/Tickets/`
- ✅ Todas con DevExpress grids, popups y validaciones

### ⚠️ Tareas Parcialmente Completadas

**Parcelas:**
- ⚠️ Falta: Asociación de sensores a parcelas
- ⚠️ Falta: Visualización en mapa

**Unidades:**
- ⚠️ Falta: Asociación de residentes a unidades

### 🔴 Módulos Pendientes de Implementación

**Portal Web:**
- 🔴 Dashboard personalizado por rol
- 🔴 Usuarios y gestión de permisos
- 🔴 Órdenes de Compra con validación IA
- 🔴 Dictámenes Técnicos
- 🔴 Facturación y Tarifas
- 🔴 Agricultura IoT (dashboard, alertas, riego automatizado)
- 🔴 Comunicación con residentes
- 🔴 Reportes y analítica
- 🔴 Agente de Voz IA
- 🔴 Internacionalización (i18n)
- 🔴 Seguridad Multinivel para documentos
- 🔴 Servicios Municipales (Fase 23)

**Aplicación Móvil:**
- 🔴 Toda la aplicación móvil (Fases 9-17)

**Integración y Testing:**
- 🔴 Property-based tests
- 🔴 Unit tests
- 🔴 Integration tests
- 🔴 Integración N8N
- 🔴 Optimizaciones de rendimiento
- 🔴 Accesibilidad
- 🔴 Auditoría de seguridad

### 📈 Progreso General

**Portal Web:** ~25% completado
- Infraestructura: 80%
- Catálogos: 100%
- Módulos Core: 30%
- Módulos Avanzados: 0%

**Aplicación Móvil:** 0% completado

**Testing:** 0% completado

**Tiempo Estimado Restante:**
- Con 1 Dev Senior + IA: 28-32 semanas (~7-8 meses)
- Con equipo tradicional: 40-60 semanas (~10-15 meses)

---

Este plan de implementación se enfoca en completar el Portal Web y desarrollar las Aplicaciones Móviles del Ecosistema JELABBC. El backend ya está operativo, por lo que las tareas se centran en frontend, integración con APIs existentes y funcionalidades móviles.

## Estimaciones de Tiempo por Fase

**Nota:** Las estimaciones asumen un equipo de desarrollo con experiencia en las tecnologías utilizadas (ASP.NET, VB.NET, DevExpress, MAUI/Xamarin, Azure). Los tiempos son aproximados y pueden variar según la complejidad real encontrada durante la implementación.

### Portal Web (Fases 1-8)

| Fase | Descripción | Tareas | Tiempo Estimado | Desarrolladores |
|------|-------------|--------|-----------------|-----------------|
| **Fase 1** | Infraestructura y Fundamentos | 4 tareas | 1-2 semanas | 2 devs |
| **Fase 1.5** | Seguridad Multinivel | 14 tareas | 1-2 semanas | 2 devs |
| **Fase 2** | Catálogos Base | 9 tareas | 2-3 semanas | 2 devs |
| **Fase 3** | Dashboard y Módulos Core | 4 tareas | 2-3 semanas | 2 devs |
| **Fase 4** | Módulo de Tickets | 5 tareas | 2-3 semanas | 2 devs |
| **Fase 5** | Órdenes de Compra y Dictámenes | 5 tareas | 3-4 semanas | 2 devs |
| **Fase 6** | Facturación y Tarifas | 4 tareas | 2-3 semanas | 2 devs |
| **Fase 7** | Agricultura Inteligente e IoT | 5 tareas | 3-4 semanas | 2 devs |
| **Fase 8** | Comunicación y Reportes | 4 tareas | 2-3 semanas | 2 devs |
| **Fase 9** | Agente de Voz IA | 3 tareas | 2-3 semanas | 1 dev + 1 IA specialist |

**Subtotal Portal Web:** 18-30 semanas (4.5-7.5 meses) con 2 desarrolladores

### Aplicación Móvil (Fases 10-18)

| Fase | Descripción | Tareas | Tiempo Estimado | Desarrolladores |
|------|-------------|--------|-----------------|-----------------|
| **Fase 10** | Infraestructura Base Móvil | 5 tareas | 2-3 semanas | 2 mobile devs |
| **Fase 9.5** | Seguridad Multinivel Móvil | 6 tareas | 1 semana | 2 mobile devs |
| **Fase 11** | Autenticación y Onboarding | 5 tareas | 2-3 semanas | 2 mobile devs |
| **Fase 12** | Dashboard y Notificaciones | 4 tareas | 2 semanas | 2 mobile devs |
| **Fase 13** | Gestión de Tickets Móvil | 5 tareas | 3-4 semanas | 2 mobile devs |
| **Fase 14** | Órdenes de Trabajo (Técnicos) | 5 tareas | 2-3 semanas | 2 mobile devs |
| **Fase 15** | Saldo y Pagos (Residentes) | 4 tareas | 2-3 semanas | 2 mobile devs |
| **Fase 16** | Monitoreo Agrícola IoT | 6 tareas | 3-4 semanas | 2 mobile devs |
| **Fase 17** | Chat con Soporte | 3 tareas | 1-2 semanas | 2 mobile devs |
| **Fase 18** | Internacionalización Móvil | 3 tareas | 1-2 semanas | 2 mobile devs |

**Subtotal App Móvil:** 19-28 semanas (4.75-7 meses) con 2 desarrolladores móviles

### Integración y Finalización (Fases 19-22)

| Fase | Descripción | Tareas | Tiempo Estimado | Desarrolladores |
|------|-------------|--------|-----------------|-----------------|
| **Fase 19** | Integración N8N y Optimizaciones | 5 tareas | 2-3 semanas | 2 devs |
| **Fase 20** | Accesibilidad y Seguridad | 5 tareas | 2-3 semanas | 2 devs + 1 security |
| **Fase 21** | Testing Integral y Documentación | 6 tareas | 3-4 semanas | 2 devs + 1 QA |
| **Fase 22** | Preparación para Producción | 6 tareas | 2-3 semanas | 2 devs + 1 DevOps |

**Subtotal Integración:** 9-13 semanas (2-3 meses)

## Resumen de Estimaciones

### 🤖 1 Desarrollador Senior + IA (Kiro/Cursor) - RECOMENDADO

**Ventajas de IA:**
- Generación automática de código boilerplate (60-70% más rápido)
- Autocompletado inteligente de funciones completas
- Generación de tests automática
- Refactoring asistido
- Documentación automática
- Detección temprana de errores

**Desarrollo Completo:**
- **Portal Web:** 10-14 semanas (2.5-3.5 meses)
- **App Móvil:** 12-16 semanas (3-4 meses)
- **Integración y Testing:** 4-6 semanas (1-1.5 meses)
- **Tiempo Total:** 26-36 semanas (6.5-9 meses)
- **Equipo:** 1 desarrollador senior full-stack + IA

**MVP Rápido (Fases críticas 1-4, 10-13):**
- **Tiempo Total:** 12-16 semanas (3-4 meses)
- **Funcionalidades:** Login, Dashboard, Entidades, Tickets, Catálogos base (web + móvil)

**MVP Ultra-Rápido (Solo Portal Web crítico):**
- **Tiempo Total:** 8-10 semanas (2-2.5 meses)
- **Funcionalidades:** Login, Dashboard, Entidades, Tickets, Catálogos base

---

### 👥 Equipo Tradicional (sin IA significativa)

**Desarrollo Secuencial:**
- **Tiempo Total:** 44-68 semanas (11-17 meses)
- **Equipo:** 2 backend devs + 2 frontend devs + 2 mobile devs + 1 QA + 1 DevOps

**Desarrollo Paralelo:**
- **Tiempo Total:** 26-41 semanas (6.5-10 meses)
- **Equipo:** 2 backend devs + 2 frontend devs + 2 mobile devs + 1 QA + 1 DevOps + 1 IA specialist

**MVP Tradicional:**
- **Tiempo Total:** 20-30 semanas (5-7.5 meses)
- **Equipo:** 2 backend devs + 2 frontend devs + 2 mobile devs + 1 QA + 1 DevOps

## Estimaciones Detalladas por Tipo de Tarea

### 🤖 Con Desarrollador Senior + IA (Kiro/Cursor)

| Tipo de Tarea | Sin IA | Con IA | Aceleración | Notas |
|---------------|--------|--------|-------------|-------|
| **Infraestructura y Configuración** | 3-5 días | 1-2 días | 60-70% | IA genera configuraciones base |
| **Catálogos CRUD** | 2-4 días | 0.5-1.5 días | 70-75% | IA genera CRUD completo con prompts |
| **Módulos Complejos** | 5-10 días | 2-4 días | 60-70% | IA ayuda con lógica y UI |
| **Integración con IA** | 5-8 días | 2-4 días | 50-60% | IA genera código de integración |
| **Integración IoT** | 5-8 días | 2-4 días | 50-60% | IA ayuda con protocolos y parsing |
| **Sincronización Offline** | 8-12 días | 3-5 días | 60-65% | IA genera lógica de sync y conflictos |
| **Property Tests** | 1-2 días | 0.5-1 día | 50-60% | IA genera tests y generadores |
| **Unit Tests** | 0.5-1 día | 0.25-0.5 días | 50-60% | IA genera tests automáticamente |
| **Documentación** | 3-5 días | 1-2 días | 60-70% | IA genera docs desde código |

### Ejemplos Concretos de Aceleración con IA

**Catálogo CRUD (ej: Proveedores):**
- **Sin IA:** 3 días
  - Día 1: Crear página ASPX, diseñar grid DevExpress
  - Día 2: Implementar Service, validaciones, API calls
  - Día 3: Testing, ajustes, debugging
  
- **Con IA:** 1 día
  - Hora 1-2: Prompt a IA: "Crear página Proveedores.aspx con CRUD completo usando DevExpress"
  - Hora 3-4: Revisar código generado, ajustar validaciones específicas
  - Hora 5-6: Integrar con API existente, testing
  - Hora 7-8: Ajustes finales, documentación

**Módulo de Tickets Completo:**
- **Sin IA:** 8 días
- **Con IA:** 3 días
  - Día 1: IA genera estructura base, formularios, grids
  - Día 2: Implementar lógica de negocio específica, workflow
  - Día 3: Testing, integración con notificaciones

**Sincronización Offline Móvil:**
- **Sin IA:** 10 días
- **Con IA:** 4 días
  - Día 1: IA genera base de datos SQLite, modelos
  - Día 2: IA genera SyncService, QueueManager
  - Día 3: Implementar resolución de conflictos específica
  - Día 4: Testing exhaustivo, edge cases

## Factores que Pueden Afectar las Estimaciones

**Factores que Reducen Tiempo:**
- ✅ Backend ya operativo
- ✅ Algunos módulos ya implementados (Login, Entidades, Conceptos)
- ✅ Flujos N8N ya creados
- ✅ Experiencia del equipo con las tecnologías
- ✅ Tests opcionales para MVP rápido

**Factores que Aumentan Tiempo:**
- ⚠️ Complejidad de integración con Azure OpenAI
- ⚠️ Complejidad de sincronización offline
- ⚠️ Testing exhaustivo de IoT en campo
- ⚠️ Certificaciones de seguridad
- ⚠️ Publicación en App Store y Google Play
- ⚠️ Curva de aprendizaje de DevExpress (si el equipo no lo conoce)

## Recomendaciones

1. **Iniciar con MVP:** Implementar Fases 1-4 y 10-13 primero (5-7.5 meses)
2. **Desarrollo Paralelo:** Portal web y app móvil en paralelo con equipos separados
3. **Iteraciones Cortas:** Sprints de 2 semanas con demos al final de cada sprint
4. **Testing Continuo:** Ejecutar tests en cada commit, no dejar para el final
5. **Documentación Incremental:** Documentar mientras se desarrolla, no al final
6. **Validación Temprana:** Involucrar usuarios reales desde el MVP

## Hitos Clave

### 🤖 Con 1 Desarrollador Senior + IA

| Hito | Descripción | Tiempo Estimado | Semanas Acumuladas |
|------|-------------|-----------------|-------------------|
| **M1** | Portal Web MVP (Login, Dashboard, Entidades, Tickets, Catálogos, Seguridad Multinivel) | Semana 9 | 9 semanas |
| **M2** | Portal Web Completo (todos los módulos) | Semana 15 | 15 semanas |
| **M3** | App Móvil MVP (Login, Dashboard, Tickets, Seguridad Multinivel) | Semana 22 | 22 semanas |
| **M4** | App Móvil Completa (todos los módulos) | Semana 28 | 28 semanas |
| **M5** | Integración, Testing y Optimización | Semana 32 | 32 semanas |
| **M6** | Documentación y Preparación Producción | Semana 34 | 34 semanas |
| **M7** | Lanzamiento a Producción | Semana 36 | 36 semanas |

**Tiempo Total: 36 semanas (9 meses)**

### 👥 Con Equipo Tradicional (sin IA)

| Hito | Descripción | Tiempo Estimado |
|------|-------------|-----------------|
| **M1** | Portal Web MVP | Mes 3 |
| **M2** | Portal Web Completo | Mes 7 |
| **M3** | App Móvil MVP | Mes 5 |
| **M4** | App Móvil Completa | Mes 9 |
| **M5** | Integración y Testing | Mes 11 |
| **M6** | Producción y Lanzamiento | Mes 12 |

## Desglose Semanal Detallado (1 Dev Senior + IA)

### Semanas 1-9: Portal Web MVP
- **Semana 1-2:** Infraestructura, i18n, autenticación mejorada
- **Semana 3:** Seguridad Multinivel (servicios, DTOs, integración)
- **Semana 4-6:** Catálogos base (8 catálogos)
- **Semana 7-8:** Dashboard y módulo de Entidades completo
- **Semana 9:** Módulo de Tickets MVP + Testing

### Semanas 10-15: Portal Web Completo
- **Semana 10-11:** Órdenes de Compra + Integración IA
- **Semana 12:** Facturación y Tarifas
- **Semana 13-14:** Agricultura IoT + Riego Automatizado
- **Semana 15:** Comunicación, Reportes + Testing

### Semanas 16-22: App Móvil MVP
- **Semana 16-17:** Infraestructura móvil + Sincronización offline
- **Semana 18:** Seguridad Multinivel Móvil
- **Semana 19:** Autenticación biométrica + Onboarding
- **Semana 20:** Dashboard móvil + Notificaciones push
- **Semana 21-22:** Tickets móvil completo + Testing

### Semanas 23-28: App Móvil Completa
- **Semana 23:** Órdenes de Trabajo (Técnicos)
- **Semana 24:** Saldo y Pagos (Residentes)
- **Semana 25-26:** Monitoreo Agrícola IoT móvil
- **Semana 27:** Chat con Soporte
- **Semana 28:** Internacionalización móvil + Testing

### Semanas 29-36: Integración y Producción
- **Semana 29-30:** Integración N8N + Optimizaciones
- **Semana 31:** Accesibilidad y Seguridad
- **Semana 32-33:** Testing integral (property tests, integration tests)
- **Semana 34:** Documentación completa
- **Semana 35:** Preparación producción + Auditoría seguridad
- **Semana 36:** Despliegue y lanzamiento

**Tiempo Total Actualizado: 36 semanas (9 meses)**

## Estrategia de Trabajo Recomendada con IA

### Flujo de Trabajo Diario
1. **Mañana (4 horas):**
   - Usar IA para generar código base de nuevas funcionalidades
   - Revisar y ajustar código generado
   - Implementar lógica de negocio específica

2. **Tarde (4 horas):**
   - Testing y debugging
   - Integración con componentes existentes
   - Documentación (asistida por IA)

### Mejores Prácticas con IA
- ✅ Usar specs detallados como contexto para la IA
- ✅ Generar código en bloques pequeños e iterativos
- ✅ Revisar y entender todo el código generado
- ✅ Usar IA para tests automáticos
- ✅ Aprovechar IA para refactoring y optimización
- ✅ Generar documentación automáticamente

### Riesgos y Mitigaciones
- ⚠️ **Riesgo:** Dependencia excesiva de IA sin entender el código
  - **Mitigación:** Revisar y entender cada línea generada
  
- ⚠️ **Riesgo:** Código generado con bugs sutiles
  - **Mitigación:** Testing exhaustivo, property tests
  
- ⚠️ **Riesgo:** Inconsistencias en estilo de código
  - **Mitigación:** Configurar linters y formatters desde el inicio

## Comparativa de Costos

### 1 Desarrollador Senior + IA (8.5 meses)
- **Salario:** $8,000-12,000 USD/mes × 8.5 = $68,000-102,000 USD
- **Licencias IA:** $20-50 USD/mes × 8.5 = $170-425 USD
- **Total:** $68,170-102,425 USD

### Equipo Tradicional (10 meses promedio)
- **8 personas × $6,000-10,000 USD/mes × 10** = $480,000-800,000 USD
- **Total:** $480,000-800,000 USD

**Ahorro con IA: 85-90%** (considerando solo costos de personal)

## Fase 0: Estructura de Base de Datos

- [ ] 0.1 Crear estructura de base de datos para módulo de Tickets
  - Ejecutar script SQL `sql/dia-2-tickets-module.sql` en base de datos MySQL (jela_qa)
  - Verificar creación de 7 tablas: cat_categorias_ticket, op_tickets, op_ticket_adjuntos, op_ticket_comentarios, op_ticket_timeline, op_ticket_notificaciones, conf_ticket_sla
  - Verificar creación de vista vw_tickets_completo
  - Verificar creación de stored procedures: sp_crear_ticket, sp_asignar_ticket, sp_cambiar_estado_ticket
  - Verificar creación de trigger trg_ticket_primera_respuesta
  - Verificar datos iniciales: 7 categorías y 12 configuraciones SLA
  - Consultar archivo `sql/INSTRUCCIONES-DIA-2.md` para detalles de ejecución
  - _Requerimientos: 5.1, 5.2, 5.3, 5.4, 5.5_
  - _Nota: Esta tarea usa la estructura de base de datos existente con nomenclatura: cat_ (catálogos), op_ (operaciones), conf_ (configuración)_
  - _Nota: Hace referencia a tablas existentes: conf_usuarios, cat_entidades, cat_sub_entidades, conf_roles_

- [ ] 0.2 Crear estructura de base de datos para módulo de Órdenes de Compra
  - Crear tabla op_ordenes_compra con campos: folio, fecha, proveedor_id, monto_total, estado, etc.
  - Crear tabla op_orden_compra_detalles con conceptos y montos
  - Crear tabla op_orden_compra_adjuntos para documentos
  - Crear tabla op_orden_compra_aprobaciones para workflow
  - Crear tabla op_orden_compra_validaciones_ia para resultados de IA
  - Agregar foreign keys a cat_proveedores, conf_usuarios, cat_entidades
  - _Requerimientos: 6.1, 6.2, 6.3, 6.4, 6.5_

- [ ] 0.3 Crear estructura de base de datos para módulo de Dictámenes
  - Crear tabla op_dictamenes_tecnicos con campos: folio, fecha, tipo, descripcion, etc.
  - Crear tabla op_dictamen_adjuntos para documentos y evidencias
  - Crear tabla op_dictamen_validaciones_ia para análisis de IA
  - Agregar foreign keys a conf_usuarios, cat_entidades
  - _Requerimientos: 7.1, 7.2, 7.3, 7.4, 7.5_

- [ ] 0.4 Crear estructura de base de datos para módulo de Facturación
  - Crear tabla cat_tarifas con conceptos, montos y periodicidad
  - Crear tabla op_facturas con datos de facturación
  - Crear tabla op_factura_detalles con conceptos facturados
  - Crear tabla op_pagos con registro de pagos
  - Crear tabla op_saldos para control de saldos por entidad/unidad
  - Agregar foreign keys a cat_conceptos, cat_entidades, cat_sub_entidades, conf_usuarios
  - _Requerimientos: 8.1, 8.2, 8.3, 8.4, 8.5_

- [ ] 0.5 Crear estructura de base de datos para módulo de Agricultura IoT
  - Crear tabla cat_tipos_sensor con tipos de sensores y unidades de medida
  - Crear tabla cat_parcelas con ubicación geográfica y datos de parcelas
  - Crear tabla op_sensores con sensores instalados
  - Crear tabla op_lecturas_sensor con datos de sensores en tiempo real
  - Crear tabla op_alertas_iot con alertas generadas
  - Crear tabla op_riego_programado con programación de riego
  - Crear tabla op_riego_historial con historial de activaciones
  - Crear tabla op_aplicaciones_fitosanitarias con trazabilidad
  - Agregar foreign keys a cat_entidades, cat_sub_entidades, conf_usuarios
  - _Requerimientos: 9.1, 9.2, 9.3, 9.4, 9.5, 10.1, 10.2, 10.3, 10.4, 10.5_

- [ ] 0.6 Crear estructura de base de datos para módulo de Comunicación
  - Crear tabla op_avisos con comunicados a residentes
  - Crear tabla op_aviso_destinatarios con lista de destinatarios
  - Crear tabla op_aviso_lecturas con registro de lectura
  - Agregar foreign keys a conf_usuarios, cat_entidades, cat_sub_entidades
  - _Requerimientos: 24.1, 24.2, 24.3, 24.4, 24.5_

- [ ] 0.7 Crear estructura de base de datos para módulo de Reportes
  - Crear tabla conf_plantillas_reporte con plantillas predefinidas
  - Crear tabla op_reportes_generados con historial de reportes
  - Crear tabla conf_reportes_programados con configuración de reportes recurrentes
  - Agregar foreign keys a conf_usuarios
  - _Requerimientos: 23.1, 23.2, 23.3, 23.4, 23.5_

- [ ] 0.8 Crear estructura de base de datos para Agente de Voz IA
  - Crear tabla op_llamadas_voz con registro de llamadas
  - Crear tabla op_llamada_transcripciones con transcripciones
  - Crear tabla op_llamada_acciones con acciones ejecutadas
  - Agregar foreign keys a conf_usuarios, op_tickets
  - _Requerimientos: 11.1, 11.2, 11.3, 11.4, 11.5_

- [ ] 0.9 Crear estructura de base de datos para Seguridad Multinivel
  - Ejecutar script SQL `sql/documentos-multinivel.sql` en base de datos MySQL (jela_qa)
  - Crear tabla op_documento_security_log para auditoría de accesos
  - Crear stored procedure sp_obtener_partidas_por_nivel con filtrado de seguridad
  - Crear stored procedure sp_obtener_documentos_por_nivel para listado filtrado
  - Crear función fn_verificar_acceso_documento para validación de acceso
  - Agregar campos de nivel jerárquico a tabla conf_usuarios si no existen
  - Agregar campos de montos multinivel a tabla op_documentos_detalle
  - Verificar índices de rendimiento en tablas de seguridad
  - _Requerimientos: 27.1, 27.2, 27.3, 27.4, 27.5, 27.6, 27.7_
  - _Nota: Implementa seguridad a nivel de base de datos para flujo multinivel de documentos_

- [ ] 0.10 Verificar integridad referencial completa
  - Ejecutar consultas de verificación de foreign keys
  - Verificar que todas las tablas tienen índices apropiados
  - Verificar que todas las tablas tienen campos de auditoría (fecha_creacion, fecha_modificacion)
  - Documentar estructura completa de base de datos
  - Crear diagrama ER de la base de datos

- [ ] 0.11 Checkpoint - Verificar estructura de base de datos
  - Asegurar que todas las tablas se crearon correctamente
  - Verificar que los datos iniciales se insertaron
  - Ejecutar consultas de prueba en cada tabla
  - Preguntar al usuario si surgen dudas

## Fase 1: Infraestructura y Fundamentos

- [ ] 1. Configurar infraestructura de internacionalización
  - Crear estructura de archivos de recursos para español e inglés
  - Implementar LocalizationService.vb para el portal web
  - Crear selector de idioma en Master Page
  - Configurar detección automática de idioma del navegador
  - _Requerimientos: 26.1, 26.2, 26.4, 26.5_

- [ ]* 1.1 Escribir property test para cambio de idioma
  - **Property 30: Cambio de idioma actualiza interfaz**
  - **Valida: Requerimientos 26.2**

- [ ]* 1.2 Escribir property test para fallback de traducciones
  - **Property 31: Textos sin traducción usan fallback**
  - **Valida: Requerimientos 26.4**

- [ ] 2. Mejorar módulo de autenticación existente
  - Agregar soporte para autenticación biométrica web (WebAuthn)
  - Implementar validación de complejidad de contraseñas
  - Agregar bloqueo de cuenta por intentos fallidos
  - Mejorar manejo de expiración de sesión
  - _Requerimientos: 1.2, 21.2, 21.3_

- [ ]* 2.1 Escribir property test para autenticación exitosa
  - **Property 1: Autenticación exitosa crea sesión válida**
  - **Valida: Requerimientos 1.1**

- [ ]* 2.2 Escribir property test para credenciales inválidas
  - **Property 3: Credenciales inválidas rechazan acceso**
  - **Valida: Requerimientos 1.3**

- [ ]* 2.3 Escribir property test para logout
  - **Property 5: Logout limpia datos sensibles**
  - **Valida: Requerimientos 1.5**

- [ ]* 2.4 Escribir property test para validación de contraseñas
  - **Property 7: Contraseñas cumplen complejidad**
  - **Valida: Requerimientos 21.2**


- [ ] 3. Implementar sistema de permisos granulares
  - Crear tabla de permisos en base de datos (si no existe)
  - Implementar AuthorizationHelper.vb para validación de permisos
  - Agregar atributos de autorización a páginas y métodos
  - Implementar validación de permisos en cada operación
  - _Requerimientos: 1.6, 4.4_

- [ ]* 3.1 Escribir property test para validación de permisos
  - **Property 6: Operaciones validan permisos**
  - **Valida: Requerimientos 1.6**

- [ ] 4. Checkpoint - Verificar autenticación y permisos
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 1.5: Portal Web - Seguridad Multinivel para Documentos

- [ ] 4.1 Implementar servicios de seguridad multinivel
  - Crear DocumentoSecurityService.vb en JelaWeb/Services/Security/
  - Implementar método ValidarAccesoDocumento(documentoId, usuarioId) con validación por nivel
  - Implementar método GetColumnasVisibles(nivelUsuario) para visibilidad de columnas
  - Implementar método FiltrarPartidas(partidas, nivelUsuario) para filtrado de montos
  - Implementar método PuedeEditarCampo(nivelUsuario, nombreCampo) para permisos de edición
  - Agregar manejo de errores y logging
  - _Requerimientos: 27.1, 27.2, 27.3, 27.6_
  - _Nota: Servicio central de seguridad para flujo multinivel_

- [ ] 4.2 Implementar logger de seguridad
  - Crear SecurityLogger.vb en JelaWeb/Services/Security/
  - Implementar método LogUnauthorizedAccess(usuarioId, documentoId, accion)
  - Implementar método LogSuccessfulAccess(usuarioId, documentoId, accion)
  - Agregar integración con Application Insights para telemetría
  - Implementar sistema de alertas para múltiples intentos no autorizados (3+ en 5 min)
  - Crear SecurityLogRepository.vb para acceso a datos de logs
  - _Requerimientos: 27.5_
  - _Nota: Auditoría completa de accesos para trazabilidad_

- [ ] 4.3 Actualizar DTOs con campos de seguridad multinivel
  - Actualizar UsuarioDTO.vb agregando propiedad NivelJerarquico As Integer
  - Actualizar DocumentoDTO.vb agregando campos de asignación por nivel
  - Actualizar DocumentoDetalleDTO.vb agregando campos de montos por nivel:
    - MontoEntidad As Decimal?
    - MontoSubEntidad As Decimal?
    - MontoProveedor As Decimal?
    - MontoReal As Decimal?
    - ObservacionesSubEntidad As String
    - ObservacionesProveedor As String
    - ObservacionesColaborador As String
  - _Requerimientos: 27.1, 27.2, 27.3_

- [ ] 4.4 Actualizar SessionHelper con nivel jerárquico
  - Modificar SessionHelper.vb para incluir NivelJerarquico en sesión
  - Actualizar método GetCurrentUser() para cargar nivel desde base de datos
  - Agregar validación de nivel jerárquico en cada request
  - _Requerimientos: 27.1, 27.2, 27.3, 27.4_

- [ ] 4.5 Integrar seguridad en módulo de Captura de Documentos
  - Actualizar CapturaDocumentos.aspx.vb con validación de acceso
  - Implementar método CargarDocumento(documentoId) con validación de seguridad
  - Implementar método CargarPartidas(documentoId) usando stored procedure seguro
  - Implementar método ConfigurarGridPartidas() para ocultar columnas según nivel
  - Agregar validación en gridPartidas_RowUpdating para permisos de edición
  - Implementar redirección a Error403.aspx para accesos denegados
  - _Requerimientos: 27.1, 27.2, 27.3, 27.4, 27.5, 27.6_
  - _Nota: Integración con módulo existente de Captura de Documentos_

- [ ] 4.6 Actualizar DocumentoService con métodos de seguridad
  - Crear método GetPartidasPorNivel(documentoId, usuarioId, nivelUsuario, ipAddress)
  - Implementar llamada a stored procedure sp_obtener_partidas_por_nivel
  - Agregar manejo de excepciones de acceso denegado
  - Implementar método GetDocumentosPorNivel(usuarioId, nivelUsuario, filtros)
  - _Requerimientos: 27.1, 27.2, 27.3, 27.4_

- [ ] 4.7 Crear páginas de error personalizadas para seguridad
  - Actualizar Error403.aspx con mensaje específico de acceso denegado
  - Agregar parámetro de mensaje personalizado
  - Implementar botón de volver al dashboard
  - Agregar registro del error en logs
  - _Requerimientos: 27.5_

- [ ]* 4.8 Escribir property test para visibilidad de montos por nivel
  - **Property 33: Visibilidad de montos por nivel**
  - **Valida: Requerimientos 27.1, 27.2, 27.3**
  - Generar usuarios aleatorios de diferentes niveles
  - Generar documentos aleatorios con partidas
  - Verificar que cada nivel ve solo los montos permitidos
  - Ejecutar 100+ iteraciones

- [ ]* 4.9 Escribir property test para acceso restringido a documentos
  - **Property 34: Acceso restringido a documentos asignados**
  - **Valida: Requerimientos 27.4**
  - Generar usuarios y documentos aleatorios
  - Verificar que solo se accede a documentos asignados al nivel
  - Ejecutar 100+ iteraciones

- [ ]* 4.10 Escribir property test para denegación de acceso
  - **Property 35: Denegación de acceso a documentos no asignados**
  - **Valida: Requerimientos 27.5**
  - Generar intentos de acceso no autorizado
  - Verificar que se deniega el acceso
  - Verificar que se registra en log de seguridad
  - Ejecutar 100+ iteraciones

- [ ]* 4.11 Escribir property test para ocultamiento de columnas
  - **Property 36: Ocultamiento de columnas por nivel**
  - **Valida: Requerimientos 27.6**
  - Generar usuarios de diferentes niveles
  - Verificar que GetColumnasVisibles retorna columnas correctas
  - Ejecutar 100+ iteraciones

- [ ]* 4.12 Escribir property test para administrador ve todo
  - **Property 37: Administrador ve toda la cadena**
  - **Valida: Requerimientos 27.7**
  - Generar administradores de nivel 1
  - Verificar que ven todas las columnas y todos los montos
  - Ejecutar 100+ iteraciones

- [ ]* 4.13 Escribir unit tests para seguridad multinivel
  - Test: SubEntidad solo ve su información
  - Test: Proveedor no ve monto_entidad
  - Test: Colaborador no ve montos anteriores
  - Test: Administrador ve todo
  - Test: Intento de acceso no autorizado genera log
  - Test: Múltiples intentos generan alerta
  - _Requerimientos: 27.1, 27.2, 27.3, 27.4, 27.5, 27.7_

- [ ] 4.14 Checkpoint - Verificar seguridad multinivel
  - Asegurar que todos los tests pasan
  - Probar con usuarios de diferentes niveles
  - Verificar logs de seguridad en base de datos
  - Verificar que columnas se ocultan correctamente en UI
  - Preguntar al usuario si surgen dudas

## Fase 2: Portal Web - Catálogos Base

- [x] 5. Desarrollar catálogo de Roles ✅ **COMPLETADO POR CURSOR**
  - ✅ Crear Roles.aspx con grid de roles
  - ✅ Implementar RolService.vb con lógica de negocio
  - ✅ Agregar CRUD completo de roles
  - ✅ Implementar asignación de permisos a roles
  - ✅ Crear vista de permisos por módulo
  - _Requerimientos: 4.2_
  - _Archivos: JelaWeb/Views/Catalogos/Roles.aspx.vb, JelaWeb/Services/RolService.vb_

- [x] 6. Desarrollar catálogo de Categorías de Tickets ✅ **COMPLETADO POR CURSOR**
  - ✅ Crear CategoriasTicket.aspx con gestión de categorías
  - ✅ Implementar CategoriaTicketService.vb
  - ✅ Agregar CRUD de categorías
  - ✅ Implementar jerarquía de categorías (padre-hijo)
  - ✅ Agregar configuración de SLA por categoría
  - _Requerimientos: 5.1_
  - _Archivos: JelaWeb/Views/Catalogos/CategoriasTicket.aspx.vb, JelaWeb/Services/CategoriaTicketService.vb_

- [x] 7. Desarrollar catálogo de Proveedores ✅ **COMPLETADO POR CURSOR**
  - ✅ Crear Proveedores.aspx con lista de proveedores
  - ✅ Implementar ProveedorService.vb
  - ✅ Agregar CRUD completo de proveedores
  - ✅ Implementar datos fiscales y contactos
  - ✅ Agregar evaluación y calificación de proveedores
  - _Requerimientos: 6.1_
  - _Archivos: JelaWeb/Views/Catalogos/Proveedores.aspx.vb, JelaWeb/Services/ProveedorService.vb_

- [x] 8. Desarrollar catálogo de Conceptos de Facturación ✅ **COMPLETADO POR CURSOR**
  - ✅ Mejorar Conceptos.aspx existente
  - ✅ Agregar categorización de conceptos
  - ✅ Implementar configuración de impuestos por concepto
  - ✅ Agregar conceptos recurrentes vs únicos
  - ✅ Crear plantillas de conceptos
  - _Requerimientos: 8.1_
  - _Archivos: JelaWeb/Views/Catalogos/Conceptos.aspx.vb_

- [x] 9. Desarrollar catálogo de Tipos de Sensor IoT ✅ **COMPLETADO POR CURSOR**
  - ✅ Crear TiposSensor.aspx con tipos de sensores
  - ✅ Implementar TipoSensorService.vb
  - ✅ Agregar CRUD de tipos de sensor
  - ✅ Implementar configuración de unidades de medida
  - ✅ Agregar configuración de umbrales por defecto
  - _Requerimientos: 9.1, 9.3_
  - _Archivos: JelaWeb/Views/Catalogos/TiposSensor.aspx.vb, JelaWeb/Services/TipoSensorService.vb_

- [x] 10. Desarrollar catálogo de Parcelas Agrícolas ✅ **COMPLETADO POR CURSOR**
  - ✅ Crear Parcelas.aspx con gestión de parcelas
  - ✅ Implementar ParcelaService.vb
  - ✅ Agregar CRUD de parcelas
  - ✅ Implementar ubicación geográfica (coordenadas)
  - ⚠️ Agregar asociación de sensores a parcelas (PENDIENTE)
  - ⚠️ Crear visualización en mapa (PENDIENTE)
  - _Requerimientos: 9.1, 10.1_
  - _Archivos: JelaWeb/Views/Catalogos/Parcelas.aspx.vb, JelaWeb/Services/ParcelaService.vb_

- [x] 11. Desarrollar catálogo de Productos Fitosanitarios ✅ **COMPLETADO POR CURSOR**
  - ✅ Crear Fitosanitarios.aspx con lista de productos
  - ✅ Implementar FitosanitarioService.vb
  - ✅ Agregar CRUD de productos fitosanitarios
  - ✅ Implementar datos de seguridad y dosificación
  - ✅ Agregar control de inventario
  - _Requerimientos: 9.5_
  - _Archivos: JelaWeb/Views/Catalogos/Fitosanitarios.aspx.vb, JelaWeb/Services/FitosanitarioService.vb_

- [x] 12. Desarrollar catálogo de Unidades/Departamentos ✅ **COMPLETADO POR CURSOR**
  - ✅ Crear Unidades.aspx para condominios
  - ✅ Implementar UnidadService.vb
  - ✅ Agregar CRUD de unidades
  - ✅ Implementar jerarquía (torre/edificio/unidad)
  - ⚠️ Agregar asociación de residentes a unidades (PENDIENTE)
  - _Requerimientos: 24.1_
  - _Archivos: JelaWeb/Views/Catalogos/Unidades.aspx.vb, JelaWeb/Services/UnidadService.vb_

- [ ]* 12.1 Escribir unit tests para catálogos
  - Test para CRUD de roles
  - Test para CRUD de categorías
  - Test para CRUD de proveedores
  - Test para CRUD de conceptos
  - _Requerimientos: 4.2, 5.1, 6.1, 8.1_

- [ ] 13. Checkpoint - Verificar catálogos base
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 3: Portal Web - Dashboard y Módulos Core

- [ ] 14. Desarrollar Dashboard personalizado
  - Crear Inicio.aspx con layout de widgets responsivo
  - Implementar DashboardService.vb para obtener datos según rol
  - Crear widgets con DevExpress (ASPxCardView, ASPxChart)
  - Implementar actualización automática sin recarga (AJAX)
  - Agregar widgets específicos por rol (admin, residente, técnico)
  - _Requerimientos: 2.1, 2.2, 2.3, 2.4, 2.5_

- [ ]* 14.1 Escribir property test para personalización por rol
  - **Property 9: Dashboard personalizado por rol**
  - **Valida: Requerimientos 2.1**

- [ ]* 14.2 Escribir unit tests para widgets específicos
  - Test para dashboard de administrador
  - Test para dashboard de residente
  - Test para dashboard de técnico
  - _Requerimientos: 2.2, 2.3, 2.4_

- [x] 15. Completar módulo de Entidades ✅ **COMPLETADO POR CURSOR**
  - ✅ Mejorar Entidades.aspx con funcionalidad completa CRUD
  - ✅ Implementar validación de dependencias antes de eliminar
  - ✅ Agregar búsqueda y filtrado avanzado en grid
  - ✅ Implementar paginación eficiente
  - ✅ Agregar gestión de SubEntidades
  - _Requerimientos: 3.1, 3.2, 3.3, 3.4, 3.5_
  - _Archivos: JelaWeb/Views/Catalogos/Entidades.aspx.vb_

- [ ]* 15.1 Escribir property test para persistencia de entidades
  - **Property 11: Entidad válida se persiste**
  - **Valida: Requerimientos 3.1**

- [ ]* 15.2 Escribir property test para edición de entidades
  - **Property 12: Edición de entidad preserva integridad**
  - **Valida: Requerimientos 3.2**

- [ ]* 15.3 Escribir property test para validación de dependencias
  - **Property 13: Eliminación valida dependencias**
  - **Valida: Requerimientos 3.3**

- [ ] 16. Desarrollar módulo de Usuarios y Roles
  - Crear Usuarios.aspx con grid de usuarios
  - Implementar CRUD completo de usuarios
  - Crear página de asignación de roles y permisos
  - Implementar activación/desactivación de usuarios
  - Agregar envío de credenciales por email
  - _Requerimientos: 4.1, 4.2, 4.3_

- [ ]* 16.1 Escribir unit tests para gestión de usuarios
  - Test para creación de usuario con email único
  - Test para asignación de roles
  - Test para desactivación de usuario
  - _Requerimientos: 4.1, 4.2, 4.3_

- [ ] 17. Checkpoint - Verificar módulos core
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 4: Portal Web - Módulo de Tickets

- [x] 18. Desarrollar módulo completo de Tickets ✅ **COMPLETADO POR CURSOR**
  - ✅ Crear Tickets.aspx con lista de tickets
  - ✅ Crear CrearTicket.aspx con formulario de captura (integrado en popup)
  - ✅ Implementar TicketService.vb con lógica de negocio
  - ✅ Agregar soporte para adjuntos (fotos, documentos)
  - ✅ Implementar sistema de categorías y prioridades
  - ✅ Integración con IA para procesamiento automático de tickets
  - ✅ Sistema de conversación integrado
  - ✅ Resolución automática con IA
  - _Requerimientos: 5.1, 5.2_
  - _Archivos: JelaWeb/Views/Operacion/Tickets/Tickets.aspx.vb, JelaWeb/Services/TicketService.vb, JelaWeb/Business/Operacion/TicketsBusiness.vb_

- [ ]* 9.1 Escribir property test para captura de campos
  - **Property 16: Ticket captura todos los campos requeridos**
  - **Valida: Requerimientos 5.1**

- [ ]* 9.2 Escribir property test para unicidad de folio
  - **Property 17: Folio de ticket es único**
  - **Valida: Requerimientos 5.2**

- [ ] 10. Implementar workflow de tickets
  - Crear página de asignación de técnicos
  - Implementar cambios de estado con validaciones
  - Agregar sistema de comentarios en tickets
  - Implementar notificaciones automáticas (integración N8N)
  - Crear página de detalle de ticket con historial
  - _Requerimientos: 5.3, 5.4_

- [ ]* 10.1 Escribir property test para auditoría de cambios
  - **Property 18: Cambio de estado registra auditoría**
  - **Valida: Requerimientos 5.3**

- [ ]* 10.2 Escribir property test para filtros
  - **Property 19: Filtros de tickets funcionan correctamente**
  - **Valida: Requerimientos 5.4**

- [ ] 11. Implementar cierre y calificación de tickets
  - Agregar funcionalidad de cierre de ticket
  - Crear formulario de calificación de servicio
  - Implementar envío de solicitud de calificación
  - Agregar reportes de satisfacción
  - _Requerimientos: 5.5_

- [ ]* 11.1 Escribir property test para solicitud de calificación
  - **Property 20: Ticket cerrado solicita calificación**
  - **Valida: Requerimientos 5.5**

- [ ] 12. Checkpoint - Verificar módulo de tickets
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 4: Portal Web - Órdenes de Compra y Dictámenes

- [ ] 13. Desarrollar módulo de Órdenes de Compra
  - Crear OrdenesCompra.aspx con lista de órdenes
  - Crear CrearOrdenCompra.aspx con formulario detallado
  - Implementar OrdenCompraService.vb con lógica de negocio
  - Agregar captura de conceptos y montos
  - Implementar adjuntos de documentos
  - _Requerimientos: 6.1_

- [ ] 14. Integrar validación con IA
  - Implementar AIValidationService.vb
  - Integrar con Azure OpenAI Service (GPT-4)
  - Crear prompts para validación de cumplimiento normativo
  - Implementar detección de inconsistencias
  - Mostrar sugerencias de corrección en UI
  - _Requerimientos: 6.2_

- [ ]* 14.1 Escribir property test para validación IA
  - **Property 21: IA valida cumplimiento normativo**
  - **Valida: Requerimientos 6.2**

- [ ] 15. Implementar workflow de aprobaciones
  - Crear sistema de aprobadores por nivel
  - Implementar notificaciones de aprobación pendiente
  - Agregar página de aprobación/rechazo
  - Implementar trazabilidad completa de aprobaciones
  - _Requerimientos: 6.3, 6.4, 6.5_

- [ ]* 15.1 Escribir property test para historial completo
  - **Property 22: Historial de orden es completo**
  - **Valida: Requerimientos 6.5**

- [ ] 16. Desarrollar módulo de Dictámenes Técnicos
  - Crear DictamenesTecnicos.aspx con lista
  - Crear formulario de captura de dictamen
  - Implementar validación con IA
  - Agregar generación de PDF oficial
  - Implementar registro de publicación
  - _Requerimientos: 7.1, 7.2, 7.3, 7.4, 7.5_

- [ ]* 16.1 Escribir unit tests para dictámenes
  - Test para creación de dictamen
  - Test para validación IA
  - Test para generación de PDF
  - _Requerimientos: 7.1, 7.2, 7.4_

- [ ] 17. Checkpoint - Verificar órdenes y dictámenes
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas


## Fase 5: Portal Web - Facturación y Tarifas

- [ ] 18. Desarrollar módulo de Tarifas
  - Crear Tarifas.aspx con configuración de tarifas
  - Implementar TarifaService.vb con lógica de negocio
  - Agregar definición de conceptos, montos y periodicidad
  - Implementar reglas de aplicabilidad
  - Crear vista de tarifas activas por entidad
  - _Requerimientos: 8.1_

- [ ] 19. Implementar facturación automática
  - Crear job programado para generación de facturas
  - Implementar FacturacionService.vb
  - Agregar generación automática según tarifas
  - Implementar envío de notificaciones de factura
  - Crear Facturas.aspx para consulta
  - _Requerimientos: 8.2, 8.3_

- [ ] 20. Desarrollar módulo de Pagos
  - Crear Pagos.aspx para registro de pagos
  - Implementar PagoService.vb
  - Agregar integración con pasarelas de pago
  - Implementar actualización de saldos
  - Crear EstadoCuenta.aspx para consulta
  - _Requerimientos: 8.4, 8.5_

- [ ]* 20.1 Escribir unit tests para facturación
  - Test para generación automática de facturas
  - Test para registro de pagos
  - Test para actualización de saldos
  - _Requerimientos: 8.2, 8.4_

- [ ] 21. Checkpoint - Verificar facturación
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 6: Portal Web - Agricultura Inteligente e IoT

- [ ] 22. Desarrollar módulo de Agricultura IoT
  - Crear AgriculturaIoT.aspx con dashboard de monitoreo
  - Implementar IoTService.vb para integración con Azure IoT Hub
  - Agregar mapa interactivo con ubicación de parcelas y sensores
  - Implementar visualización de datos en tiempo real
  - Crear gráficas de históricos con DevExpress
  - _Requerimientos: 9.1, 9.2_

- [ ]* 22.1 Escribir property test para actualización de indicadores
  - **Property 23: Datos de sensor actualizan indicadores**
  - **Valida: Requerimientos 9.2**

- [ ] 23. Implementar sistema de alertas IoT
  - Crear configuración de umbrales por tipo de sensor
  - Implementar AlertaService.vb para generación de alertas
  - Agregar notificaciones automáticas (email, SMS, push)
  - Crear página de gestión de alertas
  - Integrar recomendaciones de IA
  - _Requerimientos: 9.3, 9.4_

- [ ]* 23.1 Escribir property test para generación de alertas
  - **Property 24: Umbrales generan alertas**
  - **Valida: Requerimientos 9.3**

- [ ] 24. Desarrollar módulo de Riego Automatizado
  - Crear RiegoAutomatizado.aspx con programación
  - Implementar RiegoService.vb con lógica de automatización
  - Agregar configuración de horarios y condiciones
  - Implementar envío de comandos a actuadores IoT
  - Crear control manual de riego
  - _Requerimientos: 10.1, 10.2, 10.3_

- [ ]* 24.1 Escribir property test para activación de riego
  - **Property 25: Condiciones de riego activan sistema**
  - **Valida: Requerimientos 10.2**

- [ ]* 24.2 Escribir property test para cancelación por humedad
  - **Property 26: Humedad suficiente cancela riego**
  - **Valida: Requerimientos 10.5**

- [ ] 25. Implementar trazabilidad agrícola
  - Crear módulo de registro de aplicaciones fitosanitarias
  - Implementar historial de actividades por parcela
  - Agregar reportes de trazabilidad
  - Crear exportación de datos para certificaciones
  - _Requerimientos: 9.5, 10.4_

- [ ]* 25.1 Escribir unit tests para trazabilidad
  - Test para registro de aplicación fitosanitaria
  - Test para historial de riego
  - Test para generación de reportes
  - _Requerimientos: 9.5, 10.4_

- [ ] 26. Checkpoint - Verificar módulos IoT
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 7: Portal Web - Comunicación y Reportes

- [ ] 27. Desarrollar módulo de Comunicación con Residentes
  - Crear Avisos.aspx para gestión de comunicados
  - Implementar ComunicacionService.vb
  - Agregar selección de destinatarios (todos, por torre, por unidad)
  - Implementar envío multi-canal (email, SMS, push)
  - Crear registro de lectura de avisos
  - _Requerimientos: 24.1, 24.2, 24.3, 24.4, 24.5_

- [ ]* 27.1 Escribir unit tests para comunicación
  - Test para creación de aviso
  - Test para envío multi-canal
  - Test para registro de lectura
  - _Requerimientos: 24.1, 24.2, 24.3_

- [ ] 28. Desarrollar módulo de Reportes y Analítica
  - Crear Reportes.aspx con plantillas predefinidas
  - Implementar ReporteService.vb
  - Agregar generador de reportes personalizados
  - Implementar exportación a PDF, Excel, CSV
  - Crear visualizaciones con gráficas DevExpress
  - _Requerimientos: 23.1, 23.2, 23.3_

- [ ] 29. Implementar reportes programados
  - Crear configuración de reportes recurrentes
  - Implementar job para generación automática
  - Agregar envío por email programado
  - Crear historial de reportes generados
  - _Requerimientos: 23.4, 23.5_

- [ ]* 29.1 Escribir unit tests para reportes
  - Test para generación de reporte
  - Test para exportación a diferentes formatos
  - Test para programación de reportes
  - _Requerimientos: 23.1, 23.2, 23.4_

- [ ] 30. Checkpoint - Verificar comunicación y reportes
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 8: Integración con Agente de Voz IA

- [ ] 31. Implementar integración con Agente de Voz
  - Crear VoiceAgentService.vb para integración
  - Implementar autenticación por número telefónico
  - Agregar transcripción de llamadas con Azure Speech
  - Integrar con Azure OpenAI para procesamiento de lenguaje natural
  - Implementar detección automática de idioma
  - _Requerimientos: 11.1, 11.2, 11.3, 26.7_

- [ ] 32. Desarrollar acciones del Agente de Voz
  - Implementar creación de tickets por voz
  - Agregar consulta de saldo por voz
  - Implementar consulta de estado de tickets
  - Agregar transferencia a operador humano
  - Crear registro de interacciones de voz
  - _Requerimientos: 11.2, 11.3, 11.4, 11.5_

- [ ]* 32.1 Escribir unit tests para agente de voz
  - Test para autenticación por teléfono
  - Test para creación de ticket por voz
  - Test para consulta de saldo
  - _Requerimientos: 11.1, 11.2, 11.3_

- [ ] 33. Checkpoint - Verificar agente de voz
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas


## Fase 9: Aplicación Móvil - Infraestructura Base

- [ ] 34. Configurar proyecto de aplicación móvil
  - Crear proyecto MAUI o nativo (iOS/Android)
  - Configurar estructura de carpetas (MVVM)
  - Agregar dependencias necesarias (HTTP, SQLite, etc.)
  - Configurar navegación entre páginas
  - Implementar tema y estilos base
  - _Requerimientos: 12.1_

- [ ] 35. Implementar capa de datos local
  - Configurar SQLite para almacenamiento local
  - Crear modelos de base de datos local
  - Implementar LocalDatabaseService
  - Agregar migraciones de esquema
  - Crear repositorios locales
  - _Requerimientos: 18.1, 18.2_

- [ ] 36. Desarrollar capa de servicios
  - Implementar ApiService para llamadas HTTP
  - Crear AuthService con gestión de tokens
  - Implementar SecureStorageService para datos sensibles
  - Agregar manejo de errores de red
  - Crear interceptores HTTP para autenticación
  - _Requerimientos: 12.2, 21.4_

- [ ] 37. Implementar sistema de sincronización offline
  - Crear SyncService para sincronización bidireccional
  - Implementar QueueManager para operaciones pendientes
  - Agregar detección de conflictos
  - Implementar estrategias de resolución de conflictos
  - Crear indicadores de estado de sincronización
  - _Requerimientos: 14.5, 18.4_

- [ ]* 37.1 Escribir property test para sincronización
  - **Property 29: Sincronización procesa datos pendientes**
  - **Valida: Requerimientos 18.4**

- [ ] 38. Checkpoint - Verificar infraestructura móvil
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 9.5: Aplicación Móvil - Seguridad Multinivel

- [ ] 38.1 Implementar servicio de seguridad multinivel móvil
  - Crear DocumentSecurityService.cs en JelaApp/Services/Security/
  - Implementar método ValidateAccessAsync(documentId, userId) con validación por nivel
  - Implementar método GetVisibleColumns(nivelUsuario) para columnas visibles
  - Implementar método FilterPartidas(partidas, nivelUsuario) para filtrado de montos
  - Implementar método CanEditField(nivelUsuario, fieldName) para permisos de edición
  - Agregar manejo de errores y logging
  - _Requerimientos: 27.1, 27.2, 27.3, 27.6_
  - _Nota: Mismas reglas de seguridad que portal web_

- [ ] 38.2 Implementar logger de seguridad móvil
  - Crear SecurityLogger.cs en JelaApp/Services/Security/
  - Implementar método LogUnauthorizedAccessAsync(usuarioId, documentoId, accion)
  - Implementar método LogSuccessfulAccessAsync(usuarioId, documentoId, accion)
  - Agregar integración con telemetría (Application Insights o similar)
  - Implementar almacenamiento local de logs cuando no hay conexión
  - Agregar sincronización de logs al recuperar conexión
  - _Requerimientos: 27.5_

- [ ] 38.3 Actualizar modelos de datos móvil con seguridad multinivel
  - Actualizar UserDTO con propiedad NivelJerarquico
  - Actualizar DocumentoDTO con campos de asignación por nivel
  - Actualizar DocumentoDetalleDTO con campos de montos por nivel
  - Actualizar modelos de base de datos local (SQLite) con nuevos campos
  - _Requerimientos: 27.1, 27.2, 27.3_

- [ ] 38.4 Integrar seguridad en pantallas de documentos móvil
  - Actualizar DocumentListPage con filtrado por nivel
  - Actualizar DocumentDetailPage con validación de acceso
  - Implementar ocultamiento dinámico de campos según nivel
  - Agregar indicadores visuales de permisos de edición
  - Implementar navegación a pantalla de error para accesos denegados
  - _Requerimientos: 27.1, 27.2, 27.3, 27.4, 27.6_

- [ ]* 38.5 Escribir unit tests para seguridad móvil
  - Test: ValidateAccessAsync retorna false para documento no asignado
  - Test: FilterPartidas oculta montos correctamente por nivel
  - Test: GetVisibleColumns retorna columnas correctas
  - Test: CanEditField valida permisos correctamente
  - Test: Logs se guardan localmente cuando no hay conexión
  - _Requerimientos: 27.1, 27.2, 27.3, 27.4, 27.5_

- [ ] 38.6 Checkpoint - Verificar seguridad multinivel móvil
  - Asegurar que todos los tests pasan
  - Probar con usuarios de diferentes niveles en dispositivo
  - Verificar que campos se ocultan correctamente en UI móvil
  - Verificar sincronización de logs de seguridad
  - Preguntar al usuario si surgen dudas

## Fase 10: Aplicación Móvil - Autenticación y Onboarding

- [ ] 39. Desarrollar pantalla de Login
  - Crear LoginPage con formulario de credenciales
  - Implementar LoginViewModel con lógica de autenticación
  - Agregar validación de campos
  - Implementar manejo de errores de login
  - Agregar opción de "Recordar usuario"
  - _Requerimientos: 12.2_

- [ ] 40. Implementar autenticación biométrica
  - Crear BiometricService con integración nativa
  - Agregar soporte para Touch ID / Face ID (iOS)
  - Implementar soporte para Android Biometric
  - Crear pantalla de configuración de biometría
  - Agregar fallback a credenciales
  - _Requerimientos: 12.3, 12.4_

- [ ]* 40.1 Escribir property test para autenticación biométrica
  - **Property 2: Autenticación biométrica equivale a credenciales**
  - **Valida: Requerimientos 12.3**

- [ ] 41. Desarrollar onboarding
  - Crear pantallas de tutorial interactivo
  - Implementar navegación del tutorial
  - Agregar opción de saltar tutorial
  - Guardar preferencia de tutorial completado
  - _Requerimientos: 12.1_

- [ ] 42. Implementar recuperación de contraseña
  - Crear pantalla de recuperación
  - Implementar envío de código por email/SMS
  - Agregar validación de código
  - Crear pantalla de nueva contraseña
  - _Requerimientos: 12.5_

- [ ] 43. Checkpoint - Verificar autenticación móvil
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 11: Aplicación Móvil - Dashboard y Notificaciones

- [ ] 44. Desarrollar Dashboard móvil
  - Crear DashboardPage con widgets personalizados
  - Implementar DashboardViewModel con datos por rol
  - Agregar pull-to-refresh
  - Implementar navegación a módulos desde widgets
  - Crear indicadores visuales de estado
  - _Requerimientos: 13.1, 13.4_

- [ ] 45. Implementar notificaciones push
  - Configurar Firebase Cloud Messaging (Android)
  - Configurar Apple Push Notification Service (iOS)
  - Implementar PushNotificationService
  - Agregar registro de dispositivo en backend
  - Crear NotificationHandler para procesamiento
  - _Requerimientos: 13.2_

- [ ] 46. Desarrollar gestión de notificaciones
  - Crear pantalla de lista de notificaciones
  - Implementar navegación desde notificación
  - Agregar marcado de leído/no leído
  - Implementar eliminación de notificaciones
  - Agregar configuración de preferencias de notificaciones
  - _Requerimientos: 13.3, 13.5_

- [ ]* 46.1 Escribir unit tests para notificaciones
  - Test para recepción de notificación
  - Test para navegación desde notificación
  - Test para notificaciones en background
  - _Requerimientos: 13.2, 13.3, 13.5_

- [ ] 47. Checkpoint - Verificar dashboard y notificaciones
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 12: Aplicación Móvil - Gestión de Tickets

- [ ] 48. Desarrollar lista de tickets
  - Crear TicketListPage con lista de tickets
  - Implementar TicketListViewModel con filtros
  - Agregar búsqueda y filtrado
  - Implementar paginación o scroll infinito
  - Crear indicadores visuales de estado y prioridad
  - _Requerimientos: 14.4_

- [ ] 49. Desarrollar creación de tickets
  - Crear CreateTicketPage con formulario
  - Implementar CreateTicketViewModel
  - Agregar captura de fotos con cámara
  - Implementar captura de ubicación GPS
  - Agregar soporte para modo offline
  - _Requerimientos: 14.1, 14.2, 14.5_

- [ ]* 49.1 Escribir property test para captura de datos
  - **Property 27: Ticket móvil captura ubicación y fotos**
  - **Valida: Requerimientos 14.1**

- [ ]* 49.2 Escribir property test para sincronización offline
  - **Property 28: Ticket offline sincroniza correctamente (Round-trip)**
  - **Valida: Requerimientos 14.5**

- [ ] 50. Desarrollar detalle de ticket
  - Crear TicketDetailPage con información completa
  - Implementar TicketDetailViewModel
  - Agregar visualización de fotos adjuntas
  - Mostrar historial de cambios
  - Implementar actualización de estado (para técnicos)
  - _Requerimientos: 14.3_

- [ ] 51. Implementar modo offline para tickets
  - Agregar almacenamiento local de tickets
  - Implementar cola de sincronización
  - Crear indicadores de tickets pendientes de sincronizar
  - Agregar sincronización automática al recuperar conexión
  - Implementar manejo de conflictos
  - _Requerimientos: 14.5, 18.1, 18.2, 18.3, 18.4, 18.5_

- [ ] 52. Checkpoint - Verificar tickets móvil
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas


## Fase 13: Aplicación Móvil - Órdenes de Trabajo (Técnicos)

- [ ] 53. Desarrollar lista de órdenes de trabajo
  - Crear WorkOrderListPage para técnicos
  - Implementar WorkOrderListViewModel
  - Agregar ordenamiento por prioridad y fecha
  - Implementar filtros por estado
  - Crear indicadores visuales de urgencia
  - _Requerimientos: 15.1_

- [ ] 54. Implementar aceptación y gestión de órdenes
  - Crear WorkOrderDetailPage
  - Implementar WorkOrderDetailViewModel
  - Agregar botón de aceptar orden
  - Implementar registro de hora de inicio
  - Agregar actualización de estado en tiempo real
  - _Requerimientos: 15.2_

- [ ] 55. Desarrollar captura de evidencias
  - Agregar captura de fotos de evidencia
  - Implementar captura de firma digital del cliente
  - Crear formulario de registro de materiales usados
  - Agregar escaneo de códigos de barras
  - Implementar compresión de imágenes antes de enviar
  - _Requerimientos: 15.3, 15.4_

- [ ] 56. Implementar navegación GPS
  - Integrar servicio de mapas (Google Maps / Apple Maps)
  - Agregar botón de navegación a ubicación del trabajo
  - Implementar cálculo de distancia y tiempo estimado
  - Agregar visualización de ubicación en mapa
  - _Requerimientos: 15.5_

- [ ]* 56.1 Escribir unit tests para órdenes de trabajo
  - Test para aceptación de orden
  - Test para captura de evidencias
  - Test para registro de materiales
  - _Requerimientos: 15.2, 15.3, 15.4_

- [ ] 57. Checkpoint - Verificar órdenes de trabajo
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 14: Aplicación Móvil - Consulta de Saldo y Pagos (Residentes)

- [ ] 58. Desarrollar consulta de saldo
  - Crear BalancePage con estado de cuenta
  - Implementar BalanceViewModel
  - Mostrar facturas pendientes y pagos recientes
  - Agregar gráfica de historial de pagos
  - Implementar pull-to-refresh para actualizar
  - _Requerimientos: 16.1_

- [ ] 59. Implementar módulo de pagos
  - Crear PaymentPage con opciones de pago
  - Implementar PaymentViewModel
  - Integrar pasarelas de pago (Stripe, PayPal, etc.)
  - Agregar selección de método de pago
  - Implementar procesamiento seguro de pagos
  - _Requerimientos: 16.2, 16.3_

- [ ] 60. Desarrollar comprobantes digitales
  - Crear pantalla de comprobante de pago
  - Implementar generación de PDF con código QR
  - Agregar opción de compartir comprobante
  - Implementar descarga de comprobante
  - Crear notificación de confirmación de pago
  - _Requerimientos: 16.4, 16.5_

- [ ]* 60.1 Escribir unit tests para pagos
  - Test para consulta de saldo
  - Test para procesamiento de pago
  - Test para generación de comprobante
  - _Requerimientos: 16.1, 16.3, 16.5_

- [ ] 61. Checkpoint - Verificar saldo y pagos
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 15: Aplicación Móvil - Monitoreo Agrícola IoT

- [ ] 62. Desarrollar dashboard agrícola móvil
  - Crear AgricultureDashboardPage
  - Implementar AgricultureDashboardViewModel
  - Agregar mapa con ubicación de parcelas
  - Mostrar sensores en mapa con iconos por tipo
  - Implementar visualización de datos en tiempo real
  - _Requerimientos: 17.1_

- [ ] 63. Desarrollar detalle de sensor
  - Crear SensorDetailPage
  - Implementar SensorDetailViewModel
  - Agregar gráficas de datos históricos
  - Mostrar valor actual con indicador visual
  - Implementar actualización automática de datos
  - _Requerimientos: 17.2_

- [ ] 64. Implementar alertas IoT móvil
  - Crear pantalla de lista de alertas
  - Implementar notificaciones push para alertas
  - Agregar recomendaciones de IA en alertas
  - Mostrar acciones sugeridas
  - Implementar marcado de alerta como atendida
  - _Requerimientos: 17.3_

- [ ] 65. Desarrollar control de riego móvil
  - Crear RiegoControlPage
  - Implementar RiegoControlViewModel
  - Agregar botón de activación manual de riego
  - Mostrar estado actual del riego (activo/inactivo)
  - Implementar temporizador de riego
  - _Requerimientos: 17.4_

- [ ] 66. Implementar registro de observaciones en campo
  - Crear FieldObservationPage
  - Agregar captura de fotos de cultivos
  - Implementar captura automática de ubicación GPS
  - Agregar notas de texto
  - Implementar sincronización de observaciones
  - _Requerimientos: 17.5_

- [ ]* 66.1 Escribir unit tests para IoT móvil
  - Test para visualización de datos de sensor
  - Test para activación manual de riego
  - Test para registro de observación
  - _Requerimientos: 17.2, 17.4, 17.5_

- [ ] 67. Checkpoint - Verificar IoT móvil
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 16: Aplicación Móvil - Chat con Soporte

- [ ] 68. Desarrollar chat en tiempo real
  - Crear ChatPage con interfaz de mensajería
  - Implementar ChatViewModel
  - Integrar WebSockets para comunicación en tiempo real
  - Agregar envío y recepción de mensajes
  - Implementar indicador de "escribiendo"
  - _Requerimientos: 25.1, 25.2, 25.5_

- [ ] 69. Implementar funcionalidades de chat
  - Agregar historial de conversaciones
  - Implementar notificaciones de nuevos mensajes
  - Agregar envío de imágenes en chat
  - Implementar marcado de mensajes como leídos
  - Crear opción de conectar con Agente de Voz IA
  - _Requerimientos: 25.1, 25.3, 25.4_

- [ ]* 69.1 Escribir unit tests para chat
  - Test para envío de mensaje
  - Test para recepción de mensaje
  - Test para notificación de nuevo mensaje
  - _Requerimientos: 25.2, 25.3_

- [ ] 70. Checkpoint - Verificar chat móvil
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 17: Aplicación Móvil - Internacionalización

- [ ] 71. Implementar soporte multi-idioma móvil
  - Crear archivos de traducción JSON (es-MX, en-US)
  - Implementar LocalizationService para móvil
  - Agregar detección automática de idioma del dispositivo
  - Crear selector de idioma en configuración
  - Implementar actualización de UI al cambiar idioma
  - _Requerimientos: 26.1, 26.2, 26.5_

- [ ] 72. Traducir todas las pantallas
  - Traducir textos de autenticación y onboarding
  - Traducir dashboard y notificaciones
  - Traducir módulo de tickets
  - Traducir módulo de órdenes de trabajo
  - Traducir módulo de pagos y saldo
  - Traducir módulo agrícola IoT
  - Traducir chat y configuración
  - _Requerimientos: 26.3, 26.4_

- [ ]* 72.1 Escribir property test para persistencia de idioma
  - **Property 32: Preferencia de idioma persiste**
  - **Valida: Requerimientos 26.5**

- [ ] 73. Checkpoint - Verificar internacionalización móvil
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas


## Fase 18: Integración con N8N y Optimizaciones

- [ ] 74. Integrar flujos N8N existentes
  - Documentar flujos N8N ya creados
  - Implementar N8NService.vb para invocación de flujos
  - Integrar flujo de notificaciones de tickets
  - Integrar flujo de notificación a proveedores
  - Integrar flujo de procesamiento de alertas IoT
  - _Requerimientos: 22.1, 22.2, 22.3_

- [ ] 75. Implementar manejo de errores de N8N
  - Agregar reintentos automáticos para flujos fallidos
  - Implementar logging de ejecuciones de flujos
  - Crear alertas para administradores en caso de fallas
  - Agregar consulta de estado de flujos
  - _Requerimientos: 22.4, 22.5_

- [ ]* 75.1 Escribir unit tests para integración N8N
  - Test para invocación de flujo
  - Test para manejo de errores
  - Test para reintentos
  - _Requerimientos: 22.1, 22.4_

- [ ] 76. Optimizar rendimiento del portal web
  - Implementar caché de datos frecuentes
  - Optimizar consultas a la API
  - Agregar lazy loading de imágenes
  - Implementar paginación eficiente en grids
  - Optimizar carga de scripts y CSS
  - _Requerimientos: 20.1, 20.2, 20.4_

- [ ] 77. Optimizar rendimiento de app móvil
  - Implementar caché de imágenes
  - Optimizar consultas a base de datos local
  - Agregar compresión de imágenes antes de subir
  - Implementar lazy loading de listas
  - Optimizar sincronización en background
  - _Requerimientos: 20.3, 20.4_

- [ ] 78. Checkpoint - Verificar integraciones y optimizaciones
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 19: Accesibilidad y Seguridad

- [ ] 79. Implementar accesibilidad en portal web
  - Agregar etiquetas ARIA a todos los componentes
  - Implementar navegación completa por teclado
  - Agregar alternativas textuales para contenido visual
  - Implementar indicadores no solo por color
  - Mejorar mensajes de error para lectores de pantalla
  - _Requerimientos: 19.1, 19.2, 19.3, 19.4, 19.5_

- [ ] 80. Implementar accesibilidad en app móvil
  - Configurar VoiceOver (iOS) y TalkBack (Android)
  - Agregar etiquetas de accesibilidad a controles
  - Implementar tamaños de fuente escalables
  - Agregar contraste suficiente en colores
  - Probar con herramientas de accesibilidad nativas
  - _Requerimientos: 19.1, 19.2, 19.3, 19.4_

- [ ] 81. Reforzar seguridad del portal web
  - Implementar headers de seguridad adicionales
  - Agregar protección CSRF
  - Implementar validación de entrada en servidor
  - Agregar rate limiting en endpoints críticos
  - Implementar auditoría de acciones sensibles
  - _Requerimientos: 21.1, 21.5_

- [ ] 82. Reforzar seguridad de app móvil
  - Implementar certificate pinning
  - Agregar ofuscación de código
  - Implementar detección de jailbreak/root
  - Agregar cifrado de base de datos local
  - Implementar limpieza de datos al desinstalar
  - _Requerimientos: 21.1, 21.4, 21.5_

- [ ]* 82.1 Escribir unit tests de seguridad
  - Test para validación de entrada
  - Test para rate limiting
  - Test para cifrado de datos sensibles
  - _Requerimientos: 21.1, 21.3_

- [ ] 83. Checkpoint - Verificar accesibilidad y seguridad
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 20: Testing Integral y Documentación

- [ ] 84. Completar suite de property tests
  - Revisar que todas las propiedades tienen tests
  - Configurar ejecución de 100+ iteraciones
  - Agregar generadores inteligentes faltantes
  - Ejecutar suite completa y corregir fallos
  - Documentar casos edge encontrados
  - _Todas las propiedades 1-32_

- [ ] 85. Completar suite de unit tests
  - Revisar cobertura de código
  - Agregar tests faltantes para alcanzar objetivos
  - Ejecutar suite completa y corregir fallos
  - Refactorizar tests duplicados
  - _Todos los módulos_

- [ ]* 86. Ejecutar tests de integración
  - Test de flujo completo de ticket (creación a cierre)
  - Test de flujo de orden de compra (creación a aprobación)
  - Test de sincronización offline completa
  - Test de integración con N8N
  - Test de integración con Azure OpenAI

- [ ]* 87. Ejecutar tests de UI
  - Test de navegación en portal web
  - Test de formularios en portal web
  - Test de navegación en app móvil
  - Test de gestos en app móvil

- [ ] 88. Generar documentación técnica
  - Documentar arquitectura del sistema
  - Crear guías de instalación y configuración
  - Documentar APIs y servicios
  - Crear diagramas de flujo de procesos
  - Documentar decisiones de diseño

- [ ] 89. Generar documentación de usuario
  - Crear manual de usuario del portal web
  - Crear manual de usuario de app móvil
  - Documentar casos de uso comunes
  - Crear videos tutoriales
  - Documentar preguntas frecuentes (FAQ)

- [ ] 90. Checkpoint Final - Verificación completa
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

## Fase 21: Preparación para Producción

- [ ] 91. Configurar ambientes de producción
  - Configurar Azure App Service para portal web
  - Configurar base de datos de producción
  - Configurar CDN para assets estáticos
  - Configurar certificados SSL
  - Configurar monitoreo y alertas

- [ ] 92. Preparar apps móviles para publicación
  - Configurar perfiles de provisioning (iOS)
  - Generar APK/AAB firmado (Android)
  - Crear screenshots y descripciones para stores
  - Configurar App Store Connect (iOS)
  - Configurar Google Play Console (Android)

- [ ] 93. Realizar pruebas de carga y rendimiento
  - Ejecutar pruebas de carga en portal web
  - Medir tiempos de respuesta bajo carga
  - Identificar y optimizar cuellos de botella
  - Verificar escalabilidad del sistema
  - Documentar resultados

- [ ] 94. Realizar auditoría de seguridad
  - Ejecutar escaneo de vulnerabilidades
  - Revisar configuraciones de seguridad
  - Verificar cumplimiento de mejores prácticas
  - Corregir vulnerabilidades encontradas
  - Documentar hallazgos y correcciones

- [ ] 95. Preparar plan de despliegue
  - Documentar pasos de despliegue
  - Crear scripts de migración de datos
  - Preparar plan de rollback
  - Definir ventana de mantenimiento
  - Comunicar plan a stakeholders

- [ ] 96. Checkpoint Final de Producción
  - Asegurar que todos los tests pasan, preguntar al usuario si surgen dudas

---

## Notas Importantes

### Sobre Tests Opcionales (marcados con *)

Las tareas marcadas con `*` son opcionales y se enfocan en testing (property tests, unit tests, integration tests). Estas tareas:
- Proporcionan garantías adicionales de corrección
- Ayudan a detectar bugs tempranamente
- Facilitan el mantenimiento futuro
- Pueden omitirse para acelerar el MVP

### Sobre Checkpoints

Los checkpoints son puntos de verificación donde se debe:
- Ejecutar todos los tests implementados hasta ese punto
- Verificar que no hay errores de compilación
- Consultar al usuario si hay dudas o problemas
- Asegurar que la funcionalidad implementada funciona correctamente

### Sobre Property-Based Tests

Cada property test debe:
- Ejecutar mínimo 100 iteraciones
- Incluir comentario con formato: `**Feature: ecosistema-jelabbc, Property {número}: {texto}**`
- Usar generadores inteligentes de datos
- Validar una propiedad universal del sistema

### Contexto de Implementación

- **Backend**: Ya está operativo en Azure
- **Frontend actual**: ASP.NET Web Forms con VB.NET, DevExpress, Bootstrap 5
- **Módulos existentes**: Login, Entidades, Conceptos, Captura de Documentos
- **Integraciones**: N8N (algunos flujos ya creados), Azure OpenAI, Azure IoT Hub
- **Base de datos**: MySQL

### Priorización

Las fases están ordenadas por prioridad y dependencias:
1. Fases 1-4: Fundamentos y módulos core del portal web
2. Fases 5-8: Módulos avanzados del portal web
3. Fases 9-17: Aplicación móvil completa
4. Fases 18-21: Integraciones, optimizaciones y producción

Se recomienda completar cada fase antes de pasar a la siguiente para mantener un desarrollo incremental y validado.


## Fase 23: Portal Web - Módulo de Servicios Municipales

- [ ] 97. Crear estructura de base de datos para Servicios Municipales
  - Ejecutar script SQL para agregar campos a op_documentos (tipo_documento, fallo_id, fecha_inicio, fecha_fin, tiempo_estimado_dias, tiempo_real_dias, porcentaje_cumplimiento, folio_pago)
  - Crear tabla op_documentos_secciones con campos de montos multinivel
  - Crear tabla op_alertas_oc con configuración de alertas
  - Crear tabla op_alertas_ejecutadas con historial de alertas enviadas
  - Crear tabla op_chat_documentos para mensajería integrada
  - Agregar índices de rendimiento en todas las tablas
  - Verificar foreign keys y constraints
  - _Requerimientos: 28.1, 29.1, 30.4, 31.2_

- [ ] 98. Desarrollar servicios de negocio para Servicios Municipales
  - Crear ServiciosMunicipalesService.vb con métodos CRUD de fallos
  - Implementar OCMunicipalService.vb con gestión de estados de OC
  - Implementar DictamenService.vb con aprobación/rechazo
  - Implementar AlertaService.vb con evaluación y envío de alertas
  - Implementar ChatService.vb con mensajería en tiempo real
  - Agregar validaciones de negocio en cada servicio
  - Integrar con DocumentoSecurityService para seguridad multinivel
  - _Requerimientos: 28.1, 28.2, 28.3, 28.4, 28.5, 28.6, 28.7, 29.1, 29.2, 29.3, 29.4, 29.5, 29.6, 29.7, 30.4, 31.2, 31.3_

- [ ] 99. Desarrollar DTOs para Servicios Municipales
  - Crear FalloDTO.vb con secciones y conceptos
  - Crear SeccionDTO.vb con montos multinivel
  - Crear OCMunicipalDTO.vb con fechas y KPIs
  - Crear DictamenDTO.vb con estados y adjuntos
  - Crear KPIsDTO.vb con métricas calculadas
  - Crear AlertaOCDTO.vb con configuración de alertas
  - Crear MensajeChatDTO.vb con datos de mensajería
  - _Requerimientos: 28.1, 29.1, 30.1, 30.4, 31.2_

- [ ] 100. Desarrollar página de Gestión de Fallos
  - Crear ServiciosMunicipales.aspx con listado de fallos
  - Crear FalloLicitacion.aspx con captura de fallos
  - Implementar grid de secciones (colonias) con captura dinámica
  - Implementar grid de conceptos con montos por nivel
  - Agregar botón de asignación a SubEntidad
  - Implementar validación de campos obligatorios
  - Integrar con DocumentoSecurityService para visibilidad por nivel
  - _Requerimientos: 28.1, 28.2_

- [ ]* 100.1 Escribir property test para captura de secciones
  - **Property 38: Fallo captura secciones y conceptos**
  - **Valida: Requerimientos 28.1**
  - Generar fallos aleatorios con secciones y conceptos
  - Verificar que se guardan correctamente
  - Ejecutar 100+ iteraciones

- [ ] 101. Desarrollar página de Gestión de Órdenes de Compra
  - Crear OrdenesCompraMunicipal.aspx con listado de OCs
  - Implementar filtros por estado (OC Nueva, En Proceso, Pagada)
  - Agregar indicadores visuales de tiempo transcurrido
  - Implementar botones de cambio de estado
  - Agregar vista de dictámenes vinculados
  - Implementar cálculo automático de porcentaje de cumplimiento
  - _Requerimientos: 29.1, 29.2, 29.5, 29.6, 29.7_

- [ ]* 101.1 Escribir property test para flujo de estados de OC
  - **Property 43, 46, 48: Estados de OC**
  - **Valida: Requerimientos 29.1, 29.5, 29.7**
  - Generar OCs aleatorias
  - Simular transiciones de estado
  - Verificar timestamps y cálculos
  - Ejecutar 100+ iteraciones

- [ ] 102. Desarrollar página de Gestión de Dictámenes
  - Crear DictamenesMunicipales.aspx con listado de dictámenes
  - Implementar formulario de creación de dictamen
  - Agregar botones de aprobación/rechazo
  - Implementar popup de rechazo con campo de comentarios
  - Agregar asignación automática de folio de pago en aprobación
  - Implementar notificaciones a SubEntidad en rechazo
  - _Requerimientos: 29.4, 29.5, 29.6_

- [ ]* 102.1 Escribir property test para dictámenes
  - **Property 45, 46, 47: Dictámenes**
  - **Valida: Requerimientos 29.4, 29.5, 29.6**
  - Generar dictámenes aleatorios
  - Verificar vinculación a OC
  - Verificar aprobación y rechazo
  - Ejecutar 100+ iteraciones

- [ ] 103. Desarrollar módulo de KPIs y Dashboard
  - Crear KPIsServiciosMunicipales.aspx con dashboard de métricas
  - Implementar gráficas con DevExpress (ASPxChart)
  - Agregar filtros por rango de fechas y entidad
  - Implementar cálculo de tiempos promedio por fase
  - Agregar tabla de OCs en riesgo (cerca de vencer)
  - Implementar exportación de KPIs a Excel
  - _Requerimientos: 30.1, 30.2, 30.3, 30.6_

- [ ]* 103.1 Escribir property test para cálculo de KPIs
  - **Property 49, 50, 51: KPIs**
  - **Valida: Requerimientos 30.1, 30.2, 30.3**
  - Generar conjuntos aleatorios de OCs
  - Calcular KPIs manualmente
  - Verificar que el sistema calcula lo mismo
  - Ejecutar 100+ iteraciones

- [ ] 104. Desarrollar sistema de Alertas configurables
  - Crear AlertasOC.aspx con configuración de alertas
  - Implementar formulario de configuración (mensaje, días, teléfonos)
  - Crear job programado para evaluación de alertas (cada hora)
  - Implementar integración con WhatsAppService (Twilio)
  - Agregar registro de alertas ejecutadas
  - Implementar vista de historial de alertas enviadas
  - _Requerimientos: 30.4, 30.5, 29.3_

- [ ]* 104.1 Escribir property test para alertas
  - **Property 52, 53: Alertas**
  - **Valida: Requerimientos 30.4, 30.5**
  - Generar OCs con alertas configuradas
  - Simular paso del tiempo
  - Verificar envío de alertas
  - Ejecutar 100+ iteraciones

- [ ] 105. Desarrollar Chat integrado en documentos
  - Crear ChatDocumento.ascx como User Control
  - Implementar área de mensajes con scroll automático
  - Agregar campo de entrada de texto con botón de envío
  - Implementar actualización en tiempo real con SignalR o polling
  - Agregar indicador de "escribiendo..."
  - Implementar edición de mensajes propios
  - Agregar notificaciones push a partes involucradas
  - _Requerimientos: 31.1, 31.2, 31.3, 31.4, 31.5_

- [ ]* 105.1 Escribir property test para chat
  - **Property 55, 56, 57, 58: Chat**
  - **Valida: Requerimientos 31.2, 31.3, 31.4, 31.5**
  - Generar mensajes aleatorios
  - Verificar guardado y notificaciones
  - Verificar edición y historial
  - Ejecutar 100+ iteraciones

- [ ] 106. Integrar Servicios Municipales con seguridad multinivel
  - Actualizar ServiciosMunicipalesService para usar DocumentoSecurityService
  - Implementar validación de acceso en cada operación
  - Configurar visibilidad de columnas según nivel en todos los grids
  - Agregar validación de permisos de edición por nivel
  - Implementar logging de accesos en SecurityLogger
  - Agregar redirección a Error403 para accesos denegados
  - _Requerimientos: 27.1, 27.2, 27.3, 27.4, 27.5, 27.6, 27.7, 28.3, 28.5, 28.7_

- [ ]* 106.1 Escribir property tests para seguridad multinivel en Servicios Municipales
  - **Property 40, 41, 42: Visibilidad por nivel**
  - **Valida: Requerimientos 28.3, 28.5, 28.7**
  - Generar usuarios de diferentes niveles
  - Generar documentos con montos
  - Verificar visibilidad correcta
  - Ejecutar 100+ iteraciones

- [ ]* 106.2 Escribir unit tests para Servicios Municipales
  - Test: SubEntidad no ve monto_proveedor
  - Test: Proveedor no ve monto_entidad
  - Test: Colaborador no ve montos anteriores
  - Test: Administrador ve todo
  - Test: Creación de OC con fechas correctas
  - Test: Cambio de estado válido
  - Test: Cambio de estado inválido (debe fallar)
  - Test: Aprobación de dictamen asigna folio
  - Test: Rechazo devuelve con comentarios
  - Test: Alerta se envía en momento correcto
  - Test: Chat guarda mensaje con timestamp
  - _Requerimientos: 28.3, 28.5, 28.7, 29.1, 29.5, 29.6, 30.5, 31.2_

- [ ] 107. Checkpoint - Verificar módulo de Servicios Municipales
  - Asegurar que todos los tests pasan
  - Probar flujo completo: Fallo → OC → Dictamen → Pago
  - Verificar seguridad multinivel en todos los niveles
  - Probar alertas con diferentes configuraciones
  - Verificar chat en tiempo real
  - Verificar KPIs con datos reales
  - Preguntar al usuario si surgen dudas

## Fase 25: Portal Web - Módulo de Formularios Dinámicos (07.5)

- [ ] 112. Crear estructura de base de datos para Formularios Dinámicos
  - Ejecutar script SQL `formularios-dinamicos.sql` para crear tablas
  - Verificar tabla cat_formularios (catálogo maestro)
  - Verificar tabla cat_campos_formulario con metadatos de layout (seccion, ancho_columna, posicion_orden)
  - Verificar tabla cat_opciones_campo para dropdowns/radios
  - Verificar tabla cat_plantilla_pdf para templates HTML de generación de PDF
  - Verificar tabla op_fallo_formulario para asignación a fallos
  - Verificar tabla op_respuesta_formulario para capturas
  - Verificar tabla op_respuesta_campo para valores de campos
  - Verificar tabla op_documento_formulario_pdf para PDFs generados
  - Verificar índices de rendimiento y foreign keys
  - _Requerimientos: 34.1, 34.2, 34.5, 34.6_

- [ ] 113. Implementar servicio de Azure Document Intelligence
  - Crear DocumentIntelligenceService.vb en JelaWeb/Services/
  - Implementar ExtraerCamposDePDF(archivo As Stream) que NO almacena el PDF
  - Implementar InferirTipoCampo(valor) para detectar tipo automáticamente
  - Implementar LimpiarNombreCampo(nombre) para generar nombres válidos
  - Configurar credenciales Azure en Web.config (AzureDocIntelEndpoint, AzureDocIntelKey)
  - Agregar manejo de errores y logging
  - _Requerimientos: 34.1_

- [ ] 114. Implementar servicios de Formularios Dinámicos
  - Crear FormularioService.vb en JelaWeb/Services/
  - Implementar GetFormulariosActivos(plataforma) con filtro por plataforma
  - Implementar GetFormularioById(formularioId) con campos y opciones
  - Implementar GetCamposFormulario(formularioId) ordenados por seccion y posicion_orden
  - Implementar CrearFormularioDesdeExtraccion(nombre, campos) para crear desde PDF
  - Implementar CreateFormulario(formulario) con validación de nombre único
  - Implementar UpdateFormulario(formulario) con versionado
  - Implementar DeleteFormulario(formularioId) con validación de dependencias
  - _Requerimientos: 34.1, 34.2, 34.3, 34.4_

- [ ] 115. Implementar servicio de renderizado dinámico Web
  - Crear FormularioRenderService.vb en JelaWeb/Services/
  - Implementar RenderizarFormulario(container, formularioId) con DevExpress ASPxFormLayout
  - Implementar CrearASPxTextBox, CrearASPxSpinEdit, CrearASPxDateEdit, etc.
  - Implementar CrearASPxComboBox con opciones de cat_opciones_campo
  - Implementar CrearASPxUploadControl para fotos
  - Implementar control de firma digital
  - Manejar secciones como LayoutGroups respetando posicion_orden
  - _Requerimientos: 34.10_

- [ ] 116. Implementar servicios de Respuestas de Formulario
  - Crear RespuestaFormularioService.vb
  - Implementar IniciarRespuesta(falloFormularioId, usuarioId, tipoDispositivo)
  - Implementar GuardarRespuestaCampo(respuestaId, campoId, valor)
  - Implementar GuardarFoto(respuestaId, campoId, fotoBytes) con subida a Azure Blob (formularios-fotos)
  - Implementar GuardarFirma(respuestaId, campoId, firmaBytes) con subida a Azure Blob (formularios-firmas)
  - Implementar CompletarRespuesta(respuestaId) con cálculo de porcentaje
  - _Requerimientos: 34.14_

- [ ] 117. Implementar servicio de generación de PDF
  - Crear PdfGeneratorService.vb usando SelectPdf o iTextSharp
  - Implementar GenerarPdfFormulario(respuestaId) que usa plantilla de cat_plantilla_pdf
  - Implementar reemplazo de placeholders {{campo}} con valores de op_respuesta_campo
  - Implementar inserción de imágenes de fotos y firma
  - Implementar subida automática a Azure Blob Storage (formularios-pdf)
  - Registrar PDF en op_documento_formulario_pdf
  - _Requerimientos: 34.15_

- [ ] 118. Implementar servicio de asignación Fallo-Formulario
  - Crear FalloFormularioService.vb
  - Implementar AsignarFormularioAFallo(falloId, formularioId, usuarioAsignado)
  - Implementar GetFormulariosAsignados(falloId)
  - Implementar GetFallosConFormulario(formularioId)
  - Agregar validación de permisos por nivel jerárquico
  - _Requerimientos: 34.7, 34.8, 35.1, 35.2_

- [ ] 119. Desarrollar página FormulariosDinamicos.aspx
  - Crear página en JelaWeb/Views/Catalogos/
  - Implementar ASPxGridView con listado de formularios
  - Agregar toolbar con botones: Nuevo Manual, Crear desde PDF, Editar, Eliminar, Vista Previa
  - Implementar ASPxUploadControl para subir PDF de plantilla
  - Implementar filtros por estado y plataforma
  - Agregar columnas: nombre, descripcion, plataformas, estado, version, campos_count
  - Configurar grid según estándares DevExpress (sin paginación, filtros en cabecera)
  - _Requerimientos: 34.1_

- [ ] 120. Desarrollar popup de creación/edición de Formulario
  - Crear ASPxPopupControl para formulario
  - Implementar campos: nombre_formulario, descripcion, plataformas (checkboxes), estado
  - Agregar ASPxPageControl con pestañas: Campos, Opciones, Plantilla PDF
  - Implementar grid de campos con drag & drop para ordenar
  - Agregar botones para agregar/editar/eliminar campos
  - Implementar popup anidado para configuración de campo
  - Implementar editor HTML para plantilla PDF
  - _Requerimientos: 34.1, 34.2, 34.6_

- [ ] 121. Desarrollar configuración de campos del formulario
  - Crear popup para configuración de campo individual
  - Implementar selector de tipo_campo con iconos
  - Agregar campos: nombre_campo, etiqueta_campo, seccion, ancho_columna, es_requerido
  - Implementar configuración de opciones para dropdown/radio
  - Agregar placeholder, ayuda_campo, valor_por_defecto
  - Implementar vista previa del campo
  - _Requerimientos: 34.2, 34.3, 34.5_

- [ ] 122. Desarrollar vista previa de formulario
  - Crear control de usuario FormularioPreview.ascx
  - Implementar renderizado dinámico usando FormularioRenderService
  - Agregar estilos para diferentes plataformas (web, móvil)
  - Implementar validación en tiempo real
  - Agregar botón de prueba de envío
  - _Requerimientos: 34.1, 34.2_

- [ ] 123. Integrar asignación de formularios en módulo de Fallos
  - Modificar CapturaDocumentos.aspx para incluir selector de formulario
  - Agregar ASPxComboBox con formularios activos
  - Implementar asignación al guardar fallo
  - Mostrar formularios asignados en detalle del fallo
  - Agregar opción de cambiar/quitar formulario asignado
  - _Requerimientos: 35.1, 35.2_

- [ ]* 123.1 Escribir property tests para Formularios Dinámicos
  - **Property 43: Extracción de PDF genera campos válidos**
  - **Valida: Requerimientos 34.1**
  - Generar PDFs de prueba con diferentes estructuras
  - Verificar que campos extraídos tienen nombres y tipos válidos
  - Ejecutar 100+ iteraciones

- [ ]* 123.2 Escribir property tests para respuestas de formulario
  - **Property 44: Respuesta completa genera PDF con todos los datos**
  - **Valida: Requerimientos 34.14, 34.15**
  - Generar respuestas completas aleatorias
  - Verificar que PDF contiene todos los valores capturados
  - Ejecutar 100+ iteraciones

- [ ]* 123.3 Escribir unit tests para Formularios Dinámicos
  - Test: Crear formulario desde extracción de PDF
  - Test: Crear formulario manual con campos
  - Test: Agregar opciones a campo dropdown
  - Test: Renderizado dinámico genera controles correctos
  - Test: Asignar formulario a fallo
  - Test: Iniciar respuesta de formulario
  - Test: Guardar respuesta de campo
  - Test: Completar respuesta calcula porcentaje
  - Test: Generar PDF incluye todos los campos y fotos
  - _Requerimientos: 34.1-34.16_

- [ ] 124. Checkpoint - Verificar módulo de Formularios Dinámicos (Portal Web)
  - Asegurar que todos los tests pasan
  - Probar extracción de campos desde PDF con Document Intelligence
  - Probar creación manual de formulario con diferentes tipos de campos
  - Verificar renderizado dinámico en web
  - Verificar asignación de formulario a fallo
  - Probar generación de PDF con plantilla HTML
  - Verificar integración con Azure Blob Storage
  - Preguntar al usuario si surgen dudas

## Fase 26: Aplicación Móvil - Módulo de Formularios Dinámicos (07.5)

- [ ] 125. Implementar modelo de datos SQLite para Formularios
  - Crear LocalFormulario con campos espejo de MySQL (cat_formularios)
  - Crear LocalCampoFormulario con validaciones en JSON (cat_campos_formulario)
  - Crear LocalOpcionCampo para opciones de dropdown/radio (cat_opciones_campo)
  - Crear LocalRespuestaPendiente para respuestas offline
  - Crear LocalFotoPendiente para fotos sin sincronizar
  - Implementar migraciones de base de datos
  - _Requerimientos: 34.11, 34.16_

- [ ] 126. Implementar servicio de sincronización de Formularios
  - Crear FormularioSyncService.cs
  - Implementar SyncFormulariosAsync() para descargar formularios activos
  - Implementar SyncCamposAsync(formularioId) para descargar campos con metadatos de layout
  - Implementar SyncRespuestasPendientesAsync() para subir respuestas
  - Agregar validación de hash para integridad de datos
  - Implementar reintentos automáticos (máximo 3)
  - _Requerimientos: 34.11, 34.14, 34.16_

- [ ] 127. Desarrollar renderizador dinámico de formularios MAUI
  - Crear DynamicFormRenderer.cs
  - Implementar RenderForm(formulario, campos) que genera UI con StackLayout
  - Implementar RenderField(campo) para cada tipo de campo usando controles nativos MAUI:
    - texto: Entry
    - numero: Entry con teclado numérico
    - fecha: DatePicker
    - foto: Button + Image
    - firma: SignaturePad
    - dropdown: Picker
    - checkbox: CheckBox
    - radio: RadioButton group
    - textarea: Editor
  - Respetar seccion y posicion_orden de cat_campos_formulario
  - Agregar validación en tiempo real
  - _Requerimientos: 34.9, 35.3_

- [ ] 128. Implementar captura de fotos en formularios
  - Crear PhotoCaptureService.cs
  - Implementar CapturePhotoAsync() con cámara nativa MAUI
  - Agregar compresión de imagen (máximo 1MB)
  - Guardar foto localmente con referencia al campo
  - Implementar vista previa de foto capturada
  - Agregar opción de retomar foto
  - _Requerimientos: 34.12, 35.4_

- [ ] 129. Implementar captura de firma digital
  - Crear SignatureCaptureService.cs
  - Implementar SignaturePad con canvas de dibujo
  - Guardar firma como PNG
  - Agregar botón de limpiar firma
  - Implementar vista previa de firma
  - _Requerimientos: 34.13, 35.5_

- [ ] 130. Desarrollar página de formulario dinámico móvil
  - Crear DynamicFormPage.xaml con StackLayout principal
  - Implementar carga de formulario desde SQLite o API
  - Renderizar campos dinámicamente respetando secciones y orden
  - Agregar indicador de progreso (campos completados/total)
  - Implementar guardado automático cada 30 segundos en SQLite
  - Agregar botón de enviar con validación completa
  - _Requerimientos: 34.9, 34.11, 35.3, 35.7_

- [ ] 131. Integrar formularios en flujo de Colaborador
  - Modificar DocumentoDetallePage para mostrar formulario asignado desde op_fallo_formulario
  - Agregar botón "Llenar Formulario" si hay formulario asignado
  - Navegar a DynamicFormPage con datos del fallo
  - Mostrar estado del formulario (pendiente, en_proceso, completado)
  - Agregar notificación al completar formulario
  - _Requerimientos: 34.9, 35.3, 35.6_

- [ ] 132. Implementar sincronización de respuestas y generación de PDF
  - Crear ResponseSyncService.cs
  - Implementar cola de respuestas pendientes
  - Sincronizar respuestas al recuperar conexión → op_respuesta_formulario + op_respuesta_campo
  - Subir fotos a Azure Blob Storage (contenedor: formularios-fotos)
  - Subir firma a Azure Blob Storage (contenedor: formularios-firmas)
  - Disparar generación de PDF en servidor (usa plantilla HTML de cat_plantilla_pdf)
  - Actualizar estado local después de sincronización exitosa
  - _Requerimientos: 34.14, 34.15, 34.16, 35.6_

- [ ]* 132.1 Escribir property tests para formularios móviles
  - **Property 45: Formulario offline se sincroniza correctamente (Round-trip)**
  - **Valida: Requerimientos 34.14, 34.16**
  - Generar respuestas offline aleatorias
  - Simular sincronización
  - Verificar integridad de datos con hash
  - Verificar que PDF se genera con todos los datos
  - Ejecutar 100+ iteraciones

- [ ]* 132.2 Escribir unit tests para formularios móviles
  - Test: Renderizar campo de texto con Entry
  - Test: Renderizar campo de foto con Button + Image
  - Test: Renderizar campo de firma con SignaturePad
  - Test: Validar campo requerido vacío
  - Test: Guardar respuesta en SQLite
  - Test: Sincronizar respuesta pendiente
  - Test: Comprimir foto antes de subir (máximo 1MB)
  - Test: Hash de control se calcula correctamente
  - Test: Respetar orden de campos por seccion y posicion_orden
  - _Requerimientos: 34.9, 34.11, 34.12, 34.13, 34.14, 35.3, 35.4, 35.5_

- [ ] 133. Checkpoint - Verificar módulo de Formularios Dinámicos (App Móvil)
  - Asegurar que todos los tests pasan
  - Probar renderizado de diferentes tipos de campos
  - Verificar captura de fotos y firma
  - Probar modo offline completo
  - Verificar sincronización al recuperar conexión
  - Probar integración con flujo de Colaborador
  - Preguntar al usuario si surgen dudas

---

## Resumen de Tareas Agregadas

### Módulo de Servicios Municipales (Portal Web)
- **Fase 23**: 15 tareas principales + 7 tareas de testing
- **Tiempo estimado**: 3-4 semanas con 1 desarrollador senior + IA
- **Componentes**: Fallos, OCs, Dictámenes, KPIs, Alertas, Chat

### Módulo de Servicios Municipales (App Móvil)
- **Fase 24**: 4 tareas principales + 2 tareas de testing
- **Tiempo estimado**: 1-2 semanas con 1 desarrollador móvil + IA
- **Componentes**: Documentos para Colaboradores, Chat móvil, KPIs móvil

### Módulo de Formularios Dinámicos (Portal Web)
- **Fase 25**: 13 tareas principales + 3 tareas de testing
- **Tiempo estimado**: 2-3 semanas con 1 desarrollador senior + IA
- **Componentes**: Azure Document Intelligence, gestión de formularios, campos con layout, DevExpress ASPxFormLayout, generación PDF con SelectPdf/iTextSharp, Azure Blob Storage

### Módulo de Formularios Dinámicos (App Móvil)
- **Fase 26**: 9 tareas principales + 2 tareas de testing
- **Tiempo estimado**: 2-3 semanas con 1 desarrollador móvil + IA
- **Componentes**: SQLite offline, renderizado dinámico con StackLayout + controles MAUI nativos, captura de fotos, firma digital, sincronización con Azure Blob Storage

### Total Agregado
- **41 tareas principales**
- **14 tareas de testing (opcionales)**
- **Tiempo total estimado**: 8-12 semanas

