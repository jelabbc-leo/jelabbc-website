# REQUIREMENTS - Sistema Multi-Entidad con Selector

**Fecha:** 20 de Enero de 2026  
**Versión:** 1.0  
**Estado:** Listo para Diseño  
**Prioridad:** Alta

---

## 1. RESUMEN EJECUTIVO

### 1.1 Objetivo

Implementar un sistema multi-entidad que permita a **Administradores de Condominios** gestionar múltiples condominios desde una única cuenta, mientras que **Usuarios Internos** (Mesa Directiva, Residentes, Empleados) solo accedan a su condominio específico.

### 1.2 Problema Actual

**Situación Actual:**
- El sistema guarda `IdEntidad` en sesión pero NO lo usa para filtrar datos
- Usuarios pueden ver datos de otras entidades (riesgo de seguridad)
- No hay distinción entre administradores externos y usuarios internos
- Formularios piden seleccionar entidad manualmente en cada operación
- No hay forma de cambiar de entidad sin hacer logout

**Impacto:**
- ❌ Falta de aislamiento de datos entre entidades
- ❌ Experiencia de usuario deficiente
- ❌ Riesgo de seguridad (acceso a datos de otras entidades)
- ❌ Proceso manual y repetitivo

### 1.3 Solución Propuesta

Implementar un sistema que:
- ✅ Identifique el tipo de usuario (Administrador vs Usuario Interno)
- ✅ Permita a administradores seleccionar entidad al login
- ✅ Proporcione dropdown para cambiar de entidad sin logout
- ✅ Filtre automáticamente todos los datos por entidad seleccionada
- ✅ Elimine campos de "Entidad" de formularios
- ✅ Garantice aislamiento completo de datos

---

## 2. STAKEHOLDERS

| Rol | Nombre | Responsabilidad |
|-----|--------|-----------------|
| Product Owner | [Nombre] | Aprobación de requerimientos |
| Tech Lead | [Nombre] | Arquitectura y diseño técnico |
| Backend Developer | [Nombre] | Implementación API .NET 8 |
| Frontend Developer | [Nombre] | Implementación VB.NET |
| QA Engineer | [Nombre] | Testing y validación |
| DBA | [Nombre] | Cambios en base de datos |

---

## 3. TIPOS DE USUARIO

### 3.1 Administrador de Condominios

**Descripción:** Profesional externo que administra múltiples condominios

**Características:**
- Puede tener acceso a 1 o más entidades (condominios)
- Necesita cambiar frecuentemente entre entidades
- Cobra por cada entidad que administra
- Puede dar de alta nuevas entidades

**Necesidades:**
- Selector visual de entidades al login
- Dropdown para cambiar de entidad rápidamente
- Ver claramente qué entidad está activa
- No tener que seleccionar entidad en cada formulario

### 3.2 Mesa Directiva

**Descripción:** Miembro de la mesa directiva del condominio

**Características:**
- Solo pertenece a UNA entidad
- Tiene permisos administrativos dentro de su entidad
- No necesita cambiar de entidad

**Necesidades:**
- Acceso directo al sistema (sin selector)
- Ver solo datos de su condominio
- No ver opciones de cambio de entidad

### 3.3 Residente

**Descripción:** Residente del condominio

**Características:**
- Solo pertenece a UNA entidad
- Permisos limitados (consulta principalmente)
- No necesita cambiar de entidad

**Necesidades:**
- Acceso directo al sistema
- Ver solo información de su condominio
- Interfaz simplificada

### 3.4 Empleado

**Descripción:** Empleado del condominio (guardia, mantenimiento, etc.)

**Características:**
- Solo pertenece a UNA entidad
- Permisos específicos según su rol
- No necesita cambiar de entidad

**Necesidades:**
- Acceso directo al sistema
- Ver solo datos relevantes a su trabajo
- Interfaz simplificada

---

## 4. USER STORIES

### 4.1 Como Administrador de Condominios

