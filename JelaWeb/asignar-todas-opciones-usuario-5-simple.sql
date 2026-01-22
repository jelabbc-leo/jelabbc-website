-- ============================================
-- Script SIMPLE: Asignar todas las opciones al Usuario ID 5
-- Descripción: Versión simplificada sin variables
-- Fecha: 2026-01-20
-- ============================================

USE jela_qa;

-- ============================================
-- PASO 1: Crear rol de Administrador Total (si no existe)
-- ============================================

INSERT IGNORE INTO conf_roles (Nombre, Descripcion, Activo)
VALUES ('Administrador Total', 'Rol con acceso completo a todas las opciones del sistema', 1);

-- ============================================
-- PASO 2: Limpiar asignaciones previas del usuario 5
-- ============================================

DELETE FROM conf_usuarioroles WHERE UsuarioId = 5;

-- ============================================
-- PASO 3: Asignar rol al usuario 5
-- ============================================

INSERT INTO conf_usuarioroles (UsuarioId, RolId, Activo)
SELECT 
    5,
    r.Id,
    1
FROM conf_roles r
WHERE r.Nombre = 'Administrador Total'
LIMIT 1;

-- ============================================
-- PASO 4: Limpiar opciones del rol
-- ============================================

DELETE FROM conf_rolopciones 
WHERE RolId = (SELECT Id FROM conf_roles WHERE Nombre = 'Administrador Total' LIMIT 1);

-- ============================================
-- PASO 5: Asignar TODAS las opciones al rol
-- ============================================

INSERT INTO conf_rolopciones (RolId, OpcionId, Activo)
SELECT 
    (SELECT Id FROM conf_roles WHERE Nombre = 'Administrador Total' LIMIT 1),
    o.Id,
    1
FROM conf_opciones o
WHERE o.Activo = 1;

-- ============================================
-- VERIFICACIÓN
-- ============================================

-- Ver opciones asignadas
SELECT 
    'VERIFICACIÓN: Opciones del Usuario ID 5' AS Info;

SELECT 
    o.Id,
    o.Nombre,
    o.RibbonTab,
    o.RibbonGroup,
    o.Url
FROM conf_opciones o
INNER JOIN conf_rolopciones ro ON o.Id = ro.OpcionId
INNER JOIN conf_usuarioroles ur ON ro.RolId = ur.RolId
WHERE ur.UsuarioId = 5
  AND o.Activo = 1
ORDER BY o.OrdenTab, o.OrdenGrupo, o.OrdenOpcion;

-- Contar opciones
SELECT 
    COUNT(*) AS TotalOpcionesAsignadas
FROM conf_opciones o
INNER JOIN conf_rolopciones ro ON o.Id = ro.OpcionId
INNER JOIN conf_usuarioroles ur ON ro.RolId = ur.RolId
WHERE ur.UsuarioId = 5
  AND o.Activo = 1;

-- ============================================
-- COMPLETADO
-- ============================================

SELECT '✓ Script completado - Usuario ID 5 tiene todas las opciones' AS Estado;
