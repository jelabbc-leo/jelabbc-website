# Documento de Requerimientos - Ecosistema JELABBC

## Introducción

El **Ecosistema JELABBC** es una plataforma integral de gestión empresarial multi-industria que integra Inteligencia Artificial (IA), Internet de las Cosas (IoT) y automatización de procesos para transformar digitalmente las operaciones de condominios, empresas agrícolas, servicios municipales y proveedores de mantenimiento.

Este documento se enfoca en los requerimientos del **frontend web** y **aplicaciones móviles**, considerando que el backend ya está desarrollado y operativo. El frontend actual (ASP.NET Web Forms con VB.NET y DevExpress) tiene implementadas funcionalidades básicas de autenticación, gestión de entidades, conceptos y captura de documentos.

### Stack Tecnológico

**Backend Existente:**
- API REST operativa en Azure: `https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net`
- Lenguaje: Visual Basic .NET (VB.NET)
- Framework: ASP.NET Web Application (.NET Framework 4.8)
- Base de datos: MySQL 8.0+ (jela_qa)
- Patrón: CrudController dinámico + SqlHelpers

**Frontend Web:**
- ASP.NET Web Forms (.NET Framework 4.8.1) con VB.NET
- DevExpress ASP.NET 22.2 para componentes UI
- Bootstrap 5 para diseño responsivo
- jQuery + JavaScript modular

**Aplicaciones Móviles:**
- .NET MAUI (multiplataforma iOS/Android) - Recomendado
- Alternativa: Desarrollo nativo (Swift/Kotlin)
- Modo offline con SQLite
- Sincronización bidireccional con API REST

**Inteligencia Artificial:**
- Proveedor: Azure OpenAI Service (GPT-4)
- Funcionalidades: Análisis de tickets, clasificación automática, agente de voz

**IoT:**
- Plataforma: Azure IoT Hub
- Protocolos: MQTT, HTTPS
- Dispositivos: Sensores de humedad, temperatura, pH

**Orquestación:**
- n8n para workflows de automatización

### Estándares de UI

**IMPORTANTE**: Todos los requerimientos de UI del portal web deben implementarse siguiendo los estándares documentados en `ui-standards.md`. Estos estándares definen reglas obligatorias para:

- Configuración de ASPxGridView (toolbar, filtros, paginación)
- Uso de ASPxPopupControl para captura de datos
- Separación de CSS/JS en archivos externos (NUNCA inline)
- Nomenclatura contextual de botones
- Nomenclatura de tablas con guion bajo (op_tickets, cat_conceptos)
- Accesibilidad y navegación por teclado
- Validaciones y notificaciones con Toastr
- Clase FuncionesGridWeb.vb para estandarización

**Ver**: [Estándares de UI](./ui-standards.md) para detalles completos.

### API CRUD Dinámica

El sistema usa un **CrudController genérico** que maneja todas las operaciones CRUD para cualquier tabla:

```
GET /api/CRUD?strQuery=SELECT * FROM tabla WHERE condicion
POST /api/CRUD/{tabla}
PUT /api/CRUD/{tabla}/{id}
DELETE /api/CRUD?table={tabla}&idField={campo}&idValue={valor}
```

**Ventajas:**
- Sin necesidad de crear controladores específicos por cada tabla
- Consultas SQL parametrizadas (prevención de SQL injection)
- Tipado automático de datos desde MySQL
- Respuestas JSON estructuradas con metadatos de tipo

## Glosario

- **Sistema**: El Ecosistema JELABBC completo (frontend web + apps móviles + backend + IA)
- **Portal_Web**: Aplicación web ASP.NET para administración completa del sistema
- **App_Móvil**: Aplicaciones nativas iOS/Android para usuarios finales
- **Usuario**: Cualquier persona que interactúa con el sistema (administrador, residente, técnico, productor)
- **Entidad**: Organización cliente del sistema (condominio, empresa agrícola, municipio, proveedor) - Nivel 1 jerárquico
- **SubEntidad**: División o departamento dentro de una entidad - Nivel 2 jerárquico
- **Proveedor**: Empresa externa que provee servicios - Nivel 3 jerárquico
- **Colaborador**: Trabajador de campo del proveedor - Nivel 4 jerárquico
- **Ticket**: Solicitud de servicio o soporte registrada en el sistema
- **Orden_de_Compra**: Documento que autoriza la adquisición de bienes o servicios (OC)
- **Orden_de_Trabajo**: Asignación de tarea a un técnico o proveedor
- **Dictamen_Técnico**: Documento que valida o rechaza una propuesta técnica
- **Agente_IA**: Sistema de inteligencia artificial que asiste en validación y automatización
- **Agente_de_Voz**: Sistema de IA conversacional disponible 24/7 por teléfono
- **IoT**: Dispositivos conectados que envían datos en tiempo real (sensores, actuadores)
- **N8N**: Plataforma de automatización de flujos de trabajo
- **API_REST**: Interfaz de programación de aplicaciones del backend
- **CrudController**: Controlador genérico que maneja operaciones CRUD dinámicas
- **Residente**: Usuario que vive en un condominio gestionado por el sistema
- **Técnico**: Usuario que realiza trabajos de mantenimiento
- **Productor_Agrícola**: Usuario que gestiona operaciones agrícolas
- **KPI**: Indicador clave de rendimiento para medir eficiencia
- **CSAT**: Customer Satisfaction Score - Puntuación de satisfacción del cliente
- **Formulario_Dinámico**: Estructura configurable de campos para captura de información en múltiples plataformas
- **Fallo_Formulario**: Asignación de un formulario dinámico a un documento de fallo (relación parent-child)
- **Respuesta_Formulario**: Instancia de captura de datos de un formulario por un usuario
- **Azure_Form_Recognizer**: Servicio de Azure para extracción automática de campos desde documentos PDF
- **SQLite_Offline**: Base de datos local en dispositivos móviles para funcionamiento sin conexión

## Requerimientos

### Requerimiento 1: Autenticación y Gestión de Sesiones

**Historia de Usuario:** Como usuario del sistema, quiero autenticarme de forma segura mediante credenciales o biometría y mantener mi sesión activa, para poder acceder a las funcionalidades según mis permisos.

#### Criterios de Aceptación

1. CUANDO un usuario ingresa credenciales válidas en el Portal_Web, EL Sistema DEBERÁ autenticar al usuario mediante la API_REST y crear una sesión segura con token JWT
2. CUANDO un usuario activa autenticación biométrica en dispositivos compatibles, EL Sistema DEBERÁ permitir login mediante huella digital o reconocimiento facial
3. CUANDO un usuario ingresa credenciales inválidas, EL Sistema DEBERÁ mostrar un mensaje de error claro y registrar el intento fallido
4. CUANDO la sesión de un usuario expira por inactividad (30 minutos), EL Sistema DEBERÁ redirigir al usuario a la página de login y mostrar un mensaje informativo
5. CUANDO un usuario cierra sesión, EL Sistema DEBERÁ invalidar el token JWT y limpiar todos los datos sensibles del navegador
6. MIENTRAS un usuario tiene una sesión activa, EL Sistema DEBERÁ validar los permisos en cada operación mediante el rol asignado

### Requerimiento 2: Portal Web - Dashboard Principal

**Historia de Usuario:** Como usuario autenticado, quiero ver un dashboard personalizado según mi rol, para acceder rápidamente a las funciones más relevantes.

#### Criterios de Aceptación

1. CUANDO un usuario accede al dashboard, EL Sistema DEBERÁ mostrar widgets personalizados según el rol del usuario
2. CUANDO un administrador accede al dashboard, EL Sistema DEBERÁ mostrar métricas clave (tickets abiertos, órdenes pendientes, alertas IoT, KPIs de OC)
3. CUANDO un residente accede al dashboard, EL Sistema DEBERÁ mostrar su saldo, tickets activos y avisos del condominio
4. CUANDO un técnico accede al dashboard, EL Sistema DEBERÁ mostrar sus órdenes de trabajo asignadas y pendientes
5. CUANDO se actualiza información relevante, EL Sistema DEBERÁ refrescar el dashboard automáticamente sin recargar la página

