# TASKS - Módulo de Tickets Colaborativos con IA
## Sistema JELABBC - Plan de Implementación

**Fecha:** 18 de Enero de 2026  
**Estado:** En Desarrollo - Fase 1  
**Prioridad:** Alta

---

## 🎯 ESTADO ACTUAL DEL PROYECTO

### ✅ Completado (41 tareas - 32%)

**Base de Datos (20/20 - 100%):**
- ✅ Tabla op_tickets_v2 alterada con 13 campos nuevos
- ✅ 8 tablas nuevas creadas (logs, métricas, validación, notificaciones, robot, prompts)
- ✅ 5 tablas de Telegram creadas (clientes, whitelist, blacklist, logs, queue)
- ✅ Trigger y campos adicionales para Telegram
- ✅ 3 stored procedures (ValidarClienteDuplicado, EncolarNotificacionWhatsApp, CalcularMetricasDiarias)
- ✅ Scripts ejecutados y verificados en jela_qa

**API Backend (16/21 - 76%):**
- ✅ Modelos: TicketModels.cs y WebhookModels.cs creados
- ✅ 6 Servicios implementados:
  - TicketValidationService (validación de duplicados, historial)
  - TicketNotificationService (cola de notificaciones, reintentos)
  - TicketMetricsService (métricas tiempo real, cálculo diario)
  - PromptTuningService (análisis, ajustes automáticos)
  - YCloudService (WhatsApp Business)
  - VapiService (llamadas telefónicas)
- ✅ WebhookEndpoints.cs con 4 canales (VAPI, YCloud, ChatWeb, Firebase)
- ✅ Todos los servicios registrados en Program.cs

**Frontend (3/19 - 16%):**
- ✅ Dashboard de tickets integrado en Inicio.aspx

### 🚧 Próximas Tareas Prioritarias

**1. Completar Endpoints de API (3 tareas pendientes):**
- [ ] TicketValidationEndpoints.cs - Endpoints de validación (Tarea 2.3.2)
- [ ] TicketNotificationEndpoints.cs - Endpoints de notificaciones (Tarea 2.3.3)
- [ ] TicketMetricsEndpoints.cs - Endpoints de métricas (Tarea 2.3.4)
- [ ] Agregar configuración VAPI y YCloud en appsettings.json (Tareas 2.5.1, 2.5.2)

**NOTA:** Los servicios backend (TicketValidationService, TicketNotificationService, TicketMetricsService, etc.) YA ESTÁN COMPLETADOS ✅

**2. Background Services (5 tareas pendientes):**
- [ ] TicketMonitoringBackgroundService - Monitoreo cada 5 min (Tarea 3.1.1)
- [ ] TicketMetricsBackgroundService - Métricas cada hora (Tarea 3.1.2)
- [ ] NotificationQueueBackgroundService - Cola cada 30 seg (Tarea 3.1.3)
- [ ] PromptTuningBackgroundService - Ajustes cada 2 semanas (Tarea 3.1.4)
- [ ] Registrar Background Services en Program.cs (Tarea 3.2.1)

**3. Interfaces Web (16 tareas):**
- [ ] TicketsPrompts.aspx - Catálogo de prompts IA
- [ ] TicketsLogs.aspx - Auditoría completa
- [ ] Mejorar Tickets.aspx - Columnas dinámicas, nuevas acciones
- [ ] Agregar opciones de menú en base de datos

**4. Integraciones Externas (24 tareas):**
- [ ] Configurar VAPI (cuenta, número, webhook, pruebas)
- [ ] Configurar YCloud WhatsApp (cuenta, templates, pruebas)
- [ ] Desarrollar widget Chat Web
- [ ] Configurar Firebase (proyecto, reglas, Cloud Functions)
- [ ] Configurar Telegram Bot (BotFather, webhook, comandos)
- [ ] Pruebas de integración multicanal

**5. Expansión Chat Web Avanzado (28 tareas - NUEVO):**
- [ ] Crear 4 tablas nuevas (conf_chat_actions, conf_chat_queries, op_chat_history, op_chat_confirmations)
- [ ] Implementar 4 servicios backend (.NET 8)
- [ ] Crear 3 endpoints de API
- [ ] Mejorar widget de chat (confirmaciones, tablas, navegación)
- [ ] Configuración inicial (acciones, consultas, navegación)

**6. � Seguridad y Control de Costos IA (26 tareas - CRÍTICO):**
- [ ] Crear 2 tablas de control de costos (op_ticket_cost_control, op_ticket_cost_tracking)
- [ ] Implementar Circuit Breaker con Polly (prevenir fallos en cascada)
- [ ] Implementar reintentos controlados con exponential backoff
- [ ] Implementar control de costos en tiempo real (validar presupuesto antes de cada solicitud)
- [ ] Implementar límites de tokens por solicitud (máx 1500 tokens)
- [ ] Implementar Background Service de monitoreo de costos (cada hora)
- [ ] Implementar rate limiting avanzado por canal y por usuario
- [ ] Implementar sistema de alertas (Email, WhatsApp) al alcanzar 80% del presupuesto
- [ ] Testing exhaustivo de seguridad

### 📊 Métricas de Progreso

- **Total Tareas Fase 1:** 127
- **Completadas:** 41 (32%)
  - Base de Datos: 20/20 (100%) ✅
  - Servicios Backend: 6/6 (100%) ✅
  - Endpoints: 1/4 (25%) - Solo WebhookEndpoints completado
  - Background Services: 0/5 (0%)
  - Frontend: 3/19 (16%)
- **Pendientes:** 86 (68%)
- **Total Tareas Chat Web Avanzado:** 28 (NUEVO)
- **Total Tareas Seguridad y Costos:** 26 (CRÍTICO - Implementar ANTES de producción) 🚨
- **Estimación restante Fase 1:** 6-7 semanas
- **Estimación Chat Web Avanzado:** 6-10 días
- **Estimación Seguridad y Costos:** 3-5 días (ALTA PRIORIDAD)

---

## 📚 Referencias de Documentación

- **Design:** `.kiro/specs/tickets-colaborativos/design.md`
- **UI Standards:** `.kiro/specs/ecosistema-jelabbc/ui-standards.md`

## 📖 Convención de Referencias

- `.kiro/specs/tickets-colaborativos/design.md § X.Y` = Sección X.Y del documento de diseño
- `.kiro/specs/ecosistema-jelabbc/ui-standards.md § X` = Sección X de estándares UI
- `→` = Apunta a la sección especificada

## 🔤 Leyenda

- `[ ]` = Pendiente
- `[x]` = Completado
- `[-]` = En progreso
- `[~]` = En cola
- `*` = Tarea opcional (después del checkbox)

---

## 1. BASE DE DATOS → .kiro/specs/tickets-colaborativos/design.md § 4

### 1.1 Alteración de Tabla Existente

- [x] 1.1.1 Alterar tabla op_tickets_v2 (13 campos nuevos)
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.2 | **Script:** SQL migration
  > Agregar campos: TipoTicket, IPOrigen, DuracionLlamadaSegundos, MomentoCorte, IntentosReconexion, MontoRelacionado, PedidoRelacionado, RiesgoFraude, RequiereEscalamiento, Impacto, CSATScore, ResueltoporIA, Idioma
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/01_ALTER_op_tickets_v2_agregar_campos_nuevos.sql`

### 1.2 Creación de Tablas Nuevas (8 tablas)

- [x] 1.2.1 Crear tabla op_ticket_logs_sistema
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.3.1 | **Script:** SQL migration
  > Auditoría de eventos del sistema con severidad y metadata JSON
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/02_CREATE_op_ticket_logs_sistema.sql`

- [x] 1.2.2 Crear tabla op_ticket_logs_interacciones
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.3.2 | **Script:** SQL migration
  > Tracking de interacciones multicanal (VAPI, YCloud, ChatWeb, Firebase)
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/03_CREATE_op_ticket_logs_interacciones.sql`

- [x] 1.2.3 Crear tabla op_ticket_logprompts
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.3.3 | **Script:** SQL migration
  > Log de prompts anonimizados para análisis y mejora continua
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/04_CREATE_op_ticket_logprompts.sql`

- [x] 1.2.4 Crear tabla op_ticket_metricas
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.3.4 | **Script:** SQL migration
  > Métricas agregadas por periodo (diaria, semanal, mensual)
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/05_CREATE_op_ticket_metricas.sql`

- [x] 1.2.5 Crear tabla op_ticket_validacion_cliente
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.3.5 | **Script:** SQL migration
  > Validación de clientes duplicados por teléfono, email, IP
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/06_CREATE_op_ticket_validacion_cliente.sql`

- [x] 1.2.6 Crear tabla op_ticket_notificaciones_whatsapp
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.3.6 | **Script:** SQL migration
  > Cola de notificaciones WhatsApp con reintentos y estados
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/07_CREATE_op_ticket_notificaciones_whatsapp.sql`

- [x] 1.2.7 Crear tabla op_ticket_robot_monitoreo
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.3.7 | **Script:** SQL migration
  > Tracking de ejecuciones del robot de monitoreo automático
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/08_CREATE_op_ticket_robot_monitoreo.sql`

- [x] 1.2.8 Crear tabla op_ticket_prompt_ajustes_log
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.3.8 | **Script:** SQL migration
  > Registro de ajustes automáticos de prompts cada 2 semanas
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/09_CREATE_op_ticket_prompt_ajustes_log.sql`

### 1.2.1 Creación de Tablas de Telegram (5 tablas)

- [x] 1.2.9 Crear tabla op_telegram_clientes
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.4.1 | **Script:** SQL migration
  > Registro y gestión de clientes que interactúan vía Telegram con campos: ChatId, Username, FirstName, LastName, EstadoCliente, TipoCliente, FechaVencimiento, CreditosDisponibles, TicketsMesActual, LimiteTicketsMes
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/10_CREATE_op_telegram_clientes.sql`

- [x] 1.2.10 Crear tabla op_telegram_whitelist
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.4.2 | **Script:** SQL migration
  > Lista de clientes pre-aprobados con acceso prioritario: ChatId, ClienteNombre, Email, Empresa, FechaAprobacion, AprobadoPor, Prioridad
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/11_CREATE_op_telegram_whitelist.sql`

- [x] 1.2.11 Crear tabla op_telegram_blacklist
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.4.3 | **Script:** SQL migration
  > Lista de clientes bloqueados permanente o temporalmente: ChatId, Username, RazonBloqueo, FechaBloqueo, BloqueadoPor, Permanente, FechaLevantamiento
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/12_CREATE_op_telegram_blacklist.sql`

- [x] 1.2.12 Crear tabla op_telegram_logs_validacion
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.4.4 | **Script:** SQL migration
  > Registro de todas las validaciones de clientes (sistema de 4 niveles): ChatId, FechaValidacion, Resultado, NivelAlcanzado, RazonRechazo, IPOrigen, Metadata
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/13_CREATE_op_telegram_logs_validacion.sql`

- [x] 1.2.13 Crear tabla op_telegram_notifications_queue
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.4.5 | **Script:** SQL migration
  > Cola de notificaciones pendientes de envío a Telegram: IdTicket, ChatId, TipoNotificacion, EstadoNuevo, Mensaje, Procesado, IntentosEnvio
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/14_CREATE_op_telegram_notifications_queue.sql`

### 1.2.2 Trigger y Campos Adicionales para Telegram

- [x] 1.2.14 Crear trigger trg_NotificarCambioEstadoTelegram
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.4.6 | **Script:** SQL migration
  > Encolar automáticamente notificaciones cuando cambia el estado de un ticket de Telegram
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/15_CREATE_trigger_y_campos_telegram.sql`

- [x] 1.2.15 Agregar campos adicionales en op_tickets_v2 para Telegram
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.4.7 | **Script:** SQL migration
  > Agregar campos: ChatId (BIGINT), ClienteValidado (BOOLEAN), NivelValidacion (VARCHAR), CreditosUsados (INT)
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/15_CREATE_trigger_y_campos_telegram.sql`

### 1.3 Stored Procedures

