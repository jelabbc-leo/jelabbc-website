-- =============================================
-- Script: Migrar datos existentes a sistema multi-entidad
-- Fecha: 20 de Enero de 2026
-- Descripción: Migrar usuarios existentes a conf_usuario_entidades y actualizar campos nuevos
-- Referencia: .kiro/specs/sistema-multi-entidad/design.md § 3.3
-- =============================================

USE jelabbc;

-- =============================================
-- PASO 1: Migrar relaciones usuario-entidad existentes
-- =============================================

-- Insertar relaciones para usuarios que ya tienen IdEntidad
-- Marcar como entidad principal (EsPrincipal = TRUE)
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad, EsPrincipal, FechaAsignacion, IdUsuarioCreacion, FechaCreacion, Activo)
SELECT 
    u.Id AS IdUsuario,
    COALESCE(u.IdEntidad, 1) AS IdEntidad,  -- Si no tiene IdEntidad, asignar entidad 1 por defecto
    TRUE AS EsPrincipal,
    NOW() AS FechaAsignacion,
    1 AS IdUsuarioCreacion,
    NOW() AS FechaCreacion,
    1 AS Activo
FROM conf_usuarios u
WHERE u.Activo = 1
  AND NOT EXISTS (
    SELECT 1 FROM conf_usuario_entidades ue 
    WHERE ue.IdUsuario = u.Id AND ue.IdEntidad = COALESCE(u.IdEntidad, 1)
  );

-- Verificar cuántos registros se insertaron
SELECT COUNT(*) AS 'Usuarios migrados a conf_usuario_entidades' 
FROM conf_usuario_entidades;

-- =============================================
-- PASO 2: Actualizar IdEntidadPrincipal en conf_usuarios
-- =============================================

-- Actualizar IdEntidadPrincipal basándose en la entidad marcada como principal
UPDATE conf_usuarios u
INNER JOIN conf_usuario_entidades ue ON u.Id = ue.IdUsuario AND ue.EsPrincipal = TRUE
SET u.IdEntidadPrincipal = ue.IdEntidad
WHERE u.IdEntidadPrincipal IS NULL;

-- Verificar cuántos usuarios tienen IdEntidadPrincipal
SELECT COUNT(*) AS 'Usuarios con IdEntidadPrincipal asignado' 
FROM conf_usuarios 
WHERE IdEntidadPrincipal IS NOT NULL AND Activo = 1;

-- =============================================
-- PASO 3: Actualizar TipoUsuario por defecto
-- =============================================

-- Establecer TipoUsuario = 'Residente' para usuarios que no tienen tipo asignado
UPDATE conf_usuarios 
SET TipoUsuario = 'Residente' 
WHERE TipoUsuario IS NULL OR TipoUsuario = '';

-- Verificar distribución de tipos de usuario
SELECT 
    TipoUsuario,
    COUNT(*) AS Cantidad
FROM conf_usuarios
WHERE Activo = 1
GROUP BY TipoUsuario;

-- =============================================
-- PASO 4: Validaciones de integridad
-- =============================================

-- Validar que todos los usuarios activos tienen al menos una entidad
SELECT 
    u.Id,
    u.Username,
    u.Nombre,
    COUNT(ue.Id) AS NumeroEntidades
FROM conf_usuarios u
LEFT JOIN conf_usuario_entidades ue ON u.Id = ue.IdUsuario AND ue.Activo = 1
WHERE u.Activo = 1
GROUP BY u.Id, u.Username, u.Nombre
HAVING COUNT(ue.Id) = 0;

-- Si el query anterior retorna registros, hay usuarios sin entidades (problema)

-- Validar que todos los usuarios tienen IdEntidadPrincipal
SELECT 
    Id,
    Username,
    Nombre,
    TipoUsuario,
    IdEntidadPrincipal
FROM conf_usuarios
WHERE Activo = 1 
  AND IdEntidadPrincipal IS NULL;

-- Si el query anterior retorna registros, hay usuarios sin entidad principal (problema)

-- Validar que cada usuario tiene exactamente una entidad marcada como principal
SELECT 
    u.Id,
    u.Username,
    u.Nombre,
    COUNT(CASE WHEN ue.EsPrincipal = TRUE THEN 1 END) AS EntidadesPrincipales
FROM conf_usuarios u
INNER JOIN conf_usuario_entidades ue ON u.Id = ue.IdUsuario
WHERE u.Activo = 1 AND ue.Activo = 1
GROUP BY u.Id, u.Username, u.Nombre
HAVING COUNT(CASE WHEN ue.EsPrincipal = TRUE THEN 1 END) != 1;

-- Si el query anterior retorna registros, hay usuarios con 0 o más de 1 entidad principal (problema)

-- =============================================
-- RESUMEN DE MIGRACIÓN
-- =============================================

SELECT 'RESUMEN DE MIGRACIÓN' AS '';

SELECT 
    'Total usuarios activos' AS Metrica,
    COUNT(*) AS Valor
FROM conf_usuarios
WHERE Activo = 1

UNION ALL

SELECT 
    'Usuarios con entidades asignadas' AS Metrica,
    COUNT(DISTINCT IdUsuario) AS Valor
FROM conf_usuario_entidades
WHERE Activo = 1

UNION ALL

SELECT 
    'Total relaciones usuario-entidad' AS Metrica,
    COUNT(*) AS Valor
FROM conf_usuario_entidades
WHERE Activo = 1

UNION ALL

SELECT 
    'Usuarios con IdEntidadPrincipal' AS Metrica,
    COUNT(*) AS Valor
FROM conf_usuarios
WHERE Activo = 1 AND IdEntidadPrincipal IS NOT NULL

UNION ALL

SELECT 
    'Usuarios tipo AdministradorCondominios' AS Metrica,
    COUNT(*) AS Valor
FROM conf_usuarios
WHERE Activo = 1 AND TipoUsuario = 'AdministradorCondominios'

UNION ALL

SELECT 
    'Usuarios tipo Residente' AS Metrica,
    COUNT(*) AS Valor
FROM conf_usuarios
WHERE Activo = 1 AND TipoUsuario = 'Residente';

-- Mensaje de confirmación
SELECT 'Migración de datos completada exitosamente' AS Resultado;
