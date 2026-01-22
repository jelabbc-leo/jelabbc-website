-- ============================================================================
-- SCRIPT: 13_CREATE_op_telegram_logs_validacion.sql
-- PROPÓSITO: Crear tabla op_telegram_logs_validacion para sistema de validación
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA - Integración Telegram
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4.4.4
-- ============================================================================

USE jela_qa;

SELECT 'Creando tabla op_telegram_logs_validacion...' AS Paso;

-- ============================================================================
-- CREAR TABLA op_telegram_logs_validacion
-- ============================================================================

CREATE TABLE IF NOT EXISTS op_telegram_logs_validacion (
  Id INT NOT NULL AUTO_INCREMENT,
  IdEntidad INT NOT NULL DEFAULT 1,
  ChatId BIGINT NOT NULL,
  
  -- Validación
  FechaValidacion DATETIME DEFAULT CURRENT_TIMESTAMP,
  Resultado ENUM('aprobado', 'rechazado', 'pendiente') NOT NULL,
  NivelAlcanzado VARCHAR(50) DEFAULT NULL COMMENT 'Nivel de validación alcanzado (1-4)',
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

SELECT '✓ Tabla op_telegram_logs_validacion creada exitosamente' AS Resultado;

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================

SELECT 'Verificando estructura de la tabla...' AS Paso;

DESCRIBE op_telegram_logs_validacion;

SELECT 
    COUNT(*) AS TotalColumnas,
    'op_telegram_logs_validacion' AS Tabla
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'jela_qa'
  AND TABLE_NAME = 'op_telegram_logs_validacion';

SELECT '✓ Script ejecutado exitosamente' AS Estado;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
