# Corrección de Diseño: Panel Admin JELA - Enfoque Dinámico

## Problema Identificado

El diseño actual especifica crear **7 controladores nuevos con DTOs fijos**, pero el sistema ya tiene una **API CRUD Dinámica** que maneja todas las operaciones CRUD sin necesidad de controladores o DTOs específicos.

## API CRUD Dinámica Existente

### Endpoints Disponibles (YA EXISTEN)

```
GET /api/crud?strQuery=SELECT...
  - Ejecuta cualquier consulta SELECT
  - Retorna List<CrudDto> con campos dinámicos

POST /api/crud/{tabla}
  - Inserta registro en cualquier tabla
  - Body: CrudRequest con Dictionary<string, CampoConTipo>

PUT /api/crud/{tabla}/{id}
  - Actualiza registro por ID
  - Body: CrudRequest con Dictionary<string, CampoConTipo>

DELETE /api/crud/{tabla}/{id}
  - Elimina registro por ID
```

### Modelo Dinámico (YA EXISTE)

```csharp
public class CrudDto
{
    public Dictionary<string, CampoConTipo> Campos { get; set; } = new();
}

public class CampoConTipo
{
    public object? Valor { get; set; }
    public string Tipo { get; set; } = "System.String";
}
```

## Qué NO Crear (Diseño Incorrecto)

### ❌ NO Crear Controladores Fijos
- AdministradoresController
- EntidadesController
- LicenciasController
- DashboardController
- AuditoriaController
- AlertasController
- ConfiguracionController

### ❌ NO Crear DTOs Fijos
- CrearAdministradorDto
- ActualizarAdministradorDto
- AdministradorDto
- CrearEntidadDto
- ActualizarEntidadDto
- EntidadDto
- LicenciaDto
- etc.

### ❌ NO Crear Repositorios
- AdministradorRepository
- EntidadRepository
- LicenciaRepository
- AsignacionRepository
- etc.

## Qué SÍ Crear (Diseño Correcto)

### ✅ 1. Tablas de Base de Datos (4 nuevas)

```sql
-- 1. Licencias por administrador
CREATE TABLE conf_licencias (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdAdministrador INT NOT NULL,
    LicenciasTotales INT NOT NULL DEFAULT 0,
    LicenciasConsumidas INT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FechaModificacion DATETIME NULL,
    FOREIGN KEY (IdAdministrador) REFERENCES conf_usuarios(Id),
    INDEX idx_administrador (IdAdministrador)
);

-- 2. Auditoría de operaciones administrativas
CREATE TABLE conf_auditoria_admin (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdUsuario INT NOT NULL,
    FechaOperacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    TipoOperacion VARCHAR(50) NOT NULL,
    Descripcion TEXT NOT NULL,
    DatosAnteriores JSON NULL,
    FOREIGN KEY (IdUsuario) REFERENCES conf_usuarios(Id),
    INDEX idx_fecha (FechaOperacion),
    INDEX idx_tipo (TipoOperacion)
);

-- 3. Configuración del sistema
CREATE TABLE conf_sistema (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdEntidadPlantilla INT NOT NULL,
    FechaModificacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (IdEntidadPlantilla) REFERENCES cat_entidades(Id)
);

-- 4. Alertas de licencias
CREATE TABLE conf_alertas_licencias (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    IdAdministrador INT NOT NULL,
    TipoAlerta ENUM('Advertencia', 'Critica') NOT NULL,
    Mensaje TEXT NOT NULL,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    Resuelta TINYINT(1) NOT NULL DEFAULT 0,
    FechaResolucion DATETIME NULL,
    FOREIGN KEY (IdAdministrador) REFERENCES conf_usuarios(Id),
    INDEX idx_administrador (IdAdministrador),
    INDEX idx_resuelta (Resuelta)
);
```

### ✅ 2. Servicios de Lógica de Negocio (Solo 4)

**IMPORTANTE**: Solo crear servicios para operaciones complejas que requieren transacciones o lógica de negocio. Las operaciones CRUD simples usan directamente `/api/crud`.

#### ConfiguracionEntidadService