- [x] 1.3.1 Crear sp_ValidarClienteDuplicado
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.6.1 | **Script:** SQL migration
  > Validar si cliente tiene tickets abiertos por teléfono/email/IP
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/16_CREATE_sp_ValidarClienteDuplicado.sql`

- [x] 1.3.2 Crear sp_EncolarNotificacionWhatsApp
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.6.2 | **Script:** SQL migration
  > Encolar notificación WhatsApp en tabla de cola
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/17_CREATE_sp_EncolarNotificacionWhatsApp.sql`

- [x] 1.3.3 Crear sp_CalcularMetricasDiarias
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.6.3 | **Script:** SQL migration
  > Calcular y almacenar métricas agregadas diarias
  > **Completado:** Script SQL creado en `JelaWeb/Scripts/SQL/18_CREATE_sp_CalcularMetricasDiarias.sql`

### 1.4 Verificación de Base de Datos

- [x] 1.4.1 Ejecutar scripts de migración en ambiente de desarrollo
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4 | **Validación:** Verificar todas las tablas creadas
  > **Completado:** Scripts ejecutados exitosamente en base de datos jela_qa

- [x] 1.4.2 Verificar índices y foreign keys
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.3 | **Validación:** Verificar performance de queries
  > **Completado:** Índices y foreign keys verificados y funcionando correctamente

- [x] 1.4.3 Crear backup de base de datos antes de migración
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 9 | **Validación:** Backup completo de jela_qa
  > **Completado:** Backup de base de datos creado antes de ejecutar migraciones

---

## 2. API BACKEND (.NET 8) → design.md § 5

### 2.1 Modelos de Datos

- [x] 2.1.1 Crear TicketModels.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.1 | **Ubicación:** JELA.API/Models/TicketModels.cs
  > Modelos: TicketDto, TicketValidationRequest, TicketMetricsDto, etc.
  > **Completado:** Archivo creado con 15 modelos para validación, notificaciones, métricas, Telegram y ajuste de prompts

- [x] 2.1.2 Crear WebhookModels.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.1 | **Ubicación:** JELA.API/Models/WebhookModels.cs
  > Modelos: VAPIWebhookPayload, YCloudWebhookPayload, ChatWebMessage, etc.
  > **Completado:** Archivo creado con modelos tipados para 5 canales (VAPI, YCloud, Telegram, ChatWeb, Firebase)

### 2.2 Servicios de Lógica de Negocio

- [x] 2.2.1 Implementar TicketValidationService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.2.1 | **Ubicación:** JELA.API/Services/TicketValidationService.cs
  > **Dependencias:** Tarea 1.2.5
  > Métodos: ValidarClienteDuplicado(), ObtenerHistorialCliente()
  > **Completado:** Servicio implementado con validación de duplicados, historial de clientes, actualización de validación y bloqueo de clientes

- [x] 2.2.2 Implementar TicketNotificationService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.2.2 | **Ubicación:** JELA.API/Services/TicketNotificationService.cs
  > **Dependencias:** Tarea 1.2.6
  > Métodos: EncolarNotificacion(), ProcesarCola(), EnviarWhatsApp()
  > **Completado:** Servicio implementado con cola de notificaciones, procesamiento automático con reintentos y envío de WhatsApp

- [x] 2.2.3 Implementar TicketMetricsService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.2.3 | **Ubicación:** JELA.API/Services/TicketMetricsService.cs
  > **Dependencias:** Tarea 1.2.4
  > Métodos: ObtenerMetricasTiempoReal(), CalcularMetricasDiarias()
  > **Completado:** Servicio implementado con métricas en tiempo real, cálculo diario, métricas por canal, registro de interacciones y prompts

- [x] 2.2.4 Implementar PromptTuningService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.2.4 | **Ubicación:** JELA.API/Services/PromptTuningService.cs
  > **Dependencias:** Tarea 1.2.8
  > Métodos: AnalizarRendimientoPrompts(), ProponerAjustes()
  > **Completado:** Servicio implementado con análisis de rendimiento, propuesta automática de ajustes, registro y aprobación de cambios

- [x] 2.2.5 Implementar YCloudService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.2.5 | **Ubicación:** JELA.API/Services/YCloudService.cs
  > Métodos: EnviarMensaje(), ValidarWebhook(), ProcesarRespuesta()
  > **Completado:** Servicio implementado con envío de mensajes WhatsApp, validación de webhooks HMAC-SHA256, y procesamiento de respuestas

- [x] 2.2.6 Implementar VapiService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.2.6 | **Ubicación:** JELA.API/Services/VapiService.cs
  > Métodos: ProcesarLlamada(), ValidarWebhook(), ObtenerTranscripcion()
  > **Completado:** Servicio implementado con procesamiento de llamadas, validación de webhooks, obtención de transcripciones y manejo de eventos

### 2.3 Endpoints de API

- [x] 2.3.1 Implementar WebhookEndpoints.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.4.1 | **Ubicación:** JELA.API/Endpoints/WebhookEndpoints.cs
  > **Dependencias:** Tareas 2.2.1, 2.2.5, 2.2.6
  > **COMPLETADO:** Archivo creado con 4 endpoints (VAPI, YCloud, ChatWeb, Firebase) y métodos auxiliares
  > Endpoints: POST /api/webhooks/vapi, /ycloud, /chatweb, /firebase
  > 
  > **Funcionalidad implementada por canal:**
  > - **VAPI:** Recibe llamadas, valida duplicados, procesa con IA, crea ticket, registra interacción
  > - **YCloud:** Recibe WhatsApp, valida duplicados, procesa con IA, crea ticket, envía respuesta
  > - **ChatWeb:** Recibe mensajes web, valida por email/IP, procesa con IA, crea ticket, retorna JSON
  > - **Firebase:** Recibe mensajes app, valida duplicados, procesa con IA, crea ticket
  > 
  > **Métodos auxiliares implementados:**
  > - `CrearTicketVAPI()` - Crear ticket para llamadas telefónicas
  > - `CrearTicketYCloud()` - Crear ticket para WhatsApp
  > - `CrearTicketChatWeb()` - Crear ticket para chat web
  > - `CrearTicketFirebase()` - Crear ticket para app móvil
  > - `RegistrarInteraccion()` - Insertar en op_ticket_logs_interacciones

  - [x] 2.3.1.1 Implementar endpoint POST /api/webhooks/vapi
    > **COMPLETADO:** Endpoint implementado con validación de duplicados, procesamiento IA y creación de tickets

  - [x] 2.3.1.2 Implementar endpoint POST /api/webhooks/ycloud
    > **COMPLETADO:** Endpoint implementado con validación, procesamiento IA y envío de respuesta automática

  - [x] 2.3.1.3 Implementar endpoint POST /api/webhooks/chatweb
    > **COMPLETADO:** Endpoint implementado con validación por email/IP y respuesta JSON

  - [x] 2.3.1.4 Implementar endpoint POST /api/webhooks/firebase
    > **COMPLETADO:** Endpoint implementado con validación y creación de tickets (pendiente notificaciones push)

  - [x] 2.3.1.5 Implementar métodos auxiliares de WebhookEndpoints
    > **COMPLETADO:** Métodos CrearTicket*() y RegistrarInteraccion() implementados

- [ ] 2.3.2 Implementar TicketValidationEndpoints.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.3.2 | **Ubicación:** JELA.API/Endpoints/TicketValidationEndpoints.cs
  > **Dependencias:** Tarea 2.2.1
  > Endpoints: POST /api/tickets/validar-cliente, GET /api/tickets/historial/{telefono}

- [ ] 2.3.3 Implementar TicketNotificationEndpoints.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.3.3 | **Ubicación:** JELA.API/Endpoints/TicketNotificationEndpoints.cs
  > **Dependencias:** Tarea 2.2.2
  > Endpoints: POST /api/tickets/notificar-whatsapp, GET /api/tickets/notificaciones/cola

- [ ] 2.3.4 Implementar TicketMetricsEndpoints.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.3.4 | **Ubicación:** JELA.API/Endpoints/TicketMetricsEndpoints.cs
  > **Dependencias:** Tarea 2.2.3
  > Endpoints: GET /api/tickets/metricas/tiempo-real, /diarias, POST /calcular

### 2.4 Registro de Endpoints en Program.cs

- [x] 2.4.1 Registrar endpoints en Program.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.4 | **Ubicación:** JELA.API/Program.cs
  > **COMPLETADO:** app.MapWebhookEndpoints() ya registrado en Program.cs línea 237
  > Pendiente: Agregar TicketValidationEndpoints, TicketNotificationEndpoints, TicketMetricsEndpoints cuando se implementen

- [x] 2.4.2 Registrar servicios en DI container
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.4 | **Ubicación:** JELA.API/Program.cs
  > **COMPLETADO:** Todos los servicios ya registrados en Program.cs líneas 42-48:
  > - builder.Services.AddScoped<ITicketValidationService, TicketValidationService>()
  > - builder.Services.AddScoped<ITicketNotificationService, TicketNotificationService>()
  > - builder.Services.AddScoped<ITicketMetricsService, TicketMetricsService>()
  > - builder.Services.AddScoped<IPromptTuningService, PromptTuningService>()
  > - builder.Services.AddScoped<IYCloudService, YCloudService>()
  > - builder.Services.AddScoped<IVapiService, VapiService>()

### 2.5 Configuración

- [ ] 2.5.1 Agregar configuración de VAPI en appsettings.json
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.1 | **Ubicación:** JELA.API/appsettings.json
  > Agregar: VAPI:ApiKey, VAPI:WebhookSecret, VAPI:BaseUrl

- [ ] 2.5.2 Agregar configuración de YCloud en appsettings.json
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.2 | **Ubicación:** JELA.API/appsettings.json
  > Agregar: YCloud:ApiKey, YCloud:BaseUrl, YCloud:WebhookSecret

---

## 3. BACKGROUND SERVICES (.NET 8) → design.md § 6

### 3.1 Servicios de Fondo

- [ ] 3.1.1 Implementar TicketMonitoringBackgroundService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6.1 | **Ubicación:** JELA.API/BackgroundServices/TicketMonitoringBackgroundService.cs
  > **Dependencias:** Tarea 1.2.7
  > Ejecutar cada 5 minutos: monitorear tickets, detectar cambios, notificar

- [ ] 3.1.2 Implementar TicketMetricsBackgroundService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6.2 | **Ubicación:** JELA.API/BackgroundServices/TicketMetricsBackgroundService.cs
  > **Dependencias:** Tarea 2.2.3
  > Ejecutar cada hora: calcular métricas, actualizar dashboard

- [ ] 3.1.3 Implementar NotificationQueueBackgroundService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6.3 | **Ubicación:** JELA.API/BackgroundServices/NotificationQueueBackgroundService.cs
  > **Dependencias:** Tarea 2.2.2
  > Ejecutar cada 30 segundos: procesar cola de notificaciones WhatsApp

- [ ] 3.1.4 Implementar PromptTuningBackgroundService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6.4 | **Ubicación:** JELA.API/BackgroundServices/PromptTuningBackgroundService.cs
  > **Dependencias:** Tarea 2.2.4
  > Ejecutar cada 2 semanas: analizar prompts, proponer ajustes

### 3.2 Registro de Background Services

- [ ] 3.2.1 Registrar Background Services en Program.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6.5 | **Ubicación:** JELA.API/Program.cs
  > Agregar: builder.Services.AddHostedService<TicketMonitoringBackgroundService>(), etc.

---

## 4. INTERFACES WEB (ASP.NET VB.NET) → design.md § 7

### 4.1 Páginas Nuevas