**US-001: Seleccionar Entidad al Login**
```
Como Administrador de Condominios
Quiero ver un selector visual de mis condominios al iniciar sesión
Para elegir con cuál quiero trabajar
```

**Criterios de Aceptación:**
- [ ] Al hacer login, se muestra página con tarjetas de entidades
- [ ] Cada tarjeta muestra: nombre, dirección, badge si es principal
- [ ] Al hacer clic en una tarjeta, se establece como entidad activa
- [ ] Sistema redirige a página de inicio con datos de esa entidad
- [ ] Si solo tiene una entidad, va directo al sistema (sin selector)

---

**US-002: Cambiar de Entidad sin Logout**
```
Como Administrador de Condominios
Quiero cambiar de entidad desde un dropdown en el master page
Para no tener que cerrar sesión y volver a entrar
```

**Criterios de Aceptación:**
- [ ] Dropdown visible en status bar del master page
- [ ] Muestra lista de entidades asignadas
- [ ] Entidad actual está seleccionada
- [ ] Al cambiar, recarga página con datos de nueva entidad
- [ ] Cambio se registra en logs de auditoría

---

**US-003: No Seleccionar Entidad en Formularios**
```
Como Administrador de Condominios
Quiero que el sistema use automáticamente la entidad seleccionada
Para no tener que elegirla en cada formulario
```

**Criterios de Aceptación:**
- [ ] Formularios NO muestran campo "Entidad"
- [ ] Sistema usa automáticamente `IdEntidadActual` de sesión
- [ ] Al crear registro, se asigna automáticamente la entidad
- [ ] Al consultar datos, se filtran automáticamente por entidad
- [ ] Indicador visual muestra qué entidad está activa

---

**US-004: Solicitar Alta de Nueva Entidad**
```
Como Administrador de Condominios
Quiero poder solicitar el alta de nuevos condominios que administro
Para expandir mi cartera de clientes
Siempre y cuando tenga licencias disponibles otorgadas por el administrador del sistema
```

**Criterios de Aceptación:**
- [ ] Botón "Agregar Nuevo Condominio" en selector de entidades
- [ ] Sistema valida que el usuario tenga licencias disponibles
- [ ] Si NO tiene licencias: Mostrar mensaje "No tiene licencias disponibles. Contacte al administrador"
- [ ] Si SÍ tiene licencias: Redirigir a página Entidades.aspx con parámetro ?modo=nuevo
- [ ] Página Entidades.aspx abre popup de alta automáticamente
- [ ] Usuario completa formulario existente de entidades
- [ ] Al guardar: Sistema consume una licencia disponible automáticamente
- [ ] Nueva entidad se asigna automáticamente al usuario en conf_usuario_entidades
- [ ] Nueva entidad se marca como activa
- [ ] Notificación al administrador del sistema (registro de nueva entidad en logs)
- [ ] Redirigir de vuelta a SelectorEntidades.aspx
- [ ] Mostrar mensaje de éxito con nombre de la nueva entidad

---

### 4.2 Como Usuario Interno (Mesa Directiva/Residente/Empleado)

**US-005: Acceso Directo al Sistema**
```
Como Usuario Interno
Quiero acceder directamente al sistema después del login
Para no tener que seleccionar mi condominio cada vez
```

**Criterios de Aceptación:**
- [ ] Al hacer login, va directo a página de inicio
- [ ] NO se muestra selector de entidades
- [ ] Sistema usa automáticamente su única entidad
- [ ] Solo ve datos de su condominio

---

**US-006: No Ver Opciones de Cambio de Entidad**
```
Como Usuario Interno
No quiero ver opciones de cambio de entidad
Para tener una interfaz más simple y clara
```

**Criterios de Aceptación:**
- [ ] Dropdown de entidades NO visible en master page
- [ ] Formularios NO muestran campo "Entidad"
- [ ] Interfaz limpia sin opciones innecesarias

---

### 4.3 Como Administrador del Sistema

**US-007: Asignar Tipo de Usuario**
```
Como Administrador del Sistema
Quiero poder asignar el tipo de usuario al crear cuentas
Para controlar el comportamiento del sistema
```

