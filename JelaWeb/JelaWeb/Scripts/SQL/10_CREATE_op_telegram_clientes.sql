-- ============================================================================
-- SCRIPT: 10_CREATE_op_telegram_clientes.sql
-- PROPÓSITO: Crear tabla op_telegram_clientes para gestión de clientes Telegram
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA - Integración Telegram
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4.4.1
-- ============================================================================

USE jela_qa;

SELECT 'Creando tabla op_telegram_clientes...' AS Paso;

-- ============================================================================
-- CREAR TABLA op_telegram_clientes
-- ============================================================================

CREATE TABLE IF NOT EXISTS op_telegram_clientes (
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

SELECT '✓ Tabla op_telegram_clientes creada exitosamente' AS Resultado;

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================

SELECT 'Verificando estructura de la tabla...' AS Paso;

DESCRIBE op_telegram_clientes;

SELECT 
    COUNT(*) AS TotalColumnas,
    'op_telegram_clientes' AS Tabla
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = 'jela_qa'
  AND TABLE_NAME = 'op_telegram_clientes';

SELECT '✓ Script ejecutado exitosamente' AS Estado;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
