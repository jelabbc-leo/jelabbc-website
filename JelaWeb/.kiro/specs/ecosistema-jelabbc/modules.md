# Especificaciones de Módulos - Ecosistema JELABBC

## IMPORTANTE

Este sistema usa un patrón de API dinámico donde un solo controlador (CrudController) maneja todas las operaciones CRUD para cualquier tabla de la base de datos.

## MÓDULO 1: ENTIDADES Y USUARIOS

### 1.1 DESCRIPCIÓN

Módulo para gestión multi-organización con roles y permisos granulares.

### 1.2 TABLAS PRINCIPALES (Base de datos jela_qa)

#### Tabla: catentidades

Tabla principal de organizaciones en el sistema.

**Campos principales**:
- `Id` INT PRIMARY KEY AUTO_INCREMENT
- `TipoEntidad` VARCHAR(50) - Valores: 'Colonia', 'Agricola', 'Municipal', 'Proveedor'
- `Nombre` VARCHAR(200) NOT NULL
- `RFC` VARCHAR(20)
- `Direccion` TEXT
- `Telefono` VARCHAR(50)
- `Email` VARCHAR(200)
- `Logo` VARCHAR(500) - URL del logo
- `Activo` TINYINT(1) DEFAULT 1
- `FechaCreacion` DATETIME DEFAULT CURRENT_TIMESTAMP

#### Tabla: confusuarios

Usuarios del sistema.

**Campos principales**:
- `Id` INT PRIMARY KEY AUTO_INCREMENT
- `IdEntidad` INT - FK a catentidades
- `Nombre` VARCHAR(200) NOT NULL
- `Email` VARCHAR(200) UNIQUE NOT NULL
- `Password` VARCHAR(255) - Hash bcrypt
- `Telefono` VARCHAR(50)
- `IdRol` INT - FK a confroles
- `Activo` TINYINT(1) DEFAULT 1
- `BiometriaHabilitada` TINYINT(1) DEFAULT 0
- `UltimoAcceso` DATETIME
- `FechaCreacion` DATETIME DEFAULT CURRENT_TIMESTAMP

#### Tabla: confroles

Roles del sistema.

**Campos**:
- `Id` INT PRIMARY KEY AUTO_INCREMENT
- `Nombre` VARCHAR(100) NOT NULL
- `Descripcion` TEXT
- `Permisos` TEXT - JSON con permisos
- `Activo` TINYINT(1) DEFAULT 1

### 1.3 ENDPOINTS API (CRUD DINÁMICO)

Todas las operaciones se realizan a través del CrudController genérico:

#### GET: Consultar entidades

```http
GET /api/CRUD?strQuery=SELECT * FROM catentidades WHERE Activo = 1
```

**Respuesta**:
```json
{
  "Success": true,
  "Data": [
    {
      "Id": { "Valor": 1, "Tipo": "System.Int32" },
      "Nombre": { "Valor": "Residencial Los Pinos", "Tipo": "System.String" },
      "TipoEntidad": { "Valor": "Colonia", "Tipo": "System.String" },
      "Activo": { "Valor": true, "Tipo": "System.Boolean" }
    }
  ]
}
```

#### POST: Crear nueva entidad

```http
POST /api/CRUD/catentidades
Content-Type: application/json

{
  "Campos": {
    "TipoEntidad": { "Valor": "Colonia", "Tipo": "System.String" },
    "Nombre": { "Valor": "Residencial Los Pinos", "Tipo": "System.String" },
    "Email": { "Valor": "contacto@lospinos.com", "Tipo": "System.String" },
    "Activo": { "Valor": true, "Tipo": "System.Boolean" }
  }
}
```

#### PUT: Actualizar entidad

```http
PUT /api/CRUD/catentidades/1
Content-Type: application/json

{
  "Campos": {
    "Nombre": { "Valor": "Residencial Las Palmas Actualizado", "Tipo": "System.String" }
  }
}
```

#### DELETE: Eliminar entidad

```http
DELETE /api/CRUD?table=catentidades&idField=Id&idValue=1
```

### 1.4 INTERFAZ WEB (MÓDULO 1)

