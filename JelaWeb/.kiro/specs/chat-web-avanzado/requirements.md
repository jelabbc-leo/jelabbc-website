# Requirements: Chat Web Avanzado - Asistente Inteligente Completo

**Feature Name:** chat-web-avanzado  
**Created:** 2026-01-19  
**Status:** Draft  
**Priority:** High

## 1. Overview

Transformar el Chat Web actual de JelaWeb (que actualmente solo crea tickets) en un **asistente inteligente completo** que permita a los usuarios realizar operaciones CRUD, consultas dinámicas, navegación y reportes mediante lenguaje natural.

### Current State
- ✅ Widget flotante operativo
- ✅ Integración con Azure OpenAI (GPT-4o-mini)
- ✅ Creación automática de tickets
- ✅ Detección de usuario autenticado
- ✅ Prompts configurables desde base de datos

### Target State
Un asistente inteligente que permita:
- Operaciones CRUD mediante lenguaje natural
- Consultas dinámicas con resultados visuales
- Navegación asistida por IA
- Reportes y análisis conversacionales
- Confirmaciones interactivas
- Historial persistente de conversaciones

## 2. User Stories

### 2.1 Operaciones CRUD

**US-001: Crear Entidades mediante Lenguaje Natural**
```
Como usuario autenticado
Quiero crear entidades (unidades, residentes, proveedores, etc.) usando lenguaje natural
Para agilizar el proceso de captura sin navegar por formularios
```

**Acceptance Criteria:**
- AC-001.1: El usuario puede decir "Dar de alta una unidad 101 con propietario Juan Pérez"
- AC-001.2: El sistema extrae los parámetros (número: 101, propietario: "Juan Pérez")
- AC-001.3: El sistema solicita confirmación antes de crear
- AC-001.4: El sistema valida permisos del usuario para la operación
- AC-001.5: El sistema crea la entidad y responde con mensaje de éxito
- AC-001.6: El sistema ofrece acciones relacionadas (agregar residentes, configurar cuotas)

**US-002: Actualizar Entidades mediante Lenguaje Natural**
```
Como usuario autenticado
Quiero actualizar información de entidades existentes usando lenguaje natural
Para hacer cambios rápidos sin buscar registros manualmente
```

**Acceptance Criteria:**
- AC-002.1: El usuario puede decir "Actualizar teléfono del residente de la unidad 303 a 5551234567"
- AC-002.2: El sistema identifica la entidad a actualizar
- AC-002.3: El sistema muestra los datos actuales y los cambios propuestos
- AC-002.4: El sistema solicita confirmación
- AC-002.5: El sistema actualiza y confirma el cambio

**US-003: Eliminar Entidades con Confirmación**
```
Como usuario autenticado
Quiero eliminar entidades usando lenguaje natural con confirmación explícita
Para mantener la integridad de los datos
```

**Acceptance Criteria:**
- AC-003.1: El usuario puede decir "Eliminar el proveedor ABC"
- AC-003.2: El sistema verifica dependencias (registros relacionados)
- AC-003.3: El sistema advierte sobre impacto de la eliminación
- AC-003.4: El sistema requiere confirmación explícita
- AC-003.5: El sistema registra la eliminación en auditoría

### 2.2 Consultas Dinámicas

**US-004: Consultar Estado de Cuenta**
```
Como residente o administrador
Quiero consultar el estado de cuenta de una unidad mediante lenguaje natural
Para conocer saldos y movimientos sin navegar por reportes
```

**Acceptance Criteria:**
- AC-004.1: El usuario puede decir "Muéstrame el estado de cuenta de la unidad 101"
- AC-004.2: El sistema muestra tabla con conceptos, montos, fechas y estados
- AC-004.3: El sistema calcula y muestra total pendiente
- AC-004.4: El sistema ofrece acciones: ver detalles, registrar pago, enviar por email
- AC-004.5: La tabla es interactiva (ordenar, filtrar)

**US-005: Consultar Tickets**
```
Como usuario autenticado
Quiero consultar mis tickets o tickets del sistema usando lenguaje natural
Para dar seguimiento sin navegar por grids
```

**Acceptance Criteria:**
- AC-005.1: El usuario puede decir "¿Cuántos tickets abiertos tengo?"
- AC-005.2: El sistema cuenta y muestra el número
- AC-005.3: El usuario puede decir "Muéstrame mis tickets pendientes"
- AC-005.4: El sistema muestra lista con ID, título, categoría, fecha
- AC-005.5: El usuario puede hacer clic en un ticket para ver detalles

**US-006: Consultas con Filtros Complejos**
```
Como administrador
Quiero hacer consultas con múltiples filtros usando lenguaje natural
Para obtener información específica sin escribir SQL
```

**Acceptance Criteria:**
- AC-006.1: El usuario puede decir "Lista de residentes morosos del último mes"
- AC-006.2: El sistema interpreta filtros (morosos, último mes)
- AC-006.3: El sistema ejecuta consulta parametrizada
- AC-006.4: El sistema muestra resultados en tabla
- AC-006.5: El sistema permite exportar a Excel

### 2.3 Navegación Asistida

**US-007: Navegar a Módulos**
```
Como usuario autenticado
Quiero navegar a diferentes módulos del sistema usando lenguaje natural
Para acceder rápidamente sin usar el menú
```

**Acceptance Criteria:**
- AC-007.1: El usuario puede decir "Abre la página de unidades"
- AC-007.2: El sistema identifica la URL correcta
- AC-007.3: El sistema ofrece abrir en nueva pestaña o en la actual
- AC-007.4: El sistema valida permisos antes de redirigir
- AC-007.5: El sistema mantiene el contexto del chat

**US-008: Navegación Contextual**
```
Como usuario autenticado
Quiero que el sistema sugiera navegación basada en el contexto
Para descubrir funcionalidades relacionadas
```

**Acceptance Criteria:**
- AC-008.1: Después de crear una unidad, el sistema sugiere "¿Quieres agregar residentes?"
- AC-008.2: El sistema ofrece botones de acción rápida
- AC-008.3: Los botones ejecutan acciones o navegan a páginas
- AC-008.4: El sistema aprende de las acciones frecuentes del usuario

### 2.4 Reportes y Análisis

**US-009: Generar Reportes**
```
Como administrador
Quiero generar reportes usando lenguaje natural
Para obtener análisis sin configurar reportes manualmente
```

**Acceptance Criteria:**
- AC-009.1: El usuario puede decir "Genera reporte de pagos del último mes"
- AC-009.2: El sistema ejecuta consulta agregada
- AC-009.3: El sistema muestra resultados en tabla y gráfica
- AC-009.4: El sistema permite exportar a PDF o Excel
- AC-009.5: El sistema ofrece enviar por email

**US-010: Análisis con Gráficas**
```
Com