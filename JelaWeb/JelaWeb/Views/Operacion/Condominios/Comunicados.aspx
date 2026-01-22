<%@ Page Title="Comunicados" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Comunicados.aspx.vb" Inherits="JelaWeb.Comunicados" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/comunicados.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/comunicados.js") %>?v=20260121" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">
        
        <!-- GRID PRINCIPAL -->
        <dx:ASPxGridView ID="gridComunicados" runat="server" ClientInstanceName="gridComunicados"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridComunicados_DataBound" OnCustomCallback="gridComunicados_CustomCallback"
            EnableCallBacks="True">
            
            <ClientSideEvents ToolbarItemClick="onToolbarComunicadosClick" RowDblClick="onRowDblClick" />
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" AllowSelectByRowClick="True" />
            <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" FileName="Comunicados" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="True" EnableCollapseRootItemsToIcons="True" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevoComunicado" Text="Nuevo Comunicado">
                            <Image IconID="actions_add_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEditarComunicado" Text="Editar Comunicado">
                            <Image IconID="edit_edit_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnPublicarComunicado" Text="Publicar">
                            <Image IconID="mail_send_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEliminarComunicado" Text="Eliminar Comunicado" BeginGroup="True">
                            <Image IconID="actions_trash_16x16" />
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            
            <%-- Columnas generadas dinámicamente desde el API --%>
            <Columns>
                <dx:GridViewCommandColumn ShowSelectCheckbox="False" VisibleIndex="0" Width="60px" />
            </Columns>
            
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridComunicados" />
    </div>

    <!-- POPUP: COMUNICADO -->
    <dx:ASPxPopupControl ID="popupComunicado" runat="server" ClientInstanceName="popupComunicado"
        HeaderText="Nuevo Comunicado" Width="900px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formComunicado" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <dx:LayoutItem Caption="Tipo Comunicado">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboTipoComunicado" runat="server" ClientInstanceName="cboTipoComunicado"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="General" Value="General" Selected="True" />
                                            <dx:ListEditItem Text="Aviso" Value="Aviso" />
                                            <dx:ListEditItem Text="Urgente" Value="Urgente" />
                                            <dx:ListEditItem Text="Mantenimiento" Value="Mantenimiento" />
                                            <dx:ListEditItem Text="Evento" Value="Evento" />
                                            <dx:ListEditItem Text="Asamblea" Value="Asamblea" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Título" RequiredMarkDisplayMode="Required" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtTitulo" runat="server" ClientInstanceName="txtTitulo"
                                        Width="100%" Theme="Office2010Blue" MaxLength="200">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Destinatarios">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboDestinatarios" runat="server" ClientInstanceName="cboDestinatarios"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="Todos" Value="Todos" Selected="True" />
                                            <dx:ListEditItem Text="Propietarios" Value="Propietarios" />
                                            <dx:ListEditItem Text="Inquilinos" Value="Inquilinos" />
                                            <dx:ListEditItem Text="Morosos" Value="Morosos" />
                                            <dx:ListEditItem Text="Sección" Value="Seccion" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Sub-Entidad">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboSubEntidad" runat="server" ClientInstanceName="cboSubEntidad"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Publicación" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteFechaPublicacion" runat="server" ClientInstanceName="dteFechaPublicacion"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy HH:mm"
                                        EditFormat="DateTime" UseMaskBehavior="True">
                                        <TimeSectionProperties>
                                            <TimeEditProperties />
                                        </TimeSectionProperties>
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Expiración">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteFechaExpiracion" runat="server" ClientInstanceName="dteFechaExpiracion"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy HH:mm"
                                        EditFormat="DateTime" UseMaskBehavior="True">
                                        <TimeSectionProperties>
                                            <TimeEditProperties />
                                        </TimeSectionProperties>
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Estado">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboEstado" runat="server" ClientInstanceName="cboEstado"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                        <Items>
                                            <dx:ListEditItem Text="Borrador" Value="Borrador" Selected="True" />
                                            <dx:ListEditItem Text="Publicado" Value="Publicado" />
                                            <dx:ListEditItem Text="Expirado" Value="Expirado" />
                                            <dx:ListEditItem Text="Cancelado" Value="Cancelado" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutGroup Caption="Canales de Envío" ColSpan="2" ColCount="3">
                            <Items>
                                <dx:LayoutItem ShowCaption="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxCheckBox ID="chkEnviarEmail" runat="server" ClientInstanceName="chkEnviarEmail"
                                                Text="Email" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem ShowCaption="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxCheckBox ID="chkEnviarTelegram" runat="server" ClientInstanceName="chkEnviarTelegram"
                                                Text="Telegram" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem ShowCaption="False">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer runat="server">
                                            <dx:ASPxCheckBox ID="chkEnviarPush" runat="server" ClientInstanceName="chkEnviarPush"
                                                Text="Push" Theme="Office2010Blue" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                        <dx:LayoutItem Caption="Contenido" RequiredMarkDisplayMode="Required" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxMemo ID="txtContenido" runat="server" ClientInstanceName="txtContenido"
                                        Width="100%" Height="200px" Theme="Office2010Blue">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxMemo>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Archivo Adjunto" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxUploadControl ID="uploadArchivo" runat="server" ClientInstanceName="uploadArchivo"
                                        Width="100%" Theme="Office2010Blue" UploadMode="Auto" ShowProgressPanel="True"
                                        FileUploadMode="OnPageLoad">
                                        <ValidationSettings AllowedFileExtensions=".pdf,.jpg,.jpeg,.png,.doc,.docx" MaxFileSize="10485760" />
                                        <ClientSideEvents FileUploadComplete="onArchivoUploadComplete" />
                                    </dx:ASPxUploadControl>
                                    <asp:HiddenField ID="hfArchivoAdjunto" runat="server" ClientIDMode="Static" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                <div style="margin-top:15px; text-align:right;">
                    <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s,e){ guardarComunicado(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrar" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupComunicado.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <asp:HiddenField ID="hfId" runat="server" Value="0" ClientIDMode="Static" />

</asp:Content>
