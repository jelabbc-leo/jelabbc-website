# Sprint 2: Patrones Comunes

**Duración:** 1 día  
**Prioridad:** Media  
**Estado:** Pendiente  

## Objetivos
- Implementar respuestas directas para preguntas comunes
- Mejorar velocidad a 0.01s para patrones exactos
- Reducir llamadas a IA en 10-20% adicional

## Tareas

### Implementación
- [ ] Crear tabla `conf_ia_patrones`
- [ ] Implementar `IPatternMatchingService` interface
- [ ] Crear `PatternMatchingService` con regex matching
- [ ] Definir 20-30 patrones iniciales:
  - Horarios de áreas comunes
  - Información de contacto
  - Reglamentos básicos
  - Preguntas frecuentes
- [ ] Integrar en flujo principal (antes de caché semántico)
- [ ] Crear endpoint de administración de patrones

## Patrones Iniciales Sugeridos
```
horario.*alberca → "La alberca está abierta de 8:00 AM a 8:00 PM"
horario.*gym → "El gimnasio está abierto 24/7 para residentes"
pago.*cuota → "Puedes pagar tu cuota en línea o en la administración"
reglamento.*mascotas → "Se permiten mascotas de hasta 15kg"
contacto.*administracion → "Tel: 555-1234, Email: admin@condominio.com"
```

## Criterios de Aceptación
- ✅ Respuestas directas funcionan para patrones definidos
- ✅ Tiempo de respuesta <0.01s
- ✅ No interfiere con caché semántico
- ✅ Endpoint de administración operativo

## Riesgos
- Patrones demasiado específicos no capturan variaciones
- Mantenimiento manual de patrones

## Dependencias
- Sprint 1 completado