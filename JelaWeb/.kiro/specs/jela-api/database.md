# 🗄️ DATABASE DESIGN - JELA.API

**Fecha:** Enero 21, 2026  
**Versión:** 1.0  
**Motor:** MySQL 8.0 (Azure Database for MySQL)  

---

## 🎯 VISIÓN GENERAL DE LA BASE DE DATOS

La base de datos de JELA implementa un diseño **multi-tenant** con arquitectura **normalizada** y **escalable**, optimizada para operaciones CRUD dinámicas y análisis de datos.

### Características Principales
- **Multi-tenant:** Soporte para múltiples condominios/entidades
- **Normalización:** 3FN (Tercera Forma Normal)
- **Dinámica:** Esquemas auto-descubribles
- **Auditada:** Rastreo completo de cambios
- **Escalable:** Particionamiento por entidad

---

## 🏗️ ARQUITECTURA DE LA BASE DE DATOS

### Diagrama Entidad-Relación General
```
┌─────────────────────────────────────────────────┐
│                 ENTIDADES                       │
│  ┌─────────────────────────────────────────────┐ │
│  │              cat_entidades                  │ │
│  │  • Información básica del condominio       │ │
│  └─────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
                        │
                        │ 1:N
                        ▼
┌─────────────────────────────────────────────────┐
│                 USUARIOS                        │
│  ┌─────────────────────────────────────────────┐ │
│  │              conf_usuarios                  │ │
│  │  • Autenticación y roles                   │ │
│  └─────────────────────────────────────────────┘ │
│  ┌─────────────────────────────────────────────┐ │
│  │          conf_usuario_entidades             │ │
│  │  • Relación usuario-entidad (N:M)          │ │
│  └─────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
                        │
                        │ 1:N
                        ▼
┌─────────────────────────────────────────────────┐
│               OPERACIONES                       │
│  ┌─────────────────────────────────────────────┐ │
│  │              op_tickets                     │ │
│  │  • Sistema de tickets                      │ │
│  └─────────────────────────────────────────────┘ │
│  ┌─────────────────────────────────────────────┐ │
│  │          op_interacciones                   │ │
│  │  • Conversaciones y mensajes               │ │
│  └─────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────┘
```

---

## 📋 ESTRUCTURA DE TABLAS POR PREFIJO

### 📂 Prefijo `cat_` - Catálogos
Tablas de referencia que contienen datos maestros.

#### cat_entidades
**Propósito:** Información básica de condominios/entidades