- [x] 4.1.1 Integrar TicketsDashboard en Inicio.aspx
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.2 + .kiro/specs/ecosistema-jelabbc/ui-standards.md § 3 | **Ubicación:** JelaWeb/Views/Inicio.aspx
  > **Dependencias:** Tarea 2.3.4
  > **COMPLETADO:** Dashboard ya implementado en página de inicio
  > Funcionalidad: Métricas en tiempo real, gráficos DevExpress, KPIs

  - [x] 4.1.1.1 Actualizar Inicio.aspx (markup)
    > **COMPLETADO:** Sección de dashboard con cards de métricas y gráficas DevExpress ya implementada

  - [x] 4.1.1.2 Actualizar Inicio.aspx.vb (code-behind)
    > **COMPLETADO:** Métodos para cargar métricas vía DashboardBusiness ya implementados

  - [ ]* 4.1.1.3 Crear tickets-dashboard.css (OPCIONAL)
    > **NOTA:** Estilos ya están inline en Inicio.aspx. Crear archivo separado solo si se requiere refactorización

  - [ ]* 4.1.1.4 Crear tickets-dashboard.js (OPCIONAL)
    > **NOTA:** No se requiere JS adicional. DevExpress maneja interactividad client-side

- [ ] 4.1.2 Crear TicketsPrompts.aspx
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.3 + .kiro/specs/ecosistema-jelabbc/ui-standards.md § 3 | **Ubicación:** JelaWeb/Views/Catalogos/Tickets/
  > **Dependencias:** Tarea 2.2.4
  > Incluir: .aspx, .aspx.vb, .aspx.designer.vb
  > Funcionalidad: Catálogo de prompts de IA, versionamiento, aprobación de ajustes automáticos
  > **CRÍTICO:** Usar columnas dinámicas generadas desde DataTable (ver AreasComunes.aspx.vb)

  - [ ] 4.1.2.1 Crear TicketsPrompts.aspx (markup)
    > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 3.1 | Usar Master Page, DevExpress ASPxGridView sin columnas estáticas

  - [ ] 4.1.2.2 Crear TicketsPrompts.aspx.vb (code-behind)
    > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.3 | Consumir API vía ApiConsumerCRUD.vb, implementar GenerarColumnasDinamicas()

  - [ ] 4.1.2.3 Crear tickets-prompts.css
    > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 4 | **Ubicación:** JelaWeb/Content/CSS/

  - [ ] 4.1.2.4 Crear tickets-prompts.js
    > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 5 | **Ubicación:** JelaWeb/Scripts/app/Catalogos/Tickets/

- [ ] 4.1.3 Crear TicketsLogs.aspx
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.3 + .kiro/specs/ecosistema-jelabbc/ui-standards.md § 3, § 7 | **Ubicación:** JelaWeb/Views/Operacion/Tickets/
  > **Dependencias:** Tareas 1.2.1, 1.2.2
  > Incluir: .aspx, .aspx.vb, .aspx.designer.vb
  > Funcionalidad: Auditoría completa, filtros en columnas del grid (.kiro/specs/ecosistema-jelabbc/ui-standards.md § 7), exportación
  > **CRÍTICO:** Solo fechas arriba del grid, Tipo y Severidad como columnas con AllowHeaderFilter="True"
  > **CRÍTICO:** Usar columnas dinámicas generadas desde DataTable (ver AreasComunes.aspx.vb)

  - [ ] 4.1.3.1 Crear TicketsLogs.aspx (markup)
    > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 3.1 | Usar Master Page, DevExpress ASPxGridView sin columnas estáticas

  - [ ] 4.1.3.2 Crear TicketsLogs.aspx.vb (code-behind)
    > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.3 | Consumir API vía ApiConsumerCRUD.vb, implementar GenerarColumnasDinamicas()

  - [ ] 4.1.3.3 Crear tickets-logs.css
    > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 4 | **Ubicación:** JelaWeb/Content/CSS/

  - [ ] 4.1.3.4 Crear tickets-logs.js
    > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 5 | **Ubicación:** JelaWeb/Scripts/app/Operacion/

### 4.2 Mejoras a Página Existente

- [ ] 4.2.1 Mejorar Tickets.aspx (existente)
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.5 + .kiro/specs/ecosistema-jelabbc/ui-standards.md § 3, § 6, § 7 | **Ubicación:** JelaWeb/Views/Operacion/Tickets/
  > **Dependencias:** Tareas 2.3.1, 2.3.2
  > **Estado actual:** Filtros de fecha OK, popup modal OK, integración IA OK
  > **CRÍTICO:** Implementar columnas dinámicas (actualmente son estáticas en ASPX)
  > **Pendiente:** Corregir ShowFilterRowMenu="True", agregar SettingsSearchPanel, implementar GenerarColumnasDinamicas()

  - [ ] 4.2.1.1 Actualizar Tickets.aspx (markup)
    > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 3.2, § 6, § 7 | **CRÍTICO:** Eliminar columnas estáticas del ASPX (líneas 103-160), corregir ShowFilterRowMenu="True", agregar SettingsSearchPanel

  - [ ] 4.2.1.2 Actualizar Tickets.aspx.vb (code-behind)
    > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.5 | **CRÍTICO:** Implementar método GenerarColumnasDinamicas() (ver AreasComunes.aspx.vb), agregar métodos para nuevas acciones (Escalar, Marcar Fraude)

  - [ ] 4.2.1.3 Actualizar tickets.css
    > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 4 | Agregar estilos para badges de estado (Fraude, Escalamiento, IA)

  - [ ] 4.2.1.4 Actualizar tickets.js
    > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 5, § 7 | Agregar funcionalidad para nuevas acciones del toolbar

### 4.3 Navegación y Menú

- [ ] 4.3.1 Agregar opciones de menú en base de datos
  > **Ref:** .kiro/specs/ecosistema-jelabbc/ui-standards.md § 2 | **Ubicación:** Base de datos (tabla conf_opciones o similar)
  > **IMPORTANTE:** El menú Ribbon se construye dinámicamente desde opciones almacenadas en la base de datos
  > El método `ConstruirRibbon(opciones As JArray)` en `JelaWeb/MasterPages/Jela.Master.vb` lee las opciones desde `SessionHelper.GetOpciones()`
  > 
  > **Acciones requeridas:**
  > 1. Identificar la tabla de opciones/permisos en la base de datos (probablemente `conf_opciones`)
  > 2. Insertar registro para TicketsPrompts.aspx:
  >    - Nombre: "Prompts de IA"
  >    - Url: "~/Views/Catalogos/Tickets/TicketsPrompts.aspx"
  >    - RibbonTab: "Catálogos"
  >    - RibbonGroup: "Tickets"
  >    - Icono: (seleccionar icono apropiado)
  > 3. Insertar registro para TicketsLogs.aspx:
  >    - Nombre: "Logs de Tickets"
  >    - Url: "~/Views/Operacion/Tickets/TicketsLogs.aspx"
  >    - RibbonTab: "Operación"
  >    - RibbonGroup: "Tickets"
  >    - Icono: (seleccionar icono apropiado)
  > 4. Asignar permisos a roles/usuarios según sea necesario
  > 
  > **Nota:** TicketsDashboard se muestra en página de inicio (Inicio.aspx), no requiere enlace separado en el menú

---

## 5. INTEGRACIONES EXTERNAS → design.md § 8

### 5.1 Configuración de VAPI (Llamadas Telefónicas)

- [ ] 5.1.1 Configurar cuenta VAPI
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.1 | Obtener API Key y configurar webhook URL
  > **Acciones:**
  > 1. Crear cuenta en https://vapi.ai
  > 2. Obtener API Key desde el dashboard
  > 3. Configurar webhook URL: `https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/webhooks/vapi`
  > 4. Configurar webhook secret para validación de firma
  > 5. Guardar credenciales en Azure Key Vault o appsettings.json

- [ ] 5.1.2 Configurar número telefónico en VAPI
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.1 | Asignar número y configurar agente IA
  > **Acciones:**
  > 1. Comprar o asignar número telefónico en VAPI
  > 2. Configurar agente IA con prompt personalizado
  > 3. Configurar idioma (español)
  > 4. Configurar voz y velocidad de habla
  > 5. Habilitar transcripción en tiempo real
  > 6. Configurar detección de intención y sentimiento

- [ ] 5.1.3 Probar webhook de VAPI
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.1 | **Dependencias:** Tarea 2.3.1.1
  > Validar recepción de webhooks y procesamiento
  > **Pruebas:**
  > 1. Realizar llamada de prueba al número configurado
  > 2. Verificar que el webhook se reciba en el endpoint
  > 3. Verificar que se cree el ticket correctamente
  > 4. Verificar que se registre la interacción en op_ticket_logs_interacciones
  > 5. Verificar que se guarde la transcripción completa
  > 6. Verificar que se registre la duración de la llamada

- [ ] 5.1.4 Implementar manejo de eventos de VAPI
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.1 | Manejar eventos adicionales de VAPI
  > **Eventos a manejar:**
  > - `call.started` - Llamada iniciada
  > - `call.ended` - Llamada terminada
  > - `call.disconnected` - Llamada cortada
  > - `transcription.partial` - Transcripción parcial en tiempo real
  > - `transcription.final` - Transcripción final completa

### 5.2 Configuración de YCloud (WhatsApp Business)

- [ ] 5.2.1 Configurar cuenta YCloud WhatsApp Business
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.2 | Obtener API Key y configurar webhook URL
  > **Acciones:**
  > 1. Crear cuenta en https://www.ycloud.com
  > 2. Verificar cuenta de WhatsApp Business
  > 3. Obtener API Key desde el dashboard
  > 4. Configurar webhook URL: `https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net/api/webhooks/ycloud`
  > 5. Configurar webhook secret para validación
  > 6. Guardar credenciales en Azure Key Vault o appsettings.json

- [ ] 5.2.2 Crear templates de WhatsApp
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.2 | Crear templates: TicketCreado, TicketResuelto, etc.
  > **Templates requeridos:**
  > 1. **ticket_creado** - Confirmación de creación de ticket
  >    - Parámetros: {nombre_cliente}, {numero_ticket}, {asunto}
  > 2. **ticket_actualizado** - Notificación de cambio de estado
  >    - Parámetros: {numero_ticket}, {estado_nuevo}, {comentario}
  > 3. **ticket_resuelto** - Notificación de resolución
  >    - Parámetros: {numero_ticket}, {solucion}, {tiempo_resolucion}
  > 4. **ticket_asignado** - Notificación de asignación a agente
  >    - Parámetros: {numero_ticket}, {nombre_agente}
  > 5. **solicitud_csat** - Solicitud de calificación de satisfacción
  >    - Parámetros: {numero_ticket}, {link_encuesta}

- [ ] 5.2.3 Probar envío de mensajes WhatsApp
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.2 | **Dependencias:** Tarea 2.2.5
  > Validar envío y recepción de mensajes
  > **Pruebas:**
  > 1. Enviar mensaje de prueba desde YCloudService
  > 2. Verificar recepción en WhatsApp
  > 3. Responder desde WhatsApp
  > 4. Verificar que el webhook se reciba correctamente
  > 5. Verificar que se cree el ticket
  > 6. Verificar que se envíe respuesta automática

- [ ] 5.2.4 Implementar cola de notificaciones WhatsApp
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6.3 | **Dependencias:** Tarea 3.1.3
  > Implementar procesamiento de cola con reintentos
  > **Funcionalidad:**
  > - Procesar cola cada 30 segundos
  > - Reintentar hasta 3 veces en caso de fallo
  > - Registrar errores en op_ticket_notificaciones_whatsapp
  > - Actualizar estado de notificación (Pendiente → Enviando → Enviado/Fallido)

### 5.3 Configuración de Chat Web (Widget)

- [x] 5.3.1 Desarrollar widget de chat web
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.3 | Crear widget embebible en sitio web
  > **COMPLETADO:** Widget JavaScript completo implementado e integrado en Master Page
  > **Ubicación:** JelaWeb/Scripts/widgets/chat-widget.js
  > **Características implementadas:**
  > - Widget flotante en esquina inferior derecha ✅
  > - Formulario de contacto: Nombre, Email, Mensaje ✅
  > - Envío de mensaje vía POST a /api/webhooks/chatweb ✅
  > - Mostrar respuesta de IA en tiempo real ✅
  > - Historial de conversación en sesión (sessionStorage) ✅
  > - Diseño responsivo y personalizable (3 temas) ✅
  > - Captura automática de IP del cliente ✅
  > - Rate limiting del lado del cliente (5 msg/hora) ✅
  > - Persistencia de sesión entre recargas ✅
  > - Animaciones y accesibilidad ✅
  > **Archivos creados:**
  > - JelaWeb/Scripts/widgets/chat-widget.js
  > - JelaWeb/Content/CSS/chat-widget.css
  > - JelaWeb/Views/TestChatWidget.aspx (página de prueba)
  > **Integración:** Widget visible en todas las páginas vía Master Page

