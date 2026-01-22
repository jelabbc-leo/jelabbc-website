-- ============================================
-- Script: Asignar múltiples entidades al usuario ID 5
-- Propósito: Permitir que el usuario 5 pueda cambiar entre diferentes entidades
-- ============================================

-- Paso 1: Ver las entidades disponibles en la base de datos
SELECT 
    Id,
    Clave,
    Alias,
    RazonSocial,
    Activo,
    EsCondominio
FROM cat_entidades
WHERE Activo = 1
ORDER BY Id;

-- Paso 2: Ver la estructura de conf_usuarios
DESCRIBE conf_usuarios;

-- Paso 3: Ver el usuario 5
SELECT * FROM conf_usuarios WHERE Id = 5;

-- Paso 4: Ver las entidades actuales del usuario 5
SELECT 
    ue.IdUsuario,
    ue.IdEntidad,
    COALESCE(e.Alias, e.RazonSocial) AS NombreEntidad,
    u.Nombre AS NombreUsuario,
    u.TipoUsuario
FROM conf_usuario_entidades ue
INNER JOIN cat_entidades e ON ue.IdEntidad = e.Id
INNER JOIN conf_usuarios u ON ue.IdUsuario = u.Id
WHERE ue.IdUsuario = 5;

-- Paso 5: Asignar múltiples entidades al usuario 5
-- (Ajusta los IDs según las entidades que existan en tu base de datos)

-- Asignar entidad 1 (si existe y no está ya asignada)
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad)
SELECT 5, 1
WHERE EXISTS (SELECT 1 FROM cat_entidades WHERE Id = 1 AND Activo = 1)
AND NOT EXISTS (SELECT 1 FROM conf_usuario_entidades WHERE IdUsuario = 5 AND IdEntidad = 1);

-- Asignar entidad 2 (si existe y no está ya asignada)
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad)
SELECT 5, 2
WHERE EXISTS (SELECT 1 FROM cat_entidades WHERE Id = 2 AND Activo = 1)
AND NOT EXISTS (SELECT 1 FROM conf_usuario_entidades WHERE IdUsuario = 5 AND IdEntidad = 2);

-- Asignar entidad 3 (si existe y no está ya asignada)
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad)
SELECT 5, 3
WHERE EXISTS (SELECT 1 FROM cat_entidades WHERE Id = 3 AND Activo = 1)
AND NOT EXISTS (SELECT 1 FROM conf_usuario_entidades WHERE IdUsuario = 5 AND IdEntidad = 3);

-- Paso 6: Verificar las entidades asignadas
SELECT 
    ue.IdUsuario,
    ue.IdEntidad,
    COALESCE(e.Alias, e.RazonSocial) AS NombreEntidad,
    e.Activo,
    u.Nombre AS NombreUsuario,
    u.TipoUsuario
FROM conf_usuario_entidades ue
INNER JOIN cat_entidades e ON ue.IdEntidad = e.Id
INNER JOIN conf_usuarios u ON ue.IdUsuario = u.Id
WHERE ue.IdUsuario = 5
ORDER BY ue.IdEntidad;

-- Paso 7: Verificar que el usuario 5 es Administrador de Condominios
SELECT 
    Id,
    Nombre,
    Usuario,
    TipoUsuario,
    Activo
FROM conf_usuarios
WHERE Id = 5;

-- NOTA: El usuario debe tener TipoUsuario = 'AdministradorCondominios' 
-- para que aparezca el dropdown de entidades

-- Si necesitas cambiar el tipo de usuario:
-- UPDATE conf_usuarios 
-- SET TipoUsuario = 'AdministradorCondominios' 
-- WHERE Id = 5;