**Criterios de Aceptación:**
- [ ] Campo "Tipo de Usuario" en formulario de usuarios
- [ ] Opciones: Administrador Condominios, Mesa Directiva, Residente, Empleado
- [ ] Validación de tipo requerido
- [ ] Tipo determina flujo de login y permisos

---

**US-008: Asignar Entidades a Usuarios**
```
Como Administrador del Sistema
Quiero asignar una o más entidades a un usuario
Para controlar a qué condominios tiene acceso
```

**Criterios de Aceptación:**
- [ ] Interfaz para asignar entidades a usuario
- [ ] Puede asignar múltiples entidades a administradores
- [ ] Puede marcar una entidad como "principal"
- [ ] Validación: usuario interno solo puede tener una entidad
- [ ] Cambios se registran en auditoría

---

**US-009: Gestionar Licencias de Entidades**
```
Como Administrador del Sistema
Quiero asignar licencias de entidades a administradores de condominios
Para controlar cuántas entidades pueden dar de alta
```

**Criterios de Aceptación:**
- [ ] Campo "Licencias Disponibles" en perfil de usuario
- [ ] Interfaz para asignar/modificar número de licencias
- [ ] Al crear nueva entidad, se consume una licencia
- [ ] Sistema valida licencias disponibles antes de permitir alta
- [ ] Historial de consumo de licencias
- [ ] Notificación cuando licencias están por agotarse (< 2)
- [ ] Reporte de uso de licencias por usuario

---

## 5. REQUERIMIENTOS FUNCIONALES

### 5.1 Autenticación y Sesión

**RF-001: Identificar Tipo de Usuario**
- Sistema debe identificar el tipo de usuario al hacer login
- Tipos: AdministradorCondominios, MesaDirectiva, Residente, Empleado
- Tipo determina flujo post-login

**RF-002: Cargar Entidades del Usuario**
- Sistema debe obtener lista de entidades asignadas al usuario
- Incluir: Id, Nombre, Dirección, EsPrincipal
- Guardar en sesión para acceso rápido

**RF-003: Establecer Entidad Activa**
- Sistema debe mantener `IdEntidadActual` en sesión
- Debe persistir entre páginas
- Debe actualizarse al cambiar de entidad

---

### 5.2 Selector de Entidades

**RF-004: Mostrar Selector para Administradores**
- Mostrar selector solo si TipoUsuario = AdministradorCondominios
- Mostrar tarjetas visuales con información de cada entidad
- Permitir seleccionar una entidad

**RF-005: Omitir Selector para Usuarios Internos**
- Si TipoUsuario != AdministradorCondominios, ir directo al sistema
- Establecer automáticamente su única entidad como activa

**RF-006: Omitir Selector si Solo Tiene Una Entidad**
- Si administrador tiene solo 1 entidad, ir directo al sistema
- Establecer automáticamente esa entidad como activa

---

### 5.3 Cambio de Entidad

**RF-007: Dropdown en Master Page**
- Mostrar dropdown solo para administradores con múltiples entidades
- Listar todas las entidades asignadas
- Marcar entidad actual como seleccionada

**RF-008: Cambiar Entidad Activa**
- Al seleccionar entidad en dropdown, actualizar sesión
- Recargar página actual con datos de nueva entidad
- Registrar cambio en logs de auditoría

---

### 5.4 Filtrado Automático

**RF-009: Filtrar Consultas por Entidad**
- Todas las consultas SELECT deben filtrar por `IdEntidad = IdEntidadActual`
- Aplicar automáticamente en ApiConsumerCRUD
- No requerir código adicional en páginas

**RF-010: Agregar Entidad en Inserciones**
- Todas las inserciones deben incluir `IdEntidad = IdEntidadActual`
- Aplicar automáticamente en ApiConsumerCRUD
- No requerir código adicional en páginas

**RF-011: Validar Pertenencia en Actualizaciones**
- Antes de actualizar, validar que registro pertenezca a entidad actual
- Rechazar actualización si no pertenece
- Registrar intento en logs de seguridad

