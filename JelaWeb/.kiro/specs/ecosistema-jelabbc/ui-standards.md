# Estándares de UI - Ecosistema JELABBC

## Introducción

Este documento define los estándares de interfaz de usuario para el Ecosistema JELABBC, basados en las especificaciones del Gist oficial del proyecto. Todos los módulos del portal web deben seguir estas reglas para garantizar consistencia, usabilidad y mantenibilidad.

## Tecnologías Base

- **Framework**: ASP.NET Web Forms (.NET Framework 4.8.1) con VB.NET
- **Componentes UI**: DevExpress v22.2
- **CSS Framework**: Bootstrap 5
- **JavaScript**: jQuery + scripts personalizados

## Reglas Generales de UI

### 0. Color de Fondo Estándar

**REGLA CRÍTICA**: El color de fondo para todas las páginas del sistema DEBE ser `#E4EFFA` (azul claro suave).

**Razón**: Garantiza consistencia visual en todo el ecosistema JELABBC y proporciona una experiencia de usuario uniforme.

**Implementación:**
```css
/* En archivos CSS de módulos o páginas */
.selector-wrapper,
.page-wrapper,
body {
    background: #E4EFFA;
}
```

**Ejemplo CORRECTO:**
```css
/* selector-entidades.css */
.selector-wrapper {
    min-height: 100vh;
    background: #E4EFFA;
    padding: 40px 20px;
}
```

**Ejemplo INCORRECTO:**
```css
/* NO usar otros colores de fondo */
.selector-wrapper {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); /* ❌ */
    background: #ffffff; /* ❌ */
    background: #f5f5f5; /* ❌ */
}
```

**Excepciones:**
- Contenedores internos (cards, modales, grids) pueden usar blanco (`#ffffff`) para contraste
- Elementos de UI específicos (botones, badges) pueden usar colores según su función
- El fondo principal de la página SIEMPRE debe ser `#E4EFFA`

### 1. Separación de Código

**REGLA CRÍTICA**: CSS y JavaScript SIEMPRE deben estar en archivos separados, NUNCA inline.

**Estructura de archivos:**
```
/Content/
  /css/
    /modules/
      entidades.css
      tickets.css
      documentos.css
    site.css
    
/Scripts/
  /app/
    /shared/
      common.js
      validation.js
    /modules/
      entidades.js
      tickets.js
      documentos.js
```

**Ejemplo CORRECTO:**
```html
<!-- En la página .aspx -->
<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
    <link href="~/Content/css/modules/entidades.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="scripts" runat="server">
    <script src="~/Scripts/app/modules/entidades.js"></script>
</asp:Content>
```

**Ejemplo INCORRECTO (NO HACER):**
```html
<!-- NO hacer esto -->
<style>
    .mi-clase { color: red; }
</style>

<script>
    function miFuncion() { }
</script>
```


### 2. Nomenclatura Contextual

**REGLA**: Los botones y acciones deben usar nombres contextuales específicos, NO genéricos.

**Ejemplos CORRECTOS:**
- "Nueva Entidad" (en lugar de "Nuevo")
- "Editar Ticket" (en lugar de "Editar")
- "Eliminar Documento" (en lugar de "Eliminar")
- "Guardar Orden de Compra" (en lugar de "Guardar")

**Razón**: Mejora la accesibilidad, claridad y experiencia del usuario.

### 3. Nomenclatura de Tablas en Base de Datos

**REGLA**: Los prefijos operativos DEBEN usar guion bajo (_), NO sin separador.

**Ejemplos CORRECTOS:**
- `op_tickets`
- `op_documentos`
- `op_ordenes_compra`
- `conf_usuarios`
- `conf_entidades`

**Ejemplos INCORRECTOS:**
- `optickets` ❌
- `opdocumentos` ❌
- `confusuarios` ❌

**Prefijos estándar:**
- `op_` - Tablas operativas (transaccionales)
- `conf_` - Tablas de configuración (catálogos)
- `log_` - Tablas de auditoría y logs
- `temp_` - Tablas temporales

### 4. Nomenclatura de Campos en Base de Datos

**REGLA CRÍTICA**: Todos los nombres de campos DEBEN usar **PascalCase** (primera letra de cada palabra en mayúscula, sin separadores).

**Razón**: Las funciones de `FuncionesGridWeb.vb` (como `SUMColumn`) convierten automáticamente nombres PascalCase a formato legible con espacios. Por ejemplo:
- `ChatId` → "Chat Id"
- `ClienteNombre` → "Cliente Nombre"
- `FechaCreacion` → "Fecha Creacion"
- `EstadoResidente` → "Estado Residente"

**Ejemplos CORRECTOS:**
- `ChatId` ✅
- `ClienteNombre` ✅
- `FechaCreacion` ✅
- `EstadoResidente` ✅
- `CreditosDisponibles` ✅

**Ejemplos INCORRECTOS:**
- `chat_id` ❌ (snake_case)
- `cliente_nombre` ❌ (snake_case)
- `chatId` ❌ (camelCase - primera letra minúscula)

**Nota**: Los nombres de TABLAS sí usan guion bajo para separar el prefijo (`conf_residentes_telegram`), pero los CAMPOS dentro de las tablas usan PascalCase (`ChatId`, `FechaRegistro`).

## Estándares para ASPxGridView

### 3.5 Catálogos sin títulos y sin tarjetas

**REGLA CRÍTICA**: En páginas de catálogos, **NO** agregar títulos o subtítulos superiores. El layout debe seguir el patrón de [Views/Catalogos/Unidades.aspx](../JelaWeb/Views/Catalogos/Unidades.aspx): grid a ancho completo, **sin contenedor tipo card** y **sin sombra** debajo del grid.

**Aplicar:**
- No usar `<div class="page-header">` en catálogos.
- No usar contenedores con `box-shadow` o bordes alrededor del grid.
- El grid debe ocupar el ancho del contenedor y no desbordar la pantalla.

### 4. Toolbar del Grid - Acciones Principales

**REGLA CRÍTICA**: Todas las acciones de alta, edición y eliminación DEBEN estar en el toolbar del grid, NO como botones externos.

**Configuración estándar:**
```xml
<dx:ASPxGridView ID="gridEntidades" runat="server" AutoGenerateColumns="False">
    <SettingsCommandButton>
        <NewButton Text="Nueva Entidad">
            <Image IconID="actions_add_16x16" />
        </NewButton>
        <EditButton Text="Editar Entidad">
            <Image IconID="edit_edit_16x16" />
        </EditButton>
        <DeleteButton Text="Eliminar Entidad">
            <Image IconID="edit_delete_16x16" />
        </DeleteButton>
        <UpdateButton Text="Guardar">
            <Image IconID="save_save_16x16" />
        </UpdateButton>
        <CancelButton Text="Cancelar">
            <Image IconID="actions_cancel_16x16" />
        </CancelButton>
    </SettingsCommandButton>
    
    <Toolbars>
        <dx:GridViewToolbar>
            <Items>
                <dx:GridViewToolbarItem Command="New" />
                <dx:GridViewToolbarItem Command="Edit" />
                <dx:GridViewToolbarItem Command="Delete" />
                <dx:GridViewToolbarItem BeginGroup="true" Command="Refresh" />
                <dx:GridViewToolbarItem Command="ExportToPdf" />
                <dx:GridViewToolbarItem Command="ExportToXlsx" />
            </Items>
        </dx:GridViewToolbar>
    </Toolbars>
</dx:ASPxGridView>
```

**NO hacer esto:**
```html
<!-- INCORRECTO - botones fuera del grid -->
<asp:Button ID="btnNuevo" runat="server" Text="Nuevo" />
<asp:Button ID="btnEditar" runat="server" Text="Editar" />
<dx:ASPxGridView ID="grid" runat="server">
    <!-- grid sin toolbar -->
</dx:ASPxGridView>
```


### 5. Paginación

**REGLA CRÍTICA**: Los grids DEBEN mostrar TODOS los registros sin paginación.

**Configuración obligatoria:**
```xml
<dx:ASPxGridView ID="grid" runat="server">
    <SettingsPager Mode="ShowAllRecords" />
</dx:ASPxGridView>
```

**NO usar paginación:**
```xml
<!-- INCORRECTO - NO HACER -->
<SettingsPager PageSize="20" Mode="ShowPager">
    <PageSizeItemSettings Visible="True" Items="10, 20, 50, 100" />
</SettingsPager>
```

**Razón**: Facilita búsquedas y filtros sin necesidad de navegar entre páginas. Los usuarios pueden ver todos los datos de una vez y usar los filtros nativos del grid para encontrar lo que buscan.

**Excepción**: Solo en casos extremos con datasets muy grandes (>10,000 registros) se puede considerar paginación, pero debe ser aprobado explícitamente.

### 6. Filtros y Agrupaciones

**REGLA CRÍTICA**: Usar capacidades nativas del grid para filtros, NO crear controles externos. Todos los campos filtrables deben estar como columnas en el grid con filtros en cabecera.

**Configuración estándar:**
```xml
<dx:ASPxGridView ID="grid" runat="server">
    <Settings ShowFilterRow="False" 
              ShowFilterRowMenu="True" 
              ShowGroupPanel="True" />
    
    <SettingsBehavior AllowSort="True" 
                      AllowGroup="True" 
                      AllowFixedGroups="True" />
    
    <SettingsSearchPanel Visible="True" 
                         ShowApplyButton="True" 
                         ShowClearButton="True" />
    
    <Columns>
        <dx:GridViewDataTextColumn FieldName="CampoFiltrable" Caption="Campo">
            <Settings AllowHeaderFilter="True" AllowGroup="True" />
        </dx:GridViewDataTextColumn>
    </Columns>
</dx:ASPxGridView>
```

**Explicación:**
- `ShowFilterRow="False"` - Oculta la fila de filtros básica (OBLIGATORIO)
- `ShowFilterRowMenu="True"` - Habilita filtros tipo Excel en headers (OBLIGATORIO)
- `ShowGroupPanel="True"` - Permite agrupar arrastrando columnas (OBLIGATORIO)
- `SettingsSearchPanel` - Barra de búsqueda global en el grid (OBLIGATORIO)
- `AllowHeaderFilter="True"` - DEBE estar en TODAS las columnas filtrables (OBLIGATORIO)
- `AllowGroup="True"` - DEBE estar en TODAS las columnas agrupables (OBLIGATORIO)

**IMPORTANTE**: Si un campo necesita ser filtrable (Entidad, Estado, Categoría, etc.), debe estar como columna visible en el grid con `AllowHeaderFilter="True"`, NO como control externo arriba del grid.

### 7. Filtros Superiores - Solo Fechas

**REGLA CRÍTICA**: ÚNICAMENTE el rango de fechas debe estar arriba del grid. TODOS los demás filtros (Entidad, Estado, Categoría, etc.) DEBEN estar como columnas en el grid con `AllowHeaderFilter="True"`.

**Ejemplo CORRECTO:**
```html
<!-- Solo fechas arriba del grid -->
<div class="filter-panel">
    <div class="row">
        <div class="col-md-3">
            <label>Fecha Inicio:</label>
            <dx:ASPxDateEdit ID="dtFechaInicio" runat="server" />
        </div>
        <div class="col-md-3">
            <label>Fecha Fin:</label>
            <dx:ASPxDateEdit ID="dtFechaFin" runat="server" />
        </div>
        <div class="col-md-2">
            <dx:ASPxButton ID="btnFiltrar" runat="server" Text="Filtrar">
                <Image IconID="filter_filter_16x16" />
            </dx:ASPxButton>
        </div>
    </div>
</div>

<dx:ASPxGridView ID="gridTickets" runat="server">
    <Settings ShowFilterRow="False" ShowFilterRowMenu="True" />
    <Columns>
        <!-- Entidad como columna con filtro en cabecera -->
        <dx:GridViewDataTextColumn FieldName="EntidadNombre" Caption="Entidad">
            <Settings AllowHeaderFilter="True" AllowGroup="True" />
        </dx:GridViewDataTextColumn>
        <!-- Estado como columna con filtro en cabecera -->
        <dx:GridViewDataCheckColumn FieldName="Activo" Caption="Activo">
            <Settings AllowHeaderFilter="True" AllowGroup="True" />
        </dx:GridViewDataCheckColumn>
    </Columns>
</dx:ASPxGridView>
```

**NO hacer esto:**
```html
<!-- INCORRECTO - múltiples filtros externos -->
<div class="filters">
    <asp:DropDownList ID="ddlEstado" runat="server" />
    <asp:DropDownList ID="ddlCategoria" runat="server" />
    <asp:DropDownList ID="ddlPrioridad" runat="server" />
    <asp:DropDownList ID="ddlEntidad" runat="server" />
    <asp:TextBox ID="txtBuscar" runat="server" />
</div>
```