**Pantallas mínimas**:
1. Listado de entidades (ASPxGridView)
2. Popup modal para alta/edición de entidad
3. Listado de usuarios por entidad
4. Popup modal para alta/edición de usuario
5. Gestión de roles y permisos

**Componentes DevExpress sugeridos**:
- ASPxGridView (listados)
- ASPxPopupControl (formularios)
- ASPxComboBox (selección de entidad/rol)
- ASPxTextBox, ASPxMemo (captura de datos)
- ASPxButton (acciones)

### 1.5 INTERFAZ MÓVIL (MÓDULO 1)

**Funcionalidad limitada en móvil**:
- Consulta de perfil de usuario
- Cambio de contraseña
- Configuración de biometría
- Consulta de entidad asociada

### 1.6 AUTOMATIZACIONES n8n (MÓDULO 1)

**Ejemplos de workflows n8n relacionados con este módulo**:
1. Notificación de bienvenida al crear usuario
2. Alerta de inactividad de usuarios
3. Reporte semanal de nuevas entidades
4. Sincronización con sistemas externos



---

## MÓDULO 2: CATÁLOGOS BASE (IMPLEMENTADO)

### 2.1 DESCRIPCIÓN

Módulo de catálogos base del sistema. Todas las páginas siguen los estándares de UI definidos.

### 2.2 PÁGINAS IMPLEMENTADAS

#### 2.2.1 Roles (`/Views/Catalogos/Roles.aspx`)
- **Servicio**: `RolService.vb`
- **Tabla**: `conf_roles`
- **Funcionalidad**: CRUD de roles con permisos JSON
- **Estándares**: ✅ Grid sin paginación, ✅ DataBound con FuncionesGridWeb

#### 2.2.2 Unidades (`/Views/Catalogos/Unidades.aspx`)
- **Servicio**: `UnidadService.vb`
- **Tabla**: `cat_unidades`
- **Funcionalidad**: CRUD de unidades/departamentos por entidad
- **Campos**: Código, Nombre, Entidad, Torre, Edificio, Piso, Número, Superficie, NumeroResidentes
- **Estándares**: ✅ Grid sin paginación, ✅ DataBound con FuncionesGridWeb

#### 2.2.3 Proveedores (`/Views/Catalogos/Proveedores.aspx`)
- **Servicio**: `ProveedorService.vb`
- **Tabla**: `cat_proveedores`
- **Funcionalidad**: CRUD de proveedores con datos fiscales y contacto
- **Campos**: RazonSocial, NombreComercial, RFC, Dirección completa, Contacto
- **Estándares**: ✅ Grid sin paginación, ✅ DataBound con FuncionesGridWeb

#### 2.2.4 Categorías de Ticket (`/Views/Catalogos/CategoriasTicket.aspx`)
- **Servicio**: `CategoriaTicketService.vb`
- **Tabla**: `cat_categorias_ticket`
- **Funcionalidad**: CRUD de categorías con configuración SLA
- **Campos**: Nombre, Descripción, IconoClase, Color, Activo
- **Estándares**: ✅ Grid sin paginación, ✅ DataBound con FuncionesGridWeb

---

## MÓDULO 3: CATÁLOGOS AGRÍCOLAS (IMPLEMENTADO)

### 3.1 DESCRIPCIÓN

Catálogos específicos para el sector agrícola del ecosistema.

### 3.2 PÁGINAS IMPLEMENTADAS

#### 3.2.1 Parcelas (`/Views/Catalogos/Parcelas.aspx`)
- **Servicio**: `ParcelaService.vb`
- **Tabla**: `cat_parcelas`
- **Funcionalidad**: CRUD de parcelas agrícolas con geolocalización
- **Campos**: Nombre, Descripción, Superficie, UnidadSuperficie, Latitud, Longitud, EntidadId
- **Estándares**: ✅ Grid sin paginación, ✅ DataBound con FuncionesGridWeb