**RF-012: Validar Pertenencia en Eliminaciones**
- Antes de eliminar, validar que registro pertenezca a entidad actual
- Rechazar eliminación si no pertenece
- Registrar intento en logs de seguridad

---

### 5.5 Interfaz de Usuario

**RF-013: Eliminar Campo Entidad de Formularios**
- Remover campo "Entidad" de todos los formularios de captura
- Sistema usa automáticamente entidad de sesión
- Aplicar a: Cuotas, Unidades, Residentes, Conceptos, etc.

**RF-014: Indicador Visual de Entidad Activa**
- Mostrar nombre de entidad activa en master page
- Visible en todo momento
- Actualizar al cambiar de entidad

**RF-015: Breadcrumbs con Entidad**
- Incluir nombre de entidad en breadcrumbs (opcional)
- Ayuda a usuario a saber dónde está

---

### 5.6 Gestión de Entidades

**RF-016: Solicitud de Alta de Nueva Entidad**
- Administrador puede solicitar alta de nuevas entidades
- Sistema debe validar que tenga licencias disponibles
- Si no tiene licencias, mostrar mensaje y bloquear alta
- Si tiene licencias, redirigir a Entidades.aspx con parámetro ?modo=nuevo
- Página Entidades.aspx detecta parámetro y abre popup automáticamente
- Usuario usa formulario existente de catálogo de entidades
- Nueva entidad se asigna automáticamente al usuario
- Consumir una licencia al guardar entidad
- Registrar en logs la creación de entidad
- Redirigir de vuelta a SelectorEntidades.aspx

**RF-017: Validación de Licencias**
- Sistema debe validar licencias disponibles antes de permitir alta
- Mostrar contador de licencias disponibles en selector
- Bloquear botón "Agregar Condominio" si no hay licencias
- Notificar al usuario cuando licencias < 2

**RF-018: Asignación de Entidades**
- Admin del sistema puede asignar entidades a usuarios
- Puede asignar múltiples a administradores
- Solo una a usuarios internos
- Puede marcar una como principal

**RF-019: Gestión de Licencias**
- Admin del sistema puede asignar licencias a usuarios
- Campo "LicenciasDisponibles" en conf_usuarios
- Historial de consumo de licencias
- Reporte de uso de licencias

---

## 6. REQUERIMIENTOS NO FUNCIONALES

### 6.1 Seguridad

**RNF-001: Aislamiento de Datos**
- Garantizar que usuario solo vea datos de sus entidades asignadas
- Prevenir acceso a datos de otras entidades
- Validar pertenencia en todas las operaciones

**RNF-002: Auditoría**
- Registrar todos los cambios de entidad
- Registrar intentos de acceso no autorizado
- Logs deben incluir: usuario, entidad, acción, timestamp

**RNF-003: Validación de Sesión**
- Validar que entidad activa esté en lista de entidades asignadas
- Invalidar sesión si entidad no es válida
- Forzar re-login si sesión corrupta

---

### 6.2 Performance

**RNF-004: Tiempo de Respuesta**
- Cambio de entidad debe completarse en < 2 segundos
- Carga de selector debe ser < 1 segundo
- Filtrado automático no debe degradar performance

**RNF-005: Caché**
- Lista de entidades debe cachearse en sesión
- Evitar consultas repetidas a BD
- Invalidar caché al asignar/remover entidades

---

### 6.3 Usabilidad

**RNF-006: Experiencia de Usuario**
- Selector de entidades debe ser intuitivo y visual
- Dropdown debe ser fácil de encontrar y usar
- Indicador de entidad activa debe ser claro

**RNF-007: Accesibilidad**
- Selector debe ser accesible con teclado
- Dropdown debe cumplir estándares WCAG 2.1
- Indicadores visuales deben tener contraste adecuado

---

### 6.4 Mantenibilidad

**RNF-008: Código Reutilizable**
- Lógica de filtrado debe estar centralizada
- Evitar duplicación de código
- Usar helpers y servicios compartidos

