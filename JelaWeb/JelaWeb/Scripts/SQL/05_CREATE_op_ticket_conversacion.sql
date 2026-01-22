-- =============================================
-- Script: Crear tabla op_ticket_conversacion
-- Descripción: Tabla para almacenar el historial de conversación de tickets
-- Fecha: 2026-01-21
-- Autor: Sistema JELA
-- =============================================

USE jela_qa;

-- Verificar si la tabla ya existe
SELECT 'Verificando si la tabla op_ticket_conversacion existe...' AS Status;

-- Crear tabla op_ticket_conversacion
CREATE TABLE IF NOT EXISTS op_ticket_conversacion (
  Id INT NOT NULL AUTO_INCREMENT,
  IdTicket INT NOT NULL COMMENT 'ID del ticket (FK a op_tickets_v2)',
  TipoMensaje VARCHAR(50) NOT NULL COMMENT 'Tipo de mensaje (Cliente, Agente, Sistema, IA)',
  Mensaje TEXT NOT NULL COMMENT 'Contenido del mensaje',
  EsRespuestaIA TINYINT(1) DEFAULT 0 COMMENT 'Indica si es una respuesta generada por IA',
  IdUsuarioEnvio INT DEFAULT NULL COMMENT 'ID del usuario que envió el mensaje (FK a conf_usuarios)',
  NombreUsuarioEnvio VARCHAR(200) DEFAULT NULL COMMENT 'Nombre del usuario que envió (para clientes externos)',
  FechaEnvio DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Fecha y hora de envío',
  Leido TINYINT(1) DEFAULT 0 COMMENT 'Indica si el mensaje ha sido leído',
  FechaLectura DATETIME DEFAULT NULL COMMENT 'Fecha de lectura del mensaje',
  PRIMARY KEY (Id)
)
ENGINE = INNODB,
CHARACTER SET utf8mb4,
COLLATE utf8mb4_unicode_ci,
COMMENT = 'Historial de conversación de tickets - Módulo 07',
ROW_FORMAT = DYNAMIC;

SELECT 'Tabla op_ticket_conversacion creada exitosamente' AS Status;

-- Crear índices
SELECT 'Creando índices...' AS Status;

-- Índice por fecha de envío
CREATE INDEX IF NOT EXISTS idx_conversacion_fecha 
ON op_ticket_conversacion (FechaEnvio);

-- Índice por ticket
CREATE INDEX IF NOT EXISTS idx_conversacion_ticket 
ON op_ticket_conversacion (IdTicket);

-- Índice por tipo de mensaje
CREATE INDEX IF NOT EXISTS idx_conversacion_tipo 
ON op_ticket_conversacion (TipoMensaje);

SELECT 'Índices creados exitosamente' AS Status;

-- Crear foreign key (solo si no existe)
SELECT 'Creando foreign key...' AS Status;

-- Verificar si la FK ya existe antes de crearla
SET @fk_exists = (
    SELECT COUNT(*)
    FROM information_schema.TABLE_CONSTRAINTS
    WHERE CONSTRAINT_SCHEMA = 'jela_qa'
    AND TABLE_NAME = 'op_ticket_conversacion'
    AND CONSTRAINT_NAME = 'op_ticket_conversacion_ibfk_1'
    AND CONSTRAINT_TYPE = 'FOREIGN KEY'
);

-- Crear FK solo si no existe
SET @sql = IF(@fk_exists = 0,
    'ALTER TABLE op_ticket_conversacion ADD CONSTRAINT op_ticket_conversacion_ibfk_1 FOREIGN KEY (IdTicket) REFERENCES op_tickets_v2 (Id) ON DELETE CASCADE',
    'SELECT ''Foreign key ya existe'' AS Status'
);

PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;

SELECT 'Foreign key configurada exitosamente' AS Status;

-- Verificación final
SELECT 
    'Verificación final' AS Status,
    COUNT(*) AS TotalColumnas
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = 'jela_qa'
AND TABLE_NAME = 'op_ticket_conversacion';

SELECT '✓ Script completado exitosamente' AS Status;
