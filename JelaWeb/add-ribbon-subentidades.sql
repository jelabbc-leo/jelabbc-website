-- Agregar opción Sub Entidades al ribbon
-- Fecha: 2026-01-21
-- Tabla: conf_opciones

SET @IdEntidad := 1;
SET @NombreOpcion := 'Sub Entidades';
SET @UrlOpcion := '/Views/Catalogos/SubEntidades.aspx';
SET @RibbonTab := 'Catálogos';
SET @RibbonGroup := 'Maestros';
SET @OrdenTab := 10;
SET @OrdenGrupo := 10;
SET @OrdenOpcion := 10;

INSERT INTO conf_opciones (
    IdEntidad,
    Nombre,
    Url,
    Icono,
    Plataforma,
    RibbonTab,
    RibbonGroup,
    Activo,
    OrdenTab,
    OrdenGrupo,
    OrdenOpcion,
    Descripcion,
    CreadoPor
)
SELECT
    @IdEntidad,
    @NombreOpcion,
    @UrlOpcion,
    'fa fa-sitemap',
    'web',
    @RibbonTab,
    @RibbonGroup,
    1,
    @OrdenTab,
    @OrdenGrupo,
    @OrdenOpcion,
    'Catálogo de sub entidades (secciones/torres)',
    1
WHERE NOT EXISTS (
    SELECT 1
    FROM conf_opciones
    WHERE IdEntidad = @IdEntidad
      AND Url = @UrlOpcion
      AND Nombre = @NombreOpcion
);

-- Asignar la opción al rol "Administrador Total" si existe
SET @RolId := (
    SELECT Id
    FROM conf_roles
    WHERE Nombre = 'Administrador Total'
    LIMIT 1
);

SET @OpcionId := (
    SELECT Id
    FROM conf_opciones
    WHERE IdEntidad = @IdEntidad
      AND Url = @UrlOpcion
      AND Nombre = @NombreOpcion
    LIMIT 1
);

INSERT INTO conf_rolopciones (RolId, OpcionId, Activo)
SELECT @RolId, @OpcionId, 1
WHERE @RolId IS NOT NULL AND @OpcionId IS NOT NULL
  AND NOT EXISTS (
      SELECT 1
      FROM conf_rolopciones
      WHERE RolId = @RolId AND OpcionId = @OpcionId
  );
