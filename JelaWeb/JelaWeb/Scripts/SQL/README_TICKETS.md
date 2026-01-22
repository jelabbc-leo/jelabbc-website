# Scripts SQL - Módulo de Tickets Colaborativos con IA

## Descripción

Este directorio contiene los scripts SQL para crear la infraestructura de base de datos del módulo de Tickets Colaborativos con IA para el sistema JELABBC.

## Referencia

- **Diseño:** `.kiro/specs/tickets-colaborativos/design.md § 4`
- **Tareas:** `.kiro/specs/tickets-colaborativos/tasks.md § 1`

## Estructura de Scripts

### Script Maestro

- **`00_MAESTRO_crear_tablas_tickets.sql`** - Ejecuta todos los scripts en orden correcto

### Scripts Individuales

1. **`01_ALTER_op_tickets_v2_agregar_campos_nuevos.sql`**
   - Agrega 13 campos nuevos a la tabla existente `op_tickets_v2`
   - Crea 6 índices para optimización

2. **`02_CREATE_op_ticket_logs_sistema.sql`**
   - Crea tabla para auditoría de eventos del sistema
   - Severidad: Info, Warning, Error, Critical

3. **`03_CREATE_op_ticket_logs_interacciones.sql`**
   - Crea tabla para tracking de interacciones multicanal
   - Canales: VAPI, YCloud, Telegram, ChatWeb, Firebase

4. **`04_CREATE_op_ticket_logprompts.sql`**
   - Crea tabla para log de prompts enviados a IA
   - Incluye tokens utilizados y tiempo de respuesta

5. **`05_CREATE_op_ticket_metricas.sql`**
   - Crea tabla para métricas agregadas
   - Tipos: Diaria, Semanal, Mensual

6. **`06_CREATE_op_ticket_validacion_cliente.sql`**
   - Crea tabla para validación de clientes
   - Previene tickets duplicados

7. **`07_CREATE_op_ticket_notificaciones_whatsapp.sql`**
   - Crea tabla para cola de notificaciones WhatsApp
   - Incluye reintentos y estados

8. **`08_CREATE_op_ticket_robot_monitoreo.sql`**
   - Crea tabla para tracking del robot de monitoreo
   - Registra ejecuciones cada 5 minutos

9. **`09_CREATE_op_ticket_prompt_ajustes_log.sql`**
   - Crea tabla para log de ajustes de prompts
   - Registra mejoras automáticas cada 2 semanas

### Scripts de Telegram (5 tablas + trigger)

10. **`10_CREATE_op_telegram_clientes.sql`**
    - Crea tabla para gestión de clientes Telegram
    - Control de licencias, créditos y límites mensuales

11. **`11_CREATE_op_telegram_whitelist.sql`**
    - Crea tabla para clientes pre-aprobados
    - Prioridad: alta, media, baja

12. **`12_CREATE_op_telegram_blacklist.sql`**
    - Crea tabla para clientes bloqueados
    - Bloqueos permanentes o temporales

13. **`13_CREATE_op_telegram_logs_validacion.sql`**
    - Crea tabla para logs de validación
    - Sistema de 4 niveles de validación

14. **`14_CREATE_op_telegram_notifications_queue.sql`**
    - Crea tabla para cola de notificaciones Telegram
    - Procesamiento con reintentos

15. **`15_CREATE_trigger_y_campos_telegram.sql`**
    - Agrega 4 campos a op_tickets_v2 para Telegram
    - Crea trigger para notificaciones automáticas

## Cómo Ejecutar

### Opción 1: Script Maestro (Recomendado)

Ejecuta todos los scripts en orden:

```bash
mysql -u usuario -p jela_qa < 00_MAESTRO_crear_tablas_tickets.sql
```

### Opción 2: Scripts Individuales

Ejecuta cada script por separado en orden numérico:

```bash
mysql -u usuario -p jela_qa < 01_ALTER_op_tickets_v2_agregar_campos_nuevos.sql
mysql -u usuario -p jela_qa < 02_CREATE_op_ticket_logs_sistema.sql
mysql -u usuario -p jela_qa < 03_CREATE_op_ticket_logs_interacciones.sql
mysql -u usuario -p jela_qa < 04_CREATE_op_ticket_logprompts.sql
mysql -u usuario -p jela_qa < 05_CREATE_op_ticket_metricas.sql
mysql -u usuario -p jela_qa < 06_CREATE_op_ticket_validacion_cliente.sql
mysql -u usuario -p jela_qa < 07_CREATE_op_ticket_notificaciones_whatsapp.sql
mysql -u usuario -p jela_qa < 08_CREATE_op_ticket_robot_monitoreo.sql
mysql -u usuario -p jela_qa < 09_CREATE_op_ticket_prompt_ajustes_log.sql
mysql -u usuario -p jela_qa < 10_CREATE_op_telegram_clientes.sql
mysql -u usuario -p jela_qa < 11_CREATE_op_telegram_whitelist.sql
mysql -u usuario -p jela_qa < 12_CREATE_op_telegram_blacklist.sql
mysql -u usuario -p jela_qa < 13_CREATE_op_telegram_logs_validacion.sql
mysql -u usuario -p jela_qa < 14_CREATE_op_telegram_notifications_queue.sql
mysql -u usuario -p jela_qa < 15_CREATE_trigger_y_campos_telegram.sql
```