#### 3.2.2 Fitosanitarios (`/Views/Catalogos/Fitosanitarios.aspx`)
- **Servicio**: `FitosanitarioService.vb`
- **Tabla**: `cat_fitosanitarios`
- **Funcionalidad**: CRUD de productos fitosanitarios
- **Campos**: Nombre, NombreComercial, TipoProducto, Fabricante, IngredienteActivo, Concentracion, DosisRecomendada, TiempoCarencia, Toxicidad, Stock
- **Tipos de Producto**: Insecticida, Fungicida, Herbicida, Fertilizante, Regulador, Otro
- **Estándares**: ✅ Grid sin paginación, ✅ DataBound con FuncionesGridWeb

#### 3.2.3 Tipos de Sensor (`/Views/Catalogos/TiposSensor.aspx`)
- **Servicio**: `TipoSensorService.vb`
- **Tabla**: `cat_tipos_sensor`
- **Funcionalidad**: CRUD de tipos de sensores IoT
- **Campos**: Nombre, Descripción, UnidadMedida, SimboloUnidad, UmbralMinimo, UmbralMaximo
- **Estándares**: ✅ Grid sin paginación, ✅ DataBound con FuncionesGridWeb

---

## MÓDULO 4: FORMULARIOS DINÁMICOS (IMPLEMENTADO)

### 4.1 DESCRIPCIÓN

Sistema de formularios dinámicos con diseñador visual estilo Visual Studio. Ubicado en `/Views/Config/` para configuración del sistema.

### 4.2 PÁGINAS IMPLEMENTADAS

#### 4.2.1 Lista de Formularios (`/Views/Config/FormulariosDinamicos.aspx`)
- **Servicio**: `FormularioService.vb`
- **Tabla**: `conf_formularios_dinamicos`
- **Funcionalidad**: Grid de formularios con acciones de toolbar
- **Acciones**: Nuevo, Editar (abre diseñador), Eliminar, Vista Previa
- **Estándares**: ✅ Grid sin paginación, ✅ DataBound con FuncionesGridWeb

#### 4.2.2 Diseñador de Formularios (`/Views/Config/FormularioDisenador.aspx`)
- **Servicio**: `FormularioService.vb`, `DocumentIntelligenceService.vb`
- **Funcionalidad**: Diseñador visual con 3 paneles
  - **Panel Izquierdo (Toolbox)**: Controles arrastrables (TextBox, ComboBox, DateEdit, etc.)
  - **Panel Central (Canvas)**: Área de diseño con drag & drop
  - **Panel Derecho (Properties)**: Propiedades del control seleccionado
- **Características**:
  - Carga/edición de formularios existentes
  - Serialización JSON de campos para el diseñador
  - Soporte para plataformas (web, móvil)
  - Estados: borrador, publicado
  - Integración con DocumentIntelligenceService para limpieza de nombres
- **Assets**: `formulario-disenador.css`, `formulario-disenador.js`

#### 4.2.3 Vista Previa (`/Views/Config/FormularioVistaPrevia.aspx`)
- **Servicio**: `FormularioService.vb`
- **Funcionalidad**: 
  - Renderizado del formulario desde base de datos (por ID)
  - Vista previa desde POST (sin guardar)
  - Soporte para múltiples plataformas (web, móvil)
  - Generación de URL amigable

### 4.3 MODELO DE DATOS

```vb
' FormularioDTO.vb
Public Class FormularioDTO
    Public Property FormularioId As Integer
    Public Property NombreFormulario As String
    Public Property Estado As String           ' borrador, publicado
    Public Property Plataformas As String      ' web,movil (separado por comas)
End Class

Public Class CampoFormularioDTO
    Public Property CampoId As Integer
    Public Property FormularioId As Integer
    Public Property NombreCampo As String
    Public Property EtiquetaCampo As String
    Public Property TipoCampo As String
    Public Property Seccion As String
    Public Property PosicionOrden As Integer
    Public Property AnchoColumna As Integer    ' 1-12 (Bootstrap grid)
    Public Property AlturaCampo As Integer?
    Public Property EsRequerido As Boolean
    Public Property EsVisible As Boolean
    Public Property Placeholder As String
End Class
```

### 4.4 TIPOS DE CAMPO SOPORTADOS

