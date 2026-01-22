<%@ Page Title="Residentes" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Residentes.aspx.vb" Inherits="JelaWeb.Residentes" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/residentes.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Catalogos/residentes.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">

        <dx:ASPxGridView ID="gridResidentes" runat="server" ClientInstanceName="gridResidentes"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridResidentes_DataBound">
            
            <ClientSideEvents ToolbarItemClick="onToolbarResidentesClick" RowDblClick="onRowDblClick" />
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="Residentes" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevo" Text="Nuevo Residente">
                            <Image IconID="actions_add_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEditar" Text="Editar Residente">
                            <Image IconID="edit_edit_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEliminar" Text="Eliminar Residente" BeginGroup="True">
                            <Image IconID="actions_trash_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            
            <!-- Columnas generadas dinámicamente desde el API -->
            <Columns>
                <!-- Solo GridViewCommandColumn - las demás columnas se generan dinámicamente -->
            </Columns>
            
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridResidentes" />

    </div>

    <dx:ASPxPopupControl ID="popupResidente" runat="server" ClientInstanceName="popupResidente"
        HeaderText="Nuevo Residente" Width="900px" Height="600px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPageControl ID="tabsResidente" runat="server" ClientInstanceName="tabsResidente" 
                    Width="100%" Theme="Office2010Blue" ActiveTabIndex="2">
                    <TabPages>
                        
                        <dx:TabPage Text="Datos Personales" Name="tabDatosPersonales">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formDatosPersonales" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
                                            <dx:LayoutItem Caption="Sub-Entidad">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbSubEntidad" runat="server" ClientInstanceName="cmbSubEntidad"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                                            <ClientSideEvents SelectedIndexChanged="onSubEntidadChanged" />
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Unidad" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbUnidad" runat="server" ClientInstanceName="cmbUnidad"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="Seleccione una unidad" /></ValidationSettings>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Tipo Residente" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbTipoResidente" runat="server" ClientInstanceName="cmbTipoResidente"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                            <Items>
                                                                <dx:ListEditItem Text="Propietario" Value="Propietario" Selected="True" />
                                                                <dx:ListEditItem Text="Inquilino" Value="Inquilino" />
                                                                <dx:ListEditItem Text="Familiar" Value="Familiar" />
                                                                <dx:ListEditItem Text="Empleado" Value="Empleado" />
                                                            </Items>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Nombre" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtNombre" runat="server" ClientInstanceName="txtNombre"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="100">
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="El nombre es requerido" /></ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Apellido Paterno" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtApellidoPaterno" runat="server" ClientInstanceName="txtApellidoPaterno"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="100">
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="El apellido paterno es requerido" /></ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Apellido Materno">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtApellidoMaterno" runat="server" ClientInstanceName="txtApellidoMaterno"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="100" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Es Principal">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkEsPrincipal" runat="server" ClientInstanceName="chkEsPrincipal"
                                                            Text="Responsable principal de la unidad" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Fecha Ingreso">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxDateEdit ID="dteFechaIngreso" runat="server" ClientInstanceName="dteFechaIngreso"
                                                            Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Activo">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkActivo" runat="server" ClientInstanceName="chkActivo"
                                                            Text="Residente activo" Checked="True" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        
                        <dx:TabPage Text="Contacto" Name="tabContacto">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formContacto" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutItem Caption="Email">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtEmail" runat="server" ClientInstanceName="txtEmail"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="200">
                                                            <ValidationSettings><RegularExpression ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ErrorText="Email inválido" /></ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Teléfono">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtTelefono" runat="server" ClientInstanceName="txtTelefono"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="20" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Celular">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtCelular" runat="server" ClientInstanceName="txtCelular"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="20" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Tel. Emergencia">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtTelefonoEmergencia" runat="server" ClientInstanceName="txtTelefonoEmergencia"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="20" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutGroup Caption="Telegram" ColSpan="2" ColCount="2">
                                                <Items>
                                                    <dx:LayoutItem Caption="Chat ID">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtTelegramChatId" runat="server" ClientInstanceName="txtTelegramChatId"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Username">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtTelegramUsername" runat="server" ClientInstanceName="txtTelegramUsername"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="100" NullText="@usuario" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Telegram Activo" ColSpan="2">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxCheckBox ID="chkTelegramActivo" runat="server" ClientInstanceName="chkTelegramActivo"
                                                                    Text="Recibir notificaciones por Telegram" Theme="Office2010Blue" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Preferencias de Notificación" ColSpan="2" ColCount="3">
                                                <Items>
                                                    <dx:LayoutItem ShowCaption="False">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxCheckBox ID="chkNotifEmail" runat="server" ClientInstanceName="chkNotifEmail"
                                                                    Text="Email" Checked="True" Theme="Office2010Blue" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem ShowCaption="False">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxCheckBox ID="chkNotifTelegram" runat="server" ClientInstanceName="chkNotifTelegram"
                                                                    Text="Telegram" Checked="True" Theme="Office2010Blue" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem ShowCaption="False">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxCheckBox ID="chkNotifPush" runat="server" ClientInstanceName="chkNotifPush"
                                                                    Text="Push" Checked="True" Theme="Office2010Blue" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        
                        <dx:TabPage Text="Identificación" Name="tabIdentificacion">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <!-- SECCIÓN: ESCANEO DE INE -->
                                    <dx:ASPxFormLayout ID="formEscaneoINE" runat="server" Width="100%" ColCount="1" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutGroup Caption="Escaneo Automático de INE" ColSpan="1">
                                                <Items>
                                                    <dx:LayoutItem ShowCaption="False">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <div class="ine-scanner-container">
                                                                    <div class="ine-upload-area" id="ineDropZone">
                                                                        <div class="ine-upload-icon">
                                                                            <i class="fas fa-id-card"></i>
                                                                        </div>
                                                                        <p class="ine-upload-text">Arrastra una imagen de INE aquí o haz clic para seleccionar</p>
                                                                        <p class="ine-upload-hint">Formatos: JPG, PNG, PDF (máx. 10MB)</p>
                                                                        <input type="file" id="ineFileInput" accept="image/*,.pdf" style="display:none;" />
                                                                    </div>
                                                                    <div class="ine-preview-container" id="inePreviewContainer" style="display:none;">
                                                                        <img id="inePreviewImage" src="" alt="Vista previa INE" />
                                                                        <button type="button" class="ine-remove-btn" onclick="removeINEImage()">
                                                                            <i class="fas fa-times"></i>
                                                                        </button>
                                                                    </div>
                                                                    <div class="ine-actions">
                                                                        <dx:ASPxButton ID="btnEscanearINE" runat="server" Text="Escanear INE" 
                                                                            Theme="Office2010Blue" AutoPostBack="False" ClientInstanceName="btnEscanearINE">
                                                                            <Image IconID="businessobjects_boidentificationcard_16x16" />
                                                                            <ClientSideEvents Click="function(s, e) { escanearINE(); }" />
                                                                        </dx:ASPxButton>
                                                                        <dx:ASPxLoadingPanel ID="loadingINE" runat="server" ClientInstanceName="loadingINE"
                                                                            Modal="True" Text="Procesando INE con IA..." />
                                                                    </div>
                                                                    <div class="ine-status" id="ineStatus"></div>
                                                                </div>
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                    
                                    <!-- SECCIÓN: DATOS DE IDENTIFICACIÓN -->
                                    <dx:ASPxFormLayout ID="formIdentificacion" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutItem Caption="Tipo Identificación">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbTipoIdentificacion" runat="server" ClientInstanceName="cmbTipoIdentificacion"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                            <Items>
                                                                <dx:ListEditItem Text="INE" Value="INE" Selected="True" />
                                                                <dx:ListEditItem Text="Pasaporte" Value="Pasaporte" />
                                                                <dx:ListEditItem Text="Licencia" Value="Licencia" />
                                                                <dx:ListEditItem Text="Cédula" Value="Cedula" />
                                                                <dx:ListEditItem Text="Otro" Value="Otro" />
                                                            </Items>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="No. Identificación">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtNumeroIdentificacion" runat="server" ClientInstanceName="txtNumeroIdentificacion"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="50" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="RFC">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtRFC" runat="server" ClientInstanceName="txtRFC"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="13" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="CURP">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtCURP" runat="server" ClientInstanceName="txtCURP"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="18" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Clave Elector">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtClaveElector" runat="server" ClientInstanceName="txtClaveElector"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="18" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Vigencia INE">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtVigenciaINE" runat="server" ClientInstanceName="txtVigenciaINE"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="10" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutGroup Caption="Vehículo" ColSpan="2" ColCount="2">
                                                <Items>
                                                    <dx:LayoutItem Caption="Tiene Vehículo" ColSpan="2">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxCheckBox ID="chkTieneVehiculo" runat="server" ClientInstanceName="chkTieneVehiculo"
                                                                    Text="El residente tiene vehículo" Theme="Office2010Blue" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Placas">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTextBox ID="txtPlacas" runat="server" ClientInstanceName="txtPlacas"
                                                                    Width="100%" Theme="Office2010Blue" MaxLength="20" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        
                    </TabPages>
                </dx:ASPxPageControl>
                
                <div class="popup-footer" style="margin-top: 15px; text-align: right;">
                    <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Theme="Office2010Blue" 
                        AutoPostBack="True" OnClick="btnGuardar_Click">
                        <Image IconID="save_save_16x16" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCancelar" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="actions_cancel_16x16" />
                        <ClientSideEvents Click="function(s, e) { popupResidente.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    
    <asp:HiddenField ID="hfResidenteId" runat="server" Value="0" />
    <asp:Label ID="lblMensaje" runat="server" Visible="False" />

</asp:Content>