**Regla de implementación:**
1. Si el campo es filtrable y NO es fecha → DEBE estar como columna en el grid con `AllowHeaderFilter="True"`
2. Si el campo es fecha → Puede estar arriba del grid como control externo
3. Si el campo es solo para búsqueda → Usar `SettingsSearchPanel` del grid
4. NUNCA crear controles externos (ComboBox, TextBox) para filtrar campos que pueden estar en el grid


### 8. Columnas Dinámicas en Grids

**REGLA CRÍTICA**: Las columnas de los grids DEBEN generarse dinámicamente desde el resultado del API dinámica, NO deben definirse estáticamente en el ASPX.

**Razón**: Esto garantiza que el grid siempre muestre todas las columnas que devuelve el API, sin necesidad de mantener columnas manualmente en sincronización.

**Implementación obligatoria:**

1. **En el code-behind, crear método para generar columnas dinámicamente:**
```vb
''' <summary>
''' Genera columnas dinámicamente para un grid basándose en las columnas del DataTable.
''' Preserva columnas personalizadas (GridViewCommandColumn, columnas con DataItemTemplate).
''' </summary>
Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)
    Try
        If tabla Is Nothing OrElse tabla.Columns.Count = 0 Then Return
        
        ' Limpiar columnas previas (excepto columnas personalizadas)
        Dim columnasParaMantener As New List(Of GridViewColumn)
        
        ' Guardar columnas de acciones/botones personalizados y columnas con templates
        For Each col As GridViewColumn In grid.Columns
            Dim debeMantener As Boolean = False
            
            If TypeOf col Is GridViewCommandColumn Then
                debeMantener = True
            ElseIf TypeOf col Is GridViewDataColumn Then
                Dim dataCol = CType(col, GridViewDataColumn)
                If dataCol.DataItemTemplate IsNot Nothing Then
                    debeMantener = True
                End If
            End If
            
            If debeMantener Then
                columnasParaMantener.Add(col)
            End If
        Next
        
        ' Limpiar solo las columnas de datos, no las personalizadas
        For i As Integer = grid.Columns.Count - 1 To 0 Step -1
            Dim col = grid.Columns(i)
            If Not TypeOf col Is GridViewCommandColumn AndAlso 
               Not (TypeOf col Is GridViewDataColumn AndAlso CType(col, GridViewDataColumn).DataItemTemplate IsNot Nothing) Then
                grid.Columns.RemoveAt(i)
            End If
        Next
        
        ' Crear columnas dinámicamente desde el DataTable
        For Each col As DataColumn In tabla.Columns
            Dim nombreColumna = col.ColumnName
            
            ' Omitir columnas que ya existen con template personalizado
            Dim yaExistePersonalizada As Boolean = False
            For Each colMantener As GridViewColumn In columnasParaMantener
                If TypeOf colMantener Is GridViewDataColumn Then
                    Dim dataCol = CType(colMantener, GridViewDataColumn)
                    If dataCol.FieldName = nombreColumna AndAlso dataCol.DataItemTemplate IsNot Nothing Then
                        yaExistePersonalizada = True
                        Exit For
                    End If
                End If
            Next
            
            If yaExistePersonalizada Then Continue For
            
            ' Crear columna según el tipo de dato
            Dim gridCol As GridViewDataColumn = Nothing
            
            Select Case col.DataType
                Case GetType(Boolean)
                    gridCol = New GridViewDataCheckColumn()
                    gridCol.Width = Unit.Pixel(80)
                Case GetType(DateTime), GetType(Date)
                    gridCol = New GridViewDataDateColumn()
                    gridCol.Width = Unit.Pixel(150)
                    CType(gridCol, GridViewDataDateColumn).PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
                Case GetType(Decimal), GetType(Double), GetType(Single)
                    gridCol = New GridViewDataTextColumn()
                    gridCol.Width = Unit.Pixel(100)
                    gridCol.PropertiesEdit.DisplayFormatString = "c2"
                Case GetType(Integer), GetType(Long), GetType(Short)
                    gridCol = New GridViewDataTextColumn()
                    gridCol.Width = Unit.Pixel(100)
                    gridCol.PropertiesEdit.DisplayFormatString = "n0"
                Case Else
                    gridCol = New GridViewDataTextColumn()
                    gridCol.Width = Unit.Pixel(150)
            End Select
            
            gridCol.FieldName = nombreColumna
            gridCol.Caption = nombreColumna ' FuncionesGridWeb.SUMColumn aplicará SplitCamelCase
            gridCol.ReadOnly = True
            gridCol.Visible = True
            
            ' Configurar filtros y agrupación según estándares
            gridCol.Settings.AllowHeaderFilter = True
            gridCol.Settings.AllowGroup = True
            
            ' Ocultar columna Id si existe
            If nombreColumna.Equals("Id", StringComparison.OrdinalIgnoreCase) Then
                gridCol.Visible = False
            End If
            
            grid.Columns.Add(gridCol)
        Next
        
    Catch ex As Exception
        Logger.LogError("GenerarColumnasDinamicas", ex)
        Throw
    End Try
End Sub
```

2. **Llamar este método antes de asignar DataSource:**
```vb
Private Sub CargarDatos()
    Try
        Dim dt As DataTable = GetServicio().ListarUnidades()
        
        ' Generar columnas dinámicamente desde el DataTable
        GenerarColumnasDinamicas(gridUnidades, dt)
        
        gridUnidades.DataSource = dt
        gridUnidades.DataBind()
    Catch ex As Exception
        Logger.LogError("CargarDatos", ex)
        Throw
    End Try
End Sub
```

3. **En el ASPX, NO definir columnas estáticas** (excepto GridViewCommandColumn si se necesita):
```xml
<dx:ASPxGridView ID="grid" runat="server" AutoGenerateColumns="False">
    <!-- Columnas generadas dinámicamente desde el API -->
</dx:ASPxGridView>
```

**Excepciones:**
- Se pueden definir columnas estáticas si tienen `DataItemTemplate` personalizado (ej: badges de estatus)
- Se deben mantener `GridViewCommandColumn` para botones de acciones personalizados
- El método `GenerarColumnasDinamicas` automáticamente preserva estas columnas personalizadas

### 9. Clase Helper para Grids

**REGLA**: Usar la clase `FuncionesGridWeb.vb` para estandarizar comportamiento de grids.

**Ubicación**: `/Utilities/FuncionesGridWeb.vb`

**REGLA CRÍTICA**: Todo grid DEBE implementar el evento `DataBound` para aplicar `FuncionesGridWeb.SUMColumn`.

**Implementación obligatoria en code-behind:**
```vb
''' <summary>
''' Evento DataBound para aplicar FuncionesGridWeb
''' </summary>
Protected Sub gridEntidades_DataBound(sender As Object, e As EventArgs) Handles gridEntidades.DataBound
    Try
        Dim tabla As DataTable = TryCast(Session("dtEntidades"), DataTable)
        If tabla IsNot Nothing Then
            FuncionesGridWeb.SUMColumn(gridEntidades, tabla)
        End If
    Catch ex As Exception
        Logger.LogError("Entidades.gridEntidades_DataBound", ex)
    End Try
End Sub
```

**Nota**: Es importante guardar el DataTable en Session antes del DataBind para que FuncionesGridWeb pueda acceder a los tipos de datos de las columnas.

**Funciones principales de FuncionesGridWeb:**

```vb
Public Class FuncionesGridWeb
    
    ''' <summary>
    ''' Aplica encabezados legibles, fuentes limpias y sumatorias automáticas.
    ''' DEBE llamarse en el evento DataBound de todo grid.
    ''' </summary>
    Public Shared Sub SUMColumn(ByVal MiGrid As ASPxGridView, ByVal tabla As DataTable)
        ' - Limpia sumatorias previas
        ' - Aplica estilos de fuente (Segoe UI, tamaño 8)
        ' - Convierte nombres de columna CamelCase a "Camel Case"
        ' - Agrega contador de registros en primera columna
        ' - Detecta tipos de datos y aplica formato:
        '   - Integer/Single: formato n0 con sumatoria
        '   - Double: formato n2 con sumatoria
        '   - Decimal: formato c2 (moneda) con sumatoria
        '   - DateTime: formato G
        ' - Muestra footer con sumatorias
    End Sub
    
    ''' <summary>
    ''' Configura un grid con los estándares del proyecto
    ''' </summary>
    Public Shared Sub ConfigurarGridEstandar(grid As ASPxGridView)
        ' Paginación
        grid.SettingsPager.PageSize = -1
        grid.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords
        
        ' Filtros y búsqueda
        grid.Settings.ShowFilterRow = False
        grid.Settings.ShowFilterRowMenu = True
        grid.Settings.ShowGroupPanel = True
        grid.SettingsSearchPanel.Visible = True
        
        ' Comportamiento
        grid.SettingsBehavior.AllowSort = True
        grid.SettingsBehavior.AllowGroup = True
        grid.SettingsBehavior.AllowFixedGroups = True
        
        ' Exportación
        grid.SettingsExport.EnableClientSideExportAPI = True
        grid.SettingsExport.ExcelExportMode = DevExpress.Export.ExportType.WYSIWYG
    End Sub
    
    ''' <summary>
    ''' Configura el toolbar estándar con acciones CRUD
    ''' </summary>
    Public Shared Sub ConfigurarToolbarCRUD(grid As ASPxGridView, 
                                            nombreEntidad As String,
                                            Optional permitirNuevo As Boolean = True,
                                            Optional permitirEditar As Boolean = True,
                                            Optional permitirEliminar As Boolean = True)
        
        grid.SettingsCommandButton.NewButton.Text = $"Nueva {nombreEntidad}"
        grid.SettingsCommandButton.EditButton.Text = $"Editar {nombreEntidad}"
        grid.SettingsCommandButton.DeleteButton.Text = $"Eliminar {nombreEntidad}"
        
        ' Configurar visibilidad según permisos
        grid.SettingsCommandButton.NewButton.Visible = permitirNuevo
        grid.SettingsCommandButton.EditButton.Visible = permitirEditar
        grid.SettingsCommandButton.DeleteButton.Visible = permitirEliminar
    End Sub
    
    ''' <summary>
    ''' Aplica formato de moneda a una columna
    ''' </summary>
    Public Shared Sub FormatearColumnaMonto(columna As GridViewDataColumn)
        columna.PropertiesEdit.DisplayFormatString = "c2"
        columna.CellStyle.HorizontalAlign = HorizontalAlign.Right
    End Sub
    
End Class
```

**Uso en páginas:**
```vb
Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    If Not IsPostBack Then
        ' Configurar grid con estándares
        FuncionesGridWeb.ConfigurarGridEstandar(gridEntidades)
        FuncionesGridWeb.ConfigurarToolbarCRUD(gridEntidades, "Entidad")
        
        CargarDatos()
    End If
End Sub
```


## Estándares para Popups y Modales

### 9. ASPxPopupControl para Captura de Datos

**REGLA CRÍTICA**: Toda captura de datos debe hacerse mediante `ASPxPopupControl`, NO abrir otras páginas.

**Configuración estándar:**
```xml
<dx:ASPxPopupControl ID="popupEntidad" runat="server" 
                     ClientInstanceName="popupEntidad"
                     HeaderText="Nueva Entidad"
                     Width="600px"
                     Height="400px"
                     Modal="True"
                     PopupHorizontalAlign="WindowCenter"
                     PopupVerticalAlign="WindowCenter"
                     CloseAction="CloseButton"
                     ShowFooter="True">
    
    <HeaderStyle BackColor="#007bff" ForeColor="White" />
    
    <ContentCollection>
        <dx:PopupControlContentControl>
            <div class="form-container">
                <div class="form-group">
                    <label>Nombre:</label>
                    <dx:ASPxTextBox ID="txtNombre" runat="server" Width="100%" />
                </div>
                <div class="form-group">
                    <label>RFC:</label>
                    <dx:ASPxTextBox ID="txtRFC" runat="server" Width="100%" />
                </div>
                <!-- Más campos -->
            </div>
        </dx:PopupControlContentControl>
    </ContentCollection>
    
    <FooterTemplate>
        <div class="popup-footer">
            <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" 
                          OnClick="btnGuardar_Click">
                <Image IconID="save_save_16x16" />
            </dx:ASPxButton>
            <dx:ASPxButton ID="btnCancelar" runat="server" Text="Cancelar" 
                          AutoPostBack="False">
                <Image IconID="actions_cancel_16x16" />
                <ClientSideEvents Click="function(s, e) { popupEntidad.Hide(); }" />
            </dx:ASPxButton>
        </div>
    </FooterTemplate>
</dx:ASPxPopupControl>
```

**Abrir popup desde código:**
```vb
Protected Sub gridEntidades_RowCommand(sender As Object, e As ASPxGridViewRowCommandEventArgs)
    If e.CommandArgs.CommandName = "New" Then
        LimpiarFormulario()
        popupEntidad.HeaderText = "Nueva Entidad"
        popupEntidad.ShowOnPageLoad = True
    ElseIf e.CommandArgs.CommandName = "Edit" Then
        Dim entidadId As Integer = CInt(e.KeyValue)
        CargarEntidad(entidadId)
        popupEntidad.HeaderText = "Editar Entidad"
        popupEntidad.ShowOnPageLoad = True
    End If
End Sub
```

