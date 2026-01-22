-- =============================================
-- Script: Agregar campos IdEntidadPrincipal y LicenciasDisponibles a conf_usuarios
-- Fecha: 20 de Enero de 2026
-- Descripción: Agregar campos para entidad principal y control de licencias
-- Referencia: .kiro/specs/sistema-multi-entidad/design.md § 3.1
-- =============================================

USE jelabbc;

-- Verificar si los campos ya existen
SELECT 
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
     WHERE TABLE_SCHEMA = 'jelabbc' AND TABLE_NAME = 'conf_usuarios' AND COLUMN_NAME = 'IdEntidadPrincipal') as existe_IdEntidadPrincipal,
    (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS 
     WHERE TABLE_SCHEMA = 'jelabbc' AND TABLE_NAME = 'conf_usuarios' AND COLUMN_NAME = 'LicenciasDisponibles') as existe_LicenciasDisponibles;

-- Agregar campo IdEntidadPrincipal
ALTER TABLE conf_usuarios
ADD COLUMN IdEntidadPrincipal INT NULL 
  COMMENT 'Entidad principal para usuarios de una sola entidad';

-- Agregar campo LicenciasDisponibles
ALTER TABLE conf_usuarios
ADD COLUMN LicenciasDisponibles INT DEFAULT 0
  COMMENT 'Número de licencias disponibles para dar de alta entidades (solo para AdministradorCondominios)';

-- Agregar índice para IdEntidadPrincipal
ALTER TABLE conf_usuarios
ADD INDEX idx_usuarios_entidad_principal (IdEntidadPrincipal);

-- Agregar foreign key a cat_entidades
ALTER TABLE conf_usuarios
ADD CONSTRAINT fk_usuarios_entidad_principal 
  FOREIGN KEY (IdEntidadPrincipal) REFERENCES cat_entidades(Id)
  ON DELETE SET NULL
  ON UPDATE CASCADE;

-- Verificar que los campos se crearon correctamente
DESCRIBE conf_usuarios;

-- Verificar índices
SHOW INDEX FROM conf_usuarios WHERE Key_name IN ('idx_usuarios_entidad_principal');

-- Verificar foreign keys
SELECT 
    CONSTRAINT_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'jelabbc' 
  AND TABLE_NAME = 'conf_usuarios'
  AND CONSTRAINT_NAME = 'fk_usuarios_entidad_principal';

-- Mensaje de confirmación
SELECT 'Campos IdEntidadPrincipal y LicenciasDisponibles agregados exitosamente a conf_usuarios' AS Resultado;
