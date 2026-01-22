-- =============================================
-- Script: Agregar campo TipoUsuario a conf_usuarios
-- Fecha: 20 de Enero de 2026
-- Descripción: Agregar campo para identificar tipo de usuario (Administrador Condominios, Mesa Directiva, Residente, Empleado)
-- Referencia: .kiro/specs/sistema-multi-entidad/design.md § 3.1
-- =============================================

USE jelabbc;

-- Verificar si el campo ya existe
SELECT COUNT(*) as existe 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_SCHEMA = 'jelabbc' 
  AND TABLE_NAME = 'conf_usuarios' 
  AND COLUMN_NAME = 'TipoUsuario';

-- Agregar campo TipoUsuario
ALTER TABLE conf_usuarios
ADD COLUMN TipoUsuario ENUM('AdministradorCondominios', 'MesaDirectiva', 'Residente', 'Empleado') 
  DEFAULT 'Residente' 
  COMMENT 'Tipo de usuario para determinar acceso multi-entidad';

-- Agregar índice para mejorar performance en consultas por tipo
ALTER TABLE conf_usuarios
ADD INDEX idx_usuarios_tipo (TipoUsuario);

-- Verificar que el campo se creó correctamente
DESCRIBE conf_usuarios;

-- Verificar índices
SHOW INDEX FROM conf_usuarios WHERE Key_name = 'idx_usuarios_tipo';

-- Mensaje de confirmación
SELECT 'Campo TipoUsuario agregado exitosamente a conf_usuarios' AS Resultado;
