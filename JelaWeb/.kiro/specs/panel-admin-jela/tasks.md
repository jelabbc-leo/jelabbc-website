# Plan de Implementación: Panel Admin JELA

## Resumen

Este plan describe las tareas para implementar el Panel Admin JELA, un sistema de gestión de licencias y administradores para la plataforma JELA. El sistema se construirá como una aplicación ASP.NET WebForms (VB.NET) que se integra con JELA.API (.NET 8 C#) y comparte la base de datos MySQL con JelaWeb.

## Tareas

- [ ] 1. Configurar infraestructura de base de datos
  - Crear tablas nuevas: conf_licencias, conf_auditoria_admin, conf_sistema, conf_alertas_licencias
  - Crear índices para optimización de consultas
  - Insertar registro inicial en conf_sistema con IdEntidadPlantilla = 1
  - Crear stored procedures para operaciones complejas (opcional)
  - _Requerimientos: 4.1, 8.9, 10.1, 9.5_

- [ ] 2. Implementar modelos y DTOs en JELA.API
  - [ ] 2.1 Crear DTOs para administradores
    - CrearAdministradorDto, ActualizarAdministradorDto, AdministradorDto
    - Agregar validaciones con Data Annotations
    - _Requerimientos: 3.2, 3.3, 3.4_
  
  - [ ] 2.2 Crear DTOs para entidades
    - CrearEntidadDto, ActualizarEntidadDto, EntidadDto
    - Agregar validaciones con Data Annotations
    - _Requerimientos: 7.2, 7.3_
  
  - [ ] 2.3 Crear DTOs para licencias y otros módulos
    - LicenciaDto, ActualizarLicenciasDto, DashboardMetricasDto, AuditoriaDto
    - AsignarEntidadDto, ConfiguracionDto
    - _Requerimientos: 4.6, 2.1-2.7, 8.9_

- [ ] 3. Implementar repositorios de datos en JELA.API
  - [ ] 3.1 Crear AdministradorRepository
    - Métodos: ListarTodos, ObtenerPorId, Crear, Actualizar, Eliminar
    - Métodos auxiliares: ExisteUsuario, ExisteEmail, TieneEntidadesAsignadas
    - Usar Dapper con parámetros para prevenir SQL injection
    - _Requerimientos: 3.1, 3.3, 3.4, 3.8_
  
  - [ ] 3.2 Crear EntidadRepository
    - Métodos: ListarTodas, ObtenerPorId, Crear, Actualizar, Eliminar
    - Métodos auxiliares: ExisteClave, TieneAdministradoresAsignados
    - _Requerimientos: 7.1, 7.3, 7.8_
  
  - [ ] 3.3 Crear LicenciaRepository
    - Métodos: ListarTodas, ObtenerPorAdministrador, Crear, Actualizar
    - Métodos: IncrementarConsumo, DecrementarConsumo, ObtenerDisponibles
    - _Requerimientos: 4.1, 4.2, 4.3, 4.6_
  
  - [ ] 3.4 Crear AsignacionRepository
    - Métodos: AsignarEntidad, DesasignarEntidad, ExisteAsignacion
    - Métodos: ObtenerEntidadesAsignadas, ObtenerEntidadesDisponibles
    - _Requerimientos: 5.1, 5.3, 5.4_
  
  - [ ] 3.5 Crear AuditoriaRepository y otros repositorios
    - AuditoriaRepository: RegistrarOperacion, ListarRegistros
    - AlertasRepository: CrearAlerta, ObtenerActivas, ResolverAlerta
    - ConfiguracionRepository: ObtenerConfiguracion, ActualizarConfiguracion
    - _Requerimientos: 8.1-8.9, 9.1-9.5, 10.1-10.5_

- [ ] 4. Checkpoint - Verificar repositorios
  - Asegurarse de que todos los repositorios compilen sin errores
  - Verificar que las consultas SQL usen parámetros correctamente
  - Preguntar al usuario si hay dudas

- [ ] 5. Implementar servicios de negocio en JELA.API
  - [ ] 5.1 Crear AdministradorService
    - Implementar lógica de negocio para CRUD de administradores
    - Validar unicidad de usuario y email
    - Crear registro de licencias al crear administrador
    - Registrar operaciones en auditoría
    - _Requerimientos: 3.1-3.8, 4.1, 8.1-8.3_
  
  - [ ]* 5.2 Escribir prueba de propiedad para AdministradorService
    - **Propiedad 5: Creación de Administrador con Tipo Correcto**
    - **Valida: Requerimientos 3.1**
  
  - [ ]* 5.3 Escribir prueba de propiedad para validación de campos únicos
    - **Propiedad 6: Validación de Campos Únicos**
    - **Valida: Requerimientos 3.3, 3.4**
  
  - [ ]* 5.4 Escribir prueba de propiedad para prevención de eliminación
    - **Propiedad 7: Prevención de Eliminación con Entidades Asignadas**
    - **Valida: Requerimientos 3.8**
  
  - [ ] 5.5 Crear LicenciaService
    - Implementar lógica para gestión de licencias
    - Validar que no se reduzcan licencias por debajo del consumo
    - Incrementar/decrementar consumo al asignar/desasignar
    - Verificar disponibilidad antes de asignaciones
    - Crear alertas cuando licencias sean bajas
    - _Requerimientos: 4.1-4.7, 9.1-9.2_
  
  - [ ]* 5.6 Escribir prueba de propiedad para incremento de licencias consumidas
    - **Propiedad 9: Incremento de Licencias Consumidas al Asignar**
    - **Valida: Requerimientos 4.2**
  
  - [ ]* 5.7 Escribir prueba de propiedad para decremento de licencias consumidas
    - **Propiedad 10: Decremento de Licencias Consumidas al Desasignar**
    - **Valida: Requerimientos 4.3**
  
  - [ ]* 5.8 Escribir prueba de propiedad para restricción de reducción de licencias
    - **Propiedad 11: Restricción de Reducción de Licencias**
    - **Valida: Requerimientos 4.4**
  
  - [ ]* 5.9 Escribir prueba de propiedad para prevención de asignación sin licencias
    - **Propiedad 12: Prevención de Asignación sin Licencias Disponibles**
    - **Valida: Requerimientos 4.5, 5.2**
  
  - [ ]* 5.10 Escribir prueba de propiedad para cálculo de licencias disponibles
    - **Propiedad 13: Cálculo Correcto de Licencias Disponibles**
    - **Valida: Requerimientos 4.6**

- [ ] 6. Implementar servicios de configuración de entidades
  - [ ] 6.1 Crear EntidadService
    - Implementar lógica de negocio para CRUD de entidades
    - Validar unicidad de clave
    - Prevenir eliminación si tiene administradores asignados
    - _Requerimientos: 7.1-7.8_
  
  - [ ] 6.2 Implementar ConfigurarEntidadNueva en EntidadService
    - Copiar prompts de entidad plantilla a nueva entidad
    - Copiar SLAs de entidad plantilla
    - Copiar categorías de tickets
    - Crear áreas comunes predeterminadas
    - Crear conceptos de pago predeterminados
    - Usar transacción para garantizar atomicidad
    - Registrar operación en auditoría
    - _Requerimientos: 6.1-6.9_
  
  - [ ]* 6.3 Escribir prueba de propiedad para configuración completa de entidad
    - **Propiedad 18: Configuración Completa de Nueva Entidad**
    - **Valida: Requerimientos 6.1, 6.3, 6.5**
  
  - [ ]* 6.4 Escribir prueba de propiedad para prompts mínimos requeridos
    - **Propiedad 19: Prompts Mínimos Requeridos Presentes**
    - **Valida: Requerimientos 6.2**
  
  - [ ]* 6.5 Escribir prueba de propiedad para SLAs de todas las prioridades
    - **Propiedad 20: SLAs para Todas las Prioridades**
    - **Valida: Requerimientos 6.4**
  
  - [ ]* 6.6 Escribir prueba de propiedad para transaccionalidad
    - **Propiedad 23: Transaccionalidad en Configuración de Entidad**
    - **Valida: Requerimientos 6.8**

- [ ] 7. Implementar servicios auxiliares
  - [ ] 7.1 Crear AsignacionService
    - Implementar lógica para asignar/desasignar entidades
    - Verificar licencias disponibles antes de asignar
    - Prevenir asignaciones duplicadas
    - Incrementar/decrementar consumo de licencias
    - Registrar operaciones en auditoría
    - _Requerimientos: 5.1-5.7, 8.6-8.7_
  
  - [ ]* 7.2 Escribir prueba de propiedad para creación de registro de asignación
    - **Propiedad 15: Creación de Registro de Asignación**
    - **Valida: Requerimientos 5.1**
  
  - [ ]* 7.3 Escribir prueba de propiedad para prevención de asignaciones duplicadas
    - **Propiedad 16: Prevención de Asignaciones Duplicadas**
    - **Valida: Requerimientos 5.3**
  
  - [ ] 7.4 Crear AuditoriaService
    - Implementar RegistrarOperacion para todas las operaciones
    - Serializar datos anteriores a JSON
    - Implementar ListarRegistros con filtros
    - _Requerimientos: 8.1-8.10_
  
  - [ ]* 7.5 Escribir prueba de propiedad para registro de auditoría
    - **Propiedad 27: Registro de Auditoría para Todas las Operaciones**
    - **Valida: Requerimientos 8.1-8.8**
  
  - [ ] 7.6 Crear DashboardService y ConfiguracionService
    - DashboardService: Calcular métricas del sistema
    - ConfiguracionService: Gestionar configuración global
    - _Requerimientos: 2.1-2.7, 10.1-10.5_

- [ ] 8. Checkpoint - Verificar servicios
  - Asegurarse de que todos los servicios compilen sin errores
  - Verificar que las transacciones se manejen correctamente
  - Ejecutar pruebas de propiedad implementadas
  - Preguntar al usuario si hay dudas

- [ ] 9. Implementar controladores de API
  - [ ] 9.1 Crear AdministradoresController
    - Endpoints: GET, GET/{id}, POST, PUT/{id}, DELETE/{id}
    - Endpoints: GET/{id}/entidades, GET/{id}/entidades-disponibles
    - Endpoints: POST/{id}/asignar-entidad, DELETE/{id}/desasignar-entidad/{idEntidad}
    - Validar DTOs y manejar errores
    - _Requerimientos: 3.1-3.8, 5.1-5.7_
  
  - [ ] 9.2 Crear EntidadesController
    - Endpoints: GET, GET/{id}, POST, PUT/{id}, DELETE/{id}
    - Endpoint: GET/{id}/administradores
    - Validar DTOs y manejar errores
    - _Requerimientos: 7.1-7.8_
  
  - [ ] 9.3 Crear LicenciasController
    - Endpoints: GET, GET/{idAdministrador}, PUT/{idAdministrador}
    - Validar DTOs y manejar errores
    - _Requerimientos: 4.1-4.7_
  
  - [ ] 9.4 Crear controladores auxiliares
    - DashboardController: GET /metricas
    - AuditoriaController: GET con filtros
    - AlertasController: GET /activas, PUT /{id}/resolver
    - ConfiguracionController: GET, PUT
    - _Requerimientos: 2.1-2.7, 8.1-8.10, 9.1-9.5, 10.1-10.5_

- [ ] 10. Configurar proyecto WebForms Panel Admin
  - Crear nuevo proyecto ASP.NET WebForms en solución
  - Configurar referencias a DevExpress v22.2
  - Crear estructura de carpetas según estándares
  - Configurar Web.config con conexión a base de datos
  - Crear MasterPage base para el panel admin
  - _Requerimientos: 12.1-12.12_

- [ ] 11. Implementar autenticación de super administradores
  - [ ] 11.1 Crear página Login.aspx
    - Formulario con usuario y contraseña
    - Validación de campos requeridos
    - Aplicar estándares de UI (color de fondo #E4EFFA)
    - _Requerimientos: 1.1, 12.1_
  
  - [ ] 11.2 Implementar lógica de autenticación en Login.aspx.vb
    - Verificar credenciales contra conf_usuarios
    - Verificar que TipoUsuario = 'SuperAdministrador'
    - Crear sesión si autenticación exitosa
    - Mostrar errores si credenciales inválidas
    - _Requerimientos: 1.2, 1.3, 1.4_
  
  - [ ]* 11.3 Escribir prueba de propiedad para autenticación válida
    - **Propiedad 1: Autenticación de Super Administradores**
    - **Valida: Requerimientos 1.2**
  
  - [ ]* 11.4 Escribir prueba de propiedad para rechazo de credenciales inválidas
    - **Propiedad 2: Rechazo de Credenciales Inválidas**
    - **Valida: Requerimientos 1.3**
  
  - [ ] 11.5 Implementar verificación de sesión en MasterPage
    - Verificar sesión activa en Page_Load
    - Redirigir a Login si sesión expirada
    - _Requerimientos: 1.5_

- [ ] 12. Implementar Dashboard.aspx
  - [ ] 12.1 Crear interfaz de Dashboard
    - Tarjetas de métricas con Bootstrap
    - ASPxGridView para alertas activas
    - Aplicar estándares de UI
    - _Requerimientos: 2.1-2.7, 9.3, 12.1-12.12_
  
  - [ ] 12.2 Implementar lógica de Dashboard.aspx.vb
    - Cargar métricas desde API /dashboard/metricas
    - Cargar alertas desde API /alertas/activas
    - Configurar grid con FuncionesGridWeb
    - _Requerimientos: 2.1-2.7, 9.3_
  
  - [ ]* 12.3 Escribir prueba de propiedad para cálculo de métricas
    - **Propiedad 4: Cálculo Correcto de Métricas del Dashboard**
    - **Valida: Requerimientos 2.1-2.5**

- [ ] 13. Implementar Administradores.aspx
  - [ ] 13.1 Crear interfaz de Administradores
    - ASPxGridView con columnas dinámicas
    - ASPxPopupControl para crear/editar
    - Toolbar con acciones CRUD
    - Aplicar estándares de UI
    - _Requerimientos: 3.1-3.8, 12.1-12.12_
  
  - [ ] 13.2 Implementar formulario de administrador en popup
    - Campos: Usuario, Email, Nombre Completo, Licencias Totales, Activo
    - Validadores DevExpress para campos requeridos
    - Validación de formato de email
    - _Requerimientos: 3.2, 3.3, 3.4_
  
  - [ ] 13.3 Implementar lógica de Administradores.aspx.vb
    - Cargar datos desde API /administradores
    - Implementar eventos de grid (RowCommand, DataBound)
    - Implementar guardar (crear/actualizar)
    - Implementar eliminar con confirmación
    - Generar columnas dinámicamente
    - Aplicar FuncionesGridWeb.SUMColumn
    - _Requerimientos: 3.1-3.8, 12.9, 12.12_
  
  - [ ] 13.4 Crear archivo CSS modules/administradores.css
    - Estilos específicos para la página
    - Seguir estándares de UI
    - _Requerimientos: 12.3_
  
  - [ ] 13.5 Crear archivo JS modules/administradores.js
    - Lógica de cliente para interacciones
    - Seguir estándares de UI
    - _Requerimientos: 12.4_

- [ ] 14. Implementar Entidades.aspx
  - [ ] 14.1 Crear interfaz de Entidades
    - ASPxGridView con columnas dinámicas
    - ASPxPopupControl para crear/editar
    - Toolbar con acciones CRUD
    - Aplicar estándares de UI
    - _Requerimientos: 7.1-7.8, 12.1-12.12_
  
  - [ ] 14.2 Implementar formulario de entidad en popup
    - Campos: Clave, Alias, Razón Social, Tipo Condominio, Número Unidades, Fecha Constitución, Activo
    - Validadores DevExpress
    - ASPxComboBox para Tipo Condominio
    - _Requerimientos: 7.2, 7.3_
  
  - [ ] 14.3 Implementar lógica de Entidades.aspx.vb
    - Cargar datos desde API /entidades
    - Implementar eventos de grid
    - Implementar guardar (crear con configuración automática/actualizar)
    - Implementar eliminar con confirmación
    - Generar columnas dinámicamente
    - _Requerimientos: 7.1-7.8, 12.9_
  
  - [ ] 14.4 Crear archivos CSS y JS para Entidades
    - modules/entidades.css
    - modules/entidades.js
    - _Requerimientos: 12.3, 12.4_

- [ ] 15. Checkpoint - Verificar páginas principales
  - Asegurarse de que todas las páginas compilen sin errores
  - Verificar que se apliquen los estándares de UI
  - Probar navegación entre páginas
  - Preguntar al usuario si hay dudas

- [ ] 16. Implementar AsignacionEntidades.aspx
  - [ ] 16.1 Crear interfaz de AsignacionEntidades
    - ASPxComboBox para seleccionar administrador
    - ASPxGridView para entidades disponibles
    - ASPxGridView para entidades asignadas
    - Botones para asignar/desasignar
    - Mostrar licencias disponibles del administrador
    - _Requerimientos: 5.1-5.7, 12.1-12.12_
  
  - [ ] 16.2 Implementar lógica de AsignacionEntidades.aspx.vb
    - Cargar administradores en combo
    - Cargar entidades disponibles y asignadas según administrador seleccionado
    - Implementar asignar entidad (verificar licencias)
    - Implementar desasignar entidad
    - Actualizar grids después de operaciones
    - _Requerimientos: 5.1-5.7_
  
  - [ ] 16.3 Crear archivos CSS y JS para AsignacionEntidades
    - modules/asignacion-entidades.css
    - modules/asignacion-entidades.js
    - _Requerimientos: 12.3, 12.4_

- [ ] 17. Implementar Licencias.aspx
  - [ ] 17.1 Crear interfaz de Licencias
    - ASPxGridView mostrando administrador, licencias totales, consumidas, disponibles
    - ASPxPopupControl para modificar licencias
    - Badges de colores para alertas (rojo si disponibles = 0, amarillo si < 2)
    - _Requerimientos: 4.1-4.7, 9.1-9.2, 12.1-12.12_
  
  - [ ] 17.2 Implementar formulario de modificación de licencias
    - Mostrar información actual (solo lectura)
    - Campo para nuevas licencias totales (ASPxSpinEdit)
    - Validar que no sea menor a licencias consumidas
    - _Requerimientos: 4.4_
  
  - [ ] 17.3 Implementar lógica de Licencias.aspx.vb
    - Cargar datos desde API /licencias
    - Implementar modificar licencias
    - Mostrar alertas visuales según disponibilidad
    - Generar columnas dinámicamente
    - _Requerimientos: 4.1-4.7_
  
  - [ ] 17.4 Crear archivos CSS y JS para Licencias
    - modules/licencias.css (incluir estilos para badges de alertas)
    - modules/licencias.js
    - _Requerimientos: 12.3, 12.4_

- [ ] 18. Implementar Auditoria.aspx
  - [ ] 18.1 Crear interfaz de Auditoría
    - Filtros de fecha arriba del grid (ASPxDateEdit)
    - Filtro de tipo de operación (ASPxComboBox)
    - ASPxGridView con registros de auditoría
    - ASPxPopupControl para ver detalles JSON
    - _Requerimientos: 8.1-8.10, 12.1-12.12_
  
  - [ ] 18.2 Implementar lógica de Auditoria.aspx.vb
    - Cargar datos desde API /auditoria con filtros
    - Implementar filtrado por fecha y tipo
    - Mostrar detalles JSON en popup
    - Generar columnas dinámicamente
    - _Requerimientos: 8.10_
  
  - [ ] 18.3 Crear archivos CSS y JS para Auditoría
    - modules/auditoria.css
    - modules/auditoria.js
    - _Requerimientos: 12.3, 12.4_

- [ ] 19. Implementar Configuracion.aspx
  - [ ] 19.1 Crear interfaz de Configuración
    - ASPxComboBox para seleccionar entidad plantilla
    - Botón Guardar
    - Mostrar información de la entidad plantilla actual
    - _Requerimientos: 10.1-10.5, 12.1-12.12_
  
  - [ ] 19.2 Implementar lógica de Configuracion.aspx.vb
    - Cargar configuración desde API /configuracion
    - Cargar entidades activas en combo
    - Implementar guardar configuración
    - Validar que entidad exista y esté activa
    - _Requerimientos: 10.1-10.5_
  
  - [ ] 19.3 Crear archivos CSS y JS para Configuración
    - modules/configuracion.css
    - modules/configuracion.js
    - _Requerimientos: 12.3, 12.4_

- [ ] 20. Implementar funcionalidad de exportación
  - [ ] 20.1 Configurar exportación en todos los grids
    - Habilitar ExportToPdf y ExportToXlsx en toolbars
    - Configurar SettingsExport en todos los ASPxGridView
    - _Requerimientos: 13.1-13.5_
  
  - [ ] 20.2 Implementar eventos de exportación
    - Manejar eventos BeforeExport si se necesita personalización
    - Aplicar filtros actuales a la exportación
    - _Requerimientos: 13.5_

- [ ] 21. Implementar manejo de errores y validación
  - [ ] 21.1 Crear helper de manejo de errores en WebForms
    - Método para mostrar mensajes de error amigables
    - Método para parsear errores de API
    - Método para mostrar mensajes de éxito
    - _Requerimientos: 14.1-14.7_
  
  - [ ] 21.2 Implementar manejo de errores en API
    - Middleware para capturar excepciones
    - Formatear respuestas de error según tipo
    - Logging de errores
    - _Requerimientos: 14.1-14.7_
  
  - [ ]* 21.3 Escribir prueba de propiedad para validación de formularios
    - **Propiedad 36: Validación de Formularios Antes de Envío**
    - **Valida: Requerimientos 14.2, 14.7**
  
  - [ ]* 21.4 Escribir prueba de propiedad para mensajes de error de unicidad
    - **Propiedad 37: Mensajes de Error para Restricciones de Unicidad**
    - **Valida: Requerimientos 14.3**

- [ ] 22. Checkpoint - Verificar funcionalidad completa
  - Probar todos los flujos de usuario
  - Verificar que todas las validaciones funcionen
  - Verificar que se registren todas las operaciones en auditoría
  - Verificar que se generen alertas correctamente
  - Preguntar al usuario si hay dudas

- [ ] 23. Implementar pruebas de integración
  - [ ] 23.1 Crear pruebas de integración para flujo completo de administrador
    - Crear administrador → Verificar licencias creadas → Asignar entidad → Verificar consumo incrementado
    - _Requerimientos: 3.1, 4.1, 4.2, 5.1_
  
  - [ ] 23.2 Crear pruebas de integración para flujo completo de entidad
    - Crear entidad → Verificar configuración automática → Asignar a administrador → Verificar en JelaWeb
    - _Requerimientos: 7.1, 6.1-6.7, 5.1, 11.1-11.4_
  
  - [ ] 23.3 Crear pruebas de integración para transaccionalidad
    - Simular fallo en configuración de entidad → Verificar rollback completo
    - _Requerimientos: 6.8, 11.5_

- [ ] 24. Optimización y refinamiento
  - [ ] 24.1 Crear índices adicionales en base de datos
    - Analizar consultas frecuentes
    - Crear índices para optimizar rendimiento
    - _Requerimientos: 15.4_
  
  - [ ] 24.2 Implementar caché para configuración del sistema
    - Cachear IdEntidadPlantilla
    - Invalidar caché al actualizar configuración
    - _Requerimientos: 15.1-15.3_
  
  - [ ] 24.3 Optimizar operaciones de copia de configuración
    - Usar inserciones por lotes para prompts, SLAs, categorías
    - _Requerimientos: 15.5_

- [ ] 25. Documentación y despliegue
  - [ ] 25.1 Crear documentación de usuario
    - Guía de uso del panel admin
    - Capturas de pantalla de cada módulo
    - Explicación de flujos principales
  
  - [ ] 25.2 Crear documentación técnica
    - Diagrama de arquitectura
    - Documentación de API endpoints
    - Guía de despliegue
  
  - [ ] 25.3 Preparar scripts de despliegue
    - Script SQL para crear tablas
    - Script SQL para datos iniciales
    - Configuración de Web.config para producción

- [ ] 26. Checkpoint final - Revisión completa
  - Ejecutar todas las pruebas (unitarias, propiedades, integración)
  - Verificar cobertura de pruebas (objetivo: 80%+)
  - Revisar cumplimiento de estándares de UI
  - Verificar que todas las 38 propiedades estén implementadas
  - Realizar pruebas de usuario final
  - Preguntar al usuario si está listo para despliegue

## Notas

- Las tareas marcadas con `*` son opcionales y pueden omitirse para un MVP más rápido
- Cada tarea referencia los requerimientos específicos que implementa
- Los checkpoints aseguran validación incremental
- Las pruebas de propiedad validan correctitud universal
- Las pruebas unitarias validan ejemplos específicos y casos borde