- [x] 5.3.2 Implementar endpoint de Chat Web
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.4.1 | **Dependencias:** Tarea 2.3.1.3
  > **COMPLETADO:** Endpoint POST /api/webhooks/chatweb ya implementado en WebhookEndpoints.cs

- [ ] 5.3.3 Probar integración de Chat Web
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.3 | **Dependencias:** Tareas 5.3.1, 2.3.1.3
  > **LISTO PARA PROBAR:** Acceder a /Views/TestChatWidget.aspx
  > Validar funcionamiento end-to-end
  > **Pruebas:**
  > 1. Embeber widget en página de prueba ✅ (integrado en Master Page)
  > 2. Enviar mensaje desde el widget
  > 3. Verificar que se cree el ticket
  > 4. Verificar que se reciba respuesta de IA
  > 5. Verificar que se registre la IP del cliente
  > 6. Verificar validación de cliente duplicado por email/IP

- [ ] 5.3.4 Implementar rate limiting para Chat Web
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.3 | Prevenir abuso del widget
  > **PARCIALMENTE COMPLETADO:** Rate limiting del cliente implementado
  > **Pendiente:** Rate limiting del servidor en el endpoint
  > **Configuración actual (cliente):**
  > - Máximo 5 mensajes por hora (localStorage) ✅
  > - Bloqueo temporal de 1 hora ✅
  > **Configuración pendiente (servidor):**
  > - Máximo 5 mensajes por IP por hora
  > - Máximo 10 mensajes por email por día
  > - Registro de intentos bloqueados en logs

### 5.4 Configuración de Firebase (Chat App Móvil)

- [ ] 5.4.1 Configurar proyecto Firebase
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | Configurar Realtime Database y Authentication
  > **Acciones:**
  > 1. Crear proyecto en Firebase Console
  > 2. Habilitar Realtime Database
  > 3. Habilitar Authentication (Email/Password, Google, etc.)
  > 4. Configurar Cloud Functions para webhooks
  > 5. Obtener credenciales de servicio (service account JSON)
  > 6. Guardar credenciales en Azure Key Vault

- [ ] 5.4.2 Configurar reglas de seguridad Firebase
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | Configurar reglas de lectura/escritura
  > **Reglas requeridas:**
  > ```json
  > {
  >   "rules": {
  >     "tickets": {
  >       "$userId": {
  >         ".read": "auth != null && auth.uid == $userId",
  >         ".write": "auth != null && auth.uid == $userId"
  >       }
  >     },
  >     "messages": {
  >       "$ticketId": {
  >         ".read": "auth != null",
  >         ".write": "auth != null"
  >       }
  >     }
  >   }
  > }
  > ```

- [ ] 5.4.3 Implementar Cloud Function para webhook
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | Crear función que envíe webhook a JELA.API
  > **Ubicación:** Firebase Cloud Functions
  > **Funcionalidad:**
  > - Escuchar cambios en `/messages/{ticketId}`
  > - Enviar POST a /api/webhooks/firebase con datos del mensaje
  > - Incluir token de autenticación Firebase
  > - Manejar errores y reintentos

- [ ] 5.4.4 Probar integración con Chat App
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | **Dependencias:** Tarea 2.3.1.4
  > Validar recepción de mensajes desde app móvil
  > **Pruebas:**
  > 1. Enviar mensaje desde app móvil de prueba
  > 2. Verificar que Cloud Function se ejecute
  > 3. Verificar que el webhook llegue a /api/webhooks/firebase
  > 4. Verificar que se cree el ticket
  > 5. Verificar que se envíe notificación push de respuesta
  > 6. Verificar que el mensaje aparezca en Realtime Database

- [ ] 5.4.5 Implementar notificaciones push
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | Enviar notificaciones push vía Firebase Cloud Messaging
  > **Funcionalidad:**
  > - Enviar notificación cuando se crea ticket
  > - Enviar notificación cuando cambia estado
  > - Enviar notificación cuando hay respuesta de agente
  > - Personalizar título, cuerpo e icono de notificación
  > - Incluir deep link al ticket en la app

### 5.5 Configuración de Telegram Bot

- [ ] 5.5.1 Crear bot de Telegram con BotFather
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | Crear bot y obtener token
  > **Acciones:**
  > 1. Abrir Telegram y buscar @BotFather
  > 2. Enviar comando /newbot
  > 3. Seguir instrucciones para nombrar el bot
  > 4. Guardar el Bot Token proporcionado
  > 5. Configurar comandos del bot con /setcommands:
  >    - /start - Iniciar conversación con el bot
  >    - /help - Obtener ayuda
  >    - /status - Ver estado de mis tickets
  >    - /nuevo - Crear nuevo ticket
  > 6. Configurar descripción del bot con /setdescription
  > 7. Configurar foto de perfil con /setuserpic

- [ ] 5.5.2 Configurar webhook de Telegram
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | Configurar webhook para recibir mensajes
  > **Comando:**
  > ```bash
  > curl -X POST "https://api.telegram.org/bot<BOT_TOKEN>/setWebhook" \
  >   -H "Content-Type: application/json" \
  >   -d '{"url": "https://jela-api-xxx.azurewebsites.net/api/webhooks/telegram"}'
  > ```
  > **Validar configuración:**
  > ```bash
  > curl "https://api.telegram.org/bot<BOT_TOKEN>/getWebhookInfo"
  > ```

- [ ] 5.5.3 Agregar configuración de Telegram en appsettings.json
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | **Ubicación:** JELA.API/appsettings.json
  > ```json
  > "Telegram": {
  >   "BotToken": "bot_token_from_botfather",
  >   "WebhookUrl": "https://jela-api-xxx.azurewebsites.net/api/webhooks/telegram",
  >   "AllowedChatIds": []
  > }
  > ```

- [ ] 5.5.4 Implementar TelegramService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | **Ubicación:** JELA.API/Services/TelegramService.cs
  > **Métodos requeridos:**
  > - `Task<bool> EnviarMensajeTelegram(long chatId, string mensaje)`
  > - `Task<bool> ValidarChatIdPermitido(long chatId)` (whitelist/blacklist)
  > - `Task<TelegramUser> ObtenerInfoUsuario(long chatId)`
  > - `Task<bool> EnviarNotificacionCambioEstado(long chatId, int ticketId, string nuevoEstado)`
  > **Características:**
  > - Soporte para formato Markdown en mensajes
  > - Manejo de errores de API de Telegram
  > - Rate limiting (30 mensajes/segundo)
  > - Logging de mensajes enviados

- [ ] 5.5.5 Agregar endpoint POST /api/webhooks/telegram
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.4.1 | **Dependencias:** Tareas 5.5.1-5.5.4
  > **Ubicación:** JELA.API/Endpoints/WebhookEndpoints.cs (agregar al archivo existente)
  > **NOTA:** WebhookEndpoints.cs ya existe con 4 endpoints (VAPI, YCloud, ChatWeb, Firebase). Agregar Telegram como 5to endpoint.
  > **Funcionalidad:**
  > - Recibir webhook de Telegram Bot API
  > - Validar chat_id contra whitelist/blacklist (op_telegram_whitelist, op_telegram_blacklist)
  > - Implementar sistema de validación de 4 niveles (ver design.md § 4.5)
  > - Extraer mensaje de texto del update
  > - Validar cliente duplicado
  > - Procesar con Azure OpenAI
  > - Crear ticket automáticamente
  > - Enviar respuesta al usuario por Telegram vía TelegramService
  > - Registrar interacción en op_ticket_logs_interacciones
  > - Registrar validación en op_telegram_logs_validacion
  > **Estructura del webhook:**
  > ```json
  > {
  >   "update_id": 123456789,
  >   "message": {
  >     "message_id": 1,
  >     "from": {
  >       "id": 987654321,
  >       "first_name": "Juan",
  >       "username": "juanperez"
  >     },
  >     "chat": {
  >       "id": 987654321,
  >       "type": "private"
  >     },
  >     "text": "Tengo un problema con mi cuota"
  >   }
  > }
  > ```

- [ ] 5.5.6 Implementar comandos de Telegram
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | Implementar respuestas a comandos del bot
  > **Comandos a implementar:**
  > - `/start` - Mensaje de bienvenida y explicación del bot
  > - `/help` - Lista de comandos disponibles
  > - `/status` - Mostrar tickets abiertos del usuario
  > - `/nuevo` - Iniciar proceso de creación de ticket
  > **Lógica:**
  > - Detectar si el mensaje comienza con "/"
  > - Parsear comando y parámetros
  > - Ejecutar acción correspondiente
  > - Responder con mensaje formateado en Markdown

- [ ] 5.5.7 Probar integración con Telegram
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8.4 | **Dependencias:** Tareas 5.5.1-5.5.6
  > **Pruebas:**
  > 1. Enviar mensaje de texto al bot → Verificar creación de ticket
  > 2. Enviar comando /start → Verificar mensaje de bienvenida
  > 3. Enviar comando /status → Verificar lista de tickets
  > 4. Enviar comando /help → Verificar lista de comandos
  > 5. Cambiar estado de ticket → Verificar notificación en Telegram
  > 6. Probar con chat_id no permitido (si hay whitelist) → Verificar rechazo
  > 7. Verificar logs de interacciones en op_ticket_logs_interacciones

### 5.6 Pruebas de Integración Multicanal

- [ ] 5.6.1 Probar flujo completo VAPI → API → BD → Notificación
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 3.2 | Simular llamada telefónica completa
  > **Dependencias:** Tareas 5.1.3, 2.3.1.1, 1.2.1, 1.2.2

- [ ] 5.6.2 Probar flujo completo YCloud → API → BD → Respuesta WhatsApp
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 3.2 | Simular mensaje WhatsApp completo
  > **Dependencias:** Tareas 5.2.3, 2.3.1.2, 1.2.1, 1.2.2

- [ ] 5.6.3 Probar flujo completo Telegram → API → BD → Respuesta Telegram
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 3.2 | Simular mensaje desde Telegram completo
  > **Dependencias:** Tareas 5.5.7, 2.3.1.3, 1.2.1, 1.2.2

- [ ] 5.6.4 Probar flujo completo Chat Web → API → BD → Respuesta JSON
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 3.2 | Simular mensaje desde widget web
  > **Dependencias:** Tareas 5.3.3, 2.3.1.3, 1.2.1, 1.2.2

- [ ] 5.6.5 Probar flujo completo Firebase → API → BD → Push Notification
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 3.2 | Simular mensaje desde app móvil
  > **Dependencias:** Tareas 5.4.4, 2.3.1.4, 1.2.1, 1.2.2

- [ ] 5.6.6 Probar validación de cliente duplicado en todos los canales
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4.6.1 | Validar que funcione la prevención de duplicados
  > **Pruebas:**
  > - Cliente con ticket abierto intenta crear otro por VAPI
  > - Cliente con ticket abierto intenta crear otro por WhatsApp
  > - Cliente con ticket abierto intenta crear otro por Chat Web
  > - Cliente con ticket abierto intenta crear otro por Firebase
  > - Verificar que se retorne mensaje de ticket existente en todos los casos

---

## 5. INTEGRACIONES EXTERNAS (ANTERIOR) → design.md § 8

### 5.1 Configuración de VAPI (ANTERIOR)

---

## 6. TESTING Y VALIDACIÓN → design.md § 9

### 6.1 Pruebas Unitarias

- [ ] 6.1.1 Crear pruebas unitarias para TicketValidationService
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.2.1 | **Ubicación:** JELA.API.Tests/Services/
  > Probar: ValidarClienteDuplicado(), casos edge

