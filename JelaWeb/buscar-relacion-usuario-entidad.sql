-- Buscar cómo está relacionado el usuario con la entidad actualmente

-- Ver la estructura de cat_usuarios
DESCRIBE cat_usuarios;

-- Ver el usuario 5 y sus campos relacionados con entidad
SELECT * FROM cat_usuarios WHERE Id = 5;

-- Buscar todas las tablas que contengan 'usuario' y 'entidad' en su nombre
SHOW TABLES LIKE '%usuario%';
SHOW TABLES LIKE '%entidad%';

-- Ver si hay una columna IdEntidad en cat_usuarios
SHOW COLUMNS FROM cat_usuarios LIKE '%entidad%';
SHOW COLUMNS FROM cat_usuarios LIKE '%Entidad%';
