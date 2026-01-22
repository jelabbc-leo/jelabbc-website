# Sprint 3: Sistema de Vigencia

**Duración:** 2 días  
**Prioridad:** Media  
**Estado:** Pendiente  

## Objetivos
- Implementar expiración automática de caché
- Prevenir respuestas obsoletas
- Mantener caché limpio y eficiente

## Tareas

### Día 1: Vigencia por Categoría
- [ ] Agregar campos `VigenciaHasta` y `Categoria` a tabla caché
- [ ] Crear categorías de vigencia:
  - Horarios: 30 días
  - Reglamentos: 90 días
  - Información general: Sin expiración
  - Precios/tarifas: 15 días
- [ ] Modificar `SemanticCacheService` para respetar vigencia
- [ ] Crear método `InvalidateExpiredCache()`

### Día 2: Limpieza Automática
- [ ] Implementar job programado para limpieza diaria
- [ ] Crear stored procedure para eliminar entradas expiradas
- [ ] Agregar logging de limpieza
- [ ] Crear endpoint manual para invalidación
- [ ] Actualizar métricas para incluir tasa de expiración

## Criterios de Aceptación
- ✅ Entradas expiradas no se retornan
- ✅ Limpieza automática funciona diariamente
- ✅ Endpoint de invalidación manual operativo
- ✅ No afecta performance de consultas

## Riesgos
- Limpieza demasiado agresiva elimina datos útiles
- Job programado falla silenciosamente

## Dependencias
- Sprint 1 completado
- Sistema de jobs programados disponible