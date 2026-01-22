-- ============================================
-- VERIFICAR Y CORREGIR TipoUsuario
-- ============================================

-- 1. Verificar el valor actual del usuario
SELECT Id, Username, Nombre, TipoUsuario, Activo
FROM conf_usuarios
WHERE Username = 'jlsg.gdl@gmail.com';

-- 2. Actualizar el TipoUsuario a 'AdministradorCondominios'
UPDATE conf_usuarios
SET TipoUsuario = 'AdministradorCondominios'
WHERE Username = 'jlsg.gdl@gmail.com';

-- 3. Verificar que se actualizó correctamente
SELECT Id, Username, Nombre, TipoUsuario, Activo
FROM conf_usuarios
WHERE Username = 'jlsg.gdl@gmail.com';

-- 4. Verificar la estructura de la columna TipoUsuario
SHOW COLUMNS FROM conf_usuarios LIKE 'TipoUsuario';