| Tipo | Icono | Descripción |
|------|-------|-------------|
| **Campos de Entrada** | | |
| texto | fa-font | Texto simple (ASPxTextBox) |
| numero | fa-hashtag | Número entero (ASPxSpinEdit) |
| decimal | fa-percentage | Número decimal (ASPxSpinEdit) |
| fecha | fa-calendar | Selector de fecha (ASPxDateEdit) |
| fecha_hora | fa-calendar-alt | Fecha y hora (ASPxDateEdit) |
| hora | fa-clock | Selector de hora (ASPxTimeEdit) |
| dropdown | fa-caret-square-down | Lista desplegable (ASPxComboBox) |
| radio | fa-dot-circle | Opciones excluyentes (ASPxRadioButtonList) |
| checkbox | fa-check-square | Casilla de verificación (ASPxCheckBox) |
| textarea | fa-align-left | Área de texto multilínea (ASPxMemo) |
| foto | fa-camera | Captura de foto (ASPxUploadControl) |
| archivo | fa-file-upload | Carga de archivo (ASPxUploadControl) |
| firma | fa-signature | Captura de firma digital |
| **Botones de Acción** | | |
| boton_guardar | fa-save | Botón para guardar formulario |
| boton_cancelar | fa-times-circle | Botón para cancelar/cerrar |

### 4.5 PROPIEDADES DE CAMPO

| Propiedad | Tipo | Descripción |
|-----------|------|-------------|
| etiqueta | String | Texto visible del campo |
| nombre | String | Nombre interno (auto-generado si vacío) |
| tipo | String | Tipo de campo (ver tabla anterior) |
| seccion | String | Agrupación de campos (default: "General") |
| ancho | Integer | Columnas Bootstrap 1-12 (default: 12) |
| altura | Integer | Altura en px para textarea (80-400) |
| placeholder | String | Texto de ayuda en campo vacío |
| requerido | Boolean | Si el campo es obligatorio |

### 4.6 FLUJO DE TRABAJO

1. **Lista** → Usuario ve todos los formularios en grid
2. **Nuevo/Editar** → Abre diseñador visual
3. **Diseñador** → Drag & drop de campos desde Toolbox
4. **Propiedades** → Configurar cada campo seleccionado
5. **Vista Previa** → Renderizado real del formulario
6. **Guardar** → Persiste formulario y campos en BD

### 4.7 INTERFAZ DEL DISEÑADOR

```
┌─────────────────────────────────────────────────────────────────┐
│ [← Volver]  Nuevo Formulario              [Vista Previa] [Guardar] │
├─────────────────────────────────────────────────────────────────┤
│ Nombre: [________________]  Estado: [Borrador ▼]  □Web □Móvil   │
├──────────┬────────────────────────────────┬─────────────────────┤
│ TOOLBOX  │      CANVAS DE DISEÑO          │    PROPIEDADES      │
│          │                                │                     │
│ □ Texto  │  ┌─────────────────────────┐   │ Etiqueta: [____]    │
│ □ Número │  │ Campo 1                 │   │ Nombre:   [____]    │
│ □ Decimal│  └─────────────────────────┘   │ Tipo:     [____▼]   │
│ □ Fecha  │  ┌─────────────────────────┐   │ Sección:  [____]    │
│ □ Hora   │  │ Campo 2                 │   │ Ancho:    [====12]  │
│ □ Lista  │  └─────────────────────────┘   │ Placeholder:[____]  │
│ □ Casilla│                                │ □ Requerido         │
│ □ Área   │  [Arrastra controles aquí]     │                     │
│ □ Foto   │                                │                     │
│ □ Archivo│                                │                     │
│ □ Firma  │                                │                     │
│ ──────── │                                │                     │
│ □ Guardar│                                │                     │
│ □ Cancelar                                │                     │
└──────────┴────────────────────────────────┴─────────────────────┘
```

---

## MÓDULO 5: TICKETS (IMPLEMENTADO)

### 5.1 DESCRIPCIÓN

Sistema de tickets para atención al cliente.

### 5.2 PÁGINAS IMPLEMENTADAS

