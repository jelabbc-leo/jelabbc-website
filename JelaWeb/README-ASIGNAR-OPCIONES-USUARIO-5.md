# Asignar Todas las Opciones al Usuario ID 5

## Descripción

Scripts SQL para asignar **todas las opciones del sistema** al usuario ID 5 (jlsg.gdl@gmail.com) y hacer que se muestre el **ribbon completo** con todas las pestañas y grupos.

## Archivos Generados

### 1. `asignar-todas-opciones-usuario-5.sql` (RECOMENDADO)
Script completo con:
- ✅ Variables para mejor control
- ✅ Verificaciones paso a paso
- ✅ Mensajes informativos
- ✅ Resumen detallado al final
- ✅ Información del ribbon organizada

### 2. `asignar-todas-opciones-usuario-5-simple.sql` (ALTERNATIVO)
Script simplificado sin variables:
- ✅ Más directo y simple
- ✅ Sin uso de variables MySQL
- ✅ Ideal si el primero da problemas

## Estructura de Permisos en JELA

```
conf_usuarios (Usuario)
    ↓
conf_usuarioroles (Relación Usuario-Rol)
    ↓
conf_roles (Rol)
    ↓
conf_rolopciones (Relación Rol-Opción)
    ↓
conf_opciones (Opciones del menú/ribbon)
```

## ¿Qué hace el script?

### Paso 1: Crear Rol "Administrador Total"
Crea un rol especial con acceso completo al sistema.

### Paso 2: Asignar Rol al Usuario 5
Vincula el usuario ID 5 con el rol de Administrador Total.

### Paso 3: Asignar TODAS las Opciones
Asigna todas las opciones activas del sistema al rol.

### Paso 4: Verificación
Muestra las opciones asignadas y el resumen.

## Cómo Ejecutar

### Opción 1: MySQL Workbench
1. Abrir MySQL Workbench
2. Conectar a la base de datos `jela_qa`
3. Abrir el archivo `asignar-todas-opciones-usuario-5.sql`
4. Ejecutar (Ctrl+Shift+Enter o botón ⚡)

### Opción 2: Línea de Comandos
```bash
mysql -u root -p jela_qa < asignar-todas-opciones-usuario-5.sql
```

### Opción 3: phpMyAdmin
1. Seleccionar base de datos `jela_qa`
2. Ir a pestaña "SQL"
3. Copiar y pegar el contenido del script
4. Ejecutar

## Resultado Esperado

### Antes:
```
Usuario ID 5:
- Sin opciones en el menú
- Ribbon vacío o con pocas opciones
```

### Después:
```
Usuario ID 5:
✓ Rol: Administrador Total
✓ Opciones: TODAS las opciones activas del sistema
✓ Ribbon: Todas las pestañas y grupos visibles
✓ Acceso completo al sistema
```

## Verificación Post-Ejecución

### 1. Verificar en Base de Datos

```sql
-- Ver rol asignado
SELECT 
    u.Id,
    u.Nombre,
    u.Email,
    r.Nombre AS Rol
FROM conf_usuarios u
INNER JOIN conf_usuarioroles ur ON u.Id = ur.UsuarioId
INNER JOIN conf_roles r ON ur.RolId = r.Id
WHERE u.Id = 5;

-- Contar opciones
SELECT COUNT(*) AS TotalOpciones
FROM conf_opciones o
INNER JOIN conf_rolopciones ro ON o.Id = ro.OpcionId
INNER JOIN conf_usuarioroles ur ON ro.RolId = ur.RolId
WHERE ur.UsuarioId = 5 AND o.Activo = 1;
```

### 2. Verificar en la Aplicación

1. **Cerrar sesión** si está abierta
2. **Login** con usuario ID 5 (jlsg.gdl@gmail.com)
3. **Seleccionar entidad** en el selector
4. **Verificar ribbon**: Debe mostrar todas las pestañas:
   - Catálogos
   - Operación
   - Configuración
   - IOT
   - Reportes
   - etc.