```csharp
public class ConfiguracionEntidadService
{
    private readonly IDatabaseService _database;
    private readonly AuditoriaService _auditoria;
    
    /// <summary>
    /// Configura automáticamente una entidad nueva copiando datos de la entidad plantilla.
    /// Usa transacción para garantizar atomicidad.
    /// </summary>
    public async Task ConfigurarEntidadNueva(int idEntidad, int idEntidadPlantilla, int idUsuario)
    {
        using var transaction = await _database.BeginTransactionAsync();
        try
        {
            // 1. Copiar prompts de conf_ticket_prompts
            await CopiarPrompts(idEntidad, idEntidadPlantilla);
            
            // 2. Copiar SLAs de conf_ticket_sla
            await CopiarSLAs(idEntidad, idEntidadPlantilla);
            
            // 3. Copiar categorías de cat_categorias_ticket
            await CopiarCategorias(idEntidad, idEntidadPlantilla);
            
            // 4. Crear áreas comunes predeterminadas
            await CrearAreasComunes(idEntidad);
            
            // 5. Crear conceptos de pago predeterminados
            await CrearConceptosPago(idEntidad);
            
            // 6. Registrar en auditoría
            await _auditoria.RegistrarOperacion(
                idUsuario,
                "CrearEntidad",
                $"Entidad {idEntidad} configurada automáticamente desde plantilla {idEntidadPlantilla}"
            );
            
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    private async Task CopiarPrompts(int idEntidad, int idEntidadPlantilla)
    {
        var query = @"
            INSERT INTO conf_ticket_prompts (IdEntidad, NombrePrompt, TextoPrompt, Activo)
            SELECT @idEntidad, NombrePrompt, TextoPrompt, Activo
            FROM conf_ticket_prompts
            WHERE IdEntidad = @idEntidadPlantilla";
        
        await _database.EjecutarNoConsultaAsync(query, new Dictionary<string, object>
        {
            { "@idEntidad", idEntidad },
            { "@idEntidadPlantilla", idEntidadPlantilla }
        });
    }
    
    // Métodos similares para CopiarSLAs, CopiarCategorias, CrearAreasComunes, CrearConceptosPago
}
```

#### LicenciaService

```csharp
public class LicenciaService
{
    private readonly IDatabaseService _database;
    private readonly AlertasService _alertas;
    
    /// <summary>
    /// Incrementa el consumo de licencias al asignar una entidad.
    /// Verifica y crea alertas si es necesario.
    /// </summary>
    public async Task IncrementarConsumo(int idAdministrador)
    {
        var query = @"
            UPDATE conf_licencias 
            SET LicenciasConsumidas = LicenciasConsumidas + 1,
                FechaModificacion = NOW()
            WHERE IdAdministrador = @idAdministrador";
        
        await _database.EjecutarNoConsultaAsync(query, new Dictionary<string, object>
        {
            { "@idAdministrador", idAdministrador }
        });
        
        await _alertas.VerificarYCrearAlertas(idAdministrador);
    }
    
    /// <summary>
    /// Decrementa el consumo de licencias al desasignar una entidad.
    /// Resuelve alertas si las licencias disponibles son suficientes.
    /// </summary>
    public async Task DecrementarConsumo(int idAdministrador)
    {
        var query = @"
            UPDATE conf_licencias 
            SET LicenciasConsumidas = LicenciasConsumidas - 1,
                FechaModificacion = NOW()
            WHERE IdAdministrador = @idAdministrador";
        
        await _database.EjecutarNoConsultaAsync(query, new Dictionary<string, object>
        {
            { "@idAdministrador", idAdministrador }
        });
        
        await _alertas.ResolverAlertasSiCorresponde(idAdministrador);
    }
    
    /// <summary>
    /// Verifica si el administrador tiene licencias disponibles.
    /// </summary>
    public async Task<bool> TieneLicenciasDisponibles(int idAdministrador)
    {
        var query = @"
            SELECT (LicenciasTotales - LicenciasConsumidas) AS Disponibles
            FROM conf_licencias
            WHERE IdAdministrador = @idAdministrador";
        
        var disponibles = await _database.EjecutarEscalarAsync<int>(query, new Dictionary<string, object>
        {
            { "@idAdministrador", idAdministrador }
        });
        
        return disponibles > 0;
    }
}
```

#### AuditoriaService

```csharp
public class AuditoriaService
{
    private readonly IDatabaseService _database;
    
    /// <summary>
    /// Registra una operación administrativa en la auditoría.
    /// </summary>
    public async Task RegistrarOperacion(
        int idUsuario,
        string tipoOperacion,
        string descripcion,
        object datosAnteriores = null)
    {
        var datosJson = datosAnteriores != null 
            ? JsonSerializer.Serialize(datosAnteriores) 
            : null;
        
        var campos = new Dictionary<string, object>
        {
            { "IdUsuario", idUsuario },
            { "TipoOperacion", tipoOperacion },
            { "Descripcion", descripcion },
            { "DatosAnteriores", datosJson }
        };
        
        await _database.InsertarAsync("conf_auditoria_admin", campos);
    }
}
```

#### AlertasService