- [ ] 6.1.2 Crear pruebas unitarias para TicketNotificationService
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.2.2 | **Ubicación:** JELA.API.Tests/Services/
  > Probar: EncolarNotificacion(), reintentos

- [ ] 6.1.3 Crear pruebas unitarias para TicketMetricsService
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.2.3 | **Ubicación:** JELA.API.Tests/Services/
  > Probar: CalcularMetricasDiarias(), agregaciones

### 6.2 Pruebas de Integración

- [ ] 6.2.1 Probar flujo completo VAPI → API → BD
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 3.2 | Simular llamada telefónica completa

- [ ] 6.2.2 Probar flujo completo YCloud → API → BD
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 3.2 | Simular mensaje WhatsApp completo

- [ ] 6.2.3 Probar Background Services
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6 | Validar ejecución programada y procesamiento

### 6.3 Pruebas de UI

- [ ] 6.3.1 Probar TicketsDashboard en Inicio.aspx
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.2 + .kiro/specs/ecosistema-jelabbc/ui-standards.md § 6 | Validar métricas y gráficos integrados en página de inicio
  > **NOTA:** Dashboard ya implementado, solo requiere validación funcional

- [ ] 6.3.2 Probar TicketsPrompts.aspx
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.2 + .kiro/specs/ecosistema-jelabbc/ui-standards.md § 6 | Validar gestión de prompts

- [ ] 6.3.3 Probar TicketsLogs.aspx
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.3 + .kiro/specs/ecosistema-jelabbc/ui-standards.md § 6, § 7 | Validar auditoría y filtros en columnas del grid

### 6.4 Pruebas de Performance

- [ ] 6.4.1 Probar carga de dashboard con 1000+ tickets
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.1 | Validar tiempo de carga < 3 segundos

- [ ] 6.4.2 Probar procesamiento de cola de notificaciones
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6.3 | Validar procesamiento de 100+ notificaciones

---

## 7. DOCUMENTACIÓN → design.md § 10

### 7.1 Documentación Técnica

- [ ] 7.1.1 Documentar endpoints de API en Swagger
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.3 | Agregar descripciones y ejemplos

- [ ] 7.1.2 Crear README de configuración
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8 | Documentar configuración de VAPI, YCloud, Firebase

- [ ] 7.1.3 Documentar Background Services
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6 | Documentar frecuencia y funcionalidad

### 7.2 Documentación de Usuario

- [ ] 7.2.1 Crear guía de usuario para TicketsDashboard
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.2 | Documentar uso de métricas y gráficos en página de inicio

- [ ] 7.2.2 Crear guía de usuario para TicketsPrompts
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.2 | Documentar gestión de prompts

- [ ] 7.2.3 Crear guía de usuario para TicketsLogs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7.3 + .kiro/specs/ecosistema-jelabbc/ui-standards.md § 7 | Documentar auditoría y filtros en columnas del grid

---

## 8. DEPLOYMENT → design.md § 9

### 8.1 Preparación de Ambiente

- [ ] 8.1.1 Configurar variables de ambiente en Azure
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8 | Configurar API Keys de VAPI, YCloud, Firebase

- [ ] 8.1.2 Ejecutar migraciones de BD en producción
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 4 | **Dependencias:** Todas las tareas de sección 1
  > Ejecutar scripts SQL en jela_qa producción

- [ ] 8.1.3 Configurar webhooks en VAPI y YCloud
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 8 | Apuntar webhooks a URL de producción

### 8.2 Deployment de API

- [ ] 8.2.1 Publicar JELA.API a Azure App Service
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5 | **Dependencias:** Todas las tareas de sección 2
  > Publicar con configuración de producción

- [ ] 8.2.2 Verificar Health Checks en producción
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 5.4 | Validar /health/live y /health/ready

- [ ] 8.2.3 Verificar Background Services en producción
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 6 | Validar ejecución programada

### 8.3 Deployment de Frontend

- [ ] 8.3.1 Publicar JelaWeb a Azure App Service
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7 | **Dependencias:** Todas las tareas de sección 4
  > Publicar con configuración de producción

- [ ] 8.3.2 Verificar páginas nuevas en producción
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 7 | Validar acceso y funcionalidad de TicketsPrompts, TicketsLogs y dashboard en Inicio.aspx

### 8.4 Monitoreo Post-Deployment

- [ ] 8.4.1 Configurar Application Insights
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 9 | Configurar logging y métricas

- [ ] 8.4.2 Configurar alertas de errores
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 9 | Alertas para errores críticos

- [ ] 8.4.3 Monitorear primeras 24 horas
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 9 | Validar funcionamiento en producción

---

## 📊 RESUMEN DE PROGRESO

**FASE 1 - Sistema Funcional (ACTUAL):**
- **Total de Tareas:** 127 (120 originales + 7 de Telegram)
- **Completadas:** 45 (Base de datos: 20, Modelos: 2, Servicios: 6, Webhooks: 6, Registro: 2, Dashboard: 3, Chat Widget: 2)
- **En Progreso:** 0
- **Pendientes:** 82

**FASE 2 - Restricciones de Seguridad (FUTURA):**
- **Total de Tareas:** 17
- **Estado:** NO INICIAR hasta completar Fase 1
- **Estimación:** 2-3 semanas adicionales

**Total General:** 144 tareas (127 Fase 1 + 17 Fase 2)
- **Pendientes:** 82

**Progreso por Sección:**
- Base de Datos: 20/20 (100%) - ✅ COMPLETADO: Tabla alterada + 13 tablas creadas + trigger + 3 stored procedures + verificación completa
- API Backend: 16/21 (76%) - ✅ Modelos creados + 6 servicios implementados + 6 webhooks implementados + registro completo
- Background Services: 0/5 (0%)
- Interfaces Web: 3/19 (16%) - Dashboard ya implementado
- Integraciones: 2/24 (8%) - ✅ Chat Widget completado (2 tareas)
- Testing: 0/10 (0%)
- Documentación: 0/6 (0%)
- Deployment: 0/10 (0%)

**Tareas Críticas Agregadas:**
- ✅ **Webhooks detallados:** 5 subtareas para implementar cada canal (VAPI, YCloud, ChatWeb, Firebase)
- ✅ **Configuración VAPI:** 4 tareas para llamadas telefónicas
- ✅ **Configuración YCloud:** 4 tareas para WhatsApp Business
- ✅ **Configuración Chat Web:** 4 tareas para widget de chat
- ✅ **Configuración Firebase:** 5 tareas para app móvil
- ✅ **Pruebas de integración:** 5 tareas para validar flujos completos multicanal

---

## 🎯 ORDEN DE EJECUCIÓN RECOMENDADO

1. **Sprint 1 - Fundamentos (Semanas 1-2):**
   - Sección 1: Base de Datos (tareas 1.1 - 1.4)
   - Sección 2.1: Modelos de Datos (tareas 2.1.1 - 2.1.2)

2. **Sprint 2 - Backend Core (Semanas 3-4):**
   - Sección 2.2: Servicios (tareas 2.2.1 - 2.2.6)
   - Sección 2.3: Endpoints (tareas 2.3.1 - 2.3.4)
   - Sección 2.4: Registro (tareas 2.4.1 - 2.4.2)

3. **Sprint 3 - Background Services e Integraciones (Semanas 5-6):**
   - Sección 3: Background Services (tareas 3.1 - 3.2)
   - Sección 5: Integraciones (tareas 5.1 - 5.3)

4. **Sprint 4 - Frontend y Testing (Semanas 7-8):**
   - Sección 4: Interfaces Web (tareas 4.1 - 4.3)
   - Sección 6: Testing (tareas 6.1 - 6.4)

5. **Sprint 5 - Documentación y Deployment (Semana 9):**
   - Sección 7: Documentación (tareas 7.1 - 7.2)
   - Sección 8: Deployment (tareas 8.1 - 8.4)

---

**Última Actualización:** 18 de Enero de 2026  
**Próxima Revisión:** Al completar cada sprint










---

## 🔮 FASE 2 - TAREAS FUTURAS (POST-IMPLEMENTACIÓN)

### Contexto de Implementación en 2 Fases

**IMPORTANTE:** El sistema se desarrollará en 2 fases claramente diferenciadas:

**FASE 1 (ACTUAL - Tasks 1-8):**
- Azure OpenAI responde **libremente** sin restricciones
- Permite desarrollo y testing rápido
- Validación de funcionalidad core del sistema
- **Objetivo:** Sistema funcional completo con IA sin limitaciones

**FASE 2 (FUTURA - Tasks 9.x):**
- Implementar restricciones de fuentes de información
- Azure OpenAI SOLO responde desde base de conocimiento propia
- NO busca en internet ni usa información externa
- **Objetivo:** Sistema seguro con respuestas controladas

---

### 9. Sistema de Restricciones de Fuentes de Información (FASE 2)

**Prioridad:** FUTURA (después de completar Fase 1)  
**Dependencias:** Tareas 1-8 completadas y sistema funcionando  
**Estimación:** 2-3 semanas  
**Referencia:** ANALISIS-COMPLETO-SISTEMA-AGENTE-IA.md § 13.2

#### 9.1 Base de Datos - Tabla de Restricciones

- [ ] 9.1.1 Crear tabla conf_ia_source_restrictions
  > **Ref:** ANALISIS-COMPLETO § 13.2 | Tabla para definir fuentes permitidas/prohibidas para la IA
  
  ```sql
  CREATE TABLE conf_ia_source_restrictions (
      Id INT PRIMARY KEY AUTO_INCREMENT,
      Nombre VARCHAR(100) NOT NULL,
      TipoFuente VARCHAR(50) NOT NULL COMMENT 'Manual, BaseDatos, APIInterna, Prohibida',
      Descripcion TEXT,
      Activo BOOLEAN DEFAULT TRUE,
      FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
      FechaModificacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
      IdUsuarioCreacion INT DEFAULT 1,
      IdUsuarioModificacion INT DEFAULT 1,
      
      INDEX idx_tipo (TipoFuente),
      INDEX idx_activo (Activo),
      FOREIGN KEY (IdUsuarioCreacion) REFERENCES conf_usuarios(Id),
      FOREIGN KEY (IdUsuarioModificacion) REFERENCES conf_usuarios(Id)
  ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE utf8mb4_unicode_ci
  COMMENT='Configuración de restricciones de fuentes para IA';
  ```

- [ ] 9.1.2 Insertar datos iniciales de restricciones
  > Configurar fuentes permitidas (manuales internos, BD) y prohibidas (web, APIs externas)
  
  ```sql
  INSERT INTO conf_ia_source_restrictions (Nombre, TipoFuente, Descripcion, Activo) VALUES
  ('Manuales Internos', 'Manual', 'Base de conocimiento interna del sistema', TRUE),
  ('Datos del Residente', 'BaseDatos', 'Solo del usuario autenticado', TRUE),
  ('Información del Ticket', 'BaseDatos', 'Del ticket actual', TRUE),
  ('Políticas Internas', 'Manual', 'Configuración de la compañía', TRUE),
  ('Búsqueda Web', 'Prohibida', 'NUNCA usar navegadores externos', FALSE),
  ('Datos de Otros Residentes', 'Prohibida', 'NUNCA acceder a info de otros', FALSE),
  ('APIs Externas', 'Prohibida', 'NUNCA conectar a servicios externos', FALSE);
  ```

#### 9.2 Backend - Servicio de Gestión de Prompts con Restricciones

- [ ] 9.2.1 Crear PromptManagerService.cs en JELA.API
  > **Ref:** ANALISIS-COMPLETO § 13.2 | Servicio para cargar prompts con restricciones aplicadas
  
  **Ubicación:** `JELA.API/Services/PromptManagerService.cs`
  
  **Responsabilidades:**
  - Cargar prompts desde `conf_ticket_prompts`
  - Obtener restricciones activas desde `conf_ia_source_restrictions`
  - Inyectar restricciones en el system message de OpenAI
  - Validar que las respuestas cumplan restricciones
  
  **Métodos principales:**
  - `Task<string> CargarPromptConRestricciones(string tipoPrompt)`
  - `Task<List<RestriccionFuente>> ObtenerRestricionesActivas()`
  - `string GenerarSystemMessageConRestricciones(string promptBase, List<RestriccionFuente> restricciones)`