#### 5.2.1 Tickets (`/Views/Operacion/Tickets/Tickets.aspx`)
- **Servicio**: `TicketService.vb`
- **DTO**: `TicketDTO.vb`
- **Tabla**: `op_tickets`
- **Funcionalidad**: CRUD de tickets con estados y prioridades
- **Referencia**: Esta página es el modelo de referencia para estándares de UI

---

## MÓDULO 6: GESTIÓN DOCUMENTAL (EN DESARROLLO)

### 6.1 DESCRIPCIÓN

Sistema de gestión de documentos con Azure Document Intelligence.

### 6.2 PÁGINAS IMPLEMENTADAS

#### 6.2.1 Menú de Gestión (`/Views/Operacion/GestionDocumentos/GestionDocumentosMenu.aspx`)
- **Funcionalidad**: Menú de navegación para gestión documental

#### 6.2.2 Captura de Documentos (`/Views/Operacion/CapturaDocumentos.aspx`)
- **Servicio**: `DocumentIntelligenceService.vb`
- **Funcionalidad**: Captura y procesamiento de documentos con IA

---

## MÓDULO 7: IOT (EN DESARROLLO)

### 7.1 DESCRIPCIÓN

Dashboard de monitoreo de sensores IoT.

### 7.2 PÁGINAS IMPLEMENTADAS

#### 7.2.1 Dashboard IoT (`/Views/IOT/IOT.aspx`)
- **Funcionalidad**: Visualización de datos de sensores
- **Dependencias**: Catálogo de Tipos de Sensor

---

## RESUMEN DE IMPLEMENTACIÓN

| Módulo | Estado | Páginas | Servicios |
|--------|--------|---------|-----------|
| 1. Entidades y Usuarios | ✅ Implementado | Entidades.aspx | - |
| 2. Catálogos Base | ✅ Implementado | 4 páginas | 4 servicios |
| 3. Catálogos Agrícolas | ✅ Implementado | 3 páginas | 3 servicios |
| 4. Formularios Dinámicos | ✅ Implementado | 3 páginas | 2 servicios |
| 5. Tickets | ✅ Implementado | 1 página | 1 servicio |
| 6. Gestión Documental | 🔄 En desarrollo | 2 páginas | 1 servicio |
| 7. IoT | 🔄 En desarrollo | 1 página | - |
| 8. Condominios | 🔄 En desarrollo | 0 páginas | DynamicCrudService |

---

## MÓDULO 8: CONDOMINIOS (EN DESARROLLO)

### 8.1 DESCRIPCIÓN

Sistema completo de administración de condominios con gestión de residentes, cuotas, pagos, reservaciones de áreas comunes, control de visitantes y comunicados.

### 8.2 JERARQUÍA DEL SISTEMA

```
cat_entidades (Nivel 1)
    └── Condominio/Fraccionamiento (datos fiscales)
        Ejemplo: "Los Robles S.C.", "Residencial Las Palmas"

cat_subentidades (Nivel 2) - YA EXISTE
    └── Secciones/Torres del fraccionamiento
        Ejemplo: "Robles 1", "Robles 2", "Torre A", "Torre B"
        NOTA: NO se necesita crear cat_secciones

cat_areas_comunes (Nivel 2/3) - NUEVO
    └── Áreas comunes reservables
        - SubEntidadId = NULL → Compartidas por todo el condominio
        - SubEntidadId = ID → Exclusivas de esa torre/sección
        Ejemplo: "Salón de Fiestas", "Alberca", "Gimnasio"

cat_unidades
    └── Unidades privativas de cada SubEntidad
        Ejemplo: "Depto 101", "Casa 15", "Local 3"
```

### 8.3 ESTADO ACTUAL

**Base de datos:** ✅ Scripts SQL completados
**Frontend:** 🔄 Pendiente de desarrollo

### 8.4 TABLAS IMPLEMENTADAS

#### Catálogos (prefijo `cat_`)