```csharp
public class AlertasService
{
    private readonly IDatabaseService _database;
    
    /// <summary>
    /// Verifica las licencias disponibles y crea alertas si es necesario.
    /// </summary>
    public async Task VerificarYCrearAlertas(int idAdministrador)
    {
        var query = @"
            SELECT (LicenciasTotales - LicenciasConsumidas) AS Disponibles
            FROM conf_licencias
            WHERE IdAdministrador = @idAdministrador";
        
        var disponibles = await _database.EjecutarEscalarAsync<int>(query, new Dictionary<string, object>
        {
            { "@idAdministrador", idAdministrador }
        });
        
        if (disponibles == 0)
        {
            await CrearAlerta(idAdministrador, "Critica", "No hay licencias disponibles");
        }
        else if (disponibles < 2)
        {
            await CrearAlerta(idAdministrador, "Advertencia", $"Solo {disponibles} licencia(s) disponible(s)");
        }
    }
    
    /// <summary>
    /// Resuelve alertas si las licencias disponibles son suficientes.
    /// </summary>
    public async Task ResolverAlertasSiCorresponde(int idAdministrador)
    {
        var query = @"
            SELECT (LicenciasTotales - LicenciasConsumidas) AS Disponibles
            FROM conf_licencias
            WHERE IdAdministrador = @idAdministrador";
        
        var disponibles = await _database.EjecutarEscalarAsync<int>(query, new Dictionary<string, object>
        {
            { "@idAdministrador", idAdministrador }
        });
        
        if (disponibles >= 2)
        {
            var updateQuery = @"
                UPDATE conf_alertas_licencias
                SET Resuelta = 1, FechaResolucion = NOW()
                WHERE IdAdministrador = @idAdministrador AND Resuelta = 0";
            
            await _database.EjecutarNoConsultaAsync(updateQuery, new Dictionary<string, object>
            {
                { "@idAdministrador", idAdministrador }
            });
        }
    }
    
    private async Task CrearAlerta(int idAdministrador, string tipoAlerta, string mensaje)
    {
        var campos = new Dictionary<string, object>
        {
            { "IdAdministrador", idAdministrador },
            { "TipoAlerta", tipoAlerta },
            { "Mensaje", mensaje }
        };
        
        await _database.InsertarAsync("conf_alertas_licencias", campos);
    }
}
```

### ✅ 3. Páginas WebForms (7 páginas)

**IMPORTANTE**: Todas las páginas DEBEN seguir los estándares de UI definidos en `.kiro/specs/ecosistema-jelabbc/ui-standards.md`

#### Estándares de UI Obligatorios

1. **Color de fondo**: `#E4EFFA` en todas las páginas
2. **CSS y JavaScript**: SIEMPRE en archivos separados, NUNCA inline
3. **Nomenclatura contextual**: "Nueva Entidad" NO "Nuevo", "Editar Administrador" NO "Editar"
4. **Grids - Paginación**: `Mode="ShowAllRecords"` (mostrar TODOS los registros)
5. **Grids - Filtros**: Solo fechas arriba del grid, todos los demás filtros en columnas con `AllowHeaderFilter="True"`
6. **Grids - Columnas dinámicas**: Generar desde DataTable, NO definir estáticamente en ASPX
7. **Grids - Toolbar**: Acciones CRUD en toolbar del grid, NO como botones externos
8. **Grids - DataBound**: SIEMPRE implementar evento DataBound con `FuncionesGridWeb.SUMColumn`
9. **Popups**: Usar `ASPxPopupControl` para captura de datos, NO abrir otras páginas
10. **Validación**: Usar validadores DevExpress dentro de popups

#### Patrón Estándar de Página

Todas las páginas siguen este patrón:

1. **Cargar datos**: Llamar a `/api/crud?strQuery=SELECT...`
2. **Generar columnas dinámicamente**: Usar `GenerarColumnasDinamicas(grid, dataTable)`
3. **Operaciones CRUD**: Llamar a POST/PUT/DELETE en `/api/crud/{tabla}`
4. **Aplicar estándares**: Usar `FuncionesGridWeb.SUMColumn` en DataBound
5. **CSS separado**: Crear archivo en `/Content/css/modules/{nombre}.css`
6. **JavaScript separado**: Crear archivo en `/Scripts/app/modules/{nombre}.js`

#### Ejemplo: Administradores.aspx.vb

**IMPORTANTE**: Este ejemplo incluye todos los estándares de UI obligatorios.

##### Estructura de Archivos

```
/Views/Admin/
  Administradores.aspx          # Página principal
  Administradores.aspx.vb        # Code-behind
  
/Content/css/modules/
  administradores.css            # CSS separado (OBLIGATORIO)
  
/Scripts/app/modules/
  administradores.js             # JavaScript separado (OBLIGATORIO)
```

##### Administradores.aspx (Markup)

