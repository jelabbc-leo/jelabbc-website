-- ============================================================================
-- SCRIPT: 14_CREATE_op_telegram_notifications_queue.sql
-- PROPÓSITO: Crear tabla op_telegram_notifications_queue para cola de notificaciones
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA - Integración Telegram
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4.4.5
-- ============================================================================

USE jela_qa;

SELECT 'Creando tabla op_telegram_notifications_queue...' AS Paso;

-- ============================================================================
-- CREAR TABLA op_telegram_notifications_queue
-- ============================================================================

CREATE TABLE IF NOT EXISTS op_telegram_notifications_queue (
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

SELECT '✓ Tabla op_telegram_notifications_queue creada exitosamente' AS Resultado;

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================

SELECT 'Verificando estructura de la tabla...' AS Paso;

DESCRIBE op_telegram_notifications_queue;

SELECT 
    COUNT(*) AS TotalColumnas,
    'op_telegram_notifications_queue' AS Tabla
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'jela_qa'
  AND TABLE_NAME = 'op_telegram_notifications_queue';

SELECT '✓ Script ejecutado exitosamente' AS Estado;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