| Tabla | Descripción | Estado |
|-------|-------------|--------|
| `cat_areas_comunes` | Áreas comunes reservables (salones, albercas, gimnasios) | ✅ Script listo |
| `cat_residentes` | Propietarios e inquilinos | ✅ Script listo |
| `cat_conceptos_cuota` | Tipos de cuotas (mantenimiento, agua, etc.) | ✅ Script listo |
| `cat_unidades` | Modificada con campos adicionales | ✅ Script listo |

**Nota sobre `cat_areas_comunes`:**
- Campo `SubEntidadId` indica si el área es compartida (NULL) o exclusiva de una torre (ID)
- Campos: `Capacidad`, `CostoReservacion`, `RequiereReservacion`, `HoraApertura`, `HoraCierre`, etc.
- Las secciones/torres se manejan en `cat_subentidades` (ya existente)

#### Operativas (prefijo `op_`)

| Tabla | Descripción | Estado |
|-------|-------------|--------|
| `op_cuotas` | Cuotas generadas por unidad | ✅ Script listo |
| `op_pagos` | Pagos registrados | ✅ Script listo |
| `op_pagos_detalle` | Aplicación de pagos a cuotas | ✅ Script listo |
| `op_reservaciones` | Reservaciones de áreas comunes | ✅ Script listo |
| `op_visitantes` | Control de acceso de visitantes | ✅ Script listo |
| `op_comunicados` | Avisos y comunicados | ✅ Script listo |
| `op_comunicados_lecturas` | Registro de lecturas | ✅ Script listo |

### 8.5 VISTAS Y STORED PROCEDURES

| Objeto | Tipo | Descripción |
|--------|------|-------------|
| `vw_estado_cuenta` | Vista | Estado de cuenta por unidad |
| `vw_resumen_morosidad` | Vista | Reporte de morosidad |
| `vw_calendario_reservaciones` | Vista | Calendario de reservaciones |
| `vw_visitantes_activos` | Vista | Visitantes en condominio |
| `sp_GenerarCuotasMensuales` | SP | Genera cuotas mensuales |
| `sp_AplicarRecargosMora` | SP | Aplica recargos por mora |
| `fn_GenerarFolio` | Función | Genera folios únicos |

### 8.6 PÁGINAS A DESARROLLAR

#### Catálogos (/Views/Catalogos/)

| Página | Tabla | Prioridad | Estado |
|--------|-------|-----------|--------|
| AreasComunes.aspx | `cat_areas_comunes` | ALTA | 🔄 Pendiente |
| Residentes.aspx | `cat_residentes` | ALTA | 🔄 Pendiente |
| ConceptosCuota.aspx | `cat_conceptos_cuota` | ALTA | 🔄 Pendiente |
| Unidades.aspx | `cat_unidades` | ALTA | ⚠️ Mejorar existente |

#### Operaciones (/Views/Operacion/Condominios/)

| Página | Tabla | Prioridad | Estado |
|--------|-------|-----------|--------|
| Cuotas.aspx | `op_cuotas` | ALTA | 🔄 Pendiente |
| Pagos.aspx | `op_pagos` | ALTA | 🔄 Pendiente |
| EstadoCuenta.aspx | Vistas | ALTA | 🔄 Pendiente |
| Reservaciones.aspx | `op_reservaciones` | MEDIA | 🔄 Pendiente |
| CalendarioReservaciones.aspx | `op_reservaciones` | MEDIA | 🔄 Pendiente |
| Visitantes.aspx | `op_visitantes` | MEDIA | 🔄 Pendiente |
| Comunicados.aspx | `op_comunicados` | BAJA | 🔄 Pendiente |

### 8.7 MENÚ DE NAVEGACIÓN

```
📁 Condominios
├── 📋 Catálogos
│   ├── Áreas Comunes (salones, albercas, gimnasios)
│   ├── Residentes
│   ├── Unidades
│   └── Conceptos de Cuota
├── 💰 Cobranza
│   ├── Cuotas
│   ├── Pagos
│   └── Estado de Cuenta
├── 📅 Reservaciones
│   ├── Reservaciones
│   └── Calendario
├── 🚪 Accesos
│   ├── Visitantes
│   └── Bitácora
├── 📢 Comunicados
│   └── Comunicados
└── 📊 Reportes
    ├── Morosidad
    └── Ingresos
```

