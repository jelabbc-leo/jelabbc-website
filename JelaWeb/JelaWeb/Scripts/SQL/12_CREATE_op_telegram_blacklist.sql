-- ============================================================================
-- SCRIPT: 12_CREATE_op_telegram_blacklist.sql
-- PROPÓSITO: Crear tabla op_telegram_blacklist para clientes bloqueados
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA - Integración Telegram
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4.4.3
-- ============================================================================

USE jela_qa;

SELECT 'Creando tabla op_telegram_blacklist...' AS Paso;

-- ============================================================================
-- CREAR TABLA op_telegram_blacklist
-- ============================================================================

CREATE TABLE IF NOT EXISTS op_telegram_blacklist (
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

SELECT '✓ Tabla op_telegram_blacklist creada exitosamente' AS Resultado;

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================

SELECT 'Verificando estructura de la tabla...' AS Paso;

DESCRIBE op_telegram_blacklist;

SELECT 
    COUNT(*) AS TotalColumnas,
    'op_telegram_blacklist' AS Tabla
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'jela_qa'
  AND TABLE_NAME = 'op_telegram_blacklist';

SELECT '✓ Script ejecutado exitosamente' AS Estado;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
