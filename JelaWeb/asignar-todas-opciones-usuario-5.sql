-- ============================================
-- Script: Asignar todas las opciones al Usuario ID 5
-- Descripción: Crea un rol de Administrador Total y asigna todas las opciones del sistema
-- Fecha: 2026-01-20
-- Usuario: ID 5 (jlsg.gdl@gmail.com)
-- ============================================

USE jela_qa;

-- ============================================
-- PASO 1: Crear o actualizar rol de Administrador Total
-- ============================================

-- Verificar si existe el rol "Administrador Total"
SET @rolId = NULL;
SELECT Id INTO @rolId FROM conf_roles WHERE Nombre = 'Administrador Total' LIMIT 1;

-- Si no existe, crearlo
INSERT INTO conf_roles (Nombre, Descripcion, Activo)
SELECT 'Administrador Total', 'Rol con acceso completo a todas las opciones del sistema', 1
WHERE @rolId IS NULL;

-- Obtener el ID del rol (ya sea el existente o el recién creado)
SELECT Id INTO @rolId FROM conf_roles WHERE Nombre = 'Administrador Total' LIMIT 1;

SELECT CONCAT('✓ Rol "Administrador Total" ID: ', @rolId) AS Paso1;

-- ============================================
-- PASO 2: Asignar el rol al Usuario ID 5
-- ============================================

-- Eliminar asignaciones previas del usuario 5 para evitar duplicados
DELETE FROM conf_usuarioroles WHERE UsuarioId = 5;

-- Asignar el rol de Administrador Total al usuario 5
INSERT INTO conf_usuarioroles (UsuarioId, RolId, Activo)
VALUES (5, @rolId, 1);

SELECT '✓ Usuario ID 5 asignado al rol Administrador Total' AS Paso2;

-- ============================================
-- PASO 3: Asignar TODAS las opciones al rol
-- ============================================

-- Eliminar asignaciones previas del rol para evitar duplicados
DELETE FROM conf_rolopciones WHERE RolId = @rolId;

-- Asignar TODAS las opciones activas al rol de Administrador Total
INSERT INTO conf_rolopciones (RolId, OpcionId, Activo)
SELECT 
    @rolId,
    Id,
    1
FROM conf_opciones
WHERE Activo = 1;

-- Mostrar resumen
SELECT 
    CONCAT('✓ Se asignaron ', COUNT(*), ' opciones al rol Administrador Total') AS Paso3
FROM conf_rolopciones 
WHERE RolId = @rolId;

-- ============================================
-- PASO 4: Verificación - Mostrar opciones asignadas
-- ============================================

SELECT 
    '=== VERIFICACIÓN: Opciones asignadas al Usuario ID 5 ===' AS Titulo;

SELECT 
    o.Id,
    o.Nombre,
    o.Url,
    o.RibbonTab,
    o.RibbonGroup,
    o.OrdenTab,
    o.OrdenGrupo,
    o.OrdenOpcion,
    o.Icono,
    o.Activo
FROM conf_opciones o
INNER JOIN conf_rolopciones ro ON o.Id = ro.OpcionId
INNER JOIN conf_usuarioroles ur ON ro.RolId = ur.RolId
WHERE ur.UsuarioId = 5
  AND o.Activo = 1
  AND ro.Activo = 1
  AND ur.Activo = 1
ORDER BY o.OrdenTab, o.OrdenGrupo, o.OrdenOpcion;

-- ============================================
-- PASO 5: Resumen final
-- ============================================

SELECT 
    '=== RESUMEN FINAL ===' AS Titulo;

SELECT 
    u.Id AS UsuarioId,
    u.Nombre AS Usuario,
    u.Email,
    r.Id AS RolId,
    r.Nombre AS Rol,
    COUNT(DISTINCT ro.OpcionId) AS TotalOpciones
FROM conf_usuarios u
INNER JOIN conf_usuarioroles ur ON u.Id = ur.UsuarioId
INNER JOIN conf_roles r ON ur.RolId = r.Id
INNER JOIN conf_rolopciones ro ON r.Id = ro.RolId
WHERE u.Id = 5
  AND ur.Activo = 1
  AND ro.Activo = 1
GROUP BY u.Id, u.Nombre, u.Email, r.Id, r.Nombre;

-- ============================================
-- INFORMACIÓN ADICIONAL
-- ============================================

SELECT 
    '=== INFORMACIÓN DEL RIBBON ===' AS Titulo;

SELECT 
    o.RibbonTab AS Tab,
    o.RibbonGroup AS Grupo,
    MIN(o.OrdenTab) AS OrdenTab,
    MIN(o.OrdenGrupo) AS OrdenGrupo,
    COUNT(*) AS CantidadOpciones
FROM conf_opciones o
INNER JOIN conf_rolopciones ro ON o.Id = ro.OpcionId
INNER JOIN conf_usuarioroles ur ON ro.RolId = ur.RolId
WHERE ur.UsuarioId = 5
  AND o.Activo = 1
  AND ro.Activo = 1
  AND ur.Activo = 1
GROUP BY o.RibbonTab, o.RibbonGroup
ORDER BY MIN(o.OrdenTab), MIN(o.OrdenGrupo);

-- ============================================
-- SCRIPT COMPLETADO
-- ============================================

SELECT 
    '✓✓✓ SCRIPT COMPLETADO EXITOSAMENTE ✓✓✓' AS Estado,
    'El usuario ID 5 ahora tiene acceso a TODAS las opciones del sistema' AS Mensaje,
    'El ribbon se mostrará con todas las pestañas y grupos disponibles' AS Ribbon;