## Estructura del Ribbon

El ribbon se organiza por:

### RibbonTab (Pestañas principales)
- Catálogos
- Operación
- Configuración
- IOT
- Reportes

### RibbonGroup (Grupos dentro de cada pestaña)
- Maestros
- Residentes
- Documentos
- Tickets
- etc.

### Opciones (Botones dentro de cada grupo)
- Entidades
- Unidades
- Residentes
- Cuotas
- Pagos
- etc.

## Tablas Involucradas

### `conf_usuarios`
Tabla de usuarios del sistema.

### `conf_roles`
Roles disponibles (Administrador, MesaDirectiva, etc.).

### `conf_usuarioroles`
Relación muchos-a-muchos entre usuarios y roles.

### `conf_opciones`
Opciones del menú/ribbon con información de:
- Nombre
- URL
- Icono
- RibbonTab
- RibbonGroup
- Orden

### `conf_rolopciones`
Relación muchos-a-muchos entre roles y opciones.

## Campos Importantes

### En `conf_opciones`:
- **RibbonTab**: Pestaña del ribbon (ej: "Catálogos")
- **RibbonGroup**: Grupo dentro de la pestaña (ej: "Maestros")
- **OrdenTab**: Orden de la pestaña (1, 2, 3...)
- **OrdenGrupo**: Orden del grupo dentro de la pestaña
- **OrdenOpcion**: Orden de la opción dentro del grupo
- **Activo**: 1 = visible, 0 = oculta

## Troubleshooting

### Problema: "El ribbon sigue vacío"

**Solución:**
1. Verificar que el script se ejecutó sin errores
2. Cerrar sesión completamente
3. Limpiar caché del navegador (Ctrl+Shift+Del)
4. Login nuevamente
5. Verificar en base de datos con las queries de verificación

### Problema: "Solo veo algunas opciones"

**Solución:**
1. Verificar que las opciones tienen `Activo = 1`
2. Verificar que `conf_rolopciones.Activo = 1`
3. Verificar que `conf_usuarioroles.Activo = 1`

### Problema: "Error al ejecutar el script"

**Solución:**
1. Usar el script simple: `asignar-todas-opciones-usuario-5-simple.sql`
2. Verificar que la base de datos es `jela_qa`
3. Verificar permisos de usuario MySQL

## Rollback (Deshacer)

Si necesitas revertir los cambios:

```sql
-- Eliminar rol del usuario 5
DELETE FROM conf_usuarioroles WHERE UsuarioId = 5;

-- Eliminar rol de Administrador Total
DELETE FROM conf_rolopciones 
WHERE RolId = (SELECT Id FROM conf_roles WHERE Nombre = 'Administrador Total');

DELETE FROM conf_roles WHERE Nombre = 'Administrador Total';
```

## Notas Importantes

⚠️ **Seguridad**: Este script da acceso TOTAL al sistema. Solo usar en desarrollo/testing.

⚠️ **Producción**: En producción, asignar solo las opciones necesarias según el rol del usuario.

✅ **Testing**: Ideal para desarrollo y pruebas donde necesitas acceso completo.

✅ **Reversible**: Los cambios se pueden revertir fácilmente.

## Resumen de Ejecución

```
1. Ejecutar script SQL ✓
2. Cerrar sesión en la app ✓
3. Login con usuario ID 5 ✓
4. Seleccionar entidad ✓
5. Verificar ribbon completo ✓
```

## Soporte

Si tienes problemas:
1. Revisar logs de MySQL
2. Verificar queries de verificación
3. Revisar logs de la aplicación en `JelaWeb/App_Data/Logs/`

---

**Fecha de creación**: 2026-01-20  
**Base de datos**: jela_qa  
**Usuario afectado**: ID 5 (jlsg.gdl@gmail.com)  
**Tipo de cambio**: Asignación de permisos completos
