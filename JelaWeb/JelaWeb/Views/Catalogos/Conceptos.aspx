<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="Conceptos.aspx.vb" Inherits="JelaWeb.Conceptos" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/conceptos.css") %>" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!-- Grid principal ASPxGridView -->
    <dx:ASPxGridView ID="grdCatConceptos" runat="server" ClientInstanceName="grdCatConceptos" KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False">
        
        <SettingsPager Mode="ShowAllRecords" />
        <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
        <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFixedGroups="True" />
        <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
        
        <SettingsCommandButton>
            <NewButton Text="Nuevo Concepto">
                <Image IconID="actions_add_16x16" />
            </NewButton>
            <EditButton Text="Editar Concepto">
                <Image IconID="edit_edit_16x16" />
            </EditButton>
            <DeleteButton Text="Eliminar Concepto">
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
                <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                <Items>
                    <dx:GridViewToolbarItem Command="New" />
                    <dx:GridViewToolbarItem Command="Edit" />
                    <dx:GridViewToolbarItem Command="Delete" />
                    <dx:GridViewToolbarItem BeginGroup="true" Command="Refresh" />
                    <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                </Items>
            </dx:GridViewToolbar>
        </Toolbars>

        <ClientSideEvents ToolbarItemClick="onToolbarClick" />
        
        <SettingsResizing ColumnResizeMode="NextColumn" />
        <SettingsPopup>
            <FilterControl AutoUpdatePosition="False"></FilterControl>
        </SettingsPopup>
        
        <SettingsExport FileName="CatConceptos" />
        
        <Columns>
            <!-- Columnas generadas dinámicamente desde el API -->
        </Columns>
    </dx:ASPxGridView>

    <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="grdCatConceptos"></dx:ASPxGridViewExporter>

    <!-- Popup para nuevo documento -->
    <dx:ASPxPopupControl ID="popConceptos" runat="server" ClientInstanceName="popConceptos"
        HeaderText="Concepto" Width="800px" Height="350px"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        Modal="True" CloseAction="CloseButton" CloseOnEscape="True">

        <ContentStyle BackColor="#E0EDFA">
        </ContentStyle>

        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxHiddenField ID="hfId" runat="server" ClientInstanceName="hfId" />

                <!-- Formulario de documento -->
                <dx:ASPxFormLayout ID="formConcepto" runat="server" Width="100%" ColumnCount="2" AlignItemCaptionsInAllGroups="True">
                    <Items>

                        <dx:LayoutItem Caption="Clave">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtClave" runat="server" ClientInstanceName="txtClave" Width="100%" Theme="Office2010Blue">
                                        <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip" ErrorText="No debe estar vacio">
                                            <RequiredField IsRequired="True" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Clave Alterna">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtClaveAlt" runat="server" ClientInstanceName="txtClaveAlt" Width="100%" Theme="Office2010Blue">
                                        <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip" ErrorText="No debe estar vacio">
                                            <RequiredField IsRequired="True" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                    </Items>
                    <SettingsItemCaptions Location="Top" />

                </dx:ASPxFormLayout>

                <!-- Formulario de campo descripcion para que quede a todo el ancho -->
                <dx:ASPxFormLayout ID="formDescripcion" runat="server" Width="100%" ColumnCount="1" AlignItemCaptionsInAllGroups="True">
                    <Items>

                        <dx:LayoutItem Caption="Concepto">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtConcepto" runat="server" ClientInstanceName="txtConcepto" Width="100%" Theme="Office2010Blue">
                                        <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip" ErrorText="No debe estar vacio">
                                            <RequiredField IsRequired="True" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                    </Items>
                    <SettingsItemCaptions Location="Top" />

                </dx:ASPxFormLayout>

                <!-- Formulario de campoa faltantes -->
                <dx:ASPxFormLayout ID="formDatos" runat="server" Width="100%" ColumnCount="2" AlignItemCaptionsInAllGroups="True">
                    <Items>

                        <dx:LayoutItem Caption="Unidad de medida">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtUmedida" runat="server" ClientInstanceName="txtUmedida" Width="100%" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Existencia">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtExistencia" runat="server" ClientInstanceName="txtExistencia" Width="100%" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Familia">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxGridLookup ID="glFamilia" runat="server" ClientInstanceName="glFamilia" Width="100%" KeyFieldName="Id" TextFormatString="{0}" Theme="Office2010Blue">

                                        <GridViewProperties>
                                            <Settings ShowFilterRow="True" />
                                            <SettingsBehavior AllowFocusedRow="True" />
                                            <SettingsPager Mode="ShowAllRecords" />

                                            <SettingsPopup>
                                                <FilterControl AutoUpdatePosition="False"></FilterControl>
                                            </SettingsPopup>
                                        </GridViewProperties>

                                        <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip" ErrorText="No debe estar vacio">
                                            <RequiredField IsRequired="True" />
                                        </ValidationSettings>

                                    </dx:ASPxGridLookup>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Costo unitario">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtCosto" runat="server" ClientInstanceName="txtCosto" Width="100%" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Factor de conversion">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtFactor" runat="server" ClientInstanceName="txtFactor" Width="100%" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Cantidad minima en stock">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtMinimo" runat="server" ClientInstanceName="txtMinimo" Width="100%" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Cantidad maxima en stock">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtMaximo" runat="server" ClientInstanceName="txtMaximo" Width="100%" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Inventariable">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxCheckBox ID="chkInventariable" runat="server" Text="Inventariable" Theme="Office2010Blue"></dx:ASPxCheckBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                    </Items>

                    <SettingsItemCaptions Location="Top" />

                </dx:ASPxFormLayout>

                <div class="form-actions mt-3">
                    <dx:ASPxButton ID="btnGuardarCambios" runat="server" Text="Guardar cambios" Width="200px" CausesValidation="False">
                        <Image IconID="actions_apply_16x16">
                        </Image>
                    </dx:ASPxButton>
                </div>

            </dx:PopupControlContentControl>
        </ContentCollection>

    </dx:ASPxPopupControl>
</asp:Content>

<asp:Content ID="ContentScripts" ContentPlaceHolderID="scripts" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/app/Catalogos/Cat_Conceptos.js") %>"></script>
</asp:Content>
