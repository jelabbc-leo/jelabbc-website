-- ============================================================================
-- SCRIPT: 18_CREATE_sp_CalcularMetricasDiarias.sql
-- PROPÓSITO: Crear stored procedure sp_CalcularMetricasDiarias
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4.6.3
-- ============================================================================

USE jela_qa;

SELECT 'Creando stored procedure sp_CalcularMetricasDiarias...' AS Paso;

-- Eliminar procedure si existe (para permitir re-ejecución)
DROP PROCEDURE IF EXISTS sp_CalcularMetricasDiarias;

DELIMITER $$

CREATE PROCEDURE sp_CalcularMetricasDiarias(
  IN p_Fecha DATE,
  IN p_IdEntidad INT
)
BEGIN
  -- Calcular métricas del día
  INSERT INTO op_ticket_metricas (
    IdEntidad, 
    FechaMetrica, 
    TipoAgregacion,
    TotalTicketsCreados, 
    TotalTicketsResueltos, 
    TotalTicketsResueltosIA,
    PorcentajeResolucionIA, 
    TiempoPromedioResolucionMinutos,
    CSATPromedio, 
    IdUsuarioCreacion
  )
  SELECT
    p_IdEntidad,
    p_Fecha,
    'Diaria',
    COUNT(*) AS TotalTicketsCreados,
    SUM(CASE WHEN Estado = 'Resuelto' THEN 1 ELSE 0 END) AS TotalTicketsResueltos,
    SUM(CASE WHEN ResueltoporIA = 1 THEN 1 ELSE 0 END) AS TotalTicketsResueltosIA,
    CASE 
      WHEN COUNT(*) > 0 THEN (SUM(CASE WHEN ResueltoporIA = 1 THEN 1 ELSE 0 END) * 100.0 / COUNT(*))
      ELSE 0
    END AS PorcentajeResolucionIA,
    AVG(CASE 
      WHEN TiempoResolucionMinutos IS NOT NULL THEN TiempoResolucionMinutos 
      ELSE NULL 
    END) AS TiempoPromedioResolucionMinutos,
    AVG(CASE 
      WHEN CSATScore IS NOT NULL THEN CSATScore 
      ELSE NULL 
    END) AS CSATPromedio,
    1
  FROM op_tickets_v2
  WHERE DATE(FechaCreacion) = p_Fecha
  AND IdEntidad = p_IdEntidad
  AND Activo = 1
  ON DUPLICATE KEY UPDATE
    TotalTicketsCreados = VALUES(TotalTicketsCreados),
    TotalTicketsResueltos = VALUES(TotalTicketsResueltos),
    TotalTicketsResueltosIA = VALUES(TotalTicketsResueltosIA),
    PorcentajeResolucionIA = VALUES(PorcentajeResolucionIA),
    TiempoPromedioResolucionMinutos = VALUES(TiempoPromedioResolucionMinutos),
    CSATPromedio = VALUES(CSATPromedio),
    FechaUltimaActualizacion = NOW();
    
  -- Retornar las métricas calculadas
  SELECT 
    FechaMetrica,
    TotalTicketsCreados,
    TotalTicketsResueltos,
    TotalTicketsResueltosIA,
    PorcentajeResolucionIA,
    TiempoPromedioResolucionMinutos,
    CSATPromedio
  FROM op_ticket_metricas
  WHERE FechaMetrica = p_Fecha
    AND IdEntidad = p_IdEntidad
    AND TipoAgregacion = 'Diaria';
END$$

DELIMITER ;

SELECT '✓ Stored procedure sp_CalcularMetricasDiarias creado exitosamente' AS Resultado;

-- ============================================================================
-- PRUEBA DEL STORED PROCEDURE
-- ============================================================================

SELECT 'Probando stored procedure...' AS Paso;

-- Ejecutar el procedure con la fecha de hoy
CALL sp_CalcularMetricasDiarias(CURDATE(), 1);

SELECT 'Prueba completada - Métricas calculadas para hoy' AS Resultado;

-- Limpiar las métricas de prueba si no hay tickets hoy
DELETE FROM op_ticket_metricas 
WHERE FechaMetrica = CURDATE() 
  AND IdEntidad = 1 
  AND TotalTicketsCreados = 0;

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================

SELECT 'Verificando stored procedure creado...' AS Paso;

SELECT 
    ROUTINE_NAME AS ProcedureName,
    ROUTINE_TYPE AS Tipo,
    CREATED AS FechaCreacion
FROM INFORMATION_SCHEMA.ROUTINES
WHERE ROUTINE_SCHEMA = 'jela_qa'
  AND ROUTINE_NAME = 'sp_CalcularMetricasDiarias';

SELECT '✓ Script ejecutado exitosamente' AS Estado;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
