# Proceso de Alta de Entidades en Sistema Multi-Tenant

## Documento Técnico: Configuración de Entidades para Administradores

**Fecha:** 21 de enero de 2026  
**Sistema:** JELA BBC - Sistema Multi-Entidad  
**Versión:** 1.0

---

## 1. Resumen Ejecutivo

Este documento describe el proceso completo que debe ejecutarse cuando:
1. Se da de alta una nueva entidad en el sistema
2. Se asigna una entidad a un administrador de condominios
3. Un administrador necesita gestionar múltiples entidades

El objetivo es garantizar que todas las tablas de configuración necesarias estén pobladas para que la entidad funcione correctamente en todos los módulos del sistema.

---

## 2. Escenarios de Uso

### 2.1 Escenario A: Alta de Nueva Entidad
**Trigger:** Se crea un nuevo condominio/entidad en el sistema  
**Actor:** Administrador del sistema o proceso de onboarding  
**Resultado esperado:** Entidad completamente funcional con todas las configuraciones base

### 2.2 Escenario B: Asignación de Entidad a Administrador
**Trigger:** Se asigna una entidad existente a un administrador de condominios  
**Actor:** Administrador del sistema  
**Resultado esperado:** Administrador puede gestionar la entidad con todas las funcionalidades disponibles

### 2.3 Escenario C: Administrador Multi-Entidad
**Trigger:** Un administrador necesita gestionar múltiples condominios  
**Actor:** Administrador del sistema  
**Resultado esperado:** Administrador puede cambiar entre entidades usando el dropdown y todas funcionan correctamente

---

## 3. Tablas Involucradas en el Proceso

### 3.1 Tabla Principal: `cat_entidades`
**Propósito:** Almacena la información básica de cada entidad/condominio

**Campos críticos:**
- `Id` - Identificador único de la entidad
- `Clave` - Código corto de la entidad
- `Alias` - Nombre corto para mostrar en UI
- `RazonSocial` - Nombre legal completo
- `Activo` - Estado de la entidad (1 = activa, 0 = inactiva)
- `EsCondominio` - Indica si es un condominio (1) o empresa (0)

**Acción requerida:**
```sql
INSERT INTO cat_entidades (
    Clave, Alias, RazonSocial, Activo, EsCondominio, 
    TipoCondominio, NumeroUnidades, FechaConstitucion
) VALUES (
    'COND001', 'Residencial Vista Hermosa', 'Condominio Residencial Vista Hermosa S.A. de C.V.',
    1, 1, 'Vertical', 120, '2020-01-15'
);
```

---

### 3.2 Tabla de Relación: `conf_usuario_entidades`
**Propósito:** Relaciona usuarios (administradores) con las entidades que pueden gestionar

**Campos:**
- `IdUsuario` - FK a `conf_usuarios`
- `IdEntidad` - FK a `cat_entidades`

**Acción requerida:**
```sql
-- Asignar entidad al administrador
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad)
VALUES (5, 3);  -- Usuario 5 puede gestionar entidad 3

-- Verificar que no exista duplicado
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad)
SELECT 5, 3
WHERE NOT EXISTS (
    SELECT 1 FROM conf_usuario_entidades 
    WHERE IdUsuario = 5 AND IdEntidad = 3
);
```

**Nota importante:** Un administrador puede tener múltiples registros en esta tabla (una fila por cada entidad que gestiona).

---

### 3.3 Tabla Crítica: `conf_ticket_prompts`
**Propósito:** Almacena los prompts de IA necesarios para el sistema de tickets y chat web

**Campos críticos:**
- `IdEntidad` - FK a `cat_entidades`
- `NombrePrompt` - Identificador único del prompt (ej: 'ChatWebSistema', 'AnalisisTicket')
- `ContenidoPrompt` - Texto completo del prompt para la IA
- `Activo` - Estado del prompt