```html
<%@ Page Title="Gestión de Administradores" Language="vb" 
         MasterPageFile="~/MasterPages/Jela.Master" 
         AutoEventWireup="false" 
         CodeBehind="Administradores.aspx.vb" 
         Inherits="PanelAdmin.Administradores" %>

<!-- CSS SEPARADO - OBLIGATORIO -->
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/Content/css/modules/administradores.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Breadcrumb -->
    <nav aria-label="breadcrumb">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="~/Views/Dashboard.aspx">Inicio</a></li>
            <li class="breadcrumb-item active">Administradores</li>
        </ol>
    </nav>
    
    <!-- Título de página -->
    <div class="page-header">
        <h1>Gestión de Administradores</h1>
        <p class="text-muted">Administre los administradores de condominios del sistema</p>
    </div>
    
    <!-- Grid principal con columnas dinámicas -->
    <div class="grid-container">
        <dx:ASPxGridView ID="gridAdministradores" runat="server" 
                         AutoGenerateColumns="False"
                         KeyFieldName="Id"
                         OnRowCommand="gridAdministradores_RowCommand"
                         OnDataBound="gridAdministradores_DataBound">
            
            <!-- PAGINACIÓN: ShowAllRecords - OBLIGATORIO -->
            <SettingsPager Mode="ShowAllRecords" />
            
            <!-- FILTROS: Solo en columnas, NO arriba del grid - OBLIGATORIO -->
            <Settings ShowFilterRow="False" 
                      ShowFilterRowMenu="True" 
                      ShowGroupPanel="True" />
            
            <SettingsBehavior AllowSort="True" 
                              AllowGroup="True" 
                              AllowFixedGroups="True" />
            
            <!-- Barra de búsqueda global - OBLIGATORIO -->
            <SettingsSearchPanel Visible="True" 
                                 ShowApplyButton="True" 
                                 ShowClearButton="True" />
            
            <!-- NOMENCLATURA CONTEXTUAL - OBLIGATORIO -->
            <SettingsCommandButton>
                <NewButton Text="Nuevo Administrador">
                    <Image IconID="actions_add_16x16" />
                </NewButton>
                <EditButton Text="Editar Administrador">
                    <Image IconID="edit_edit_16x16" />
                </EditButton>
                <DeleteButton Text="Eliminar Administrador">
                    <Image IconID="edit_delete_16x16" />
                </DeleteButton>
            </SettingsCommandButton>
            
            <!-- TOOLBAR: Acciones en toolbar, NO botones externos - OBLIGATORIO -->
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
            
            <!-- COLUMNAS DINÁMICAS: NO definir aquí, generar en code-behind - OBLIGATORIO -->
            <Columns>
                <!-- Las columnas se generan dinámicamente en GenerarColumnasDinamicas() -->
            </Columns>
        </dx:ASPxGridView>
    </div>
    
    <!-- POPUP: Para captura de datos, NO abrir otras páginas - OBLIGATORIO -->
    <dx:ASPxPopupControl ID="popupAdministrador" runat="server" 
                         ClientInstanceName="popupAdministrador"
                         HeaderText="Nuevo Administrador"
                         Width="600px"
                         Height="500px"
                         Modal="True"
                         PopupHorizontalAlign="WindowCenter"
                         PopupVerticalAlign="WindowCenter"
                         CloseAction="CloseButton"
                         ShowFooter="True">
        
        <HeaderStyle BackColor="#007bff" ForeColor="White" />
        
        <ContentCollection>
            <dx:PopupControlContentControl>
                <div class="form-container">
                    <!-- VALIDACIÓN: Usar validadores DevExpress - OBLIGATORIO -->
                    <dx:ASPxValidationSummary ID="validationSummary" runat="server" 
                                              ValidationGroup="AdminValidation" />
                    
                    <div class="form-group">
                        <label>Usuario: <span class="required">*</span></label>
                        <dx:ASPxTextBox ID="txtUsuario" runat="server" Width="100%">
                            <ValidationSettings ValidationGroup="AdminValidation">
                                <RequiredField IsRequired="True" ErrorText="El usuario es requerido" />
                            </ValidationSettings>
                        </dx:ASPxTextBox>
                    </div>
                    
                    <div class="form-group">
                        <label>Email: <span class="required">*</span></label>
                        <dx:ASPxTextBox ID="txtEmail" runat="server" Width="100%">
                            <ValidationSettings ValidationGroup="AdminValidation">
                                <RequiredField IsRequired="True" ErrorText="El email es requerido" />
                                <RegularExpression ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                                                   ErrorText="Email inválido" />
                            </ValidationSettings>
                        </dx:ASPxTextBox>
                    </div>
                    
                    <div class="form-group">
                        <label>Nombre Completo: <span class="required">*</span></label>
                        <dx:ASPxTextBox ID="txtNombreCompleto" runat="server" Width="100%">
                            <ValidationSettings ValidationGroup="AdminValidation">
                                <RequiredField IsRequired="True" ErrorText="El nombre completo es requerido" />
                            </ValidationSettings>
                        </dx:ASPxTextBox>
                    </div>
                    
                    <div class="form-group">
                        <label>Licencias Totales: <span class="required">*</span></label>
                        <dx:ASPxSpinEdit ID="txtLicenciasTotales" runat="server" Width="100%" 
                                         MinValue="1" MaxValue="999">
                            <ValidationSettings ValidationGroup="AdminValidation">
                                <RequiredField IsRequired="True" ErrorText="Las licencias son requeridas" />
                            </ValidationSettings>
                        </dx:ASPxSpinEdit>
                    </div>
                    
                    <div class="form-group">
                        <dx:ASPxCheckBox ID="chkActivo" runat="server" Text="Activo" Checked="True" />
                    </div>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
        
        <FooterTemplate>
            <div class="popup-footer">
                <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" 
                              OnClick="btnGuardar_Click"
                              ValidationGroup="AdminValidation">
                    <Image IconID="save_save_16x16" />
                </dx:ASPxButton>
                <dx:ASPxButton ID="btnCancelar" runat="server" Text="Cancelar" 
                              AutoPostBack="False">
                    <Image IconID="actions_cancel_16x16" />
                    <ClientSideEvents Click="function(s, e) { popupAdministrador.Hide(); }" />
                </dx:ASPxButton>
            </div>
        </FooterTemplate>
    </dx:ASPxPopupControl>
</asp:Content>

<!-- JAVASCRIPT SEPARADO - OBLIGATORIO -->
<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="~/Scripts/app/modules/administradores.js"></script>
</asp:Content>
```

