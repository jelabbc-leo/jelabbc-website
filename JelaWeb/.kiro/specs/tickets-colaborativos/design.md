# DISEÑO TÉCNICO - MÓDULO DE TICKETS COLABORATIVOS CON IA
## Sistema JELABBC - Especificación Completa

**Fecha:** 18 de Enero de 2026  
**Versión:** 2.0  
**Estado:** Listo para Desarrollo  
**Tipo:** Módulo Adicional del Sistema JELABBC

---

## TABLA DE CONTENIDOS

1. [Resumen Ejecutivo](#1-resumen-ejecutivo)
2. [Contexto del Módulo](#2-contexto-del-módulo)
3. [Arquitectura del Sistema](#3-arquitectura-del-sistema)
4. [Base de Datos](#4-base-de-datos)
5. [API Backend (.NET 8)](#5-api-backend-net-8)
6. [Background Services (.NET 8)](#6-background-services-net-8)
7. [Interfaces Web (ASP.NET VB.NET)](#7-interfaces-web-aspnet-vbnet)
8. [Integraciones Externas](#8-integraciones-externas)
9. [Plan de Implementación](#9-plan-de-implementación)
10. [Checklist de Desarrollo](#10-checklist-de-desarrollo)
11. [Expansión Chat Web Avanzado](#11-expansión-chat-web-avanzado)

---

## 1. RESUMEN EJECUTIVO

### 1.1 Objetivo del Módulo

Desarrollar un **módulo adicional** para el sistema JELABBC que permita gestionar tickets de atención al cliente con inteligencia artificial, incluyendo:

- Recepción multicanal (VAPI, YCloud, Telegram, Chat Web, Firebase)
- Procesamiento automático con Azure OpenAI
- Validación de clientes y prevención de duplicados
- Notificaciones automáticas por WhatsApp
- Monitoreo y métricas en tiempo real
- Mejora continua de prompts

### 1.2 Alcance

**Este NO es un sistema independiente**, sino un **módulo que se integra** al ecosistema JELABBC existente:

✅ **Extiende proyectos existentes:**
- JELA.API (.NET 8) - Agregar endpoints y servicios
- JelaWeb (VB.NET) - Agregar páginas ASP.NET
- Base de datos jela_qa - Agregar tablas

✅ **Reutiliza infraestructura:**
- Autenticación JWT existente
- ApiConsumerCRUD.vb
- Logger y helpers compartidos
- Master page y navegación
- Configuraciones globales

✅ **Mantiene consistencia:**
- Mismos estándares UI
- Misma estructura de carpetas
- Mismos patrones de código

### 1.3 Principios de Diseño Obligatorios

**CRÍTICO - Cumplimiento de Estándares:**

1. ✅ **Sin conexiones directas a BD** - Todo a través de JELA.API
2. ✅ **Prefijos de tablas** - `op_` operativas, `conf_` configuración
3. ✅ **Campos PascalCase** - `ChatId`, `FechaCreacion`
4. ✅ **Lógica en JELA.API** - NO servicios VB.NET separados
5. ✅ **Background Services** - Para tareas programadas
6. ✅ **Columnas dinámicas** - Generadas desde API
7. ✅ **Popups para captura** - No páginas nuevas
8. ✅ **Filtros en grid** - Solo fechas arriba

---

## 2. CONTEXTO DEL MÓDULO

### 2.1 Sistema Existente

**JELABBC** es un sistema de gestión para condominios/fraccionamientos que incluye:

- Gestión de entidades y unidades
- Residentes y visitantes
- Cuotas y pagos
- Comunicados
- Reservaciones de áreas comunes
- Formularios dinámicos
- Documentos

### 2.2 Integración del Módulo

Este módulo de tickets se integra con:

**Entidades (cat_entidades):**
- Cada ticket pertenece a una entidad (IdEntidad)
- Configuraciones por entidad

**Usuarios (conf_usuarios):**
- Agentes que atienden tickets
- Creadores de tickets
- Permisos y roles

**Residentes (cat_residentes):**
- Clientes que crean tickets
- Historial de interacciones

### 2.3 Dependencias

**Tablas Existentes Requeridas:**
- `cat_entidades` - Entidades del sistema
- `conf_usuarios` - Usuarios y agentes
- `cat_residentes` - Residentes/clientes (opcional)

**Servicios Existentes:**
- JELA.API - API REST .NET 8
- ApiConsumerCRUD.vb - Consumidor de API
- Logger.vb - Sistema de logging
- AuthHelper.vb - Autenticación

---

## 3. ARQUITECTURA DEL SISTEMA

### 3.1 Diagrama de Arquitectura Completa

```
┌──────────────────────────────────────────────────────────────────────────────┐
│                    MÓDULO DE TICKETS COLABORATIVOS                           │
│                    Integrado en Sistema JELABBC                              │
└──────────────────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────────────────┐
│ CANALES DE ENTRADA (5 Canales)                                              │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  [VAPI]      [YCloud]     [Telegram]    [Chat Web]     [Firebase]          │
│  Llamadas    WhatsApp     Bot           Widget          Chat App            │
│     │            │            │             │               │                │
│     │ Webhook    │ Webhook    │ Webhook     │ HTTP          │ HTTP           │
│     └────────────┴────────────┴─────────────┴───────────────┘                │
│                              │                                               │
└──────────────────────────────┼───────────────────────────────────────────────┘
                               │
                               ▼
┌──────────────────────────────────────────────────────────────────────────────┐
│ JELA.API (.NET 8) - Azure Mexico Central                                    │
├──────────────────────────────────────────────────────────────────────────────┤
│                                                                              │
│  ENDPOINTS NUEVOS:                                                           │
│  ┌────────────────────────────────────────────────────────────────┐         │
│  │ WebhookEndpoints.cs                                            │         │
│  │  • POST /api/webhooks/vapi                                     │         │
│  │  • POST /api/webhooks/ycloud                                   │         │
│  │  • POST /api/webhooks/telegram                                 │         │
│  │  • POST /api/webhooks/chatweb                                  │         │
│  │  • POST /api/webhooks/firebase                                 │         │
│  └────────────────────────────────────────────────────────────────┘         │
│                                                                              │
│  ┌────────────────────────────────────────────────────────────────┐         │
│  │ TicketValidationEndpoints.cs                                   │         │
│  │  • POST /api/tickets/validar-cliente                           │         │
│  │  • GET  /api/tickets/historial/{telefono}                      │         │
│  └────────────────────────────────────────────────────────────────┘         │
│                                                                              │
│  ┌────────────────────────────────────────────────────────────────┐         │
│  │ TicketNotificationEndpoints.cs                                 │         │
│  │  • POST /api/tickets/notificar-whatsapp                        │         │
│  │  • GET  /api/tickets/notificaciones/cola                       │         │
│  │  • PUT  /api/tickets/notificaciones/{id}/estado                │         │
│  └────────────────────────────────────────────────────────────────┘         │
│                                                                              │
│  ┌────────────────────────────────────────────────────────────────┐         │
│  │ TicketMetricsEndpoints.cs                                      │         │
│  │  • GET  /api/tickets/metricas/tiempo-real                      │         │
│  │  • GET  /api/tickets/metricas/diarias                          │         │
│  │  • POST /api/tickets/metricas/calcular                         │         │
│  └────────────────────────────────────────────────────────────────┘         │
│                                                                              │
│  SERVICIOS DE LÓGICA DE NEGOCIO:                                            │
│  ┌────────────────────────────────────────────────────────────────┐         │
│  │ Services/                                                      │         │
│  │  • TicketValidationService.cs                                  │         │
│  │  • TicketNotificationService.cs                                │         │
│  │  • TicketMetricsService.cs                                     │         │
│  │  • PromptTuningService.cs                                      │         │
│  │  • YCloudService.cs                                            │         │
│  │  • TelegramService.cs                                          │         │
│  │  • VapiService.cs                                              │         │
│  └────────────────────────────────────────────────────────────────┘         │
│                                                                              │
│  BACKGROUND SERVICES (Tareas Programadas):                                  │
│  ┌────────────────────────────────────────────────────────────────┐         │
│  │ BackgroundServices/                                            │         │
│  │  • TicketMonitoringBackgroundService.cs (cada 5 min)           │         │
│  │  • TicketMetricsBackgroundService.cs (cada hora)               │         │
│  │  • PromptTuningBackgroundService.cs (cada 2 semanas)           │         │
│  │  • NotificationQueueBackgroundService.cs (cada 30 seg)         │         │
│  └────────────────────────────────────────────────────────────────┘         │
│                                                                              │
│  CARACTERÍSTICAS:                                                            │
│  ✅ JWT Authentication                                                       │
│  ✅ Rate Limiting (100 req/min)                                              │
│  ✅ Swagger/OpenAPI                                                          │
│  ✅ Serilog Logging                                                          │
│  ✅ Health Checks                                                            │
│  ✅ Dapper + MySQL                                                           │
│                                                                              │
└──────────────────┬───────────────────────────────────────────────────────────┘
                   │
     ┌─────────────┴─────────────┐
     │                           │
     ▼                           ▼
┌─────────────────┐    ┌──────────────────────┐
│ MySQL Azure     │    │ Azure OpenAI         │
│ jela_qa         │    │ GPT-4o-mini          │
│                 │    │                      │
│ TABLAS NUEVAS:  │    │ Funciones:           │
│ • op_tickets_v2 │    │ • Categorización     │
│   (+13 campos)  │    │ • Sentimiento        │
│ • 8 tablas logs │    │ • Respuestas         │
│ • métricas      │    │ • Validación         │
│ • validación    │    │                      │
│ • notificaciones│    │                      │
└─────────────────┘    └──────────────────────┘
           │
           │
           ▼
┌──────────────────────────────────────────────────────────────────┐
│ JelaWeb (ASP.NET VB.NET 4.8.1)                                   │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  PÁGINAS NUEVAS:                                                 │
│  • Dashboard de Tickets en Views/Inicio.aspx (integrar)         │
│  • Views/Catalogos/Tickets/TicketsPrompts.aspx                  │
│  • Views/Operacion/Tickets/TicketsLogs.aspx                     │
│                                                                  │
│  PÁGINA EXISTENTE (Mejorar):                                     │
│  • Views/Operacion/Tickets/Tickets.aspx                         │
│                                                                  │
│  CONSUME API VÍA:                                                │
│  • ApiConsumerCRUD.vb                                            │
│  • JWT Authentication                                            │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────────────────┐
│ INTEGRACIONES EXTERNAS                                           │
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  [VAPI API]         [YCloud API]        [Telegram Bot API]      │
│  Llamadas           WhatsApp            Bot de Telegram          │
│  telefónicas        Business                                     │
│                                                                  │
│  • Webhook          • POST /messages    • getUpdates             │
│  • Transcripción    • GET /messages     • sendMessage            │
│  • Síntesis voz     • Webhook           • Webhook                │
│                                                                  │
│  [Chat Web]         [Firebase]                                   │
│  Widget             Chat App                                     │
│                                                                  │
│  • HTTP POST        • Realtime DB                                │
│                     • Push Notif                                 │
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

### 3.2 Flujo de Datos Principal

```
FLUJO: Ticket desde WhatsApp (YCloud)
═════════════════════════════════════

Cliente → YCloud → JELA.API → Validation → OpenAI → MySQL → Notification → YCloud
   │         │         │          Service      │       │        Service        │
   │         │         │             │         │       │           │           │
   │ Mensaje │         │             │         │       │           │           │
   │────────>│         │             │         │       │           │           │
   │         │         │             │         │       │           │           │
   │         │ POST /api/webhooks/ycloud       │       │           │           │
   │         │────────>│             │         │       │           │           │
   │         │         │             │         │       │           │           │
   │         │         │ ValidarClienteDuplicado│       │           │           │
   │         │         │────────────>│         │       │           │           │
   │         │         │             │         │       │           │           │
   │         │         │<────────────│         │       │           │           │
   │         │         │ OK/Duplicado│         │       │           │           │
   │         │         │             │         │       │           │           │
   │         │         │ POST /api/openai      │       │           │           │
   │         │         │──────────────────────>│       │           │           │
   │         │         │             │         │       │           │           │
   │         │         │<──────────────────────│       │           │           │
   │         │         │ Categoría + Respuesta │       │           │           │
   │         │         │             │         │       │           │           │
   │         │         │ INSERT ticket         │       │           │           │
   │         │         │───────────────────────────────>│           │           │
   │         │         │             │         │       │           │           │
   │         │         │ INSERT logs           │       │           │           │
   │         │         │───────────────────────────────>│           │           │
   │         │         │             │         │       │           │           │
   │         │         │ EncolarNotificacion   │       │           │           │
   │         │         │───────────────────────────────────────────>│           │
   │         │         │             │         │       │           │           │
   │         │<────────│             │         │       │           │           │
   │         │ 200 OK  │             │         │       │           │           │
   │         │         │             │         │       │           │           │
   │         │         │             │         │       │ [Background Service]  │
   │         │         │             │         │       │ Procesa cola cada 30s │
   │         │         │             │         │       │           │           │
   │         │         │             │         │       │ POST /messages        │
   │         │         │             │         │       │───────────────────────>│
   │         │         │             │         │       │           │           │
   │<────────────────────────────────────────────────────────────────────────────│
   │ Respuesta WhatsApp│             │         │       │           │           │

TIEMPO TOTAL: 2-3 segundos
```

### 3.3 Componentes del Sistema

#### 3.3.1 API Layer (JELA.API - .NET 8)

**Archivos Nuevos:**
- `Endpoints/WebhookEndpoints.cs`
- `Endpoints/TicketValidationEndpoints.cs`
- `Endpoints/TicketNotificationEndpoints.cs`
- `Endpoints/TicketMetricsEndpoints.cs`
- `Services/TicketValidationService.cs`
- `Services/TicketNotificationService.cs`
- `Services/TicketMetricsService.cs`
- `Services/PromptTuningService.cs`
- `Services/YCloudService.cs`
- `Services/VapiService.cs`
- `BackgroundServices/TicketMonitoringBackgroundService.cs`
- `BackgroundServices/TicketMetricsBackgroundService.cs`
- `BackgroundServices/PromptTuningBackgroundService.cs`
- `BackgroundServices/NotificationQueueBackgroundService.cs`
- `Models/TicketModels.cs`
- `Models/WebhookModels.cs`

#### 3.3.2 Frontend Layer (JelaWeb - VB.NET)

**Páginas Nuevas:**
- Dashboard de Tickets integrado en `Views/Inicio.aspx` (modificar existente)
- `Views/Catalogos/Tickets/TicketsPrompts.aspx` + .vb + .designer.vb
- `Views/Operacion/Tickets/TicketsLogs.aspx` + .vb + .designer.vb

**Página Existente (Mejorar):**
- `Views/Operacion/Tickets/Tickets.aspx` + .vb + .designer.vb

**CSS:**
- `Content/CSS/tickets-dashboard.css`
- `Content/CSS/tickets-prompts.css`
- `Content/CSS/tickets-logs.css`

**JavaScript:**
- `Scripts/app/Operacion/tickets-dashboard.js`
- `Scripts/app/Operacion/tickets-prompts.js`
- `Scripts/app/Operacion/tickets-logs.js`

#### 3.3.3 Data Layer (MySQL Azure)

**Tabla Existente (Modificar):**
- `op_tickets_v2` - Agregar 13 campos nuevos

**Tablas Nuevas (8):**
- `op_ticket_logs_sistema`
- `op_ticket_logs_interacciones`
- `op_ticket_logprompts`
- `op_ticket_metricas`
- `op_ticket_validacion_cliente`
- `op_ticket_notificaciones_whatsapp`
- `op_ticket_robot_monitoreo`
- `op_ticket_prompt_ajustes_log`

---

## 4. BASE DE DATOS

### 4.1 Convenciones de Nomenclatura

**CRÍTICO - Reglas Obligatorias:**

1. **Prefijos de Tablas:**
   - `op_` - Tablas operativas/transaccionales
   - `conf_` - Tablas de configuración/catálogos
   - Usar guion bajo (_): `op_tickets_v2` ✅ NO `optickets` ❌

2. **Nombres de Campos:**
   - **PascalCase** obligatorio: `ChatId`, `FechaCreacion`, `ClienteNombre` ✅
   - NO usar snake_case: `chat_id` ❌
   - NO usar camelCase: `chatId` ❌
   - **Razón**: `FuncionesGridWeb.SUMColumn` convierte PascalCase automáticamente

3. **Campos Estándar:**
   ```sql
   Id INT AUTO_INCREMENT PRIMARY KEY,
   IdEntidad INT NOT NULL DEFAULT 1,
   IdUsuarioCreacion INT NOT NULL,
   FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
   FechaUltimaActualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
   Activo TINYINT(1) DEFAULT 1
   ```

### 4.2 Tabla Existente: op_tickets_v2 (Modificar)

**Estado:** EXISTENTE - Agregar 13 campos nuevos

**Script de Alteración:**

```sql
-- ============================================================================
-- AGREGAR 13 CAMPOS NUEVOS A op_tickets_v2
-- ============================================================================

ALTER TABLE op_tickets_v2

-- 1. Tipo de Ticket
ADD COLUMN TipoTicket ENUM('Accion','Inaccion','LlamadaCortada','ChatWeb','ChatApp','WhatsApp') 
  DEFAULT NULL 
  COMMENT 'Tipo de ticket según origen',

-- 2. IP de Origen
ADD COLUMN IPOrigen VARCHAR(50) DEFAULT NULL 
  COMMENT 'Dirección IP desde donde se originó',

-- 3. Duración de Llamada (para VAPI)
ADD COLUMN DuracionLlamadaSegundos INT DEFAULT NULL 
  COMMENT 'Duración total de la llamada en segundos',

-- 4. Momento de Corte
ADD COLUMN MomentoCorte VARCHAR(100) DEFAULT NULL 
  COMMENT 'Momento en que se cortó la llamada',

-- 5. Intentos de Reconexión
ADD COLUMN IntentosReconexion INT DEFAULT 0 
  COMMENT 'Número de intentos de reconexión',

-- 6. Monto Relacionado
ADD COLUMN MontoRelacionado DECIMAL(10,2) DEFAULT NULL 
  COMMENT 'Monto relacionado con el ticket',

-- 7. Pedido Relacionado
ADD COLUMN PedidoRelacionado VARCHAR(100) DEFAULT NULL 
  COMMENT 'ID del pedido o transacción',

-- 8. Riesgo de Fraude
ADD COLUMN RiesgoFraude BOOLEAN DEFAULT FALSE 
  COMMENT 'Indica si la IA detectó posible fraude',

-- 9. Requiere Escalamiento
ADD COLUMN RequiereEscalamiento BOOLEAN DEFAULT FALSE 
  COMMENT 'Indica si requiere escalamiento a humano',

-- 10. Impacto
ADD COLUMN Impacto ENUM('Individual','Grupal','Masivo') DEFAULT 'Individual' 
  COMMENT 'Impacto del ticket',

-- 11. CSAT Score (1-5)
ADD COLUMN CSATScore INT DEFAULT NULL 
  COMMENT 'Customer Satisfaction Score (1-5)',
  ADD CONSTRAINT chk_csat_score CHECK (CSATScore BETWEEN 1 AND 5),

-- 12. Resuelto por IA
ADD COLUMN ResueltoporIA BOOLEAN DEFAULT FALSE 
  COMMENT 'Indica si fue resuelto por IA sin intervención humana',

-- 13. Idioma
ADD COLUMN Idioma VARCHAR(10) DEFAULT 'es' 
  COMMENT 'Idioma del ticket: es, en, etc.';

-- Agregar índices para optimizar consultas
CREATE INDEX idx_ticket_tipo ON op_tickets_v2(TipoTicket);
CREATE INDEX idx_ticket_ip ON op_tickets_v2(IPOrigen);
CREATE INDEX idx_ticket_riesgo ON op_tickets_v2(RiesgoFraude);
CREATE INDEX idx_ticket_escalamiento ON op_tickets_v2(RequiereEscalamiento);
CREATE INDEX idx_ticket_resuelto_ia ON op_tickets_v2(ResueltoporIA);
CREATE INDEX idx_ticket_idioma ON op_tickets_v2(Idioma);
```

### 4.3 Tablas Nuevas

#### 4.3.1 op_ticket_logs_sistema

**Propósito:** Auditoría de eventos del sistema

```sql
CREATE TABLE op_ticket_logs_sistema (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  IdTicket INT DEFAULT NULL COMMENT 'FK a op_tickets_v2 (opcional)',
  TipoEvento VARCHAR(100) NOT NULL COMMENT 'Tipo de evento: TicketCreado, EstadoCambiado, etc.',
  Descripcion TEXT DEFAULT NULL COMMENT 'Descripción del evento',
  Severidad ENUM('Info','Warning','Error','Critical') DEFAULT 'Info',
  Metadata JSON DEFAULT NULL COMMENT 'Datos adicionales en JSON',
  IdUsuarioCreacion INT DEFAULT NULL COMMENT 'FK a conf_usuarios',
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  PRIMARY KEY (Id),
  INDEX idx_logs_sistema_ticket (IdTicket),
  INDEX idx_logs_sistema_tipo (TipoEvento),
  INDEX idx_logs_sistema_fecha (FechaCreacion),
  INDEX idx_logs_sistema_severidad (Severidad),
  INDEX idx_logs_sistema_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
  FOREIGN KEY (IdTicket) REFERENCES op_tickets_v2(Id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Auditoría de eventos del sistema de tickets';
```

#### 4.3.2 op_ticket_logs_interacciones

**Propósito:** Tracking de interacciones multicanal

```sql
CREATE TABLE op_ticket_logs_interacciones (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  IdTicket INT NOT NULL COMMENT 'FK a op_tickets_v2',
  Canal VARCHAR(50) NOT NULL COMMENT 'VAPI, YCloud, ChatWeb, Firebase',
  TipoInteraccion VARCHAR(100) NOT NULL COMMENT 'LlamadaIniciada, MensajeRecibido, etc.',
  IdExternoCanal VARCHAR(200) DEFAULT NULL COMMENT 'ID del mensaje/llamada en el canal externo',
  DatosInteraccion JSON DEFAULT NULL COMMENT 'Datos completos de la interacción',
  Duracion INT DEFAULT NULL COMMENT 'Duración en segundos (para llamadas)',
  Exitosa BOOLEAN DEFAULT TRUE COMMENT 'Indica si la interacción fue exitosa',
  MensajeError TEXT DEFAULT NULL COMMENT 'Mensaje de error si falló',
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  PRIMARY KEY (Id),
  INDEX idx_logs_interacciones_ticket (IdTicket),
  INDEX idx_logs_interacciones_canal (Canal),
  INDEX idx_logs_interacciones_fecha (FechaCreacion),
  INDEX idx_logs_interacciones_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
  FOREIGN KEY (IdTicket) REFERENCES op_tickets_v2(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Tracking de interacciones multicanal';
```

#### 4.3.3 op_ticket_logprompts

**Propósito:** Prompts anonimizados para análisis

```sql
CREATE TABLE op_ticket_logprompts (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  IdTicket INT DEFAULT NULL COMMENT 'FK a op_tickets_v2 (anonimizado)',
  IdPrompt INT DEFAULT NULL COMMENT 'FK a conf_ticket_prompts',
  PromptEnviado TEXT NOT NULL COMMENT 'Prompt enviado a la IA (anonimizado)',
  RespuestaIA TEXT DEFAULT NULL COMMENT 'Respuesta de la IA',
  ModeloUtilizado VARCHAR(100) DEFAULT NULL COMMENT 'gpt-4o-mini, gpt-4, etc.',
  TokensUtilizados INT DEFAULT NULL COMMENT 'Tokens consumidos',
  TiempoRespuestaMs INT DEFAULT NULL COMMENT 'Tiempo de respuesta en milisegundos',
  Exitoso BOOLEAN DEFAULT TRUE,
  MensajeError TEXT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  PRIMARY KEY (Id),
  INDEX idx_logprompts_ticket (IdTicket),
  INDEX idx_logprompts_prompt (IdPrompt),
  INDEX idx_logprompts_fecha (FechaCreacion),
  INDEX idx_logprompts_modelo (ModeloUtilizado),
  INDEX idx_logprompts_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
  FOREIGN KEY (IdTicket) REFERENCES op_tickets_v2(Id) ON DELETE SET NULL,
  FOREIGN KEY (IdPrompt) REFERENCES conf_ticket_prompts(Id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Log de prompts enviados a IA (anonimizados)';
```

#### 4.3.4 op_ticket_metricas

**Propósito:** Métricas agregadas por periodo

```sql
CREATE TABLE op_ticket_metricas (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  FechaMetrica DATE NOT NULL COMMENT 'Fecha de la métrica',
  TipoAgregacion ENUM('Diaria','Semanal','Mensual') DEFAULT 'Diaria',
  Canal VARCHAR(50) DEFAULT NULL COMMENT 'Canal específico o NULL para todos',
  TotalTicketsCreados INT DEFAULT 0,
  TotalTicketsResueltos INT DEFAULT 0,
  TotalTicketsResueltosIA INT DEFAULT 0,
  PorcentajeResolucionIA DECIMAL(5,2) DEFAULT 0.00,
  TiempoPromedioResolucionMinutos DECIMAL(10,2) DEFAULT NULL,
  TiempoPromedioRespuestaMinutos DECIMAL(10,2) DEFAULT NULL,
  CSATPromedio DECIMAL(3,2) DEFAULT NULL COMMENT 'Promedio de satisfacción',
  TotalLlamadasVAPI INT DEFAULT 0,
  TotalMensajesWhatsApp INT DEFAULT 0,
  TotalChatWeb INT DEFAULT 0,
  TotalChatApp INT DEFAULT 0,
  TokensIAUtilizados INT DEFAULT 0,
  CostoEstimadoIA DECIMAL(10,4) DEFAULT 0.0000,
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FechaUltimaActualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  PRIMARY KEY (Id),
  UNIQUE INDEX uk_metrica_fecha_canal (IdEntidad, FechaMetrica, TipoAgregacion, Canal),
  INDEX idx_metricas_fecha (FechaMetrica),
  INDEX idx_metricas_tipo (TipoAgregacion),
  INDEX idx_metricas_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Métricas agregadas del sistema de tickets';
```

#### 4.3.5 op_ticket_validacion_cliente

**Propósito:** Validación de clientes duplicados

```sql
CREATE TABLE op_ticket_validacion_cliente (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  TelefonoCliente VARCHAR(50) DEFAULT NULL,
  EmailCliente VARCHAR(255) DEFAULT NULL,
  IPOrigen VARCHAR(50) DEFAULT NULL,
  TieneTicketAbierto BOOLEAN DEFAULT FALSE,
  IdTicketAbierto INT DEFAULT NULL COMMENT 'FK a op_tickets_v2',
  NumeroTicketsHistoricos INT DEFAULT 0,
  UltimaInteraccion DATETIME DEFAULT NULL,
  Bloqueado BOOLEAN DEFAULT FALSE COMMENT 'Cliente bloqueado por spam/abuso',
  MotivoBloqueo TEXT DEFAULT NULL,
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FechaUltimaActualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  PRIMARY KEY (Id),
  INDEX idx_validacion_telefono (TelefonoCliente),
  INDEX idx_validacion_email (EmailCliente),
  INDEX idx_validacion_ip (IPOrigen),
  INDEX idx_validacion_bloqueado (Bloqueado),
  INDEX idx_validacion_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
  FOREIGN KEY (IdTicketAbierto) REFERENCES op_tickets_v2(Id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Validación de clientes para prevenir duplicados';
```

#### 4.3.6 op_ticket_notificaciones_whatsapp

**Propósito:** Cola de notificaciones WhatsApp

```sql
CREATE TABLE op_ticket_notificaciones_whatsapp (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  IdTicket INT NOT NULL COMMENT 'FK a op_tickets_v2',
  NumeroWhatsApp VARCHAR(50) NOT NULL COMMENT 'Número destino con código país',
  TipoNotificacion VARCHAR(100) NOT NULL COMMENT 'TicketCreado, RespuestaAgente, etc.',
  MensajeTexto TEXT NOT NULL COMMENT 'Texto del mensaje a enviar',
  PlantillaId VARCHAR(100) DEFAULT NULL COMMENT 'ID de plantilla de YCloud (opcional)',
  ParametrosPlantilla JSON DEFAULT NULL COMMENT 'Parámetros para plantilla',
  Estado ENUM('Pendiente','Enviando','Enviado','Fallido','Cancelado') DEFAULT 'Pendiente',
  IntentosEnvio INT DEFAULT 0,
  MaximoIntentos INT DEFAULT 3,
  ProximoIntento DATETIME DEFAULT NULL COMMENT 'Fecha del próximo intento',
  FechaEnvio DATETIME DEFAULT NULL COMMENT 'Fecha de envío exitoso',
  IdMensajeYCloud VARCHAR(200) DEFAULT NULL COMMENT 'ID del mensaje en YCloud',
  RespuestaYCloudJSON TEXT DEFAULT NULL COMMENT 'Respuesta completa de YCloud',
  MensajeError TEXT DEFAULT NULL,
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FechaUltimaActualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  PRIMARY KEY (Id),
  INDEX idx_notif_ticket (IdTicket),
  INDEX idx_notif_estado (Estado),
  INDEX idx_notif_proximo_intento (ProximoIntento),
  INDEX idx_notif_fecha_creacion (FechaCreacion),
  INDEX idx_notif_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
  FOREIGN KEY (IdTicket) REFERENCES op_tickets_v2(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Cola de notificaciones WhatsApp';
```

#### 4.3.7 op_ticket_robot_monitoreo

**Propósito:** Tracking del robot de monitoreo

```sql
CREATE TABLE op_ticket_robot_monitoreo (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  FechaEjecucion DATETIME NOT NULL COMMENT 'Fecha/hora de ejecución',
  TipoMonitoreo VARCHAR(100) NOT NULL COMMENT 'TicketsSinAtender, TicketsVencidos, etc.',
  TicketsDetectados INT DEFAULT 0 COMMENT 'Número de tickets detectados',
  AccionesRealizadas INT DEFAULT 0 COMMENT 'Número de acciones tomadas',
  NotificacionesEnviadas INT DEFAULT 0,
  TiempoEjecucionMs INT DEFAULT NULL COMMENT 'Tiempo de ejecución en milisegundos',
  Exitoso BOOLEAN DEFAULT TRUE,
  MensajeError TEXT DEFAULT NULL,
  DetallesJSON JSON DEFAULT NULL COMMENT 'Detalles de la ejecución',
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  PRIMARY KEY (Id),
  INDEX idx_robot_fecha (FechaEjecucion),
  INDEX idx_robot_tipo (TipoMonitoreo),
  INDEX idx_robot_exitoso (Exitoso),
  INDEX idx_robot_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Tracking de ejecuciones del robot de monitoreo';
```

#### 4.3.8 op_ticket_prompt_ajustes_log

**Propósito:** Log de ajustes automáticos de prompts

```sql
CREATE TABLE op_ticket_prompt_ajustes_log (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  IdPrompt INT NOT NULL COMMENT 'FK a conf_ticket_prompts',
  FechaAjuste DATETIME NOT NULL COMMENT 'Fecha del ajuste',
  VersionAnterior TEXT NOT NULL COMMENT 'Versión anterior del prompt',
  VersionNueva TEXT NOT NULL COMMENT 'Versión nueva del prompt',
  MotivoAjuste TEXT DEFAULT NULL COMMENT 'Razón del ajuste',
  MetricasAntes JSON DEFAULT NULL COMMENT 'Métricas antes del ajuste',
  MetricasDespues JSON DEFAULT NULL COMMENT 'Métricas después del ajuste',
  MejoraDetectada BOOLEAN DEFAULT NULL COMMENT 'Si hubo mejora o no',
  PorcentajeMejora DECIMAL(5,2) DEFAULT NULL,
  Aprobado BOOLEAN DEFAULT FALSE COMMENT 'Si el ajuste fue aprobado',
  IdUsuarioAprobacion INT DEFAULT NULL COMMENT 'FK a conf_usuarios',
  FechaAprobacion DATETIME DEFAULT NULL,
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  PRIMARY KEY (Id),
  INDEX idx_ajustes_prompt (IdPrompt),
  INDEX idx_ajustes_fecha (FechaAjuste),
  INDEX idx_ajustes_aprobado (Aprobado),
  INDEX idx_ajustes_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
  FOREIGN KEY (IdPrompt) REFERENCES conf_ticket_prompts(Id) ON DELETE CASCADE,
  FOREIGN KEY (IdUsuarioAprobacion) REFERENCES conf_usuarios(Id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Log de ajustes automáticos de prompts';
```

### 4.4 Tablas Específicas para Telegram

**NOTA IMPORTANTE:** Las siguientes tablas son específicas para la integración con Telegram y el sistema de validación de clientes. Estas tablas son críticas para el funcionamiento del canal Telegram.

#### 4.4.1 op_telegram_clientes

**Propósito:** Registro y gestión de clientes que interactúan vía Telegram

**Convenciones aplicadas:**
- Prefijo `op_` (tabla operativa)
- Campos en PascalCase
- Campos estándar de auditoría

```sql
CREATE TABLE op_telegram_clientes (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  ChatId BIGINT UNIQUE NOT NULL COMMENT 'ID de Telegram del cliente',
  Username VARCHAR(255) DEFAULT NULL COMMENT 'Username de Telegram (@usuario)',
  FirstName VARCHAR(255) DEFAULT NULL,
  LastName VARCHAR(255) DEFAULT NULL,
  
  -- Estado y Tipo de Cliente
  EstadoCliente VARCHAR(20) DEFAULT 'activo' COMMENT 'activo, bloqueado, suspendido',
  TipoCliente VARCHAR(20) DEFAULT 'standard' COMMENT 'standard, premium, trial',
  
  -- Control de Licencia/Suscripción
  FechaVencimiento DATE DEFAULT NULL COMMENT 'Fecha de vencimiento de licencia',
  CreditosDisponibles INT DEFAULT 0 COMMENT 'Créditos disponibles para tickets',
  TicketsMesActual INT DEFAULT 0 COMMENT 'Tickets creados en el mes actual',
  LimiteTicketsMes INT DEFAULT 50 COMMENT 'Límite mensual de tickets',
  
  -- Actividad y Seguridad
  UltimaActividad DATETIME DEFAULT NULL,
  RazonBloqueo TEXT DEFAULT NULL,
  BloqueadoPor VARCHAR(100) DEFAULT NULL,
  FechaBloqueo DATETIME DEFAULT NULL,
  IntentosFallidos INT DEFAULT 0 COMMENT 'Intentos fallidos de validación',
  IPUltimoAcceso VARCHAR(50) DEFAULT NULL,
  
  -- Auditoría
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FechaUltimaActualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  
  PRIMARY KEY (Id),
  UNIQUE INDEX uk_telegram_chat_id (ChatId),
  INDEX idx_telegram_estado (EstadoCliente),
  INDEX idx_telegram_tipo (TipoCliente),
  INDEX idx_telegram_username (Username),
  INDEX idx_telegram_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Clientes registrados de Telegram';
```

#### 4.4.2 op_telegram_whitelist

**Propósito:** Lista de clientes pre-aprobados con acceso prioritario

```sql
CREATE TABLE op_telegram_whitelist (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  ChatId BIGINT UNIQUE NOT NULL,
  ClienteNombre VARCHAR(255) NOT NULL,
  Email VARCHAR(255) DEFAULT NULL,
  Empresa VARCHAR(255) DEFAULT NULL,
  
  -- Aprobación
  FechaAprobacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  AprobadoPor VARCHAR(100) DEFAULT NULL COMMENT 'Usuario que aprobó',
  Notas TEXT DEFAULT NULL COMMENT 'Notas sobre la aprobación',
  
  -- Prioridad
  Prioridad ENUM('alta', 'media', 'baja') DEFAULT 'media',
  
  -- Auditoría
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FechaUltimaActualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  
  PRIMARY KEY (Id),
  UNIQUE INDEX uk_whitelist_chat_id (ChatId),
  INDEX idx_whitelist_activo (Activo),
  INDEX idx_whitelist_prioridad (Prioridad),
  INDEX idx_whitelist_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Clientes pre-aprobados (whitelist)';
```

#### 4.4.3 op_telegram_blacklist

**Propósito:** Lista de clientes bloqueados permanente o temporalmente

```sql
CREATE TABLE op_telegram_blacklist (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  ChatId BIGINT UNIQUE NOT NULL,
  Username VARCHAR(255) DEFAULT NULL,
  
  -- Bloqueo
  RazonBloqueo TEXT NOT NULL,
  FechaBloqueo DATETIME DEFAULT CURRENT_TIMESTAMP,
  BloqueadoPor VARCHAR(100) DEFAULT NULL COMMENT 'Usuario que bloqueó',
  
  -- Tipo de Bloqueo
  Permanente BOOLEAN DEFAULT 0,
  FechaLevantamiento DATETIME DEFAULT NULL COMMENT 'Fecha de levantamiento si es temporal',
  
  -- Notas
  NotasAdicionales TEXT DEFAULT NULL,
  
  -- Auditoría
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FechaUltimaActualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  
  PRIMARY KEY (Id),
  UNIQUE INDEX uk_blacklist_chat_id (ChatId),
  INDEX idx_blacklist_permanente (Permanente),
  INDEX idx_blacklist_fecha_levantamiento (FechaLevantamiento),
  INDEX idx_blacklist_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Clientes bloqueados (blacklist)';
```

#### 4.4.4 op_telegram_logs_validacion

**Propósito:** Registro de todas las validaciones de clientes (sistema de 7 niveles)

```sql
CREATE TABLE op_telegram_logs_validacion (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  ChatId BIGINT NOT NULL,
  
  -- Validación
  FechaValidacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  Resultado ENUM('aprobado', 'rechazado', 'pendiente') NOT NULL,
  NivelAlcanzado VARCHAR(50) DEFAULT NULL COMMENT 'Nivel de validación alcanzado (1-7)',
  RazonRechazo TEXT DEFAULT NULL COMMENT 'Razón del rechazo si aplica',
  
  -- Metadatos
  IPOrigen VARCHAR(50) DEFAULT NULL,
  Metadata JSON DEFAULT NULL COMMENT 'Información adicional en formato JSON',
  
  -- Auditoría
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  
  PRIMARY KEY (Id),
  INDEX idx_telegram_logs_chat_id (ChatId),
  INDEX idx_telegram_logs_fecha (FechaValidacion DESC),
  INDEX idx_telegram_logs_resultado (Resultado),
  INDEX idx_telegram_logs_chat_fecha (ChatId, FechaValidacion DESC),
  INDEX idx_telegram_logs_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Logs de validación de clientes Telegram';
```

#### 4.4.5 op_telegram_notifications_queue

**Propósito:** Cola de notificaciones pendientes de envío a Telegram

```sql
CREATE TABLE op_telegram_notifications_queue (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  
  -- Ticket Relacionado
  IdTicket INT DEFAULT NULL,
  ChatId BIGINT NOT NULL,
  
  -- Notificación
  TipoNotificacion VARCHAR(50) NOT NULL COMMENT 'cambio_estado, asignacion, resolucion, etc.',
  EstadoNuevo VARCHAR(50) DEFAULT NULL COMMENT 'Nuevo estado del ticket',
  Mensaje TEXT NOT NULL,
  
  -- Estado de Procesamiento
  Procesado BOOLEAN DEFAULT 0,
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  FechaProcesado DATETIME DEFAULT NULL,
  
  -- Reintentos
  IntentosEnvio INT DEFAULT 0,
  UltimoError TEXT DEFAULT NULL,
  
  -- Auditoría
  IdUsuarioCreacion INT DEFAULT NULL,
  FechaUltimaActualizacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  Activo TINYINT(1) DEFAULT 1,
  
  PRIMARY KEY (Id),
  INDEX idx_telegram_notif_pendientes (Procesado, FechaCreacion),
  INDEX idx_telegram_notif_chat_id (ChatId),
  INDEX idx_telegram_notif_ticket (IdTicket),
  INDEX idx_telegram_notif_entidad (IdEntidad),
  FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
  FOREIGN KEY (IdTicket) REFERENCES op_tickets_v2(Id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Cola de notificaciones para Telegram';
```

#### 4.4.6 Trigger: trg_NotificarCambioEstadoTelegram

**Propósito:** Encolar automáticamente notificaciones cuando cambia el estado de un ticket de Telegram

```sql
DELIMITER $

CREATE TRIGGER trg_NotificarCambioEstadoTelegram
AFTER UPDATE ON op_tickets_v2
FOR EACH ROW
BEGIN
  -- Solo si cambió el estado y el canal es Telegram
  IF OLD.Estado != NEW.Estado AND NEW.Canal = 'Telegram' THEN
    INSERT INTO op_telegram_notifications_queue (
      IdEntidad,
      IdTicket,
      ChatId,
      TipoNotificacion,
      EstadoNuevo,
      Mensaje,
      IdUsuarioCreacion
    ) VALUES (
      NEW.IdEntidad,
      NEW.Id,
      NEW.TelefonoCliente, -- Asumiendo que ChatId se guarda en TelefonoCliente
      'cambio_estado',
      NEW.Estado,
      CONCAT('Tu ticket #', NEW.Id, ' ha cambiado a estado: ', NEW.Estado),
      NEW.IdUsuarioCreacion
    );
  END IF;
END$

DELIMITER ;
```

#### 4.4.7 Campos Adicionales en op_tickets_v2 para Telegram

**IMPORTANTE:** Para soportar Telegram completamente, considerar agregar estos campos adicionales a `op_tickets_v2`:

```sql
-- Campos opcionales para mejor soporte de Telegram
ALTER TABLE op_tickets_v2
ADD COLUMN ChatId BIGINT DEFAULT NULL COMMENT 'ID de Telegram del cliente',
ADD COLUMN ClienteValidado BOOLEAN DEFAULT 0 COMMENT 'Si el cliente pasó validación',
ADD COLUMN NivelValidacion VARCHAR(50) DEFAULT 'pendiente' COMMENT 'Nivel de validación alcanzado',
ADD COLUMN CreditosUsados INT DEFAULT 0 COMMENT 'Créditos consumidos por este ticket';

-- Índices
CREATE INDEX idx_tickets_chat_id ON op_tickets_v2(ChatId);
CREATE INDEX idx_tickets_cliente_validado ON op_tickets_v2(ClienteValidado);
```

### 4.5 Sistema de Validación de 4 Niveles

El sistema implementa un proceso de validación en cascada con 4 niveles de verificación antes de permitir la creación de un ticket.

#### Niveles de Validación:

**Nivel 1: Verificación de Blacklist**
- Consulta tabla de clientes bloqueados
- Si el cliente está bloqueado (permanente o temporal sin fecha de levantamiento), se rechaza inmediatamente
- Mensaje: "❌ Lo sentimos, tu cuenta ha sido bloqueada. Contacta a soporte."

**Nivel 2: Estado del Cliente**
- Consulta tabla de clientes
- Verifica que el estado del cliente sea 'activo'
- Si es 'bloqueado' o 'suspendido', se rechaza
- Mensaje: "❌ Tu cuenta está suspendida. Contacta a soporte."

**Nivel 3: Licencia/Suscripción (si tiene adeudos)**
- Verifica campo de fecha de vencimiento
- Si la fecha es anterior a HOY, se rechaza
- Mensaje: "❌ Tu licencia ha vencido. Renueva tu suscripción."

**Nivel 4: Límite Mensual**
- Compara tickets del mes actual con límite mensual
- Si se alcanzó el límite, se rechaza
- Mensaje: "❌ Has alcanzado tu límite mensual de tickets."

### 4.6 Stored Procedures

#### 4.6.1 sp_ValidarClienteDuplicado

```sql
DELIMITER $$

CREATE PROCEDURE sp_ValidarClienteDuplicado(
  IN p_Telefono VARCHAR(50),
  IN p_Email VARCHAR(255),
  IN p_IP VARCHAR(50),
  IN p_IdEntidad INT,
  OUT p_TieneTicketAbierto BOOLEAN,
  OUT p_IdTicketAbierto INT
)
BEGIN
  -- Buscar tickets abiertos del cliente
  SELECT 
    COUNT(*) > 0,
    MAX(Id)
  INTO 
    p_TieneTicketAbierto,
    p_IdTicketAbierto
  FROM op_tickets_v2
  WHERE (
    TelefonoCliente = p_Telefono 
    OR EmailCliente = p_Email 
    OR IPOrigen = p_IP
  )
  AND Estado IN ('Abierto', 'EnProceso')
  AND IdEntidad = p_IdEntidad
  AND Activo = 1;
  
  -- Actualizar tabla de validación
  INSERT INTO op_ticket_validacion_cliente (
    IdEntidad, TelefonoCliente, EmailCliente, IPOrigen,
    TieneTicketAbierto, IdTicketAbierto, UltimaInteraccion,
    IdUsuarioCreacion
  ) VALUES (
    p_IdEntidad, p_Telefono, p_Email, p_IP,
    p_TieneTicketAbierto, p_IdTicketAbierto, NOW(),
    1
  )
  ON DUPLICATE KEY UPDATE
    TieneTicketAbierto = p_TieneTicketAbierto,
    IdTicketAbierto = p_IdTicketAbierto,
    UltimaInteraccion = NOW(),
    FechaUltimaActualizacion = NOW();
END$$

DELIMITER ;
```

#### 4.6.2 sp_EncolarNotificacionWhatsApp

```sql
DELIMITER $$

CREATE PROCEDURE sp_EncolarNotificacionWhatsApp(
  IN p_IdTicket INT,
  IN p_NumeroWhatsApp VARCHAR(50),
  IN p_TipoNotificacion VARCHAR(100),
  IN p_MensajeTexto TEXT,
  IN p_IdEntidad INT
)
BEGIN
  INSERT INTO op_ticket_notificaciones_whatsapp (
    IdEntidad, IdTicket, NumeroWhatsApp, TipoNotificacion,
    MensajeTexto, Estado, IntentosEnvio, ProximoIntento,
    IdUsuarioCreacion
  ) VALUES (
    p_IdEntidad, p_IdTicket, p_NumeroWhatsApp, p_TipoNotificacion,
    p_MensajeTexto, 'Pendiente', 0, NOW(),
    1
  );
END$$

DELIMITER ;
```

#### 4.6.3 sp_CalcularMetricasDiarias

```sql
DELIMITER $$

CREATE PROCEDURE sp_CalcularMetricasDiarias(
  IN p_Fecha DATE,
  IN p_IdEntidad INT
)
BEGIN
  -- Calcular métricas del día
  INSERT INTO op_ticket_metricas (
    IdEntidad, FechaMetrica, TipoAgregacion,
    TotalTicketsCreados, TotalTicketsResueltos, TotalTicketsResueltosIA,
    PorcentajeResolucionIA, TiempoPromedioResolucionMinutos,
    CSATPromedio, IdUsuarioCreacion
  )
  SELECT
    p_IdEntidad,
    p_Fecha,
    'Diaria',
    COUNT(*),
    SUM(CASE WHEN Estado = 'Resuelto' THEN 1 ELSE 0 END),
    SUM(CASE WHEN ResueltoporIA = 1 THEN 1 ELSE 0 END),
    (SUM(CASE WHEN ResueltoporIA = 1 THEN 1 ELSE 0 END) * 100.0 / COUNT(*)),
    AVG(TiempoResolucionMinutos),
    AVG(CSATScore),
    1
  FROM op_tickets_v2
  WHERE DATE(FechaCreacion) = p_Fecha
  AND IdEntidad = p_IdEntidad
  AND Activo = 1
  ON DUPLICATE KEY UPDATE
    TotalTicketsCreados = VALUES(TotalTicketsCreados),
    TotalTicketsResueltos = VALUES(TotalTicketsResueltos),
    TotalTicketsResueltosIA = VALUES(TotalTicketsResueltosIA),
    PorcentajeResolucionIA = VALUES(PorcentajeResolucionIA),
    TiempoPromedioResolucionMinutos = VALUES(TiempoPromedioResolucionMinutos),
    CSATPromedio = VALUES(CSATPromedio),
    FechaUltimaActualizacion = NOW();
END$$

DELIMITER ;
```

---

## 5. API BACKEND (.NET 8)

### 5.1 Ubicación y Estructura

**Proyecto:** JELA.API (.NET 8)  
**Ubicación:** `D:\DesarrolloWEB\JelaAzure\JelaWeb\JELA.API`  
**URL Base:** `https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net`

**Estructura de Archivos Nuevos:**
```
JELA.API/
├── Endpoints/
│   ├── WebhookEndpoints.cs (NUEVO)
│   ├── TicketValidationEndpoints.cs (NUEVO)
│   ├── TicketNotificationEndpoints.cs (NUEVO)
│   ├── TicketMetricsEndpoints.cs (NUEVO)
├── Services/
│   ├── TicketValidationService.cs (NUEVO)
│   ├── TicketNotificationService.cs (NUEVO)
│   ├── TicketMetricsService.cs (NUEVO)
│   ├── PromptTuningService.cs (NUEVO)
│   ├── YCloudService.cs (NUEVO)
│   ├── VapiService.cs (NUEVO)
├── BackgroundServices/
│   ├── TicketMonitoringBackgroundService.cs (NUEVO)
│   ├── TicketMetricsBackgroundService.cs (NUEVO)
│   ├── PromptTuningBackgroundService.cs (NUEVO)
│   ├── NotificationQueueBackgroundService.cs (NUEVO)
├── Models/
│   ├── TicketModels.cs (NUEVO)
│   └── WebhookModels.cs (NUEVO)
```

### 5.2 Modelos de Datos

**IMPORTANTE:** El API JELA.API ya utiliza **DTOs dinámicos** mediante `CrudDto` (ver `Models/CrudModels.cs`), lo que permite trabajar con cualquier estructura de datos sin necesidad de definir modelos estáticos. Esto es fundamental para la escalabilidad futura.

#### 5.2.1 Uso de DTOs Dinámicos Existentes

El API ya cuenta con:

```csharp
// Ya existe en Models/CrudModels.cs
public class CrudDto
{
    public Dictionary<string, CampoConTipo> Campos { get; set; } = new();
    
    public object? this[string key]
    {
        get => Campos.TryGetValue(key, out var campo) ? campo.Valor : null;
        set
        {
            var tipo = value?.GetType().FullName ?? "System.Object";
            Campos[key] = new CampoConTipo { Valor = value, Tipo = tipo };
        }
    }
}

public class CrudRequest
{
    public Dictionary<string, CampoConTipo> Campos { get; set; } = new();
}
```

**Ventajas de usar DTOs dinámicos:**
- ✅ No requiere modificar código al agregar campos nuevos a las tablas
- ✅ Soporta cualquier estructura de datos
- ✅ Facilita integración con sistemas externos
- ✅ Reduce mantenimiento de código

#### 5.2.2 Modelos Específicos Solo para Webhooks

**Solo se requieren modelos tipados para webhooks externos** (VAPI, YCloud, Firebase) ya que estos tienen estructuras fijas definidas por terceros.

**Archivo:** `Models/WebhookModels.cs` (NUEVO)

```csharp
namespace JELA.API.Models;

// ============================================================================
// WEBHOOK REQUEST MODELS - Estructuras fijas de APIs externas
// ============================================================================

/// <summary>
/// Webhook de VAPI para llamadas telefónicas
/// </summary>
public record VapiWebhookRequest(
    string CallId,
    string PhoneNumber,
    string Transcription,
    int DurationSeconds,
    string Status,
    string? DisconnectReason
);

/// <summary>
/// Webhook de YCloud para WhatsApp
/// </summary>
public record YCloudWebhookRequest(
    string MessageId,
    string From,
    string To,
    string Text,
    DateTime Timestamp,
    string Type
);

/// <summary>
/// Webhook de Telegram Bot
/// </summary>
public record TelegramWebhookRequest(
    long UpdateId,
    TelegramMessage? Message
);

public record TelegramMessage(
    long MessageId,
    TelegramUser From,
    TelegramChat Chat,
    long Date,
    string? Text
);

public record TelegramUser(
    long Id,
    bool IsBot,
    string FirstName,
    string? LastName,
    string? Username
);

public record TelegramChat(
    long Id,
    string Type,
    string? FirstName,
    string? LastName,
    string? Username
);

/// <summary>
/// Request de Chat Web
/// </summary>
public record ChatWebRequest(
    string Email,
    string Nombre,
    string Mensaje,
    string IPOrigen,
    int IdEntidad
);

/// <summary>
/// Webhook de Firebase para Chat App
/// </summary>
public record FirebaseWebhookRequest(
    string UserId,
    string Message,
    DateTime Timestamp,
    string DeviceId
);

// ============================================================================
// WEBHOOK RESPONSE MODELS
// ============================================================================

/// <summary>
/// Respuesta estándar para webhooks
/// </summary>
public record WebhookResponse(
    bool Success,
    int? TicketId,
    string? Mensaje,
    string? RespuestaIA
);
```

#### 5.2.3 Uso de CrudDto en Servicios

**Ejemplo de cómo usar CrudDto en los servicios:**

```csharp
// En lugar de DTOs estáticos, usar CrudDto dinámico
public async Task<IEnumerable<CrudDto>> ObtenerTickets(DateTime fechaInicio, DateTime fechaFin)
{
    var query = @"
        SELECT * FROM op_tickets_v2 
        WHERE FechaCreacion BETWEEN @FechaInicio AND @FechaFin 
        AND Activo = 1";
    
    var parametros = new Dictionary<string, object>
    {
        { "@FechaInicio", fechaInicio },
        { "@FechaFin", fechaFin }
    };
    
    // Retorna CrudDto dinámico - soporta cualquier campo
    return await _db.EjecutarConsultaAsync(query, parametros);
}

// Acceder a campos dinámicamente
public async Task ProcesarTicket(CrudDto ticket)
{
    var id = ticket["Id"];
    var asunto = ticket["AsuntoCorto"];
    var tipoTicket = ticket["TipoTicket"]; // Campo nuevo - funciona sin cambios
    var riesgoFraude = ticket["RiesgoFraude"]; // Campo nuevo - funciona sin cambios
    
    // El tipo se preserva automáticamente
    var tipo = ticket.TipoDe("RiesgoFraude"); // "System.Boolean"
}

// Insertar con campos dinámicos
public async Task<int> CrearTicket(Dictionary<string, object> campos)
{
    // Funciona con cualquier campo - presente o futuro
    return await _db.InsertarAsync("op_tickets_v2", campos);
}
```

#### 5.2.4 Ventajas del Enfoque Dinámico

**Escenario 1: Agregar campo nuevo a op_tickets_v2**
```sql
-- Se agrega campo nuevo en BD
ALTER TABLE op_tickets_v2 ADD COLUMN NuevoCampo VARCHAR(100);
```

**Sin cambios en código:**
- ✅ El API automáticamente lo incluye en las consultas
- ✅ Los endpoints CRUD lo soportan inmediatamente
- ✅ Las páginas VB.NET lo reciben automáticamente
- ✅ Los grids lo muestran con columnas dinámicas

**Escenario 2: Integración con sistema externo**
```csharp
// Sistema externo envía estructura desconocida
var datosExternos = new Dictionary<string, object>
{
    { "CampoExterno1", "valor" },
    { "CampoExterno2", 123 },
    { "CampoNuevo", DateTime.Now }
};

// Se inserta sin problemas
await _db.InsertarAsync("tabla_integracion", datosExternos);
```

**NO se requieren modelos estáticos para:**
- ❌ Tickets (usar CrudDto o Dictionary<string, object>)
- ❌ Métricas (usar CrudDto o Dictionary<string, object>)
- ❌ Logs (usar CrudDto o Dictionary<string, object>)
- ❌ Validaciones (usar Dictionary<string, object>)
- ❌ Notificaciones (usar CrudDto o Dictionary<string, object>)

**Solo se requieren modelos tipados para:**
- ✅ Webhooks de APIs externas (estructura fija)
- ✅ Requests de validación específicos (si se necesita validación fuerte)

**RESUMEN DE CAMBIOS A DTOs DINÁMICOS:**

Todos los servicios y endpoints han sido actualizados para usar DTOs dinámicos:

1. **TicketValidationService:**
   - `ValidarClienteDuplicado()` → Retorna `Dictionary<string, object>`
   - `ObtenerHistorialCliente()` → Retorna `IEnumerable<CrudDto>`

2. **TicketMetricsService:**
   - `ObtenerMetricasTiempoReal()` → Retorna `CrudDto`
   - `ObtenerMetricasDiarias()` → Retorna `IEnumerable<CrudDto>`

3. **Páginas VB.NET:**
   - Deserializan como `Dictionary(Of String, Object)` o `List(Of Dictionary(Of String, Object))`
   - Usan `ConvertirDiccionariosADataTable()` para convertir a DataTable
   - NO requieren clases DTO estáticas

4. **Ventajas:**
   - ✅ Agregar campos nuevos sin modificar código
   - ✅ Soporta cualquier estructura de datos
   - ✅ Facilita integraciones futuras
   - ✅ Reduce mantenimiento


### 5.3 Servicios de Lógica de Negocio

**REGLA CRÍTICA**: NUNCA hardcodear prompts en código. SIEMPRE usar `IPromptTuningService.ObtenerPromptPorNombreAsync()` para obtener prompts desde la tabla `conf_ticket_prompts`. Si un prompt no existe, el sistema DEBE lanzar una excepción `InvalidOperationException`.

**Razón**: Los prompts deben ser configurables y ajustables sin necesidad de recompilar y redesplegar el código. Esto permite:
- ✅ Ajustar prompts en tiempo real sin downtime
- ✅ A/B testing de diferentes versiones de prompts
- ✅ Mejora continua basada en métricas
- ✅ Personalización por entidad
- ✅ Auditoría de cambios en prompts
- ✅ Sistema 100% dinámico sin código estático
- ✅ Fuerza configuración correcta en BD

**Filosofía del Sistema 100% Dinámico:**

El sistema está diseñado para ser completamente dinámico, eliminando cualquier configuración estática que requiera recompilación. Esto facilita:

1. **Mantenimiento Simplificado:**
   - Cambios de configuración sin redespliegue
   - Ajustes en producción sin downtime
   - Rollback instantáneo de cambios

2. **Escalabilidad:**
   - Agregar nuevos canales sin modificar código
   - Personalización por entidad sin duplicar código
   - Expansión a nuevos idiomas sin cambios en API

3. **Crecimiento Futuro:**
   - Nuevas funcionalidades mediante configuración
   - Integración con nuevos servicios externos
   - Adaptación a cambios de negocio sin desarrollo

4. **Detección Temprana de Errores:**
   - Sistema falla rápido con mensajes claros
   - Imposible olvidar configuración en BD
   - Errores detectados en desarrollo, no en producción

**Implementación correcta:**

```csharp
// ✅ CORRECTO - Cargar desde BD y validar
var promptSistema = await promptService.ObtenerPromptPorNombreAsync("ChatWebSistema", idEntidad);
var promptUsuario = await promptService.ObtenerPromptPorNombreAsync("ChatWebUsuario", idEntidad);

// Validar que existan - NO usar fallbacks
if (string.IsNullOrEmpty(promptSistema))
{
    logger.LogError("Prompt ChatWebSistema no encontrado en BD");
    throw new InvalidOperationException(
        "Prompt 'ChatWebSistema' no encontrado en conf_ticket_prompts. " +
        "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
}

if (string.IsNullOrEmpty(promptUsuario))
{
    logger.LogError("Prompt ChatWebUsuario no encontrado en BD");
    throw new InvalidOperationException(
        "Prompt 'ChatWebUsuario' no encontrado en conf_ticket_prompts. " +
        "Ejecute el script insert-prompts-iniciales.sql para configurar los prompts.");
}
```

```csharp
// ❌ INCORRECTO - Hardcodeado o con fallback
var promptSistema = @"Eres un asistente..."; // NO HACER ESTO

// ❌ INCORRECTO - Fallback silencioso
if (string.IsNullOrEmpty(promptSistema))
{
    promptSistema = "Prompt por defecto..."; // NO HACER ESTO
}
```

**Comportamiento esperado:**
- ✅ Si el prompt existe en BD: Sistema funciona normalmente
- ❌ Si el prompt NO existe: Sistema lanza `InvalidOperationException` con mensaje claro
- ❌ Si hay error de BD: Sistema lanza excepción de conexión
- ✅ Logs claros en todos los casos

**Ventajas del enfoque sin fallbacks:**

1. **Configuración Forzada:**
   - Desarrolladores no pueden olvidar configurar BD
   - Ambiente de desarrollo requiere misma configuración que producción
   - Errores de configuración detectados inmediatamente

2. **Código Más Limpio:**
   - Sin duplicación de prompts (BD + código)
   - Sin lógica condicional de fallback
   - Única fuente de verdad (BD)

3. **Auditoría Completa:**
   - Todos los prompts en `conf_ticket_prompts`
   - Historial de cambios en `op_ticket_prompt_ajustes_log`
   - Trazabilidad completa de modificaciones

4. **Testing Más Confiable:**
   - Tests usan mismos prompts que producción
   - No hay comportamiento diferente por fallbacks
   - Validación de configuración en CI/CD

**Nombres de prompts por canal:**
- VAPI: `VAPISistema`, `VAPIUsuario`
- YCloud: `YCloudSistema`, `YCloudUsuario`
- ChatWeb: `ChatWebSistema`, `ChatWebUsuario`
- Firebase: `FirebaseSistema`, `FirebaseUsuario`

**Script de inicialización:**
```bash
# Ejecutar ANTES de publicar API por primera vez
mysql -h servidor -u usuario -p base_datos < JELA.API/insert-prompts-iniciales.sql
```

**Verificación:**
```sql
-- Verificar que todos los prompts existen
SELECT NombrePrompt, Activo, FechaCreacion 
FROM conf_ticket_prompts 
WHERE NombrePrompt IN (
    'VAPISistema', 'VAPIUsuario',
    'YCloudSistema', 'YCloudUsuario',
    'ChatWebSistema', 'ChatWebUsuario',
    'FirebaseSistema', 'FirebaseUsuario'
);
-- Debe retornar 8 filas
```

#### 5.3.1 ITicketValidationService y TicketValidationService

**Archivo:** `Services/TicketValidationService.cs`

```csharp
namespace JELA.API.Services;

public interface ITicketValidationService
{
    Task<Dictionary<string, object>> ValidarClienteDuplicado(
        string? telefono, string? email, string? ip, int idEntidad);
    Task<IEnumerable<CrudDto>> ObtenerHistorialCliente(string telefono);
    Task<bool> ClienteBloqueado(string telefono, string email);
}

public class TicketValidationService : ITicketValidationService
{
    private readonly IDatabaseService _db;
    private readonly ILogger<TicketValidationService> _logger;
    
    public TicketValidationService(
        IDatabaseService db, 
        ILogger<TicketValidationService> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task<Dictionary<string, object>> ValidarClienteDuplicado(
        string? telefono, string? email, string? ip, int idEntidad)
    {
        try
        {
            _logger.LogInformation(
                "Validando cliente: Tel={Telefono}, Email={Email}, IP={IP}", 
                telefono, email, ip);
            
            var query = @"
                SELECT 
                    COUNT(*) > 0 as TieneTicketAbierto,
                    MAX(Id) as IdTicketAbierto
                FROM op_tickets_v2 
                WHERE (
                    (@Telefono IS NOT NULL AND TelefonoCliente = @Telefono) OR
                    (@Email IS NOT NULL AND EmailCliente = @Email) OR
                    (@IP IS NOT NULL AND IPOrigen = @IP)
                )
                AND Estado IN ('Abierto', 'EnProceso')
                AND IdEntidad = @IdEntidad
                AND Activo = 1";
            
            var parametros = new Dictionary<string, object>
            {
                { "@Telefono", telefono ?? (object)DBNull.Value },
                { "@Email", email ?? (object)DBNull.Value },
                { "@IP", ip ?? (object)DBNull.Value },
                { "@IdEntidad", idEntidad }
            };
            
            // Usar CrudDto dinámico
            var resultados = await _db.EjecutarConsultaAsync(query, parametros);
            var result = resultados.FirstOrDefault();
            
            bool tieneTicket = result?["TieneTicketAbierto"] as bool? ?? false;
            int? idTicket = result?["IdTicketAbierto"] as int?;
            
            // Registrar en tabla de validación
            await RegistrarValidacion(telefono, email, ip, idEntidad, tieneTicket, idTicket);
            
            // Retornar diccionario dinámico
            return new Dictionary<string, object>
            {
                { "TieneTicketAbierto", tieneTicket },
                { "IdTicketAbierto", idTicket ?? 0 },
                { "Mensaje", tieneTicket 
                    ? $"Cliente ya tiene ticket abierto #{idTicket}"
                    : "Cliente puede crear nuevo ticket" }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validando cliente duplicado");
            throw;
        }
    }
    
    public async Task<IEnumerable<CrudDto>> ObtenerHistorialCliente(string telefono)
    {
        var query = @"
            SELECT * FROM op_tickets_v2 
            WHERE TelefonoCliente = @Telefono 
            ORDER BY FechaCreacion DESC 
            LIMIT 10";
        
        var parametros = new Dictionary<string, object>
        {
            { "@Telefono", telefono }
        };
        
        // Retorna CrudDto dinámico - soporta todos los campos actuales y futuros
        return await _db.EjecutarConsultaAsync(query, parametros);
    }
    
    public async Task<bool> ClienteBloqueado(string telefono, string email)
    {
        var query = @"
            SELECT Bloqueado 
            FROM op_ticket_validacion_cliente 
            WHERE (TelefonoCliente = @Telefono OR EmailCliente = @Email)
            AND Bloqueado = 1 
            AND Activo = 1
            LIMIT 1";
        
        var bloqueado = await _db.QueryFirstOrDefaultAsync<bool?>(
            query, 
            new { Telefono = telefono, Email = email });
        
        return bloqueado ?? false;
    }
    
    private async Task RegistrarValidacion(
        string? telefono, string? email, string? ip, 
        int idEntidad, bool tieneTicket, int? idTicket)
    {
        var query = @"
            INSERT INTO op_ticket_validacion_cliente (
                IdEntidad, TelefonoCliente, EmailCliente, IPOrigen,
                TieneTicketAbierto, IdTicketAbierto, UltimaInteraccion,
                IdUsuarioCreacion, FechaCreacion
            ) VALUES (
                @IdEntidad, @Telefono, @Email, @IP,
                @TieneTicket, @IdTicket, NOW(), 1, NOW()
            )";
        
        await _db.ExecuteAsync(query, new
        {
            IdEntidad = idEntidad,
            Telefono = telefono,
            Email = email,
            IP = ip,
            TieneTicket = tieneTicket,
            IdTicket = idTicket
        });
    }
}
```


#### 5.3.2 IYCloudService y YCloudService

**Archivo:** `Services/YCloudService.cs`

```csharp
namespace JELA.API.Services;

public interface IYCloudService
{
    Task<string> EnviarMensajeWhatsApp(string numero, string mensaje);
    Task<bool> ValidarWebhookSignature(string signature, string payload);
}

public class YCloudService : IYCloudService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<YCloudService> _logger;
    private readonly string _apiKey;
    private readonly string _apiUrl;
    
    public YCloudService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<YCloudService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
        _apiKey = configuration["YCloud:ApiKey"] ?? throw new InvalidOperationException("YCloud ApiKey not configured");
        _apiUrl = configuration["YCloud:ApiUrl"] ?? "https://api.ycloud.com/v2";
    }
    
    public async Task<string> EnviarMensajeWhatsApp(string numero, string mensaje)
    {
        try
        {
            _logger.LogInformation("Enviando mensaje WhatsApp a {Numero}", numero);
            
            var request = new
            {
                to = numero,
                type = "text",
                text = new { body = mensaje }
            };
            
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");
            
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            
            var response = await _httpClient.PostAsync(
                $"{_apiUrl}/whatsapp/messages", 
                content);
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseContent);
                var messageId = result.GetProperty("id").GetString();
                
                _logger.LogInformation(
                    "Mensaje WhatsApp enviado exitosamente. MessageId={MessageId}", 
                    messageId);
                
                return messageId ?? string.Empty;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Error enviando mensaje WhatsApp: {Error}", error);
                throw new Exception($"Error YCloud: {error}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en EnviarMensajeWhatsApp");
            throw;
        }
    }
    
    public Task<bool> ValidarWebhookSignature(string signature, string payload)
    {
        // Implementar validación de firma según documentación de YCloud
        // Por ahora retornamos true
        return Task.FromResult(true);
    }
}
```

#### 5.3.3 ITicketMetricsService y TicketMetricsService

**Archivo:** `Services/TicketMetricsService.cs`

```csharp
namespace JELA.API.Services;

public interface ITicketMetricsService
{
    Task<CrudDto> ObtenerMetricasTiempoReal(int? idEntidad);
    Task<IEnumerable<CrudDto>> ObtenerMetricasDiarias(
        DateTime fechaInicio, DateTime fechaFin, int? idEntidad);
    Task CalcularMetricasDiarias(DateTime fecha, int idEntidad);
}

public class TicketMetricsService : ITicketMetricsService
{
    private readonly IDatabaseService _db;
    private readonly ILogger<TicketMetricsService> _logger;
    
    public TicketMetricsService(
        IDatabaseService db,
        ILogger<TicketMetricsService> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task<CrudDto> ObtenerMetricasTiempoReal(int? idEntidad)
    {
        var query = @"
            SELECT 
                COUNT(*) as TotalTickets,
                SUM(CASE WHEN Estado = 'Abierto' THEN 1 ELSE 0 END) as TicketsAbiertos,
                SUM(CASE WHEN Estado = 'Resuelto' THEN 1 ELSE 0 END) as TicketsResueltos,
                SUM(CASE WHEN ResueltoporIA = 1 THEN 1 ELSE 0 END) as ResueltosIA,
                AVG(TiempoResolucionMinutos) as TiempoPromedioResolucion,
                AVG(CSATScore) as CSATPromedio
            FROM op_tickets_v2
            WHERE DATE(FechaCreacion) = CURDATE()
            AND (@IdEntidad IS NULL OR IdEntidad = @IdEntidad)
            AND Activo = 1";
        
        var parametros = new Dictionary<string, object>
        {
            { "@IdEntidad", idEntidad ?? (object)DBNull.Value }
        };
        
        // Retorna CrudDto dinámico
        var resultados = await _db.EjecutarConsultaAsync(query, parametros);
        var metricas = resultados.FirstOrDefault();
        
        // Si no hay datos, retornar métricas en cero
        if (metricas == null)
        {
            var metricasVacias = new CrudDto();
            metricasVacias["TotalTickets"] = 0;
            metricasVacias["TicketsAbiertos"] = 0;
            metricasVacias["TicketsResueltos"] = 0;
            metricasVacias["ResueltosIA"] = 0;
            metricasVacias["TiempoPromedioResolucion"] = null;
            metricasVacias["CSATPromedio"] = null;
            return metricasVacias;
        }
        
        return metricas;
    }
    
    public async Task<IEnumerable<CrudDto>> ObtenerMetricasDiarias(
        DateTime fechaInicio, DateTime fechaFin, int? idEntidad)
    {
        var query = @"
            SELECT 
                FechaMetrica, Canal, TotalTicketsCreados, TotalTicketsResueltos,
                TotalTicketsResueltosIA, PorcentajeResolucionIA,
                TiempoPromedioResolucionMinutos, CSATPromedio
            FROM op_ticket_metricas 
            WHERE TipoAgregacion = 'Diaria'
            AND FechaMetrica BETWEEN @FechaInicio AND @FechaFin
            AND (@IdEntidad IS NULL OR IdEntidad = @IdEntidad)
            AND Activo = 1
            ORDER BY FechaMetrica DESC";
        
        var parametros = new Dictionary<string, object>
        {
            { "@FechaInicio", fechaInicio },
            { "@FechaFin", fechaFin },
            { "@IdEntidad", idEntidad ?? (object)DBNull.Value }
        };
        
        // Retorna CrudDto dinámico - soporta todos los campos
        return await _db.EjecutarConsultaAsync(query, parametros);
    }
    
    public async Task CalcularMetricasDiarias(DateTime fecha, int idEntidad)
    {
        try
        {
            _logger.LogInformation(
                "Calculando métricas diarias para fecha {Fecha}, entidad {IdEntidad}", 
                fecha, idEntidad);
            
            await _db.ExecuteAsync(
                "CALL sp_CalcularMetricasDiarias(@Fecha, @IdEntidad)",
                new { Fecha = fecha, IdEntidad = idEntidad });
            
            _logger.LogInformation("Métricas calculadas exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculando métricas diarias");
            throw;
        }
    }
}
```


### 5.4 Endpoints

#### 5.4.1 WebhookEndpoints

**Archivo:** `Endpoints/WebhookEndpoints.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace JELA.API.Endpoints;

public static class WebhookEndpoints
{
    public static void MapWebhookEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/webhooks")
            .WithTags("Webhooks")
            .RequireRateLimiting("fixed");

        // POST /api/webhooks/vapi
        group.MapPost("/vapi", async (
            [FromBody] VapiWebhookRequest request,
            [FromServices] ITicketValidationService validationService,
            [FromServices] IOpenAIService openAIService,
            [FromServices] IDatabaseService db,
            [FromServices] ILogger<Program> logger) =>
        {
            try
            {
                logger.LogInformation("Webhook VAPI recibido: {CallId}", request.CallId);
                
                // Validar cliente duplicado
                var validation = await validationService.ValidarClienteDuplicado(
                    request.PhoneNumber, null, null, 1);
                
                if (validation.TieneTicketAbierto)
                {
                    return Results.Ok(new WebhookResponse(
                        true, 
                        validation.IdTicketAbierto, 
                        validation.Mensaje, 
                        null));
                }
                
                // Procesar con IA
                var iaResponse = await openAIService.ProcesarTicket(
                    request.Transcription, "VAPI");
                
                // Crear ticket
                var ticketId = await CrearTicket(db, request, iaResponse, 1);
                
                // Registrar log de interacción
                await RegistrarInteraccion(db, ticketId, "VAPI", request);
                
                return Results.Ok(new WebhookResponse(
                    true, ticketId, "Ticket creado", iaResponse.Respuesta));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error procesando webhook VAPI");
                return Results.Problem("Error procesando llamada");
            }
        })
        .WithName("WebhookVAPI")
        .WithOpenApi();

        // POST /api/webhooks/ycloud
        group.MapPost("/ycloud", async (
            [FromBody] YCloudWebhookRequest request,
            [FromServices] ITicketValidationService validationService,
            [FromServices] IOpenAIService openAIService,
            [FromServices] IDatabaseService db,
            [FromServices] IYCloudService yCloudService,
            [FromServices] ILogger<Program> logger) =>
        {
            try
            {
                logger.LogInformation("Webhook YCloud recibido: {MessageId}", request.MessageId);
                
                // Validar cliente
                var validation = await validationService.ValidarClienteDuplicado(
                    request.From, null, null, 1);
                
                if (validation.TieneTicketAbierto)
                {
                    // Enviar respuesta de ticket existente
                    await yCloudService.EnviarMensajeWhatsApp(
                        request.From, 
                        validation.Mensaje);
                    
                    return Results.Ok(new WebhookResponse(
                        true, validation.IdTicketAbierto, validation.Mensaje, null));
                }
                
                // Procesar con IA
                var iaResponse = await openAIService.ProcesarTicket(
                    request.Text, "WhatsApp");
                
                // Crear ticket
                var ticketId = await CrearTicket(db, request, iaResponse, 1);
                
                // Registrar interacción
                await RegistrarInteraccion(db, ticketId, "YCloud", request);
                
                // Enviar respuesta automática
                await yCloudService.EnviarMensajeWhatsApp(
                    request.From, 
                    iaResponse.Respuesta);
                
                return Results.Ok(new WebhookResponse(
                    true, ticketId, "Ticket creado", iaResponse.Respuesta));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error procesando webhook YCloud");
                return Results.Problem("Error procesando mensaje WhatsApp");
            }
        })
        .WithName("WebhookYCloud")
        .WithOpenApi();

        // POST /api/webhooks/chatweb
        group.MapPost("/chatweb", async (
            [FromBody] ChatWebRequest request,
            [FromServices] ITicketValidationService validationService,
            [FromServices] IOpenAIService openAIService,
            [FromServices] IDatabaseService db,
            [FromServices] ILogger<Program> logger) =>
        {
            try
            {
                logger.LogInformation("Mensaje Chat Web recibido de: {Email}", request.Email);
                
                // Validar cliente
                var validation = await validationService.ValidarClienteDuplicado(
                    null, request.Email, request.IPOrigen, request.IdEntidad);
                
                if (validation.TieneTicketAbierto)
                {
                    return Results.Ok(new WebhookResponse(
                        true, validation.IdTicketAbierto, validation.Mensaje, null));
                }
                
                // Procesar con IA
                var iaResponse = await openAIService.ProcesarTicket(
                    request.Mensaje, "ChatWeb");
                
                // Crear ticket
                var ticketId = await CrearTicket(db, request, iaResponse, request.IdEntidad);
                
                // Registrar interacción
                await RegistrarInteraccion(db, ticketId, "ChatWeb", request);
                
                return Results.Ok(new WebhookResponse(
                    true, ticketId, "Ticket creado", iaResponse.Respuesta));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error procesando chat web");
                return Results.Problem("Error procesando mensaje");
            }
        })
        .WithName("WebhookChatWeb")
        .WithOpenApi();
    }
    
    // Métodos auxiliares privados
    private static async Task<int> CrearTicket(
        IDatabaseService db, 
        object request, 
        dynamic iaResponse, 
        int idEntidad)
    {
        // Implementación de creación de ticket
        var query = @"
            INSERT INTO op_tickets_v2 (
                IdEntidad, AsuntoCorto, MensajeOriginal, Canal,
                NombreCompleto, EmailCliente, TelefonoCliente,
                CategoriaAsignada, SentimientoDetectado, PrioridadAsignada,
                RespuestaIA, Estado, IdUsuarioCreacion, FechaCreacion
            ) VALUES (
                @IdEntidad, @Asunto, @Mensaje, @Canal,
                @Nombre, @Email, @Telefono,
                @Categoria, @Sentimiento, @Prioridad,
                @RespuestaIA, 'Abierto', 1, NOW()
            );
            SELECT LAST_INSERT_ID();";
        
        // Extraer datos según tipo de request
        // ... (implementación específica)
        
        var ticketId = await db.ExecuteScalarAsync<int>(query, new { /* parámetros */ });
        return ticketId;
    }
    
    private static async Task RegistrarInteraccion(
        IDatabaseService db,
        int ticketId,
        string canal,
        object request)
    {
        var query = @"
            INSERT INTO op_ticket_logs_interacciones (
                IdEntidad, IdTicket, Canal, TipoInteraccion,
                DatosInteraccion, Exitosa, IdUsuarioCreacion, FechaCreacion
            ) VALUES (
                1, @TicketId, @Canal, 'MensajeRecibido',
                @Datos, 1, 1, NOW()
            )";
        
        await db.ExecuteAsync(query, new
        {
            TicketId = ticketId,
            Canal = canal,
            Datos = JsonSerializer.Serialize(request)
        });
    }
}
```


#### 5.4.2 TicketValidationEndpoints

**Archivo:** `Endpoints/TicketValidationEndpoints.cs`

```csharp
namespace JELA.API.Endpoints;

public static class TicketValidationEndpoints
{
    public static void MapTicketValidationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tickets")
            .WithTags("Ticket Validation")
            .RequireAuthorization()
            .RequireRateLimiting("fixed");

        // POST /api/tickets/validar-cliente
        group.MapPost("/validar-cliente", async (
            [FromBody] ValidarClienteRequest request,
            [FromServices] ITicketValidationService validationService) =>
        {
            var result = await validationService.ValidarClienteDuplicado(
                request.Telefono,
                request.Email,
                request.IPOrigen,
                request.IdEntidad);
            
            return Results.Ok(result);
        })
        .WithName("ValidarCliente")
        .WithOpenApi();

        // GET /api/tickets/historial/{telefono}
        group.MapGet("/historial/{telefono}", async (
            string telefono,
            [FromServices] ITicketValidationService validationService) =>
        {
            var tickets = await validationService.ObtenerHistorialCliente(telefono);
            return Results.Ok(tickets);
        })
        .WithName("HistorialCliente")
        .WithOpenApi();
    }
}
```

#### 5.4.3 TicketNotificationEndpoints

**Archivo:** `Endpoints/TicketNotificationEndpoints.cs`

```csharp
namespace JELA.API.Endpoints;

public static class TicketNotificationEndpoints
{
    public static void MapTicketNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tickets")
            .WithTags("Notifications")
            .RequireAuthorization()
            .RequireRateLimiting("fixed");

        // POST /api/tickets/notificar-whatsapp
        group.MapPost("/notificar-whatsapp", async (
            [FromBody] NotificarWhatsAppRequest request,
            [FromServices] IDatabaseService db,
            [FromServices] ILogger<Program> logger) =>
        {
            try
            {
                await db.ExecuteAsync(
                    "CALL sp_EncolarNotificacionWhatsApp(@IdTicket, @Numero, @Tipo, @Mensaje, @IdEntidad)",
                    new
                    {
                        IdTicket = request.IdTicket,
                        Numero = request.NumeroWhatsApp,
                        Tipo = request.TipoNotificacion,
                        Mensaje = request.MensajeTexto,
                        IdEntidad = request.IdEntidad
                    });
                
                logger.LogInformation(
                    "Notificación WhatsApp encolada para ticket {TicketId}", 
                    request.IdTicket);
                
                return Results.Ok(new { success = true, mensaje = "Notificación encolada" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error encolando notificación");
                return Results.Problem("Error encolando notificación");
            }
        })
        .WithName("NotificarWhatsApp")
        .WithOpenApi();

        // GET /api/tickets/notificaciones/cola
        group.MapGet("/notificaciones/cola", async (
            [FromQuery] int? idEntidad,
            [FromServices] IDatabaseService db) =>
        {
            var query = @"
                SELECT 
                    Id, IdTicket, NumeroWhatsApp, TipoNotificacion,
                    MensajeTexto, Estado, IntentosEnvio, ProximoIntento
                FROM op_ticket_notificaciones_whatsapp 
                WHERE Estado = 'Pendiente' 
                AND (@IdEntidad IS NULL OR IdEntidad = @IdEntidad)
                AND (ProximoIntento IS NULL OR ProximoIntento <= NOW())
                AND Activo = 1
                ORDER BY FechaCreacion ASC 
                LIMIT 100";
            
            var notificaciones = await db.QueryAsync<NotificacionWhatsAppDto>(
                query, new { IdEntidad = idEntidad });
            
            return Results.Ok(notificaciones);
        })
        .WithName("ObtenerColaNotificaciones")
        .WithOpenApi();

        // PUT /api/tickets/notificaciones/{id}/estado
        group.MapPut("/notificaciones/{id}/estado", async (
            int id,
            [FromBody] ActualizarEstadoNotificacionRequest request,
            [FromServices] IDatabaseService db) =>
        {
            var query = @"
                UPDATE op_ticket_notificaciones_whatsapp 
                SET Estado = @Estado,
                    FechaEnvio = CASE WHEN @Estado = 'Enviado' THEN NOW() ELSE FechaEnvio END,
                    IdMensajeYCloud = COALESCE(@IdMensaje, IdMensajeYCloud),
                    RespuestaYCloudJSON = COALESCE(@Respuesta, RespuestaYCloudJSON),
                    IntentosEnvio = IntentosEnvio + 1,
                    MensajeError = @Error,
                    FechaUltimaActualizacion = NOW()
                WHERE Id = @Id";
            
            await db.ExecuteAsync(query, new
            {
                Id = id,
                Estado = request.Estado,
                IdMensaje = request.IdMensajeYCloud,
                Respuesta = request.RespuestaJSON,
                Error = request.MensajeError
            });
            
            return Results.Ok(new { success = true });
        })
        .WithName("ActualizarEstadoNotificacion")
        .WithOpenApi();
    }
}

public record ActualizarEstadoNotificacionRequest(
    string Estado,
    string? IdMensajeYCloud,
    string? RespuestaJSON,
    string? MensajeError
);
```

#### 5.4.4 TicketMetricsEndpoints

**Archivo:** `Endpoints/TicketMetricsEndpoints.cs`

```csharp
namespace JELA.API.Endpoints;

public static class TicketMetricsEndpoints
{
    public static void MapTicketMetricsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tickets/metricas")
            .WithTags("Metrics")
            .RequireAuthorization()
            .RequireRateLimiting("fixed");

        // GET /api/tickets/metricas/tiempo-real
        group.MapGet("/tiempo-real", async (
            [FromQuery] int? idEntidad,
            [FromServices] ITicketMetricsService metricsService) =>
        {
            var metricas = await metricsService.ObtenerMetricasTiempoReal(idEntidad);
            return Results.Ok(metricas);
        })
        .WithName("MetricasTiempoReal")
        .WithOpenApi();

        // GET /api/tickets/metricas/diarias
        group.MapGet("/diarias", async (
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin,
            [FromQuery] int? idEntidad,
            [FromServices] ITicketMetricsService metricsService) =>
        {
            var metricas = await metricsService.ObtenerMetricasDiarias(
                fechaInicio, fechaFin, idEntidad);
            
            return Results.Ok(metricas);
        })
        .WithName("MetricasDiarias")
        .WithOpenApi();

        // POST /api/tickets/metricas/calcular
        group.MapPost("/calcular", async (
            [FromBody] CalcularMetricasRequest request,
            [FromServices] ITicketMetricsService metricsService,
            [FromServices] ILogger<Program> logger) =>
        {
            try
            {
                await metricsService.CalcularMetricasDiarias(
                    request.Fecha, request.IdEntidad);
                
                logger.LogInformation(
                    "Métricas calculadas para fecha {Fecha}", request.Fecha);
                
                return Results.Ok(new { success = true, mensaje = "Métricas calculadas" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error calculando métricas");
                return Results.Problem("Error calculando métricas");
            }
        })
        .WithName("CalcularMetricas")
        .WithOpenApi();
    }
}
```

### 5.5 Registro en Program.cs

**Archivo:** `Program.cs` (agregar al final antes de `app.Run()`)

```csharp
// ============================================================================
// REGISTRAR SERVICIOS DEL MÓDULO DE TICKETS
// ============================================================================

// Servicios de lógica de negocio
builder.Services.AddScoped<ITicketValidationService, TicketValidationService>();
builder.Services.AddScoped<ITicketMetricsService, TicketMetricsService>();
builder.Services.AddScoped<IYCloudService, YCloudService>();

// HttpClient para YCloud
builder.Services.AddHttpClient<IYCloudService, YCloudService>();

// Background Services (tareas programadas)
builder.Services.AddHostedService<TicketMonitoringBackgroundService>();
builder.Services.AddHostedService<TicketMetricsBackgroundService>();
builder.Services.AddHostedService<NotificationQueueBackgroundService>();
builder.Services.AddHostedService<PromptTuningBackgroundService>();

// ============================================================================
// REGISTRAR ENDPOINTS DEL MÓDULO DE TICKETS
// ============================================================================

app.MapWebhookEndpoints();
app.MapTicketValidationEndpoints();
app.MapTicketNotificationEndpoints();
app.MapTicketMetricsEndpoints();
```

---

## 6. BACKGROUND SERVICES (.NET 8)

### 6.1 Introducción a Background Services

Los Background Services son servicios que se ejecutan en segundo plano dentro de JELA.API para realizar tareas programadas sin intervención del usuario. Heredan de `BackgroundService` de .NET y se ejecutan continuamente mientras la aplicación está activa.

**Ventajas:**
- Se ejecutan dentro del mismo proceso de la API
- Comparten el mismo contexto de dependencias
- Logging centralizado
- Fácil monitoreo y debugging
- No requieren servicios Windows separados

### 6.2 TicketMonitoringBackgroundService

**Propósito:** Monitorear tickets cada 5 minutos para detectar problemas y tomar acciones automáticas

**Archivo:** `BackgroundServices/TicketMonitoringBackgroundService.cs`

```csharp
namespace JELA.API.BackgroundServices;

public class TicketMonitoringBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TicketMonitoringBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
    
    public TicketMonitoringBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<TicketMonitoringBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TicketMonitoringBackgroundService iniciado");
        
        // Esperar 1 minuto antes de la primera ejecución
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MonitorearTickets(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("TicketMonitoringBackgroundService detenido");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo de monitoreo");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
    
    private async Task MonitorearTickets(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        var yCloudService = scope.ServiceProvider.GetRequiredService<IYCloudService>();
        
        var inicioEjecucion = DateTime.Now;
        var ticketsDetectados = 0;
        var accionesRealizadas = 0;
        var notificacionesEnviadas = 0;
        
        try
        {
            _logger.LogInformation("Iniciando monitoreo de tickets");
            
            // 1. Detectar tickets sin atender (más de 24 horas)
            var ticketsSinAtender = await DetectarTicketsSinAtender(db);
            ticketsDetectados += ticketsSinAtender.Count;
            
            foreach (var ticket in ticketsSinAtender)
            {
                await EscalarTicket(db, ticket.Id, "Ticket sin atender por más de 24 horas");
                accionesRealizadas++;
                
                // Notificar al supervisor
                await NotificarSupervisor(db, yCloudService, ticket);
                notificacionesEnviadas++;
            }
            
            // 2. Detectar tickets vencidos (según SLA)
            var ticketsVencidos = await DetectarTicketsVencidos(db);
            ticketsDetectados += ticketsVencidos.Count;
            
            foreach (var ticket in ticketsVencidos)
            {
                await MarcarComoVencido(db, ticket.Id);
                accionesRealizadas++;
                
                await NotificarSupervisor(db, yCloudService, ticket);
                notificacionesEnviadas++;
            }
            
            // 3. Detectar tickets con alta prioridad sin asignar
            var ticketsAltaPrioridad = await DetectarTicketsAltaPrioridadSinAsignar(db);
            ticketsDetectados += ticketsAltaPrioridad.Count;
            
            foreach (var ticket in ticketsAltaPrioridad)
            {
                await AsignarAutomaticamente(db, ticket.Id);
                accionesRealizadas++;
            }
            
            // 4. Detectar posibles tickets duplicados
            var ticketsDuplicados = await DetectarTicketsDuplicados(db);
            ticketsDetectados += ticketsDuplicados.Count;
            
            foreach (var grupo in ticketsDuplicados)
            {
                await MarcarComoDuplicados(db, grupo);
                accionesRealizadas++;
            }
            
            // Registrar ejecución
            var tiempoEjecucion = (int)(DateTime.Now - inicioEjecucion).TotalMilliseconds;
            await RegistrarEjecucion(db, "MonitoreoGeneral", ticketsDetectados, 
                accionesRealizadas, notificacionesEnviadas, tiempoEjecucion, true, null);
            
            _logger.LogInformation(
                "Monitoreo completado: {Detectados} tickets detectados, {Acciones} acciones realizadas",
                ticketsDetectados, accionesRealizadas);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en MonitorearTickets");
            
            var tiempoEjecucion = (int)(DateTime.Now - inicioEjecucion).TotalMilliseconds;
            await RegistrarEjecucion(db, "MonitoreoGeneral", ticketsDetectados,
                accionesRealizadas, notificacionesEnviadas, tiempoEjecucion, false, ex.Message);
            
            throw;
        }
    }
    
    private async Task<List<TicketMonitoreoDto>> DetectarTicketsSinAtender(IDatabaseService db)
    {
        var query = @"
            SELECT Id, AsuntoCorto, TelefonoCliente, EmailCliente, FechaCreacion
            FROM op_tickets_v2
            WHERE Estado = 'Abierto'
            AND IdAgenteAsignado IS NULL
            AND TIMESTAMPDIFF(HOUR, FechaCreacion, NOW()) >= 24
            AND Activo = 1";
        
        var tickets = await db.QueryAsync<TicketMonitoreoDto>(query);
        return tickets.ToList();
    }
    
    private async Task<List<TicketMonitoreoDto>> DetectarTicketsVencidos(IDatabaseService db)
    {
        var query = @"
            SELECT t.Id, t.AsuntoCorto, t.TelefonoCliente, t.EmailCliente, t.FechaCreacion
            FROM op_tickets_v2 t
            INNER JOIN conf_ticket_sla sla ON t.PrioridadAsignada = sla.Prioridad
            WHERE t.Estado IN ('Abierto', 'EnProceso')
            AND TIMESTAMPDIFF(HOUR, t.FechaCreacion, NOW()) > sla.TiempoRespuestaHoras
            AND t.Activo = 1";
        
        var tickets = await db.QueryAsync<TicketMonitoreoDto>(query);
        return tickets.ToList();
    }
    
    private async Task<List<TicketMonitoreoDto>> DetectarTicketsAltaPrioridadSinAsignar(IDatabaseService db)
    {
        var query = @"
            SELECT Id, AsuntoCorto, TelefonoCliente, EmailCliente, FechaCreacion
            FROM op_tickets_v2
            WHERE Estado = 'Abierto'
            AND PrioridadAsignada IN ('Alta', 'Crítica')
            AND IdAgenteAsignado IS NULL
            AND TIMESTAMPDIFF(MINUTE, FechaCreacion, NOW()) >= 30
            AND Activo = 1";
        
        var tickets = await db.QueryAsync<TicketMonitoreoDto>(query);
        return tickets.ToList();
    }
    
    private async Task<List<List<int>>> DetectarTicketsDuplicados(IDatabaseService db)
    {
        var query = @"
            SELECT GROUP_CONCAT(Id) as Ids
            FROM op_tickets_v2
            WHERE Estado = 'Abierto'
            AND FechaCreacion >= DATE_SUB(NOW(), INTERVAL 1 HOUR)
            AND Activo = 1
            GROUP BY TelefonoCliente, AsuntoCorto
            HAVING COUNT(*) > 1";
        
        var resultados = await db.QueryAsync<string>(query);
        return resultados
            .Select(ids => ids.Split(',').Select(int.Parse).ToList())
            .ToList();
    }
    
    private async Task EscalarTicket(IDatabaseService db, int ticketId, string motivo)
    {
        var query = @"
            UPDATE op_tickets_v2 
            SET RequiereEscalamiento = 1,
                FechaUltimaActualizacion = NOW()
            WHERE Id = @TicketId";
        
        await db.ExecuteAsync(query, new { TicketId = ticketId });
        
        // Registrar acción
        await RegistrarAccion(db, ticketId, "Escalamiento", motivo);
    }
    
    private async Task MarcarComoVencido(IDatabaseService db, int ticketId)
    {
        await RegistrarAccion(db, ticketId, "TicketVencido", "Ticket vencido según SLA");
    }
    
    private async Task AsignarAutomaticamente(IDatabaseService db, int ticketId)
    {
        // Buscar agente disponible con menos carga
        var query = @"
            SELECT u.Id
            FROM conf_usuarios u
            LEFT JOIN op_tickets_v2 t ON t.IdAgenteAsignado = u.Id AND t.Estado IN ('Abierto', 'EnProceso')
            WHERE u.Activo = 1
            GROUP BY u.Id
            ORDER BY COUNT(t.Id) ASC
            LIMIT 1";
        
        var agenteId = await db.QueryFirstOrDefaultAsync<int?>(query);
        
        if (agenteId.HasValue)
        {
            var updateQuery = @"
                UPDATE op_tickets_v2 
                SET IdAgenteAsignado = @AgenteId,
                    FechaAsignacionAgente = NOW(),
                    Estado = 'EnProceso',
                    FechaUltimaActualizacion = NOW()
                WHERE Id = @TicketId";
            
            await db.ExecuteAsync(updateQuery, new { TicketId = ticketId, AgenteId = agenteId });
            
            await RegistrarAccion(db, ticketId, "AsignacionAutomatica", 
                $"Asignado automáticamente a agente {agenteId}");
        }
    }
    
    private async Task MarcarComoDuplicados(IDatabaseService db, List<int> ticketIds)
    {
        var ticketPrincipal = ticketIds.First();
        var ticketsDuplicados = ticketIds.Skip(1);
        
        foreach (var ticketId in ticketsDuplicados)
        {
            var query = @"
                UPDATE op_tickets_v2 
                SET Estado = 'Cerrado',
                    FechaResolucion = NOW(),
                    FechaUltimaActualizacion = NOW()
                WHERE Id = @TicketId";
            
            await db.ExecuteAsync(query, new { TicketId = ticketId });
            
            await RegistrarAccion(db, ticketId, "MarcadoDuplicado", 
                $"Duplicado del ticket #{ticketPrincipal}");
        }
    }
    
    private async Task NotificarSupervisor(
        IDatabaseService db, 
        IYCloudService yCloudService, 
        TicketMonitoreoDto ticket)
    {
        // Obtener número del supervisor (configuración)
        var supervisorTelefono = "+521234567890"; // TODO: Obtener de configuración
        
        var mensaje = $"⚠️ ALERTA: Ticket #{ticket.Id} requiere atención.\n" +
                     $"Asunto: {ticket.AsuntoCorto}\n" +
                     $"Creado: {ticket.FechaCreacion:dd/MM/yyyy HH:mm}";
        
        try
        {
            await yCloudService.EnviarMensajeWhatsApp(supervisorTelefono, mensaje);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error enviando notificación a supervisor");
        }
    }
    
    private async Task RegistrarAccion(
        IDatabaseService db, 
        int ticketId, 
        string tipoAccion, 
        string descripcion)
    {
        var query = @"
            INSERT INTO op_ticket_acciones (
                IdTicket, TipoAccion, Descripcion, EsAccionIA, 
                IdUsuarioAccion, FechaAccion
            ) VALUES (
                @TicketId, @TipoAccion, @Descripcion, 1, NULL, NOW()
            )";
        
        await db.ExecuteAsync(query, new
        {
            TicketId = ticketId,
            TipoAccion = tipoAccion,
            Descripcion = descripcion
        });
    }
    
    private async Task RegistrarEjecucion(
        IDatabaseService db,
        string tipoMonitoreo,
        int ticketsDetectados,
        int accionesRealizadas,
        int notificacionesEnviadas,
        int tiempoEjecucionMs,
        bool exitoso,
        string? mensajeError)
    {
        var query = @"
            INSERT INTO op_ticket_robot_monitoreo (
                IdEntidad, FechaEjecucion, TipoMonitoreo, TicketsDetectados,
                AccionesRealizadas, NotificacionesEnviadas, TiempoEjecucionMs,
                Exitoso, MensajeError, IdUsuarioCreacion, FechaCreacion
            ) VALUES (
                1, NOW(), @TipoMonitoreo, @TicketsDetectados,
                @AccionesRealizadas, @NotificacionesEnviadas, @TiempoEjecucionMs,
                @Exitoso, @MensajeError, 1, NOW()
            )";
        
        await db.ExecuteAsync(query, new
        {
            TipoMonitoreo = tipoMonitoreo,
            TicketsDetectados = ticketsDetectados,
            AccionesRealizadas = accionesRealizadas,
            NotificacionesEnviadas = notificacionesEnviadas,
            TiempoEjecucionMs = tiempoEjecucionMs,
            Exitoso = exitoso,
            MensajeError = mensajeError
        });
    }
}

public record TicketMonitoreoDto(
    int Id,
    string AsuntoCorto,
    string? TelefonoCliente,
    string? EmailCliente,
    DateTime FechaCreacion
);
```



### 6.3 TicketMetricsBackgroundService

**Propósito:** Calcular métricas diarias cada hora

**Archivo:** `BackgroundServices/TicketMetricsBackgroundService.cs`

```csharp
namespace JELA.API.BackgroundServices;

public class TicketMetricsBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TicketMetricsBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);
    
    public TicketMetricsBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<TicketMetricsBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TicketMetricsBackgroundService iniciado");
        
        // Esperar 5 minutos antes de la primera ejecución
        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CalcularMetricas(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("TicketMetricsBackgroundService detenido");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo de cálculo de métricas");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }
    
    private async Task CalcularMetricas(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var metricsService = scope.ServiceProvider.GetRequiredService<ITicketMetricsService>();
        var db = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        
        try
        {
            _logger.LogInformation("Iniciando cálculo de métricas");
            
            // Calcular métricas del día anterior
            var fechaAyer = DateTime.Today.AddDays(-1);
            
            // Obtener todas las entidades activas
            var entidades = await ObtenerEntidadesActivas(db);
            
            foreach (var entidadId in entidades)
            {
                try
                {
                    await metricsService.CalcularMetricasDiarias(fechaAyer, entidadId);
                    _logger.LogInformation(
                        "Métricas calculadas para entidad {IdEntidad}, fecha {Fecha}",
                        entidadId, fechaAyer);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, 
                        "Error calculando métricas para entidad {IdEntidad}", entidadId);
                }
            }
            
            _logger.LogInformation("Cálculo de métricas completado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en CalcularMetricas");
            throw;
        }
    }
    
    private async Task<List<int>> ObtenerEntidadesActivas(IDatabaseService db)
    {
        var query = "SELECT Id FROM cat_entidades WHERE Activo = 1";
        var entidades = await db.QueryAsync<int>(query);
        return entidades.ToList();
    }
}
```


### 6.4 NotificationQueueBackgroundService

**Propósito:** Procesar cola de notificaciones WhatsApp cada 30 segundos

**Archivo:** `BackgroundServices/NotificationQueueBackgroundService.cs`

```csharp
namespace JELA.API.BackgroundServices;

public class NotificationQueueBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationQueueBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);
    
    public NotificationQueueBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<NotificationQueueBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("NotificationQueueBackgroundService iniciado");
        
        // Esperar 10 segundos antes de la primera ejecución
        await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcesarColaNotificaciones(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("NotificationQueueBackgroundService detenido");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo de procesamiento de notificaciones");
                await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
            }
        }
    }
    
    private async Task ProcesarColaNotificaciones(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        var yCloudService = scope.ServiceProvider.GetRequiredService<IYCloudService>();
        
        try
        {
            // Obtener notificaciones pendientes
            var notificaciones = await ObtenerNotificacionesPendientes(db);
            
            if (notificaciones.Count == 0)
                return;
            
            _logger.LogInformation(
                "Procesando {Count} notificaciones pendientes", notificaciones.Count);
            
            foreach (var notif in notificaciones)
            {
                try
                {
                    // Actualizar estado a "Enviando"
                    await ActualizarEstadoNotificacion(db, notif.Id, "Enviando", null, null, null);
                    
                    // Enviar mensaje
                    var messageId = await yCloudService.EnviarMensajeWhatsApp(
                        notif.NumeroWhatsApp, notif.MensajeTexto);
                    
                    // Actualizar estado a "Enviado"
                    await ActualizarEstadoNotificacion(
                        db, notif.Id, "Enviado", messageId, null, null);
                    
                    _logger.LogInformation(
                        "Notificación {Id} enviada exitosamente. MessageId={MessageId}",
                        notif.Id, messageId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error enviando notificación {Id}", notif.Id);
                    
                    // Actualizar estado según intentos
                    var nuevoEstado = notif.IntentosEnvio + 1 >= notif.MaximoIntentos 
                        ? "Fallido" 
                        : "Pendiente";
                    
                    var proximoIntento = nuevoEstado == "Pendiente"
                        ? DateTime.Now.AddMinutes(5 * (notif.IntentosEnvio + 1))
                        : (DateTime?)null;
                    
                    await ActualizarEstadoNotificacion(
                        db, notif.Id, nuevoEstado, null, proximoIntento, ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en ProcesarColaNotificaciones");
            throw;
        }
    }
    
    private async Task<List<NotificacionPendienteDto>> ObtenerNotificacionesPendientes(
        IDatabaseService db)
    {
        var query = @"
            SELECT 
                Id, IdTicket, NumeroWhatsApp, TipoNotificacion, MensajeTexto,
                IntentosEnvio, MaximoIntentos
            FROM op_ticket_notificaciones_whatsapp
            WHERE Estado = 'Pendiente'
            AND (ProximoIntento IS NULL OR ProximoIntento <= NOW())
            AND Activo = 1
            ORDER BY FechaCreacion ASC
            LIMIT 50";
        
        var notificaciones = await db.QueryAsync<NotificacionPendienteDto>(query);
        return notificaciones.ToList();
    }
    
    private async Task ActualizarEstadoNotificacion(
        IDatabaseService db,
        int id,
        string estado,
        string? idMensajeYCloud,
        DateTime? proximoIntento,
        string? mensajeError)
    {
        var query = @"
            UPDATE op_ticket_notificaciones_whatsapp
            SET Estado = @Estado,
                FechaEnvio = CASE WHEN @Estado = 'Enviado' THEN NOW() ELSE FechaEnvio END,
                IdMensajeYCloud = COALESCE(@IdMensaje, IdMensajeYCloud),
                IntentosEnvio = IntentosEnvio + 1,
                ProximoIntento = @ProximoIntento,
                MensajeError = @MensajeError,
                FechaUltimaActualizacion = NOW()
            WHERE Id = @Id";
        
        await db.ExecuteAsync(query, new
        {
            Id = id,
            Estado = estado,
            IdMensaje = idMensajeYCloud,
            ProximoIntento = proximoIntento,
            MensajeError = mensajeError
        });
    }
}

public record NotificacionPendienteDto(
    int Id,
    int IdTicket,
    string NumeroWhatsApp,
    string TipoNotificacion,
    string MensajeTexto,
    int IntentosEnvio,
    int MaximoIntentos
);
```


### 6.5 PromptTuningBackgroundService

**Propósito:** Ajustar prompts automáticamente cada 2 semanas basándose en métricas

**Archivo:** `BackgroundServices/PromptTuningBackgroundService.cs`

```csharp
namespace JELA.API.BackgroundServices;

public class PromptTuningBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PromptTuningBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromDays(14); // 2 semanas
    
    public PromptTuningBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<PromptTuningBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("PromptTuningBackgroundService iniciado");
        
        // Esperar 1 hora antes de la primera ejecución
        await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await AjustarPrompts(stoppingToken);
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("PromptTuningBackgroundService detenido");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo de ajuste de prompts");
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }
    }
    
    private async Task AjustarPrompts(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        var openAIService = scope.ServiceProvider.GetRequiredService<IOpenAIService>();
        
        try
        {
            _logger.LogInformation("Iniciando ajuste automático de prompts");
            
            // Obtener prompts activos
            var prompts = await ObtenerPromptsActivos(db);
            
            foreach (var prompt in prompts)
            {
                try
                {
                    // Obtener métricas del prompt en las últimas 2 semanas
                    var metricas = await ObtenerMetricasPrompt(db, prompt.Id);
                    
                    if (metricas.TotalEjecuciones < 100)
                    {
                        _logger.LogInformation(
                            "Prompt {Id} tiene pocas ejecuciones ({Count}), omitiendo ajuste",
                            prompt.Id, metricas.TotalEjecuciones);
                        continue;
                    }
                    
                    // Analizar si el prompt necesita mejora
                    var necesitaMejora = metricas.TasaExito < 0.85m || 
                                        metricas.TiempoPromedioMs > 3000;
                    
                    if (!necesitaMejora)
                    {
                        _logger.LogInformation(
                            "Prompt {Id} tiene buen rendimiento, no requiere ajuste", prompt.Id);
                        continue;
                    }
                    
                    // Generar versión mejorada del prompt usando IA
                    var promptMejorado = await GenerarPromptMejorado(
                        openAIService, prompt, metricas);
                    
                    // Registrar ajuste (pendiente de aprobación)
                    await RegistrarAjustePrompt(
                        db, prompt.Id, prompt.TextoPrompt, promptMejorado, metricas);
                    
                    _logger.LogInformation(
                        "Ajuste de prompt {Id} registrado para aprobación", prompt.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error ajustando prompt {Id}", prompt.Id);
                }
            }
            
            _logger.LogInformation("Ajuste de prompts completado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en AjustarPrompts");
            throw;
        }
    }
    
    private async Task<List<PromptDto>> ObtenerPromptsActivos(IDatabaseService db)
    {
        var query = @"
            SELECT Id, NombrePrompt, TextoPrompt, TipoPrompt
            FROM conf_ticket_prompts
            WHERE Activo = 1";
        
        var prompts = await db.QueryAsync<PromptDto>(query);
        return prompts.ToList();
    }
    
    private async Task<MetricasPromptDto> ObtenerMetricasPrompt(
        IDatabaseService db, int promptId)
    {
        var query = @"
            SELECT 
                COUNT(*) as TotalEjecuciones,
                AVG(CASE WHEN Exitoso = 1 THEN 1.0 ELSE 0.0 END) as TasaExito,
                AVG(TiempoRespuestaMs) as TiempoPromedioMs,
                AVG(TokensUtilizados) as TokensPromedio
            FROM op_ticket_logprompts
            WHERE IdPrompt = @PromptId
            AND FechaCreacion >= DATE_SUB(NOW(), INTERVAL 14 DAY)";
        
        var metricas = await db.QueryFirstOrDefaultAsync<MetricasPromptDto>(
            query, new { PromptId = promptId });
        
        return metricas ?? new MetricasPromptDto(0, 0, 0, 0);
    }
    
    private async Task<string> GenerarPromptMejorado(
        IOpenAIService openAIService,
        PromptDto prompt,
        MetricasPromptDto metricas)
    {
        var systemMessage = @"Eres un experto en ingeniería de prompts. 
Tu tarea es mejorar prompts para sistemas de atención al cliente con IA.
Analiza el prompt actual y las métricas, y genera una versión mejorada que:
1. Sea más claro y específico
2. Reduzca ambigüedades
3. Mejore la tasa de éxito
4. Mantenga el mismo propósito original";
        
        var userMessage = $@"Prompt actual:
{prompt.TextoPrompt}

Tipo: {prompt.TipoPrompt}

Métricas actuales:
- Tasa de éxito: {metricas.TasaExito:P2}
- Tiempo promedio: {metricas.TiempoPromedioMs}ms
- Tokens promedio: {metricas.TokensPromedio}

Genera una versión mejorada del prompt.";
        
        var response = await openAIService.GenerarRespuesta(
            userMessage, systemMessage, 0.7, 1000);
        
        return response;
    }
    
    private async Task RegistrarAjustePrompt(
        IDatabaseService db,
        int promptId,
        string versionAnterior,
        string versionNueva,
        MetricasPromptDto metricas)
    {
        var query = @"
            INSERT INTO op_ticket_prompt_ajustes_log (
                IdEntidad, IdPrompt, FechaAjuste, VersionAnterior, VersionNueva,
                MotivoAjuste, MetricasAntes, Aprobado, IdUsuarioCreacion, FechaCreacion
            ) VALUES (
                1, @PromptId, NOW(), @VersionAnterior, @VersionNueva,
                @Motivo, @Metricas, 0, 1, NOW()
            )";
        
        var motivo = $"Ajuste automático: Tasa éxito {metricas.TasaExito:P2}, " +
                    $"Tiempo {metricas.TiempoPromedioMs}ms";
        
        var metricasJson = JsonSerializer.Serialize(new
        {
            TotalEjecuciones = metricas.TotalEjecuciones,
            TasaExito = metricas.TasaExito,
            TiempoPromedioMs = metricas.TiempoPromedioMs,
            TokensPromedio = metricas.TokensPromedio
        });
        
        await db.ExecuteAsync(query, new
        {
            PromptId = promptId,
            VersionAnterior = versionAnterior,
            VersionNueva = versionNueva,
            Motivo = motivo,
            Metricas = metricasJson
        });
    }
}

public record PromptDto(
    int Id,
    string NombrePrompt,
    string TextoPrompt,
    string TipoPrompt
);

public record MetricasPromptDto(
    int TotalEjecuciones,
    decimal TasaExito,
    decimal TiempoPromedioMs,
    decimal TokensPromedio
);
```

---

## 7. INTERFACES WEB (ASP.NET VB.NET)

### 7.1 Introducción

Las interfaces web se desarrollan en ASP.NET WebForms con VB.NET 4.8.1, siguiendo los estándares establecidos en `ui-standards.md`. Todas las páginas consumen la API .NET 8 a través de `ApiConsumerCRUD.vb`.

**Ubicación:** `JelaWeb/Views/Operacion/Tickets/`

**Páginas a desarrollar:**
1. **TicketsDashboard** (INTEGRAR EN INICIO.ASPX) - Dashboard con métricas en tiempo real se muestra en página de inicio
2. **TicketsPrompts.aspx** (NUEVO) - Gestión de prompts de IA
3. **TicketsLogs.aspx** (NUEVO) - Auditoría y logs del sistema
4. **Tickets.aspx** (EXISTENTE - Mejorar) - Gestión principal de tickets



### 7.2 TicketsDashboard (YA IMPLEMENTADO EN INICIO.ASPX)

**Propósito:** Dashboard con métricas en tiempo real y gráficos

**ESTADO:** ✅ **YA IMPLEMENTADO** en `Views/Inicio.aspx`

#### 7.2.1 Implementación Actual

**Archivo:** `Views/Inicio.aspx`

**Características implementadas:**
- ✅ Cards de métricas con iconos Font Awesome
  - Tickets Abiertos (azul)
  - Tickets Cerrados (verde)
  - Tickets en Proceso (amarillo)
  - Tickets Pendientes (cyan)
  - Tickets Urgentes (rojo)
  - Total de Tickets (gris)
- ✅ Gráfica de pastel (Pie Chart) - Tickets por Estado
- ✅ Gráfica de barras (Bar Chart) - Tickets por Mes
- ✅ Estilos CSS inline con diseño responsivo
- ✅ Integración con `DashboardBusiness` para obtener métricas
- ✅ Localización de textos con `LocalizationHelper`
- ✅ Navegación a gestión de tickets
- ✅ Filtrado por rol de usuario

**Code-behind:** `Views/Inicio.aspx.vb`

**Métodos implementados:**
- `LoadDashboardData()` - Carga métricas del dashboard
- `LoadStatusChart()` - Carga gráfica de tickets por estado
- `LoadMonthlyChart()` - Carga gráfica de tickets por mes
- `LoadLocalizedTexts()` - Carga textos localizados
- `GetUserRoleId()` - Obtiene rol del usuario
- `lnkGestionTickets_Click()` - Navega a gestión de tickets
- `lnkTicketsAbiertos_Click()` - Navega a tickets abiertos

#### 7.2.2 Mejoras Opcionales (Futuras)

**Si se requiere refactorización:**

1. **Extraer estilos CSS a archivo separado:**

**Agregar sección de dashboard:**

```aspx
<!-- Agregar en Views/Inicio.aspx dentro del MainContent -->

<!-- Sección de Dashboard de Tickets -->
<div class="dashboard-tickets-section">
    <div class="page-header">
        <h2>Dashboard de Tickets con IA</h2>
        <p class="text-muted">Métricas en tiempo real y análisis de rendimiento</p>
    </div>
    
    <!-- Filtro de fecha -->
    <div class="filter-panel">
        <div class="row">
            <div class="col-md-3">
                <label>Fecha Inicio:</label>
                <dx:ASPxDateEdit ID="dtFechaInicio" runat="server" Width="100%">
                    <ClientSideEvents ValueChanged="function(s, e) { TicketsDashboardModule.cargarDatos(); }" />
                </dx:ASPxDateEdit>
            </div>
            <div class="col-md-3">
                <label>Fecha Fin:</label>
                <dx:ASPxDateEdit ID="dtFechaFin" runat="server" Width="100%">
                    <ClientSideEvents ValueChanged="function(s, e) { TicketsDashboardModule.cargarDatos(); }" />
                </dx:ASPxDateEdit>
            </div>
            <div class="col-md-2">
                <dx:ASPxButton ID="btnActualizar" runat="server" Text="Actualizar" 
                              OnClick="btnActualizar_Click" AutoPostBack="true">
                    <Image IconID="actions_refresh_16x16" />
                </dx:ASPxButton>
            </div>
        </div>
    </div>
    
    <!-- Cards de métricas -->
    <div class="metrics-cards">
        <div class="row">
            <div class="col-md-3">
                <div class="metric-card card-primary">
                    <div class="metric-icon">
                        <i class="fas fa-ticket-alt"></i>
                    </div>
                    <div class="metric-content">
                        <h3 id="totalTickets">0</h3>
                        <p>Total Tickets</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="metric-card card-success">
                    <div class="metric-icon">
                        <i class="fas fa-check-circle"></i>
                    </div>
                    <div class="metric-content">
                        <h3 id="ticketsResueltos">0</h3>
                        <p>Resueltos</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="metric-card card-info">
                    <div class="metric-icon">
                        <i class="fas fa-robot"></i>
                    </div>
                    <div class="metric-content">
                        <h3 id="resueltosIA">0</h3>
                        <p>Resueltos por IA</p>
                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="metric-card card-warning">
                    <div class="metric-icon">
                        <i class="fas fa-clock"></i>
                    </div>
                    <div class="metric-content">
                        <h3 id="tiempoPromedio">0</h3>
                        <p>Tiempo Promedio (min)</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Gráficos -->
    <div class="charts-container">
        <div class="row">
            <div class="col-md-6">
                <div class="chart-card">
                    <h4>Tickets por Estado</h4>
                    <dx:WebChartControl ID="chartTicketsByStatus" runat="server" Height="300px" Width="100%">
                        <SeriesTemplate ArgumentDataMember="Estado" />
                    </dx:WebChartControl>
                </div>
            </div>
            <div class="col-md-6">
                <div class="chart-card">
                    <h4>Tickets por Canal</h4>
                    <dx:WebChartControl ID="chartTicketsByChannel" runat="server" Height="300px" Width="100%">
                        <SeriesTemplate ArgumentDataMember="Canal" />
                    </dx:WebChartControl>
                </div>
            </div>
        </div>
        <div class="row mt-3">
            <div class="col-md-12">
                <div class="chart-card">
                    <h4>Tendencia de Tickets (Últimos 30 días)</h4>
                    <dx:WebChartControl ID="chartTicketsTrend" runat="server" Height="300px" Width="100%">
                        <SeriesTemplate ArgumentDataMember="Fecha" />
                    </dx:WebChartControl>
                </div>
            </div>
        </div>
    </div>
    
    <!-- Grid de métricas diarias -->
    <div class="grid-container mt-4">
        <h4>Métricas Diarias Detalladas</h4>
        <dx:ASPxGridView ID="gridMetricasDiarias" runat="server" 
                         AutoGenerateColumns="False"
                         KeyFieldName="FechaMetrica"
                         OnDataBound="gridMetricasDiarias_DataBound">
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" />
            
            <Columns>
                <dx:GridViewDataDateColumn FieldName="FechaMetrica" Caption="Fecha" Width="120px">
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy" />
                    <Settings AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataDateColumn>
                <dx:GridViewDataTextColumn FieldName="Canal" Caption="Canal" Width="100px">
                    <Settings AllowHeaderFilter="True" AllowGroup="True" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="TotalTicketsCreados" Caption="Creados" Width="80px">
                    <PropertiesTextEdit DisplayFormatString="n0" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="TotalTicketsResueltos" Caption="Resueltos" Width="80px">
                    <PropertiesTextEdit DisplayFormatString="n0" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="TotalTicketsResueltosIA" Caption="Resueltos IA" Width="100px">
                    <PropertiesTextEdit DisplayFormatString="n0" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="PorcentajeResolucionIA" Caption="% IA" Width="80px">
                    <PropertiesTextEdit DisplayFormatString="p2" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="TiempoPromedioResolucionMinutos" Caption="Tiempo Prom." Width="100px">
                    <PropertiesTextEdit DisplayFormatString="n2" />
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="CSATPromedio" Caption="CSAT" Width="80px">
                    <PropertiesTextEdit DisplayFormatString="n2" />
                </dx:GridViewDataTextColumn>
            </Columns>
        </dx:ASPxGridView>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="~/Scripts/app/Operacion/tickets-dashboard.js"></script>
</asp:Content>
```


#### 7.2.2 Code-Behind VB.NET

**Archivo:** `Views/Operacion/Tickets/TicketsDashboard.aspx.vb`

```vb
Imports JelaWeb.Infrastructure.Helpers
Imports JelaWeb.Services.API
Imports System.Data

Namespace JelaWeb
    Public Class TicketsDashboard
        Inherits BasePage
        
        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            Try
                If Not IsPostBack Then
                    InicializarPagina()
                    CargarDatos()
                End If
            Catch ex As Exception
                Logger.LogError("TicketsDashboard.Page_Load", ex)
                MostrarError("Error cargando dashboard: " & ex.Message)
            End Try
        End Sub
        
        Private Sub InicializarPagina()
            ' Configurar fechas por defecto (últimos 30 días)
            dtFechaFin.Value = DateTime.Today
            dtFechaInicio.Value = DateTime.Today.AddDays(-30)
            
            ' Configurar grid
            FuncionesGridWeb.ConfigurarGridEstandar(gridMetricasDiarias)
        End Sub
        
        Protected Sub btnActualizar_Click(sender As Object, e As EventArgs)
            Try
                CargarDatos()
            Catch ex As Exception
                Logger.LogError("TicketsDashboard.btnActualizar_Click", ex)
                MostrarError("Error actualizando datos: " & ex.Message)
            End Try
        End Sub
        
        Private Sub CargarDatos()
            CargarMetricasTiempoReal()
            CargarMetricasDiarias()
            CargarGraficos()
        End Sub
        
        Private Sub CargarMetricasTiempoReal()
            Try
                Dim apiConsumer As New ApiConsumerCRUD()
                Dim idEntidad As Integer? = SessionHelper.GetIdEntidad()
                
                ' Llamar al endpoint de métricas en tiempo real
                Dim url As String = $"/api/tickets/metricas/tiempo-real?idEntidad={idEntidad}"
                
                ' El API retorna CrudDto dinámico - deserializar como Dictionary
                Dim response = apiConsumer.Get(Of Dictionary(Of String, Object))(url)
                
                If response IsNot Nothing Then
                    ' Los valores se actualizarán en el cliente via JavaScript
                    ' Guardar en ViewState para acceso desde JavaScript
                    ViewState("MetricasTiempoReal") = response
                End If
            Catch ex As Exception
                Logger.LogError("TicketsDashboard.CargarMetricasTiempoReal", ex)
                Throw
            End Try
        End Sub
        
        Private Sub CargarMetricasDiarias()
            Try
                Dim apiConsumer As New ApiConsumerCRUD()
                Dim fechaInicio As DateTime = CDate(dtFechaInicio.Value)
                Dim fechaFin As DateTime = CDate(dtFechaFin.Value)
                Dim idEntidad As Integer? = SessionHelper.GetIdEntidad()
                
                Dim url As String = $"/api/tickets/metricas/diarias?" &
                    $"fechaInicio={fechaInicio:yyyy-MM-dd}&" &
                    $"fechaFin={fechaFin:yyyy-MM-dd}&" &
                    $"idEntidad={idEntidad}"
                
                ' El API retorna IEnumerable<CrudDto> - deserializar como lista de diccionarios
                Dim metricas = apiConsumer.Get(Of List(Of Dictionary(Of String, Object)))(url)
                
                If metricas IsNot Nothing Then
                    ' Convertir a DataTable para el grid
                    Dim dt As DataTable = ConvertirDiccionariosADataTable(metricas)
                    Session("dtMetricasDiarias") = dt
                    
                    gridMetricasDiarias.DataSource = dt
                    gridMetricasDiarias.DataBind()
                End If
            Catch ex As Exception
                Logger.LogError("TicketsDashboard.CargarMetricasDiarias", ex)
                Throw
            End Try
        End Sub
        
        Private Sub CargarGraficos()
            Try
                CargarGraficoEstados()
                CargarGraficoCanales()
                CargarGraficoTendencia()
            Catch ex As Exception
                Logger.LogError("TicketsDashboard.CargarGraficos", ex)
                Throw
            End Try
        End Sub
        
        Private Sub CargarGraficoEstados()
            Dim apiConsumer As New ApiConsumerCRUD()
            Dim query As String = "SELECT Estado, COUNT(*) as Total " &
                "FROM op_tickets_v2 " &
                "WHERE FechaCreacion >= @FechaInicio AND FechaCreacion <= @FechaFin " &
                "AND Activo = 1 " &
                "GROUP BY Estado"
            
            Dim dt = apiConsumer.ExecuteQuery(query, New Dictionary(Of String, Object) From {
                {"@FechaInicio", dtFechaInicio.Value},
                {"@FechaFin", dtFechaFin.Value}
            })
            
            chartTicketsByStatus.DataSource = dt
            chartTicketsByStatus.DataBind()
        End Sub
        
        Private Sub CargarGraficoCanales()
            Dim apiConsumer As New ApiConsumerCRUD()
            Dim query As String = "SELECT Canal, COUNT(*) as Total " &
                "FROM op_tickets_v2 " &
                "WHERE FechaCreacion >= @FechaInicio AND FechaCreacion <= @FechaFin " &
                "AND Activo = 1 " &
                "GROUP BY Canal"
            
            Dim dt = apiConsumer.ExecuteQuery(query, New Dictionary(Of String, Object) From {
                {"@FechaInicio", dtFechaInicio.Value},
                {"@FechaFin", dtFechaFin.Value}
            })
            
            chartTicketsByChannel.DataSource = dt
            chartTicketsByChannel.DataBind()
        End Sub
        
        Private Sub CargarGraficoTendencia()
            Dim apiConsumer As New ApiConsumerCRUD()
            Dim query As String = "SELECT DATE(FechaCreacion) as Fecha, COUNT(*) as Total " &
                "FROM op_tickets_v2 " &
                "WHERE FechaCreacion >= DATE_SUB(NOW(), INTERVAL 30 DAY) " &
                "AND Activo = 1 " &
                "GROUP BY DATE(FechaCreacion) " &
                "ORDER BY Fecha"
            
            Dim dt = apiConsumer.ExecuteQuery(query)
            
            chartTicketsTrend.DataSource = dt
            chartTicketsTrend.DataBind()
        End Sub
        
        Protected Sub gridMetricasDiarias_DataBound(sender As Object, e As EventArgs)
            Try
                Dim tabla As DataTable = TryCast(Session("dtMetricasDiarias"), DataTable)
                If tabla IsNot Nothing Then
                    FuncionesGridWeb.SUMColumn(gridMetricasDiarias, tabla)
                End If
            Catch ex As Exception
                Logger.LogError("TicketsDashboard.gridMetricasDiarias_DataBound", ex)
            End Try
        End Sub
        
        Private Function ConvertirDiccionariosADataTable(metricas As List(Of Dictionary(Of String, Object))) As DataTable
            Dim dt As New DataTable()
            
            ' Si no hay datos, retornar tabla vacía
            If metricas Is Nothing OrElse metricas.Count = 0 Then
                Return dt
            End If
            
            ' Crear columnas dinámicamente desde el primer diccionario
            Dim primerRegistro = metricas(0)
            For Each kvp In primerRegistro
                Dim columnType As Type = GetType(Object)
                
                ' Intentar determinar el tipo de la columna
                If kvp.Value IsNot Nothing Then
                    columnType = kvp.Value.GetType()
                End If
                
                dt.Columns.Add(kvp.Key, columnType)
            Next
            
            ' Agregar filas
            For Each metrica In metricas
                Dim row = dt.NewRow()
                For Each kvp In metrica
                    row(kvp.Key) = If(kvp.Value, DBNull.Value)
                Next
                dt.Rows.Add(row)
            Next
            
            Return dt
        End Function
                    metrica.CSATPromedio
                )
            Next
            
            Return dt
        End Function
        
        Private Sub MostrarError(mensaje As String)
            ' Implementar notificación de error (toastr, alert, etc.)
            ClientScript.RegisterStartupScript(Me.GetType(), "error", 
                $"toastr.error('{mensaje}');", True)
        End Sub
    End Class
    
    ' NOTA: Ya no se necesitan DTOs estáticos
    ' El API retorna CrudDto dinámico que se deserializa como Dictionary(Of String, Object)
    ' Esto permite agregar campos nuevos sin modificar código
End Namespace
```


#### 7.2.3 Designer VB.NET

**Archivo:** `Views/Operacion/Tickets/TicketsDashboard.aspx.designer.vb`

```vb
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated. 
' </auto-generated>
'------------------------------------------------------------------------------

Namespace JelaWeb
    Partial Public Class TicketsDashboard
        
        Protected WithEvents dtFechaInicio As Global.DevExpress.Web.ASPxDateEdit
        Protected WithEvents dtFechaFin As Global.DevExpress.Web.ASPxDateEdit
        Protected WithEvents btnActualizar As Global.DevExpress.Web.ASPxButton
        Protected WithEvents chartTicketsByStatus As Global.DevExpress.XtraCharts.Web.WebChartControl
        Protected WithEvents chartTicketsByChannel As Global.DevExpress.XtraCharts.Web.WebChartControl
        Protected WithEvents chartTicketsTrend As Global.DevExpress.XtraCharts.Web.WebChartControl
        Protected WithEvents gridMetricasDiarias As Global.DevExpress.Web.ASPxGridView
    End Class
End Namespace
```

#### 7.2.4 CSS

**Archivo:** `Content/CSS/tickets-dashboard.css`

```css
/* ============================================================================
   TICKETS DASHBOARD - ESTILOS
   ============================================================================ */

/* Métricas Cards */
.metrics-cards {
    margin-bottom: 30px;
}

.metric-card {
    background: white;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    display: flex;
    align-items: center;
    transition: transform 0.2s;
}

.metric-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 4px 8px rgba(0,0,0,0.15);
}

.metric-icon {
    font-size: 48px;
    margin-right: 20px;
    opacity: 0.8;
}

.metric-content h3 {
    font-size: 32px;
    font-weight: bold;
    margin: 0;
    color: #333;
}

.metric-content p {
    margin: 5px 0 0 0;
    color: #666;
    font-size: 14px;
}

/* Card Colors */
.card-primary {
    border-left: 4px solid #007bff;
}

.card-primary .metric-icon {
    color: #007bff;
}

.card-success {
    border-left: 4px solid #28a745;
}

.card-success .metric-icon {
    color: #28a745;
}

.card-info {
    border-left: 4px solid #17a2b8;
}

.card-info .metric-icon {
    color: #17a2b8;
}

.card-warning {
    border-left: 4px solid #ffc107;
}

.card-warning .metric-icon {
    color: #ffc107;
}

/* Charts Container */
.charts-container {
    margin-bottom: 30px;
}

.chart-card {
    background: white;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    margin-bottom: 20px;
}

.chart-card h4 {
    margin-top: 0;
    margin-bottom: 15px;
    color: #333;
    font-weight: 600;
}

/* Grid Container */
.grid-container {
    background: white;
    border-radius: 8px;
    padding: 20px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.grid-container h4 {
    margin-top: 0;
    margin-bottom: 15px;
    color: #333;
    font-weight: 600;
}

/* Responsive */
@media (max-width: 768px) {
    .metric-card {
        margin-bottom: 15px;
    }
    
    .metric-icon {
        font-size: 36px;
        margin-right: 15px;
    }
    
    .metric-content h3 {
        font-size: 24px;
    }
}
```

#### 7.2.5 JavaScript

**Archivo:** `Scripts/app/Operacion/tickets-dashboard.js`

```javascript
// ============================================================================
// TICKETS DASHBOARD - JAVASCRIPT MODULE
// ============================================================================

var TicketsDashboardModule = (function() {
    'use strict';
    
    // Variables privadas
    var _refreshInterval = null;
    var _refreshIntervalMs = 60000; // 1 minuto
    
    // Inicialización
    function init() {
        console.log('TicketsDashboardModule inicializado');
        
        bindEvents();
        cargarMetricasTiempoReal();
        iniciarAutoRefresh();
    }
    
    // Event handlers
    function bindEvents() {
        // Eventos personalizados si es necesario
    }
    
    // Cargar métricas en tiempo real
    function cargarMetricasTiempoReal() {
        $.ajax({
            url: '/api/tickets/metricas/tiempo-real',
            method: 'GET',
            headers: {
                'Authorization': 'Bearer ' + getAuthToken()
            },
            success: function(data) {
                actualizarCards(data);
            },
            error: function(xhr, status, error) {
                console.error('Error cargando métricas:', error);
            }
        });
    }
    
    // Actualizar cards de métricas
    function actualizarCards(data) {
        $('#totalTickets').text(data.totalTickets || 0);
        $('#ticketsResueltos').text(data.ticketsResueltos || 0);
        $('#resueltosIA').text(data.resueltosIA || 0);
        
        var tiempoPromedio = data.tiempoPromedioResolucion 
            ? Math.round(data.tiempoPromedioResolucion) 
            : 0;
        $('#tiempoPromedio').text(tiempoPromedio);
        
        // Animar los números
        animarNumeros();
    }
    
    // Animar números en los cards
    function animarNumeros() {
        $('.metric-content h3').each(function() {
            var $this = $(this);
            $this.addClass('pulse-animation');
            setTimeout(function() {
                $this.removeClass('pulse-animation');
            }, 500);
        });
    }
    
    // Auto-refresh
    function iniciarAutoRefresh() {
        _refreshInterval = setInterval(function() {
            cargarMetricasTiempoReal();
        }, _refreshIntervalMs);
    }
    
    function detenerAutoRefresh() {
        if (_refreshInterval) {
            clearInterval(_refreshInterval);
            _refreshInterval = null;
        }
    }
    
    // Obtener token de autenticación
    function getAuthToken() {
        // Implementar según el sistema de autenticación
        return sessionStorage.getItem('authToken') || '';
    }
    
    // Cargar datos (llamado desde el servidor)
    function cargarDatos() {
        // Trigger postback para recargar datos
        __doPostBack('btnActualizar', '');
    }
    
    // API pública
    return {
        init: init,
        cargarDatos: cargarDatos,
        cargarMetricasTiempoReal: cargarMetricasTiempoReal
    };
})();

// Inicializar cuando el DOM esté listo
$(document).ready(function() {
    TicketsDashboardModule.init();
});

// Detener auto-refresh al salir de la página
$(window).on('beforeunload', function() {
    if (TicketsDashboardModule.detenerAutoRefresh) {
        TicketsDashboardModule.detenerAutoRefresh();
    }
});
```


### 7.3 TicketsPrompts.aspx (NUEVO - CATÁLOGO)

**Propósito:** Catálogo de prompts de IA con historial de ajustes y versionamiento

**Ubicación:** `Views/Catalogos/Tickets/TicketsPrompts.aspx`

**Tipo:** Catálogo con funcionalidades avanzadas (auto-optimización, versionamiento, aprobaciones)

#### 7.3.1 Resumen de Archivos

**Archivos a crear:**
- `Views/Catalogos/Tickets/TicketsPrompts.aspx` - Interfaz principal
- `Views/Catalogos/Tickets/TicketsPrompts.aspx.vb` - Code-behind
- `Views/Catalogos/Tickets/TicketsPrompts.aspx.designer.vb` - Designer
- `Content/CSS/tickets-prompts.css` - Estilos
- `Scripts/app/Catalogos/Tickets/tickets-prompts.js` - JavaScript

**Características principales:**
- Grid con todos los prompts activos
- **Columnas dinámicas generadas desde DataTable** (ver AreasComunes.aspx.vb)
- Popup para editar prompts
- Grid de historial de ajustes automáticos
- Aprobación/rechazo de ajustes sugeridos por IA
- Métricas de rendimiento por prompt
- Versionamiento de prompts

**Estructura de la página:**
```
- Breadcrumb
- Título y descripción
- Grid principal de prompts (conf_ticket_prompts)
  * Columnas: Nombre, Tipo, Texto (truncado), Activo, Acciones
  * Toolbar: Nuevo Prompt, Editar, Ver Historial
- Popup de edición de prompt
  * Campos: Nombre, Tipo, Texto completo, Activo
  * Botones: Guardar, Cancelar
- Grid de ajustes pendientes de aprobación
  * Columnas: Fecha, Prompt, Motivo, Métricas Antes/Después
  * Acciones: Aprobar, Rechazar, Ver Comparación
- Popup de comparación de versiones
  * Vista lado a lado: Versión anterior vs Nueva
  * Métricas comparativas
  * Botones: Aprobar, Rechazar
```

**Endpoints consumidos:**
- `GET /api/crud?strQuery=SELECT * FROM conf_ticket_prompts`
- `POST /api/crud/conf_ticket_prompts` - Crear prompt
- `PUT /api/crud/conf_ticket_prompts/{id}` - Actualizar prompt
- `GET /api/crud?strQuery=SELECT * FROM op_ticket_prompt_ajustes_log WHERE Aprobado = 0`
- `PUT /api/crud/op_ticket_prompt_ajustes_log/{id}` - Aprobar/rechazar ajuste

**Código simplificado (estructura similar a TicketsDashboard):**

```vb
' Code-behind principal
Public Class TicketsPrompts
    Inherits BasePage
    
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CargarPrompts()
            CargarAjustesPendientes()
        End If
    End Sub
    
    Private Sub CargarPrompts()
        Dim query = "SELECT * FROM conf_ticket_prompts WHERE Activo = 1"
        Dim dt = ApiConsumerCRUD.ExecuteQuery(query)
        
        ' Generar columnas dinámicamente desde el DataTable
        GenerarColumnasDinamicas(gridPrompts, dt)
        
        ' Guardar DataTable en Session ANTES de DataBind para FuncionesGridWeb
        Session("dtPrompts") = dt
        
        gridPrompts.DataSource = dt
        gridPrompts.DataBind()
    End Sub
    
    Private Sub CargarAjustesPendientes()
        Dim query = "SELECT * FROM op_ticket_prompt_ajustes_log WHERE Aprobado = 0"
        Dim dt = ApiConsumerCRUD.ExecuteQuery(query)
        
        ' Generar columnas dinámicamente
        GenerarColumnasDinamicas(gridAjustes, dt)
        
        Session("dtAjustes") = dt
        
        gridAjustes.DataSource = dt
        gridAjustes.DataBind()
    End Sub
    
    Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)
        ' Implementación igual que en AreasComunes.aspx.vb
        ' Ver design.md § 7.5.2 para código completo
    End Sub
    
    Protected Sub gridPrompts_DataBound(sender As Object, e As EventArgs)
        Dim tabla As DataTable = TryCast(Session("dtPrompts"), DataTable)
        If tabla IsNot Nothing Then
            FuncionesGridWeb.SUMColumn(gridPrompts, tabla)
        End If
    End Sub
    
    Protected Sub btnGuardarPrompt_Click(sender As Object, e As EventArgs)
        ' Guardar o actualizar prompt
    End Sub
    
    Protected Sub btnAprobarAjuste_Click(sender As Object, e As EventArgs)
        ' Aprobar ajuste y aplicar nueva versión
    End Sub
End Class
```

**CSS principal:**
```css
.prompt-card {
    background: white;
    padding: 20px;
    border-radius: 8px;
    margin-bottom: 20px;
}

.prompt-text-preview {
    max-height: 100px;
    overflow: hidden;
    text-overflow: ellipsis;
}

.version-comparison {
    display: flex;
    gap: 20px;
}

.version-column {
    flex: 1;
    padding: 15px;
    background: #f8f9fa;
    border-radius: 5px;
}
```


### 7.4 TicketsLogs.aspx (NUEVO)

**Propósito:** Auditoría completa del sistema de tickets con IA

#### 7.4.1 Resumen de Archivos

**Archivos a crear:**
- `Views/Operacion/Tickets/TicketsLogs.aspx` - Interfaz principal
- `Views/Operacion/Tickets/TicketsLogs.aspx.vb` - Code-behind
- `Views/Operacion/Tickets/TicketsLogs.aspx.designer.vb` - Designer
- `Content/CSS/tickets-logs.css` - Estilos
- `Scripts/app/Operacion/tickets-logs.js` - JavaScript

**Características principales:**
- Tabs para diferentes tipos de logs
- **Columnas dinámicas generadas desde DataTable** (ver AreasComunes.aspx.vb)
- **Filtros según estándares:** Solo fechas arriba, resto en columnas del grid (ui-standards.md § 7)
- Tipo de log y severidad como columnas con `AllowHeaderFilter="True"`
- Exportación a Excel
- Búsqueda global con `SettingsSearchPanel`
- Visualización de JSON formateado

**Estructura de la página:**
```
- Breadcrumb
- Título y descripción
- **Filtros superiores:** Solo rango de fechas (ui-standards.md § 7)
- **Tipo de log y severidad:** Como columnas en grid con `AllowHeaderFilter="True"`
- Tabs de logs:
  1. Logs del Sistema (op_ticket_logs_sistema)
  2. Logs de Interacciones (op_ticket_logs_interacciones)
  3. Logs de Prompts IA (op_ticket_logprompts)
  4. Logs de Monitoreo (op_ticket_robot_monitoreo)
- Grid dinámico según tab seleccionado
- Popup de detalles con JSON formateado
```

**Endpoints consumidos:**
- `GET /api/crud?strQuery=SELECT * FROM op_ticket_logs_sistema WHERE ...`
- `GET /api/crud?strQuery=SELECT * FROM op_ticket_logs_interacciones WHERE ...`
- `GET /api/crud?strQuery=SELECT * FROM op_ticket_logprompts WHERE ...`
- `GET /api/crud?strQuery=SELECT * FROM op_ticket_robot_monitoreo WHERE ...`

**Código simplificado:**

```vb
Public Class TicketsLogs
    Inherits BasePage
    
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            InicializarFiltrosFecha() ' Solo fechas según ui-standards.md § 7
            ConfigurarGridConFiltrosEnColumnas() ' Filtros en headers
            CargarLogsSegunTab()
        End If
    End Sub
    
    Private Sub CargarLogsSegunTab()
        Select Case tabControl.ActiveTabIndex
            Case 0 ' Logs Sistema
                CargarLogsSistema()
            Case 1 ' Logs Interacciones
                CargarLogsInteracciones()
            Case 2 ' Logs Prompts
                CargarLogsPrompts()
            Case 3 ' Logs Monitoreo
                CargarLogsMonitoreo()
        End Select
    End Sub
    
    Private Sub CargarLogsSistema()
        Dim query = BuildQueryLogsSistema()
        Dim dt = ApiConsumerCRUD.ExecuteQuery(query)
        
        ' Generar columnas dinámicamente desde el DataTable
        GenerarColumnasDinamicas(gridLogs, dt)
        
        ' Guardar DataTable en Session ANTES de DataBind para FuncionesGridWeb
        Session("dtLogs") = dt
        
        gridLogs.DataSource = dt
        gridLogs.DataBind()
    End Sub
    
    Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)
        ' Implementación igual que en AreasComunes.aspx.vb
        ' Ver design.md § 7.5.2 para código completo
    End Sub
    
    Protected Sub gridLogs_DataBound(sender As Object, e As EventArgs)
        Dim tabla As DataTable = TryCast(Session("dtLogs"), DataTable)
        If tabla IsNot Nothing Then
            FuncionesGridWeb.SUMColumn(gridLogs, tabla)
        End If
    End Sub
    
    Protected Sub btnExportar_Click(sender As Object, e As EventArgs)
        gridLogs.ExportXlsxToResponse()
    End Sub
End Class
```

**CSS principal:**
```css
.logs-container {
    background: white;
    padding: 20px;
    border-radius: 8px;
}

.log-severity-info {
    color: #17a2b8;
}

.log-severity-warning {
    color: #ffc107;
}

.log-severity-error {
    color: #dc3545;
}

.log-severity-critical {
    color: #721c24;
    font-weight: bold;
}

.json-viewer {
    background: #f8f9fa;
    padding: 15px;
    border-radius: 5px;
    font-family: 'Courier New', monospace;
    font-size: 12px;
    overflow-x: auto;
}
```


### 7.5 Tickets.aspx (EXISTENTE - Mejoras)

**Propósito:** Mejorar la página existente de gestión de tickets

#### 7.5.1 Estado Actual

**Implementado:**
- ✅ Filtros de fecha arriba del grid (cumple ui-standards.md § 7)
- ✅ Popup modal para captura y detalle de tickets
- ✅ Tabs en popup: Cliente, Conversación, Resumen IA, Resolución
- ✅ Integración con IA para procesamiento automático
- ✅ Toolbar con acciones: Nuevo, Ver, Resolver con IA, Refresh, Export

**Pendiente de corrección:**
- ❌ **Columnas estáticas en ASPX** - Deben ser dinámicas generadas desde DataTable (ver AreasComunes.aspx.vb)
- ❌ ShowFilterRowMenu="False" (debe ser "True" según ui-standards.md § 6)
- ❌ Falta SettingsSearchPanel.Visible="True" para búsqueda global
- ❌ Falta método GenerarColumnasDinamicas() en code-behind

#### 7.5.2 Mejoras a Implementar

**1. Implementar columnas dinámicas (CRÍTICO):**

El grid actualmente tiene columnas estáticas definidas en ASPX (líneas 103-160). Esto NO cumple con el estándar del sistema. TODAS las columnas deben ser dinámicas, generadas desde el DataTable que regresa el API.

**Patrón a seguir (ver AreasComunes.aspx.vb, ConceptosCuota.aspx.vb, Entidades.aspx.vb):**

```vb
Private Sub CargarTickets()
    Try
        ' Obtener datos del servicio
        Dim dt As DataTable = TicketService.ListarTickets()

        ' Generar columnas dinámicamente desde el DataTable
        GenerarColumnasDinamicas(gridTickets, dt)

        ' Guardar DataTable en Session ANTES de DataBind para FuncionesGridWeb
        Session("dtTickets") = dt

        gridTickets.DataSource = dt
        gridTickets.DataBind()

    Catch ex As Exception
        Logger.LogError("Tickets.CargarTickets", ex)
        MostrarError("Error al cargar tickets")
    End Try
End Sub

Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)
    Try
        If tabla Is Nothing OrElse tabla.Columns.Count = 0 Then Return

        ' Limpiar columnas previas (excepto columnas personalizadas)
        Dim indicesColumnasParaMantener As New List(Of Integer)

        ' Guardar índices de columnas personalizadas antes de limpiar
        For i As Integer = 0 To grid.Columns.Count - 1
            Dim col As GridViewColumn = grid.Columns(i)
            Dim debeMantener As Boolean = False

            ' Mantener GridViewCommandColumn (botones de acciones)
            If TypeOf col Is GridViewCommandColumn Then
                debeMantener = True
            ElseIf TypeOf col Is GridViewDataColumn Then
                Dim dataCol = CType(col, GridViewDataColumn)

                ' Mantener columnas con DataItemTemplate
                If dataCol.DataItemTemplate IsNot Nothing Then
                    debeMantener = True
                End If
            End If

            If debeMantener Then
                indicesColumnasParaMantener.Add(i)
            End If
        Next

        ' Limpiar solo las columnas de datos, no las personalizadas
        For i As Integer = grid.Columns.Count - 1 To 0 Step -1
            If Not indicesColumnasParaMantener.Contains(i) Then
                grid.Columns.RemoveAt(i)
            End If
        Next

        ' Crear columnas dinámicamente desde el DataTable
        For Each col As DataColumn In tabla.Columns
            Dim nombreColumna = col.ColumnName

            ' Omitir columna Id (se ocultará)
            If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) Then
                Continue For
            End If

            ' Crear columna según el tipo de dato
            Dim gridCol As GridViewDataColumn = Nothing

            Select Case col.DataType
                Case GetType(Boolean)
                    gridCol = New GridViewDataCheckColumn()
                    gridCol.Width = Unit.Pixel(80)

                Case GetType(DateTime), GetType(Date)
                    gridCol = New GridViewDataDateColumn()
                    gridCol.Width = Unit.Pixel(150)
                    CType(gridCol, GridViewDataDateColumn).PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"

                Case GetType(Decimal), GetType(Double), GetType(Single)
                    gridCol = New GridViewDataTextColumn()
                    gridCol.Width = Unit.Pixel(100)
                    gridCol.PropertiesEdit.DisplayFormatString = "c2"

                Case GetType(Integer), GetType(Long), GetType(Short)
                    gridCol = New GridViewDataTextColumn()
                    gridCol.Width = Unit.Pixel(100)
                    gridCol.PropertiesEdit.DisplayFormatString = "n0"

                Case Else
                    gridCol = New GridViewDataTextColumn()
                    gridCol.Width = Unit.Pixel(150)
            End Select

            gridCol.FieldName = nombreColumna
            gridCol.Caption = nombreColumna ' FuncionesGridWeb.SUMColumn aplicará SplitCamelCase
            gridCol.ReadOnly = True

            ' Ocultar columna Id si existe
            If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) Then
                gridCol.Visible = False
            Else
                gridCol.Visible = True
                ' Configurar filtros y agrupación según estándares
                gridCol.Settings.AllowHeaderFilter = True
                gridCol.Settings.AllowGroup = True
            End If

            grid.Columns.Add(gridCol)
        Next

    Catch ex As Exception
        Logger.LogError("Tickets.GenerarColumnasDinamicas", ex)
        Throw
    End Try
End Sub

Protected Sub gridTickets_DataBound(sender As Object, e As EventArgs)
    Try
        ' Leer DataTable desde Session (guardado antes de DataBind)
        Dim tabla As DataTable = TryCast(Session("dtTickets"), DataTable)

        If tabla IsNot Nothing Then
            FuncionesGridWeb.SUMColumn(gridTickets, tabla)
        End If

    Catch ex As Exception
        Logger.LogError("Tickets.gridTickets_DataBound", ex)
    End Try
End Sub
```

**2. Corregir configuración de filtros (ui-standards.md § 6, § 7):**

```aspx
<!-- CAMBIAR ESTO: -->
<Settings ShowHeaderFilterButton="True" ShowFilterRow="False" ShowFilterRowMenu="False" 
    ShowGroupPanel="True" VerticalScrollableHeight="500" VerticalScrollBarMode="Visible" />

<!-- POR ESTO: -->
<Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" 
    VerticalScrollableHeight="500" VerticalScrollBarMode="Visible" />
<SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
```

**3. Eliminar columnas estáticas del ASPX:**

Eliminar todas las definiciones de columnas estáticas (líneas 103-160 aprox.) del archivo Tickets.aspx. Las columnas se generarán dinámicamente en el code-behind.

**4. Agregar columnas nuevas al DataTable (desde API):**

El API debe regresar estas columnas adicionales en el DataTable:
   - TipoTicket (Accion, Inaccion, LlamadaCortada, etc.)
   - IPOrigen
   - DuracionLlamadaSegundos (para VAPI)
   - RiesgoFraude (boolean)
   - RequiereEscalamiento (boolean)
   - ResueltoporIA (boolean)
   - Idioma

Las columnas se generarán automáticamente con el método `GenerarColumnasDinamicas()`.

**5. Mejorar popup de detalles:**
   - Mostrar toda la información del ticket
   - Historial de conversación (ya implementado)
   - Logs de interacciones
   - Acciones realizadas
   - Archivos adjuntos

**6. Agregar acciones nuevas:**
   - Escalar a supervisor
   - Marcar como fraude
   - Reasignar agente
   - Ver análisis de IA

**Código de mejoras (agregar al existente):**

```vb
' Agregar al code-behind existente

Protected Sub btnEscalar_Click(sender As Object, e As EventArgs)
    Try
        Dim ticketId As Integer = GetSelectedTicketId()
        Dim apiConsumer As New ApiConsumerCRUD()
        
        ' Actualizar ticket
        Dim updateData = New With {
            .RequiereEscalamiento = True,
            .FechaUltimaActualizacion = DateTime.Now
        }
        
        apiConsumer.Update("op_tickets_v2", ticketId, updateData)
        
        ' Registrar acción
        RegistrarAccion(ticketId, "Escalamiento", "Escalado manualmente por usuario")
        
        MostrarExito("Ticket escalado exitosamente")
        CargarTickets()
    Catch ex As Exception
        Logger.LogError("Tickets.btnEscalar_Click", ex)
        MostrarError("Error escalando ticket: " & ex.Message)
    End Try
End Sub

Protected Sub btnMarcarFraude_Click(sender As Object, e As EventArgs)
    Try
        Dim ticketId As Integer = GetSelectedTicketId()
        Dim apiConsumer As New ApiConsumerCRUD()
        
        Dim updateData = New With {
            .RiesgoFraude = True,
            .Estado = "Cerrado",
            .FechaResolucion = DateTime.Now
        }
        
        apiConsumer.Update("op_tickets_v2", ticketId, updateData)
        
        RegistrarAccion(ticketId, "MarcadoFraude", "Marcado como fraude por usuario")
        
        MostrarExito("Ticket marcado como fraude")
        CargarTickets()
    Catch ex As Exception
        Logger.LogError("Tickets.btnMarcarFraude_Click", ex)
        MostrarError("Error: " & ex.Message)
    End Try
End Sub

Private Sub RegistrarAccion(ticketId As Integer, tipoAccion As String, descripcion As String)
    Dim apiConsumer As New ApiConsumerCRUD()
    Dim accion = New With {
        .IdTicket = ticketId,
        .TipoAccion = tipoAccion,
        .Descripcion = descripcion,
        .EsAccionIA = False,
        .IdUsuarioAccion = SessionHelper.GetIdUsuario(),
        .FechaAccion = DateTime.Now
    }
    
    apiConsumer.Insert("op_ticket_acciones", accion)
End Sub
```

**Agregar al ASPX existente:**

```aspx
<!-- Agregar botones de acción en el toolbar -->
<dx:GridViewToolbarItem Text="Escalar" Name="Escalar">
    <Image IconID="arrows_up_16x16" />
</dx:GridViewToolbarItem>
<dx:GridViewToolbarItem Text="Marcar Fraude" Name="MarcarFraude">
    <Image IconID="security_warning_16x16" />
</dx:GridViewToolbarItem>

<!-- Agregar columnas con templates para badges -->
<dx:GridViewDataCheckColumn FieldName="ResueltoporIA" Caption="Resuelto IA" Width="100px">
    <DataItemTemplate>
        <%# If(CBool(Eval("ResueltoporIA")), 
            "<span class='badge badge-success'>IA</span>", 
            "<span class='badge badge-secondary'>Manual</span>") %>
    </DataItemTemplate>
</dx:GridViewDataCheckColumn>

<dx:GridViewDataCheckColumn FieldName="RiesgoFraude" Caption="Riesgo" Width="80px">
    <DataItemTemplate>
        <%# If(CBool(Eval("RiesgoFraude")), 
            "<span class='badge badge-danger'>Fraude</span>", "") %>
    </DataItemTemplate>
</dx:GridViewDataCheckColumn>
```

### 7.6 Navegación y Menú

**IMPORTANTE:** El sistema JELABBC utiliza un **menú Ribbon dinámico** que se construye automáticamente desde opciones almacenadas en la base de datos.

#### 7.6.1 Cómo Funciona el Menú Dinámico

El menú **NO está hardcodeado** en el Master Page. En su lugar:

1. **Las opciones se almacenan en la base de datos** (tabla `conf_opciones` o similar)
2. **El método `ConstruirRibbon(opciones As JArray)`** en `JelaWeb/MasterPages/Jela.Master.vb` lee las opciones desde la sesión
3. **Las opciones se obtienen** mediante `SessionHelper.GetOpciones()` al cargar la página
4. **El menú se construye dinámicamente** agrupando por `RibbonTab` y `RibbonGroup`

**Código relevante en Jela.Master.vb:**

```vb
Private Sub ConstruirRibbon(opciones As JArray)
    ' Agrupar por RibbonTab
    Dim tabs = opciones.GroupBy(Function(o) o("RibbonTab").ToString())

    For Each tabGroup In tabs
        Dim ribbonTab As New RibbonTab(tabGroup.Key)
        ribbonControl.Tabs.Add(ribbonTab)

        ' Agrupar por RibbonGroup dentro del tab
        Dim groups = tabGroup.GroupBy(Function(o) o("RibbonGroup").ToString())

        For Each group In groups
            Dim ribbonGroup As New DevExpress.Web.RibbonGroup(group.Key)
            ribbonTab.Groups.Add(ribbonGroup)

            For Each opcion In group
                Dim btn As New DevExpress.Web.RibbonButtonItem()
                btn.Text = opcion("Nombre").ToString()
                btn.LargeImage.Url = opcion("Icono").ToString()
                btn.NavigateUrl = opcion("Url").ToString()
                btn.Size = RibbonItemSize.Large
                ribbonGroup.Items.Add(btn)
            Next
        Next
    Next
End Sub
```

#### 7.6.2 Estructura de Opciones en Base de Datos

Cada opción de menú debe tener la siguiente estructura JSON aproximada:

```json
{
  "Nombre": "Prompts de IA",
  "Url": "~/Views/Catalogos/Tickets/TicketsPrompts.aspx",
  "Icono": "~/Content/Images/Iconos/formulario.png",
  "RibbonTab": "Catálogos",
  "RibbonGroup": "Tickets"
}
```

**Campos requeridos:**
- `Nombre` - Texto que aparece en el botón del menú
- `Url` - Ruta relativa a la página (usar `~/` para rutas desde la raíz)
- `Icono` - Ruta al icono del botón
- `RibbonTab` - Pestaña principal del Ribbon (ej: "Inicio", "Catálogos", "Operación")
- `RibbonGroup` - Grupo dentro de la pestaña (ej: "Tickets", "Residentes", "Documentos")

#### 7.6.3 Opciones a Agregar para el Módulo de Tickets

**1. TicketsPrompts.aspx (Catálogo de Prompts de IA)**

```sql
INSERT INTO conf_opciones (
    Nombre, 
    Url, 
    Icono, 
    RibbonTab, 
    RibbonGroup,
    Orden,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    'Prompts de IA',
    '~/Views/Catalogos/Tickets/TicketsPrompts.aspx',
    '~/Content/Images/Iconos/formulario.png',
    'Catálogos',
    'Tickets',
    10, -- Ajustar según orden deseado
    1,
    1, -- ID del usuario que crea
    NOW()
);
```

**2. TicketsLogs.aspx (Auditoría de Logs)**

```sql
INSERT INTO conf_opciones (
    Nombre, 
    Url, 
    Icono, 
    RibbonTab, 
    RibbonGroup,
    Orden,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    'Logs de Tickets',
    '~/Views/Operacion/Tickets/TicketsLogs.aspx',
    '~/Content/Images/Iconos/documentos.png',
    'Operación',
    'Tickets',
    20, -- Ajustar según orden deseado
    1,
    1, -- ID del usuario que crea
    NOW()
);
```

**NOTA:** TicketsDashboard NO requiere enlace en el menú porque ya está integrado en `Views/Inicio.aspx` (página de inicio).

#### 7.6.4 Asignación de Permisos

Después de insertar las opciones, es necesario asignar permisos a los roles/usuarios que deben tener acceso:

```sql
-- Ejemplo: Asignar opción a un rol específico
INSERT INTO conf_rol_opciones (
    IdRol,
    IdOpcion,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    1, -- ID del rol (ej: Administrador)
    (SELECT Id FROM conf_opciones WHERE Nombre = 'Prompts de IA'),
    1,
    1,
    NOW()
);

INSERT INTO conf_rol_opciones (
    IdRol,
    IdOpcion,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    1, -- ID del rol (ej: Administrador)
    (SELECT Id FROM conf_opciones WHERE Nombre = 'Logs de Tickets'),
    1,
    1,
    NOW()
);
```

#### 7.6.5 Verificación

Después de insertar las opciones y asignar permisos:

1. **Cerrar sesión** y volver a iniciar sesión
2. **Verificar que las opciones aparezcan** en el menú Ribbon
3. **Verificar que los enlaces funcionen** correctamente
4. **Verificar permisos** por rol/usuario

#### 7.6.6 Notas Importantes

- ✅ **El menú se actualiza automáticamente** al iniciar sesión (las opciones se cargan desde la BD)
- ✅ **No se requiere modificar código** en el Master Page
- ✅ **Los permisos se controlan por rol** mediante la tabla de relación `conf_rol_opciones`
- ✅ **El orden de las opciones** se controla con el campo `Orden`
- ✅ **Las opciones inactivas** (`Activo = 0`) no aparecen en el menú

---

## 8. INTEGRACIONES EXTERNAS

### 8.1 VAPI API (Llamadas Telefónicas)

**Documentación:** https://docs.vapi.ai

**Configuración en appsettings.json:**
```json
{
  "VAPI": {
    "ApiKey": "vapi_xxx",
    "WebhookUrl": "https://jela-api.azurewebsites.net/api/webhooks/vapi",
    "PhoneNumber": "+521234567890"
  }
}
```

**Webhook recibido de VAPI:**
```json
{
  "callId": "call_abc123",
  "phoneNumber": "+521234567890",
  "transcription": "Hola, tengo un problema con mi pago",
  "durationSeconds": 120,
  "status": "completed",
  "disconnectReason": "user_hangup"
}
```

**Flujo de integración:**
1. Cliente llama al número configurado
2. VAPI transcribe en tiempo real
3. VAPI envía webhook a `/api/webhooks/vapi`
4. Sistema valida cliente duplicado
5. Sistema procesa con Azure OpenAI
6. Sistema crea ticket automáticamente
7. Sistema responde al cliente por voz (síntesis)

### 8.2 YCloud API (WhatsApp Business)

**Documentación:** https://docs.ycloud.com

**Configuración en appsettings.json:**
```json
{
  "YCloud": {
    "ApiKey": "ycloud_xxx",
    "ApiUrl": "https://api.ycloud.com/v2",
    "WebhookUrl": "https://jela-api.azurewebsites.net/api/webhooks/ycloud",
    "PhoneNumber": "+521234567890"
  }
}
```

**Webhook recibido de YCloud:**
```json
{
  "messageId": "msg_abc123",
  "from": "+521234567890",
  "to": "+521234567891",
  "text": "Hola, necesito ayuda",
  "timestamp": "2026-01-18T10:30:00Z",
  "type": "text"
}
```

**Envío de mensaje:**
```csharp
POST https://api.ycloud.com/v2/whatsapp/messages
Headers:
  X-API-Key: ycloud_xxx
  Content-Type: application/json

Body:
{
  "to": "+521234567890",
  "type": "text",
  "text": {
    "body": "Tu ticket #123 ha sido creado"
  }
}
```

### 8.3 Firebase (Chat App)

**Configuración:**
```json
{
  "Firebase": {
    "ProjectId": "jela-tickets",
    "DatabaseUrl": "https://jela-tickets.firebaseio.com",
    "ApiKey": "firebase_xxx"
  }
}
```

**Estructura de datos en Realtime Database:**
```json
{
  "chats": {
    "chat_id_123": {
      "userId": "user_abc",
      "messages": [
        {
          "text": "Hola",
          "timestamp": 1705574400000,
          "sender": "user"
        }
      ]
    }
  }
}
```

### 8.4 Telegram Bot API

**Documentación:** https://core.telegram.org/bots/api

**Configuración:**
```json
{
  "Telegram": {
    "BotToken": "bot_token_from_botfather",
    "WebhookUrl": "https://jela-api-xxx.azurewebsites.net/api/webhooks/telegram",
    "AllowedChatIds": [] // Lista de chat_ids permitidos (opcional)
  }
}
```

**Webhook de Telegram:**
```json
POST /api/webhooks/telegram
{
  "update_id": 123456789,
  "message": {
    "message_id": 1,
    "from": {
      "id": 987654321,
      "is_bot": false,
      "first_name": "Juan",
      "last_name": "Pérez",
      "username": "juanperez"
    },
    "chat": {
      "id": 987654321,
      "first_name": "Juan",
      "last_name": "Pérez",
      "username": "juanperez",
      "type": "private"
    },
    "date": 1705574400,
    "text": "Tengo un problema con mi cuota"
  }
}
```

**Enviar mensaje de respuesta:**
```csharp
// TelegramService.cs
public async Task<bool> EnviarMensajeTelegram(long chatId, string mensaje)
{
    var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
    var payload = new
    {
        chat_id = chatId,
        text = mensaje,
        parse_mode = "Markdown"
    };
    
    var response = await _httpClient.PostAsJsonAsync(url, payload);
    return response.IsSuccessStatusCode;
}
```

**Configurar webhook:**
```bash
curl -X POST "https://api.telegram.org/bot<BOT_TOKEN>/setWebhook" \
  -H "Content-Type: application/json" \
  -d '{"url": "https://jela-api-xxx.azurewebsites.net/api/webhooks/telegram"}'
```

**Características:**
- ✅ Recepción de mensajes de texto
- ✅ Soporte para comandos (/start, /help, /status)
- ✅ Validación de chat_id permitidos (whitelist/blacklist)
- ✅ Respuestas automáticas con IA
- ✅ Notificaciones de cambio de estado de ticket
- ✅ Formato Markdown en respuestas

**Flujo de trabajo:**
1. Usuario envía mensaje a bot de Telegram
2. Telegram envía webhook a `/api/webhooks/telegram`
3. Sistema valida chat_id (whitelist/blacklist)
4. Sistema valida cliente duplicado
5. Sistema procesa con Azure OpenAI
6. Sistema crea ticket automáticamente
7. Sistema responde al usuario por Telegram

### 8.5 Azure OpenAI

**Ya configurado en JELA.API**

**Endpoint existente:** `POST /api/openai`

**Uso desde servicios:**
```csharp
var response = await openAIService.GenerarRespuesta(
    prompt: "Categoriza este ticket: Cliente reporta problema de pago",
    systemMessage: "Eres un asistente de categorización de tickets",
    temperature: 0.7,
    maxTokens: 500
);
```

#### 8.5.1 Estrategia de Implementación en 2 Fases

**⚠️ IMPORTANTE - DESARROLLO EN 2 FASES:**

El sistema de IA se implementará en **2 fases claramente diferenciadas** para optimizar el desarrollo:

**FASE 1 - Sistema Funcional (ACTUAL):**
- ✅ Azure OpenAI responde **libremente sin restricciones**
- ✅ Permite desarrollo y testing rápido del sistema completo
- ✅ Validación de funcionalidad core (webhooks, validación, notificaciones, métricas)
- ✅ Pruebas de integración con VAPI, YCloud, Firebase
- ✅ **Objetivo:** Sistema completamente funcional con IA sin limitaciones

**FASE 2 - Restricciones de Seguridad (FUTURA):**
- 🔒 Implementar restricciones de fuentes de información
- 🔒 Azure OpenAI SOLO responde desde base de conocimiento propia del sistema
- 🔒 NO busca en internet ni usa información externa
- 🔒 Sistema de aislamiento de datos por usuario
- 🔒 Validación de respuestas para detectar violaciones
- 🔒 **Objetivo:** Sistema seguro con respuestas controladas

**Razón de esta estrategia:**
1. **Eficiencia:** Es más rápido desarrollar y validar el sistema completo primero
2. **Testing:** Permite probar toda la funcionalidad sin complejidad adicional
3. **Iteración:** Facilita ajustes y correcciones antes de agregar restricciones
4. **Preparación:** Da tiempo para preparar la base de conocimiento (manuales internos)

**Componentes de Fase 2 (NO implementar ahora):**
- ❌ Tabla `conf_ia_source_restrictions` (restricciones de fuentes)
- ❌ Servicio `PromptManagerService` (gestión de prompts con restricciones)
- ❌ Clase `DataIsolationContext` (aislamiento de datos por usuario)
- ❌ Prompts con restricciones explícitas (NO buscar en internet)
- ❌ Validación de respuestas (detectar violaciones)

**Referencia:** Ver `tasks.md` § 9 "FASE 2 - TAREAS FUTURAS" para detalles completos de implementación de restricciones.

**Criterio de transición a Fase 2:**
- ✅ Sistema Fase 1 completamente funcional en producción
- ✅ Validado con usuarios reales
- ✅ Base de conocimiento (manuales internos) preparada y lista
- ✅ Aprobación para implementar restricciones

---

## 9. PLAN DE IMPLEMENTACIÓN

### 9.1 Fases de Desarrollo

**Fase 1: Base de Datos (1-2 semanas)**
- Ejecutar scripts de alteración de `op_tickets_v2`
- Crear 8 tablas nuevas
- Crear 3 stored procedures
- Validar integridad referencial
- Poblar datos de prueba

**Fase 2: API Backend (3-4 semanas)**
- Crear modelos de datos
- Implementar 3 servicios de lógica de negocio
- Crear 4 archivos de endpoints
- Implementar 4 Background Services
- Pruebas unitarias de servicios
- Pruebas de integración

**Fase 3: Integraciones Externas (2-3 semanas)**
- Configurar VAPI API
- Configurar YCloud API
- Configurar Firebase
- Implementar webhooks
- Pruebas de extremo a extremo

**Fase 4: Interfaces Web (2-3 semanas)**
- Integrar TicketsDashboard en Inicio.aspx
- Crear TicketsPrompts.aspx
- Crear TicketsLogs.aspx
- Mejorar Tickets.aspx existente
- Pruebas de UI/UX

**Fase 5: Pruebas y Ajustes (1-2 semanas)**
- Pruebas de carga
- Pruebas de seguridad
- Ajustes de rendimiento
- Documentación final

**Total estimado: 9-14 semanas**



### 9.2 Recursos Necesarios

**Equipo de desarrollo:**
- 1 Desarrollador Backend (.NET 8 / C#)
- 1 Desarrollador Frontend (VB.NET / ASP.NET)
- 1 DBA (MySQL)
- 1 QA Tester

**Infraestructura:**
- Azure App Service (ya existente)
- Azure MySQL Database (ya existente)
- Azure OpenAI (ya existente)
- Cuentas de VAPI y YCloud (a contratar)

**Herramientas:**
- Visual Studio 2022
- Azure Data Studio
- Postman (pruebas de API)
- Git/GitHub

### 9.3 Dependencias Críticas

1. **Acceso a VAPI API** - Requiere cuenta y configuración
2. **Acceso a YCloud API** - Requiere cuenta WhatsApp Business
3. **Permisos en Azure** - Para desplegar servicios
4. **Acceso a BD de producción** - Para ejecutar scripts

### 9.4 Riesgos y Mitigaciones

| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|--------------|---------|------------|
| Cambios en APIs externas | Media | Alto | Implementar capa de abstracción |
| Problemas de rendimiento | Media | Medio | Pruebas de carga tempranas |
| Errores en migración de BD | Baja | Alto | Backup completo antes de scripts |
| Retrasos en integraciones | Alta | Medio | Comenzar con mocks/simuladores |

---

## 10. CHECKLIST DE DESARROLLO

### 10.1 Base de Datos

**Tabla op_tickets_v2 (Modificar):**
- [ ] Ejecutar script de alteración (13 campos nuevos)
- [ ] Crear índices para optimización
- [ ] Validar constraints
- [ ] Poblar datos de prueba

**Tablas Nuevas (8):**
- [ ] op_ticket_logs_sistema
- [ ] op_ticket_logs_interacciones
- [ ] op_ticket_logprompts
- [ ] op_ticket_metricas
- [ ] op_ticket_validacion_cliente
- [ ] op_ticket_notificaciones_whatsapp
- [ ] op_ticket_robot_monitoreo
- [ ] op_ticket_prompt_ajustes_log

**Stored Procedures (3):**
- [ ] sp_ValidarClienteDuplicado
- [ ] sp_EncolarNotificacionWhatsApp
- [ ] sp_CalcularMetricasDiarias

### 10.2 API Backend (.NET 8)

**Modelos:**
- [ ] Models/TicketModels.cs
- [ ] Models/WebhookModels.cs

**Servicios (6):**
- [ ] Services/TicketValidationService.cs
- [ ] Services/TicketNotificationService.cs
- [ ] Services/TicketMetricsService.cs
- [ ] Services/PromptTuningService.cs
- [ ] Services/YCloudService.cs
- [ ] Services/VapiService.cs

**Endpoints (4):**
- [ ] Endpoints/WebhookEndpoints.cs
- [ ] Endpoints/TicketValidationEndpoints.cs
- [ ] Endpoints/TicketNotificationEndpoints.cs
- [ ] Endpoints/TicketMetricsEndpoints.cs

**Background Services (4):**
- [ ] BackgroundServices/TicketMonitoringBackgroundService.cs
- [ ] BackgroundServices/TicketMetricsBackgroundService.cs
- [ ] BackgroundServices/NotificationQueueBackgroundService.cs
- [ ] BackgroundServices/PromptTuningBackgroundService.cs

**Configuración:**
- [ ] Registrar servicios en Program.cs
- [ ] Registrar endpoints en Program.cs
- [ ] Registrar Background Services en Program.cs
- [ ] Agregar configuraciones en appsettings.json

### 10.3 Interfaces Web (VB.NET)

**TicketsDashboard (INTEGRAR EN INICIO.ASPX):**
- [ ] Modificar Inicio.aspx (agregar sección de dashboard)
- [ ] Modificar Inicio.aspx.vb (agregar métodos de métricas)
- [ ] Content/CSS/tickets-dashboard.css
- [ ] Scripts/app/Operacion/tickets-dashboard.js

**TicketsPrompts.aspx (NUEVO):**
- [ ] TicketsPrompts.aspx
- [ ] TicketsPrompts.aspx.vb
- [ ] TicketsPrompts.aspx.designer.vb
- [ ] Content/CSS/tickets-prompts.css
- [ ] Scripts/app/Operacion/tickets-prompts.js

**TicketsLogs.aspx (NUEVO):**
- [ ] TicketsLogs.aspx
- [ ] TicketsLogs.aspx.vb
- [ ] TicketsLogs.aspx.designer.vb
- [ ] Content/CSS/tickets-logs.css
- [ ] Scripts/app/Operacion/tickets-logs.js

**Tickets.aspx (MEJORAR):**
- [ ] Implementar columnas dinámicas (eliminar columnas estáticas del ASPX)
- [ ] Implementar método GenerarColumnasDinamicas() en code-behind
- [ ] Corregir ShowFilterRowMenu="True"
- [ ] Agregar SettingsSearchPanel
- [ ] Mejorar popup de detalles
- [ ] Agregar acciones nuevas (Escalar, Marcar Fraude)

### 10.4 Integraciones Externas

**VAPI API:**
- [ ] Crear cuenta en VAPI
- [ ] Configurar número telefónico
- [ ] Configurar webhook URL
- [ ] Implementar VapiService.cs
- [ ] Probar flujo completo de llamada

**YCloud API:**
- [ ] Crear cuenta en YCloud
- [ ] Configurar WhatsApp Business
- [ ] Configurar webhook URL
- [ ] Implementar YCloudService.cs
- [ ] Probar envío/recepción de mensajes

**Firebase:**
- [ ] Crear proyecto en Firebase
- [ ] Configurar Realtime Database
- [ ] Configurar reglas de seguridad
- [ ] Implementar integración
- [ ] Probar chat en tiempo real

**Azure OpenAI:**
- [ ] Validar configuración existente
- [ ] Probar endpoint /api/openai
- [ ] Ajustar prompts según necesidades

### 10.5 Pruebas

**Pruebas Unitarias:**
- [ ] Servicios de validación
- [ ] Servicios de notificación
- [ ] Servicios de métricas
- [ ] Background Services

**Pruebas de Integración:**
- [ ] Webhooks VAPI
- [ ] Webhooks YCloud
- [ ] Endpoints de API
- [ ] Stored Procedures

**Pruebas de UI:**
- [ ] TicketsDashboard en Inicio.aspx
- [ ] TicketsPrompts.aspx
- [ ] TicketsLogs.aspx
- [ ] Tickets.aspx (mejoras)

**Pruebas de Extremo a Extremo:**
- [ ] Flujo completo: Llamada VAPI → Ticket → Notificación
- [ ] Flujo completo: WhatsApp → Ticket → Respuesta
- [ ] Flujo completo: Chat Web → Ticket → Resolución
- [ ] Monitoreo automático funcionando
- [ ] Métricas calculándose correctamente
- [ ] Ajuste de prompts funcionando

### 10.6 Despliegue

**Preparación:**
- [ ] Backup completo de BD
- [ ] Backup de código actual
- [ ] Plan de rollback documentado

**Base de Datos:**
- [ ] Ejecutar scripts en ambiente de pruebas
- [ ] Validar integridad de datos
- [ ] Ejecutar scripts en producción
- [ ] Validar migración exitosa

**API Backend:**
- [ ] Compilar proyecto JELA.API
- [ ] Ejecutar pruebas
- [ ] Desplegar a Azure App Service
- [ ] Validar Health Checks
- [ ] Validar Background Services iniciados

**Frontend:**
- [ ] Compilar proyecto JelaWeb
- [ ] Ejecutar pruebas
- [ ] Desplegar a Azure App Service
- [ ] Validar páginas nuevas accesibles

**Configuración:**
- [ ] Configurar variables de entorno
- [ ] Configurar connection strings
- [ ] Configurar API keys (VAPI, YCloud)
- [ ] Configurar webhooks en servicios externos

**Validación Post-Despliegue:**
- [ ] Probar creación de ticket manual
- [ ] Probar creación de ticket por WhatsApp
- [ ] Probar creación de ticket por llamada
- [ ] Validar dashboard mostrando datos
- [ ] Validar logs registrándose
- [ ] Validar Background Services ejecutándose

### 10.7 Documentación

**Documentación Técnica:**
- [ ] Diagrama de arquitectura actualizado
- [ ] Documentación de API (Swagger)
- [ ] Documentación de BD (esquema)
- [ ] Guía de despliegue

**Documentación de Usuario:**
- [ ] Manual de usuario - Dashboard
- [ ] Manual de usuario - Gestión de Prompts
- [ ] Manual de usuario - Logs y Auditoría
- [ ] Manual de usuario - Gestión de Tickets

**Documentación de Operaciones:**
- [ ] Guía de monitoreo
- [ ] Guía de troubleshooting
- [ ] Procedimientos de backup/restore
- [ ] Contactos de soporte

---

---

## 11. EXPANSIÓN CHAT WEB AVANZADO

### 11.1 Visión General

El Chat Web actual de JelaWeb funciona correctamente para la creación de tickets. Esta expansión lo transforma en un **asistente inteligente completo** que permite realizar operaciones CRUD, consultas dinámicas, navegación y generación de reportes mediante lenguaje natural.

**Referencia completa:** Ver `.kiro/specs/tickets-colaborativos/CHAT-WEB-AVANZADO-EXPANSION.md` para detalles completos.

### 11.2 Nuevas Tablas de Base de Datos

#### 11.2.1 conf_chat_actions

Catálogo de acciones disponibles en el chat (CRUD, Consultas, Navegación):

**Campos principales:**
- `NombreAccion` - Identificador único de la acción
- `TipoAccion` - CRUD, CONSULTA, NAVEGACION
- `EndpointAPI` - Endpoint a llamar
- `RequiereParametros` - JSON con parámetros requeridos
- `RequierePermisos` - Permisos necesarios

#### 11.2.2 conf_chat_queries

Consultas SQL parametrizadas ejecutables desde el chat:

**Campos principales:**
- `NombreConsulta` - Identificador único
- `QuerySQL` - Query parametrizado
- `TipoResultado` - GRID, VALOR_UNICO, GRAFICA, LISTA
- `FormatoRespuesta` - Template para formatear respuesta

#### 11.2.3 op_chat_history

Historial completo de conversaciones para auditoría:

**Campos principales:**
- `SessionId` - Identificador de sesión
- `Mensaje` - Mensaje del usuario
- `AccionEjecutada` - Acción ejecutada
- `ParametrosUsados` - JSON con parámetros
- `ResultadoExitoso` - Si la acción fue exitosa

#### 11.2.4 op_chat_confirmations

Confirmaciones pendientes de acciones críticas:

**Campos principales:**
- `AccionPendiente` - Acción a confirmar
- `ParametrosAccion` - JSON con parámetros
- `Estado` - PENDIENTE, CONFIRMADO, CANCELADO, EXPIRADO

### 11.3 Nuevos Servicios Backend (.NET 8)

#### 11.3.1 ChatOrchestratorService

**Responsabilidad:** Coordinar el flujo completo de procesamiento de mensajes

**Métodos principales:**
```csharp
Task<ChatResponse> ProcessMessageAsync(ChatRequest request);
Task<ChatResponse> ConfirmActionAsync(int confirmationId, string action);
```

#### 11.3.2 ChatActionService

**Responsabilidad:** Ejecutar acciones CRUD y validar permisos

**Métodos principales:**
```csharp
Task<List<FunctionDefinition>> GetAvailableFunctionsAsync(int userId);
Task<bool> ValidatePermissionsAsync(string actionName, int userId);
Task<ChatResponse> ExecuteActionAsync(AIFunctionCall functionCall);
```

#### 11.3.3 ChatQueryService

**Responsabilidad:** Ejecutar consultas dinámicas configuradas en BD

**Métodos principales:**
```csharp
Task<QueryResult> ExecuteQueryAsync(string queryName, Dictionary<string, object> parameters, int userId);
Task<List<ChatQuery>> GetAvailableQueriesAsync(int userId);
```

#### 11.3.4 ChatHistoryService

**Responsabilidad:** Gestionar historial de conversaciones

**Métodos principales:**
```csharp
Task RegisterAsync(ChatRequest request, AIFunctionCall aiResponse, ChatResponse result);
Task<List<ChatHistoryEntry>> GetHistoryAsync(string sessionId);
```

### 11.4 Nuevos Endpoints de API

#### 11.4.1 POST /api/chat/process

Procesa mensajes del chat y ejecuta acciones mediante Azure OpenAI Function Calling.

**Request:**
```json
{
  "Mensaje": "Dar de alta una unidad 101 con propietario Juan Pérez",
  "IdUsuario": 1,
  "IdEntidad": 1,
  "SessionId": "session_abc123"
}
```

**Response:**
```json
{
  "Success": true,
  "RequiereConfirmacion": true,
  "IdConfirmacion": 123,
  "Mensaje": "¿Confirmas crear la unidad 101 con propietario Juan Pérez?",
  "Botones": ["Confirmar", "Cancelar", "Modificar"]
}
```

#### 11.4.2 POST /api/chat/confirm

Confirma o cancela acciones pendientes.

#### 11.4.3 GET /api/chat/history/{sessionId}

Obtiene el historial de una sesión de chat.

### 11.5 Mejoras en el Widget de Chat

**Nuevas capacidades:**
- ✅ Soporte para confirmaciones con botones interactivos
- ✅ Renderizado de tablas interactivas con datos
- ✅ Navegación a páginas del sistema
- ✅ Visualización de gráficas y reportes
- ✅ Contexto de conversación persistente

**Archivos a modificar:**
- `JelaWeb/Scripts/widgets/chat-widget.js` - Lógica del widget
- `JelaWeb/Content/CSS/chat-widget.css` - Estilos adicionales

### 11.6 Flujo de Trabajo Ejemplo

**Escenario:** Usuario quiere crear una unidad

1. **Usuario:** "Dar de alta una unidad 101 con propietario Juan Pérez"
2. **Azure OpenAI:** Identifica función `crear_unidad` con parámetros `{numero: "101", propietario: "Juan Pérez"}`
3. **API:** Valida permisos del usuario
4. **API:** Solicita confirmación
5. **Usuario:** Click en "Confirmar"
6. **API:** Ejecuta `POST /api/crud/cat_unidades`
7. **API:** Registra en `op_chat_history`
8. **Bot:** "✓ He creado la unidad 101 con propietario Juan Pérez exitosamente. El ID asignado es 456."

### 11.7 Ejemplos de Configuración

**Acción CRUD - Crear Unidad:**
```sql
INSERT INTO conf_chat_actions (
    IdEntidad, NombreAccion, TipoAccion, Descripcion,
    EndpointAPI, MetodoHTTP, RequiereParametros, RequierePermisos
) VALUES (
    1, 'crear_unidad', 'CRUD', 'Crea una nueva unidad en el sistema',
    '/api/crud/cat_unidades', 'POST',
    '{"numero": "string", "propietario": "string"}', 'Unidades.Crear'
);
```

**Consulta Dinámica - Estado de Cuenta:**
```sql
INSERT INTO conf_chat_queries (
    IdEntidad, NombreConsulta, QuerySQL, Parametros, TipoResultado
) VALUES (
    1, 'estado_cuenta_unidad',
    'SELECT Concepto, Monto, FechaVencimiento, Estado FROM vw_estado_cuenta WHERE IdUnidad = @idUnidad',
    '{"idUnidad": "int"}', 'GRID'
);
```

### 11.8 Estimación de Esfuerzo

**Total: 6-10 días de desarrollo**

- Base de Datos (4 tablas): 1-2 días
- Servicios Backend (4 servicios): 2-3 días
- Endpoints API (3 endpoints): 1 día
- Widget de Chat (mejoras): 2-3 días
- Pruebas y ajustes: 1-2 días

### 11.9 Impacto Esperado

**Métricas de Éxito:**
- Reducción del 60% en tiempo de operaciones comunes
- 50% de usuarios activos usando el chat semanalmente
- 4.5/5 estrellas en satisfacción
- 90% de comandos ejecutados correctamente

---

## RESUMEN FINAL

Este diseño técnico completo especifica la implementación del **Módulo de Tickets Colaborativos con IA** para el sistema JELABBC, incluyendo la **Expansión Chat Web Avanzado**. El módulo se integra completamente con la infraestructura existente y sigue todos los estándares establecidos.

**Componentes principales:**
- ✅ 13 campos nuevos en tabla existente
- ✅ 8 tablas nuevas para logs, métricas y validación
- ✅ 5 tablas de Telegram (clientes, whitelist, blacklist, logs, queue)
- ✅ 4 tablas de Chat Web Avanzado (actions, queries, history, confirmations)
- ✅ 3 stored procedures
- ✅ 6 servicios de lógica de negocio en .NET 8
- ✅ 4 servicios de Chat Web Avanzado en .NET 8
- ✅ 4 archivos de endpoints en .NET 8
- ✅ 3 endpoints de Chat Web Avanzado
- ✅ 4 Background Services para tareas programadas
- ✅ 4 páginas web completas en VB.NET
- ✅ Mejoras en widget de chat (confirmaciones, tablas, navegación)
- ✅ Integraciones con VAPI, YCloud, Firebase, Telegram y Azure OpenAI

**Tiempo estimado:** 9-14 semanas con equipo de 4 personas

**Próximos pasos:**
1. Revisar y aprobar este diseño
2. Crear plan detallado de sprints
3. Comenzar con Fase 1 (Base de Datos)

---

**FIN DEL DOCUMENTO DE DISEÑO TÉCNICO**

**Versión:** 2.0  
**Fecha:** 18 de Enero de 2026  
**Estado:** Completo - Listo para Implementación


---

## 12. INTEGRACIÓN TELEGRAM

### 12.1 Resumen

Sistema completo de tickets vía Telegram Bot con validación de 7 niveles.

**Tablas requeridas (5):**
- `clientes_telegram` - Registro de clientes
- `clientes_whitelist` - Clientes pre-aprobados
- `clientes_blacklist` - Clientes bloqueados
- `logs_validacion` - Historial de validaciones
- `notifications_queue` - Cola de notificaciones

**Servicios VB.NET:**
- `TelegramValidationService.vb` - Validación de clientes
- `TelegramNotificationService.vb` - Envío de notificaciones

**Referencia completa:** Ver sección 3.3 y 3.4 del documento ANALISIS-COMPLETO-SISTEMA-AGENTE-IA.md

---

## 13. SEGURIDAD Y EVALUACIÓN DE IA

### 13.1 Brechas Críticas Identificadas

**8 brechas de seguridad detectadas:**

1. ✅ Restricción de fuentes no explícita
2. ✅ Aislamiento débil de datos entre usuarios
3. ✅ Falta de auditoría de violaciones
4. ✅ Evaluación binaria (no cuantitativa)
5. ✅ Sin validación de filtración de datos
6. ✅ Sin contexto de usuario en IA
7. ✅ Sin LLM Juez para evaluación
8. ✅ Sin tests de seguridad

### 13.2 Soluciones Implementadas

**Tablas nuevas (3):**
- `conf_ia_source_restrictions` - Restricciones de fuentes
- `op_ia_security_audit` - Auditoría de seguridad
- `op_ia_attempted_violations` - Intentos de violación

**Servicios nuevos:**
- `DataIsolationContext.vb` - Contexto aislado por usuario
- `PromptManager.vb` - Gestión de prompts con restricciones
- `LLMJudgeService.vb` - Evaluación cuantitativa (0-10)
- `SecurityPropertyTests.vb` - Suite de tests de seguridad

**Campos nuevos en op_tickets_v2:**
- `PuntuacionCorreccion` DECIMAL(4,2)
- `PuntuacionCompleitud` DECIMAL(4,2)
- `PuntuacionSeguridad` DECIMAL(4,2)
- `PuntuacionPrivacidad` DECIMAL(4,2)
- `PuntuacionCumplimiento` DECIMAL(4,2)
- `EvaluacionLLMJuez` JSON

### 13.3 Estimación

**Total: 144-204 horas (3.5-5 semanas con 1 dev)**

**Fase 1 (Urgente):** 44-64 horas
- DataIsolationContext
- Restricciones de fuentes
- Validación de filtración

**Fase 2 (Importante):** 52-72 horas
- Tablas de auditoría
- Test suite de seguridad
- LLM Judge

**Fase 3 (Mejora Continua):** 48-68 horas
- Dashboard de seguridad
- Métricas y alertas
- CI/CD integration

**Referencia completa:** Ver sección 13 del documento ANALISIS-COMPLETO-SISTEMA-AGENTE-IA.md

---

