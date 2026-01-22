-- ============================================
-- Script: Asignar múltiples entidades al usuario ID 5
-- Propósito: Permitir que el usuario 5 (Administrador de Condominios) 
--            pueda cambiar entre diferentes entidades usando el dropdown
-- ============================================

-- Paso 1: Ver las entidades disponibles en la base de datos
SELECT 
    Id,
    Clave,
    Alias,
    RazonSocial,
    Activo,
    EsCondominio,
    TipoCondominio
FROM cat_entidades
WHERE Activo = 1
ORDER BY Id;

-- Paso 2: Ver las entidades actuales del usuario 5
SELECT 
    ue.IdUsuario,
    ue.IdEntidad,
    COALESCE(e.Alias, e.RazonSocial) AS NombreEntidad,
    u.Nombre AS NombreUsuario
FROM cat_usuarios_entidades ue
INNER JOIN cat_entidades e ON ue.IdEntidad = e.Id
INNER JOIN cat_usuarios u ON ue.IdUsuario = u.Id
WHERE ue.IdUsuario = 5;

-- Paso 3: Asignar múltiples entidades al usuario 5
-- (Ajusta los IDs según las entidades que existan en tu base de datos)

-- Primero, eliminar asignaciones existentes si quieres empezar limpio
-- DELETE FROM cat_usuarios_entidades WHERE IdUsuario = 5;

-- Asignar entidad 1 (si existe)
INSERT INTO cat_usuarios_entidades (IdUsuario, IdEntidad, FechaAsignacion)
SELECT 5, 1, NOW()
WHERE EXISTS (SELECT 1 FROM cat_entidades WHERE Id = 1 AND Activo = 1)
AND NOT EXISTS (SELECT 1 FROM cat_usuarios_entidades WHERE IdUsuario = 5 AND IdEntidad = 1);

-- Asignar entidad 2 (si existe)
INSERT INTO cat_usuarios_entidades (IdUsuario, IdEntidad, FechaAsignacion)
SELECT 5, 2, NOW()
WHERE EXISTS (SELECT 1 FROM cat_entidades WHERE Id = 2 AND Activo = 1)
AND NOT EXISTS (SELECT 1 FROM cat_usuarios_entidades WHERE IdUsuario = 5 AND IdEntidad = 2);

-- Asignar entidad 3 (si existe)
INSERT INTO cat_usuarios_entidades (IdUsuario, IdEntidad, FechaAsignacion)
SELECT 5, 3, NOW()
WHERE EXISTS (SELECT 1 FROM cat_entidades WHERE Id = 3 AND Activo = 1)
AND NOT EXISTS (SELECT 1 FROM cat_usuarios_entidades WHERE IdUsuario = 5 AND IdEntidad = 3);

-- Paso 4: Verificar las entidades asignadas
SELECT 
    ue.IdUsuario,
    ue.IdEntidad,
    COALESCE(e.Alias, e.RazonSocial) AS NombreEntidad,
    e.Activo,
    ue.FechaAsignacion,
    u.Nombre AS NombreUsuario,
    u.TipoUsuario
FROM cat_usuarios_entidades ue
INNER JOIN cat_entidades e ON ue.IdEntidad = e.Id
INNER JOIN cat_usuarios u ON ue.IdUsuario = u.Id
WHERE ue.IdUsuario = 5
ORDER BY ue.IdEntidad;

-- Paso 5: Verificar que el usuario 5 es Administrador de Condominios
SELECT 
    Id,
    Nombre,
    Usuario,
    TipoUsuario,
    Activo
FROM cat_usuarios
WHERE Id = 5;

-- NOTA: El usuario debe tener TipoUsuario = 'AdministradorCondominios' 
-- para que aparezca el dropdown de entidades

-- Si necesitas cambiar el tipo de usuario:
-- UPDATE cat_usuarios 
-- SET TipoUsuario = 'AdministradorCondominios' 
-- WHERE Id = 5;
