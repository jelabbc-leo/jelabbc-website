-- ============================================================================
-- SCRIPT: 11_CREATE_op_telegram_whitelist.sql
-- PROPÓSITO: Crear tabla op_telegram_whitelist para clientes pre-aprobados
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA - Integración Telegram
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4.4.2
-- ============================================================================

USE jela_qa;

SELECT 'Creando tabla op_telegram_whitelist...' AS Paso;

-- ============================================================================
-- CREAR TABLA op_telegram_whitelist
-- ============================================================================

CREATE TABLE IF NOT EXISTS op_telegram_whitelist (
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

SELECT '✓ Tabla op_telegram_whitelist creada exitosamente' AS Resultado;

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================

SELECT 'Verificando estructura de la tabla...' AS Paso;

DESCRIBE op_telegram_whitelist;

SELECT 
    COUNT(*) AS TotalColumnas,
    'op_telegram_whitelist' AS Tabla
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'jela_qa'
  AND TABLE_NAME = 'op_telegram_whitelist';

SELECT '✓ Script ejecutado exitosamente' AS Estado;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