**Prompts requeridos mínimos:**
1. `ChatWebSistema` - Para el widget de chat web
2. `AnalisisTicket` - Para análisis automático de tickets
3. `ResolucionTicket` - Para sugerencias de resolución
4. `CategorizacionTicket` - Para categorización automática

**Acción requerida:**
```sql
-- Copiar todos los prompts de una entidad base (ej: entidad 1) a la nueva entidad
INSERT INTO conf_ticket_prompts (
    IdEntidad, NombrePrompt, Descripcion, ContenidoPrompt, Activo, IdUsuarioCreacion
)
SELECT 
    3 AS IdEntidad,  -- Nueva entidad
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion
FROM conf_ticket_prompts
WHERE IdEntidad = 1  -- Entidad base/plantilla
AND NOT EXISTS (
    SELECT 1 FROM conf_ticket_prompts cp2 
    WHERE cp2.IdEntidad = 3 AND cp2.NombrePrompt = conf_ticket_prompts.NombrePrompt
);
```

**⚠️ CRÍTICO:** Sin estos prompts, el chat web y el sistema de tickets NO funcionarán para la entidad.

---

### 3.4 Tabla de Configuración: `conf_ticket_sla`
**Propósito:** Define los SLA (Service Level Agreements) para diferentes prioridades de tickets

**Campos críticos:**
- `IdEntidad` - FK a `cat_entidades`
- `Prioridad` - Nivel de prioridad (Baja, Media, Alta, Crítica)
- `TiempoRespuestaMinutos` - Tiempo máximo para primera respuesta
- `TiempoResolucionMinutos` - Tiempo máximo para resolución

**Acción requerida:**
```sql
-- Copiar SLAs de entidad base a nueva entidad
INSERT INTO conf_ticket_sla (
    IdEntidad, Prioridad, TiempoRespuestaMinutos, TiempoResolucionMinutos, Activo
)
SELECT 
    3 AS IdEntidad,
    Prioridad,
    TiempoRespuestaMinutos,
    TiempoResolucionMinutos,
    Activo
FROM conf_ticket_sla
WHERE IdEntidad = 1
AND NOT EXISTS (
    SELECT 1 FROM conf_ticket_sla sla2 
    WHERE sla2.IdEntidad = 3 AND sla2.Prioridad = conf_ticket_sla.Prioridad
);
```

---

### 3.5 Tablas de Catálogos por Entidad

Estas tablas almacenan datos específicos de cada entidad y deben tener al menos registros base:

#### 3.5.1 `cat_categorias_ticket`
**Propósito:** Categorías de tickets disponibles para la entidad

**Acción requerida:**
```sql
-- Copiar categorías base
INSERT INTO cat_categorias_ticket (IdEntidad, Nombre, Descripcion, Activo)
SELECT 3, Nombre, Descripcion, Activo
FROM cat_categorias_ticket
WHERE IdEntidad = 1;
```

#### 3.5.2 `cat_areas_comunes`
**Propósito:** Áreas comunes del condominio (alberca, salón de eventos, etc.)

**Acción requerida:**
```sql
-- Crear áreas comunes básicas o copiar de plantilla
INSERT INTO cat_areas_comunes (IdEntidad, Nombre, Descripcion, Activo)
VALUES 
    (3, 'Alberca', 'Área de alberca', 1),
    (3, 'Salón de Eventos', 'Salón para eventos sociales', 1),
    (3, 'Gimnasio', 'Área de gimnasio', 1);
```

#### 3.5.3 `cat_conceptos_cuota`
**Propósito:** Conceptos de cobro (cuota de mantenimiento, agua, etc.)

**Acción requerida:**
```sql
-- Crear conceptos básicos
INSERT INTO cat_conceptos_cuota (IdEntidad, Nombre, Descripcion, Monto, Activo)
VALUES 
    (3, 'Cuota de Mantenimiento', 'Cuota mensual de mantenimiento', 1500.00, 1),
    (3, 'Agua', 'Consumo de agua', 0.00, 1);
```

---