**Abrir popup desde JavaScript:**
```javascript
function mostrarNuevaEntidad() {
    popupEntidad.Show();
}
```

### 10. Validación en Popups

**REGLA**: Usar `ASPxValidationSummary` y validadores DevExpress dentro del popup.

**Ejemplo:**
```xml
<dx:PopupControlContentControl>
    <dx:ASPxValidationSummary ID="validationSummary" runat="server" 
                              ValidationGroup="EntidadValidation" />
    
    <div class="form-group">
        <label>Nombre: <span class="required">*</span></label>
        <dx:ASPxTextBox ID="txtNombre" runat="server" Width="100%">
            <ValidationSettings ValidationGroup="EntidadValidation">
                <RequiredField IsRequired="True" ErrorText="El nombre es requerido" />
            </ValidationSettings>
        </dx:ASPxTextBox>
    </div>
    
    <div class="form-group">
        <label>Email: <span class="required">*</span></label>
        <dx:ASPxTextBox ID="txtEmail" runat="server" Width="100%">
            <ValidationSettings ValidationGroup="EntidadValidation">
                <RequiredField IsRequired="True" ErrorText="El email es requerido" />
                <RegularExpression ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                                   ErrorText="Email inválido" />
            </ValidationSettings>
        </dx:ASPxTextBox>
    </div>
</dx:PopupControlContentControl>
```


## Estándares de Layout y Diseño

### 11. Estructura de Página Estándar

**REGLA**: Todas las páginas deben seguir esta estructura:

```html
<%@ Page Title="Gestión de Entidades" Language="vb" 
         MasterPageFile="~/MasterPages/Jela.Master" 
         AutoEventWireup="false" 
         CodeBehind="Entidades.aspx.vb" 
         Inherits="JelaWeb.Entidades" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/Content/css/modules/entidades.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="~/Views/Inicio.aspx">Inicio</a></li>
            <li class="breadcrumb-item active">Entidades</li>
        </ol>
    </nav>
    
    <!-- Título de página -->
    <div class="page-header">
        <h1>Gestión de Entidades</h1>
        <p class="text-muted">Administre las entidades del sistema</p>
    </div>
    
    <!-- Filtros superiores (solo fechas si aplica) -->
    <div class="filter-panel" id="filterPanel" runat="server" visible="false">
        <!-- Filtros de fecha aquí -->
    </div>
    
    <!-- Grid principal -->
    <div class="grid-container">
        <dx:ASPxGridView ID="gridEntidades" runat="server">
            <!-- Configuración del grid -->
        </dx:ASPxGridView>
    </div>
    
    <!-- Popups -->
    <dx:ASPxPopupControl ID="popupEntidad" runat="server">
        <!-- Configuración del popup -->
    </dx:ASPxPopupControl>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="~/Scripts/app/modules/entidades.js"></script>
</asp:Content>
```

### 12. Clases CSS Estándar

**REGLA**: Usar clases Bootstrap 5 + clases personalizadas del proyecto.

**Clases personalizadas estándar (en site.css):**
```css
/* Contenedores */
.page-header {
    margin-bottom: 20px;
    padding-bottom: 10px;
    border-bottom: 2px solid #007bff;
}

.filter-panel {
    background-color: #f8f9fa;
    padding: 15px;
    margin-bottom: 20px;
    border-radius: 5px;
}

.grid-container {
    background-color: white;
    padding: 15px;
    border-radius: 5px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

/* Formularios */
.form-container {
    padding: 20px;
}

.form-group {
    margin-bottom: 15px;
}

.form-group label {
    font-weight: 600;
    margin-bottom: 5px;
    display: block;
}

.required {
    color: #dc3545;
}

/* Popups */
.popup-footer {
    text-align: right;
    padding: 10px;
}

.popup-footer .dxbButton {
    margin-left: 10px;
}

/* Estados */
.status-badge {
    padding: 5px 10px;
    border-radius: 3px;
    font-size: 12px;
    font-weight: 600;
}

.status-activo {
    background-color: #28a745;
    color: white;
}

.status-inactivo {
    background-color: #6c757d;
    color: white;
}

.status-pendiente {
    background-color: #ffc107;
    color: #212529;
}
```


## Estándares de JavaScript

### 13. Estructura de Archivos JavaScript

**REGLA**: Cada módulo debe tener su propio archivo JS con estructura estándar.

**Ejemplo: `/Scripts/app/modules/entidades.js`**
```javascript
// Namespace del módulo
var EntidadesModule = (function() {
    'use strict';
    
    // Variables privadas
    var _gridInstance = null;
    var _popupInstance = null;
    
    // Inicialización
    function init() {
        _gridInstance = gridEntidades;
        _popupInstance = popupEntidad;
        
        bindEvents();
        configurarValidaciones();
    }
    
    // Event handlers
    function bindEvents() {
        // Eventos del grid
        if (_gridInstance) {
            _gridInstance.RowClick.AddHandler(onGridRowClick);
        }
        
        // Eventos de botones
        $('#btnExportar').on('click', exportarDatos);
    }
    
    // Funciones públicas
    function mostrarNuevo() {
        limpiarFormulario();
        _popupInstance.SetHeaderText('Nueva Entidad');
        _popupInstance.Show();
    }
    
    function mostrarEditar(entidadId) {
        cargarEntidad(entidadId);
        _popupInstance.SetHeaderText('Editar Entidad');
        _popupInstance.Show();
    }
    
    // Funciones privadas
    function onGridRowClick(s, e) {
        // Lógica del evento
    }
    
    function limpiarFormulario() {
        txtNombre.SetValue('');
        txtRFC.SetValue('');
        // Limpiar más campos
    }
    
    function cargarEntidad(id) {
        // Cargar datos de la entidad
        $.ajax({
            url: '/api/entidades/' + id,
            method: 'GET',
            success: function(data) {
                txtNombre.SetValue(data.nombre);
                txtRFC.SetValue(data.rfc);
                // Cargar más campos
            },
            error: function(xhr, status, error) {
                mostrarError('Error cargando entidad: ' + error);
            }
        });
    }
    
    function exportarDatos() {
        _gridInstance.ExportToXlsx();
    }
    
    function configurarValidaciones() {
        // Validaciones personalizadas
    }
    
    function mostrarError(mensaje) {
        toastr.error(mensaje);
    }
    
    function mostrarExito(mensaje) {
        toastr.success(mensaje);
    }
    
    // API pública
    return {
        init: init,
        mostrarNuevo: mostrarNuevo,
        mostrarEditar: mostrarEditar
    };
})();

// Inicializar cuando el DOM esté listo
$(document).ready(function() {
    EntidadesModule.init();
});
```

### 14. Llamadas AJAX Estándar

**REGLA**: Usar jQuery AJAX con manejo de errores consistente.

**Template estándar:**
```javascript
function guardarEntidad(datos) {
    $.ajax({
        url: '/api/entidades',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(datos),
        beforeSend: function() {
            // Mostrar loading
            ASPxClientUtils.ShowLoadingPanel();
        },
        success: function(response) {
            toastr.success('Entidad guardada exitosamente');
            _popupInstance.Hide();
            _gridInstance.Refresh();
        },
        error: function(xhr, status, error) {
            var mensaje = 'Error guardando entidad';
            if (xhr.responseJSON && xhr.responseJSON.message) {
                mensaje = xhr.responseJSON.message;
            }
            toastr.error(mensaje);
        },
        complete: function() {
            // Ocultar loading
            ASPxClientUtils.HideLoadingPanel();
        }
    });
}
```


## Estándares de Accesibilidad

### 15. ARIA Labels y Roles

**REGLA**: Todos los controles interactivos deben tener labels y roles ARIA apropiados.

**Ejemplos:**
```html
<!-- Botones -->
<dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar">
    <ClientSideEvents Click="guardarEntidad" />
    <Attributes>
        <dx:Attribute Name="aria-label" Value="Guardar entidad" />
    </Attributes>
</dx:ASPxButton>

<!-- Campos de formulario -->
<div class="form-group">
    <label for="txtNombre" id="lblNombre">Nombre:</label>
    <dx:ASPxTextBox ID="txtNombre" runat="server" Width="100%">
        <Attributes>
            <dx:Attribute Name="aria-labelledby" Value="lblNombre" />
            <dx:Attribute Name="aria-required" Value="true" />
        </Attributes>
    </dx:ASPxTextBox>
</div>

<!-- Grid -->
<dx:ASPxGridView ID="gridEntidades" runat="server">
    <Attributes>
        <dx:Attribute Name="role" Value="grid" />
        <dx:Attribute Name="aria-label" Value="Lista de entidades" />
    </Attributes>
</dx:ASPxGridView>
```

### 16. Navegación por Teclado

**REGLA**: Todas las funciones deben ser accesibles mediante teclado.

**Configuración DevExpress:**
```xml
<dx:ASPxGridView ID="grid" runat="server">
    <SettingsBehavior AllowFocusedRow="True" />
    <KeyboardSupport>
        <dx:ASPxClientKeyboardSupport Enabled="True" />
    </KeyboardSupport>
</dx:ASPxGridView>
```

**JavaScript para atajos de teclado:**
```javascript
// Atajos de teclado estándar
$(document).on('keydown', function(e) {
    // Ctrl+N = Nuevo
    if (e.ctrlKey && e.key === 'n') {
        e.preventDefault();
        EntidadesModule.mostrarNuevo();
    }
    
    // Ctrl+S = Guardar
    if (e.ctrlKey && e.key === 's') {
        e.preventDefault();
        guardarEntidad();
    }
    
    // Esc = Cerrar popup
    if (e.key === 'Escape') {
        if (_popupInstance && _popupInstance.IsVisible()) {
            _popupInstance.Hide();
        }
    }
});
```

## Estándares de Notificaciones

### 17. Toastr para Mensajes

**REGLA**: Usar Toastr para notificaciones al usuario.

**Configuración en Master Page:**
```html
<link href="~/Content/toastr/toastr.min.css" rel="stylesheet" />
<script src="~/Scripts/toastr/toastr.min.js"></script>

<script>
    // Configuración global de Toastr
    toastr.options = {
        "closeButton": true,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "timeOut": "5000"
    };
</script>
```

**Uso:**
```javascript
// Éxito
toastr.success('Operación completada exitosamente');

// Error
toastr.error('Ocurrió un error al procesar la solicitud');

// Advertencia
toastr.warning('Esta acción no se puede deshacer');

// Información
toastr.info('Los cambios se guardarán automáticamente');
```

**Desde código VB.NET:**
```vb
Protected Sub MostrarMensaje(tipo As String, mensaje As String)
    Dim script As String = $"toastr.{tipo}('{mensaje}');"
    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastr", script, True)
End Sub

' Uso
MostrarMensaje("success", "Entidad guardada exitosamente")
MostrarMensaje("error", "Error al guardar la entidad")
```


## Estándares de Rendimiento

### 18. Carga Diferida de Datos

**REGLA**: Para grids con muchos datos, usar carga diferida (lazy loading).

**Configuración:**
```xml
<dx:ASPxGridView ID="grid" runat="server" 
                 EnableCallBacks="True"
                 OnCustomCallback="grid_CustomCallback">
    <SettingsPager Mode="ShowPager" PageSize="50" />
    <Settings VerticalScrollBarMode="Auto" 
              VerticalScrollableHeight="500" />
</dx:ASPxGridView>
```

### 19. Optimización de Imágenes

**REGLA**: Todas las imágenes deben estar optimizadas y usar lazy loading.

**HTML:**
```html
<img src="placeholder.jpg" 
     data-src="imagen-real.jpg" 
     class="lazy-load" 
     alt="Descripción de la imagen" />
```

**JavaScript:**
```javascript
// Lazy loading de imágenes
$(document).ready(function() {
    $('.lazy-load').each(function() {
        var img = $(this);
        img.attr('src', img.data('src'));
    });
});
```

## Estándares de Seguridad en UI

### 20. Validación de Permisos en UI

**REGLA**: Ocultar/deshabilitar controles según permisos del usuario.

**Código VB.NET:**
```vb
Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
    If Not IsPostBack Then
        ConfigurarPermisos()
        CargarDatos()
    End If
End Sub

Private Sub ConfigurarPermisos()
    Dim usuario = SessionHelper.GetCurrentUser()
    
    ' Verificar permisos
    Dim puedeCrear = PermisosHelper.TienePermiso(usuario, "entidades.crear")
    Dim puedeEditar = PermisosHelper.TienePermiso(usuario, "entidades.editar")
    Dim puedeEliminar = PermisosHelper.TienePermiso(usuario, "entidades.eliminar")
    
    ' Configurar toolbar del grid
    FuncionesGridWeb.ConfigurarToolbarCRUD(
        gridEntidades, 
        "Entidad",
        puedeCrear,
        puedeEditar,
        puedeEliminar
    )
    
    ' Ocultar botones adicionales si no tiene permisos
    btnExportar.Visible = PermisosHelper.TienePermiso(usuario, "entidades.exportar")
End Sub
```