##### Administradores.aspx.vb (Code-Behind)

```vb
Public Class Administradores
    Inherits BasePage
    
    Private Const API_URL As String = "https://jela-api-ctb8a6ggbpdqbxhg.mexicocentral-01.azurewebsites.net"
    
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ConfigurarGrid()
            CargarDatos()
        End If
    End Sub
    
    Private Sub ConfigurarGrid()
        ' Aplicar estándares de UI
        FuncionesGridWeb.ConfigurarGridEstandar(gridAdministradores)
        FuncionesGridWeb.ConfigurarToolbarCRUD(gridAdministradores, "Administrador")
    End Sub
    
    ''' <summary>
    ''' MÉTODO CRÍTICO: Genera columnas dinámicamente desde DataTable
    ''' OBLIGATORIO según estándares de UI
    ''' </summary>
    Private Sub GenerarColumnasDinamicas(grid As ASPxGridView, tabla As DataTable)
        Try
            If tabla Is Nothing OrElse tabla.Columns.Count = 0 Then Return
            
            ' Limpiar columnas previas (excepto columnas personalizadas)
            For i As Integer = grid.Columns.Count - 1 To 0 Step -1
                Dim col = grid.Columns(i)
                If Not TypeOf col Is GridViewCommandColumn Then
                    grid.Columns.RemoveAt(i)
                End If
            Next
            
            ' Crear columnas dinámicamente desde el DataTable
            For Each col As DataColumn In tabla.Columns
                Dim nombreColumna = col.ColumnName
                Dim gridCol As GridViewDataColumn = Nothing
                
                ' Crear columna según el tipo de dato
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
                
                ' FILTROS: Configurar según estándares - OBLIGATORIO
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
    
    Private Sub CargarDatos()
        Try
            ' Construir consulta SQL
            Dim query As String = "
                SELECT 
                    u.Id,
                    u.Usuario,
                    u.Email,
                    u.NombreCompleto,
                    u.Activo,
                    u.FechaCreacion,
                    l.LicenciasTotales,
                    l.LicenciasConsumidas,
                    (l.LicenciasTotales - l.LicenciasConsumidas) AS LicenciasDisponibles
                FROM conf_usuarios u
                LEFT JOIN conf_licencias l ON u.Id = l.IdAdministrador
                WHERE u.TipoUsuario = 'AdministradorCondominios'"
            
            ' Llamar a API CRUD dinámica
            Dim url As String = $"{API_URL}/api/crud?strQuery={Uri.EscapeDataString(query)}"
            Dim dt As DataTable = ApiConsumer.GetDataTable(url)
            
            ' Guardar en sesión para FuncionesGridWeb
            Session("dtAdministradores") = dt
            
            ' Generar columnas dinámicamente
            GenerarColumnasDinamicas(gridAdministradores, dt)
            
            ' Asignar datos
            gridAdministradores.DataSource = dt
            gridAdministradores.DataBind()
            
        Catch ex As Exception
            Logger.LogError("Administradores.CargarDatos", ex)
            MostrarError("Error al cargar administradores")
        End Try
    End Sub
    
    Protected Sub gridAdministradores_RowCommand(sender As Object, e As ASPxGridViewRowCommandEventArgs)
        Try
            If e.CommandArgs.CommandName = "New" Then
                LimpiarFormulario()
                popupAdministrador.HeaderText = "Nuevo Administrador"
                popupAdministrador.ShowOnPageLoad = True
                
            ElseIf e.CommandArgs.CommandName = "Edit" Then
                Dim id As Integer = CInt(e.KeyValue)
                CargarAdministrador(id)
                popupAdministrador.HeaderText = "Editar Administrador"
                popupAdministrador.ShowOnPageLoad = True
                
            ElseIf e.CommandArgs.CommandName = "Delete" Then
                Dim id As Integer = CInt(e.KeyValue)
                EliminarAdministrador(id)
            End If
            
        Catch ex As Exception
            Logger.LogError("Administradores.RowCommand", ex)
            MostrarError("Error al procesar la operación")
        End Try
    End Sub
    
    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs)
        Try
            ' Validar formulario
            If Not ASPxEdit.ValidateEditorsInContainer(popupAdministrador) Then
                Return
            End If
            
            Dim idAdministrador As Integer = If(ViewState("IdAdministrador"), 0)
            
            If idAdministrador = 0 Then
                ' Crear nuevo administrador
                CrearAdministrador()
            Else
                ' Actualizar administrador existente
                ActualizarAdministrador(idAdministrador)
            End If
            
            popupAdministrador.ShowOnPageLoad = False
            CargarDatos()
            MostrarExito("Operación completada exitosamente")
            
        Catch ex As Exception
            Logger.LogError("Administradores.btnGuardar_Click", ex)
            MostrarError("Error al guardar: " & ex.Message)
        End Try
    End Sub
    
    Private Sub CrearAdministrador()
        ' 1. Verificar unicidad de usuario y email
        If ExisteUsuario(txtUsuario.Text) Then
            Throw New Exception("El usuario ya existe")
        End If
        
        If ExisteEmail(txtEmail.Text) Then
            Throw New Exception("El email ya existe")
        End If
        
        ' 2. Crear usuario usando API CRUD dinámica
        Dim campos As New Dictionary(Of String, Object) From {
            {"Usuario", txtUsuario.Text},
            {"Email", txtEmail.Text},
            {"NombreCompleto", txtNombreCompleto.Text},
            {"TipoUsuario", "AdministradorCondominios"},
            {"Password", SecurityHelper.HashPassword(txtPassword.Text)},
            {"Activo", chkActivo.Checked}
        }
        
        Dim url As String = $"{API_URL}/api/crud/conf_usuarios"
        Dim response = ApiConsumer.Post(url, campos)
        Dim idUsuario As Integer = response("id")
        
        ' 3. Crear registro de licencias
        Dim camposLicencia As New Dictionary(Of String, Object) From {
            {"IdAdministrador", idUsuario},
            {"LicenciasTotales", CInt(txtLicenciasTotales.Value)},
            {"LicenciasConsumidas", 0}
        }
        
        url = $"{API_URL}/api/crud/conf_licencias"
        ApiConsumer.Post(url, camposLicencia)
        
        ' 4. Registrar en auditoría (llamar a servicio)
        ' TODO: Implementar llamada a AuditoriaService
    End Sub
    
    Private Sub ActualizarAdministrador(idAdministrador As Integer)
        ' Actualizar usando API CRUD dinámica
        Dim campos As New Dictionary(Of String, Object) From {
            {"Email", txtEmail.Text},
            {"NombreCompleto", txtNombreCompleto.Text},
            {"Activo", chkActivo.Checked}
        }
        
        Dim url As String = $"{API_URL}/api/crud/conf_usuarios/{idAdministrador}"
        ApiConsumer.Put(url, campos)
        
        ' Registrar en auditoría
        ' TODO: Implementar llamada a AuditoriaService
    End Sub
    
    Private Sub EliminarAdministrador(idAdministrador As Integer)
        ' 1. Verificar que no tenga entidades asignadas
        If TieneEntidadesAsignadas(idAdministrador) Then
            MostrarError("No se puede eliminar el administrador porque tiene entidades asignadas")
            Return
        End If
        
        ' 2. Eliminar usando API CRUD dinámica
        Dim url As String = $"{API_URL}/api/crud/conf_usuarios/{idAdministrador}"
        ApiConsumer.Delete(url)
        
        CargarDatos()
        MostrarExito("Administrador eliminado exitosamente")
    End Sub
    
    Private Function ExisteUsuario(usuario As String) As Boolean
        Dim query As String = $"SELECT COUNT(*) FROM conf_usuarios WHERE Usuario = '{usuario}'"
        Dim url As String = $"{API_URL}/api/crud?strQuery={Uri.EscapeDataString(query)}"
        Dim count As Integer = ApiConsumer.GetScalar(Of Integer)(url)
        Return count > 0
    End Function
    
    Private Function ExisteEmail(email As String) As Boolean
        Dim query As String = $"SELECT COUNT(*) FROM conf_usuarios WHERE Email = '{email}'"
        Dim url As String = $"{API_URL}/api/crud?strQuery={Uri.EscapeDataString(query)}"
        Dim count As Integer = ApiConsumer.GetScalar(Of Integer)(url)
        Return count > 0
    End Function
    
    Private Function TieneEntidadesAsignadas(idAdministrador As Integer) As Boolean
        Dim query As String = $"SELECT COUNT(*) FROM conf_usuario_entidades WHERE IdUsuario = {idAdministrador}"
        Dim url As String = $"{API_URL}/api/crud?strQuery={Uri.EscapeDataString(query)}"
        Dim count As Integer = ApiConsumer.GetScalar(Of Integer)(url)
        Return count > 0
    End Function
    
    Protected Sub gridAdministradores_DataBound(sender As Object, e As EventArgs) Handles gridAdministradores.DataBound
        Try
            Dim tabla As DataTable = TryCast(Session("dtAdministradores"), DataTable)
            If tabla IsNot Nothing Then
                FuncionesGridWeb.SUMColumn(gridAdministradores, tabla)
            End If
        Catch ex As Exception
            Logger.LogError("Administradores.gridAdministradores_DataBound", ex)
        End Try
    End Sub
End Class
```

