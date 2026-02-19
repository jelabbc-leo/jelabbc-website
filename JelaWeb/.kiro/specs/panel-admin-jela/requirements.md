# Documento de Requerimientos - Panel Admin JELA

## Introducción

El Panel Admin JELA es un sistema de gestión de licencias y administradores para la plataforma multi-tenant de jela-api-logistica JELA. Actualmente, JelaWeb permite a los administradores gestionar múltiples condominios sin ningún control de licencias ni proceso automatizado de alta. Este sistema proporcionará a los super-administradores de JELA herramientas para gestionar cuentas de administradores de condominios, controlar la asignación de licencias, automatizar la configuración de entidades y monitorear el uso del sistema.

El sistema se construirá como una aplicación ASP.NET WebForms separada (VB.NET Framework 4.8) que se integra con la JELA.API existente (.NET 8 C#) y comparte la base de datos MySQL `jela_qa` con JelaWeb.

## Glosario

- **Sistema**: La aplicación Panel Admin JELA
- **Super_Administrador**: Miembro del personal de JELA con acceso completo al sistema para gestionar todos los administradores y licencias
- **Administrador_Condominio**: Usuario que gestiona una o más entidades de condominio
- **Entidad**: Una organización de condominio o gestión de propiedades en el sistema (almacenada en `cat_entidades`)
- **Licencia**: Permiso para gestionar una entidad; cada licencia permite gestionar exactamente una entidad
- **Entidad_Plantilla**: Una entidad preconfigurada utilizada como fuente para copiar prompts, SLAs y catálogos a nuevas entidades
- **Prompt**: Instrucción de IA almacenada en `conf_ticket_prompts` requerida para la funcionalidad de chat y tickets
- **SLA**: Configuración de Acuerdo de Nivel de Servicio almacenada en `conf_ticket_sla`
- **Registro_Auditoria**: Registro de todas las operaciones administrativas almacenadas en `conf_auditoria_admin`
- **Consumo_Licencias**: Número de licencias actualmente en uso por un administrador
- **Licencias_Disponibles**: Total de licencias menos licencias consumidas para un administrador

## Requerimientos

### Requerimiento 1: Autenticación de Super Administrador

**Historia de Usuario:** Como super administrador de JELA, quiero autenticarme de forma segura en el panel admin, para que solo el personal autorizado pueda gestionar el sistema.

#### Criterios de Aceptación

1. WHEN un usuario accede al panel admin THEN EL Sistema SHALL mostrar una página de login requiriendo usuario y contraseña
2. WHEN un usuario envía credenciales válidas de super administrador THEN EL Sistema SHALL otorgar acceso al panel admin
3. WHEN un usuario envía credenciales inválidas THEN EL Sistema SHALL mostrar un mensaje de error y prevenir el acceso
4. WHEN un usuario no es super administrador THEN EL Sistema SHALL denegar el acceso incluso con credenciales válidas
5. WHEN la sesión de un super administrador expira THEN EL Sistema SHALL redirigir a la página de login

### Requerimiento 2: Dashboard y Métricas del Sistema

**Historia de Usuario:** Como super administrador, quiero ver métricas del sistema en un dashboard, para poder monitorear la salud general y el uso del sistema.

#### Criterios de Aceptación

1. WHEN un super administrador accede al dashboard THEN EL Sistema SHALL mostrar el número total de administradores de condominios
2. WHEN se muestran métricas del dashboard THEN EL Sistema SHALL mostrar el número total de entidades en el sistema
3. WHEN se muestran métricas del dashboard THEN EL Sistema SHALL mostrar el total de licencias asignadas a todos los administradores
4. WHEN se muestran métricas del dashboard THEN EL Sistema SHALL mostrar el total de licencias consumidas en todos los administradores
5. WHEN se muestran métricas del dashboard THEN EL Sistema SHALL mostrar el total de licencias disponibles (asignadas menos consumidas)
6. WHEN se muestran métricas del dashboard THEN EL Sistema SHALL mostrar el número de entidades activas versus inactivas
7. WHEN los datos del dashboard cambian THEN EL Sistema SHALL actualizar las métricas en tiempo real al refrescar la página

### Requerimiento 3: Gestión de Administradores de Condominios

**Historia de Usuario:** Como super administrador, quiero crear y gestionar cuentas de administradores de condominios, para poder controlar quién tiene acceso para gestionar entidades.

#### Criterios de Aceptación

1. WHEN un super administrador crea un nuevo administrador THEN EL Sistema SHALL almacenar el registro en `conf_usuarios` con `TipoUsuario = 'AdministradorCondominios'`
2. WHEN se crea un administrador THEN EL Sistema SHALL requerir usuario, email, nombre completo y cantidad inicial de licencias
3. WHEN se crea un administrador THEN EL Sistema SHALL validar que el email sea único en el sistema
4. WHEN se crea un administrador THEN EL Sistema SHALL validar que el usuario sea único en el sistema
5. WHEN un super administrador edita un administrador THEN EL Sistema SHALL permitir actualizar email, nombre completo, estado activo y cantidad de licencias
6. WHEN un super administrador desactiva un administrador THEN EL Sistema SHALL establecer `Activo = 0` en `conf_usuarios`
7. WHEN se muestran administradores THEN EL Sistema SHALL mostrar usuario, email, nombre completo, cantidad de licencias, licencias consumidas, licencias disponibles y estado activo
8. WHEN un super administrador elimina un administrador THEN EL Sistema SHALL prevenir la eliminación si el administrador tiene entidades asignadas

### Requerimiento 4: Asignación y Control de Licencias

**Historia de Usuario:** Como super administrador, quiero asignar y controlar licencias para administradores, para poder gestionar la capacidad del sistema y facturación.

#### Criterios de Aceptación

1. WHEN se crea un administrador THEN EL Sistema SHALL almacenar la cantidad de licencias en la tabla `conf_licencias`
2. WHEN se asigna una entidad a un administrador THEN EL Sistema SHALL incrementar `LicenciasConsumidas` en `conf_licencias`
3. WHEN se desasigna una entidad de un administrador THEN EL Sistema SHALL decrementar `LicenciasConsumidas` en `conf_licencias`
4. WHEN un super administrador intenta reducir licencias THEN EL Sistema SHALL prevenir la reducción por debajo de `LicenciasConsumidas` actual
5. WHEN un administrador intenta gestionar más entidades que licencias disponibles THEN EL Sistema SHALL prevenir la asignación y mostrar un error
6. WHEN se muestra información de licencias THEN EL Sistema SHALL calcular licencias disponibles como `LicenciasTotales - LicenciasConsumidas`
7. WHEN cambia la asignación de licencias THEN EL Sistema SHALL actualizar el timestamp `FechaModificacion` en `conf_licencias`

### Requerimiento 5: Asignación de Entidades a Administradores

**Historia de Usuario:** Como super administrador, quiero asignar entidades a administradores, para que puedan gestionar sus condominios asignados.

#### Criterios de Aceptación

1. WHEN un super administrador asigna una entidad a un administrador THEN EL Sistema SHALL crear un registro en `conf_usuario_entidades`
2. WHEN se asigna una entidad THEN EL Sistema SHALL verificar que el administrador tenga licencias disponibles
3. WHEN se asigna una entidad THEN EL Sistema SHALL prevenir asignaciones duplicadas (mismo usuario y entidad)
4. WHEN un super administrador desasigna una entidad THEN EL Sistema SHALL eliminar el registro de `conf_usuario_entidades`
5. WHEN se muestran asignaciones de entidades THEN EL Sistema SHALL mostrar nombre de entidad, nombre de administrador y fecha de asignación
6. WHEN un administrador no tiene licencias disponibles THEN EL Sistema SHALL prevenir nuevas asignaciones de entidades
7. WHEN se desasigna una entidad THEN EL Sistema SHALL decrementar el contador de licencias consumidas del administrador

### Requerimiento 6: Configuración Automatizada de Entidades

**Historia de Usuario:** Como super administrador, quiero que las nuevas entidades se configuren automáticamente con los datos requeridos, para que sean inmediatamente funcionales para los administradores.

#### Criterios de Aceptación

1. WHEN se crea una nueva entidad THEN EL Sistema SHALL copiar todos los prompts de la entidad plantilla a `conf_ticket_prompts` con el nuevo `IdEntidad`
2. WHEN se copian prompts THEN EL Sistema SHALL asegurar que como mínimo se copien los prompts `ChatWebSistema`, `AnalisisTicket`, `ResolucionTicket` y `CategorizacionTicket`
3. WHEN se crea una nueva entidad THEN EL Sistema SHALL copiar todas las configuraciones SLA de la entidad plantilla a `conf_ticket_sla`
4. WHEN se copian SLAs THEN EL Sistema SHALL copiar configuraciones para todos los niveles de prioridad (Baja, Media, Alta, Crítica)
5. WHEN se crea una nueva entidad THEN EL Sistema SHALL copiar categorías de tickets de la entidad plantilla a `cat_categorias_ticket`
6. WHEN se crea una nueva entidad THEN EL Sistema SHALL crear registros de áreas predeterminadas en `cat_areas_comunes` (Alberca, Salón de Eventos, Gimnasio)
7. WHEN se crea una nueva entidad THEN EL Sistema SHALL crear conceptos de pago predeterminados en `cat_conceptos_cuota` (Cuota de Mantenimiento, Agua)
8. WHEN falla la configuración de entidad THEN EL Sistema SHALL revertir todos los cambios y mostrar un mensaje de error
9. WHEN se completa la configuración de entidad THEN EL Sistema SHALL registrar la operación en `conf_auditoria_admin`

### Requerimiento 7: Gestión de Entidades

**Historia de Usuario:** Como super administrador, quiero crear y gestionar entidades, para poder dar de alta nuevos condominios en el sistema.

#### Criterios de Aceptación

1. WHEN un super administrador crea una nueva entidad THEN EL Sistema SHALL almacenar el registro en `cat_entidades`
2. WHEN se crea una entidad THEN EL Sistema SHALL requerir Clave, Alias, RazonSocial y TipoCondominio
3. WHEN se crea una entidad THEN EL Sistema SHALL validar que Clave sea única
4. WHEN se crea una entidad THEN EL Sistema SHALL establecer `Activo = 1` y `EsCondominio = 1` por defecto
5. WHEN un super administrador edita una entidad THEN EL Sistema SHALL permitir actualizar todos los campos de entidad excepto Id
6. WHEN un super administrador desactiva una entidad THEN EL Sistema SHALL establecer `Activo = 0` en `cat_entidades`
7. WHEN se muestran entidades THEN EL Sistema SHALL mostrar Clave, Alias, RazonSocial, TipoCondominio, NumeroUnidades y estado Activo
8. WHEN un super administrador elimina una entidad THEN EL Sistema SHALL prevenir la eliminación si la entidad tiene administradores asignados

### Requerimiento 8: Registro de Auditoría

**Historia de Usuario:** Como super administrador, quiero que todas las operaciones administrativas se registren, para poder rastrear cambios y mantener responsabilidad.

#### Criterios de Aceptación

1. WHEN se crea un administrador THEN EL Sistema SHALL registrar la operación en `conf_auditoria_admin` con tipo de acción "CrearAdministrador"
2. WHEN se modifica un administrador THEN EL Sistema SHALL registrar la operación con tipo de acción "ModificarAdministrador"
3. WHEN se elimina un administrador THEN EL Sistema SHALL registrar la operación con tipo de acción "EliminarAdministrador"
4. WHEN se crea una entidad THEN EL Sistema SHALL registrar la operación con tipo de acción "CrearEntidad"
5. WHEN se modifica una entidad THEN EL Sistema SHALL registrar la operación con tipo de acción "ModificarEntidad"
6. WHEN se asigna una entidad a un administrador THEN EL Sistema SHALL registrar la operación con tipo de acción "AsignarEntidad"
7. WHEN se desasigna una entidad de un administrador THEN EL Sistema SHALL registrar la operación con tipo de acción "DesasignarEntidad"
8. WHEN se modifican licencias THEN EL Sistema SHALL registrar la operación con tipo de acción "ModificarLicencias"
9. WHEN se registran operaciones THEN EL Sistema SHALL almacenar IdUsuario (super admin), FechaOperacion, TipoOperacion, Descripcion y DatosAnteriores (JSON)
10. WHEN se muestran registros de auditoría THEN EL Sistema SHALL mostrar todos los campos en orden cronológico con filtrado por rango de fechas y tipo de operación

### Requerimiento 9: Sistema de Alertas de Licencias

**Historia de Usuario:** Como super administrador, quiero recibir alertas cuando los administradores se acerquen a los límites de licencias, para poder gestionar proactivamente la capacidad.

#### Criterios de Aceptación

1. WHEN las licencias disponibles de un administrador caen por debajo de 2 THEN EL Sistema SHALL crear una alerta en `conf_alertas_licencias`
2. WHEN un administrador tiene cero licencias disponibles THEN EL Sistema SHALL crear una alerta crítica
3. WHEN se muestra el dashboard THEN EL Sistema SHALL mostrar todas las alertas activas
4. WHEN se resuelve una alerta (se agregan licencias) THEN EL Sistema SHALL marcar la alerta como `Resuelta = 1`
5. WHEN se crea una alerta THEN EL Sistema SHALL almacenar IdAdministrador, TipoAlerta, Mensaje, FechaCreacion y estado Resuelta

### Requerimiento 10: Gestión de Configuración del Sistema

**Historia de Usuario:** Como super administrador, quiero configurar ajustes globales del sistema, para poder controlar comportamientos predeterminados y selecciones de plantillas.

#### Criterios de Aceptación

1. WHEN el sistema se inicializa THEN EL Sistema SHALL cargar la configuración de la tabla `conf_sistema`
2. WHEN se muestra la configuración del sistema THEN EL Sistema SHALL mostrar el ID de entidad plantilla actual
3. WHEN un super administrador cambia la entidad plantilla THEN EL Sistema SHALL actualizar `IdEntidadPlantilla` en `conf_sistema`
4. WHEN un super administrador cambia la entidad plantilla THEN EL Sistema SHALL validar que la entidad exista y esté activa
5. WHEN se modifica la configuración del sistema THEN EL Sistema SHALL registrar el cambio en `conf_auditoria_admin`

### Requerimiento 11: Integración con el Sistema JelaWeb Existente

**Historia de Usuario:** Como arquitecto de sistemas, quiero que el panel admin se integre sin problemas con JelaWeb, para que los cambios se reflejen inmediatamente en el sistema de producción.

#### Criterios de Aceptación

1. WHEN el panel admin modifica `conf_usuarios` THEN EL Sistema SHALL asegurar compatibilidad con la autenticación de JelaWeb
2. WHEN el panel admin modifica `conf_usuario_entidades` THEN EL Sistema SHALL asegurar que el selector de entidades en JelaWeb refleje los cambios inmediatamente
3. WHEN el panel admin crea una entidad THEN EL Sistema SHALL asegurar que JelaWeb pueda acceder a la entidad sin configuración adicional
4. WHEN el panel admin copia prompts THEN EL Sistema SHALL asegurar que el widget de chat en JelaWeb funcione correctamente para la nueva entidad
5. WHEN fallan operaciones de base de datos THEN EL Sistema SHALL revertir transacciones para mantener la integridad de datos

### Requerimiento 12: Cumplimiento de Estándares de Interfaz de Usuario

**Historia de Usuario:** Como arquitecto de sistemas, quiero que el panel admin siga los estándares de UI de JELA, para que proporcione una experiencia de usuario consistente.

#### Criterios de Aceptación

1. EL Sistema SHALL usar el color de fondo `#E4EFFA` para todas las páginas
2. EL Sistema SHALL usar componentes DevExpress v22.2 para todos los grids y formularios
3. EL Sistema SHALL colocar todo el CSS en archivos separados bajo `/Content/css/modules/`
4. EL Sistema SHALL colocar todo el JavaScript en archivos separados bajo `/Scripts/app/modules/`
5. EL Sistema SHALL usar ASPxGridView con `SettingsPager Mode="ShowAllRecords"` para mostrar todos los registros sin paginación
6. EL Sistema SHALL usar ASPxPopupControl para todos los formularios de captura de datos, no páginas separadas
7. EL Sistema SHALL usar nombres de botones contextuales (ej: "Nuevo Administrador" no "Nuevo")
8. EL Sistema SHALL usar PascalCase para todos los nombres de campos de base de datos
9. EL Sistema SHALL generar columnas de grid dinámicamente desde resultados del API
10. EL Sistema SHALL usar filtros de encabezado (`ShowFilterRowMenu="True"`) para todas las columnas filtrables
11. EL Sistema SHALL colocar solo filtros de rango de fechas arriba de los grids; todos los demás filtros SHALL estar en encabezados de grid
12. EL Sistema SHALL usar `FuncionesGridWeb.SUMColumn` en todos los eventos DataBound de grids

### Requerimiento 13: Reportes y Exportación

**Historia de Usuario:** Como super administrador, quiero exportar datos y generar reportes, para poder analizar el uso del sistema y compartir información con stakeholders.

#### Criterios de Aceptación

1. WHEN se visualiza el grid de administradores THEN EL Sistema SHALL proporcionar funcionalidad de exportación a Excel
2. WHEN se visualiza el grid de entidades THEN EL Sistema SHALL proporcionar funcionalidad de exportación a Excel
3. WHEN se visualiza el grid de registro de auditoría THEN EL Sistema SHALL proporcionar funcionalidad de exportación a Excel
4. WHEN se exportan datos THEN EL Sistema SHALL incluir todas las columnas visibles en la exportación
5. WHEN se exportan datos THEN EL Sistema SHALL aplicar los filtros actuales a la exportación
6. WHEN se genera un reporte de uso de licencias THEN EL Sistema SHALL mostrar nombre de administrador, licencias totales, licencias consumidas, licencias disponibles y entidades asignadas
7. WHEN se genera un reporte de configuración de entidades THEN EL Sistema SHALL mostrar nombre de entidad, administradores asignados, cantidad de prompts, cantidad de SLAs y estado de configuración

### Requerimiento 14: Manejo de Errores y Validación

**Historia de Usuario:** Como super administrador, quiero mensajes de error claros y validación, para poder corregir problemas rápidamente y evitar corrupción de datos.

#### Criterios de Aceptación

1. WHEN falla una operación de base de datos THEN EL Sistema SHALL mostrar un mensaje de error amigable para el usuario
2. WHEN falla la validación THEN EL Sistema SHALL resaltar los campos inválidos y mostrar mensajes de error específicos
3. WHEN se viola una restricción de unicidad THEN EL Sistema SHALL mostrar un mensaje indicando qué campo debe ser único
4. WHEN se viola una restricción de clave foránea THEN EL Sistema SHALL mostrar un mensaje explicando la dependencia de relación
5. WHEN una operación violaría reglas de negocio THEN EL Sistema SHALL prevenir la operación y explicar por qué
6. WHEN ocurre un error inesperado THEN EL Sistema SHALL registrar los detalles completos del error y mostrar un mensaje genérico al usuario
7. WHEN falla la validación de formulario THEN EL Sistema SHALL prevenir el envío del formulario hasta que se corrijan todos los errores

### Requerimiento 15: Rendimiento y Escalabilidad

**Historia de Usuario:** Como arquitecto de sistemas, quiero que el panel admin funcione eficientemente, para que pueda manejar números crecientes de administradores y entidades.

#### Criterios de Aceptación

1. WHEN se carga el dashboard THEN EL Sistema SHALL mostrar métricas dentro de 2 segundos
2. WHEN se carga un grid con hasta 1000 registros THEN EL Sistema SHALL renderizar el grid dentro de 3 segundos
3. WHEN se crea una entidad con configuración completa THEN EL Sistema SHALL completar la operación dentro de 5 segundos
4. WHEN se ejecutan consultas de base de datos THEN EL Sistema SHALL usar columnas indexadas para filtrado y joins
5. WHEN se copian datos de configuración THEN EL Sistema SHALL usar operaciones de inserción por lotes para eficiencia