### Requerimiento 3: Portal Web - Gestión de Entidades y SubEntidades

**Historia de Usuario:** Como administrador del sistema, quiero gestionar entidades y subentidades, para organizar la estructura jerárquica de mis clientes.

#### Criterios de Aceptación

1. CUANDO un administrador crea una nueva entidad, EL Sistema DEBERÁ validar los datos obligatorios (TipoEntidad, Nombre, Email) y guardar mediante POST /api/CRUD/cat_entidades
2. CUANDO un administrador edita una entidad existente, EL Sistema DEBERÁ cargar los datos actuales en un ASPxPopupControl y permitir su modificación
3. CUANDO un administrador elimina una entidad, EL Sistema DEBERÁ solicitar confirmación y verificar que no tenga dependencias activas (usuarios, documentos)
4. CUANDO un administrador visualiza la lista de entidades, EL Sistema DEBERÁ mostrar un ASPxGridView con toolbar estándar, filtros en cabecera y sin paginación (PageSize=-1)
5. CUANDO un administrador crea una subentidad, EL Sistema DEBERÁ asociarla correctamente a su entidad padre mediante IdEntidad

### Requerimiento 4: Portal Web - Gestión de Usuarios y Roles

**Historia de Usuario:** Como administrador de entidad, quiero gestionar usuarios y asignar roles, para controlar el acceso a las funcionalidades del sistema.

#### Criterios de Aceptación

1. CUANDO un administrador crea un nuevo usuario, EL Sistema DEBERÁ validar que el email sea único en conf_usuarios y enviar credenciales de acceso
2. CUANDO un administrador asigna un rol a un usuario, EL Sistema DEBERÁ aplicar los permisos correspondientes de conf_roles inmediatamente
3. CUANDO un administrador desactiva un usuario, EL Sistema DEBERÁ cerrar todas sus sesiones activas y marcar Activo=false
4. CUANDO un usuario intenta acceder a una función sin permisos, EL Sistema DEBERÁ mostrar un mensaje de acceso denegado
5. MIENTRAS un usuario tiene sesión activa, EL Sistema DEBERÁ validar sus permisos en cada operación

### Requerimiento 5: Portal Web - Módulo de Tickets Tipo Klarna

**Historia de Usuario:** Como usuario del sistema, quiero crear y dar seguimiento a tickets de soporte con procesamiento automático de IA, para resolver problemas eficientemente con 66% de automatización.

#### Criterios de Aceptación

1. CUANDO un usuario crea un ticket, EL Sistema DEBERÁ capturar descripción, canal de entrada y adjuntos, y enviar a procesamiento IA automáticamente
2. CUANDO se crea un ticket, EL Agente_IA DEBERÁ generar automáticamente: ResumenIA, AsuntoCorto, Categoría, Subcategoría, SentimientoDetectado, PrioridadAsignada y Urgencia
3. CUANDO el Agente_IA puede resolver el ticket automáticamente, EL Sistema DEBERÁ responder al cliente directamente y marcar ResueltoporIA=true
4. CUANDO el Agente_IA no puede resolver el ticket, EL Sistema DEBERÁ asignar a un agente humano según prioridad, categoría y sentimiento
5. CUANDO un técnico actualiza el estado de un ticket, EL Sistema DEBERÁ registrar el cambio en op_ticket_historial y notificar al solicitante
6. CUANDO un usuario consulta sus tickets, EL Sistema DEBERÁ mostrar un ASPxGridView con filtro de fechas arriba y filtros tipo Excel en cabeceras
7. CUANDO un ticket se cierra, EL Sistema DEBERÁ solicitar calificación CSAT (1-5 estrellas) al solicitante
8. MIENTRAS un ticket está activo, EL Sistema DEBERÁ mantener el historial completo de conversación en op_ticket_conversacion

### Requerimiento 6: Portal Web - Módulo de Órdenes de Compra con KPIs

**Historia de Usuario:** Como funcionario de Entidad, quiero gestionar órdenes de compra con seguimiento de tiempos y KPIs, para medir eficiencia y prevenir retrasos.

#### Criterios de Aceptación

1. CUANDO una Entidad genera una OC para SubEntidad, EL Sistema DEBERÁ crear la OC en op_documentos con FechaInicio, FechaFin y estado "OC Nueva"
2. CUANDO una OC se crea, EL Sistema DEBERÁ iniciar el conteo de tiempo de ejecución para generar KPIs
3. CUANDO una OC permanece en estado "OC Nueva" por más del tiempo configurado, EL Sistema DEBERÁ enviar alertas vía WhatsApp a los usuarios asignados (configurable: 3 días, 6 días)
4. CUANDO una SubEntidad crea un dictamen nuevo, EL Sistema DEBERÁ vincularlo a la OC correspondiente mediante DocumentoRelacionado y notificar a la Entidad
5. CUANDO una Entidad aprueba un dictamen, EL Sistema DEBERÁ cambiar el estado de la OC a "En Proceso de Pago" y asignar folio de pago
6. CUANDO una Entidad rechaza un dictamen, EL Sistema DEBERÁ devolverlo a SubEntidad con comentarios de rechazo en el campo de chat compartido
7. CUANDO una OC se paga, EL Sistema DEBERÁ cambiar el estado a "OC Pagada" y registrar el tiempo total de ejecución para KPIs
8. CUANDO se consulta el módulo de KPIs, EL Sistema DEBERÁ mostrar tiempos de ejecución por fase y porcentaje de cumplimiento (tiempo real vs estimado)

### Requerimiento 7: Portal Web - Módulo de Servicios Municipales - Gestión de Licitaciones y Fallos

**Historia de Usuario:** Como funcionario municipal (Entidad), quiero gestionar licitaciones, fallos y órdenes de compra con flujo multinivel, para controlar el proceso completo desde la licitación hasta la ejecución en campo.

#### Criterios de Aceptación

1. CUANDO un funcionario de Entidad crea un documento de fallo, EL Sistema DEBERÁ mostrar un ASPxPopupControl con dos grupos: cabecera arriba y ASPxPageControl con pestañas (Colonias/Secciones, Conceptos) abajo
2. CUANDO un funcionario captura secciones en el grid de Colonias, EL Sistema DEBERÁ permitir ingresar Clave de colonia, buscar automáticamente en cat_colonias, y llenar Id, Nombre; luego posicionar cursor en MontoMinimo para captura manual
3. CUANDO un funcionario captura conceptos en el grid de Conceptos, EL Sistema DEBERÁ permitir ingresar Clave de concepto, buscar automáticamente en cat_conceptos, y llenar Id, Descripción, CostoUnitario; luego posicionar cursor en CantidadPedida
4. CUANDO un funcionario presiona Enter o Flecha Abajo en la última columna de captura, EL Sistema DEBERÁ agregar automáticamente una nueva fila vacía para continuar captura
5. CUANDO un funcionario guarda el documento, EL Sistema DEBERÁ guardar cabecera en op_documentos, colonias en op_documentos_colonias, y conceptos en op_documentos_detalle usando API CRUD dinámica
6. CUANDO un funcionario de Entidad asigna un documento a SubEntidad, EL Sistema DEBERÁ cambiar el estado a "Asignado a SubEntidad", registrar en op_documento_historial y notificar al responsable

### Requerimiento 8: Portal Web - Flujo Multinivel de Documentos

**Historia de Usuario:** Como usuario de un nivel jerárquico específico, quiero ver únicamente los documentos asignados a mi nivel y los montos capturados por el nivel anterior, para mantener la privacidad y seguridad de la información.

#### Criterios de Aceptación

