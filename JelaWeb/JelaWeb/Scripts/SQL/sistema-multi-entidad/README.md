# Scripts SQL - Sistema Multi-Entidad

**Fecha:** 20 de Enero de 2026  
**Versión:** 1.0  
**Referencia:** `.kiro/specs/sistema-multi-entidad/design.md`

---

## 📋 Descripción

Scripts SQL para implementar el sistema multi-entidad que permite a Administradores de Condominios gestionar múltiples entidades desde una única cuenta.

---

## 🔢 Orden de Ejecución

**IMPORTANTE:** Ejecutar los scripts en el orden indicado. Cada script depende del anterior.

### 1. Backup de Base de Datos (OBLIGATORIO)

Antes de ejecutar cualquier script, realizar backup completo:

```bash
mysqldump -u root -p jelabbc > backup_pre_multi_entidad_$(date +%Y%m%d_%H%M%S).sql
```

### 2. Scripts de Migración

Ejecutar en este orden:

| # | Script | Descripción | Tiempo Est. |
|---|--------|-------------|-------------|
| 1 | `01_ALTER_conf_usuarios_agregar_TipoUsuario.sql` | Agrega campo TipoUsuario a conf_usuarios | < 1 min |
| 2 | `02_ALTER_conf_usuarios_agregar_IdEntidadPrincipal_Licencias.sql` | Agrega campos IdEntidadPrincipal y LicenciasDisponibles | < 1 min |
| 3 | `03_CREATE_conf_usuario_entidades.sql` | Crea tabla de relación usuario-entidad | < 1 min |
| 4 | `04_MIGRATE_datos_usuarios_entidades.sql` | Migra datos existentes al nuevo esquema | 1-5 min |

### 3. Comando de Ejecución

```bash
# Opción 1: Ejecutar todos los scripts en orden
mysql -u root -p jelabbc < 01_ALTER_conf_usuarios_agregar_TipoUsuario.sql
mysql -u root -p jelabbc < 02_ALTER_conf_usuarios_agregar_IdEntidadPrincipal_Licencias.sql
mysql -u root -p jelabbc < 03_CREATE_conf_usuario_entidades.sql
mysql -u root -p jelabbc < 04_MIGRATE_datos_usuarios_entidades.sql

# Opción 2: Ejecutar desde MySQL Workbench
# Abrir cada archivo y ejecutar con Ctrl+Shift+Enter
```

---

## ✅ Validaciones Post-Ejecución

Después de ejecutar todos los scripts, validar:

### 1. Estructura de conf_usuarios

```sql
DESCRIBE conf_usuarios;
-- Debe mostrar: TipoUsuario, IdEntidadPrincipal, LicenciasDisponibles
```

### 2. Tabla conf_usuario_entidades

```sql
DESCRIBE conf_usuario_entidades;
-- Debe existir con todos los campos
```

### 3. Datos Migrados

```sql
-- Todos los usuarios deben tener al menos una entidad
SELECT COUNT(*) FROM conf_usuarios u
LEFT JOIN conf_usuario_entidades ue ON u.Id = ue.IdUsuario
WHERE u.Activo = 1 AND ue.Id IS NULL;
-- Debe retornar 0

-- Todos los usuarios deben tener IdEntidadPrincipal
SELECT COUNT(*) FROM conf_usuarios
WHERE Activo = 1 AND IdEntidadPrincipal IS NULL;
-- Debe retornar 0
```

### 4. Integridad Referencial

```sql
-- Verificar foreign keys
SELECT 
    CONSTRAINT_NAME,
    TABLE_NAME,
    COLUMN_NAME,
    REFERENCED_TABLE_NAME,
    REFERENCED_COLUMN_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_SCHEMA = 'jelabbc' 
  AND TABLE_NAME IN ('conf_usuarios', 'conf_usuario_entidades')
  AND REFERENCED_TABLE_NAME IS NOT NULL;
```

---

## 🔄 Plan de Rollback

Si algo sale mal, restaurar el backup:

```bash
# Detener aplicación
# Restaurar backup
mysql -u root -p jelabbc < backup_pre_multi_entidad_YYYYMMDD_HHMMSS.sql

# Verificar restauración
mysql -u root -p jelabbc -e "SELECT COUNT(*) FROM conf_usuarios;"
```

---

## 📊 Cambios en el Esquema

### Tabla: conf_usuarios (MODIFICADA)

**Campos Nuevos:**
- `TipoUsuario` - ENUM('AdministradorCondominios', 'MesaDirectiva', 'Residente', 'Empleado')
- `IdEntidadPrincipal` - INT NULL (FK a cat_entidades)
- `LicenciasDisponibles` - INT DEFAULT 0

**Índices Nuevos:**
- `idx_usuarios_tipo` - Índice en TipoUsuario
- `idx_usuarios_entidad_principal` - Índice en IdEntidadPrincipal

**Foreign Keys Nuevas:**
- `fk_usuarios_entidad_principal` - IdEntidadPrincipal → cat_entidades(Id)

### Tabla: conf_usuario_entidades (NUEVA)

**Propósito:** Relación muchos a muchos entre usuarios y entidades

**Campos:**
- `Id` - INT AUTO_INCREMENT (PK)
- `IdUsuario` - INT NOT NULL (FK a conf_usuarios)
- `IdEntidad` - INT NOT NULL (FK a cat_entidades)
- `EsPrincipal` - BOOLEAN DEFAULT FALSE
- `FechaAsignacion` - DATETIME
- `IdUsuarioCreacion` - INT DEFAULT 1
- `FechaCreacion` - DATETIME
- `Activo` - TINYINT(1) DEFAULT 1

**Índices:**
- `uk_usuario_entidad` - UNIQUE (IdUsuario, IdEntidad)
- `idx_usuario` - Índice en IdUsuario
- `idx_entidad` - Índice en IdEntidad
- `idx_principal` - Índice en EsPrincipal

**Foreign Keys:**
- `fk_usuario_entidades_usuario` - IdUsuario → conf_usuarios(Id) ON DELETE CASCADE
- `fk_usuario_entidades_entidad` - IdEntidad → cat_entidades(Id) ON DELETE CASCADE

---

## 🚨 Notas Importantes

1. **Backup Obligatorio:** NUNCA ejecutar sin backup previo
2. **Entorno de Prueba:** Probar primero en desarrollo/staging
3. **Horario:** Ejecutar en horario de bajo tráfico
4. **Monitoreo:** Verificar logs de aplicación después de la migración
5. **Usuarios Existentes:** Todos los usuarios se migran como tipo "Residente" por defecto
6. **Licencias:** Todos los usuarios inician con 0 licencias disponibles

---

## 📞 Soporte

Si encuentras problemas durante la ejecución:

1. NO continuar con los siguientes scripts
2. Revisar mensajes de error en MySQL
3. Verificar logs de la aplicación
4. Considerar rollback si es necesario
5. Documentar el error para análisis

---

**Estado:** ✅ Scripts Listos para Ejecución  
**Última Actualización:** 20 de Enero de 2026