## 4. Proceso Completo de Alta de Entidad

### 4.1 Diagrama de Flujo

```
┌─────────────────────────────────────┐
│ 1. Crear registro en cat_entidades │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 2. Asignar a administrador          │
│    (conf_usuario_entidades)         │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 3. Copiar prompts de IA             │
│    (conf_ticket_prompts)            │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 4. Copiar configuración SLA         │
│    (conf_ticket_sla)                │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 5. Crear catálogos base             │
│    - Categorías de tickets          │
│    - Áreas comunes                  │
│    - Conceptos de cuota             │
└──────────────┬──────────────────────┘
               │
               ▼
┌─────────────────────────────────────┐
│ 6. Verificar configuración          │
│    - Prompts activos                │
│    - SLAs configurados              │
│    - Catálogos poblados             │
└─────────────────────────────────────┘
```

### 4.2 Script SQL Completo

```sql
-- ============================================
-- SCRIPT COMPLETO: Alta de Nueva Entidad
-- ============================================

-- Variables (ajustar según necesidad)
SET @nueva_entidad_id = 3;
SET @entidad_plantilla_id = 1;
SET @id_administrador = 5;

-- PASO 1: Crear entidad (si no existe)
INSERT INTO cat_entidades (
    Clave, Alias, RazonSocial, Activo, EsCondominio, 
    TipoCondominio, NumeroUnidades
) VALUES (
    'COND003', 
    'Residencial Los Pinos', 
    'Condominio Residencial Los Pinos S.A. de C.V.',
    1, 1, 'Horizontal', 80
);

-- PASO 2: Asignar a administrador
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad)
SELECT @id_administrador, @nueva_entidad_id
WHERE NOT EXISTS (
    SELECT 1 FROM conf_usuario_entidades 
    WHERE IdUsuario = @id_administrador AND IdEntidad = @nueva_entidad_id
);

-- PASO 3: Copiar prompts de IA
INSERT INTO conf_ticket_prompts (
    IdEntidad, NombrePrompt, Descripcion, ContenidoPrompt, Activo, IdUsuarioCreacion
)
SELECT 
    @nueva_entidad_id,
    NombrePrompt,
    Descripcion,
    ContenidoPrompt,
    Activo,
    IdUsuarioCreacion
FROM conf_ticket_prompts
WHERE IdEntidad = @entidad_plantilla_id;

-- PASO 4: Copiar SLAs
INSERT INTO conf_ticket_sla (
    IdEntidad, Prioridad, TiempoRespuestaMinutos, TiempoResolucionMinutos, Activo
)
SELECT 
    @nueva_entidad_id,
    Prioridad,
    TiempoRespuestaMinutos,
    TiempoResolucionMinutos,
    Activo
FROM conf_ticket_sla
WHERE IdEntidad = @entidad_plantilla_id;

-- PASO 5: Copiar categorías de tickets
INSERT INTO cat_categorias_ticket (IdEntidad, Nombre, Descripcion, Activo)
SELECT @nueva_entidad_id, Nombre, Descripcion, Activo
FROM cat_categorias_ticket
WHERE IdEntidad = @entidad_plantilla_id;

-- PASO 6: Crear áreas comunes básicas
INSERT INTO cat_areas_comunes (IdEntidad, Nombre, Descripcion, Activo)
VALUES 
    (@nueva_entidad_id, 'Alberca', 'Área de alberca', 1),
    (@nueva_entidad_id, 'Salón de Eventos', 'Salón para eventos sociales', 1),
    (@nueva_entidad_id, 'Gimnasio', 'Área de gimnasio', 1);

-- PASO 7: Crear conceptos de cuota básicos
INSERT INTO cat_conceptos_cuota (IdEntidad, Nombre, Descripcion, Monto, Activo)
VALUES 
    (@nueva_entidad_id, 'Cuota de Mantenimiento', 'Cuota mensual', 1500.00, 1),
    (@nueva_entidad_id, 'Agua', 'Consumo de agua', 0.00, 1);

-- VERIFICACIÓN
SELECT 'Prompts copiados' AS Verificacion, COUNT(*) AS Total
FROM conf_ticket_prompts WHERE IdEntidad = @nueva_entidad_id
UNION ALL
SELECT 'SLAs configurados', COUNT(*)
FROM conf_ticket_sla WHERE IdEntidad = @nueva_entidad_id
UNION ALL
SELECT 'Categorías creadas', COUNT(*)
FROM cat_categorias_ticket WHERE IdEntidad = @nueva_entidad_id
UNION ALL
SELECT 'Áreas comunes', COUNT(*)
FROM cat_areas_comunes WHERE IdEntidad = @nueva_entidad_id
UNION ALL
SELECT 'Conceptos de cuota', COUNT(*)
FROM cat_conceptos_cuota WHERE IdEntidad = @nueva_entidad_id;
```

