-- Verificar los prompts configurados por entidad

-- Ver la estructura de conf_ticket_prompts
DESCRIBE conf_ticket_prompts;

-- Ver todos los prompts existentes
SELECT 
    Id,
    IdEntidad,
    Nombre,
    TipoPrompt,
    Activo,
    FechaCreacion
FROM conf_ticket_prompts
ORDER BY IdEntidad, Nombre;

-- Ver específicamente el prompt 'ChatWebSistema'
SELECT 
    Id,
    IdEntidad,
    Nombre,
    TipoPrompt,
    Contenido,
    Activo
FROM conf_ticket_prompts
WHERE Nombre = 'ChatWebSistema';

-- Contar prompts por entidad
SELECT 
    IdEntidad,
    COUNT(*) as TotalPrompts,
    SUM(CASE WHEN Activo = 1 THEN 1 ELSE 0 END) as PromptsActivos
FROM conf_ticket_prompts
GROUP BY IdEntidad
ORDER BY IdEntidad;
