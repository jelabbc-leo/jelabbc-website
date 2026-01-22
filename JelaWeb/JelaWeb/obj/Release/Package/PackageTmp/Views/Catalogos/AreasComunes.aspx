<%@ Page Title="Áreas Comunes" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="AreasComunes.aspx.vb" Inherits="JelaWeb.AreasComunes" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/areas-comunes.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Catalogos/areas-comunes.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">

        <!-- GRID PRINCIPAL -->
        <dx:ASPxGridView ID="gridAreas" runat="server" ClientInstanceName="gridAreas"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridAreas_DataBound">
            
            <ClientSideEvents ToolbarItemClick="onToolbarAreasClick" RowDblClick="onRowDblClick" />
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="AreasComunes" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevo" Text="Nueva Área">
                            <Image IconID="actions_add_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEditar" Text="Editar Área">
                            <Image IconID="edit_edit_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEliminar" Text="Eliminar Área" BeginGroup="True">
                            <Image IconID="actions_trash_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            
            <Columns>
                <%-- Columnas generadas dinámicamente desde el API - Solo GridViewCommandColumn - las demás columnas se generan dinámicamente --%>
            </Columns>
            
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridAreas" />

    </div>

    <!-- POPUP: NUEVO/EDITAR ÁREA -->
    <dx:ASPxPopupControl ID="popupArea" runat="server" ClientInstanceName="popupArea"
        HeaderText="Nueva Área Común" Width="800px" Height="600px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPageControl ID="tabsArea" runat="server" ClientInstanceName="tabsArea" 
                    Width="100%" Theme="Office2010Blue" ActiveTabIndex="1">
                    <%-- TAB: DATOS GENERALES --%>
                    <TabPages>
                        <dx:TabPage Text="Datos Generales" Name="tabDatosGenerales">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formDatosGenerales" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <!-- Campo Entidad eliminado - El sistema usa IdEntidadActual automáticamente -->
                                            <dx:LayoutItem Caption="Sub-Entidad">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbSubEntidad" runat="server" ClientInstanceName="cmbSubEntidad"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.Int32"
                                                            NullText="(Compartida - Toda la entidad)" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Clave" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtClave" runat="server" ClientInstanceName="txtClave"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="20">
                                                            <ValidationSettings><RequiredField IsRequired="True" ErrorText="La clave es requerida" /></ValidationSettings>
                                                        </dx:ASPxTextBox>
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
                                            <dx:LayoutItem Caption="Tipo Área">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbTipoArea" runat="server" ClientInstanceName="cmbTipoArea"
                                                            Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                                            <Items>
                                                                <dx:ListEditItem Text="Salón" Value="Salon" Selected="True" />
                                                                <dx:ListEditItem Text="Alberca" Value="Alberca" />
                                                                <dx:ListEditItem Text="Gimnasio" Value="Gimnasio" />
                                                                <dx:ListEditItem Text="Jardín" Value="Jardin" />
                                                                <dx:ListEditItem Text="Terraza" Value="Terraza" />
                                                                <dx:ListEditItem Text="Asador" Value="Asador" />
                                                                <dx:ListEditItem Text="Ludoteca" Value="Ludoteca" />
                                                                <dx:ListEditItem Text="Sala Juntas" Value="SalaJuntas" />
                                                                <dx:ListEditItem Text="Estacionamiento" Value="Estacionamiento" />
                                                                <dx:ListEditItem Text="Cancha" Value="Cancha" />
                                                                <dx:ListEditItem Text="Otro" Value="Otro" />
                                                            </Items>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Capacidad">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnCapacidad" runat="server" ClientInstanceName="spnCapacidad"
                                                            Width="100%" Theme="Office2010Blue" NumberType="Integer"
                                                            MinValue="0" MaxValue="9999" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Ubicación" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtUbicacion" runat="server" ClientInstanceName="txtUbicacion"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="200" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Descripción" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxMemo ID="txtDescripcion" runat="server" ClientInstanceName="txtDescripcion"
                                                            Width="100%" Height="60px" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Activo">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkActivo" runat="server" ClientInstanceName="chkActivo"
                                                            Text="Área activa" Checked="True" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                        <%-- TAB: RESERVACIÓN Y COSTOS --%>
                        <dx:TabPage Text="Reservación y Costos" Name="tabReservacion">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formReservacion" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutItem Caption="Requiere Reservación" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkRequiereReservacion" runat="server" ClientInstanceName="chkRequiereReservacion"
                                                            Text="Requiere reservación previa" Checked="True" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Costo Reservación">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnCostoReservacion" runat="server" ClientInstanceName="spnCostoReservacion"
                                                            Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                                            MinValue="0" MaxValue="999999.99" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Depósito Requerido">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnDeposito" runat="server" ClientInstanceName="spnDeposito"
                                                            Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2"
                                                            MinValue="0" MaxValue="999999.99" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutGroup Caption="Horarios" ColSpan="2" ColCount="2">
                                                <Items>
                                                    <dx:LayoutItem Caption="Hora Apertura">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTimeEdit ID="tmeHoraApertura" runat="server" ClientInstanceName="tmeHoraApertura"
                                                                    Width="100%" Theme="Office2010Blue" DisplayFormatString="HH:mm" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Hora Cierre">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxTimeEdit ID="tmeHoraCierre" runat="server" ClientInstanceName="tmeHoraCierre"
                                                                    Width="100%" Theme="Office2010Blue" DisplayFormatString="HH:mm" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Duración de Reservación" ColSpan="2" ColCount="2">
                                                <Items>
                                                    <dx:LayoutItem Caption="Mínima (horas)">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxSpinEdit ID="spnDuracionMinima" runat="server" ClientInstanceName="spnDuracionMinima"
                                                                    Width="100%" Theme="Office2010Blue" NumberType="Integer"
                                                                    MinValue="1" MaxValue="24" Number="2" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Máxima (horas)">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxSpinEdit ID="spnDuracionMaxima" runat="server" ClientInstanceName="spnDuracionMaxima"
                                                                    Width="100%" Theme="Office2010Blue" NumberType="Integer"
                                                                    MinValue="1" MaxValue="24" Number="8" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutGroup Caption="Anticipación" ColSpan="2" ColCount="2">
                                                <Items>
                                                    <dx:LayoutItem Caption="Mínima (días)">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxSpinEdit ID="spnAnticipacionMinima" runat="server" ClientInstanceName="spnAnticipacionMinima"
                                                                    Width="100%" Theme="Office2010Blue" NumberType="Integer"
                                                                    MinValue="0" MaxValue="365" Number="1" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                    <dx:LayoutItem Caption="Máxima (días)">
                                                        <LayoutItemNestedControlCollection>
                                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                                <dx:ASPxSpinEdit ID="spnAnticipacionMaxima" runat="server" ClientInstanceName="spnAnticipacionMaxima"
                                                                    Width="100%" Theme="Office2010Blue" NumberType="Integer"
                                                                    MinValue="1" MaxValue="365" Number="30" />
                                                            </dx:LayoutItemNestedControlContainer>
                                                        </LayoutItemNestedControlCollection>
                                                    </dx:LayoutItem>
                                                </Items>
                                            </dx:LayoutGroup>
                                            <dx:LayoutItem Caption="Días Disponibles" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtDiasDisponibles" runat="server" ClientInstanceName="txtDiasDisponibles"
                                                            Width="100%" Theme="Office2010Blue" MaxLength="20" Text="L,M,X,J,V,S,D"
                                                            NullText="L,M,X,J,V,S,D" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
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
                        <ClientSideEvents Click="function(s, e) { popupArea.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>
    
    <!-- HIDDEN FIELDS -->
    <asp:HiddenField ID="hfAreaId" runat="server" Value="0" />

</asp:Content>