**Nota:** Las secciones/torres se gestionan en el catálogo de SubEntidades (ya existente).

### 8.8 SCRIPTS SQL

Ubicación: `.kiro/specs/ecosistema-jelabbc/sql/condominios/`

| Archivo | Descripción |
|---------|-------------|
| `00_ejecutar_todos.sql` | Script maestro de ejecución |
| `01_catalogos_base.sql` | Tablas de catálogos (cat_areas_comunes, residentes, conceptos) |
| `02_cuotas_pagos.sql` | Tablas de cuotas y pagos |
| `03_reservaciones_visitantes.sql` | Tablas de reservaciones y visitantes |
| `04_datos_iniciales.sql` | Datos iniciales y stored procedures |

### 8.9 RELACIÓN CON OTROS MÓDULOS

- **conf_residentes_telegram**: `cat_residentes.TelegramChatId` vincula con `conf_residentes_telegram.ChatId` para notificaciones
- **op_tickets**: Puede usarse para tickets de mantenimiento del condominio
- **cat_entidades**: Cada condominio es una entidad con `TipoEntidad='Condominio'`
- **cat_subentidades**: Las torres/secciones del condominio (ya existente)

### 8.10 DOCUMENTACIÓN

- Análisis completo: `.kiro/specs/ecosistema-jelabbc/analisis-modulo-condominios.md`

---

## ESTRUCTURA DE CARPETAS ESTÁNDAR

### Estructura Recomendada

```
JelaWeb/
├── Views/                    # Páginas organizadas por módulo
│   ├── Auth/                 # Autenticación
│   ├── Catalogos/            # Catálogos (prefijo cat_)
│   ├── Config/               # Configuración del sistema (prefijo conf_)
│   ├── Error/                # Páginas de error
│   ├── IOT/                  # Dashboard IoT
│   └── Operacion/            # Módulos operativos (prefijo op_)
│       ├── GestionDocumentos/
│       └── Tickets/
│
├── Services/                 # Un servicio por entidad
│   ├── API/                  # Consumo de API REST
│   ├── Auth/                 # Servicios de autenticación
│   └── [Entidad]Service.vb   # Servicios de negocio
│
├── Models/
│   └── DTOs/                 # TODOS los DTOs aquí
│       └── [Entidad]DTO.vb
│
├── Content/
│   └── CSS/                  # TODOS los CSS aquí
│       └── [modulo].css
│
├── Scripts/
│   └── app/                  # TODOS los JS aquí
│       ├── Catalogos/
│       ├── Config/
│       ├── IOT/
│       ├── Operacion/
│       └── shared/           # Funciones compartidas
│
└── Utilities/
    └── FuncionesGridWeb.vb   # Helpers del grid
```

### Convenciones de Nomenclatura

| Tipo | Patrón | Ejemplo |
|------|--------|---------|
| Página | `[Entidad].aspx` | `Proveedores.aspx` |
| Code-behind | `[Entidad].aspx.vb` | `Proveedores.aspx.vb` |
| Servicio | `[Entidad]Service.vb` | `ProveedorService.vb` |
| DTO | `[Entidad]DTO.vb` | `ProveedorDTO.vb` |
| CSS | `[modulo].css` | `proveedores.css` |
| JavaScript | `[modulo].js` | `proveedores.js` |

---

## ESTÁNDARES APLICADOS

Todas las páginas implementadas cumplen con:

1. ✅ **Separación de código**: CSS y JS en archivos externos
2. ✅ **Nomenclatura contextual**: Botones con nombres específicos
3. ✅ **Toolbar del grid**: Acciones CRUD en toolbar integrado
4. ✅ **Sin paginación**: `Mode="ShowAllRecords"` en todos los grids
5. ✅ **FuncionesGridWeb**: Evento `DataBound` implementado
6. ✅ **Popups**: Captura de datos mediante `ASPxPopupControl`
7. ✅ **Logger**: Firma correcta `Logger.LogError(mensaje, ex)`