## Resumen de Cambios

### Antes (Diseño Incorrecto)
- 7 controladores nuevos
- 15+ DTOs fijos
- 5 repositorios
- 6 servicios con métodos CRUD

### Después (Diseño Correcto)
- 0 controladores nuevos (usar `/api/crud` existente)
- 0 DTOs fijos (usar `CrudDto` dinámico)
- 0 repositorios (usar `MySqlDatabaseService` existente)
- 4 servicios solo para lógica compleja

### Beneficios del Enfoque Dinámico

1. **Menos código**: No hay que mantener DTOs ni controladores para cada entidad
2. **Más flexible**: Agregar campos a tablas no requiere cambios en código
3. **Consistente**: Todas las operaciones CRUD usan el mismo patrón
4. **Reutilizable**: El mismo código funciona para cualquier tabla
5. **Mantenible**: Cambios en esquema de BD no rompen el código

### Tareas Actualizadas

Las tareas deben actualizarse para reflejar este enfoque:

- ❌ Eliminar: Tareas 2 (DTOs), 3 (Repositorios), 9 (Controladores)
- ✅ Mantener: Tarea 1 (Tablas), Tareas de WebForms
- ✅ Simplificar: Tareas de servicios (solo 4 servicios en lugar de 6)
- ✅ Agregar: Tareas para implementar llamadas a `/api/crud` desde WebForms

