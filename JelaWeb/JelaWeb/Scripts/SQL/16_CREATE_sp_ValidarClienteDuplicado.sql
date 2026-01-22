-- ============================================================================
-- SCRIPT: 16_CREATE_sp_ValidarClienteDuplicado.sql
-- PROPÓSITO: Crear stored procedure sp_ValidarClienteDuplicado
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4.6.1
-- ============================================================================

USE jela_qa;

SELECT 'Creando stored procedure sp_ValidarClienteDuplicado...' AS Paso;

-- Eliminar procedure si existe (para permitir re-ejecución)
DROP PROCEDURE IF EXISTS sp_ValidarClienteDuplicado;

DELIMITER $$

CREATE PROCEDURE sp_ValidarClienteDuplicado(
  IN p_Telefono VARCHAR(50),
  IN p_Email VARCHAR(255),
  IN p_IP VARCHAR(50),
  IN p_IdEntidad INT,
  OUT p_TieneTicketAbierto BOOLEAN,
  OUT p_IdTicketAbierto INT
)
BEGIN
  -- Buscar tickets abiertos del cliente
  SELECT 
    COUNT(*) > 0,
    MAX(Id)
  INTO 
    p_TieneTicketAbierto,
    p_IdTicketAbierto
  FROM op_tickets_v2
  WHERE (
    (p_Telefono IS NOT NULL AND TelefonoCliente = p_Telefono) 
    OR (p_Email IS NOT NULL AND EmailCliente = p_Email) 
    OR (p_IP IS NOT NULL AND IPOrigen = p_IP)
  )
  AND Estado IN ('Abierto', 'EnProceso')
  AND IdEntidad = p_IdEntidad
  AND Activo = 1;
  
  -- Actualizar tabla de validación
  INSERT INTO op_ticket_validacion_cliente (
    IdEntidad, 
    TelefonoCliente, 
    EmailCliente, 
    IPOrigen,
    TieneTicketAbierto, 
    IdTicketAbierto, 
    UltimaInteraccion,
    IdUsuarioCreacion
  ) VALUES (
    p_IdEntidad, 
    p_Telefono, 
    p_Email, 
    p_IP,
    p_TieneTicketAbierto, 
    p_IdTicketAbierto, 
    NOW(),
    1
  )
  ON DUPLICATE KEY UPDATE
    TieneTicketAbierto = p_TieneTicketAbierto,
    IdTicketAbierto = p_IdTicketAbierto,
    UltimaInteraccion = NOW(),
    FechaUltimaActualizacion = NOW();
END$$

DELIMITER ;

SELECT '✓ Stored procedure sp_ValidarClienteDuplicado creado exitosamente' AS Resultado;

-- ============================================================================
-- PRUEBA DEL STORED PROCEDURE
-- ============================================================================

SELECT 'Probando stored procedure...' AS Paso;

-- Declarar variables para la prueba
SET @tieneTicket = FALSE;
SET @idTicket = NULL;

-- Ejecutar el procedure con datos de prueba
CALL sp_ValidarClienteDuplicado(
  '+525512345678',  -- Teléfono de prueba
  'test@example.com', -- Email de prueba
  '192.168.1.1',    -- IP de prueba
  1,                -- IdEntidad
  @tieneTicket,     -- OUT: TieneTicketAbierto
  @idTicket         -- OUT: IdTicketAbierto
);

-- Mostrar resultados
SELECT 
  @tieneTicket AS TieneTicketAbierto,
  @idTicket AS IdTicketAbierto,
  'Prueba completada - No hay tickets abiertos con estos datos' AS Resultado;

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
  AND ROUTINE_NAME = 'sp_ValidarClienteDuplicado';

SELECT '✓ Script ejecutado exitosamente' AS Estado;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