### 21. Prevención de XSS

**REGLA**: Siempre encodear datos del usuario antes de mostrarlos.

**Ejemplo:**
```vb
' En code-behind
Protected Function GetNombreSeguro(nombre As String) As String
    Return HttpUtility.HtmlEncode(nombre)
End Function
```

```html
<!-- En la página -->
<asp:Label ID="lblNombre" runat="server" 
           Text='<%# GetNombreSeguro(Eval("Nombre").ToString()) %>' />
```

## Checklist de Implementación

Al crear un nuevo módulo, verificar:

- [ ] CSS y JS en archivos separados (NO inline)
- [ ] Nomenclatura contextual en botones (ej: "Nueva Entidad")
- [ ] Tablas con prefijos correctos (ej: `op_tickets`)
- [ ] **Campos en PascalCase** (ej: `ChatId`, `FechaCreacion`)
- [ ] **Columnas generadas dinámicamente desde el API** (NO estáticas en ASPX)
- [ ] Acciones CRUD en toolbar del grid
- [ ] Paginación configurada (`Mode="ShowAllRecords"`)
- [ ] **NO filtros externos** (excepto fechas) - todos los filtros como columnas en el grid
- [ ] **ShowFilterRow="False"** - fila de filtros deshabilitada
- [ ] **ShowFilterRowMenu="True"** - filtros tipo Excel en cabeceras
- [ ] **AllowHeaderFilter="True"** en TODAS las columnas filtrables
- [ ] **AllowGroup="True"** en TODAS las columnas agrupables
- [ ] Solo filtro de fechas arriba del grid (si aplica)
- [ ] Popups para captura de datos (NO páginas nuevas)
- [ ] Validaciones en popups
- [ ] Estructura de página estándar
- [ ] Clases CSS estándar aplicadas
- [ ] JavaScript con patrón de módulo
- [ ] ARIA labels en controles
- [ ] Navegación por teclado funcional
- [ ] Toastr para notificaciones
- [ ] Permisos validados en UI
- [ ] Datos encodificados (prevención XSS)
- [ ] Uso de `FuncionesGridWeb.vb` para configuración
- [ ] **Evento DataBound implementado con FuncionesGridWeb.SUMColumn**
- [ ] **DataTable guardado en Session antes del DataBind para FuncionesGridWeb**

## Recursos Adicionales

