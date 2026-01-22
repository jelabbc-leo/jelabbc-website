# Script para Eliminar Comentarios HTML Problemáticos

## Archivos Afectados

Los siguientes archivos tienen comentarios HTML dentro de controles DevExpress que causan errores de compilación:

1. `JelaWeb/Views/Catalogos/AreasComunes.aspx` - Línea 71
2. `JelaWeb/Views/Catalogos/Residentes.aspx` - Línea 70
3. `JelaWeb/Views/Catalogos/Unidades.aspx` - Línea 216
4. `JelaWeb/Views/Operacion/Condominios/Comunicados.aspx` - Línea 68
5. `JelaWeb/Views/Operacion/Condominios/EstadoCuenta.aspx` - Línea 17
6. `JelaWeb/Views/Operacion/Condominios/Reservaciones.aspx` - Línea 107
7. `JelaWeb/Views/Operacion/Condominios/Cuotas.aspx` - Líneas 113 y 250
8. `JelaWeb/Views/Operacion/Condominios/Pagos.aspx` - Línea 117

## Comentario a Eliminar

```html
<!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
```

## Solución

Eliminar completamente el comentario de todos los archivos. Los comentarios HTML no están permitidos dentro de `<Items>` de `ASPxFormLayout`.

## Patrón de Búsqueda y Reemplazo

**Buscar:**
```
<!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
```

**Reemplazar con:** (vacío - eliminar la línea)

## Verificación

Después de eliminar los comentarios:
1. Clean + Rebuild del proyecto
2. Verificar que no haya errores de compilación
3. Probar navegación a cada una de las páginas afectadas
