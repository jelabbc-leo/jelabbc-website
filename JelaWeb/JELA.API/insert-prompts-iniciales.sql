-- ============================================================================
-- SCRIPT: Insertar Prompts Iniciales para Sistema de Tickets
-- ============================================================================
-- Descripción: Inserta los prompts iniciales para canales VAPI, YCloud y Firebase
--              (ChatWeb ya existe en la base de datos)
-- Fecha: 19 de Enero de 2026
-- 
-- ⚠️ IMPORTANTE: Este script es OBLIGATORIO antes de publicar el API
-- El sistema es 100% dinámico y NO tiene prompts hardcodeados como fallback
-- Si los prompts no existen en BD, el sistema lanzará excepciones
-- ============================================================================

USE jela_qa;

-- ============================================================================
-- MANEJO DE DUPLICADOS
-- ============================================================================
-- Si los prompts ya existen, este script fallará por la constraint UNIQUE
-- Para evitar errores, primero verificamos y eliminamos duplicados si existen
-- ============================================================================

-- Verificar si ya existen prompts para estos canales
SELECT 
    NombrePrompt,
    COUNT(*) AS Cantidad
FROM conf_ticket_prompts
WHERE NombrePrompt IN (
    'VAPISistema', 'VAPIUsuario',
    'YCloudSistema', 'YCloudUsuario',
    'FirebaseSistema', 'FirebaseUsuario'
)
GROUP BY NombrePrompt
HAVING COUNT(*) > 0;

-- Si el resultado anterior muestra prompts existentes, puede:
-- OPCIÓN 1: Eliminar los existentes (⚠️ CUIDADO: perderá personalizaciones)
-- DELETE FROM conf_ticket_prompts WHERE NombrePrompt IN ('VAPISistema', 'VAPIUsuario', 'YCloudSistema', 'YCloudUsuario', 'FirebaseSistema', 'FirebaseUsuario');

-- OPCIÓN 2: Actualizar los existentes en lugar de insertar (recomendado)
-- Ver sección de UPDATE al final del script

-- ============================================================================
-- NOTA: Los prompts de ChatWeb ya existen en la base de datos
-- Solo se insertarán prompts para VAPI, YCloud y Firebase
-- ============================================================================

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
    IdUsuarioCreacion
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
    1
);

-- Prompt Usuario VAPI
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion
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
    1
);

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
    IdUsuarioCreacion
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
    1
);

-- Prompt Usuario YCloud
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion
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
    1
);

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
    IdUsuarioCreacion
) VALUES (
    1,
    'FirebaseSistema',
    'Prompt de sistema para mensajes de app móvil',
    'Eres un asistente de atención al cliente por app móvil.
Analiza el mensaje y proporciona una respuesta útil y profesional.',
    1,
    1
);

-- Prompt Usuario Firebase
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion
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
    1
);

-- ============================================================================
-- VERIFICACIÓN
-- ============================================================================

-- Verificar que todos los prompts se insertaron correctamente
SELECT 
    Id,
    NombrePrompt,
    Descripcion,
    Activo,
    FechaCreacion
FROM conf_ticket_prompts
WHERE NombrePrompt IN (
    'VAPISistema', 'VAPIUsuario',
    'YCloudSistema', 'YCloudUsuario',
    'ChatWebSistema', 'ChatWebUsuario',
    'FirebaseSistema', 'FirebaseUsuario'
)
ORDER BY NombrePrompt;

-- ============================================================================
-- ALTERNATIVA: ACTUALIZAR PROMPTS EXISTENTES (Si ya existen)
-- ============================================================================
-- Si los prompts ya existen y desea actualizarlos, use estos UPDATE:

/*
-- Actualizar Prompt Sistema VAPI
UPDATE conf_ticket_prompts
SET ContenidoPrompt = 'Eres un asistente de atención al cliente. 
Analiza la transcripción de la llamada y:
1. Identifica el problema o solicitud principal
2. Determina si es una acción requerida o una llamada cortada
3. Sugiere una categoría apropiada
4. Proporciona una respuesta profesional y empática',
    Descripcion = 'Prompt de sistema para análisis de llamadas telefónicas VAPI',
    FechaUltimaActualizacion = NOW()
WHERE NombrePrompt = 'VAPISistema' AND IdEntidad = 1;

-- Actualizar Prompt Usuario VAPI
UPDATE conf_ticket_prompts
SET ContenidoPrompt = 'Transcripción de llamada telefónica:
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
    Descripcion = 'Prompt de usuario para análisis de llamadas telefónicas VAPI',
    FechaUltimaActualizacion = NOW()
WHERE NombrePrompt = 'VAPIUsuario' AND IdEntidad = 1;

-- Actualizar Prompt Sistema YCloud
UPDATE conf_ticket_prompts
SET ContenidoPrompt = 'Eres un asistente de atención al cliente por WhatsApp.
Analiza el mensaje y:
1. Identifica el problema o solicitud
2. Determina la categoría apropiada
3. Evalúa la prioridad
4. Proporciona una respuesta profesional, breve y empática en español',
    Descripcion = 'Prompt de sistema para mensajes de WhatsApp Business',
    FechaUltimaActualizacion = NOW()
WHERE NombrePrompt = 'YCloudSistema' AND IdEntidad = 1;

-- Actualizar Prompt Usuario YCloud
UPDATE conf_ticket_prompts
SET ContenidoPrompt = 'Mensaje de WhatsApp:
De: {From}
Mensaje: {Text}

Por favor, analiza este mensaje y proporciona:
1. Resumen del problema
2. Categoría sugerida
3. Prioridad (Alta/Media/Baja)
4. Respuesta para el cliente (máximo 160 caracteres)',
    Descripcion = 'Prompt de usuario para mensajes de WhatsApp Business',
    FechaUltimaActualizacion = NOW()
WHERE NombrePrompt = 'YCloudUsuario' AND IdEntidad = 1;

-- Actualizar Prompt Sistema Firebase
UPDATE conf_ticket_prompts
SET ContenidoPrompt = 'Eres un asistente de atención al cliente por app móvil.
Analiza el mensaje y proporciona una respuesta útil y profesional.',
    Descripcion = 'Prompt de sistema para mensajes de app móvil',
    FechaUltimaActualizacion = NOW()
WHERE NombrePrompt = 'FirebaseSistema' AND IdEntidad = 1;

-- Actualizar Prompt Usuario Firebase
UPDATE conf_ticket_prompts
SET ContenidoPrompt = 'Mensaje de app móvil:
Usuario: {UserId}
Mensaje: {Message}

Por favor, proporciona:
1. Respuesta para el cliente
2. Categoría sugerida
3. Prioridad (Alta/Media/Baja)',
    Descripcion = 'Prompt de usuario para mensajes de app móvil',
    FechaUltimaActualizacion = NOW()
WHERE NombrePrompt = 'FirebaseUsuario' AND IdEntidad = 1;
*/

-- ============================================================================
-- NOTAS IMPORTANTES
-- ============================================================================
-- 1. Los prompts de ChatWeb ya existían en la base de datos, por lo que
--    este script solo inserta los prompts para VAPI, YCloud y Firebase
-- 2. Los prompts usan placeholders entre llaves {} que serán reemplazados
--    en tiempo de ejecución por los valores reales
-- 3. Cada canal tiene dos prompts: Sistema (instrucciones generales) y 
--    Usuario (contexto específico del mensaje)
-- 4. Los prompts pueden ser editados posteriormente desde la interfaz web
--    en Views/Catalogos/Tickets/TicketsPrompts.aspx
-- 
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
