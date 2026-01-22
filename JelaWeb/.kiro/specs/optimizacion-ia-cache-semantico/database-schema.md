# Esquemas de Base de Datos

**Módulo:** optimizacion-ia-cache-semantico  
**Base de Datos:** JELA.API Database  

## Tabla 1: conf_ia_cache_semantico (ESENCIAL)

```sql
CREATE TABLE conf_ia_cache_semantico (
    Id INT NOT NULL AUTO_INCREMENT,
    IdEntidad INT NOT NULL,
    
    -- Prompt original
    PromptOriginal TEXT NOT NULL,
    PromptNormalizado TEXT NOT NULL COMMENT 'Sin acentos, minúsculas',
    
    -- Vector para búsqueda semántica
    VectorEmbedding JSON NOT NULL COMMENT 'Array de 1536 floats',
    
    -- Respuesta cacheada
    RespuestaIA TEXT NOT NULL,
    Categoria VARCHAR(50) DEFAULT NULL,
    
    -- Métricas de uso
    NumeroUsos INT DEFAULT 1,
    UltimaConsulta DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    -- Control de vigencia
    VigenciaHasta DATETIME DEFAULT NULL COMMENT 'NULL = permanente',
    Activo TINYINT(1) DEFAULT 1,
    
    PRIMARY KEY (Id),
    INDEX idx_entidad (IdEntidad),
    INDEX idx_categoria (Categoria),
    INDEX idx_vigencia (VigenciaHasta, Activo),
    INDEX idx_uso (NumeroUsos DESC),
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
    
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Caché semántico de respuestas IA';
```

## Tabla 2: conf_ia_patrones (SPRINT 2)

```sql
CREATE TABLE conf_ia_patrones (
    Id INT NOT NULL AUTO_INCREMENT,
    IdEntidad INT NOT NULL,
    
    -- Patrón de búsqueda
    PatronRegex VARCHAR(255) NOT NULL,
    Descripcion VARCHAR(255) NOT NULL,
    
    -- Respuesta directa
    RespuestaDirecta TEXT NOT NULL,
    
    -- Métricas
    NumeroUsos INT DEFAULT 0,
    UltimaConsulta DATETIME DEFAULT NULL,
    
    -- Control
    Activo TINYINT(1) DEFAULT 1,
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    
    PRIMARY KEY (Id),
    INDEX idx_activo (Activo),
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
    
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Patrones de respuestas directas';
```

## Tabla 3: op_ia_metricas_cache (SPRINT 4)

```sql
CREATE TABLE op_ia_metricas_cache (
    Id INT NOT NULL AUTO_INCREMENT,
    IdEntidad INT NOT NULL,
    Fecha DATE NOT NULL,
    
    -- Contadores
    TotalConsultas INT DEFAULT 0,
    ConsultasPatron INT DEFAULT 0,
    ConsultasCache INT DEFAULT 0,
    ConsultasAzureOpenAI INT DEFAULT 0,
    
    -- Métricas de rendimiento
    TiempoPromedioMs INT DEFAULT 0,
    CostoTotal DECIMAL(10,4) DEFAULT 0.00,
    
    -- Efectividad del caché
    TasaCacheHit DECIMAL(5,2) DEFAULT 0.00 COMMENT 'Porcentaje de cache hits',
    
    PRIMARY KEY (Id),
    UNIQUE INDEX uk_entidad_fecha (IdEntidad, Fecha),
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id)
    
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci
COMMENT='Métricas diarias de optimización de IA';
```

## Stored Procedures

### SP: sp_limpiar_cache_expirado
```sql
DELIMITER //
CREATE PROCEDURE sp_limpiar_cache_expirado()
BEGIN
    DELETE FROM conf_ia_cache_semantico 
    WHERE VigenciaHasta IS NOT NULL 
      AND VigenciaHasta < NOW() 
      AND Activo = 1;
      
    SELECT ROW_COUNT() as 'Entradas eliminadas';
END //
DELIMITER ;
```

### SP: sp_actualizar_metricas_diarias
```sql
DELIMITER //
CREATE PROCEDURE sp_actualizar_metricas_diarias(IN p_fecha DATE, IN p_entidad_id INT)
BEGIN
    INSERT INTO op_ia_metricas_cache (
        IdEntidad, Fecha, TotalConsultas, 
        ConsultasPatron, ConsultasCache, ConsultasAzureOpenAI,
        TiempoPromedioMs, CostoTotal, TasaCacheHit
    )
    SELECT 
        p_entidad_id,
        p_fecha,
        COUNT(*) as total,
        SUM(CASE WHEN Source = 'Patron' THEN 1 ELSE 0 END) as patrones,
        SUM(CASE WHEN Source = 'Cache' THEN 1 ELSE 0 END) as cache,
        SUM(CASE WHEN Source = 'Azure' THEN 1 ELSE 0 END) as azure,
        AVG(ResponseTimeMs) as tiempo_promedio,
        SUM(Cost) as costo_total,
        (SUM(CASE WHEN Source IN ('Patron','Cache') THEN 1 ELSE 0 END) / COUNT(*)) * 100 as tasa_hit
    FROM ia_consultas_log 
    WHERE DATE(FechaConsulta) = p_fecha 
      AND IdEntidad = p_entidad_id
    ON DUPLICATE KEY UPDATE
        TotalConsultas = VALUES(TotalConsultas),
        ConsultasPatron = VALUES(ConsultasPatron),
        ConsultasCache = VALUES(ConsultasCache),
        ConsultasAzureOpenAI = VALUES(ConsultasAzureOpenAI),
        TiempoPromedioMs = VALUES(TiempoPromedioMs),
        CostoTotal = VALUES(CostoTotal),
        TasaCacheHit = VALUES(TasaCacheHit);
END //
DELIMITER ;
```

## Notas de Implementación
- Usar JSON para VectorEmbedding por flexibilidad
- Índices optimizados para búsquedas por entidad y fecha
- Stored procedures para operaciones batch eficientes
- Considerar particionamiento si tabla crece mucho (>1M registros)