---

## 5. Verificación Post-Configuración

### 5.1 Checklist de Verificación

- [ ] **Entidad creada** en `cat_entidades` con `Activo = 1`
- [ ] **Relación usuario-entidad** existe en `conf_usuario_entidades`
- [ ] **Prompts de IA** copiados (mínimo 4 prompts activos)
- [ ] **SLAs configurados** para todas las prioridades
- [ ] **Categorías de tickets** disponibles (mínimo 3)
- [ ] **Áreas comunes** creadas (mínimo 1)
- [ ] **Conceptos de cuota** configurados (mínimo 1)

### 5.2 Script de Verificación

```sql
-- Verificar configuración completa de una entidad
SET @entidad_verificar = 3;

SELECT 
    'Entidad' AS Componente,
    CASE WHEN COUNT(*) > 0 THEN '✓ OK' ELSE '✗ FALTA' END AS Estado,
    COUNT(*) AS Cantidad
FROM cat_entidades 
WHERE Id = @entidad_verificar AND Activo = 1

UNION ALL

SELECT 
    'Administradores asignados',
    CASE WHEN COUNT(*) > 0 THEN '✓ OK' ELSE '✗ FALTA' END,
    COUNT(*)
FROM conf_usuario_entidades 
WHERE IdEntidad = @entidad_verificar

UNION ALL

SELECT 
    'Prompts de IA',
    CASE WHEN COUNT(*) >= 4 THEN '✓ OK' ELSE '⚠ INCOMPLETO' END,
    COUNT(*)
FROM conf_ticket_prompts 
WHERE IdEntidad = @entidad_verificar AND Activo = 1

UNION ALL

SELECT 
    'SLAs configurados',
    CASE WHEN COUNT(*) >= 4 THEN '✓ OK' ELSE '⚠ INCOMPLETO' END,
    COUNT(*)
FROM conf_ticket_sla 
WHERE IdEntidad = @entidad_verificar AND Activo = 1

UNION ALL

SELECT 
    'Categorías de tickets',
    CASE WHEN COUNT(*) > 0 THEN '✓ OK' ELSE '✗ FALTA' END,
    COUNT(*)
FROM cat_categorias_ticket 
WHERE IdEntidad = @entidad_verificar AND Activo = 1;
```

---

## 6. Problemas Comunes y Soluciones

### 6.1 Error: "Prompt 'ChatWebSistema' no encontrado"
**Causa:** Falta copiar los prompts a la nueva entidad  
**Solución:** Ejecutar el script de copia de prompts (Paso 3)

### 6.2 Dropdown de entidades no aparece
**Causa:** Usuario no tiene `TipoUsuario = 'AdministradorCondominios'`  
**Solución:**
```sql
UPDATE conf_usuarios 
SET TipoUsuario = 'AdministradorCondominios' 
WHERE Id = 5;
```

### 6.3 Chat widget funciona solo con entidad 1
**Causa:** Prompts no copiados a otras entidades  
**Solución:** Ejecutar script de copia de prompts para cada entidad