- [ ] 9.2.2 Crear modelo RestriccionFuente
  > Modelo para representar restricciones de fuentes
  
  **Ubicación:** `JELA.API/Models/RestriccionFuente.cs`
  
  ```csharp
  public record RestriccionFuente(
      int Id,
      string Nombre,
      string TipoFuente,
      string Descripcion,
      bool Activo
  );
  ```

- [ ] 9.2.3 Actualizar AzureOpenAIService para usar PromptManagerService
  > Modificar servicio existente para aplicar restricciones en todas las llamadas a OpenAI
  
  **Cambios:**
  - Inyectar `IPromptManagerService` en constructor
  - Modificar método `GenerarRespuesta()` para usar prompts con restricciones
  - Agregar validación de respuestas (detectar si violó restricciones)
  - Registrar violaciones en logs

#### 9.3 Backend - Sistema de Aislamiento de Datos

- [ ] 9.3.1 Crear DataIsolationContext.cs
  > **Ref:** ANALISIS-COMPLETO § 13.3 | Contexto para garantizar que IA solo accede a datos del usuario actual
  
  **Ubicación:** `JELA.API/Services/DataIsolationContext.cs`
  
  **Responsabilidades:**
  - Validar que ticket pertenece al usuario autenticado
  - Crear contexto restringido con solo datos permitidos
  - Prevenir acceso a datos de otros residentes
  
  **Métodos principales:**
  - `static Task<DataIsolationContext> CrearContextoRestringido(int idUsuario, int idTicket)`
  - `Task<Dictionary<string, object>> ObtenerDatosPermitidos()`
  - `Task ValidarAcceso(int idUsuario, int idTicket)`

- [ ] 9.3.2 Integrar DataIsolationContext en WebhookEndpoints
  > Aplicar aislamiento de datos en todos los webhooks (VAPI, YCloud, ChatWeb)
  
  **Cambios:**
  - Crear contexto de aislamiento antes de procesar con IA
  - Pasar solo datos permitidos al prompt
  - Validar que respuesta no incluye datos de otros usuarios

#### 9.4 Configuración de Prompts del Sistema

- [ ] 9.4.1 Actualizar prompts en conf_ticket_prompts con restricciones
  > Modificar prompts existentes para incluir restricciones explícitas
  
  **Formato del prompt con restricciones:**
  ```
  [PROMPT BASE]
  
  === RESTRICCIONES CRÍTICAS (NO SE PUEDEN VIOLAR) ===
  
  ✗ PROHIBIDO ABSOLUTAMENTE:
    - Usar información de navegación web o búsqueda externa
    - Usar datos de otros residentes que no sean el usuario actual
    - Inventar información no documentada en manuales
    - Revelar información de otros tickets o residentes
    - Incluir links externos en respuestas
    - Usar APIs externas no autorizadas
    - Generar respuestas que no estén en los manuales
  
  ✓ PERMITIDO SOLAMENTE:
    - Manuales Internos (Base de conocimiento interna)
    - Datos del Residente (Solo del usuario autenticado)
    - Información del Ticket (Del ticket actual)
    - Políticas Internas (Configuración de la compañía)
  
  REGLA DE RESPUESTA CUANDO NO ENCUENTRES INFO:
  Si la información NO ESTÁ en los manuales internos, DEBES responder:
  "No tengo esta información en nuestros manuales internos. Voy a escalar tu 
  solicitud a un agente humano especializado que podrá ayudarte."
  
  NUNCA inventar, asumir o buscar en internet.
  ```

- [ ] 9.4.2 Crear prompts específicos para validación de restricciones
  > Prompts para detectar si una respuesta violó restricciones
  
  **Tipos de prompts:**
  - Prompt de validación de fuentes
  - Prompt de detección de información externa
  - Prompt de verificación de aislamiento de datos

#### 9.5 Testing de Restricciones

- [ ] 9.5.1 Crear tests de restricciones de fuentes
  > Verificar que IA NO busca en internet ni usa información externa
  
  **Casos de prueba:**
  - Test: Pregunta sobre información no en manuales → Debe escalar a humano
  - Test: Pregunta sobre otro residente → Debe rechazar acceso
  - Test: Solicitud de búsqueda web → Debe rechazar
  - Test: Pregunta sobre información en manuales → Debe responder correctamente

- [ ] 9.5.2 Crear tests de aislamiento de datos
  > Verificar que IA solo accede a datos del usuario autenticado
  
  **Casos de prueba:**
  - Test: Usuario A intenta acceder a ticket de Usuario B → Debe rechazar
  - Test: Usuario A accede a su propio ticket → Debe permitir
  - Test: Respuesta incluye datos de otro usuario → Debe detectar y rechazar

- [ ] 9.5.3 Crear tests de validación de respuestas
  > Verificar que respuestas cumplen con restricciones
  
  **Casos de prueba:**
  - Test: Respuesta incluye URL externa → Debe detectar violación
  - Test: Respuesta menciona información no en manuales → Debe detectar
  - Test: Respuesta incluye datos de otro residente → Debe detectar

#### 9.6 Interfaz Web - Gestión de Restricciones

- [ ] 9.6.1 Agregar sección de restricciones en TicketsPrompts.aspx
  > Permitir configurar restricciones de fuentes desde interfaz web
  
  **Funcionalidades:**
  - Grid con restricciones activas/inactivas
  - Popup para agregar/editar restricciones
  - Activar/desactivar restricciones
  - Ver historial de cambios

- [ ] 9.6.2 Agregar indicadores de restricciones en TicketsLogs.aspx
  > Mostrar si una interacción violó restricciones
  
  **Indicadores:**
  - Columna "Restricciones Cumplidas" (Sí/No)
  - Detalle de violaciones detectadas
  - Filtro por violaciones de restricciones

#### 9.7 Documentación

- [ ] 9.7.1 Documentar sistema de restricciones
  > Crear documentación técnica del sistema de restricciones
  
  **Contenido:**
  - Arquitectura del sistema de restricciones
  - Cómo agregar nuevas restricciones
  - Cómo configurar prompts con restricciones
  - Troubleshooting de violaciones

- [ ] 9.7.2 Crear guía de migración de Fase 1 a Fase 2
  > Documentar proceso de activación de restricciones
  
  **Contenido:**
  - Checklist de pre-migración
  - Pasos para activar restricciones
  - Validación post-migración
  - Plan de rollback si hay problemas

---

## 9. EXPANSIÓN CHAT WEB AVANZADO → design.md § 11

**Referencia:** `.kiro/specs/tickets-colaborativos/CHAT-WEB-AVANZADO-EXPANSION.md`

### 9.1 Base de Datos (4 tablas)

- [ ] 9.1.1 Crear tabla conf_chat_actions
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.2.1 | **Script:** SQL migration
  > Catálogo de acciones disponibles en el chat (CRUD, Consultas, Navegación)
  > Campos: NombreAccion, TipoAccion, EndpointAPI, MetodoHTTP, RequiereParametros, RequierePermisos

- [ ] 9.1.2 Crear tabla conf_chat_queries
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.2.2 | **Script:** SQL migration
  > Consultas SQL parametrizadas ejecutables desde el chat
  > Campos: NombreConsulta, QuerySQL, Parametros, TipoResultado, FormatoRespuesta

- [ ] 9.1.3 Crear tabla op_chat_history
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.2.3 | **Script:** SQL migration
  > Historial completo de conversaciones para auditoría
  > Campos: SessionId, Mensaje, AccionEjecutada, ParametrosUsados, ResultadoExitoso

- [ ] 9.1.4 Crear tabla op_chat_confirmations
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.2.4 | **Script:** SQL migration
  > Confirmaciones pendientes de acciones críticas
  > Campos: AccionPendiente, ParametrosAccion, Estado (PENDIENTE, CONFIRMADO, CANCELADO)

### 9.2 Servicios Backend (.NET 8)

- [ ] 9.2.1 Implementar ChatOrchestratorService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.3.1 | **Ubicación:** JELA.API/Services/ChatOrchestratorService.cs
  > **Dependencias:** Tarea 9.1 (todas las tablas)
  > Coordina el flujo completo de procesamiento de mensajes
  > Métodos: ProcessMessageAsync(), ConfirmActionAsync()

- [ ] 9.2.2 Implementar ChatActionService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.3.2 | **Ubicación:** JELA.API/Services/ChatActionService.cs
  > **Dependencias:** Tarea 9.1.1
  > Ejecuta acciones CRUD y valida permisos
  > Métodos: GetAvailableFunctionsAsync(), ValidatePermissionsAsync(), ExecuteActionAsync()

- [ ] 9.2.3 Implementar ChatQueryService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.3.3 | **Ubicación:** JELA.API/Services/ChatQueryService.cs
  > **Dependencias:** Tarea 9.1.2
  > Ejecuta consultas dinámicas configuradas en BD
  > Métodos: ExecuteQueryAsync(), GetAvailableQueriesAsync()

- [ ] 9.2.4 Implementar ChatHistoryService.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.3.4 | **Ubicación:** JELA.API/Services/ChatHistoryService.cs
  > **Dependencias:** Tarea 9.1.3
  > Gestiona historial de conversaciones
  > Métodos: RegisterAsync(), GetHistoryAsync(), GetUserHistoryAsync()

### 9.3 Endpoints de API

- [ ] 9.3.1 Implementar ChatEndpoints.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.4 | **Ubicación:** JELA.API/Endpoints/ChatEndpoints.cs
  > **Dependencias:** Tareas 9.2.1, 9.2.2, 9.2.3, 9.2.4
  > Endpoints: POST /api/chat/process, POST /api/chat/confirm, GET /api/chat/history/{sessionId}

  - [ ] 9.3.1.1 Implementar endpoint POST /api/chat/process
    > Procesa mensajes del chat y ejecuta acciones mediante Azure OpenAI Function Calling

  - [ ] 9.3.1.2 Implementar endpoint POST /api/chat/confirm
    > Confirma o cancela acciones pendientes

  - [ ] 9.3.1.3 Implementar endpoint GET /api/chat/history/{sessionId}
    > Obtiene el historial de una sesión de chat

### 9.4 Mejoras en Widget de Chat

- [ ] 9.4.1 Agregar soporte para confirmaciones en chat-widget.js
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.5 | **Ubicación:** JelaWeb/Scripts/widgets/chat-widget.js
  > **Dependencias:** Tarea 9.3.1.2
  > Funcionalidad: Mostrar botones de confirmación (Confirmar, Cancelar, Modificar)
  > Métodos: mostrarConfirmacion(), confirmarAccion()

- [ ] 9.4.2 Agregar soporte para tablas interactivas en chat-widget.js
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.5 | **Ubicación:** JelaWeb/Scripts/widgets/chat-widget.js
  > Funcionalidad: Renderizar tablas con datos y acciones
  > Métodos: mostrarTabla(), ejecutarAccionTabla()

- [ ] 9.4.3 Agregar soporte para navegación en chat-widget.js
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.5 | **Ubicación:** JelaWeb/Scripts/widgets/chat-widget.js
  > Funcionalidad: Redirigir a páginas del sistema
  > Métodos: manejarNavegacion(), abrirUrl()

- [ ] 9.4.4 Actualizar estilos en chat-widget.css
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.5 | **Ubicación:** JelaWeb/Content/CSS/chat-widget.css
  > Agregar estilos para: confirmaciones, tablas interactivas, botones de navegación

### 9.5 Configuración Inicial

- [ ] 9.5.1 Insertar acciones CRUD iniciales en conf_chat_actions
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.7
  > Acciones: crear_unidad, actualizar_unidad, eliminar_unidad, crear_residente, etc.
  > Mínimo 10 acciones CRUD para catálogos principales

- [ ] 9.5.2 Insertar consultas dinámicas iniciales en conf_chat_queries
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.7
  > Consultas: estado_cuenta_unidad, tickets_abiertos_usuario, total_ingresos_mes, etc.
  > Mínimo 10 consultas para operaciones comunes

