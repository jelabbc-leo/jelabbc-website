# Sprint 1: MVP Caché Semántico

**Duración:** 3 días  
**Prioridad:** Alta  
**Estado:** Pendiente  

## Objetivos
- Implementar caché semántico básico
- Integrar con Azure OpenAI existente
- Reducir 60% de llamadas a IA

## Tareas

### Día 1: Base de Datos y Servicios Base
- [ ] Crear tabla `conf_ia_cache_semantico`
- [ ] Implementar `ISemanticCacheService` interface
- [ ] Crear `SemanticCacheService` con métodos básicos
- [ ] Implementar generación de embeddings (Azure OpenAI)
- [ ] Crear método de búsqueda por similitud coseno

### Día 2: Integración con Azure OpenAI
- [ ] Modificar `AzureOpenAIService` existente
- [ ] Agregar lógica de consulta a caché antes de IA
- [ ] Implementar guardado automático en caché
- [ ] Crear método `GetOptimizedResponseAsync()`
- [ ] Actualizar endpoints de API para usar nuevo servicio

### Día 3: Pruebas y Ajustes
- [ ] Crear pruebas unitarias para `SemanticCacheService`
- [ ] Probar con 50 preguntas reales
- [ ] Ajustar umbral de similitud (85% inicial)
- [ ] Medir tiempos de respuesta y costos
- [ ] Documentar API changes

## Criterios de Aceptación
- ✅ Servicio responde desde caché cuando similitud >85%
- ✅ Nuevas respuestas se guardan automáticamente
- ✅ No hay regresión en funcionalidad existente
- ✅ Tiempos de respuesta <0.5s promedio
- ✅ Reducción de costos >50% en pruebas

## Riesgos
- Embeddings muy grandes afectan performance
- Umbral de similitud demasiado estricto/rígido

## Dependencias
- Azure OpenAI API key configurada
- Base de datos accesible