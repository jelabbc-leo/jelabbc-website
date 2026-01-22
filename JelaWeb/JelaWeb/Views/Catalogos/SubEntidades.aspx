<%@ Page Title="Sub Entidades" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="SubEntidades.aspx.vb" Inherits="JelaWeb.SubEntidades" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/modules/sub-entidades.css") %>" rel="stylesheet" type="text/css" />
    <script src='<%= ResolveUrl("~/Scripts/app/Catalogos/sub-entidades.js") %>?v=20260121' type="text/javascript"></script>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container-fluid">
        <div class="subentidades-grid-container">
            <dx:ASPxGridView ID="gridSubEntidades" runat="server" ClientInstanceName="gridSubEntidades"
                KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
                OnDataBound="gridSubEntidades_DataBound">

                <ClientSideEvents ToolbarItemClick="onToolbarSubEntidadesClick" RowDblClick="onRowDblClickSubEntidades" />

                <SettingsPager Mode="ShowAllRecords" />
                <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
                <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
                <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="SubEntidades" />

                <Toolbars>
                    <dx:GridViewToolbar>
                        <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                        <Items>
                            <dx:GridViewToolbarItem Name="btnNuevo" Text="Nueva Sub Entidad">
                                <Image IconID="actions_add_16x16" />
                            </dx:GridViewToolbarItem>
                            <dx:GridViewToolbarItem Name="btnEditar" Text="Editar Sub Entidad">
                                <Image IconID="edit_edit_16x16" />
                            </dx:GridViewToolbarItem>
                            <dx:GridViewToolbarItem Name="btnEliminar" Text="Eliminar Sub Entidad" BeginGroup="True">
                                <Image IconID="actions_trash_16x16" />
                            </dx:GridViewToolbarItem>
                            <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                            <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                        </Items>
                    </dx:GridViewToolbar>
                </Toolbars>

                <Columns>
                    <%-- Columnas generadas dinámicamente desde el API --%>
                </Columns>
            </dx:ASPxGridView>
            <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridSubEntidades" />
        </div>
    </div>

    <dx:ASPxPopupControl ID="popupSubEntidad" runat="server" ClientInstanceName="popupSubEntidad"
        HeaderText="Nueva Sub Entidad" Width="1100px" Modal="True"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True"
        CssClass="subentidades-popup">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxPageControl ID="tabsSubEntidad" runat="server" ClientInstanceName="tabsSubEntidad"
                    Width="100%" Theme="Office2010Blue" ActiveTabIndex="1">
                    <TabPages>
                        <dx:TabPage Text="Datos Generales" Name="tabGenerales">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formGenerales" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutItem Caption="Clave" RequiredMarkDisplayMode="Required">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtClave" runat="server" ClientInstanceName="txtClave" Width="100%" MaxLength="50">
                                                            <ValidationSettings>
                                                                <RequiredField IsRequired="True" ErrorText="La clave es obligatoria" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Alias">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtAlias" runat="server" ClientInstanceName="txtAlias" Width="100%" MaxLength="50" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="CIF">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtCIF" runat="server" ClientInstanceName="txtCIF" Width="100%" MaxLength="20" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="RFC">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtRFC" runat="server" ClientInstanceName="txtRFC" Width="100%" MaxLength="20" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Razón Social" RequiredMarkDisplayMode="Required" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtRazonSocial" runat="server" ClientInstanceName="txtRazonSocial" Width="100%" MaxLength="255">
                                                            <ValidationSettings>
                                                                <RequiredField IsRequired="True" ErrorText="La razón social es obligatoria" />
                                                            </ValidationSettings>
                                                        </dx:ASPxTextBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Fecha Alta">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxDateEdit ID="dateFechaAlta" runat="server" ClientInstanceName="dateFechaAlta" Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Activo">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkActivo" runat="server" ClientInstanceName="chkActivo" Checked="True" Theme="Office2010Blue" />
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
                                            <dx:LayoutItem Caption="Teléfonos">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtTelefonos" runat="server" ClientInstanceName="txtTelefonos" Width="100%" MaxLength="50" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="WhatsApp">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtWhatsapp" runat="server" ClientInstanceName="txtWhatsapp" Width="100%" MaxLength="20" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Correo">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtMail" runat="server" ClientInstanceName="txtMail" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Administrador" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtAdministrador" runat="server" ClientInstanceName="txtAdministrador" Width="100%" MaxLength="200" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Teléfono Vigilancia">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtTelefonoVigilancia" runat="server" ClientInstanceName="txtTelefonoVigilancia" Width="100%" MaxLength="20" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>

                        <dx:TabPage Text="Dirección" Name="tabDireccion">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formDireccion" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutItem Caption="CP">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtCP" runat="server" ClientInstanceName="txtCP" Width="100%" MaxLength="10" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Tipo Vialidad">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtTipoVialidad" runat="server" ClientInstanceName="txtTipoVialidad" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Nombre Vialidad" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtNombreVialidad" runat="server" ClientInstanceName="txtNombreVialidad" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="No. Exterior">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtNoExterior" runat="server" ClientInstanceName="txtNoExterior" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="No. Interior">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtNoInterior" runat="server" ClientInstanceName="txtNoInterior" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Colonia" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtColonia" runat="server" ClientInstanceName="txtColonia" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Localidad">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtLocalidad" runat="server" ClientInstanceName="txtLocalidad" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Municipio">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtMunicipio" runat="server" ClientInstanceName="txtMunicipio" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Entidad Federativa" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtEntidadFederativa" runat="server" ClientInstanceName="txtEntidadFederativa" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Entre Calle" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxTextBox ID="txtEntreCalle" runat="server" ClientInstanceName="txtEntreCalle" Width="100%" MaxLength="255" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>

                        <dx:TabPage Text="Sección" Name="tabSeccion">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formSeccion" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutItem Caption="Es Sección de Condominio" ColSpan="2">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkEsSeccionCondominio" runat="server" ClientInstanceName="chkEsSeccionCondominio" Theme="Office2010Blue">
                                                            <ClientSideEvents CheckedChanged="onEsSeccionChanged" />
                                                        </dx:ASPxCheckBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Tipo Sección">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxComboBox ID="cmbTipoSeccion" runat="server" ClientInstanceName="cmbTipoSeccion" Width="100%" Theme="Office2010Blue">
                                                            <Items>
                                                                <dx:ListEditItem Text="Torre" Value="Torre" />
                                                                <dx:ListEditItem Text="Sección" Value="Seccion" />
                                                                <dx:ListEditItem Text="Edificio" Value="Edificio" />
                                                                <dx:ListEditItem Text="Cluster" Value="Cluster" />
                                                                <dx:ListEditItem Text="Privada" Value="Privada" />
                                                                <dx:ListEditItem Text="Otro" Value="Otro" />
                                                            </Items>
                                                        </dx:ASPxComboBox>
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Número de Niveles">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnNumeroNiveles" runat="server" ClientInstanceName="spnNumeroNiveles" Width="100%" NumberType="Integer" MinValue="0" MaxValue="999" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Número de Unidades">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnNumeroUnidades" runat="server" ClientInstanceName="spnNumeroUnidades" Width="100%" NumberType="Integer" MinValue="0" MaxValue="100000" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Tiene Elevador">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkTieneElevador" runat="server" ClientInstanceName="chkTieneElevador" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Número de Elevadores">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnNumeroElevadores" runat="server" ClientInstanceName="spnNumeroElevadores" Width="100%" NumberType="Integer" MinValue="0" MaxValue="100" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Tiene Estacionamiento">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxCheckBox ID="chkTieneEstacionamiento" runat="server" ClientInstanceName="chkTieneEstacionamiento" Theme="Office2010Blue" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Cajones de Estacionamiento">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnCajonesEstacionamiento" runat="server" ClientInstanceName="spnCajonesEstacionamiento" Width="100%" NumberType="Integer" MinValue="0" MaxValue="100000" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                        </Items>
                                    </dx:ASPxFormLayout>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>

                        <dx:TabPage Text="Ubicación" Name="tabUbicacion">
                            <ContentCollection>
                                <dx:ContentControl runat="server">
                                    <dx:ASPxFormLayout ID="formUbicacion" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                                        <Items>
                                            <dx:LayoutItem Caption="Latitud">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnLatitud" runat="server" ClientInstanceName="spnLatitud" Width="100%" NumberType="Float" DecimalPlaces="6" MinValue="-90" MaxValue="90" />
                                                    </dx:LayoutItemNestedControlContainer>
                                                </LayoutItemNestedControlCollection>
                                            </dx:LayoutItem>
                                            <dx:LayoutItem Caption="Longitud">
                                                <LayoutItemNestedControlCollection>
                                                    <dx:LayoutItemNestedControlContainer runat="server">
                                                        <dx:ASPxSpinEdit ID="spnLongitud" runat="server" ClientInstanceName="spnLongitud" Width="100%" NumberType="Float" DecimalPlaces="6" MinValue="-180" MaxValue="180" />
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

                <div class="subentidades-popup-footer">
                    <dx:ASPxButton ID="btnGuardarSubEntidad" runat="server" Text="Guardar" Theme="Office2010Blue" OnClick="btnGuardarSubEntidad_Click">
                        <Image IconID="save_save_16x16" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrarSubEntidad" runat="server" Text="Cerrar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="actions_cancel_16x16" />
                        <ClientSideEvents Click="onCerrarSubEntidadClick" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <asp:HiddenField ID="hfSubEntidadId" runat="server" Value="0" ClientIDMode="Static" />
</asp:Content>
