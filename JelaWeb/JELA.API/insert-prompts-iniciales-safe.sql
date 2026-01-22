-- ============================================================================
-- SCRIPT SEGURO: Insertar/Actualizar Prompts Iniciales (Sin Errores)
-- ============================================================================
-- Descripción: Inserta o actualiza prompts para VAPI, YCloud y Firebase
--              Usa INSERT ... ON DUPLICATE KEY UPDATE para evitar errores
-- Fecha: 19 de Enero de 2026
-- 
-- ⚠️ VENTAJAS DE ESTE SCRIPT:
-- ✅ No falla si los prompts ya existen
-- ✅ Actualiza prompts existentes automáticamente
-- ✅ Seguro para ejecutar múltiples veces
-- ✅ Ideal para CI/CD y automatización
-- ============================================================================

USE jela_qa;

-- ============================================================================
-- 1. PROMPTS PARA VAPI (Llamadas Telefónicas)
-- ============================================================================

-- Prompt Sistema VAPI
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    1,
    'VAPISistema',
    'Prompt de sistema para análisis de llamadas telefónicas VAPI',
    'Eres un asistente de atención al cliente. 
Analiza la transcripción de la llamada y:
1. Identifica el problema o solicitud principal
2. Determina si es una acción requerida o una llamada cortada
3. Sugiere una categoría apropiada
4. Proporciona una respuesta profesional y empática',
    1,
    1,
    NOW()
)
ON DUPLICATE KEY UPDATE
    ContenidoPrompt = VALUES(ContenidoPrompt),
    Descripcion = VALUES(Descripcion),
    FechaUltimaActualizacion = NOW();

-- Prompt Usuario VAPI
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    1,
    'VAPIUsuario',
    'Prompt de usuario para análisis de llamadas telefónicas VAPI',
    'Transcripción de llamada telefónica:
Teléfono: {PhoneNumber}
Duración: {DurationSeconds} segundos
Estado: {Status}
{DisconnectReason}

Transcripción:
{Transcription}

Por favor, analiza esta llamada y proporciona:
1. Resumen del problema
2. Categoría sugerida
3. Prioridad (Alta/Media/Baja)
4. Tipo de ticket (Accion/LlamadaCortada)',
    1,
    1,
    NOW()
)
ON DUPLICATE KEY UPDATE
    ContenidoPrompt = VALUES(ContenidoPrompt),
    Descripcion = VALUES(Descripcion),
    FechaUltimaActualizacion = NOW();

-- ============================================================================
-- 2. PROMPTS PARA YCLOUD (WhatsApp Business)
-- ============================================================================

-- Prompt Sistema YCloud
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    1,
    'YCloudSistema',
    'Prompt de sistema para mensajes de WhatsApp Business',
    'Eres un asistente de atención al cliente por WhatsApp.
Analiza el mensaje y:
1. Identifica el problema o solicitud
2. Determina la categoría apropiada
3. Evalúa la prioridad
4. Proporciona una respuesta profesional, breve y empática en español',
    1,
    1,
    NOW()
)
ON DUPLICATE KEY UPDATE
    ContenidoPrompt = VALUES(ContenidoPrompt),
    Descripcion = VALUES(Descripcion),
    FechaUltimaActualizacion = NOW();

-- Prompt Usuario YCloud
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    1,
    'YCloudUsuario',
    'Prompt de usuario para mensajes de WhatsApp Business',
    'Mensaje de WhatsApp:
De: {From}
Mensaje: {Text}

Por favor, analiza este mensaje y proporciona:
1. Resumen del problema
2. Categoría sugerida
3. Prioridad (Alta/Media/Baja)
4. Respuesta para el cliente (máximo 160 caracteres)',
    1,
    1,
    NOW()
)
ON DUPLICATE KEY UPDATE
    ContenidoPrompt = VALUES(ContenidoPrompt),
    Descripcion = VALUES(Descripcion),
    FechaUltimaActualizacion = NOW();

-- ============================================================================
-- 3. PROMPTS PARA FIREBASE (App Móvil)
-- ============================================================================