1. CUANDO un usuario de SubEntidad (nivel 2) consulta un documento, EL Sistema DEBERÁ mostrar únicamente MontoMinimo_Entidad y MontoMaximo_Entidad sin mostrar montos de niveles posteriores
2. CUANDO un usuario de Proveedor (nivel 3) consulta un documento, EL Sistema DEBERÁ mostrar únicamente MontoMinimo_Subentidad y MontoMaximo_Subentidad sin mostrar montos de Entidad ni Colaborador
3. CUANDO un usuario de Colaborador (nivel 4) consulta un documento, EL Sistema DEBERÁ mostrar únicamente MontoMinimo_Proveedor y MontoMaximo_Proveedor sin mostrar montos de niveles anteriores
4. CUANDO un usuario consulta la lista de documentos, EL Sistema DEBERÁ mostrar únicamente los documentos asignados a su nivel jerárquico o creados por él
5. CUANDO un usuario intenta acceder a un documento no asignado a su nivel, EL Sistema DEBERÁ denegar el acceso y registrar el intento en log de seguridad
6. MIENTRAS un usuario visualiza partidas de un documento, EL Sistema DEBERÁ ocultar las columnas de montos de niveles a los que no tiene acceso
7. CUANDO un administrador de Entidad (nivel 1) consulta documentos, EL Sistema DEBERÁ mostrar toda la cadena jerárquica y todos los montos para trazabilidad completa

### Requerimiento 9: Portal Web - Chat Integrado en Documentos

**Historia de Usuario:** Como usuario de cualquier nivel, quiero comunicarme con las otras partes involucradas en un documento mediante un chat integrado, para resolver dudas y notificar cambios en tiempo real.

#### Criterios de Aceptación

1. CUANDO un usuario visualiza un documento, EL Sistema DEBERÁ mostrar un campo de chat compartido entre todas las partes involucradas
2. CUANDO un usuario escribe un mensaje en el chat, EL Sistema DEBERÁ guardarlo en op_documentos_chat con timestamp, IdUsuario y Mensaje
3. CUANDO se envía un mensaje en el chat, EL Sistema DEBERÁ notificar a todas las partes involucradas en el documento
4. CUANDO un usuario edita un mensaje del chat, EL Sistema DEBERÁ registrar la edición con timestamp
5. MIENTRAS un documento está activo, EL Sistema DEBERÁ mantener el historial completo del chat para trazabilidad

### Requerimiento 10: Portal Web - Módulo de Dictámenes Técnicos

**Historia de Usuario:** Como funcionario municipal, quiero generar dictámenes técnicos validados por IA, para agilizar procesos de licitación.

#### Criterios de Aceptación

1. CUANDO un usuario crea un dictamen técnico, EL Sistema DEBERÁ capturar datos de la licitación, criterios de evaluación y propuestas en op_documentos con IdTipoDocumento=4
2. CUANDO se solicita validación de un dictamen, EL Agente_IA DEBERÁ verificar cumplimiento de requisitos legales y técnicos
3. CUANDO el Agente_IA detecta inconsistencias, EL Sistema DEBERÁ resaltar los campos problemáticos y sugerir correcciones
4. CUANDO se aprueba un dictamen, EL Sistema DEBERÁ generar el documento oficial en formato PDF
5. CUANDO se publica un dictamen, EL Sistema DEBERÁ registrar la fecha y hora de publicación en FechaUltimoMovimiento para trazabilidad

### Requerimiento 11: Portal Web - Módulo de Facturación y Tarifas

**Historia de Usuario:** Como administrador de entidad, quiero configurar tarifas y generar facturación automática, para optimizar la cobranza.

#### Criterios de Aceptación

1. CUANDO un administrador configura una tarifa, EL Sistema DEBERÁ permitir definir concepto, monto, periodicidad y aplicabilidad en cat_tarifas
2. CUANDO llega la fecha de facturación, EL Sistema DEBERÁ generar automáticamente los cargos según las tarifas configuradas mediante workflow n8n
3. CUANDO se genera una factura, EL Sistema DEBERÁ enviar notificación al usuario con opciones de pago
4. CUANDO un usuario realiza un pago, EL Sistema DEBERÁ registrar la transacción en op_pagos y actualizar el saldo en op_saldos
5. CUANDO un usuario consulta su estado de cuenta, EL Sistema DEBERÁ mostrar facturas, pagos y saldo actual

### Requerimiento 12: Portal Web - Módulo de Agricultura Inteligente

**Historia de Usuario:** Como productor agrícola, quiero monitorear mis cultivos con datos IoT y recibir recomendaciones de IA, para optimizar la producción.

#### Criterios de Aceptación

1. CUANDO un productor accede al módulo agrícola, EL Sistema DEBERÁ mostrar un mapa con la ubicación de sus parcelas (cat_parcelas) y sensores (cat_medidor)
2. CUANDO un sensor IoT envía datos, EL Sistema DEBERÁ actualizar los indicadores en tiempo real en op_medidor_lectura (humedad, temperatura, pH)
3. CUANDO los datos IoT superan umbrales configurados, EL Sistema DEBERÁ generar alertas automáticas en op_medidor_evento mediante workflow n8n
4. CUANDO el Agente_IA detecta condiciones de riesgo, EL Sistema DEBERÁ enviar recomendaciones preventivas
5. CUANDO un productor registra una aplicación fitosanitaria, EL Sistema DEBERÁ guardar la trazabilidad completa

### Requerimiento 13: Portal Web - Módulo de Control de Riego Automatizado

**Historia de Usuario:** Como productor agrícola, quiero automatizar el riego basado en datos IoT, para optimizar el uso de agua.

#### Criterios de Aceptación

1. CUANDO un productor configura un programa de riego, EL Sistema DEBERÁ permitir definir horarios, duración y condiciones
2. CUANDO se cumplen las condiciones de riego, EL Sistema DEBERÁ enviar comandos a los actuadores IoT mediante Azure IoT Hub para activar el riego
3. CUANDO el riego está activo, EL Sistema DEBERÁ mostrar el estado en tiempo real y permitir control manual
4. CUANDO se completa un ciclo de riego, EL Sistema DEBERÁ registrar consumo de agua y generar reportes
5. SI la humedad del suelo es suficiente, ENTONCES EL Sistema DEBERÁ cancelar el riego programado automáticamente

### Requerimiento 14: Portal Web - Integración con Agente de Voz IA

**Historia de Usuario:** Como usuario del sistema, quiero interactuar por voz con el sistema 24/7, para crear tickets y consultar información sin usar la interfaz web.

#### Criterios de Aceptación

1. CUANDO un usuario llama al número del Agente_de_Voz, EL Sistema DEBERÁ autenticar al usuario mediante su número telefónico registrado en conf_usuarios
2. CUANDO el Agente_de_Voz recibe una solicitud de crear ticket, EL Sistema DEBERÁ capturar la descripción por voz, transcribir y crear el ticket automáticamente con Canal='Telefono'
3. CUANDO un usuario consulta su saldo por voz, EL Agente_de_Voz DEBERÁ obtener la información de la API_REST y responder en lenguaje natural
4. CUANDO el Agente_de_Voz no comprende la solicitud, EL Sistema DEBERÁ solicitar aclaración o transferir a un operador humano
5. CUANDO finaliza una llamada, EL Sistema DEBERÁ registrar la transcripción y las acciones realizadas en op_llamadas_voz

### Requerimiento 15: Portal Web - Reportes y Analítica

**Historia de Usuario:** Como administrador, quiero generar reportes y visualizar analítica, para tomar decisiones basadas en datos.

#### Criterios de Aceptación

1. CUANDO un administrador accede al módulo de reportes, EL Sistema DEBERÁ mostrar plantillas predefinidas y opción de crear personalizados
2. CUANDO se genera un reporte, EL Sistema DEBERÁ permitir exportar en formatos PDF, Excel y CSV usando FuncionesGridWeb.ExportarExcel/PDF
3. CUANDO se visualizan gráficas, EL Sistema DEBERÁ permitir filtrar por fechas, entidades y categorías
4. CUANDO se programa un reporte recurrente, EL Sistema DEBERÁ enviarlo automáticamente por email mediante workflow n8n
5. MIENTRAS se genera un reporte grande, EL Sistema DEBERÁ mostrar progreso y permitir cancelar

### Requerimiento 16: Portal Web - Comunicación con Residentes

**Historia de Usuario:** Como administrador de condominio, quiero enviar avisos y comunicados a residentes, para mantenerlos informados.

