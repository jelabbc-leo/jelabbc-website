# Scripts SQL - Módulo Condominios

## Descripción

Scripts SQL para crear la estructura de base de datos del módulo de administración de condominios del ecosistema JELABBC.

## Jerarquía del Sistema

```
cat_entidades (Nivel 1)
    └── Condominio/Fraccionamiento (datos fiscales)
        Ejemplo: "Los Robles S.C.", "Residencial Las Palmas"

cat_subentidades (Nivel 2) - YA EXISTE
    └── Secciones/Torres del fraccionamiento
        Ejemplo: "Robles 1", "Robles 2", "Torre A", "Torre B"
        NOTA: NO se necesita crear cat_secciones

cat_areas_comunes (Nivel 2/3) - NUEVO
    └── Áreas comunes reservables
        - SubEntidadId = NULL → Compartidas por todo el condominio
        - SubEntidadId = ID → Exclusivas de esa torre/sección
        Ejemplo: "Salón de Fiestas", "Alberca", "Gimnasio"

cat_unidades
    └── Unidades privativas de cada SubEntidad
        Ejemplo: "Depto 101", "Casa 15", "Local 3"
```

## Requisitos Previos

- MySQL 8.0+
- Base de datos: `jela_qa`
- Tablas existentes:
  - `cat_entidades` (con al menos un registro)
  - `cat_subentidades` (secciones/torres del condominio)
  - `cat_unidades`
  - `conf_usuarios`
  - `conf_opciones`

## Orden de Ejecución

Ejecutar los scripts en el siguiente orden:

| # | Archivo | Descripción |
|---|---------|-------------|
| 1 | `01_catalogos_base.sql` | Tablas de catálogos: `cat_areas_comunes`, residentes, conceptos de cuota |
| 2 | `02_cuotas_pagos.sql` | Tablas operativas: cuotas, pagos, detalle de pagos + triggers y vistas |
| 3 | `03_reservaciones_visitantes.sql` | Tablas operativas: reservaciones, visitantes, comunicados + vistas |
| 4 | `04_datos_iniciales.sql` | Datos de ejemplo, opciones de menú, stored procedures |

## Ejecución

### Opción 1: Script por script (recomendado)

```bash
mysql -u usuario -p jela_qa < 01_catalogos_base.sql
mysql -u usuario -p jela_qa < 02_cuotas_pagos.sql
mysql -u usuario -p jela_qa < 03_reservaciones_visitantes.sql
mysql -u usuario -p jela_qa < 04_datos_iniciales.sql
```

### Opción 2: MySQL Workbench

1. Abrir MySQL Workbench
2. Conectar a la base de datos `jela_qa`
3. Abrir cada archivo en orden y ejecutar (Ctrl+Shift+Enter)

## Objetos Creados

### Tablas (10)

**Catálogos:**
- `cat_areas_comunes` - Áreas comunes reservables (salones, albercas, gimnasios)
- `cat_residentes` - Propietarios e inquilinos
- `cat_conceptos_cuota` - Tipos de cuotas

**Operativas:**
- `op_cuotas` - Cuotas generadas
- `op_pagos` - Pagos registrados
- `op_pagos_detalle` - Aplicación de pagos
- `op_reservaciones` - Reservaciones de áreas
- `op_visitantes` - Control de acceso
- `op_comunicados` - Avisos
- `op_comunicados_lecturas` - Lecturas de comunicados

### Tabla: cat_areas_comunes

Esta tabla maneja las áreas comunes reservables del condominio:

| Campo | Descripción |
|-------|-------------|
| `EntidadId` | FK a cat_entidades (Condominio) |
| `SubEntidadId` | FK a cat_subentidades (NULL=compartida, ID=exclusiva de torre) |
| `TipoArea` | Salon, Alberca, Gimnasio, Jardin, Terraza, Asador, Ludoteca, SalaJuntas, etc. |

**Nota importante:** Las secciones/torres se manejan en `cat_subentidades` (tabla existente), NO en esta tabla.

### Vistas (4)

- `vw_estado_cuenta` - Estado de cuenta por unidad
- `vw_resumen_morosidad` - Reporte de morosidad
- `vw_calendario_reservaciones` - Calendario de reservaciones
- `vw_visitantes_activos` - Visitantes actualmente en el condominio

### Stored Procedures (3)

- `sp_GenerarCuotasMensuales(EntidadId, Periodo, UsuarioId)` - Genera cuotas mensuales
- `sp_AplicarRecargosMora(EntidadId, FechaCorte)` - Aplica recargos por mora
- `fn_GenerarFolio(Prefijo, Tabla)` - Genera folios únicos

### Triggers (2)

- `trg_pagos_detalle_after_insert` - Actualiza saldos al registrar pago
- `trg_pagos_detalle_after_delete` - Actualiza saldos al eliminar pago

## Verificación

Después de ejecutar los scripts, verificar con:

```sql
-- Verificar tablas
SELECT TABLE_NAME, TABLE_ROWS, TABLE_COMMENT
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'jela_qa' 
AND TABLE_NAME IN ('cat_areas_comunes', 'cat_residentes', 'op_cuotas', 'op_pagos');

-- Verificar vistas
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS 
WHERE TABLE_SCHEMA = 'jela_qa' AND TABLE_NAME LIKE 'vw_%';

-- Verificar stored procedures
SELECT ROUTINE_NAME FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_SCHEMA = 'jela_qa' AND ROUTINE_NAME LIKE 'sp_%';
```

## Notas Importantes

1. **Datos de ejemplo**: El script `04_datos_iniciales.sql` contiene datos de ejemplo comentados. Descomentar y ajustar `{ENTIDAD_ID}` según sea necesario.

2. **Menú de navegación**: Se insertan opciones en `conf_opciones` para el menú de Condominios.

3. **Modificación de cat_unidades**: El script `01_catalogos_base.sql` agrega columnas a la tabla existente `cat_unidades` de forma segura (verifica si existen antes de agregar).

4. **Secciones/Torres**: Se manejan en `cat_subentidades` (tabla existente). NO se crea `cat_secciones`.

5. **Áreas comunes exclusivas**: Usar `SubEntidadId` para indicar si un área es exclusiva de una torre/sección específica.

## Estándares Aplicados

- Nombres de tablas: `snake_case` con prefijos (`cat_`, `op_`, `conf_`, `log_`)
- Nombres de campos: `PascalCase`
- Charset: `utf8mb4`
- Engine: `InnoDB`

---

*Generado: 5 de enero de 2026*
