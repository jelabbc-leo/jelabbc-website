-- ============================================================================
-- SCRIPT: 15_CREATE_trigger_y_campos_telegram.sql
-- PROPÓSITO: Crear trigger y agregar campos adicionales para Telegram
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA - Integración Telegram
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4.4.6 y § 4.4.7
-- ============================================================================

USE jela_qa;

-- ============================================================================
-- PARTE 1: AGREGAR CAMPOS ADICIONALES EN op_tickets_v2 PARA TELEGRAM
-- ============================================================================

SELECT 'Agregando campos adicionales para Telegram en op_tickets_v2...' AS Paso;

-- Verificar y agregar columna ChatId
SET @dbname = DATABASE();
SET @tablename = 'op_tickets_v2';
SET @columnname = 'ChatId';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @columnname)
  ) > 0,
  'SELECT ''Column ChatId already exists'' AS Info;',
  'ALTER TABLE op_tickets_v2 ADD COLUMN ChatId BIGINT DEFAULT NULL COMMENT ''ID de Telegram del cliente'';'
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- Verificar y agregar columna ClienteValidado
SET @columnname = 'ClienteValidado';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @columnname)
  ) > 0,
  'SELECT ''Column ClienteValidado already exists'' AS Info;',
  'ALTER TABLE op_tickets_v2 ADD COLUMN ClienteValidado BOOLEAN DEFAULT 0 COMMENT ''Si el cliente pasó validación'';'
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- Verificar y agregar columna NivelValidacion
SET @columnname = 'NivelValidacion';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @columnname)
  ) > 0,
  'SELECT ''Column NivelValidacion already exists'' AS Info;',
  'ALTER TABLE op_tickets_v2 ADD COLUMN NivelValidacion VARCHAR(50) DEFAULT ''pendiente'' COMMENT ''Nivel de validación alcanzado'';'
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- Verificar y agregar columna CreditosUsados
SET @columnname = 'CreditosUsados';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (COLUMN_NAME = @columnname)
  ) > 0,
  'SELECT ''Column CreditosUsados already exists'' AS Info;',
  'ALTER TABLE op_tickets_v2 ADD COLUMN CreditosUsados INT DEFAULT 0 COMMENT ''Créditos consumidos por este ticket'';'
));
PREPARE alterIfNotExists FROM @preparedStatement;
EXECUTE alterIfNotExists;
DEALLOCATE PREPARE alterIfNotExists;

-- Crear índices para los nuevos campos (solo si no existen)
SET @indexname = 'idx_tickets_chat_id';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (INDEX_NAME = @indexname)
  ) > 0,
  'SELECT ''Index idx_tickets_chat_id already exists'' AS Info;',
  'CREATE INDEX idx_tickets_chat_id ON op_tickets_v2(ChatId);'
));
PREPARE createIndexIfNotExists FROM @preparedStatement;
EXECUTE createIndexIfNotExists;
DEALLOCATE PREPARE createIndexIfNotExists;

SET @indexname = 'idx_tickets_cliente_validado';
SET @preparedStatement = (SELECT IF(
  (
    SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
    WHERE
      (TABLE_SCHEMA = @dbname)
      AND (TABLE_NAME = @tablename)
      AND (INDEX_NAME = @indexname)
  ) > 0,
  'SELECT ''Index idx_tickets_cliente_validado already exists'' AS Info;',
  'CREATE INDEX idx_tickets_cliente_validado ON op_tickets_v2(ClienteValidado);'
));
PREPARE createIndexIfNotExists FROM @preparedStatement;
EXECUTE createIndexIfNotExists;
DEALLOCATE PREPARE createIndexIfNotExists;

SELECT '✓ Campos adicionales agregados exitosamente' AS Resultado;

-- ============================================================================
-- PARTE 2: CREAR TRIGGER trg_NotificarCambioEstadoTelegram
-- ============================================================================

SELECT 'Creando trigger trg_NotificarCambioEstadoTelegram...' AS Paso;

-- Eliminar trigger si existe (para permitir re-ejecución)
DROP TRIGGER IF EXISTS trg_NotificarCambioEstadoTelegram;

DELIMITER $$

CREATE TRIGGER trg_NotificarCambioEstadoTelegram
AFTER UPDATE ON op_tickets_v2
FOR EACH ROW
BEGIN
  -- Solo si cambió el estado y el canal es Telegram
  IF OLD.Estado != NEW.Estado AND NEW.Canal = 'Telegram' AND NEW.ChatId IS NOT NULL THEN
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
      NEW.ChatId,
      'cambio_estado',
      NEW.Estado,
      CONCAT('🔔 Tu ticket #', NEW.Id, ' ha cambiado a estado: ', NEW.Estado),
      NEW.IdUsuarioCreacion
    );
  END IF;
END$$

DELIMITER ;

SELECT '✓ Trigger trg_NotificarCambioEstadoTelegram creado exitosamente' AS Resultado;

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================

SELECT 'Verificando campos agregados...' AS Paso;

SELECT 
    COLUMN_NAME AS Campo,
    COLUMN_TYPE AS Tipo,
    IS_NULLABLE AS Nulable,
    COLUMN_DEFAULT AS ValorPorDefecto,
    COLUMN_COMMENT AS Comentario
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'jela_qa'
  AND TABLE_NAME = 'op_tickets_v2'
  AND COLUMN_NAME IN ('ChatId', 'ClienteValidado', 'NivelValidacion', 'CreditosUsados')
ORDER BY ORDINAL_POSITION;

SELECT 'Verificando índices creados...' AS Paso;

SELECT 
    INDEX_NAME AS Indice,
    COLUMN_NAME AS Columna,
    SEQ_IN_INDEX AS Secuencia
FROM INFORMATION_SCHEMA.STATISTICS
WHERE TABLE_SCHEMA = 'jela_qa'
  AND TABLE_NAME = 'op_tickets_v2'
  AND INDEX_NAME IN ('idx_tickets_chat_id', 'idx_tickets_cliente_validado')
ORDER BY INDEX_NAME, SEQ_IN_INDEX;

SELECT 'Verificando trigger creado...' AS Paso;

SELECT 
    TRIGGER_NAME AS TriggerName,
    EVENT_MANIPULATION AS Evento,
    EVENT_OBJECT_TABLE AS Tabla,
    ACTION_TIMING AS Momento
FROM INFORMATION_SCHEMA.TRIGGERS
WHERE TRIGGER_SCHEMA = 'jela_qa'
  AND TRIGGER_NAME = 'trg_NotificarCambioEstadoTelegram';

SELECT '✓ Script ejecutado exitosamente' AS Estado;
SELECT 'Se agregaron 4 campos nuevos a op_tickets_v2' AS Resumen1;
SELECT 'Se crearon 2 índices' AS Resumen2;
SELECT 'Se creó 1 trigger para notificaciones automáticas' AS Resumen3;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