#### Criterios de Aceptación

1. CUANDO un administrador crea un aviso, EL Sistema DEBERÁ permitir seleccionar destinatarios (todos, por torre, por unidad) en op_avisos
2. CUANDO se publica un aviso, EL Sistema DEBERÁ enviar notificaciones por email, SMS y push mediante workflow n8n
3. CUANDO un residente lee un aviso, EL Sistema DEBERÁ registrar la lectura en op_aviso_lecturas para trazabilidad
4. CUANDO se envía un comunicado urgente, EL Sistema DEBERÁ priorizar la entrega y solicitar confirmación de lectura
5. MIENTRAS un aviso está activo, EL Sistema DEBERÁ mostrarlo en el dashboard de los residentes

### Requerimiento 17: Aplicación Móvil - Autenticación y Onboarding

**Historia de Usuario:** Como usuario móvil, quiero autenticarme fácilmente mediante credenciales o biometría y recibir una introducción al usar la app por primera vez, para comenzar a usar el sistema rápidamente.

#### Criterios de Aceptación

1. CUANDO un usuario abre la app por primera vez, LA App_Móvil DEBERÁ mostrar un tutorial interactivo de las funciones principales
2. CUANDO un usuario ingresa credenciales válidas, LA App_Móvil DEBERÁ autenticar mediante la API_REST y guardar el token JWT de forma segura en SecureStorage
3. CUANDO un usuario activa autenticación biométrica, LA App_Móvil DEBERÁ permitir login con huella digital o reconocimiento facial en dispositivos compatibles
4. CUANDO un usuario intenta autenticarse con biometría, LA App_Móvil DEBERÁ validar la identidad mediante las APIs nativas del dispositivo (Touch ID, Face ID, Android Biometric)
5. CUANDO un usuario olvida su contraseña, LA App_Móvil DEBERÁ permitir recuperación mediante email o SMS
6. MIENTRAS la sesión está activa, LA App_Móvil DEBERÁ mantener el token actualizado automáticamente

### Requerimiento 18: Aplicación Móvil - Dashboard y Notificaciones Push

**Historia de Usuario:** Como usuario móvil, quiero ver información relevante en el dashboard y recibir notificaciones push, para estar informado en tiempo real.

#### Criterios de Aceptación

1. CUANDO un usuario abre la app, LA App_Móvil DEBERÁ mostrar un dashboard con información personalizada según su rol
2. CUANDO ocurre un evento relevante, EL Sistema DEBERÁ enviar una notificación push al dispositivo del usuario mediante Firebase/APNs
3. CUANDO un usuario toca una notificación, LA App_Móvil DEBERÁ navegar directamente a la pantalla relacionada
4. CUANDO un usuario desliza para refrescar, LA App_Móvil DEBERÁ actualizar los datos desde la API_REST
5. MIENTRAS la app está en segundo plano, LA App_Móvil DEBERÁ seguir recibiendo notificaciones push

### Requerimiento 19: Aplicación Móvil - Gestión de Tickets

**Historia de Usuario:** Como usuario móvil, quiero crear y dar seguimiento a tickets desde mi dispositivo, para reportar problemas en cualquier momento.

#### Criterios de Aceptación

1. CUANDO un usuario crea un ticket desde la app, LA App_Móvil DEBERÁ permitir capturar descripción, fotos (comprimidas) y ubicación GPS
2. CUANDO un usuario toma fotos para un ticket, LA App_Móvil DEBERÁ comprimir las imágenes antes de enviarlas para optimizar ancho de banda
3. CUANDO un técnico actualiza un ticket, LA App_Móvil DEBERÁ enviar notificación push al solicitante
4. CUANDO un usuario consulta sus tickets, LA App_Móvil DEBERÁ mostrar una lista con indicadores visuales de estado (colores por prioridad/sentimiento)
5. CUANDO un usuario está sin conexión, LA App_Móvil DEBERÁ permitir crear tickets en modo offline y sincronizar al reconectar

### Requerimiento 20: Aplicación Móvil - Órdenes de Trabajo para Técnicos

**Historia de Usuario:** Como técnico, quiero gestionar mis órdenes de trabajo desde la app móvil, para actualizar el estado y registrar evidencias en campo.

#### Criterios de Aceptación

1. CUANDO un técnico abre la app, LA App_Móvil DEBERÁ mostrar sus órdenes de trabajo asignadas ordenadas por prioridad
2. CUANDO un técnico acepta una orden, LA App_Móvil DEBERÁ actualizar el estado y registrar la hora de inicio
3. CUANDO un técnico completa una orden, LA App_Móvil DEBERÁ solicitar fotos de evidencia y firma del cliente
4. CUANDO un técnico registra materiales usados, LA App_Móvil DEBERÁ permitir escanear códigos de barras
5. CUANDO un técnico está en ruta, LA App_Móvil DEBERÁ mostrar navegación GPS hacia la ubicación del trabajo

### Requerimiento 21: Aplicación Móvil - Consulta de Saldo y Pagos (Residentes)

**Historia de Usuario:** Como residente, quiero consultar mi saldo y realizar pagos desde la app, para mantener mis cuentas al día.

#### Criterios de Aceptación

1. CUANDO un residente consulta su saldo, LA App_Móvil DEBERÁ mostrar facturas pendientes, pagos recientes y saldo total desde op_saldos
2. CUANDO un residente selecciona una factura para pagar, LA App_Móvil DEBERÁ mostrar opciones de pago disponibles
3. CUANDO un residente realiza un pago, LA App_Móvil DEBERÁ procesar la transacción y mostrar comprobante digital
4. CUANDO se registra un pago, LA App_Móvil DEBERÁ enviar notificación de confirmación
5. CUANDO un residente descarga un comprobante, LA App_Móvil DEBERÁ generar un PDF con código QR de validación

### Requerimiento 22: Aplicación Móvil - Monitoreo Agrícola IoT

**Historia de Usuario:** Como productor agrícola, quiero monitorear mis cultivos desde la app móvil, para tomar decisiones en campo.

#### Criterios de Aceptación

1. CUANDO un productor abre el módulo agrícola, LA App_Móvil DEBERÁ mostrar un mapa con sus parcelas y sensores
2. CUANDO un productor selecciona un sensor, LA App_Móvil DEBERÁ mostrar gráficas de datos históricos y valor actual desde op_medidor_lectura
3. CUANDO un sensor genera una alerta, LA App_Móvil DEBERÁ enviar notificación push con recomendaciones
4. CUANDO un productor activa el riego manualmente, LA App_Móvil DEBERÁ enviar el comando al actuador IoT mediante Azure IoT Hub
5. CUANDO un productor registra una observación en campo, LA App_Móvil DEBERÁ guardar la ubicación GPS y fotos

### Requerimiento 23: Aplicación Móvil - Captura de Costos Reales (Colaborador)

**Historia de Usuario:** Como colaborador de campo (nivel 4), quiero registrar los costos reales de ejecución desde la app móvil, para completar el flujo multinivel de documentos.

#### Criterios de Aceptación

1. CUANDO un colaborador abre la app, LA App_Móvil DEBERÁ mostrar los documentos asignados a su nivel con estado "Asignado a Colaborador"
2. CUANDO un colaborador selecciona un documento, LA App_Móvil DEBERÁ mostrar únicamente los montos de Proveedor (nivel 3) sin mostrar montos de niveles anteriores
3. CUANDO un colaborador captura costos reales, LA App_Móvil DEBERÁ permitir ingresar MontoReal por cada partida/concepto
4. CUANDO un colaborador adjunta evidencias, LA App_Móvil DEBERÁ permitir tomar fotos con ubicación GPS y timestamp
5. CUANDO un colaborador finaliza la captura, LA App_Móvil DEBERÁ sincronizar los datos con el servidor y notificar al Proveedor

### Requerimiento 24: Aplicación Móvil - Modo Offline

**Historia de Usuario:** Como usuario móvil en zonas sin cobertura, quiero usar funciones básicas offline, para trabajar sin interrupciones.

