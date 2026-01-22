-- ============================================================================
-- SCRIPT MAESTRO: 00_MAESTRO_crear_tablas_tickets.sql
-- PROPÓSITO: Ejecutar todos los scripts de creación de tablas en orden
-- FECHA: 18 de Enero de 2026
-- MÓDULO: Tickets Colaborativos con IA
-- REFERENCIA: .kiro/specs/tickets-colaborativos/design.md § 4
-- ============================================================================

USE jela_qa;

SELECT '============================================================================' AS '';
SELECT 'INICIO DE MIGRACIÓN - MÓDULO DE TICKETS COLABORATIVOS CON IA' AS '';
SELECT '============================================================================' AS '';
SELECT NOW() AS FechaInicio;

-- ============================================================================
-- PASO 1: ALTERAR TABLA EXISTENTE op_tickets_v2
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 1/18: Alterando tabla op_tickets_v2 (13 campos nuevos)' AS '';
SELECT '============================================================================' AS '';

SOURCE 01_ALTER_op_tickets_v2_agregar_campos_nuevos.sql;

-- ============================================================================
-- PASO 2: CREAR TABLA op_ticket_logs_sistema
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 2/18: Creando tabla op_ticket_logs_sistema' AS '';
SELECT '============================================================================' AS '';

SOURCE 02_CREATE_op_ticket_logs_sistema.sql;

-- ============================================================================
-- PASO 3: CREAR TABLA op_ticket_logs_interacciones
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 3/18: Creando tabla op_ticket_logs_interacciones' AS '';
SELECT '============================================================================' AS '';

SOURCE 03_CREATE_op_ticket_logs_interacciones.sql;

-- ============================================================================
-- PASO 4: CREAR TABLA op_ticket_logprompts
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 4/18: Creando tabla op_ticket_logprompts' AS '';
SELECT '============================================================================' AS '';

SOURCE 04_CREATE_op_ticket_logprompts.sql;

-- ============================================================================
-- PASO 5: CREAR TABLA op_ticket_metricas
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 5/18: Creando tabla op_ticket_metricas' AS '';
SELECT '============================================================================' AS '';

SOURCE 05_CREATE_op_ticket_metricas.sql;

-- ============================================================================
-- PASO 6: CREAR TABLA op_ticket_validacion_cliente
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 6/18: Creando tabla op_ticket_validacion_cliente' AS '';
SELECT '============================================================================' AS '';

SOURCE 06_CREATE_op_ticket_validacion_cliente.sql;

-- ============================================================================
-- PASO 7: CREAR TABLA op_ticket_notificaciones_whatsapp
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 7/18: Creando tabla op_ticket_notificaciones_whatsapp' AS '';
SELECT '============================================================================' AS '';

SOURCE 07_CREATE_op_ticket_notificaciones_whatsapp.sql;

-- ============================================================================
-- PASO 8: CREAR TABLA op_ticket_robot_monitoreo
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 8/18: Creando tabla op_ticket_robot_monitoreo' AS '';
SELECT '============================================================================' AS '';

SOURCE 08_CREATE_op_ticket_robot_monitoreo.sql;

-- ============================================================================
-- PASO 9: CREAR TABLA op_ticket_prompt_ajustes_log
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 9/18: Creando tabla op_ticket_prompt_ajustes_log' AS '';
SELECT '============================================================================' AS '';

SOURCE 09_CREATE_op_ticket_prompt_ajustes_log.sql;

-- ============================================================================
-- PASO 10: CREAR TABLA op_telegram_clientes
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 10/18: Creando tabla op_telegram_clientes' AS '';
SELECT '============================================================================' AS '';

SOURCE 10_CREATE_op_telegram_clientes.sql;

-- ============================================================================
-- PASO 11: CREAR TABLA op_telegram_whitelist
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 11/18: Creando tabla op_telegram_whitelist' AS '';
SELECT '============================================================================' AS '';

SOURCE 11_CREATE_op_telegram_whitelist.sql;

-- ============================================================================
-- PASO 12: CREAR TABLA op_telegram_blacklist
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 12/18: Creando tabla op_telegram_blacklist' AS '';
SELECT '============================================================================' AS '';

SOURCE 12_CREATE_op_telegram_blacklist.sql;