-- Prompt Sistema Firebase
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    1,
    'FirebaseSistema',
    'Prompt de sistema para mensajes de app móvil',
    'Eres un asistente de atención al cliente por app móvil.
Analiza el mensaje y proporciona una respuesta útil y profesional.',
    1,
    1,
    NOW()
)
ON DUPLICATE KEY UPDATE
    ContenidoPrompt = VALUES(ContenidoPrompt),
    Descripcion = VALUES(Descripcion),
    FechaUltimaActualizacion = NOW();

-- Prompt Usuario Firebase
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion,
    FechaCreacion
) VALUES (
    1,
    'FirebaseUsuario',
    'Prompt de usuario para mensajes de app móvil',
    'Mensaje de app móvil:
Usuario: {UserId}
Mensaje: {Message}

Por favor, proporciona:
1. Respuesta para el cliente
2. Categoría sugerida
3. Prioridad (Alta/Media/Baja)',
    1,
    1,
    NOW()
)
ON DUPLICATE KEY UPDATE
    ContenidoPrompt = VALUES(ContenidoPrompt),
    Descripcion = VALUES(Descripcion),
    FechaUltimaActualizacion = NOW();

-- ============================================================================
-- VERIFICACIÓN FINAL
-- ============================================================================

-- Verificar que todos los prompts existen y están activos
SELECT 
    Id,
    NombrePrompt,
    Descripcion,
    Activo,
    FechaCreacion,
    FechaUltimaActualizacion
FROM conf_ticket_prompts
WHERE NombrePrompt IN (
    'VAPISistema', 'VAPIUsuario',
    'YCloudSistema', 'YCloudUsuario',
    'ChatWebSistema', 'ChatWebUsuario',
    'FirebaseSistema', 'FirebaseUsuario'
)
AND IdEntidad = 1
ORDER BY NombrePrompt;

-- Contar prompts por canal
SELECT 
    CASE 
        WHEN NombrePrompt LIKE 'VAPI%' THEN 'VAPI'
        WHEN NombrePrompt LIKE 'YCloud%' THEN 'YCloud'
        WHEN NombrePrompt LIKE 'ChatWeb%' THEN 'ChatWeb'
        WHEN NombrePrompt LIKE 'Firebase%' THEN 'Firebase'
        ELSE 'Otro'
    END AS Canal,
    COUNT(*) AS TotalPrompts
FROM conf_ticket_prompts
WHERE NombrePrompt IN (
    'VAPISistema', 'VAPIUsuario',
    'YCloudSistema', 'YCloudUsuario',
    'ChatWebSistema', 'ChatWebUsuario',
    'FirebaseSistema', 'FirebaseUsuario'
)
AND IdEntidad = 1
AND Activo = 1
GROUP BY Canal
ORDER BY Canal;

-- ============================================================================
-- RESULTADO ESPERADO
-- ============================================================================
-- Debe mostrar 8 prompts activos:
-- - VAPI: 2 prompts (Sistema, Usuario)
-- - YCloud: 2 prompts (Sistema, Usuario)
-- - ChatWeb: 2 prompts (Sistema, Usuario)
-- - Firebase: 2 prompts (Sistema, Usuario)
-- ============================================================================

-- ============================================================================
-- NOTAS IMPORTANTES
-- ============================================================================
-- ⚠️ CRÍTICO: SISTEMA 100% DINÁMICO - SIN FALLBACKS
-- ============================================================================
-- El sistema NO tiene prompts hardcodeados como fallback.
-- Si un prompt no existe en la BD, el sistema lanzará una excepción:
--   InvalidOperationException: "Prompt 'XXX' no encontrado en conf_ticket_prompts"
-- 
-- Esto es INTENCIONAL para:
-- ✅ Forzar configuración correcta de la BD
-- ✅ Detectar errores en desarrollo, no en producción
-- ✅ Mantener única fuente de verdad (BD)
-- ✅ Facilitar mantenimiento sin redespliegues
-- 
-- ANTES DE PUBLICAR: Ejecute este script en la BD de producción
-- ============================================================================