### Documentación DevExpress
- [ASPxGridView](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxGridView)
- [ASPxPopupControl](https://docs.devexpress.com/AspNet/DevExpress.Web.ASPxPopupControl)
- [Client-Side API](https://docs.devexpress.com/AspNet/401358/client-side-api)

### Documentación Bootstrap 5
- [Components](https://getbootstrap.com/docs/5.0/components/)
- [Utilities](https://getbootstrap.com/docs/5.0/utilities/)

### Accesibilidad
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [ARIA Authoring Practices](https://www.w3.org/WAI/ARIA/apg/)

## Reglas de Programación VB.NET

### Manejo de Valores Null/Nothing

**REGLA CRÍTICA**: Al trabajar con objetos deserializados desde JSON o diccionarios, SIEMPRE validar que el valor no sea `Nothing` antes de llamar a métodos como `.ToString()` o conversiones de tipo.

**Problema común:**
```vb
' ❌ INCORRECTO - Causa NullReferenceException si datos("Email") es Nothing
nuevaFila("Email") = If(datos.ContainsKey("Email"), datos("Email").ToString(), "")
```

**Solución correcta:**
```vb
' ✅ CORRECTO - Valida existencia Y que no sea Nothing
nuevaFila("Email") = If(datos.ContainsKey("Email") AndAlso datos("Email") IsNot Nothing, datos("Email").ToString(), "")
```

**Patrón estándar para valores String:**
```vb
' Usar siempre este patrón para strings
valor = If(diccionario.ContainsKey("clave") AndAlso diccionario("clave") IsNot Nothing, diccionario("clave").ToString(), "")
```

**Patrón estándar para valores Boolean:**
```vb
' Para booleanos, usar Boolean.TryParse para mayor seguridad
Dim valorBool As Boolean = False
If diccionario.ContainsKey("clave") AndAlso diccionario("clave") IsNot Nothing Then
    Boolean.TryParse(diccionario("clave").ToString(), valorBool)
End If
nuevaFila("CampoBoolean") = valorBool
```

**Patrón estándar para valores Integer (nullable):**
```vb
' Para enteros que pueden ser null, usar TryParse con DBNull.Value
Dim valorInt As Object = DBNull.Value
If diccionario.ContainsKey("clave") AndAlso diccionario("clave") IsNot Nothing Then
    Dim intTemp As Integer
    If Integer.TryParse(diccionario("clave").ToString(), intTemp) Then
        valorInt = intTemp
    End If
End If
nuevaFila("CampoInt") = valorInt
```

**Principios:**
1. **NUNCA** llamar `.ToString()` directamente sobre un valor que puede ser `Nothing`
2. **SIEMPRE** usar la sintaxis `If(condicion, valorSiVerdadero, valorSiFalso)` con validación completa
3. **SIEMPRE** verificar tanto `ContainsKey()` como `IsNot Nothing` antes de acceder al valor
4. Para conversiones de tipo, preferir métodos `TryParse` sobre conversiones directas (`CBool`, `CInt`, etc.)

### Espaciado entre Bloques de Código

**REGLA CRÍTICA**: Debe haber una línea en blanco entre cada bloque de código independiente (bloques `If...End If`, `Select Case...End Select`, etc.).

**✅ CORRECTO - Con espacio entre bloques:**
```vb
If dt.Columns.Contains("TipoResidente") Then
    Dim tipoResidente = If(datos.ContainsKey("TipoResidente") AndAlso datos("TipoResidente") IsNot Nothing, datos("TipoResidente").ToString(), "")
    nuevaFila("TipoResidente") = If(String.IsNullOrEmpty(tipoResidente), "Propietario", tipoResidente)
End If

If dt.Columns.Contains("EsPrincipal") Then
    Dim esPrincipalValor As Boolean = False
    If datos.ContainsKey("EsPrincipal") AndAlso datos("EsPrincipal") IsNot Nothing Then
        Boolean.TryParse(datos("EsPrincipal").ToString(), esPrincipalValor)
    End If
    nuevaFila("EsPrincipal") = esPrincipalValor
End If
```

**❌ INCORRECTO - Sin espacio entre bloques:**
```vb
If dt.Columns.Contains("Email") Then
    nuevaFila("Email") = If(datos.ContainsKey("Email") AndAlso datos("Email") IsNot Nothing, datos("Email").ToString(), "")
End If
If dt.Columns.Contains("Telefono") Then
    nuevaFila("Telefono") = If(datos.ContainsKey("Telefono") AndAlso datos("Telefono") IsNot Nothing, datos("Telefono").ToString(), "")
End If
If dt.Columns.Contains("Celular") Then
    nuevaFila("Celular") = If(datos.ContainsKey("Celular") AndAlso datos("Celular") IsNot Nothing, datos("Celular").ToString(), "")
End If
```

**Principios:**
1. **SIEMPRE** dejar una línea en blanco después de cada `End If`, `End Select`, `End Try`, `End Using`, `End Sub`, `End Function`, `End Property`, etc.
2. **SIEMPRE** dejar una línea en blanco antes de cada nuevo bloque `If`, `Select Case`, `Try`, `Using`, etc., cuando no está anidado dentro de otro bloque
3. **EXCEPCIÓN**: No se requiere espacio entre bloques anidados (un `If` dentro de otro `If`) a menos que mejore la legibilidad
4. Los comentarios pueden estar en la misma línea que el bloque o en líneas separadas sin espacio adicional requerido

**Ejemplo completo - Asignación desde JSON deserializado:**
```vb
Dim datos As Dictionary(Of String, Object) = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(jsonString)

' String seguro
nuevaFila("Nombre") = If(datos.ContainsKey("Nombre") AndAlso datos("Nombre") IsNot Nothing, datos("Nombre").ToString(), "")

' Boolean seguro
Dim activoValor As Boolean = True
If datos.ContainsKey("Activo") AndAlso datos("Activo") IsNot Nothing Then
    Boolean.TryParse(datos("Activo").ToString(), activoValor)
End If
nuevaFila("Activo") = activoValor

' Integer nullable seguro
Dim anioValor As Object = DBNull.Value
If datos.ContainsKey("Anio") AndAlso datos("Anio") IsNot Nothing Then
    Dim anioTemp As Integer
    If Integer.TryParse(datos("Anio").ToString(), anioTemp) Then
        anioValor = anioTemp
    End If
End If
nuevaFila("Anio") = anioValor
```

### Uso de DynamicCrudService para Inserts

**REGLA CRÍTICA**: Cuando se necesite obtener el ID del registro insertado, SIEMPRE usar `DynamicCrudService.InsertarConId()` en lugar de `DynamicCrudService.Insertar()` seguido de una consulta adicional.

**Razón**: El API ya retorna el último ID insertado en la respuesta. Usar `Insertar()` + consulta adicional (`SELECT LAST_INSERT_ID()` o `SELECT MAX(Id)`) es ineficiente, propenso a errores en entornos concurrentes, y requiere una llamada adicional innecesaria.

**❌ INCORRECTO - NO HACER:**
```vb
If id = 0 Then
    resultado = DynamicCrudService.Insertar("cat_residentes", datosGuardar)
    ' Obtener el ID insertado - INEFICIENTE Y PROPENSO A ERRORES
    If resultado Then
        Dim dt As DataTable = DynamicCrudService.EjecutarConsulta("SELECT LAST_INSERT_ID() AS Id")
        If dt.Rows.Count > 0 Then nuevoId = Convert.ToInt32(dt.Rows(0)("Id"))
    End If
Else
    resultado = DynamicCrudService.Actualizar("cat_residentes", id, datosGuardar)
    nuevoId = id
End If
```

**❌ INCORRECTO - MUY PROPENSO A ERRORES EN CONCURRENCIA:**
```vb
If id = 0 Then
    resultado = DynamicCrudService.Insertar("cat_residentes", datosGuardar)
    If resultado Then
        ' ❌ INCORRECTO: SELECT MAX puede devolver ID de otro registro insertado concurrentemente
        Dim query As String = "SELECT MAX(Id) AS Id FROM cat_residentes WHERE UnidadId = " & unidadId
        Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
        If dt.Rows.Count > 0 Then nuevoId = Convert.ToInt32(dt.Rows(0)("Id"))
    End If
End If
```

**✅ CORRECTO - USA LA RESPUESTA DEL API:**
```vb
Dim resultado As Boolean
Dim nuevoId As Integer = id

If id = 0 Then
    ' Usar InsertarConId que obtiene el ID directamente de la respuesta del API
    nuevoId = DynamicCrudService.InsertarConId("cat_residentes", datosGuardar)
    resultado = nuevoId > 0
Else
    resultado = DynamicCrudService.Actualizar("cat_residentes", id, datosGuardar)
    nuevoId = id
End If

Return New With {
    .success = resultado,
    .message = If(resultado, "Guardado correctamente", "Error al guardar"),
    .data = New With {.id = nuevoId}
}
```

**Ventajas de usar `InsertarConId()`:**

1. **Más eficiente**: Solo una llamada al API que retorna el ID directamente
2. **Thread-safe**: Obtiene el ID exacto del registro insertado en la conexión actual, sin problemas de concurrencia
3. **Más preciso**: No depende de `MAX(Id)` que puede retornar el ID de otro registro insertado concurrentemente
4. **Consistente**: Todos los métodos siguen el mismo patrón que el API utiliza internamente
5. **Menos código**: No requiere consultas adicionales ni manejo de `DataTable`

**Cómo funciona internamente:**

- `DynamicCrudService.InsertarConId()` llama a `ApiConsumerCRUD.EnviarPostId()`
- `EnviarPostId()` hace el POST al API y extrae el ID de la respuesta: `Return Convert.ToInt32(resultado("id"))`
- El API ya retorna el ID en su respuesta: `Return Ok(New With {.id = idInsertado, .mensaje = "Registro insertado correctamente."})`

**Aplicar esta regla a:**

- Todos los métodos `Guardar*` (ej: `GuardarUnidad`, `GuardarResidente`, `GuardarVehiculo`, `GuardarTag`, `GuardarDocumento`)
- Cualquier método que realice un INSERT y necesite el ID del registro insertado
- Métodos de WebMethods que insertan registros y retornan el ID al cliente

**Nota**: Los métodos que insertan archivos o registros auxiliares que no necesitan retornar el ID al cliente pueden seguir usando `Insertar()` si no requieren el ID.

### Formato de Código VB.NET

**REGLA CRÍTICA**: Cuando existan bloques de declaraciones `Dim` consecutivas, solo debe haber un salto de línea al final del bloque, después del último `Dim`. No debe haber saltos de línea después de cada `Dim` individual dentro del bloque.

**✅ CORRECTO - Con salto de línea solo al final del bloque de Dim:**
```vb
Dim id As Integer = Convert.ToInt32(datos("id"))
Dim nombre As String = If(datos.ContainsKey("nombre"), datos("nombre")?.ToString(), "")
Dim apellidoPaterno As String = If(datos.ContainsKey("apellidoPaterno"), datos("apellidoPaterno")?.ToString(), "")
Dim apellidoMaterno As String = If(datos.ContainsKey("apellidoMaterno"), datos("apellidoMaterno")?.ToString(), "")
Dim email As String = If(datos.ContainsKey("email"), datos("email")?.ToString(), "")
Dim celular As String = If(datos.ContainsKey("celular"), datos("celular")?.ToString(), "")

' Validar campos requeridos
If String.IsNullOrEmpty(nombre) Then
    ' Código de validación
End If
```

**❌ INCORRECTO - Con saltos de línea después de cada Dim:**
```vb
Dim id As Integer = Convert.ToInt32(datos("id"))

Dim nombre As String = If(datos.ContainsKey("nombre"), datos("nombre")?.ToString(), "")

Dim apellidoPaterno As String = If(datos.ContainsKey("apellidoPaterno"), datos("apellidoPaterno")?.ToString(), "")

Dim apellidoMaterno As String = If(datos.ContainsKey("apellidoMaterno"), datos("apellidoMaterno")?.ToString(), "")
```

**REGLA CRÍTICA**: Los saltos de línea en bloques de control (`For...Next`, `While...Wend`, `Try...Catch`) deben estar **ANTES** y **DESPUÉS** de los bloques, NO dentro del contenido.

**Reglas específicas:**

1. **Try...Catch...End Try**:
   - Salto de línea **ANTES** del `Try`
   - **NO** salto de línea después del `Try` (contenido pegado)
   - Salto de línea **ANTES** del `Catch`
   - **NO** salto de línea después del `Catch` (contenido pegado)
   - Salto de línea **DESPUÉS** del `End Try`

2. **For...Next**:
   - Salto de línea **ANTES** del `For`
   - **NO** salto de línea después del `For` (contenido pegado)
   - Salto de línea **DESPUÉS** del `Next`

3. **While...Wend**:
   - Salto de línea **ANTES** del `While`
   - **NO** salto de línea después del `While` (contenido pegado)
   - Salto de línea **DESPUÉS** del `Wend`

**✅ CORRECTO - Saltos de línea ANTES y DESPUÉS de bloques (no dentro):**
```vb
' Código anterior...
Dim valor As String = "test"

For Each row As DataRow In dt.Rows
    Dim campo = row("Campo")
    ' Procesar valor
Next

' Código siguiente...
resultado = True

Try
    Dim resultado As Boolean = DynamicCrudService.Insertar("tabla", datos)
    ' Procesar resultado

Catch ex As Exception
    Logger.LogError("Metodo", ex)
    Throw

End Try

' Código siguiente...
```

**❌ INCORRECTO - Saltos de línea DENTRO de bloques:**
```vb
Try

    Dim resultado As Boolean = DynamicCrudService.Insertar("tabla", datos)

Catch ex As Exception

    Logger.LogError("Metodo", ex)

End Try
For Each row As DataRow In dt.Rows

    Dim valor = row("Campo")

Next
```

**Principios:**
1. **SIEMPRE** dejar una línea en blanco después del último `Dim` de un bloque de declaraciones consecutivas
2. **NO** dejar líneas en blanco entre `Dim` individuales dentro del mismo bloque
3. **SIEMPRE** dejar una línea en blanco **ANTES** de cada bloque `For`, `While`, `Try`, `Select Case`
4. **SIEMPRE** dejar una línea en blanco **ANTES** del `Catch` (separando del contenido del Try)
5. **SIEMPRE** dejar una línea en blanco **DESPUÉS** de cada `Next`, `Wend`, `End Try`, `End Select`
6. **NUNCA** dejar línea en blanco inmediatamente después de `Try`, `For`, `While`, `Catch`
7. **EXCEPCIÓN**: No se requiere espacio adicional si el bloque está al inicio o final de un método/función (ya hay separación natural)

**Ejemplo completo:**
```vb
Protected Sub ProcesarDatos(datosJson As String)
    Dim datos As Dictionary(Of String, Object) = JsonConvert.DeserializeObject(Of Dictionary(Of String, Object))(datosJson)
    Dim nombre As String = If(datos.ContainsKey("Nombre") AndAlso datos("Nombre") IsNot Nothing, datos("Nombre").ToString(), "")
    Dim apellidoPaterno As String = If(datos.ContainsKey("ApellidoPaterno") AndAlso datos("ApellidoPaterno") IsNot Nothing, datos("ApellidoPaterno").ToString(), "")
    Dim dt As DataTable = TryCast(Session("dtDatos"), DataTable)

    If dt Is Nothing Then
        dt = New DataTable()
    End If

    Try
        Dim nuevaFila = dt.NewRow()
        nuevaFila("Nombre") = nombre
        nuevaFila("ApellidoPaterno") = apellidoPaterno
        dt.Rows.Add(nuevaFila)

    Catch ex As Exception
        Logger.LogError("ProcesarDatos", ex)
        Throw

    End Try

    For Each row As DataRow In dt.Rows
        Dim valor = row("Nombre")
        ' Procesar valor
    Next

End Sub
```

4. **Select Case...End Select**:
   - Salto de línea **DESPUÉS** del `Select Case ...` (antes del primer `Case`)
   - Salto de línea **ANTES** de cada `Case` (separando los bloques Case entre sí)
   - Salto de línea **ANTES** del `End Select`
   - **NO** salto de línea inmediatamente después de cada `Case` (contenido pegado al `Case`)

**✅ CORRECTO - Select Case con separación entre bloques:**
```vb
Select Case col.DataType

    Case GetType(Boolean)
        gridCol = New GridViewDataCheckColumn()
        gridCol.Width = Unit.Pixel(80)

    Case GetType(DateTime), GetType(Date)
        gridCol = New GridViewDataDateColumn()
        gridCol.Width = Unit.Pixel(150)
        CType(gridCol, GridViewDataDateColumn).PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"

    Case GetType(Decimal), GetType(Double), GetType(Single)
        gridCol = New GridViewDataTextColumn()
        gridCol.Width = Unit.Pixel(100)
        gridCol.PropertiesEdit.DisplayFormatString = "c2"

    Case GetType(Integer), GetType(Long), GetType(Short)
        gridCol = New GridViewDataTextColumn()
        gridCol.Width = Unit.Pixel(100)
        gridCol.PropertiesEdit.DisplayFormatString = "n0"

    Case Else
        gridCol = New GridViewDataTextColumn()
        gridCol.Width = Unit.Pixel(150)

End Select
```

**❌ INCORRECTO - Select Case sin separación entre bloques:**
```vb
Select Case col.DataType
    Case GetType(Boolean)
        gridCol = New GridViewDataCheckColumn()
        gridCol.Width = Unit.Pixel(80)
    Case GetType(DateTime), GetType(Date)
        gridCol = New GridViewDataDateColumn()
        gridCol.Width = Unit.Pixel(150)
        CType(gridCol, GridViewDataDateColumn).PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
    Case GetType(Decimal), GetType(Double), GetType(Single)
        gridCol = New GridViewDataTextColumn()
        gridCol.Width = Unit.Pixel(100)
        gridCol.PropertiesEdit.DisplayFormatString = "c2"
    Case Else
        gridCol = New GridViewDataTextColumn()
        gridCol.Width = Unit.Pixel(150)
End Select
```

### Prohibición de Consultas SQL Hardcodeadas en Code-Behind

**REGLA CRÍTICA**: NO deben existir consultas SQL (queries) hardcodeadas en NINGUNA parte del code-behind (`.aspx.vb`). Esto aplica a TODO el archivo, no solo a regiones específicas.

**Áreas donde NO debe haber SQL hardcodeado:**

1. **Eventos de página** (`Page_Load`, `Page_Init`, etc.)
2. **Eventos de controles** (`Button_Click`, `GridView_RowCommand`, etc.)
3. **CustomCallbacks** de grids (`CustomCallback`, `CustomDataCallback`)
4. **Métodos privados/públicos** de cualquier tipo
5. **Regiones** (`#Region "WebMethods"`, `#Region "Helpers"`, etc.)
6. **Cualquier otro lugar** del code-behind

**Ejemplo INCORRECTO - NO HACER:**

```vb
' ❌ INCORRECTO - SQL hardcodeado en eventos de página
Protected Sub Page_Load(sender As Object, e As EventArgs)
    If Not IsPostBack Then
        ' ❌ INCORRECTO - Query SQL directamente en el code-behind
        Dim query As String = "SELECT * FROM cat_unidades WHERE Activo = 1 ORDER BY Nombre"
        Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
        gridUnidades.DataSource = dt
        gridUnidades.DataBind()
    End If
End Sub

' ❌ INCORRECTO - SQL hardcodeado en CustomCallback
Protected Sub gridResidentes_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)
    Try
        Dim partes = e.Parameters.Split("|"c)
        Dim unidadId As Integer = Integer.Parse(partes(1))
        
        ' ❌ INCORRECTO - Query SQL hardcodeado
        Dim query As String = "SELECT r.Id, " &
                              "CONCAT(r.Nombre, ' ', r.ApellidoPaterno) AS NombreCompleto, " &
                              "r.Email, r.Telefono " &
                              "FROM cat_residentes r " &
                              "WHERE r.UnidadId = " & unidadId & " " &
                              "ORDER BY r.Nombre"
        Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
        
        gridResidentes.DataSource = dt
        gridResidentes.DataBind()
    Catch ex As Exception
        Logger.LogError("Error", ex)
    End Try
End Sub

' ❌ INCORRECTO - SQL hardcodeado en métodos privados
Private Sub CargarDatos()
    ' ❌ INCORRECTO - Query SQL directamente en el code-behind
    Dim query As String = "SELECT u.*, e.RazonSocial AS NombreEntidad " &
                          "FROM cat_unidades u " &
                          "INNER JOIN cat_entidades e ON u.entidad_id = e.Id " &
                          "WHERE u.Activo = 1"
    Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
    gridUnidades.DataSource = dt
    gridUnidades.DataBind()
End Sub

' ❌ INCORRECTO - SQL hardcodeado en WebMethods
<System.Web.Services.WebMethod()>
Public Shared Function ObtenerResidente(id As Integer) As Object
    ' ❌ INCORRECTO - Query SQL directamente en el WebMethod del code-behind
    Dim query As String = $"SELECT * FROM cat_residentes WHERE Id = {id}"
    Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
    ' ...
End Function
```

**Ejemplo CORRECTO - SQL en clases de Servicio:**

```vb
' ✅ CORRECTO - Code-behind solo delega al servicio
Protected Sub Page_Load(sender As Object, e As EventArgs)
    If Not IsPostBack Then
        CargarDatos()
    End If
End Sub

Private Sub CargarDatos()
    ' ✅ CORRECTO - Delegando al servicio (sin SQL en el code-behind)
    Dim dt As DataTable = UnidadesService.ListarUnidades()
    GenerarColumnasDinamicas(gridUnidades, dt)
    Session("dtUnidades") = dt
    gridUnidades.DataSource = dt
    gridUnidades.DataBind()
End Sub

' ✅ CORRECTO - CustomCallback delega al servicio
Protected Sub gridResidentes_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)
    Try
        Dim unidadIdActual As String = If(hfUnidadIdActual IsNot Nothing, hfUnidadIdActual.Value, "")
        
        ' ✅ CORRECTO - Toda la lógica SQL está en el servicio
        Dim dt As DataTable = UnidadesService.ProcesarCustomCallbackResidentes(e.Parameters, Session, unidadIdActual)
        
        If dt IsNot Nothing Then
            GenerarColumnasDinamicas(gridResidentes, dt)
            Session("dtResidentes") = dt
            gridResidentes.DataSource = dt
            gridResidentes.DataBind()
        End If
    Catch ex As Exception
        Logger.LogError("gridResidentes_CustomCallback", ex)
    End Try
End Sub

' ✅ CORRECTO - WebMethod solo delega al servicio
<System.Web.Services.WebMethod()>
Public Shared Function ObtenerResidente(id As Integer) As Object
    Return UnidadesService.ObtenerResidente(id)
End Function
```

**Ejemplo CORRECTO - SQL en el Servicio (Services/UnidadesService.vb):**

```vb
Public Class UnidadesService

    Private Const TABLA_UNIDADES As String = "cat_unidades"
    Private Const TABLA_RESIDENTES As String = "cat_residentes"

    ''' <summary>
    ''' Lista todas las unidades activas
    ''' </summary>
    Public Shared Function ListarUnidades() As DataTable
        ' ✅ CORRECTO - SQL en el servicio, no en el code-behind
        Dim query As String = "SELECT u.*, e.RazonSocial AS NombreEntidad " &
                              "FROM cat_unidades u " &
                              "INNER JOIN cat_entidades e ON u.entidad_id = e.Id " &
                              "WHERE u.Activo = 1 " &
                              "ORDER BY u.Nombre"
        Return DynamicCrudService.EjecutarConsulta(query)
    End Function

    ''' <summary>
    ''' Procesa el CustomCallback del grid de residentes
    ''' </summary>
    Public Shared Function ProcesarCustomCallbackResidentes(parameters As String, session As HttpSessionState, unidadIdActual As String) As DataTable
        Dim partes = parameters.Split("|"c)
        
        If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
            Dim unidadId As Integer = Integer.Parse(partes(1))
            
            ' ✅ CORRECTO - SQL en el servicio
            Dim query As String = "SELECT r.Id, " &
                                  "CONCAT(r.Nombre, ' ', r.ApellidoPaterno, ' ', r.ApellidoMaterno) AS NombreCompleto, " &
                                  "r.Email, r.TelefonoCelular AS Celular " &
                                  "FROM cat_residentes r " &
                                  "WHERE r.UnidadId = " & unidadId & " " &
                                  "ORDER BY r.Nombre"
            Return DynamicCrudService.EjecutarConsulta(query)
        End If
        
        Return Nothing
    End Function

    ''' <summary>
    ''' Obtiene un residente por ID
    ''' </summary>
    Public Shared Function ObtenerResidente(id As Integer) As Object
        ' ✅ CORRECTO - SQL en el servicio
        Dim registro As DataRow = DynamicCrudService.ObtenerPorId(TABLA_RESIDENTES, id)
        ' ... procesamiento ...
        Return New With {.success = True, .data = datos}
    End Function

End Class
```

**Dónde debe estar el SQL:**

| Tipo de Operación | Ubicación Correcta |
|-------------------|-------------------|
| Consultas SELECT | `[Modulo]Service.vb` en `/Services/` |
| INSERT/UPDATE/DELETE | `[Modulo]Service.vb` en `/Services/` |
| Lógica de CustomCallback | `[Modulo]Service.ProcesarCustomCallback*()` |
| Consultas con parámetros | `[Modulo]Service.vb` con validación de parámetros |
| Consultas complejas con JOINs | `[Modulo]Service.vb` |

**Principios:**

1. **NUNCA** escribir consultas SQL directamente en archivos `.aspx.vb`
2. **SIEMPRE** colocar las consultas SQL en clases de Servicio (`/Services/[Modulo]Service.vb`)
3. **SIEMPRE** el code-behind solo debe llamar métodos del servicio y hacer DataBind
4. **SIEMPRE** centralizar las consultas SQL en un solo lugar para facilitar mantenimiento
5. Los nombres de tablas deben definirse como constantes en el servicio (`Private Const TABLA_X As String = "..."`)

**Ventajas:**

- **Mantenibilidad**: Cambios en queries SQL se hacen en un solo lugar
- **Seguridad**: Facilita la validación y sanitización de parámetros SQL
- **Reutilización**: Los métodos del servicio pueden ser llamados desde múltiples páginas
- **Testabilidad**: Los servicios pueden ser probados independientemente
- **Separación de responsabilidades**: El code-behind solo maneja la presentación, no el acceso a datos
- **Consistencia**: Todas las consultas a una tabla están centralizadas en su servicio correspondiente

**Aplicar esta regla a:**

- Todos los archivos `.aspx.vb` del proyecto
- Todos los eventos de página, controles y callbacks
- Todos los WebMethods
- Cualquier método que actualmente contenga SQL

---

### Separación de WebMethods y Helpers

**REGLA CRÍTICA**: Todo el código dentro de `#Region "WebMethods"` y `#Region "Helpers"` DEBE estar en clases separadas, NO en el code-behind de las páginas ASPX.

**Estructura obligatoria:**

1. **WebMethods** → Clases de servicio en `JelaWeb/Services/`
   - Cada módulo debe tener su propia clase de servicio (ej: `ComunicadosService.vb`, `PagosService.vb`, `UnidadesService.vb`)
   - Los métodos deben ser `Public Shared` y marcados con `<System.Web.Services.WebMethod()>`
   - La lógica de negocio completa debe estar en estas clases

2. **Helpers** → Clases helper en `JelaWeb/Infrastructure/Helpers/`
   - Cada módulo debe tener su propia clase helper (ej: `ComunicadosHelper.vb`, `PagosHelper.vb`, `UnidadesHelper.vb`)
   - Los métodos deben ser `Public Shared`
   - Funciones auxiliares como `MostrarMensaje`, conversiones de datos, formateo, etc.

3. **Code-behind de páginas ASPX** → Solo delegación
   - Los WebMethods en las páginas solo deben delegar a los servicios correspondientes
   - Los Helpers en las páginas solo deben delegar a las clases helper correspondientes
   - NO debe haber lógica de negocio en el code-behind

**Ejemplo CORRECTO - Estructura de archivos:**

```
JelaWeb/
├── Services/
│   ├── ComunicadosService.vb
│   ├── PagosService.vb
│   ├── UnidadesService.vb
│   └── ...
├── Infrastructure/
│   └── Helpers/
│       ├── ComunicadosHelper.vb
│       ├── PagosHelper.vb
│       ├── UnidadesHelper.vb
│       ├── DataTableHelper.vb (helpers genéricos)
│       └── ...
└── Views/
    └── Operacion/
        └── Condominios/
            ├── Comunicados.aspx
            └── Comunicados.aspx.vb (solo delegación)
```

**Ejemplo CORRECTO - Servicio (Services/ComunicadosService.vb):**

```vb
Imports System.Data
Imports System.Collections.Generic
Imports JelaWeb.Utilities
Imports JelaWeb.Infrastructure.Helpers

''' <summary>
''' Servicio para gestión de Comunicados
''' </summary>
Public Class ComunicadosService

    Private Const TABLA_COMUNICADOS As String = "op_comunicados"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerComunicado(id As Integer) As Object
        Try
            Dim query As String = $"SELECT * FROM {TABLA_COMUNICADOS} WHERE Id = {id} AND Activo = 1"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
            
            If dt.Rows.Count > 0 Then
                Return DataTableHelper.ConvertDataTableToList(dt)(0)
            End If
            
            Return Nothing
        Catch ex As Exception
            Logger.LogError("ComunicadosService.ObtenerComunicado", ex)
            Return New With {.success = False, .message = ex.Message}
        End Try
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarComunicado(datos As Dictionary(Of String, Object)) As Object
        Try
            Dim id As Integer = If(datos.ContainsKey("Id") AndAlso datos("Id") IsNot Nothing, Convert.ToInt32(datos("Id")), 0)
            Dim resultado As Boolean
            Dim nuevoId As Integer = id

            If id = 0 Then
                nuevoId = DynamicCrudService.InsertarConId(TABLA_COMUNICADOS, datos)
                resultado = nuevoId > 0
            Else
                resultado = DynamicCrudService.Actualizar(TABLA_COMUNICADOS, id, datos)
                nuevoId = id
            End If

            Return New With {
                .success = resultado,
                .message = If(resultado, "Guardado correctamente", "Error al guardar"),
                .data = New With {.id = nuevoId}
            }
        Catch ex As Exception
            Logger.LogError("ComunicadosService.GuardarComunicado", ex)
            Return New With {.success = False, .message = ex.Message}
        End Try
    End Function

End Class
```

**Ejemplo CORRECTO - Helper (Infrastructure/Helpers/ComunicadosHelper.vb):**

```vb
Imports System.Web.UI

''' <summary>
''' Helper para funcionalidades auxiliares de Comunicados
''' </summary>
Public Class ComunicadosHelper

    ''' <summary>
    ''' Muestra un mensaje al usuario mediante JavaScript
    ''' </summary>
    Public Shared Sub MostrarMensaje(page As Page, mensaje As String, tipo As String)
        Dim script As String = $"if (typeof showToast !== 'undefined') {{ showToast('{tipo}', '{mensaje.Replace("'", "\'")}'); }} else {{ console.log('[{tipo.ToUpper()}] {mensaje}'); }}"
        ScriptManager.RegisterStartupScript(page, page.GetType(), "showAlert", script, True)
    End Sub

End Class
```

**Ejemplo CORRECTO - Code-behind (Views/Operacion/Condominios/Comunicados.aspx.vb):**

```vb
Imports System.Linq
Imports System.Data
Imports DevExpress.Web
Imports System.Configuration
Imports JelaWeb.Utilities
Imports JelaWeb.Services
Imports JelaWeb.Infrastructure.Helpers

Partial Public Class Comunicados
    Inherits BasePage

#Region "Page Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Try
            If Not IsPostBack Then
                CargarCombos()
                CargarComunicados()
            End If
        Catch ex As Exception
            Logger.LogError("Comunicados.Page_Load", ex)
            ComunicadosHelper.MostrarMensaje(Me, "Error al cargar la página", "error")
        End Try
    End Sub

#End Region

#Region "WebMethods"

    ' Los WebMethods deben permanecer en la página para que ASP.NET los descubra,
    ' pero solo delegan a los servicios correspondientes

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerComunicado(id As Integer) As Object
        Return ComunicadosService.ObtenerComunicado(id)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function GuardarComunicado(datos As Dictionary(Of String, Object)) As Object
        Return ComunicadosService.GuardarComunicado(datos)
    End Function

    <System.Web.Services.WebMethod()>
    Public Shared Function EliminarComunicado(id As Integer) As Object
        Return ComunicadosService.EliminarComunicado(id)
    End Function

#End Region

#Region "Helpers"

    ' Los Helpers se han movido a ComunicadosHelper
    ' Si se necesita algún helper específico de la página, se delega a ComunicadosHelper

#End Region

End Class
```

**Ejemplo INCORRECTO - NO HACER (código en el code-behind):**

```vb
#Region "WebMethods"

    <System.Web.Services.WebMethod()>
    Public Shared Function ObtenerComunicado(id As Integer) As Object
        ' ❌ INCORRECTO - Lógica de negocio directamente en el code-behind
        Try
            Dim query As String = $"SELECT * FROM op_comunicados WHERE Id = {id}"
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
            ' ... más lógica ...
        Catch ex As Exception
            ' ... manejo de errores ...
        End Try
    End Function

#End Region

#Region "Helpers"

    Private Sub MostrarMensaje(mensaje As String, tipo As String)
        ' ❌ INCORRECTO - Helper directamente en el code-behind
        Dim script As String = $"toastr.{tipo}('{mensaje}');"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "toastr", script, True)
    End Sub

#End Region
```

**Principios:**

1. **SIEMPRE** crear una clase de servicio en `JelaWeb/Services/` para cada módulo que tenga WebMethods
2. **SIEMPRE** crear una clase helper en `JelaWeb/Infrastructure/Helpers/` para cada módulo que tenga helpers
3. **SIEMPRE** mantener los WebMethods en las páginas ASPX (requerido por ASP.NET), pero solo como delegación
4. **NUNCA** poner lógica de negocio directamente en el code-behind de las páginas
5. **NUNCA** poner helpers directamente en el code-behind de las páginas
6. Para helpers genéricos (ej: `ConvertDataTableToList`), usar clases helper genéricas como `DataTableHelper.vb`
7. Los helpers que requieren acceso a `Page` (ej: `MostrarMensaje`) deben recibir `page As Page` como parámetro

**Ventajas:**

- **Separación de responsabilidades**: La lógica de negocio está separada de la presentación
- **Reutilización**: Los servicios y helpers pueden ser reutilizados desde otras páginas o servicios
- **Mantenibilidad**: Es más fácil mantener y probar código organizado en clases separadas
- **Testabilidad**: Las clases de servicio pueden ser probadas independientemente
- **Cumplimiento de SOLID**: Sigue principios de diseño orientado a objetos

**Aplicar esta regla a:**

- Todos los archivos `.aspx.vb` que contengan `#Region "WebMethods"` o `#Region "Helpers"`
- Todos los módulos nuevos que se creen
- Refactorizar módulos existentes para seguir esta estructura

### Separación de Lógica en CustomCallback de Grids

**REGLA CRÍTICA**: La lógica de procesamiento dentro de los eventos `CustomCallback` de los grids (ASPxGridView) DEBE estar en métodos del servicio correspondiente, NO en el code-behind de las páginas ASPX.

**Estructura obligatoria:**

1. **Servicio** (`JelaWeb/Services/`) → Métodos `ProcesarCustomCallback*`
   - Contienen toda la lógica de negocio: consultas SQL, procesamiento de datos, manejo de filas unbound
   - Reciben los parámetros necesarios: `parameters`, `session`, y otros valores relevantes
   - Retornan `DataTable` que el code-behind usará para el DataBind

2. **Code-behind** (`.aspx.vb`) → Solo delegación y DataBind
   - Llama al método del servicio correspondiente
   - Genera columnas dinámicas si es necesario
   - Almacena el DataTable en Session
   - Ejecuta el DataBind del grid

**Ejemplo CORRECTO - Servicio (Services/UnidadesService.vb):**

```vb
''' <summary>
''' Procesa el CustomCallback del grid de residentes
''' </summary>
Public Shared Function ProcesarCustomCallbackResidentes(parameters As String, session As System.Web.SessionState.HttpSessionState, unidadIdActual As String) As DataTable
    Try
        Dim partes = parameters.Split("|"c)

        If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
            Dim unidadId As Integer = Integer.Parse(partes(1))

            ' Consulta SQL con toda la lógica de negocio
            Dim query As String = "SELECT r.Id, " &
                                  "CONCAT(IFNULL(r.Nombre, ''), ' ', IFNULL(r.ApellidoPaterno, ''), ' ', IFNULL(r.ApellidoMaterno,'')) AS NombreCompleto, " &
                                  "IFNULL(r.Email, '') AS Email " &
                                  "FROM cat_residentes r WHERE r.UnidadId = " & unidadId

            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)

            ' Procesamiento de datos (conversión de tipos, etc.)
            If dt IsNot Nothing AndAlso dt.Columns.Contains("EsPrincipal") Then
                ' Convertir columna Boolean si es necesario
                ' ... lógica de conversión ...
            End If

            Return dt
        ElseIf partes.Length >= 2 AndAlso partes(0) = "agregarUnbound" Then
            ' Lógica para agregar filas unbound (sin guardar en BD)
            ' ... toda la lógica de parsing JSON, creación de DataTable, etc. ...
            Return dt
        End If

        Return Nothing
    Catch ex As Exception
        Logger.LogError("UnidadesService.ProcesarCustomCallbackResidentes", ex)
        Return Nothing
    End Try
End Function
```

**Ejemplo CORRECTO - Code-behind (Views/Catalogos/Unidades.aspx.vb):**

```vb
Protected Sub gridResidentes_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)

    Try

        Dim unidadIdActual As String = If(hfUnidadIdActual IsNot Nothing, hfUnidadIdActual.Value, "")
        Dim dt As DataTable = UnidadesService.ProcesarCustomCallbackResidentes(e.Parameters, Session, unidadIdActual)

        If dt IsNot Nothing Then
            ' Generar columnas dinámicamente
            GenerarColumnasDinamicas(gridResidentes, dt)

            Session("dtResidentes") = dt
            gridResidentes.DataSource = dt
            gridResidentes.DataBind()
        End If

    Catch ex As Exception

        Logger.LogError("Unidades.gridResidentes_CustomCallback", ex)

    End Try

End Sub
```

**Ejemplo INCORRECTO - NO HACER (lógica en el code-behind):**

```vb
Protected Sub gridResidentes_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs)
    ' ❌ INCORRECTO - Toda esta lógica debería estar en el servicio
    Try
        Dim partes = e.Parameters.Split("|"c)
        
        If partes.Length >= 2 AndAlso partes(0) = "cargar" Then
            Dim unidadId As Integer = Integer.Parse(partes(1))
            
            ' ❌ INCORRECTO - Query SQL hardcodeado en el code-behind
            Dim query As String = "SELECT * FROM cat_residentes WHERE UnidadId = " & unidadId
            Dim dt As DataTable = DynamicCrudService.EjecutarConsulta(query)
            
            ' ❌ INCORRECTO - Procesamiento de datos en el code-behind
            If dt IsNot Nothing AndAlso dt.Columns.Contains("EsPrincipal") Then
                ' ... conversión de tipos ...
            End If
            
            gridResidentes.DataSource = dt
            gridResidentes.DataBind()
        End If
    Catch ex As Exception
        Logger.LogError("Error", ex)
    End Try
End Sub
```

**Nomenclatura estándar para métodos de CustomCallback:**

- `ProcesarCustomCallback[NombreGrid]` - Ejemplo: `ProcesarCustomCallbackResidentes`, `ProcesarCustomCallbackVehiculos`
- El método debe recibir:
  - `parameters As String` - Los parámetros del callback (`e.Parameters`)
  - `session As System.Web.SessionState.HttpSessionState` - La sesión para acceder/guardar DataTables
  - Otros parámetros específicos según el caso (ej: `unidadIdActual As String`)
- El método debe retornar `DataTable` o `Nothing` si hay error

**Principios:**

1. **NUNCA** poner queries SQL directamente en eventos CustomCallback del code-behind
2. **NUNCA** procesar datos (conversiones, parsing JSON, etc.) en el code-behind
3. **SIEMPRE** delegar toda la lógica al método `ProcesarCustomCallback*` del servicio correspondiente
4. **SIEMPRE** el code-behind solo debe: llamar al servicio, generar columnas dinámicas, guardar en Session, y hacer DataBind
5. Los métodos `ProcesarCustomCallback*` no necesitan el atributo `<WebMethod()>` porque no son llamados desde JavaScript

**Ventajas:**

- **Consistencia**: Toda la lógica de negocio en un solo lugar (el servicio)
- **Mantenibilidad**: Fácil de encontrar y modificar la lógica
- **Reutilización**: Los métodos del servicio pueden ser llamados desde otras partes del código
- **Testabilidad**: La lógica puede ser probada independientemente del UI
- **Separación de responsabilidades**: El code-behind solo maneja la presentación

**Aplicar esta regla a:**

- Todos los eventos `CustomCallback` de grids (`ASPxGridView`)
- Todos los eventos `CustomDataCallback` de grids
- Cualquier evento que procese datos y actualice grids

---

### Prohibición de Conexiones Directas a Base de Datos

**REGLA CRÍTICA**: NO deben existir conexiones directas a la base de datos en NINGUNA parte del código. Todas las operaciones de datos DEBEN realizarse a través de la API dinámica (`DynamicCrudService`).

**Lo que está PROHIBIDO:**

1. **Conexiones directas con SqlConnection, MySqlConnection, etc.**
2. **Uso de ADO.NET directo** (SqlCommand, SqlDataAdapter, DataReader)
3. **Entity Framework directo** sin pasar por la API
4. **Cualquier otra forma de acceso directo a la BD**

**Ejemplo INCORRECTO - NO HACER:**

```vb
' ❌ INCORRECTO - Conexión directa a base de datos
Imports System.Data.SqlClient

Public Function ObtenerDatos() As DataTable
    ' ❌ NUNCA HACER ESTO
    Dim connectionString As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Using conn As New SqlConnection(connectionString)
        conn.Open()
        Dim cmd As New SqlCommand("SELECT * FROM cat_unidades", conn)
        Dim adapter As New SqlDataAdapter(cmd)
        Dim dt As New DataTable()
        adapter.Fill(dt)
        Return dt
    End Using
End Function

' ❌ INCORRECTO - MySql directo
Imports MySql.Data.MySqlClient

Public Function ObtenerUsuarios() As DataTable
    Using conn As New MySqlConnection(connectionString)
        ' ❌ NUNCA HACER ESTO
        conn.Open()
        ' ...
    End Using
End Function
```

**Ejemplo CORRECTO - Usar DynamicCrudService:**

```vb
' ✅ CORRECTO - Usar la API dinámica
Imports JelaWeb.Utilities

Public Function ObtenerDatos() As DataTable
    ' ✅ SIEMPRE usar DynamicCrudService
    Dim query As String = "SELECT * FROM cat_unidades WHERE Activo = 1"
    Return DynamicCrudService.EjecutarConsulta(query)
End Function

Public Function ObtenerPorId(id As Integer) As DataRow
    ' ✅ CORRECTO - Usando el método del servicio
    Return DynamicCrudService.ObtenerPorId("cat_unidades", id)
End Function

Public Function Insertar(datos As Dictionary(Of String, Object)) As Boolean
    ' ✅ CORRECTO - Usando el método del servicio
    Return DynamicCrudService.Insertar("cat_unidades", datos)
End Function

Public Function InsertarConId(datos As Dictionary(Of String, Object)) As Integer
    ' ✅ CORRECTO - Obtener ID del registro insertado
    Return DynamicCrudService.InsertarConId("cat_unidades", datos)
End Function

Public Function Actualizar(id As Integer, datos As Dictionary(Of String, Object)) As Boolean
    ' ✅ CORRECTO - Usando el método del servicio
    Return DynamicCrudService.Actualizar("cat_unidades", id, datos)
End Function

Public Function Eliminar(id As Integer) As Boolean
    ' ✅ CORRECTO - Usando el método del servicio
    Return DynamicCrudService.Eliminar("cat_unidades", id)
End Function

Public Function ObtenerTodos() As DataTable
    ' ✅ CORRECTO - Usando el método del servicio
    Return DynamicCrudService.ObtenerTodos("cat_unidades")
End Function

Public Function ObtenerConFiltro(filtro As String) As DataTable
    ' ✅ CORRECTO - Usando el método del servicio
    Return DynamicCrudService.ObtenerTodosConFiltro("cat_unidades", "Activo = 1 AND EntidadId = 5")
End Function
```

**Métodos disponibles en DynamicCrudService:**

| Método | Descripción | Retorno |
|--------|-------------|---------|
| `EjecutarConsulta(query)` | Ejecuta una consulta SELECT | `DataTable` |
| `ObtenerPorId(tabla, id)` | Obtiene un registro por ID | `DataRow` |
| `ObtenerTodos(tabla)` | Obtiene todos los registros | `DataTable` |
| `ObtenerTodosConFiltro(tabla, filtro)` | Obtiene registros con filtro WHERE | `DataTable` |
| `Insertar(tabla, datos)` | Inserta un registro | `Boolean` |
| `InsertarConId(tabla, datos)` | Inserta y retorna el ID | `Integer` |
| `Actualizar(tabla, id, datos)` | Actualiza un registro | `Boolean` |
| `Eliminar(tabla, id)` | Elimina un registro | `Boolean` |

**Por qué usar la API dinámica:**

1. **Seguridad**: La API centraliza la validación y sanitización de datos
2. **Auditabilidad**: Todas las operaciones pasan por un punto central que puede ser auditado
3. **Mantenibilidad**: Cambios en la conexión se hacen en un solo lugar
4. **Escalabilidad**: Facilita migración a microservicios o cambio de BD
5. **Consistencia**: Todos los desarrolladores usan el mismo patrón
6. **Configurabilidad**: La configuración de conexión está centralizada

**Archivos donde verificar esta regla:**

- Todos los archivos en `Services/`
- Todos los archivos en `Infrastructure/Helpers/`
- Todos los archivos `.aspx.vb` (code-behind)
- Cualquier archivo `.vb` del proyecto

**Excepciones:**

- La única excepción es el propio `DynamicCrudService.vb` que internamente consume el API REST
- Clases de infraestructura que implementan la comunicación con el API (`ApiConsumer.vb`, `ApiConsumerCRUD.vb`)

---

### Uso Obligatorio de DynamicCrudService (No usar ApiConsumer directo)

**REGLA CRÍTICA**: SIEMPRE usar `DynamicCrudService` para acceder a datos. NUNCA usar `ApiConsumer` o `ApiConsumerCRUD` directamente en Services, Helpers o Code-behind.

**Arquitectura del acceso a datos:**

```
┌─────────────────────────────────────────────────────────────────┐
│                        TU CÓDIGO                                │
│         (Services, Code-behind, Helpers)                        │
└───────────────────────┬─────────────────────────────────────────┘
                        │
                        ▼
              ┌─────────────────────┐
              │  DynamicCrudService │  ◄── USAR SIEMPRE ESTE
              │   (Alto nivel)      │
              └─────────┬───────────┘
                        │ Internamente usa
                        ▼
              ┌─────────────────────┐
              │    ApiConsumer      │  ◄── NO USAR DIRECTAMENTE
              │  ApiConsumerCRUD    │
              └─────────┬───────────┘
                        │
                        ▼
              ┌─────────────────────┐
              │   API REST (HTTP)   │
              └─────────────────────┘
```

**Diferencias entre ambos:**

| Aspecto | `DynamicCrudService` | `ApiConsumer` (directo) |
|---------|---------------------|------------------------|
| **Nivel** | Alto nivel (abstracción) | Bajo nivel (implementación) |
| **Uso** | `ObtenerTodos("tabla")` | Construir URL + `ObtenerDatos(url)` + `ConvertirADatatable()` |
| **CRUD** | Métodos listos: `Insertar`, `Actualizar`, `Eliminar` | Solo GET, necesita `ApiConsumerCRUD` para POST/PUT/DELETE |
| **Sanitización** | Incluida automáticamente | Manual |
| **Manejo de errores** | Centralizado con logging | Manual en cada llamada |
| **Código requerido** | 1 línea | 3-5 líneas |

**Ejemplo INCORRECTO - NO HACER (usar ApiConsumer directo):**

```vb
' ❌ INCORRECTO - Usar ApiConsumer directamente
Dim apiConsumer As New ApiConsumer()
Dim baseUrl As String = ConfigurationManager.AppSettings("ApiBaseUrl")

Dim query As String = "SELECT Id, Nombre FROM conf_usuarios WHERE Activo = 1"
Dim url As String = baseUrl & System.Web.HttpUtility.UrlEncode(query)
Dim datos = apiConsumer.ObtenerDatos(url)
Dim dt As DataTable = apiConsumer.ConvertirADatatable(datos)

' ❌ INCORRECTO - Múltiples líneas para una operación simple
Dim queryAgentes As String = "SELECT Id, Nombre FROM conf_usuarios WHERE Activo = 1 ORDER BY Nombre"
Dim urlAgentes As String = apiBaseUrl & System.Web.HttpUtility.UrlEncode(queryAgentes)
Dim datosAgentes = apiConsumer.ObtenerDatos(urlAgentes)
Dim dtAgentes = apiConsumer.ConvertirADatatable(datosAgentes)
```

**Ejemplo CORRECTO - Usar DynamicCrudService:**

```vb
' ✅ CORRECTO - Una sola línea con DynamicCrudService
Dim dt As DataTable = DynamicCrudService.EjecutarConsulta("SELECT Id, Nombre FROM conf_usuarios WHERE Activo = 1")

' ✅ CORRECTO - Métodos específicos para operaciones comunes
Dim dtUsuarios As DataTable = DynamicCrudService.ObtenerTodosConFiltro("conf_usuarios", "Activo = 1")

' ✅ CORRECTO - CRUD simplificado
Dim registro As DataRow = DynamicCrudService.ObtenerPorId("cat_unidades", 5)
Dim exito As Boolean = DynamicCrudService.Insertar("cat_unidades", datos)
Dim exito As Boolean = DynamicCrudService.Actualizar("cat_unidades", 5, datos)
Dim exito As Boolean = DynamicCrudService.Eliminar("cat_unidades", 5)

' ✅ CORRECTO - Insertar y obtener ID generado
Dim nuevoId As Integer = DynamicCrudService.InsertarConId("cat_unidades", datos)
```

**Ventajas de usar DynamicCrudService:**

1. **Menos código**: Operaciones en 1 línea vs 3-5 líneas
2. **Sanitización automática**: Protección contra SQL injection integrada
3. **Manejo de errores centralizado**: Logging automático de errores
4. **Métodos CRUD completos**: No necesitas instanciar múltiples clases
5. **Consistencia**: Todos los desarrolladores usan el mismo patrón
6. **Mantenibilidad**: Cambios en la comunicación con el API se hacen en un solo lugar

**Cuándo se permite usar ApiConsumer directo:**

- **NUNCA** en Services, Helpers o Code-behind
- Solo está permitido dentro de `DynamicCrudService.vb` (que es quien lo encapsula)
- Solo en clases de infraestructura de muy bajo nivel

**Archivos que deben refactorizarse si usan ApiConsumer directo:**

- `Tickets.aspx.vb` - Actualmente usa `apiConsumer` directo, debe migrar a `DynamicCrudService`
- Cualquier otro archivo que instancie `New ApiConsumer()` o `New ApiConsumerCRUD()`

**Principios:**

1. **SIEMPRE** usar `DynamicCrudService` para cualquier acceso a datos
2. **NUNCA** instanciar `ApiConsumer` o `ApiConsumerCRUD` fuera de la capa de infraestructura
3. **SIEMPRE** preferir los métodos específicos (`ObtenerPorId`, `ObtenerTodosConFiltro`) sobre `EjecutarConsulta` cuando sea posible
4. Para queries complejos con JOINs, usar `DynamicCrudService.EjecutarConsulta()`

---

**Última actualización**: Enero 2025  
**Versión**: 1.3  
**Mantenido por**: Equipo de Desarrollo JELABBC

e estándar:**
```javascript
$.ajax({
    url: '/api/endpoint',
    method: 'POST',
    data: JSON.stringify(requestData),
    contentType: 'application/json',
    success: function(response) {
        // Manejar respuesta exitosa
        mostrarExito('Operación completada');
    },
    error: function(xhr, status, error) {
        // Manejar error
        var mensaje = 'Error en la operación';
        if (xhr.responseJSON && xhr.responseJSON.message) {
            mensaje = xhr.responseJSON.message;
        }
        mostrarError(mensaje);
    }
});
```

---

## Estándares de Arquitectura Backend

### 15. Servicios Backend y Lógica de Negocio

**REGLA CRÍTICA**: Toda lógica de negocio, servicios backend y tareas programadas DEBEN implementarse en **JELA.API (.NET 8)**, NO como servicios VB.NET separados.

**Razón**: 
- Centralizar toda la lógica de negocio en un solo lugar
- Aprovechar las ventajas de .NET 8 (rendimiento, seguridad, escalabilidad)
- Facilitar el mantenimiento y testing
- Evitar duplicación de código
- Mejor observabilidad y logging

**Implementación Obligatoria:**

#### 15.1 Servicios de Lógica de Negocio

**Ubicación**: `JELA.API/Services/`

**Ejemplo de servicio:**
```csharp
namespace JELA.API.Services;

public interface ITicketValidationService
{
    Task<ValidationResult> ValidarClienteDuplicado(string telefono, string email, string ip, int idEntidad);
    Task<List<TicketDto>> ObtenerHistorialCliente(string telefono);
}

public class TicketValidationService : ITicketValidationService
{
    private readonly IDatabaseService _db;
    private readonly ILogger<TicketValidationService> _logger;
    
    public TicketValidationService(IDatabaseService db, ILogger<TicketValidationService> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public async Task<ValidationResult> ValidarClienteDuplicado(
        string telefono, string email, string ip, int idEntidad)
    {
        try
        {
            var query = @"
                SELECT COUNT(*) as TieneTicketAbierto, 
                       MAX(Id) as IdTicketAbierto
                FROM op_tickets_v2 
                WHERE (TelefonoCliente = @Telefono OR EmailCliente = @Email OR IPOrigen = @IP)
                AND Estado IN ('Abierto', 'EnProceso')
                AND IdEntidad = @IdEntidad
                AND Activo = 1";
            
            var result = await _db.QueryFirstOrDefaultAsync<ValidationResult>(
                query, 
                new { Telefono = telefono, Email = email, IP = ip, IdEntidad = idEntidad });
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validando cliente duplicado");
            throw;
        }
    }
    
    public async Task<List<TicketDto>> ObtenerHistorialCliente(string telefono)
    {
        var query = @"
            SELECT * FROM op_tickets_v2 
            WHERE TelefonoCliente = @Telefono 
            ORDER BY FechaCreacion DESC 
            LIMIT 10";
        
        return (await _db.QueryAsync<TicketDto>(query, new { Telefono = telefono })).ToList();
    }
}
```

**Registro en Program.cs:**
```csharp
// Registrar servicios de lógica de negocio
builder.Services.AddScoped<ITicketValidationService, TicketValidationService>();
builder.Services.AddScoped<ITicketNotificationService, TicketNotificationService>();
```

#### 15.2 Background Services (Tareas Programadas)

**Ubicación**: `JELA.API/BackgroundServices/`

**Para tareas que deben ejecutarse periódicamente** (robots, cálculo de métricas, etc.), usar `BackgroundService` de .NET:

```csharp
namespace JELA.API.BackgroundServices;

public class TicketMonitoringBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TicketMonitoringBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);
    
    public TicketMonitoringBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<TicketMonitoringBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TicketMonitoringBackgroundService iniciado");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await MonitorearTickets();
                await Task.Delay(_interval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en ciclo de monitoreo");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
    
    private async Task MonitorearTickets()
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDatabaseService>();
        
        // Lógica de monitoreo
        var ticketsPendientes = await db.QueryAsync<TicketDto>(@"
            SELECT * FROM op_tickets_v2 
            WHERE Estado = 'Abierto' 
            AND TIMESTAMPDIFF(HOUR, FechaCreacion, NOW()) > 24
            AND Activo = 1");
        
        foreach (var ticket in ticketsPendientes)
        {
            _logger.LogWarning("Ticket {TicketId} lleva más de 24 horas sin atender", ticket.Id);
            // Enviar notificación, escalar, etc.
        }
    }
}
```

**Registro en Program.cs:**
```csharp
// Registrar background services
builder.Services.AddHostedService<TicketMonitoringBackgroundService>();
builder.Services.AddHostedService<TicketMetricsBackgroundService>();
builder.Services.AddHostedService<PromptTuningBackgroundService>();
```

#### 15.3 Endpoints para Servicios

**Los servicios de lógica de negocio se exponen a través de endpoints:**

```csharp
// Endpoints/TicketValidationEndpoints.cs
public static class TicketValidationEndpoints
{
    public static void MapTicketValidationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tickets")
            .WithTags("Ticket Validation")
            .RequireAuthorization();

        group.MapPost("/validar-cliente", async (
            [FromBody] ValidarClienteRequest request,
            [FromServices] ITicketValidationService validationService) =>
        {
            var result = await validationService.ValidarClienteDuplicado(
                request.Telefono, 
                request.Email, 
                request.IPOrigen, 
                request.IdEntidad);
            
            return Results.Ok(result);
        })
        .WithName("ValidarCliente")
        .WithOpenApi();
    }
}
```

#### 15.4 Consumo desde VB.NET

**Las páginas ASP.NET VB.NET consumen estos servicios a través de la API:**

```vb
' En JelaWeb (VB.NET)
Public Class TicketsPage
    Inherits BasePage
    
    Private _apiConsumer As ApiConsumerCRUD
    
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        _apiConsumer = New ApiConsumerCRUD()
        
        If Not IsPostBack Then
            CargarTickets()
        End If
    End Sub
    
    Private Sub CargarTickets()
        Try
            ' Llamar al endpoint de la API
            Dim endpoint = "/api/tickets/validar-cliente"
            Dim requestData = New With {
                .Telefono = "1234567890",
                .Email = "cliente@example.com",
                .IPOrigen = "192.168.1.1",
                .IdEntidad = 1
            }
            
            Dim response = _apiConsumer.PostAsync(endpoint, requestData).Result
            
            If response.IsSuccessStatusCode Then
                Dim result = JsonConvert.DeserializeObject(Of ValidationResult)(
                    response.Content.ReadAsStringAsync().Result)
                
                ' Procesar resultado
            End If
        Catch ex As Exception
            Logger.LogError("CargarTickets", ex)
        End Try
    End Sub
End Class
```

#### 15.5 Ventajas de Este Enfoque

1. **Centralización**: Toda la lógica en un solo lugar (JELA.API)
2. **Reutilización**: Los mismos servicios se usan desde web, webhooks, background tasks
3. **Testing**: Más fácil hacer unit tests y integration tests
4. **Escalabilidad**: Se puede escalar la API independientemente
5. **Observabilidad**: Logging y métricas centralizadas
6. **Mantenibilidad**: Un solo lenguaje y framework para backend (.NET 8)
7. **Seguridad**: Autenticación JWT centralizada
8. **Documentación**: Swagger/OpenAPI automático

#### 15.6 Prohibiciones

**NO HACER:**
- ❌ Crear servicios Windows en VB.NET separados
- ❌ Crear aplicaciones de consola VB.NET para tareas programadas
- ❌ Duplicar lógica de negocio entre VB.NET y C#
- ❌ Acceder directamente a la base de datos desde VB.NET

**HACER:**
- ✅ Implementar toda la lógica en JELA.API (.NET 8)
- ✅ Usar Background Services para tareas programadas
- ✅ Exponer funcionalidad a través de endpoints
- ✅ Consumir API desde VB.NET usando ApiConsumerCRUD

---

## Resumen de Estándares Críticos

### Checklist de Cumplimiento

Antes de implementar cualquier módulo, verificar:

- [ ] CSS y JavaScript en archivos separados
- [ ] Nomenclatura contextual en botones
- [ ] Tablas con prefijo y guion bajo (`op_`, `conf_`)
- [ ] Campos en PascalCase
- [ ] Toolbar del grid con acciones CRUD
- [ ] Sin paginación en grids
- [ ] Filtros nativos del grid (no externos)
- [ ] Solo fechas arriba del grid
- [ ] Columnas dinámicas generadas desde API
- [ ] Popups para captura (no páginas nuevas)
- [ ] Evento DataBound con FuncionesGridWeb.SUMColumn
- [ ] **Lógica de negocio en JELA.API (.NET 8)**
- [ ] **Background Services para tareas programadas**
- [ ] **Consumo de API desde VB.NET**

---

**Última actualización:** 18 de Enero de 2026  
**Versión:** 2.0