#### Criterios de Aceptación

1. CUANDO la app pierde conexión, LA App_Móvil DEBERÁ mostrar un indicador de modo offline
2. MIENTRAS está offline, LA App_Móvil DEBERÁ permitir consultar datos previamente cargados desde SQLite local
3. CUANDO un usuario crea un ticket offline, LA App_Móvil DEBERÁ guardarlo localmente en PendingSync con indicador de pendiente
4. CUANDO la app recupera conexión, LA App_Móvil DEBERÁ sincronizar automáticamente los datos pendientes en orden cronológico
5. SI la sincronización falla, ENTONCES LA App_Móvil DEBERÁ reintentar automáticamente (máximo 3 intentos) y notificar al usuario

### Requerimiento 25: Aplicación Móvil - Chat con Soporte

**Historia de Usuario:** Como usuario móvil, quiero chatear con soporte en tiempo real, para resolver dudas rápidamente.

#### Criterios de Aceptación

1. CUANDO un usuario abre el chat, LA App_Móvil DEBERÁ mostrar el historial de conversaciones previas
2. CUANDO un usuario envía un mensaje, LA App_Móvil DEBERÁ entregarlo en tiempo real usando WebSockets
3. CUANDO un agente de soporte responde, LA App_Móvil DEBERÁ mostrar notificación y actualizar el chat
4. CUANDO no hay agentes disponibles, LA App_Móvil DEBERÁ ofrecer conectar con el Agente_de_Voz IA
5. MIENTRAS el chat está activo, LA App_Móvil DEBERÁ mostrar indicador de "escribiendo" cuando el agente está respondiendo

### Requerimiento 26: Portal Web y App Móvil - Soporte Multi-Idioma

**Historia de Usuario:** Como usuario del sistema, quiero usar la interfaz en mi idioma preferido, para comprender mejor las funcionalidades y trabajar más eficientemente.

#### Criterios de Aceptación

1. CUANDO un usuario accede al sistema por primera vez, EL Sistema DEBERÁ detectar el idioma del navegador o dispositivo y configurarlo como predeterminado
2. CUANDO un usuario cambia el idioma en la configuración, EL Sistema DEBERÁ actualizar toda la interfaz al idioma seleccionado inmediatamente
3. CUANDO se muestra contenido traducible, EL Sistema DEBERÁ obtener el texto del archivo de recursos correspondiente al idioma activo (Resources.es-MX.resx, Resources.en-US.resx)
4. CUANDO un idioma no tiene traducción para un texto específico, EL Sistema DEBERÁ mostrar el texto en el idioma predeterminado (español)
5. MIENTRAS un usuario tiene sesión activa, EL Sistema DEBERÁ mantener la preferencia de idioma en todas las páginas y módulos
6. CUANDO se generan documentos o reportes, EL Sistema DEBERÁ usar el idioma configurado por el usuario
7. CUANDO el Agente_de_Voz IA recibe una llamada, EL Sistema DEBERÁ detectar el idioma del usuario y responder en ese idioma

### Requerimiento 27: Integración con Flujos N8N

**Historia de Usuario:** Como administrador del sistema, quiero que los flujos N8N se integren automáticamente, para automatizar procesos sin intervención manual.

#### Criterios de Aceptación

1. CUANDO se crea un ticket, EL Sistema DEBERÁ invocar el flujo N8N de notificaciones automáticamente
2. CUANDO se aprueba una orden de compra, EL Sistema DEBERÁ invocar el flujo N8N de notificación a proveedores
3. CUANDO un sensor IoT envía datos, EL Sistema DEBERÁ invocar el flujo N8N de procesamiento de alertas
4. CUANDO un flujo N8N falla, EL Sistema DEBERÁ registrar el error y reintentar según la configuración
5. MIENTRAS un flujo N8N está en ejecución, EL Sistema DEBERÁ permitir consultar su estado

### Requerimiento 28: Portal Web y App Móvil - Accesibilidad

**Historia de Usuario:** Como usuario con discapacidad, quiero usar el sistema con tecnologías de asistencia, para acceder a todas las funcionalidades.

#### Criterios de Aceptación

1. CUANDO un usuario con lector de pantalla navega el sistema, EL Sistema DEBERÁ proporcionar etiquetas ARIA descriptivas
2. CUANDO un usuario navega con teclado, EL Sistema DEBERÁ permitir acceso a todas las funciones sin mouse
3. CUANDO se muestra información visual, EL Sistema DEBERÁ proporcionar alternativas textuales
4. CUANDO se usan colores para indicar estado, EL Sistema DEBERÁ incluir iconos o texto adicional
5. MIENTRAS un usuario interactúa con formularios, EL Sistema DEBERÁ proporcionar mensajes de error claros y accesibles

### Requerimiento 29: Portal Web y App Móvil - Rendimiento

**Historia de Usuario:** Como usuario del sistema, quiero que las interfaces respondan rápidamente, para trabajar eficientemente.

#### Criterios de Aceptación

1. CUANDO un usuario carga una página del portal, EL Sistema DEBERÁ renderizar el contenido principal en menos de 2 segundos
2. CUANDO un usuario realiza una búsqueda, EL Sistema DEBERÁ mostrar resultados en menos de 1 segundo
3. CUANDO la app móvil carga datos, LA App_Móvil DEBERÁ mostrar indicadores de progreso claros
4. CUANDO se cargan listas grandes, EL Sistema DEBERÁ mostrar todos los registros (PageSize=-1) con scroll virtual para rendimiento
5. MIENTRAS se procesan operaciones largas, EL Sistema DEBERÁ permitir al usuario continuar usando otras funciones

### Requerimiento 30: Portal Web y App Móvil - Seguridad

**Historia de Usuario:** Como administrador del sistema, quiero garantizar la seguridad de los datos, para proteger la información de los usuarios.

#### Criterios de Aceptación

1. CUANDO se transmiten datos sensibles, EL Sistema DEBERÁ usar cifrado HTTPS/TLS
2. CUANDO un usuario ingresa contraseñas, EL Sistema DEBERÁ validar complejidad mínima (8 caracteres, mayúsculas, números, especiales)
3. CUANDO se detectan múltiples intentos fallidos de login (>5), EL Sistema DEBERÁ bloquear temporalmente la cuenta por 15 minutos
4. CUANDO se almacenan tokens de sesión, LA App_Móvil DEBERÁ usar almacenamiento seguro del dispositivo (SecureStorage)
5. MIENTRAS un usuario está inactivo por más de 30 minutos, EL Sistema DEBERÁ cerrar la sesión automáticamente

### Requerimiento 31: Módulo de Conceptos y Productos

**Historia de Usuario:** Como administrador, quiero gestionar el catálogo de conceptos y productos, para mantener actualizada la información de servicios disponibles.

#### Criterios de Aceptación

1. CUANDO un administrador crea un concepto, EL Sistema DEBERÁ capturar Clave, ClaveAlterna, Descripcion, IdUnidadMedida, CostoUnitario, IdFamilia en cat_conceptos
2. CUANDO un administrador edita un concepto, EL Sistema DEBERÁ mostrar ASPxPopupControl con los datos actuales y permitir modificación
3. CUANDO un administrador consulta conceptos, EL Sistema DEBERÁ mostrar ASPxGridView con filtros tipo Excel en cabeceras y agrupación habilitada
4. CUANDO un concepto se usa en documentos, EL Sistema DEBERÁ prevenir su eliminación y mostrar mensaje de dependencias
5. MIENTRAS se capturan partidas en documentos, EL Sistema DEBERÁ permitir búsqueda por Clave para autocompletar datos del concepto

### Requerimiento 32: Módulo de Proveedores y Colaboradores

**Historia de Usuario:** Como administrador, quiero gestionar proveedores y sus colaboradores, para mantener el directorio actualizado.

#### Criterios de Aceptación