-- ============================================================================
-- PASO 13: CREAR TABLA op_telegram_logs_validacion
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 13/18: Creando tabla op_telegram_logs_validacion' AS '';
SELECT '============================================================================' AS '';

SOURCE 13_CREATE_op_telegram_logs_validacion.sql;

-- ============================================================================
-- PASO 14: CREAR TABLA op_telegram_notifications_queue
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 14/18: Creando tabla op_telegram_notifications_queue' AS '';
SELECT '============================================================================' AS '';

SOURCE 14_CREATE_op_telegram_notifications_queue.sql;

-- ============================================================================
-- PASO 15: CREAR TRIGGER Y CAMPOS ADICIONALES PARA TELEGRAM
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 15/18: Creando trigger y campos adicionales para Telegram' AS '';
SELECT '============================================================================' AS '';

SOURCE 15_CREATE_trigger_y_campos_telegram.sql;

-- ============================================================================
-- PASO 16: CREAR STORED PROCEDURE sp_ValidarClienteDuplicado
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 16/18: Creando stored procedure sp_ValidarClienteDuplicado' AS '';
SELECT '============================================================================' AS '';

SOURCE 16_CREATE_sp_ValidarClienteDuplicado.sql;

-- ============================================================================
-- PASO 17: CREAR STORED PROCEDURE sp_EncolarNotificacionWhatsApp
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 17/18: Creando stored procedure sp_EncolarNotificacionWhatsApp' AS '';
SELECT '============================================================================' AS '';

SOURCE 17_CREATE_sp_EncolarNotificacionWhatsApp.sql;

-- ============================================================================
-- PASO 18: CREAR STORED PROCEDURE sp_CalcularMetricasDiarias
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'PASO 18/18: Creando stored procedure sp_CalcularMetricasDiarias' AS '';
SELECT '============================================================================' AS '';

SOURCE 18_CREATE_sp_CalcularMetricasDiarias.sql;

-- ============================================================================
-- RESUMEN FINAL
-- ============================================================================

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT '✓ MIGRACIÓN COMPLETADA EXITOSAMENTE' AS '';
SELECT '============================================================================' AS '';
SELECT NOW() AS FechaFin;

SELECT '' AS '';
SELECT 'RESUMEN DE CAMBIOS:' AS '';
SELECT '- 1 tabla alterada: op_tickets_v2 (+13 campos iniciales + 4 campos Telegram)' AS '';
SELECT '- 13 tablas nuevas creadas (8 generales + 5 Telegram)' AS '';
SELECT '- 8 índices agregados a op_tickets_v2' AS '';
SELECT '- 1 trigger creado: trg_NotificarCambioEstadoTelegram' AS '';
SELECT '- 3 stored procedures creados' AS '';
SELECT '- Múltiples índices en tablas nuevas' AS '';

SELECT '' AS '';
SELECT 'TABLAS CREADAS:' AS '';
SELECT '1. op_ticket_logs_sistema' AS '';
SELECT '2. op_ticket_logs_interacciones' AS '';
SELECT '3. op_ticket_logprompts' AS '';
SELECT '4. op_ticket_metricas' AS '';
SELECT '5. op_ticket_validacion_cliente' AS '';
SELECT '6. op_ticket_notificaciones_whatsapp' AS '';
SELECT '7. op_ticket_robot_monitoreo' AS '';
SELECT '8. op_ticket_prompt_ajustes_log' AS '';
SELECT '9. op_telegram_clientes' AS '';
SELECT '10. op_telegram_whitelist' AS '';
SELECT '11. op_telegram_blacklist' AS '';
SELECT '12. op_telegram_logs_validacion' AS '';
SELECT '13. op_telegram_notifications_queue' AS '';

SELECT '' AS '';
SELECT 'STORED PROCEDURES CREADOS:' AS '';
SELECT '1. sp_ValidarClienteDuplicado' AS '';
SELECT '2. sp_EncolarNotificacionWhatsApp' AS '';
SELECT '3. sp_CalcularMetricasDiarias' AS '';

SELECT '' AS '';
SELECT '============================================================================' AS '';
SELECT 'BASE DE DATOS COMPLETADA - Continuar con sección 2: API Backend' AS '';
SELECT '============================================================================' AS '';

-- ============================================================================
-- FIN DEL SCRIPT MAESTRO
-- ============================================================================