## Próximos Pasos

1. Actualizar `design.md` para reflejar el enfoque dinámico
2. Actualizar `tasks.md` para eliminar tareas innecesarias
3. Confirmar con el usuario que el enfoque es correcto
4. Proceder con la implementación usando API CRUD dinámica


##### administradores.css (CSS Separado - OBLIGATORIO)

```css
/* 
 * ESTÁNDAR CRÍTICO: Color de fondo #E4EFFA
 * Todos los módulos DEBEN usar este color
 */
.page-wrapper {
    min-height: 100vh;
    background: #E4EFFA;
    padding: 20px;
}

/* Contenedor del grid */
.grid-container {
    background-color: white;
    padding: 15px;
    border-radius: 5px;
    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    margin-bottom: 20px;
}

/* Header de página */
.page-header {
    margin-bottom: 20px;
    padding-bottom: 10px;
    border-bottom: 2px solid #007bff;
}

.page-header h1 {
    font-size: 24px;
    font-weight: 600;
    color: #333;
    margin-bottom: 5px;
}

.page-header .text-muted {
    font-size: 14px;
    color: #6c757d;
}

/* Formularios en popup */
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
    color: #333;
}

.required {
    color: #dc3545;
    font-weight: bold;
}

/* Footer del popup */
.popup-footer {
    text-align: right;
    padding: 10px;
    background-color: #f8f9fa;
}

.popup-footer .dxbButton {
    margin-left: 10px;
}

/* Badges de estado */
.status-badge {
    padding: 5px 10px;
    border-radius: 3px;
    font-size: 12px;
    font-weight: 600;
    display: inline-block;
}

.status-activo {
    background-color: #28a745;
    color: white;
}

.status-inactivo {
    background-color: #6c757d;
    color: white;
}

/* Alertas de licencias */
.alerta-critica {
    background-color: #dc3545;
    color: white;
    padding: 5px 10px;
    border-radius: 3px;
    font-size: 12px;
    font-weight: 600;
}

.alerta-advertencia {
    background-color: #ffc107;
    color: #212529;
    padding: 5px 10px;
    border-radius: 3px;
    font-size: 12px;
    font-weight: 600;
}
```

##### administradores.js (JavaScript Separado - OBLIGATORIO)