### Opción 3: Desde MySQL Workbench

1. Abrir MySQL Workbench
2. Conectar a la base de datos `jela_qa`
3. Abrir el script `00_MAESTRO_crear_tablas_tickets.sql`
4. Ejecutar el script completo

## Verificación Post-Ejecución

Después de ejecutar los scripts, verifica que todas las tablas fueron creadas:

```sql
USE jela_qa;

-- Verificar tabla alterada
SHOW COLUMNS FROM op_tickets_v2 LIKE 'TipoTicket';
SHOW COLUMNS FROM op_tickets_v2 LIKE 'ResueltoporIA';
SHOW COLUMNS FROM op_tickets_v2 LIKE 'ChatId';
SHOW COLUMNS FROM op_tickets_v2 LIKE 'ClienteValidado';

-- Verificar tablas nuevas
SHOW TABLES LIKE 'op_ticket_%';
SHOW TABLES LIKE 'op_telegram_%';

-- Verificar trigger
SHOW TRIGGERS WHERE `Trigger` = 'trg_NotificarCambioEstadoTelegram';

-- Contar registros (deben estar vacías)
SELECT 
    'op_ticket_logs_sistema' AS Tabla,
    COUNT(*) AS Registros
FROM op_ticket_logs_sistema
UNION ALL
SELECT 'op_ticket_logs_interacciones', COUNT(*) FROM op_ticket_logs_interacciones
UNION ALL
SELECT 'op_ticket_logprompts', COUNT(*) FROM op_ticket_logprompts
UNION ALL
SELECT 'op_ticket_metricas', COUNT(*) FROM op_ticket_metricas
UNION ALL
SELECT 'op_ticket_validacion_cliente', COUNT(*) FROM op_ticket_validacion_cliente
UNION ALL
SELECT 'op_ticket_notificaciones_whatsapp', COUNT(*) FROM op_ticket_notificaciones_whatsapp
UNION ALL
SELECT 'op_ticket_robot_monitoreo', COUNT(*) FROM op_ticket_robot_monitoreo
UNION ALL
SELECT 'op_ticket_prompt_ajustes_log', COUNT(*) FROM op_ticket_prompt_ajustes_log
UNION ALL
SELECT 'op_telegram_clientes', COUNT(*) FROM op_telegram_clientes
UNION ALL
SELECT 'op_telegram_whitelist', COUNT(*) FROM op_telegram_whitelist
UNION ALL
SELECT 'op_telegram_blacklist', COUNT(*) FROM op_telegram_blacklist
UNION ALL
SELECT 'op_telegram_logs_validacion', COUNT(*) FROM op_telegram_logs_validacion
UNION ALL
SELECT 'op_telegram_notifications_queue', COUNT(*) FROM op_telegram_notifications_queue;
```

## Convenciones Aplicadas

### Nomenclatura de Tablas
- Prefijo `op_` para tablas operativas/transaccionales
- Guion bajo (_) para separar palabras: `op_ticket_logs_sistema` ✅

### Nomenclatura de Campos
- **PascalCase** obligatorio: `TipoTicket`, `FechaCreacion`, `ResueltoporIA` ✅
- NO usar snake_case: `tipo_ticket` ❌
- NO usar camelCase: `tipoTicket` ❌

### Campos Estándar
Todas las tablas incluyen:
- `Id` - INT AUTO_INCREMENT PRIMARY KEY
- `IdEntidad` - INT NOT NULL DEFAULT 1
- `IdUsuarioCreacion` - INT DEFAULT NULL
- `FechaCreacion` - DATETIME DEFAULT CURRENT_TIMESTAMP
- `FechaUltimaActualizacion` - DATETIME (en tablas que lo requieren)
- `Activo` - TINYINT(1) DEFAULT 1

### Índices
- Índices en campos de búsqueda frecuente
- Índices en foreign keys
- Índices compuestos donde sea necesario

## Rollback

Si necesitas revertir los cambios:

```sql
-- ADVERTENCIA: Esto eliminará todas las tablas y datos

USE jela_qa;

-- Eliminar tablas nuevas (en orden inverso por dependencias)
DROP TABLE IF EXISTS op_telegram_notifications_queue;
DROP TABLE IF EXISTS op_telegram_logs_validacion;
DROP TABLE IF EXISTS op_telegram_blacklist;
DROP TABLE IF EXISTS op_telegram_whitelist;
DROP TABLE IF EXISTS op_telegram_clientes;
DROP TABLE IF EXISTS op_ticket_prompt_ajustes_log;
DROP TABLE IF EXISTS op_ticket_robot_monitoreo;
DROP TABLE IF EXISTS op_ticket_notificaciones_whatsapp;
DROP TABLE IF EXISTS op_ticket_validacion_cliente;
DROP TABLE IF EXISTS op_ticket_metricas;
DROP TABLE IF EXISTS op_ticket_logprompts;
DROP TABLE IF EXISTS op_ticket_logs_interacciones;
DROP TABLE IF EXISTS op_ticket_logs_sistema;

-- Eliminar trigger
DROP TRIGGER IF EXISTS trg_NotificarCambioEstadoTelegram;

-- Revertir cambios en op_tickets_v2
ALTER TABLE op_tickets_v2
  DROP COLUMN IF EXISTS TipoTicket,
  DROP COLUMN IF EXISTS IPOrigen,
  DROP COLUMN IF EXISTS DuracionLlamadaSegundos,
  DROP COLUMN IF EXISTS MomentoCorte,
  DROP COLUMN IF EXISTS IntentosReconexion,
  DROP COLUMN IF EXISTS MontoRelacionado,
  DROP COLUMN IF EXISTS PedidoRelacionado,
  DROP COLUMN IF EXISTS RiesgoFraude,
  DROP COLUMN IF EXISTS RequiereEscalamiento,
  DROP COLUMN IF EXISTS Impacto,
  DROP COLUMN IF EXISTS CSATScore,
  DROP COLUMN IF EXISTS ResueltoporIA,
  DROP COLUMN IF EXISTS Idioma,
  DROP COLUMN IF EXISTS ChatId,
  DROP COLUMN IF EXISTS ClienteValidado,
  DROP COLUMN IF EXISTS NivelValidacion,
  DROP COLUMN IF EXISTS CreditosUsados;

-- Eliminar índices
DROP INDEX IF EXISTS idx_ticket_tipo ON op_tickets_v2;
DROP INDEX IF EXISTS idx_ticket_ip ON op_tickets_v2;
DROP INDEX IF EXISTS idx_ticket_riesgo ON op_tickets_v2;
DROP INDEX IF EXISTS idx_ticket_escalamiento ON op_tickets_v2;
DROP INDEX IF EXISTS idx_ticket_resuelto_ia ON op_tickets_v2;
DROP INDEX IF EXISTS idx_ticket_idioma ON op_tickets_v2;
DROP INDEX IF EXISTS idx_tickets_chat_id ON op_tickets_v2;
DROP INDEX IF EXISTS idx_tickets_cliente_validado ON op_tickets_v2;
```

## Próximos Pasos

Después de ejecutar estos scripts, continuar con:

1. **Sección 1.3** - Stored Procedures (3 procedures)
2. **Sección 1.4** - Verificación de Base de Datos
3. **Sección 2** - API Backend (.NET 8)

## Notas Importantes

- ✅ Todos los scripts son idempotentes (pueden ejecutarse múltiples veces)
- ✅ Incluyen verificaciones de existencia (`IF NOT EXISTS`)
- ✅ Incluyen comentarios descriptivos en cada campo
- ✅ Siguen las convenciones de nomenclatura del proyecto
- ✅ Incluyen índices para optimización de consultas
- ✅ Trigger automático para notificaciones de Telegram
- ⚠️ Requieren que existan las tablas: `cat_entidades`, `conf_usuarios`, `op_tickets_v2`

## Sistema de Validación de Telegram (4 Niveles)

El sistema implementa validación en cascada:

**Nivel 1:** Verificación de Blacklist
- Consulta `op_telegram_blacklist`
- Rechaza si está bloqueado

**Nivel 2:** Estado del Cliente
- Consulta `op_telegram_clientes`
- Verifica estado 'activo'

**Nivel 3:** Licencia/Suscripción
- Verifica `FechaVencimiento`
- Rechaza si venció

**Nivel 4:** Límite Mensual
- Compara `TicketsMesActual` con `LimiteTicketsMes`
- Rechaza si alcanzó el límite

## Soporte

Para dudas o problemas:
- Revisar el documento de diseño: `.kiro/specs/tickets-colaborativos/design.md`
- Revisar el documento de tareas: `.kiro/specs/tickets-colaborativos/tasks.md`
- Consultar los estándares UI: `.kiro/specs/ecosistema-jelabbc/ui-standards.md`

---

**Fecha de Creación:** 18 de Enero de 2026  
**Versión:** 1.0  
**Estado:** Listo para Ejecución