- [ ] 9.5.3 Insertar acciones de navegación en conf_chat_actions
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.7
  > Navegación: navegar_unidades, navegar_pagos, navegar_tickets, navegar_residentes, etc.
  > Mínimo 8 acciones de navegación para módulos principales

### 9.6 Registro de Servicios

- [ ] 9.6.1 Registrar servicios de Chat en Program.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.3 | **Ubicación:** JELA.API/Program.cs
  > Agregar: builder.Services.AddScoped<IChatOrchestratorService, ChatOrchestratorService>(), etc.
  > Registrar los 4 servicios de chat

- [ ] 9.6.2 Registrar endpoints de Chat en Program.cs
  > **Ref:** .kiro/specs/tickets-colaborativos/design.md § 11.4 | **Ubicación:** JELA.API/Program.cs
  > Agregar: app.MapChatEndpoints()

### 9.7 Pruebas y Validación

- [ ] 9.7.1 Probar acciones CRUD mediante chat
  > Validar creación, actualización, eliminación de registros
  > Probar al menos 5 acciones CRUD diferentes

- [ ] 9.7.2 Probar consultas dinámicas mediante chat
  > Validar ejecución de queries y formateo de resultados
  > Probar consultas tipo GRID, VALOR_UNICO, LISTA

- [ ] 9.7.3 Probar navegación mediante chat
  > Validar redirección a páginas del sistema
  > Probar apertura en misma pestaña y nueva pestaña

- [ ] 9.7.4 Probar sistema de confirmaciones
  > Validar flujo completo: solicitud → confirmación → ejecución
  > Probar confirmación, cancelación y modificación

- [ ] 9.7.5 Probar validación de permisos
  > Validar que usuarios sin permisos no puedan ejecutar acciones
  > Probar con diferentes roles de usuario

### 9.8 Documentación

- [ ] 9.8.1 Documentar configuración de acciones
  > Crear guía para agregar nuevas acciones en conf_chat_actions
  > Incluir ejemplos de acciones CRUD, consultas y navegación

- [ ] 9.8.2 Documentar configuración de consultas
  > Crear guía para agregar nuevas consultas en conf_chat_queries
  > Incluir ejemplos de queries parametrizados y formateo de respuestas

- [ ] 9.8.3 Crear manual de usuario del chat avanzado
  > Documentar comandos disponibles y ejemplos de uso
  > Incluir capturas de pantalla y casos de uso comunes

---

### 📊 RESUMEN DE EXPANSIÓN CHAT WEB AVANZADO

**Total de Tareas:** 28 tareas
**Estimación:** 6-10 días de desarrollo
**Dependencias:** Sistema de tickets base funcional

**Componentes Principales:**
1. ✅ 4 tablas nuevas (conf_chat_actions, conf_chat_queries, op_chat_history, op_chat_confirmations)
2. ✅ 4 servicios backend (.NET 8)
3. ✅ 3 endpoints de API
4. ✅ Mejoras en widget de chat (confirmaciones, tablas, navegación)
5. ✅ Configuración inicial (acciones, consultas, navegación)

**Criterios de Éxito:**
- ✅ Usuarios pueden crear/actualizar/eliminar registros mediante lenguaje natural
- ✅ Usuarios pueden consultar información mediante lenguaje natural
- ✅ Usuarios pueden navegar a páginas mediante lenguaje natural
- ✅ Sistema solicita confirmación para acciones críticas
- ✅ Sistema valida permisos correctamente
- ✅ 90% de comandos ejecutados correctamente

**Impacto Esperado:**
- Reducción del 60% en tiempo de operaciones comunes
- 50% de usuarios activos usando el chat semanalmente
- 4.5/5 estrellas en satisfacción de usuario

---

## 10. FASE 2: RESTRICCIONES DE SEGURIDAD IA (OPCIONAL)

**Total de Tareas Fase 2:** 17 tareas
**Estimación:** 2-3 semanas
**Dependencias:** Sistema Fase 1 completamente funcional

**Componentes Principales:**
1. ✅ Tabla `conf_ia_source_restrictions`
2. ✅ Servicio `PromptManagerService`
3. ✅ Clase `DataIsolationContext`
4. ✅ Actualización de prompts con restricciones
5. ✅ Testing exhaustivo de restricciones
6. ✅ Interfaz web para gestión
7. ✅ Documentación completa

**Criterios de Éxito Fase 2:**
- ✅ IA NUNCA busca en internet
- ✅ IA SOLO responde desde manuales internos
- ✅ IA NUNCA accede a datos de otros usuarios
- ✅ Sistema detecta y registra violaciones
- ✅ Tests de restricciones pasan al 100%

---

## 11. SEGURIDAD Y CONTROL DE COSTOS IA (CRÍTICO) 🚨