1. CUANDO un administrador crea un proveedor, EL Sistema DEBERÁ capturar RFC, RazonSocial, Contacto, Telefono, Email, Direccion en cat_proveedores
2. CUANDO un administrador asigna colaboradores a un proveedor, EL Sistema DEBERÁ vincularlos mediante IdProveedor en cat_colaboradores
3. CUANDO un proveedor se desactiva, EL Sistema DEBERÁ verificar que no tenga documentos activos asignados
4. CUANDO se consulta un proveedor, EL Sistema DEBERÁ mostrar sus colaboradores asociados y documentos históricos
5. MIENTRAS un proveedor tiene documentos pendientes, EL Sistema DEBERÁ mostrar indicador visual de carga de trabajo

### Requerimiento 33: Módulo de Medidores IoT

**Historia de Usuario:** Como administrador de agricultura, quiero gestionar los medidores IoT instalados, para monitorear el estado de los sensores.

#### Criterios de Aceptación

1. CUANDO un administrador registra un medidor, EL Sistema DEBERÁ capturar NumeroSerie, IdEntidad, TipoMedidor, Ubicacion, FechaInstalacion en cat_medidor
2. CUANDO un medidor envía lecturas, EL Sistema DEBERÁ registrarlas en op_medidor_lectura con FechaLectura, Lectura, TipoLectura
3. CUANDO un medidor genera un evento, EL Sistema DEBERÁ registrarlo en op_medidor_evento con TipoEvento, Descripcion, Severidad
4. CUANDO se consultan medidores, EL Sistema DEBERÁ mostrar estado actual (Activo/Inactivo), última lectura y alertas pendientes
5. MIENTRAS un medidor está fuera de línea por más de 24 horas, EL Sistema DEBERÁ generar alerta automática

### Requerimiento 34: Módulo de Formularios Dinámicos

**Historia de Usuario:** Como administrador, quiero crear formularios dinámicos extrayendo campos de un PDF con Azure Document Intelligence, para que los colaboradores capturen información en campo y el sistema genere automáticamente un PDF con el formato original lleno con los datos capturados.

#### Criterios de Aceptación - Fase 1: Creación de Formularios (Portal Web)

1. CUANDO un administrador sube un PDF de plantilla, EL Sistema DEBERÁ enviar el archivo a Azure Document Intelligence (sin almacenar el PDF), extraer campos detectados con sus tipos inferidos, y mostrar vista previa para revisión
2. CUANDO un administrador confirma los campos extraídos, EL Sistema DEBERÁ crear registro en cat_formularios con nombre y descripción, crear registros en cat_campos_formulario con tipo, posición, sección y configuración de cada campo
3. CUANDO un administrador ajusta campos manualmente, EL Sistema DEBERÁ permitir modificar nombre_campo, etiqueta_campo, tipo_campo, posicion_orden, seccion, ancho_columna y es_requerido
4. CUANDO un administrador crea un formulario manualmente (sin PDF), EL Sistema DEBERÁ permitir agregar campos uno a uno configurando tipo, etiqueta, sección y validaciones
5. CUANDO un campo es tipo dropdown o radio, EL Sistema DEBERÁ permitir configurar opciones en cat_opciones_campo con valor_opcion, etiqueta_opcion y posicion_orden
6. CUANDO un administrador configura la plantilla PDF para generación, EL Sistema DEBERÁ guardar el template HTML/CSS en cat_plantilla_pdf que replica el formato del PDF original

#### Criterios de Aceptación - Fase 2: Asignación (Portal Web)

7. CUANDO un funcionario asigna un formulario a un fallo, EL Sistema DEBERÁ crear registro en op_fallo_formulario vinculando fallo_id, formulario_id, entidad_id y usuario_asignado
8. CUANDO un funcionario consulta formularios asignados, EL Sistema DEBERÁ mostrar estado (pendiente, en_proceso, completado) y porcentaje de avance

#### Criterios de Aceptación - Fase 3: Captura en Campo (App Móvil y Web)

9. CUANDO un colaborador abre un fallo en la App_Móvil, LA App_Móvil DEBERÁ cargar el formulario asignado y renderizar campos dinámicamente usando StackLayout + controles nativos MAUI según cat_campos_formulario
10. CUANDO un colaborador abre un fallo en el Portal_Web, EL Sistema DEBERÁ renderizar el formulario dinámicamente usando DevExpress ASPxFormLayout según cat_campos_formulario
11. CUANDO un colaborador captura datos, LA App_Móvil DEBERÁ guardar respuestas localmente en SQLite hasta sincronización
12. CUANDO un colaborador toma fotos, LA App_Móvil DEBERÁ comprimir imágenes y guardarlas localmente con referencia al campo
13. CUANDO un colaborador captura firma digital, LA App_Móvil DEBERÁ guardar como imagen PNG asociada al formulario

#### Criterios de Aceptación - Fase 4: Sincronización y Generación de PDF (Automático)

14. CUANDO el colaborador completa y sincroniza el formulario, EL Sistema DEBERÁ guardar respuestas en op_respuesta_formulario y op_respuesta_campo, subir fotos a Azure Blob Storage (contenedor: formularios-fotos), subir firma a Azure Blob Storage (contenedor: formularios-firmas), y actualizar URLs en op_respuesta_campo
15. CUANDO se completa la sincronización de datos, EL Sistema DEBERÁ generar PDF usando la plantilla HTML de cat_plantilla_pdf + datos capturados + fotos + firma, subir PDF a Azure Blob Storage (contenedor: formularios-pdf), registrar en op_documento_formulario_pdf con url_pdf_azure, y vincular PDF al fallo como documento hijo
16. MIENTRAS la App_Móvil está sin conexión, LA App_Móvil DEBERÁ permitir captura completa y encolar para sincronización posterior

### Requerimiento 35: Integración de Formularios con Módulo de Servicios Municipales

**Historia de Usuario:** Como funcionario municipal, quiero asignar formularios dinámicos a los fallos de licitación, para que los colaboradores en campo capturen información estructurada con evidencias fotográficas y firmas, y el sistema genere automáticamente el PDF con el formato de la plantilla original.

#### Criterios de Aceptación

1. CUANDO un funcionario de Entidad crea un fallo, EL Sistema DEBERÁ mostrar opción para seleccionar formularios disponibles de cat_formularios donde estado='activo'
2. CUANDO un funcionario selecciona un formulario para el fallo, EL Sistema DEBERÁ crear registro en op_fallo_formulario con la asignación correspondiente
3. CUANDO un colaborador (nivel 4) abre un fallo en la App_Móvil, LA App_Móvil DEBERÁ cargar el formulario asignado desde op_fallo_formulario y renderizar campos dinámicamente según cat_campos_formulario respetando secciones y orden
4. CUANDO un colaborador captura fotos en el formulario, LA App_Móvil DEBERÁ comprimir imágenes, guardar localmente y subir a Azure Blob Storage al sincronizar
5. CUANDO un colaborador captura firma digital, LA App_Móvil DEBERÁ guardar como imagen PNG y asociar al campo correspondiente
6. CUANDO un colaborador finaliza el formulario y sincroniza, EL Sistema DEBERÁ generar PDF con plantilla HTML de cat_plantilla_pdf + datos + fotos + firma, y vincularlo al fallo como documento hijo en op_documento_formulario_pdf
7. MIENTRAS el formulario está en proceso, EL Sistema DEBERÁ mostrar porcentaje de avance y campos pendientes en dashboard de seguimiento

### Requerimiento 36: Módulo de Condominios - Catálogo de Áreas Comunes

**Historia de Usuario:** Como administrador de condominio, quiero gestionar las áreas comunes reservables del condominio (salones, albercas, gimnasios), para organizar los espacios compartidos y sus configuraciones de reservación.

**NOTA:** Las secciones/torres se manejan en `cat_subentidades` (ya existente). Este catálogo es exclusivamente para áreas comunes.

#### Criterios de Aceptación