### 6.4 Administrador no ve la entidad en el dropdown
**Causa:** Falta registro en `conf_usuario_entidades`  
**Solución:**
```sql
INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad)
VALUES (5, 3);
```

---

## 7. Recomendaciones

### 7.1 Automatización
Se recomienda crear un **stored procedure** que ejecute todo el proceso:

```sql
DELIMITER $$

CREATE PROCEDURE sp_AltaEntidadCompleta(
    IN p_clave VARCHAR(50),
    IN p_alias VARCHAR(50),
    IN p_razon_social VARCHAR(255),
    IN p_id_administrador INT,
    IN p_entidad_plantilla INT
)
BEGIN
    DECLARE v_nueva_entidad_id INT;
    
    -- Crear entidad
    INSERT INTO cat_entidades (Clave, Alias, RazonSocial, Activo, EsCondominio)
    VALUES (p_clave, p_alias, p_razon_social, 1, 1);
    
    SET v_nueva_entidad_id = LAST_INSERT_ID();
    
    -- Asignar a administrador
    INSERT INTO conf_usuario_entidades (IdUsuario, IdEntidad)
    VALUES (p_id_administrador, v_nueva_entidad_id);
    
    -- Copiar prompts
    INSERT INTO conf_ticket_prompts (IdEntidad, NombrePrompt, Descripcion, ContenidoPrompt, Activo)
    SELECT v_nueva_entidad_id, NombrePrompt, Descripcion, ContenidoPrompt, Activo
    FROM conf_ticket_prompts
    WHERE IdEntidad = p_entidad_plantilla;
    
    -- Copiar SLAs
    INSERT INTO conf_ticket_sla (IdEntidad, Prioridad, TiempoRespuestaMinutos, TiempoResolucionMinutos, Activo)
    SELECT v_nueva_entidad_id, Prioridad, TiempoRespuestaMinutos, TiempoResolucionMinutos, Activo
    FROM conf_ticket_sla
    WHERE IdEntidad = p_entidad_plantilla;
    
    -- Retornar ID de nueva entidad
    SELECT v_nueva_entidad_id AS NuevaEntidadId;
END$$

DELIMITER ;

-- Uso:
CALL sp_AltaEntidadCompleta('COND004', 'Residencial Alameda', 'Condominio Residencial Alameda S.A.', 5, 1);
```

### 7.2 Auditoría
Mantener un log de todas las entidades creadas y sus configuraciones:

```sql
CREATE TABLE log_alta_entidades (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdEntidad INT NOT NULL,
    IdUsuarioCreacion INT,
    FechaAlta DATETIME DEFAULT CURRENT_TIMESTAMP,
    PromptsCopiados INT,
    SLAsCopiados INT,
    CategoriasCreadas INT,
    Estado ENUM('Completa', 'Incompleta', 'Error') DEFAULT 'Completa'
);
```

---

## 8. Conclusiones

El proceso de alta de entidades es **crítico** para el funcionamiento correcto del sistema multi-tenant. Cada entidad debe tener:

1. ✅ Registro en `cat_entidades`
2. ✅ Relación con administrador en `conf_usuario_entidades`
3. ✅ Prompts de IA completos en `conf_ticket_prompts`
4. ✅ SLAs configurados en `conf_ticket_sla`
5. ✅ Catálogos base poblados

**Sin estos elementos, la entidad NO funcionará correctamente** y los usuarios experimentarán errores 500 en el chat web y otros módulos.

---

## 9. Referencias

- Script de prompts iniciales: `JELA.API/insert-prompts-iniciales.sql`
- Documentación de sistema multi-entidad: `.kiro/specs/sistema-multi-entidad/`
- Tabla de entidades: `cat_entidades`
- Configuración de usuarios: `conf_usuarios`, `conf_usuario_entidades`

---

**Documento generado:** 21 de enero de 2026  
**Última actualización:** 21 de enero de 2026  
**Versión:** 1.0
