# Plan de Migración - Múltiples Archivos por Registro

## Objetivo

Migrar de almacenamiento de un archivo único (campo LONGTEXT) a sistema de múltiples archivos (tablas relacionadas) para Residentes, Vehículos y Documentos.

## Pasos de Migración

### 1. Ejecutar Scripts SQL

**Orden de ejecución:**
1. `08_tablas_archivos_multiples.sql` - Crea las 3 nuevas tablas
2. `09_migracion_archivos_a_tablas.sql` - Migra datos existentes

**Verificación:**
```sql
-- Verificar que se crearon las tablas
SELECT TABLE_NAME FROM information_schema.TABLES 
WHERE TABLE_SCHEMA = 'jela_qa' 
  AND TABLE_NAME IN ('cat_residente_archivos', 'cat_vehiculo_archivos', 'cat_documento_unidad_archivos');

-- Verificar migración
SELECT 'Residentes' AS Tipo, COUNT(*) AS Total FROM cat_residente_archivos
UNION ALL
SELECT 'Vehículos', COUNT(*) FROM cat_vehiculo_archivos
UNION ALL
SELECT 'Documentos', COUNT(*) FROM cat_documento_unidad_archivos;
```

### 2. Desplegar Cambios en Código

**Archivos a modificar:**
1. `Unidades.aspx` - Agregar grids/listas de archivos
2. `unidades.js` - Manejar arrays de archivos
3. `Unidades.aspx.vb` - WebMethods para múltiples archivos
4. `VisorArchivo.aspx.vb` - Soporte para múltiples archivos

### 3. Probar Funcionalidad

**Casos de prueba:**
- [ ] Crear nuevo residente con múltiples archivos INE
- [ ] Crear nuevo vehículo con múltiples archivos de tarjeta
- [ ] Crear nuevo documento con múltiples archivos
- [ ] Ver archivos guardados previamente (migrados)
- [ ] Eliminar archivos individuales
- [ ] Verificar que archivos migrados funcionan correctamente

### 4. (Opcional) Limpiar Campos Antiguos

**Después de verificar que todo funciona:**
```sql
-- Eliminar campos antiguos (ejecutar solo después de verificar que todo funciona)
ALTER TABLE cat_residentes DROP COLUMN ImagenINE;
ALTER TABLE cat_vehiculos_unidad DROP COLUMN TarjetaCirculacionBase64;
ALTER TABLE cat_documentos_unidad DROP COLUMN ArchivoBase64;
```

## Rollback

Si algo falla, los campos originales NO se eliminan automáticamente, por lo que se puede:
1. Revertir cambios en código
2. Los datos originales siguen disponibles en los campos antiguos
3. Ejecutar de nuevo la migración si es necesario

## Notas Importantes

- Los campos originales (`ImagenINE`, `TarjetaCirculacionBase64`, `ArchivoBase64`) **NO se eliminan** durante la migración
- Los datos migrados se duplican (original + nuevo), no se mueven
- Se puede eliminar los campos originales después de verificar que todo funciona