1. CUANDO un administrador crea un área común, EL Sistema DEBERÁ capturar Clave, Nombre, SubEntidadId (NULL=compartida, ID=exclusiva de torre), TipoArea (Salon, Alberca, Gimnasio, Jardin, Terraza, Asador, Ludoteca, SalaJuntas, Cancha), Capacidad, CostoReservacion, DepositoRequerido, RequiereReservacion, HoraApertura, HoraCierre, DuracionMinimaHoras, DuracionMaximaHoras, AnticipacionMinimaDias, AnticipacionMaximaDias en cat_areas_comunes
2. CUANDO un administrador edita un área común, EL Sistema DEBERÁ cargar los datos actuales en un ASPxPopupControl
3. CUANDO un administrador elimina un área común, EL Sistema DEBERÁ verificar que no tenga reservaciones futuras
4. CUANDO un administrador consulta áreas comunes, EL Sistema DEBERÁ mostrar un ASPxGridView filtrado por EntidadId con columnas: Clave, Nombre, TipoArea, SubEntidad (si aplica), Capacidad, CostoReservacion, RequiereReservacion
5. CUANDO un área común requiere reservación (RequiereReservacion=1), EL Sistema DEBERÁ validar disponibilidad antes de confirmar reservaciones
6. CUANDO un administrador configura reglamento de área común, EL Sistema DEBERÁ guardar texto en campo Reglamento para mostrar al residente
7. CUANDO un área común tiene SubEntidadId asignado, EL Sistema DEBERÁ mostrar que es exclusiva de esa torre/sección
8. MIENTRAS un área común tiene reservaciones futuras, EL Sistema DEBERÁ no permitir eliminación pero sí desactivación

### Requerimiento 37: Módulo de Condominios - Catálogo de Residentes

**Historia de Usuario:** Como administrador de condominio, quiero gestionar el catálogo de residentes (propietarios e inquilinos), para mantener un registro completo de las personas que habitan el condominio.

#### Criterios de Aceptación

1. CUANDO un administrador crea un residente, EL Sistema DEBERÁ capturar Nombre, ApellidoPaterno, ApellidoMaterno, TipoResidente (Propietario, Inquilino, Familiar, Empleado), Email, Telefono, TelefonoCelular, CURP, RFC, FechaNacimiento, UnidadId en cat_residentes
2. CUANDO un administrador asocia un residente a una unidad, EL Sistema DEBERÁ actualizar PropietarioId o InquilinoId en cat_unidades según TipoResidente
3. CUANDO un administrador vincula Telegram a un residente, EL Sistema DEBERÁ guardar TelegramChatId para notificaciones
4. CUANDO un administrador consulta residentes, EL Sistema DEBERÁ mostrar NombreCompleto (campo calculado), TipoResidente, Unidad asociada y estado Activo
5. MIENTRAS un residente tiene cuotas pendientes, EL Sistema DEBERÁ mostrar indicador visual de morosidad

### Requerimiento 38: Módulo de Condominios - Catálogo de Unidades Privativas

**Historia de Usuario:** Como administrador de condominio, quiero gestionar las unidades privativas (departamentos, casas, locales), para registrar información completa de cada propiedad.

**NOTA:** Las unidades pertenecen a SubEntidades (torres/secciones), NO a cat_areas_comunes.

#### Criterios de Aceptación

1. CUANDO un administrador crea una unidad, EL Sistema DEBERÁ capturar Codigo, Nombre, SubEntidadId (torre/sección), TipoUnidad (Departamento, Casa, Local, Bodega, Estacionamiento), Torre, Edificio, Piso, Numero, Superficie, Indiviso, CuotaOrdinaria, EstatusOcupacion en cat_unidades
2. CUANDO un administrador asigna propietario a una unidad, EL Sistema DEBERÁ vincular PropietarioId desde cat_residentes
3. CUANDO un administrador asigna inquilino a una unidad, EL Sistema DEBERÁ vincular InquilinoId desde cat_residentes (opcional)
4. CUANDO un administrador consulta unidades, EL Sistema DEBERÁ mostrar Codigo, Nombre, SubEntidad (torre/sección), Propietario, Inquilino, CuotaOrdinaria, EstatusOcupacion
5. MIENTRAS una unidad tiene saldo pendiente, EL Sistema DEBERÁ mostrar indicador visual de morosidad con monto adeudado

### Requerimiento 39: Módulo de Condominios - Catálogo de Conceptos de Cuota

**Historia de Usuario:** Como administrador de condominio, quiero configurar los conceptos de cuota (mantenimiento, agua, gas, extraordinarias), para definir los tipos de cargos que se generan a los residentes.

#### Criterios de Aceptación

1. CUANDO un administrador crea un concepto de cuota, EL Sistema DEBERÁ capturar Clave, Nombre, TipoCuota (Ordinaria, Extraordinaria, Servicio, Multa, Recargo), MontoDefault, EsRecurrente, DiaVencimiento, DiasGracia, PorcentajeRecargo en cat_conceptos_cuota
2. CUANDO un concepto es recurrente (EsRecurrente=1), EL Sistema DEBERÁ incluirlo en la generación automática de cuotas mensuales
3. CUANDO un concepto tiene PorcentajeRecargo > 0, EL Sistema DEBERÁ aplicar recargo automático a cuotas vencidas después de DiasGracia
4. CUANDO un administrador consulta conceptos, EL Sistema DEBERÁ mostrar Clave, Nombre, TipoCuota, MontoDefault, EsRecurrente, PorcentajeRecargo
5. MIENTRAS un concepto tiene cuotas generadas, EL Sistema DEBERÁ no permitir eliminación pero sí desactivación

### Requerimiento 40: Módulo de Condominios - Generación de Cuotas

**Historia de Usuario:** Como administrador de condominio, quiero generar cuotas mensuales automáticamente para todas las unidades, para mantener actualizado el estado de cuenta de los residentes.

#### Criterios de Aceptación

1. CUANDO un administrador ejecuta generación de cuotas, EL Sistema DEBERÁ crear registros en op_cuotas para cada unidad activa y concepto recurrente del periodo seleccionado
2. CUANDO se genera una cuota, EL Sistema DEBERÁ asignar Folio único (CUO-YYYYMM-XXXX), calcular Monto desde CuotaOrdinaria de unidad o MontoDefault de concepto, y establecer FechaVencimiento según DiaVencimiento
3. CUANDO ya existe cuota para unidad/concepto/periodo, EL Sistema DEBERÁ omitir la generación y no duplicar
4. CUANDO se genera cuota, EL Sistema DEBERÁ vincular ResidenteId desde PropietarioId de la unidad
5. MIENTRAS existen cuotas pendientes de periodos anteriores, EL Sistema DEBERÁ mostrar alerta de morosidad acumulada

### Requerimiento 41: Módulo de Condominios - Registro de Pagos

**Historia de Usuario:** Como administrador de condominio, quiero registrar los pagos de los residentes, para actualizar automáticamente el saldo de sus cuotas pendientes.

#### Criterios de Aceptación

1. CUANDO un administrador registra un pago, EL Sistema DEBERÁ capturar UnidadId, Monto, FormaPago (Efectivo, Transferencia, Tarjeta, Cheque, Deposito, SPEI), Referencia, Banco, FechaPago en op_pagos
2. CUANDO se registra un pago, EL Sistema DEBERÁ asignar Folio único (PAG-YYYYMMDD-XXXX) y crear registros en op_pagos_detalle aplicando el monto a cuotas pendientes en orden cronológico (FIFO)
3. CUANDO se aplica pago a una cuota, EL Sistema DEBERÁ actualizar MontoPagado y Estado (Pendiente→Parcial→Pagada) automáticamente mediante trigger
4. CUANDO el pago excede el saldo total pendiente, EL Sistema DEBERÁ registrar el excedente como saldo a favor
5. MIENTRAS se registra un pago, EL Sistema DEBERÁ mostrar cuotas pendientes de la unidad con opción de seleccionar a cuáles aplicar

### Requerimiento 42: Módulo de Condominios - Estado de Cuenta

**Historia de Usuario:** Como administrador o residente, quiero consultar el estado de cuenta de una unidad, para ver el historial de cuotas, pagos y saldo actual.

#### Criterios de Aceptación

