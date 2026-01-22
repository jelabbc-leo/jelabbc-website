-- ============================================================================
-- SCRIPT: 17_CREATE_sp_EncolarNotificacionWhatsApp.sql
-- PROPÓSITO: Crear stored procedure sp_EncolarNotificacionWhatsApp
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4.6.2
-- ============================================================================

USE jela_qa;

SELECT 'Creando stored procedure sp_EncolarNotificacionWhatsApp...' AS Paso;

-- Eliminar procedure si existe (para permitir re-ejecución)
DROP PROCEDURE IF EXISTS sp_EncolarNotificacionWhatsApp;

DELIMITER $$

CREATE PROCEDURE sp_EncolarNotificacionWhatsApp(
  IN p_IdTicket INT,
  IN p_NumeroWhatsApp VARCHAR(50),
  IN p_TipoNotificacion VARCHAR(100),
  IN p_MensajeTexto TEXT,
  IN p_IdEntidad INT
)
BEGIN
  -- Insertar notificación en la cola
  INSERT INTO op_ticket_notificaciones_whatsapp (
    IdEntidad, 
    IdTicket, 
    NumeroWhatsApp, 
    TipoNotificacion,
    MensajeTexto, 
    Estado, 
    IntentosEnvio, 
    ProximoIntento,
    IdUsuarioCreacion
  ) VALUES (
    p_IdEntidad, 
    p_IdTicket, 
    p_NumeroWhatsApp, 
    p_TipoNotificacion,
    p_MensajeTexto, 
    'Pendiente', 
    0, 
    NOW(),
    1
  );
  
  -- Retornar el ID de la notificación creada
  SELECT LAST_INSERT_ID() AS IdNotificacion;
END$$

DELIMITER ;

SELECT '✓ Stored procedure sp_EncolarNotificacionWhatsApp creado exitosamente' AS Resultado;

-- ============================================================================
-- PRUEBA DEL STORED PROCEDURE (OPCIONAL)
-- ============================================================================

-- Nota: Esta prueba requiere que exista al menos un ticket en op_tickets_v2
-- Descomente las siguientes líneas para ejecutar la prueba

/*
SELECT 'Probando stored procedure...' AS Paso;

-- Verificar si hay tickets disponibles para la prueba
SET @ticketId = (SELECT Id FROM op_tickets_v2 LIMIT 1);

-- Ejecutar el procedure con datos de prueba (solo si hay tickets)
-- Reemplazar NULL con @ticketId si desea probar con un ticket real
CALL sp_EncolarNotificacionWhatsApp(
  NULL,                   -- IdTicket (usar @ticketId si hay tickets disponibles)
  '+525512345678',        -- Número WhatsApp de prueba
  'TicketCreado',         -- Tipo de notificación
  'Tu ticket ha sido creado exitosamente', -- Mensaje
  1                       -- IdEntidad
);

SELECT 'Prueba completada - Notificación encolada' AS Resultado;

-- Limpiar la notificación de prueba
DELETE FROM op_ticket_notificaciones_whatsapp 
WHERE NumeroWhatsApp = '+525512345678' 
  AND MensajeTexto = 'Tu ticket ha sido creado exitosamente';
  
SELECT 'Notificación de prueba eliminada' AS Limpieza;
*/

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
  AND ROUTINE_NAME = 'sp_EncolarNotificacionWhatsApp';

SELECT '✓ Script ejecutado exitosamente' AS Estado;

-- ============================================================================
-- FIN DEL SCRIPT
-- ============================================================================
