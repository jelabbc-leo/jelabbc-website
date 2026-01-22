-- =============================================
-- Script: Crear tabla conf_usuario_entidades
-- Fecha: 20 de Enero de 2026
-- Descripción: Crear tabla de relación muchos a muchos entre usuarios y entidades
-- Referencia: .kiro/specs/sistema-multi-entidad/design.md § 3.2
-- =============================================

USE jelabbc;

-- Verificar si la tabla ya existe
SELECT COUNT(*) as existe 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'jelabbc' 
  AND TABLE_NAME = 'conf_usuario_entidades';

-- Crear tabla conf_usuario_entidades
CREATE TABLE IF NOT EXISTS conf_usuario_entidades (
  Id INT NOT NULL AUTO_INCREMENT COMMENT 'Identificador único',
  IdUsuario INT NOT NULL COMMENT 'FK a conf_usuarios',
  IdEntidad INT NOT NULL COMMENT 'FK a cat_entidades',
  EsPrincipal BOOLEAN DEFAULT FALSE COMMENT 'Indica si es la entidad principal del usuario',
  FechaAsignacion DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Fecha en que se asignó la entidad al usuario',
  IdUsuarioCreacion INT DEFAULT 1 COMMENT 'Usuario que realizó la asignación',
  FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP COMMENT 'Fecha de creación del registro',
  Activo TINYINT(1) DEFAULT 1 COMMENT 'Estado del registro (1=Activo, 0=Inactivo)',
  
  PRIMARY KEY (Id),
  
  -- Índice único para evitar duplicados (un usuario no puede tener la misma entidad dos veces)
  UNIQUE INDEX uk_usuario_entidad (IdUsuario, IdEntidad),
  
  -- Índices para mejorar performance en consultas
  INDEX idx_usuario (IdUsuario),
  INDEX idx_entidad (IdEntidad),
  INDEX idx_principal (EsPrincipal),
  
  -- Foreign keys con acciones en cascada
  CONSTRAINT fk_usuario_entidades_usuario 
    FOREIGN KEY (IdUsuario) REFERENCES conf_usuarios(Id) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE,
    
  CONSTRAINT fk_usuario_entidades_entidad 
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id) 
    ON DELETE CASCADE 
    ON UPDATE CASCADE
    
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Relación muchos a muchos entre usuarios y entidades para sistema multi-entidad';

-- Verificar que la tabla se creó correctamente
DESCRIBE conf_usuario_entidades;

-- Verificar índices
SHOW INDEX FROM conf_usuario_entidades;

-- Verificar foreign keys
SELECT 
    CONSTRAINT_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'jelabbc' 
  AND TABLE_NAME = 'conf_usuario_entidades'
  AND REFERENCED_TABLE_NAME IS NOT NULL;

-- Mensaje de confirmación
SELECT 'Tabla conf_usuario_entidades creada exitosamente' AS Resultado;