1. CUANDO un usuario consulta estado de cuenta, EL Sistema DEBERÁ mostrar datos de la vista vw_estado_cuenta filtrados por UnidadId
2. CUANDO se muestra estado de cuenta, EL Sistema DEBERÁ incluir Periodo, ConceptoNombre, Monto, Descuento, Recargo, MontoTotal, MontoPagado, Saldo, Estado, DiasVencido
3. CUANDO existen cuotas vencidas, EL Sistema DEBERÁ resaltar visualmente con color rojo y mostrar días de atraso
4. CUANDO un usuario solicita exportar, EL Sistema DEBERÁ generar PDF o Excel con el estado de cuenta completo
5. MIENTRAS exista saldo pendiente, EL Sistema DEBERÁ mostrar total adeudado y botón para registrar pago

### Requerimiento 43: Módulo de Condominios - Reservaciones de Áreas Comunes

**Historia de Usuario:** Como residente, quiero reservar áreas comunes del condominio, para organizar eventos o actividades.

#### Criterios de Aceptación

1. CUANDO un residente crea una reservación, EL Sistema DEBERÁ capturar AreaComunId, UnidadId, FechaReservacion, HoraInicio, HoraFin, NumeroInvitados, Motivo, TipoEvento en op_reservaciones
2. CUANDO se crea reservación, EL Sistema DEBERÁ validar disponibilidad del área en fecha/hora solicitada, verificar anticipación mínima/máxima, y calcular CostoReservacion y DepositoRequerido
3. CUANDO existe conflicto de horario, EL Sistema DEBERÁ rechazar la reservación y mostrar horarios disponibles
4. CUANDO un administrador aprueba reservación, EL Sistema DEBERÁ cambiar Estado a Confirmada y notificar al residente
5. MIENTRAS una reservación está pendiente de depósito, EL Sistema DEBERÁ mostrar indicador y no permitir uso del área
6. CUANDO un residente intenta reservar un área común de la Entidad (SubEntidadId=NULL), EL Sistema DEBERÁ permitir la reservación desde cualquier unidad de cualquier sub-entidad del condominio
7. CUANDO un residente intenta reservar un área común exclusiva de una SubEntidad (SubEntidadId=ID), EL Sistema DEBERÁ validar que la unidad del residente pertenezca a esa misma sub-entidad, rechazando la reservación si no corresponde
8. CUANDO un residente consulta áreas disponibles para reservar, EL Sistema DEBERÁ mostrar únicamente las áreas de la Entidad (compartidas) más las áreas exclusivas de su sub-entidad

### Requerimiento 44: Módulo de Condominios - Calendario de Reservaciones

**Historia de Usuario:** Como administrador o residente, quiero ver un calendario con las reservaciones de áreas comunes, para conocer la disponibilidad.

#### Criterios de Aceptación

1. CUANDO un usuario accede al calendario, EL Sistema DEBERÁ mostrar vista mensual con reservaciones de la vista vw_calendario_reservaciones
2. CUANDO un usuario selecciona un área, EL Sistema DEBERÁ filtrar el calendario para mostrar solo reservaciones de esa área
3. CUANDO un usuario hace clic en una reservación, EL Sistema DEBERÁ mostrar detalle con Unidad, Residente, Horario, Estado
4. CUANDO un usuario hace clic en fecha/hora vacía, EL Sistema DEBERÁ abrir formulario de nueva reservación con fecha preseleccionada
5. MIENTRAS se visualiza el calendario, EL Sistema DEBERÁ usar colores diferentes por estado (Pendiente=amarillo, Confirmada=verde, EnUso=azul, Completada=gris)

### Requerimiento 45: Módulo de Condominios - Control de Visitantes

**Historia de Usuario:** Como vigilante del condominio, quiero registrar la entrada y salida de visitantes, para mantener control de acceso y seguridad.

#### Criterios de Aceptación

1. CUANDO un vigilante registra entrada de visitante, EL Sistema DEBERÁ capturar UnidadId (destino), NombreVisitante, TipoIdentificacion, NumeroIdentificacion, TipoVisita (Personal, Proveedor, Delivery, Emergencia, Trabajador), MotivoVisita, FechaEntrada en op_visitantes
2. CUANDO el visitante tiene vehículo, EL Sistema DEBERÁ capturar PlacasVehiculo, ColorVehiculo, MarcaVehiculo, ModeloVehiculo
3. CUANDO un vigilante registra salida, EL Sistema DEBERÁ actualizar FechaSalida y Estado a 'Salida', calculando TiempoEstancia automáticamente
4. CUANDO un vigilante consulta visitantes activos, EL Sistema DEBERÁ mostrar vista vw_visitantes_activos con MinutosEnCondominio
5. MIENTRAS un visitante está en el condominio (Estado='EnCondominio'), EL Sistema DEBERÁ mostrar en lista de visitantes activos

### Requerimiento 46: Módulo de Condominios - Comunicados y Avisos

**Historia de Usuario:** Como administrador de condominio, quiero enviar comunicados y avisos a los residentes, para mantenerlos informados de eventos, mantenimientos y noticias importantes.

#### Criterios de Aceptación

1. CUANDO un administrador crea un comunicado, EL Sistema DEBERÁ capturar Titulo, Contenido, Resumen, TipoComunicado (Aviso, Urgente, Mantenimiento, Evento, Asamblea, Cobranza, General), Prioridad, FechaPublicacion, FechaExpiracion, Destinatarios en op_comunicados
2. CUANDO un administrador selecciona canales de envío, EL Sistema DEBERÁ permitir activar EnviarEmail, EnviarTelegram, EnviarPush, EnviarSMS
3. CUANDO se publica un comunicado, EL Sistema DEBERÁ enviar notificaciones por los canales seleccionados a los destinatarios configurados
4. CUANDO un residente lee un comunicado, EL Sistema DEBERÁ registrar lectura en op_comunicados_lecturas con FechaLectura y Canal
5. MIENTRAS un comunicado está activo (no expirado), EL Sistema DEBERÁ mostrarlo en el dashboard de residentes

### Requerimiento 47: Módulo de Condominios - Reporte de Morosidad

**Historia de Usuario:** Como administrador de condominio, quiero consultar un reporte de morosidad, para identificar las unidades con pagos pendientes y tomar acciones de cobranza.

#### Criterios de Aceptación

1. CUANDO un administrador consulta morosidad, EL Sistema DEBERÁ mostrar datos de la vista vw_resumen_morosidad ordenados por SaldoTotal descendente
2. CUANDO se muestra reporte, EL Sistema DEBERÁ incluir UnidadCodigo, UnidadNombre, CuotasPendientes, SaldoTotal, PeriodoMasAntiguo, MaxDiasVencido
3. CUANDO un administrador filtra por rango de morosidad, EL Sistema DEBERÁ permitir filtrar por monto mínimo, días vencidos o periodo
4. CUANDO un administrador exporta reporte, EL Sistema DEBERÁ generar Excel o PDF con el listado completo
5. MIENTRAS existan unidades morosas, EL Sistema DEBERÁ mostrar totales generales de morosidad en dashboard

---

## Notas Importantes

1. **Backend existente**: API REST operativa en Azure con CrudController dinámico
2. **Frontend web**: ASP.NET Web Forms con VB.NET, DevExpress 22.2 y Bootstrap 5
3. **Módulos implementados**: Login, Entidades, Conceptos, Captura de Documentos básica, Tickets con IA
4. **Integraciones**: Flujos N8N para notificaciones, Azure OpenAI (GPT-4) para IA, Azure IoT Hub
5. **Apps móviles**: Desarrollar con .NET MAUI o nativo (Swift/Kotlin)
6. **Nomenclatura de tablas**: Usar guion bajo (op_tickets, cat_conceptos, conf_usuarios)
7. **Estándares UI**: Ver ui-standards.md para reglas de DevExpress
8. **Módulo Condominios**: Scripts SQL en `.kiro/specs/ecosistema-jelabbc/sql/condominios/`

## Referencias

- [Arquitectura Tecnológica](./architecture.md)
- [Especificaciones de Módulos](./modules.md)
- [Estándares de UI](./ui-standards.md)
- [Diseño Técnico](./design.md)