```sql
CREATE TABLE cat_entidades (
    Id INT NOT NULL AUTO_INCREMENT,
    Clave VARCHAR(10) NOT NULL COMMENT 'Código único de la entidad',
    Alias VARCHAR(50) NOT NULL COMMENT 'Nombre corto para UI',
    RazonSocial VARCHAR(200) NOT NULL COMMENT 'Nombre legal completo',
    RFC VARCHAR(13) NULL COMMENT 'RFC de la entidad',
    TipoCondominio ENUM('Vertical', 'Horizontal') DEFAULT 'Vertical',
    NumeroUnidades INT DEFAULT 0,
    FechaConstitucion DATE NULL,
    Direccion TEXT NULL,
    Ciudad VARCHAR(100) NULL,
    Estado VARCHAR(50) NULL,
    CodigoPostal VARCHAR(10) NULL,
    Telefono VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    SitioWeb VARCHAR(200) NULL,
    Activo BOOLEAN DEFAULT TRUE,
    EsCondominio BOOLEAN DEFAULT TRUE,
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaModificacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    UNIQUE KEY uk_entidades_clave (Clave),
    INDEX idx_entidades_activo (Activo),
    INDEX idx_entidades_tipo (TipoCondominio)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### cat_proveedores
**Propósito:** Directorio de proveedores de servicios

```sql
CREATE TABLE cat_proveedores (
    Id INT NOT NULL AUTO_INCREMENT,
    IdEntidad INT NOT NULL COMMENT 'FK a cat_entidades',
    RazonSocial VARCHAR(200) NOT NULL,
    NombreComercial VARCHAR(200) NULL,
    RFC VARCHAR(13) NOT NULL,
    TipoProveedor ENUM('Servicio', 'Producto', 'Ambos') DEFAULT 'Servicio',
    Categoria VARCHAR(50) NULL COMMENT 'Plomería, Electricidad, etc.',
    Especialidad TEXT NULL COMMENT 'Servicios específicos',
    Direccion TEXT NULL,
    Ciudad VARCHAR(100) NULL,
    Estado VARCHAR(50) NULL,
    CodigoPostal VARCHAR(10) NULL,
    Telefono VARCHAR(20) NULL,
    Email VARCHAR(100) NULL,
    SitioWeb VARCHAR(200) NULL,
    ContactoNombre VARCHAR(100) NULL,
    ContactoTelefono VARCHAR(20) NULL,
    ContactoEmail VARCHAR(100) NULL,
    Calificacion DECIMAL(3,2) DEFAULT 0.00 COMMENT '1.00 a 5.00',
    NumeroServicios INT DEFAULT 0,
    CostoPromedio DECIMAL(10,2) NULL,
    TiempoRespuestaPromedio INT NULL COMMENT 'Minutos',
    Activo BOOLEAN DEFAULT TRUE,
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaModificacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
    UNIQUE KEY uk_proveedores_rfc_entidad (RFC, IdEntidad),
    INDEX idx_proveedores_entidad (IdEntidad),
    INDEX idx_proveedores_activo (Activo),
    INDEX idx_proveedores_categoria (Categoria),
    INDEX idx_proveedores_calificacion (Calificacion DESC)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

---

### 📂 Prefijo `conf_` - Configuración
Tablas de configuración del sistema y usuarios.

#### conf_usuarios
**Propósito:** Usuarios del sistema

```sql
CREATE TABLE conf_usuarios (
    Id INT NOT NULL AUTO_INCREMENT,
    Email VARCHAR(100) NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    Nombre VARCHAR(100) NOT NULL,
    ApellidoPaterno VARCHAR(50) NULL,
    ApellidoMaterno VARCHAR(50) NULL,
    Telefono VARCHAR(20) NULL,
    TipoUsuario ENUM('AdministradorCondominios', 'MesaDirectiva', 'Residente', 'Empleado') DEFAULT 'Residente',
    IdEntidadPrincipal INT NULL COMMENT 'Para usuarios de una sola entidad',
    Activo BOOLEAN DEFAULT TRUE,
    EmailConfirmado BOOLEAN DEFAULT FALSE,
    FechaUltimoLogin DATETIME NULL,
    IntentosFallidos INT DEFAULT 0,
    BloqueadoHasta DATETIME NULL,
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaModificacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    UNIQUE KEY uk_usuarios_email (Email),
    FOREIGN KEY (IdEntidadPrincipal) REFERENCES cat_entidades(Id),
    INDEX idx_usuarios_activo (Activo),
    INDEX idx_usuarios_tipo (TipoUsuario),
    INDEX idx_usuarios_entidad_principal (IdEntidadPrincipal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### conf_usuario_entidades
**Propósito:** Relación muchos-a-muchos entre usuarios y entidades

```sql
CREATE TABLE conf_usuario_entidades (
    Id INT NOT NULL AUTO_INCREMENT,
    IdUsuario INT NOT NULL,
    IdEntidad INT NOT NULL,
    EsPrincipal BOOLEAN DEFAULT FALSE COMMENT 'Entidad principal del usuario',
    Rol VARCHAR(50) DEFAULT 'Usuario' COMMENT 'Rol específico en esta entidad',
    Permisos JSON NULL COMMENT 'Permisos específicos para esta entidad',
    FechaAsignacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    AsignadoPor INT NULL COMMENT 'Usuario que realizó la asignación',

    PRIMARY KEY (Id),
    FOREIGN KEY (IdUsuario) REFERENCES conf_usuarios(Id),
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
    FOREIGN KEY (AsignadoPor) REFERENCES conf_usuarios(Id),
    UNIQUE KEY uk_usuario_entidad (IdUsuario, IdEntidad),
    INDEX idx_usuario_entidades_usuario (IdUsuario),
    INDEX idx_usuario_entidades_entidad (IdEntidad)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### conf_ticket_prompts
**Propósito:** Prompts de IA para diferentes canales

```sql
CREATE TABLE conf_ticket_prompts (
    Id INT NOT NULL AUTO_INCREMENT,
    IdEntidad INT NOT NULL,
    NombrePrompt VARCHAR(50) NOT NULL COMMENT 'ChatWebSistema, VAPISistema, etc.',
    ContenidoPrompt TEXT NOT NULL COMMENT 'Prompt completo para IA',
    Canal ENUM('VAPI', 'YCloud', 'ChatWeb', 'Firebase') NOT NULL,
    TipoPrompt ENUM('Sistema', 'Usuario', 'Analisis') DEFAULT 'Sistema',
    Version INT DEFAULT 1,
    Activo BOOLEAN DEFAULT TRUE,
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaModificacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
    UNIQUE KEY uk_prompts_entidad_nombre (IdEntidad, NombrePrompt),
    INDEX idx_prompts_entidad (IdEntidad),
    INDEX idx_prompts_canal (Canal),
    INDEX idx_prompts_activo (Activo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

---

### 📂 Prefijo `op_` - Operaciones
Tablas transaccionales del negocio.

#### op_tickets
**Propósito:** Sistema de tickets de atención al cliente

```sql
CREATE TABLE op_tickets (
    Id INT NOT NULL AUTO_INCREMENT,
    IdEntidad INT NOT NULL,
    NumeroTicket VARCHAR(20) NOT NULL COMMENT 'AUTO-2026-0001',
    Titulo VARCHAR(200) NOT NULL,
    Descripcion TEXT NULL,
    Estado ENUM('Abierto', 'EnProceso', 'Resuelto', 'Cerrado', 'Cancelado') DEFAULT 'Abierto',
    Prioridad ENUM('Baja', 'Media', 'Alta', 'Critica') DEFAULT 'Media',
    Categoria VARCHAR(50) NULL,
    Subcategoria VARCHAR(50) NULL,
    Canal ENUM('Telefono', 'WhatsApp', 'Web', 'Email', 'App') DEFAULT 'Web',
    IdUsuarioCreador INT NOT NULL COMMENT 'Usuario que creó el ticket',
    IdUsuarioAsignado INT NULL COMMENT 'Usuario asignado',
    IdProveedorAsignado INT NULL COMMENT 'Proveedor asignado',
    Unidad VARCHAR(20) NULL COMMENT 'Número de unidad afectada',
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,
    FechaModificacion DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FechaCierre DATETIME NULL,
    FechaResolucion DATETIME NULL,
    TiempoResolucion INT NULL COMMENT 'Minutos',
    Satisfaccion INT NULL COMMENT '1-5 estrellas',
    ComentariosCierre TEXT NULL,

    PRIMARY KEY (Id),
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id),
    FOREIGN KEY (IdUsuarioCreador) REFERENCES conf_usuarios(Id),
    FOREIGN KEY (IdUsuarioAsignado) REFERENCES conf_usuarios(Id),
    FOREIGN KEY (IdProveedorAsignado) REFERENCES cat_proveedores(Id),
    UNIQUE KEY uk_tickets_numero (NumeroTicket),
    INDEX idx_tickets_entidad (IdEntidad),
    INDEX idx_tickets_estado (Estado),
    INDEX idx_tickets_prioridad (Prioridad),
    INDEX idx_tickets_canal (Canal),
    INDEX idx_tickets_creador (IdUsuarioCreador),
    INDEX idx_tickets_asignado (IdUsuarioAsignado),
    INDEX idx_tickets_fecha_creacion (FechaCreacion),
    INDEX idx_tickets_fecha_cierre (FechaCierre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### op_interacciones
**Propósito:** Historial de conversaciones en tickets

```sql
CREATE TABLE op_interacciones (
    Id INT NOT NULL AUTO_INCREMENT,
    IdTicket INT NOT NULL,
    Tipo ENUM('MensajeUsuario', 'MensajeSistema', 'RespuestaIA', 'NotaInterna') NOT NULL,
    Contenido TEXT NOT NULL,
    IdUsuario INT NULL COMMENT 'Usuario que escribió (NULL para IA/sistema)',
    Canal ENUM('Telefono', 'WhatsApp', 'Web', 'Email', 'App') NULL,
    Metadata JSON NULL COMMENT 'Datos adicionales (ubicación, dispositivo, etc.)',
    EsVisibleUsuario BOOLEAN DEFAULT TRUE,
    FechaCreacion DATETIME DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    FOREIGN KEY (IdTicket) REFERENCES op_tickets(Id) ON DELETE CASCADE,
    FOREIGN KEY (IdUsuario) REFERENCES conf_usuarios(Id),
    INDEX idx_interacciones_ticket (IdTicket),
    INDEX idx_interacciones_tipo (Tipo),
    INDEX idx_interacciones_usuario (IdUsuario),
    INDEX idx_interacciones_fecha (FechaCreacion),
    INDEX idx_interacciones_visible (EsVisibleUsuario)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

---

### 📂 Prefijo `log_` - Auditoría
Tablas de logging y auditoría.

#### log_crud_operations
**Propósito:** Auditoría de todas las operaciones CRUD

```sql
CREATE TABLE log_crud_operations (
    Id BIGINT NOT NULL AUTO_INCREMENT,
    IdUsuario INT NULL COMMENT 'Usuario que realizó la operación',
    Tabla VARCHAR(100) NOT NULL,
    Operacion ENUM('INSERT', 'UPDATE', 'DELETE', 'SELECT') NOT NULL,
    IdRegistro VARCHAR(50) NULL COMMENT 'ID del registro afectado',
    DatosAnteriores JSON NULL COMMENT 'Estado antes de la operación',
    DatosNuevos JSON NULL COMMENT 'Estado después de la operación',
    IpAddress VARCHAR(45) NULL COMMENT 'IPv4/IPv6 del usuario',
    UserAgent TEXT NULL,
    Endpoint VARCHAR(200) NULL COMMENT 'Endpoint que realizó la operación',
    Exitoso BOOLEAN DEFAULT TRUE,
    ErrorMensaje TEXT NULL,
    TiempoEjecucion INT NULL COMMENT 'Milisegundos',
    FechaOperacion DATETIME DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    FOREIGN KEY (IdUsuario) REFERENCES conf_usuarios(Id),
    INDEX idx_log_usuario (IdUsuario),
    INDEX idx_log_tabla (Tabla),
    INDEX idx_log_operacion (Operacion),
    INDEX idx_log_fecha (FechaOperacion),
    INDEX idx_log_exitoso (Exitoso)
) PARTITION BY RANGE (YEAR(FechaOperacion)) (
    PARTITION p2024 VALUES LESS THAN (2025),
    PARTITION p2025 VALUES LESS THAN (2026),
    PARTITION p2026 VALUES LESS THAN (2027),
    PARTITION p_future VALUES LESS THAN MAXVALUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

#### log_api_requests
**Propósito:** Logging de todas las requests al API

```sql
CREATE TABLE log_api_requests (
    Id BIGINT NOT NULL AUTO_INCREMENT,
    TraceId VARCHAR(50) NULL COMMENT 'ID de correlación',
    Metodo VARCHAR(10) NOT NULL COMMENT 'GET, POST, etc.',
    Endpoint VARCHAR(500) NOT NULL,
    QueryString TEXT NULL,
    StatusCode INT NOT NULL,
    ResponseTime INT NOT NULL COMMENT 'Milisegundos',
    RequestSize INT NULL COMMENT 'Bytes',
    ResponseSize INT NULL COMMENT 'Bytes',
    IpAddress VARCHAR(45) NULL,
    UserAgent TEXT NULL,
    IdUsuario INT NULL,
    ErrorMessage TEXT NULL,
    FechaRequest DATETIME DEFAULT CURRENT_TIMESTAMP,

    PRIMARY KEY (Id),
    FOREIGN KEY (IdUsuario) REFERENCES conf_usuarios(Id),
    INDEX idx_api_fecha (FechaRequest),
    INDEX idx_api_metodo (Metodo),
    INDEX idx_api_endpoint (Endpoint(100)),
    INDEX idx_api_status (StatusCode),
    INDEX idx_api_usuario (IdUsuario)
) PARTITION BY RANGE (YEAR(FechaRequest)) (
    PARTITION p2024 VALUES LESS THAN (2025),
    PARTITION p2025 VALUES LESS THAN (2026),
    PARTITION p2026 VALUES LESS THAN (2027),
    PARTITION p_future VALUES LESS THAN MAXVALUE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
```

---

## 🔗 RELACIONES Y CONSTRAINTS

### Foreign Keys Principales
```sql
-- Entidades
ALTER TABLE cat_proveedores ADD CONSTRAINT fk_proveedores_entidad
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id);

-- Usuarios
ALTER TABLE conf_usuario_entidades ADD CONSTRAINT fk_usuario_entidades_usuario
    FOREIGN KEY (IdUsuario) REFERENCES conf_usuarios(Id);
ALTER TABLE conf_usuario_entidades ADD CONSTRAINT fk_usuario_entidades_entidad
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id);

-- Tickets
ALTER TABLE op_tickets ADD CONSTRAINT fk_tickets_entidad
    FOREIGN KEY (IdEntidad) REFERENCES cat_entidades(Id);
ALTER TABLE op_tickets ADD CONSTRAINT fk_tickets_creador
    FOREIGN KEY (IdUsuarioCreador) REFERENCES conf_usuarios(Id);
ALTER TABLE op_tickets ADD CONSTRAINT fk_tickets_asignado
    FOREIGN KEY (IdUsuarioAsignado) REFERENCES conf_usuarios(Id);
ALTER TABLE op_tickets ADD CONSTRAINT fk_tickets_proveedor
    FOREIGN KEY (IdProveedorAsignado) REFERENCES cat_proveedores(Id);

-- Interacciones
ALTER TABLE op_interacciones ADD CONSTRAINT fk_interacciones_ticket
    FOREIGN KEY (IdTicket) REFERENCES op_tickets(Id) ON DELETE CASCADE;
```

---

## 📊 ÍNDICES OPTIMIZADOS

### Índices de Performance
```sql
-- Búsquedas frecuentes
CREATE INDEX idx_tickets_busqueda ON op_tickets (Estado, Prioridad, FechaCreacion DESC);
CREATE INDEX idx_interacciones_ticket_fecha ON op_interacciones (IdTicket, FechaCreacion DESC);

-- Reportes
CREATE INDEX idx_tickets_reportes ON op_tickets (IdEntidad, Estado, FechaCreacion, Categoria);
CREATE INDEX idx_proveedores_reportes ON cat_proveedores (IdEntidad, Activo, Calificacion DESC);

-- Auditoría
CREATE INDEX idx_log_operations_auditoria ON log_crud_operations (IdUsuario, Tabla, FechaOperacion DESC);
```

### Índices de Texto Completo
```sql
-- Búsqueda en tickets
ALTER TABLE op_tickets ADD FULLTEXT INDEX ft_tickets_contenido (Titulo, Descripcion);

-- Búsqueda en interacciones
ALTER TABLE op_interacciones ADD FULLTEXT INDEX ft_interacciones_contenido (Contenido);
```

---

## 🔄 SISTEMA CRUD DINÁMICO

### Validación de Tablas Permitidas
```sql
-- Tablas permitidas para operaciones CRUD
CREATE TABLE conf_tablas_permitidas (
    Id INT NOT NULL AUTO_INCREMENT,
    Prefijo VARCHAR(10) NOT NULL,
    Descripcion VARCHAR(100) NULL,
    Activo BOOLEAN DEFAULT TRUE,

    PRIMARY KEY (Id),
    UNIQUE KEY uk_prefijo (Prefijo)
) ENGINE=InnoDB;

INSERT INTO conf_tablas_permitidas (Prefijo, Descripcion) VALUES
('cat_', 'Catálogos'),
('conf_', 'Configuración'),
('op_', 'Operaciones'),
('log_', 'Logs y Auditoría');
```

### Descubrimiento de Esquemas
```sql
-- Query para obtener estructura de tabla dinámicamente
SELECT
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT,
    CHARACTER_MAXIMUM_LENGTH,
    NUMERIC_PRECISION,
    NUMERIC_SCALE,
    COLUMN_KEY,
    EXTRA
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
    AND TABLE_NAME = ?
ORDER BY ORDINAL_POSITION;
```

---

## 📈 OPTIMIZACIONES DE PERFORMANCE

### Particionamiento
```sql
-- Particionamiento por entidad para tablas grandes
ALTER TABLE op_tickets PARTITION BY HASH(IdEntidad) PARTITIONS 16;
ALTER TABLE op_interacciones PARTITION BY HASH(IdTicket DIV 10000) PARTITIONS 64;
```

### Archiving Strategy
```sql
-- Procedimiento para archivar tickets antiguos
DELIMITER //
CREATE PROCEDURE ArchiveOldTickets()
BEGIN
    DECLARE cutoff_date DATE;
    SET cutoff_date = DATE_SUB(CURDATE(), INTERVAL 2 YEAR);

    INSERT INTO op_tickets_archived
    SELECT * FROM op_tickets
    WHERE FechaCreacion < cutoff_date
        AND Estado IN ('Cerrado', 'Cancelado');

    DELETE FROM op_tickets
    WHERE FechaCreacion < cutoff_date
        AND Estado IN ('Cerrado', 'Cancelado');
END //
DELIMITER ;
```

---

## 🔐 SEGURIDAD DE DATOS

### Encriptación
```sql
-- Campos sensibles encriptados
ALTER TABLE conf_usuarios ADD COLUMN PasswordHash VARBINARY(255);
ALTER TABLE cat_proveedores ADD COLUMN RFC_Encriptado VARBINARY(255);

-- Función de encriptación
DELIMITER //
CREATE FUNCTION EncryptData(data TEXT, key_text TEXT)
RETURNS TEXT
DETERMINISTIC
BEGIN
    RETURN AES_ENCRYPT(data, key_text);
END //
DELIMITER ;
```

### Row Level Security
```sql
-- Vista con RLS para usuarios
CREATE VIEW vw_tickets_usuario AS
SELECT t.* FROM op_tickets t
INNER JOIN conf_usuario_entidades ue ON t.IdEntidad = ue.IdEntidad
WHERE ue.IdUsuario = @CurrentUserId;
```

---

## 📊 MÉTRICAS Y MONITOREO

### Queries de Monitoreo
```sql
-- Tamaño de tablas
SELECT
    table_name,
    ROUND(data_length / 1024 / 1024, 2) AS data_mb,
    ROUND(index_length / 1024 / 1024, 2) AS index_mb,
    ROUND((data_length + index_length) / 1024 / 1024, 2) AS total_mb
FROM information_schema.tables
WHERE table_schema = DATABASE()
ORDER BY total_mb DESC;

-- Queries lentas
SELECT
    sql_text,
    exec_count,
    avg_timer_wait / 1000000000 AS avg_time_sec
FROM performance_schema.events_statements_summary_by_digest
WHERE avg_timer_wait > 1000000000  -- Más de 1 segundo
ORDER BY avg_timer_wait DESC;

-- Conexiones activas
SHOW PROCESSLIST;
```

---

## 🚀 ESTRATEGIAS DE ESCALABILIDAD

### Read Replicas
```sql
-- Configuración de replica de lectura
CHANGE MASTER TO
    MASTER_HOST='replica-server',
    MASTER_USER='replica_user',
    MASTER_PASSWORD='password',
    MASTER_LOG_FILE='mysql-bin.000001',
    MASTER_LOG_POS=0;

START SLAVE;
```

### Sharding por Entidad
```sql
-- Función de sharding
DELIMITER //
CREATE FUNCTION GetShard(entidad_id INT)
RETURNS INT
DETERMINISTIC
BEGIN
    RETURN entidad_id % 4;  -- 4 shards
END //
DELIMITER ;
```

---

## 🔄 ESTRATEGIAS DE BACKUP

### Backup Incremental
```bash
# Backup completo semanal
mysqldump --all-databases --single-transaction > full_backup_$(date +%Y%m%d).sql

# Backup incremental diario
mysqlbinlog --read-from-remote-server mysql-bin.000001 > incremental_$(date +%Y%m%d).sql
```

### Point-in-Time Recovery
```sql
-- Restaurar a un punto específico
mysqlbinlog --stop-datetime="2026-01-21 10:00:00" mysql-bin.000001 | mysql
```

---

## 📋 CHECKLIST DE CALIDAD

### Diseño
- [ ] Normalización 3FN cumplida
- [ ] Foreign keys definidas
- [ ] Índices optimizados
- [ ] Constraints apropiadas
- [ ] Nombres consistentes

### Performance
- [ ] Queries optimizadas
- [ ] Índices apropiados
- [ ] Particionamiento implementado
- [ ] Caché configurado

### Seguridad
- [ ] Datos sensibles encriptados
- [ ] RLS implementado
- [ ] Auditoría completa
- [ ] Backups automáticos

### Escalabilidad
- [ ] Sharding strategy definida
- [ ] Read replicas configuradas
- [ ] Connection pooling
- [ ] Monitoring implementado

---

## 🔗 REFERENCIAS

- [README.md](./README.md) - Documentación general
- [rules.md](./rules.md) - Reglas de programación
- [architecture.md](./architecture.md) - Arquitectura detallada
- [endpoints.md](./endpoints.md) - Documentación de endpoints