```javascript
// Namespace del módulo - OBLIGATORIO según estándares
var AdministradoresModule = (function() {
    'use strict';
    
    // Variables privadas
    var _gridInstance = null;
    var _popupInstance = null;
    
    // Inicialización
    function init() {
        _gridInstance = gridAdministradores;
        _popupInstance = popupAdministrador;
        
        bindEvents();
        configurarValidaciones();
    }
    
    // Event handlers
    function bindEvents() {
        // Eventos del grid
        if (_gridInstance) {
            _gridInstance.RowClick.AddHandler(onGridRowClick);
        }
        
        // Eventos de botones personalizados si existen
        // $('#btnCustom').on('click', customAction);
    }
    
    // Configurar validaciones adicionales
    function configurarValidaciones() {
        // Validaciones personalizadas si se necesitan
    }
    
    // Funciones públicas
    function mostrarNuevo() {
        limpiarFormulario();
        _popupInstance.SetHeaderText('Nuevo Administrador');
        _popupInstance.Show();
    }
    
    function mostrarEditar(adminId) {
        // Cargar datos del administrador
        _popupInstance.SetHeaderText('Editar Administrador');
        _popupInstance.Show();
    }
    
    // Funciones privadas
    function onGridRowClick(s, e) {
        // Lógica del evento si se necesita
    }
    
    function limpiarFormulario() {
        if (typeof txtUsuario !== 'undefined') txtUsuario.SetValue('');
        if (typeof txtEmail !== 'undefined') txtEmail.SetValue('');
        if (typeof txtNombreCompleto !== 'undefined') txtNombreCompleto.SetValue('');
        if (typeof txtLicenciasTotales !== 'undefined') txtLicenciasTotales.SetValue(1);
        if (typeof chkActivo !== 'undefined') chkActivo.SetChecked(true);
    }
    
    // API pública del módulo
    return {
        init: init,
        mostrarNuevo: mostrarNuevo,
        mostrarEditar: mostrarEditar
    };
})();

// Inicializar cuando el DOM esté listo
$(document).ready(function() {
    AdministradoresModule.init();
});
```

## Checklist de Estándares de UI

Antes de considerar completa cualquier página, verificar:

### ✅ Estructura de Archivos
- [ ] CSS en archivo separado en `/Content/css/modules/`
- [ ] JavaScript en archivo separado en `/Scripts/app/modules/`
- [ ] NO hay `<style>` inline en el ASPX
- [ ] NO hay `<script>` inline en el ASPX

### ✅ Color de Fondo
- [ ] Página usa `background: #E4EFFA`
- [ ] Contenedores internos usan `background: white` para contraste

### ✅ Grids
- [ ] `SettingsPager Mode="ShowAllRecords"` (sin paginación)
- [ ] `Settings ShowFilterRow="False"` (filtros en headers, no en fila)
- [ ] `Settings ShowFilterRowMenu="True"` (filtros tipo Excel)
- [ ] `Settings ShowGroupPanel="True"` (permitir agrupación)
- [ ] `SettingsSearchPanel Visible="True"` (barra de búsqueda global)
- [ ] Columnas generadas dinámicamente con `GenerarColumnasDinamicas()`
- [ ] Evento `DataBound` implementado con `FuncionesGridWeb.SUMColumn`
- [ ] Acciones CRUD en toolbar, NO como botones externos
- [ ] Solo filtros de fecha arriba del grid (si aplica)

### ✅ Nomenclatura
- [ ] Botones usan nombres contextuales ("Nuevo Administrador" NO "Nuevo")
- [ ] Campos de BD usan PascalCase (`IdAdministrador`, `FechaCreacion`)
- [ ] Tablas usan prefijo con guion bajo (`conf_usuarios`, `op_tickets`)

### ✅ Popups
- [ ] Usar `ASPxPopupControl` para captura de datos
- [ ] NO abrir otras páginas para edición
- [ ] Validadores DevExpress dentro del popup
- [ ] `ValidationGroup` configurado correctamente

### ✅ Validación
- [ ] `ASPxValidationSummary` en el popup
- [ ] Campos requeridos con `RequiredField IsRequired="True"`
- [ ] Validación de email con `RegularExpression`
- [ ] Validación de rangos con `Range` o `MinValue/MaxValue`

### ✅ API Dinámica
- [ ] Usar `/api/crud?strQuery=SELECT...` para consultas
- [ ] Usar `/api/crud/{tabla}` para POST/PUT/DELETE
- [ ] NO crear controladores fijos
- [ ] NO crear DTOs fijos

## Resumen Final

### ✅ Estándares de UI Aplicados

El documento de corrección ahora incluye:

1. **Color de fondo obligatorio** `#E4EFFA`
2. **CSS y JavaScript separados** (con ejemplos completos)
3. **Nomenclatura contextual** en todos los botones
4. **Grids sin paginación** (`ShowAllRecords`)
5. **Filtros solo en columnas** (no arriba del grid, excepto fechas)
6. **Columnas dinámicas** con método `GenerarColumnasDinamicas()`
7. **Toolbar con acciones CRUD** (no botones externos)
8. **Evento DataBound** con `FuncionesGridWeb.SUMColumn`
9. **Popups para captura** (no páginas separadas)
10. **Validadores DevExpress** en popups

### ✅ API Dinámica Aplicada

1. **NO crear controladores fijos** (usar `/api/crud`)
2. **NO crear DTOs fijos** (usar `CrudDto` dinámico)
3. **Consultas SQL directas** en WebForms
4. **Solo 4 servicios** para lógica compleja

### ✅ Archivos de Ejemplo Completos

- `Administradores.aspx` (markup completo con estándares)
- `Administradores.aspx.vb` (code-behind con API dinámica)
- `administradores.css` (CSS separado con color #E4EFFA)
- `administradores.js` (JavaScript separado con namespace)

El diseño está ahora **100% alineado** con los estándares de UI del ecosistema JELABBC y usa la API CRUD dinámica existente.