**RNF-009: Documentación**
- Documentar flujos de autenticación
- Documentar helpers y servicios nuevos
- Crear guía de migración para páginas existentes

---

## 7. RESTRICCIONES Y SUPUESTOS

### 7.1 Restricciones

**Técnicas:**
- Debe integrarse con sistema existente sin romper funcionalidad
- Debe usar tecnologías actuales: .NET 8, VB.NET 4.8.1, MySQL
- Debe mantener compatibilidad con JWT existente

**Negocio:**
- No se puede cambiar modelo de cobro actual
- Debe implementarse en máximo 2 semanas
- Debe ser retrocompatible con usuarios existentes

**Operacionales:**
- Migración debe hacerse sin downtime
- Datos existentes deben migrarse automáticamente
- Rollback debe ser posible

---

### 7.2 Supuestos

- Tabla `conf_usuarios` puede modificarse
- Usuarios existentes pueden migrarse a tipo "Residente"
- Administradores de condominios se identificarán manualmente
- Cada entidad tiene un `IdEntidad` único
- Todas las tablas operativas tienen campo `IdEntidad`
- **Sistema de licencias se implementa en esta fase**
- **Administrador del sistema asigna licencias manualmente**
- **Una licencia = una entidad que puede dar de alta**

---

## 8. DEPENDENCIAS

### 8.1 Dependencias Técnicas

- Sistema de autenticación JWT existente
- ApiConsumerCRUD.vb
- SessionHelper.vb
- Master page (Jela.Master)
- Base de datos MySQL con tablas existentes

### 8.2 Dependencias de Negocio

- Definición de tipos de usuario
- Proceso de alta de nuevas entidades
- Modelo de cobro por entidad
- Políticas de asignación de entidades

---

## 9. CRITERIOS DE ÉXITO

### 9.1 Métricas de Éxito

| Métrica | Objetivo | Medición |
|---------|----------|----------|
| Aislamiento de datos | 100% | Auditoría de seguridad |
| Tiempo de cambio de entidad | < 2 seg | Performance testing |
| Satisfacción de usuario | > 4/5 | Encuesta post-implementación |
| Bugs críticos | 0 | Testing QA |
| Cobertura de testing | > 80% | Unit tests + integration tests |

### 9.2 Definición de "Done"

Una funcionalidad está "Done" cuando:
- [ ] Código implementado y revisado
- [ ] Unit tests escritos y pasando
- [ ] Integration tests pasando
- [ ] Documentación actualizada
- [ ] QA aprobado
- [ ] Desplegado en ambiente de pruebas
- [ ] Validado por Product Owner

---

## 10. RIESGOS

| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|--------------|---------|------------|
| Migración de datos falla | Media | Alto | Backup completo + rollback plan |
| Performance degradada | Baja | Medio | Testing de carga + índices en BD |
| Usuarios confundidos | Media | Medio | Capacitación + documentación |
| Bugs en producción | Media | Alto | Testing exhaustivo + despliegue gradual |
| Incompatibilidad con páginas existentes | Alta | Alto | Actualización gradual + testing |

---

## 11. PLAN DE ROLLOUT

### 11.1 Fase 1: Piloto (Semana 1)
- Implementar en ambiente de desarrollo
- Testing interno con equipo
- Ajustes según feedback

### 11.2 Fase 2: Beta (Semana 2)
- Desplegar en ambiente de staging
- Invitar a 5-10 usuarios beta
- Recolectar feedback
- Corrección de bugs

### 11.3 Fase 3: Producción (Semana 3)
- Desplegar en producción
- Monitoreo intensivo primeras 48 horas
- Soporte dedicado primera semana
- Capacitación a usuarios

---

## 12. APROBACIONES

| Rol | Nombre | Firma | Fecha |
|-----|--------|-------|-------|
| Product Owner | | | |
| Tech Lead | | | |
| QA Lead | | | |
| Stakeholder | | | |

---

**Última Actualización:** 20 de Enero de 2026  
**Próxima Revisión:** Después de aprobación de diseño