**Prioridad:** ALTA - Implementar ANTES de producción  
**Referencia:** [Documento de Recomendaciones de Seguridad](https://docs.google.com/document/d/1--JFUIpHGlLSzZFUDO5TAyLcLUstioTQ9AoFSCSVk7k/edit?tab=t.0)  
**Riesgo Financiero:** $500-$7,000 USD/mes sin estos controles  
**Estimación:** 3-5 días de desarrollo

### 11.1 Base de Datos - Control de Costos

- [ ] 11.1.1 Crear tabla op_ticket_cost_control
  > **Ref:** Documento de Recomendaciones § 1 | **Script:** SQL migration
  > Tabla para configurar límites de costos y presupuestos
  
  ```sql
  CREATE TABLE op_ticket_cost_control (
      Id INT PRIMARY KEY AUTO_INCREMENT,
      IdEntidad INT NOT NULL DEFAULT 1,
      MaxTokensPerRequest INT DEFAULT 1500 COMMENT 'Máximo de tokens por solicitud',
      DailyBudgetUSD DECIMAL(10,2) DEFAULT 100.00 COMMENT 'Presupuesto diario en USD',
      MonthlyBudgetUSD DECIMAL(10,2) DEFAULT 2000.00 COMMENT 'Presupuesto mensual en USD',
      AlertThresholdPercent INT DEFAULT 80 COMMENT 'Porcentaje para alertas (80%)',
      CurrentDailySpendUSD DECIMAL(10,2) DEFAULT 0.00 COMMENT 'Gasto actual del día',
      CurrentMonthlySpendUSD DECIMAL(10,2) DEFAULT 0.00 COMMENT 'Gasto actual del mes',
      LastResetDate DATE DEFAULT CURRENT_DATE COMMENT 'Última fecha de reset',
      SystemPaused BOOLEAN DEFAULT FALSE COMMENT 'Sistema pausado por exceso de presupuesto',
      FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
      FechaUltimaActualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
      Activo TINYINT(1) DEFAULT 1,
      PRIMARY KEY (Id),
      INDEX idx_cost_control_entidad (IdEntidad),
      FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
  ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Control de costos y presupuestos de IA';
  ```

- [ ] 11.1.2 Insertar configuración inicial de control de costos
  > Configurar límites conservadores por defecto
  
  ```sql
  INSERT INTO op_ticket_cost_control (IdEntidad, MaxTokensPerRequest, DailyBudgetUSD, MonthlyBudgetUSD, AlertThresholdPercent) 
  VALUES (1, 1500, 100.00, 2000.00, 80);
  ```

- [ ] 11.1.3 Crear tabla op_ticket_cost_tracking
  > **Ref:** Documento de Recomendaciones § 1 | **Script:** SQL migration
  > Tracking detallado de costos por solicitud
  
  ```sql
  CREATE TABLE op_ticket_cost_tracking (
      Id INT PRIMARY KEY AUTO_INCREMENT,
      IdEntidad INT NOT NULL DEFAULT 1,
      IdTicket INT DEFAULT NULL COMMENT 'FK a op_tickets_v2',
      Canal VARCHAR(50) NOT NULL COMMENT 'VAPI, YCloud, ChatWeb, Firebase, Telegram',
      TokensUtilizados INT NOT NULL,
      CostoUSD DECIMAL(10,4) NOT NULL COMMENT 'Costo en USD',
      ModeloUtilizado VARCHAR(100) DEFAULT 'gpt-4o-mini',
      FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
      PRIMARY KEY (Id),
      INDEX idx_cost_tracking_ticket (IdTicket),
      INDEX idx_cost_tracking_fecha (FechaCreacion),
      INDEX idx_cost_tracking_canal (Canal),
      INDEX idx_cost_tracking_entidad (IdEntidad),
      FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
      FOREIGN KEY (IdTicket) REFERENCES op_tickets_v2(Id) ON DELETE SET NULL
  ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
  COMMENT='Tracking detallado de costos de IA por solicitud';
  ```

### 11.2 Backend - Circuit Breaker y Reintentos

- [ ] 11.2.1 Instalar paquete Polly para Circuit Breaker
  > **Ref:** Documento de Recomendaciones § 4 | **Ubicación:** JELA.API/JELA.API.csproj
  > Agregar paquete NuGet: `Polly` y `Polly.Extensions.Http`
  
  ```bash
  dotnet add package Polly
  dotnet add package Polly.Extensions.Http
  ```

- [ ] 11.2.2 Implementar CircuitBreakerService.cs
  > **Ref:** Documento de Recomendaciones § 4 | **Ubicación:** JELA.API/Services/CircuitBreakerService.cs
  > Servicio para manejar Circuit Breaker de Azure OpenAI
  
  **Funcionalidad:**
  - Circuit Breaker con 5 fallos consecutivos para abrir
  - Timeout de 30 segundos por solicitud
  - Fallback a respuestas predefinidas cuando el circuito está abierto
  - Logging de estado del circuito
  
  **Métodos principales:**
  - `Task<string> ExecuteWithCircuitBreaker(Func<Task<string>> action)`
  - `Task<string> GetFallbackResponse(string tipoTicket)`
  - `CircuitState GetCircuitState()`

- [ ] 11.2.3 Implementar RetryPolicyService.cs
  > **Ref:** Documento de Recomendaciones § 3 | **Ubicación:** JELA.API/Services/RetryPolicyService.cs
  > Servicio para manejar reintentos con exponential backoff
  
  **Funcionalidad:**
  - Máximo 3 reintentos
  - Exponential backoff: 2s, 4s, 8s
  - Logging de cada reintento
  - Solo reintentar en errores transitorios (429, 503, timeout)
  
  **Métodos principales:**
  - `Task<T> ExecuteWithRetry<T>(Func<Task<T>> action, string operationName)`
  - `bool IsTransientError(Exception ex)`

- [ ] 11.2.4 Actualizar AzureOpenAIService para usar Circuit Breaker
  > **Ref:** Documento de Recomendaciones § 4 | **Ubicación:** JELA.API/Services/AzureOpenAIService.cs
  > Integrar CircuitBreakerService y RetryPolicyService
  
  **Cambios:**
  - Inyectar `ICircuitBreakerService` y `IRetryPolicyService` en constructor
  - Envolver todas las llamadas a OpenAI con Circuit Breaker
  - Agregar timeout de 30 segundos
  - Implementar fallback cuando el circuito está abierto
  - Registrar estado del circuito en logs

### 11.3 Backend - Control de Costos en Tiempo Real

- [ ] 11.3.1 Implementar CostControlService.cs
  > **Ref:** Documento de Recomendaciones § 1, 2 | **Ubicación:** JELA.API/Services/CostControlService.cs
  > **Dependencias:** Tareas 11.1.1, 11.1.3
  > Servicio para control de costos en tiempo real
  
  **Funcionalidad:**
  - Validar presupuesto antes de cada solicitud a OpenAI
  - Registrar costo después de cada solicitud
  - Pausar sistema automáticamente al alcanzar 100% del presupuesto
  - Enviar alertas al alcanzar 80% del presupuesto
  - Reset automático de contadores diarios/mensuales
  
  **Métodos principales:**
  - `Task<bool> CanProcessRequest(int idEntidad, int estimatedTokens)`
  - `Task RegisterCost(int idEntidad, int idTicket, string canal, int tokensUsed, decimal costUSD)`
  - `Task<CostStatus> GetCostStatus(int idEntidad)`
  - `Task SendBudgetAlert(int idEntidad, decimal percentUsed)`
  - `Task ResetDailyCounters()`

- [ ] 11.3.2 Implementar TokenLimitService.cs
  > **Ref:** Documento de Recomendaciones § 2 | **Ubicación:** JELA.API/Services/TokenLimitService.cs
  > **Dependencias:** Tarea 11.1.1
  > Servicio para limitar tokens por solicitud
  
  **Funcionalidad:**
  - Truncar prompts que excedan el límite de tokens
  - Estimar tokens antes de enviar a OpenAI
  - Configurar límite máximo por tipo de solicitud
  
  **Métodos principales:**
  - `int EstimateTokens(string text)`
  - `string TruncateToTokenLimit(string text, int maxTokens)`
  - `Task<int> GetMaxTokensForEntity(int idEntidad)`

- [ ] 11.3.3 Actualizar WebhookEndpoints para validar presupuesto
  > **Ref:** Documento de Recomendaciones § 1 | **Ubicación:** JELA.API/Endpoints/WebhookEndpoints.cs
  > Agregar validación de presupuesto antes de procesar con IA
  
  **Cambios:**
  - Inyectar `ICostControlService` en constructor
  - Validar presupuesto antes de llamar a OpenAI: `await _costControl.CanProcessRequest(idEntidad, estimatedTokens)`
  - Si presupuesto excedido, retornar mensaje de error y NO procesar
  - Registrar costo después de cada llamada a OpenAI
  - Agregar header `X-Cost-USD` en respuesta con el costo de la solicitud

- [ ] 11.3.4 Actualizar AzureOpenAIService para limitar tokens
  > **Ref:** Documento de Recomendaciones § 2 | **Ubicación:** JELA.API/Services/AzureOpenAIService.cs
  > Agregar límite de tokens por solicitud
  
  **Cambios:**
  - Inyectar `ITokenLimitService` en constructor
  - Truncar prompts que excedan el límite antes de enviar
  - Configurar `max_tokens` en la solicitud a OpenAI
  - Registrar tokens utilizados en logs

### 11.4 Background Services - Monitoreo de Costos

- [ ] 11.4.1 Implementar CostMonitoringBackgroundService.cs
  > **Ref:** Documento de Recomendaciones § 3 | **Ubicación:** JELA.API/BackgroundServices/CostMonitoringBackgroundService.cs
  > **Dependencias:** Tareas 11.3.1, 11.1.1, 11.1.3
  > Background Service para monitoreo de costos cada hora
  
  **Funcionalidad:**
  - Ejecutar cada hora
  - Calcular gasto actual del día y del mes
  - Actualizar tabla `op_ticket_cost_control`
  - Enviar alertas si se alcanza 80% del presupuesto
  - Pausar sistema si se alcanza 100% del presupuesto
  - Reset automático de contadores diarios a las 00:00
  - Reset automático de contadores mensuales el día 1
  
  **Métodos principales:**
  - `Task ExecuteAsync(CancellationToken stoppingToken)`
  - `Task CalculateCurrentSpend(int idEntidad)`
  - `Task CheckBudgetThresholds(int idEntidad)`
  - `Task SendAlert(int idEntidad, string alertType, decimal percentUsed)`

- [ ] 11.4.2 Registrar CostMonitoringBackgroundService en Program.cs
  > **Ref:** Documento de Recomendaciones § 3 | **Ubicación:** JELA.API/Program.cs
  > Agregar: `builder.Services.AddHostedService<CostMonitoringBackgroundService>()`

### 11.5 Rate Limiting Avanzado

- [ ] 11.5.1 Implementar rate limiting por canal
  > **Ref:** Documento de Recomendaciones § 5 | **Ubicación:** JELA.API/Program.cs
  > Configurar límites específicos por canal
  
  **Configuración:**
  - VAPI: 20 solicitudes/minuto
  - YCloud: 30 solicitudes/minuto
  - Telegram: 30 solicitudes/minuto
  - ChatWeb: 50 solicitudes/minuto
  - Firebase: 40 solicitudes/minuto
  
  ```csharp
  builder.Services.AddRateLimiter(options =>
  {
      options.AddPolicy("vapi", context => 
          RateLimitPartition.GetFixedWindowLimiter(
              partitionKey: "vapi",
              factory: _ => new FixedWindowRateLimiterOptions
              {
                  PermitLimit = 20,
                  Window = TimeSpan.FromMinutes(1)
              }));
      // ... configurar otros canales
  });
  ```

- [ ] 11.5.2 Implementar rate limiting por usuario
  > **Ref:** Documento de Recomendaciones § 5 | **Ubicación:** JELA.API/Program.cs
  > Limitar solicitudes por usuario/teléfono/email
  
  **Configuración:**
  - 10 solicitudes por usuario por hora
  - Usar teléfono/email/IP como clave de partición
  - Retornar 429 Too Many Requests cuando se exceda

### 11.6 Alertas y Notificaciones

- [ ] 11.6.1 Implementar AlertService.cs
  > **Ref:** Documento de Recomendaciones § 3 | **Ubicación:** JELA.API/Services/AlertService.cs
  > Servicio para enviar alertas de costos
  
  **Funcionalidad:**
  - Enviar email cuando se alcanza 80% del presupuesto
  - Enviar WhatsApp cuando se alcanza 90% del presupuesto
  - Enviar notificación crítica cuando se alcanza 100%
  - Enviar alerta cuando el Circuit Breaker se abre
  - Logging de todas las alertas enviadas
  
  **Métodos principales:**
  - `Task SendBudgetAlert(int idEntidad, decimal percentUsed, string alertType)`
  - `Task SendCircuitBreakerAlert(string serviceName, CircuitState state)`
  - `Task SendSystemPausedAlert(int idEntidad)`

- [ ] 11.6.2 Configurar destinatarios de alertas en appsettings.json
  > **Ref:** Documento de Recomendaciones § 3 | **Ubicación:** JELA.API/appsettings.json
  
  ```json
  "Alerts": {
    "Email": {
      "Enabled": true,
      "Recipients": ["admin@jelabbc.com", "dev@jelabbc.com"]
    },
    "WhatsApp": {
      "Enabled": true,
      "PhoneNumbers": ["+52XXXXXXXXXX"]
    },
    "Slack": {
      "Enabled": false,
      "WebhookUrl": ""
    }
  }
  ```

### 11.7 Testing de Seguridad

- [ ] 11.7.1 Crear tests de Circuit Breaker
  > **Ref:** Documento de Recomendaciones § 4 | **Ubicación:** JELA.API.Tests/Services/
  > Probar: Apertura del circuito, fallback, recuperación
  
  **Casos de prueba:**
  - Test: 5 fallos consecutivos → Circuito se abre
  - Test: Circuito abierto → Retorna respuesta de fallback
  - Test: Después de timeout → Circuito intenta cerrarse
  - Test: Solicitud exitosa → Circuito se cierra

- [ ] 11.7.2 Crear tests de control de costos
  > **Ref:** Documento de Recomendaciones § 1, 2 | **Ubicación:** JELA.API.Tests/Services/
  > Probar: Validación de presupuesto, registro de costos, alertas
  
  **Casos de prueba:**
  - Test: Presupuesto disponible → Permite solicitud
  - Test: Presupuesto excedido → Rechaza solicitud
  - Test: 80% del presupuesto → Envía alerta
  - Test: 100% del presupuesto → Pausa sistema
  - Test: Reset diario → Reinicia contadores

- [ ] 11.7.3 Crear tests de reintentos
  > **Ref:** Documento de Recomendaciones § 3 | **Ubicación:** JELA.API.Tests/Services/
  > Probar: Reintentos con exponential backoff
  
  **Casos de prueba:**
  - Test: Error transitorio → Reintenta 3 veces
  - Test: Error permanente → No reintenta
  - Test: Exponential backoff → Espera 2s, 4s, 8s
  - Test: Éxito en 2do intento → Retorna resultado

- [ ] 11.7.4 Crear tests de límite de tokens
  > **Ref:** Documento de Recomendaciones § 2 | **Ubicación:** JELA.API.Tests/Services/
  > Probar: Truncado de prompts, estimación de tokens
  
  **Casos de prueba:**
  - Test: Prompt > 1500 tokens → Trunca a 1500
  - Test: Prompt < 1500 tokens → No trunca
  - Test: Estimación de tokens → Precisión ±10%

### 11.8 Documentación

- [ ] 11.8.1 Documentar configuración de control de costos
  > Crear README con instrucciones de configuración
  
  **Contenido:**
  - Cómo configurar límites de presupuesto
  - Cómo configurar alertas
  - Cómo pausar/reanudar el sistema manualmente
  - Cómo interpretar métricas de costos

- [ ] 11.8.2 Documentar Circuit Breaker y reintentos
  > Crear documentación técnica del patrón implementado
  
  **Contenido:**
  - Cómo funciona el Circuit Breaker
  - Cuándo se abre/cierra el circuito
  - Respuestas de fallback disponibles
  - Cómo monitorear el estado del circuito

---

### 📊 RESUMEN DE SEGURIDAD Y CONTROL DE COSTOS

**Total de Tareas:** 26 tareas críticas
**Estimación:** 3-5 días de desarrollo
**Prioridad:** ALTA - Implementar ANTES de producción

**Componentes Principales:**
1. ✅ 2 tablas nuevas (op_ticket_cost_control, op_ticket_cost_tracking)
2. ✅ 5 servicios nuevos (CircuitBreaker, RetryPolicy, CostControl, TokenLimit, Alert)
3. ✅ 1 Background Service (CostMonitoring cada hora)
4. ✅ Rate limiting avanzado por canal y por usuario
5. ✅ Sistema de alertas (Email, WhatsApp)
6. ✅ Testing exhaustivo de seguridad

**Criterios de Éxito:**
- ✅ Sistema NUNCA excede presupuesto configurado
- ✅ Alertas se envían al alcanzar 80% del presupuesto
- ✅ Sistema se pausa automáticamente al alcanzar 100%
- ✅ Circuit Breaker previene cascadas de fallos
- ✅ Reintentos controlados con exponential backoff
- ✅ Límite de tokens por solicitud respetado
- ✅ Rate limiting por canal funciona correctamente
- ✅ Tests de seguridad pasan al 100%

**Impacto Esperado:**
- Reducción de 90% en riesgo de explosión de costos
- Prevención de bucles infinitos de procesamiento
- Protección contra fallos en cascada
- Visibilidad completa de costos en tiempo real
- Alertas tempranas antes de exceder presupuesto

**Riesgo Mitigado:**
- ❌ Sin controles: $500-$7,000 USD/mes de riesgo
- ✅ Con controles: Costo predecible y controlado

---

### ⚠️ NOTA IMPORTANTE

**NO iniciar Fase 2 hasta que:**
1. ✅ Todas las tareas 1-8 estén completadas
2. ✅ Sistema esté funcionando correctamente en producción
3. ✅ Se haya validado funcionalidad core con usuarios reales
4. ✅ Se tenga base de conocimiento (manuales internos) lista para alimentar al sistema

**Razón:** Es más eficiente desarrollar y validar el sistema completo primero, y luego agregar las restricciones de seguridad una vez que todo funciona correctamente.

---

### ⚠️ ADVERTENCIA CRÍTICA - SEGURIDAD Y COSTOS

**IMPLEMENTAR SECCIÓN 11 (Seguridad y Control de Costos) ANTES DE PRODUCCIÓN**

Sin estos controles, el sistema tiene los siguientes riesgos:

1. **Bucles Infinitos de Procesamiento** → $500-$2,000 USD/mes
2. **Explosión de Costos por Tokens** → $1,000-$5,000 USD/mes
3. **Fallos en Cascada** → Sistema completamente inoperante
4. **Sin Visibilidad de Costos** → Sorpresas en la factura de Azure

**Total de Riesgo Financiero:** $1,500-$7,000 USD/mes

**Implementar tareas 11.1 a 11.8 es OBLIGATORIO antes de desplegar a producción.**

