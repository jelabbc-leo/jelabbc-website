-- ============================================
-- Script: Copiar prompts de entidad 1 a otras entidades
-- Propósito: Permitir que todas las entidades tengan los prompts necesarios
-- ============================================

-- Paso 1: Ver los prompts existentes
SELECT 
    Id,
    IdEntidad,
    NombrePrompt,
    Descripcion,
    Activo,
    FechaCreacion
FROM conf_ticket_prompts
ORDER BY IdEntidad, NombrePrompt;

-- Paso 2: Ver específicamente el prompt 'ChatWebSistema'
SELECT 
    Id,
    IdEntidad,
    NombrePrompt,
    Descripcion,
    LEFT(ContenidoPrompt, 100) AS ContenidoInicio,
    Activo
FROM conf_ticket_prompts
WHERE NombrePrompt = 'ChatWebSistema';

-- Paso 3: Contar prompts por entidad
SELECT 
    IdEntidad,
    COUNT(*) as TotalPrompts,
    SUM(CASE WHEN Activo = 1 THEN 1 ELSE 0 END) as PromptsActivos
FROM conf_ticket_prompts
GROUP BY IdEntidad
ORDER BY IdEntidad;

-- Paso 4: Copiar todos los prompts de la entidad 1 a la entidad 2
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion
)
SELECT 
    2 AS IdEntidad,  -- Nueva entidad
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion
FROM conf_ticket_prompts
WHERE IdEntidad = 1
AND NOT EXISTS (
    SELECT 1 
    FROM conf_ticket_prompts cp2 
    WHERE cp2.IdEntidad = 2 
    AND cp2.NombrePrompt = conf_ticket_prompts.NombrePrompt
);

-- Paso 5: Copiar todos los prompts de la entidad 1 a la entidad 3
INSERT INTO conf_ticket_prompts (
    IdEntidad,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion
)
SELECT 
    3 AS IdEntidad,  -- Nueva entidad
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion
FROM conf_ticket_prompts
WHERE IdEntidad = 1
AND NOT EXISTS (
    SELECT 1 
    FROM conf_ticket_prompts cp2 
    WHERE cp2.IdEntidad = 3 
    AND cp2.NombrePrompt = conf_ticket_prompts.NombrePrompt
);

-- Paso 6: Verificar que los prompts se copiaron correctamente
SELECT 
    IdEntidad,
    COUNT(*) as TotalPrompts,
    SUM(CASE WHEN Activo = 1 THEN 1 ELSE 0 END) as PromptsActivos,
    GROUP_CONCAT(NombrePrompt ORDER BY NombrePrompt SEPARATOR ', ') AS Prompts
FROM conf_ticket_prompts
GROUP BY IdEntidad
ORDER BY IdEntidad;

-- Paso 7: Verificar específicamente el prompt 'ChatWebSistema' en todas las entidades
SELECT 
    Id,
    IdEntidad,
    NombrePrompt,
    Activo,
    FechaCreacion
FROM conf_ticket_prompts
WHERE NombrePrompt = 'ChatWebSistema'
ORDER BY IdEntidad;
