<%@ Page Title="Proveedores" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Proveedores.aspx.vb" Inherits="JelaWeb.Proveedores" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/Content/css/modules/proveedores.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="proveedores-wrapper">
        <div class="proveedores-container">
            <div class="proveedores-header">
                <div>
                    <h1 class="proveedores-title">Proveedores</h1>
                    <p class="proveedores-subtitle">Gestión de proveedores del sistema</p>
                </div>
            </div>

            <dx:ASPxGridView ID="gridProveedores" runat="server" ClientInstanceName="gridProveedores"
                Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
                OnRowInserting="gridProveedores_RowInserting"
                OnRowUpdating="gridProveedores_RowUpdating"
                OnRowDeleting="gridProveedores_RowDeleting"
                OnDataBound="gridProveedores_DataBound">

                <SettingsPager Mode="ShowAllRecords" />
                <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
                <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
                <SettingsBehavior AllowSort="True" AllowGroup="True" ConfirmDelete="True" />
                <SettingsEditing Mode="PopupEditForm" />
                <SettingsPopup>
                    <EditForm Width="700px" Modal="True" HorizontalAlign="WindowCenter" VerticalAlign="WindowCenter" />
                </SettingsPopup>
                <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" />
                <SettingsText ConfirmDelete="¿Está seguro que desea eliminar este proveedor?" />

                <SettingsCommandButton>
                    <NewButton Text="Nuevo Proveedor">
                        <Image IconID="actions_add_16x16" />
                    </NewButton>
                    <EditButton Text="Editar Proveedor">
                        <Image IconID="edit_edit_16x16" />
                    </EditButton>
                    <DeleteButton Text="Eliminar Proveedor">
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
                <!-- Columnas generadas dinámicamente desde el API --               <dx:GridViewColumnLayoutItem ColumnName="RFC" />
                                <dx:GridViewColumnLayoutItem ColumnName="Telefono" />
                                <dx:GridViewColumnLayoutItem ColumnName="Email" ColSpan="2" />
                                <dx:GridViewColumnLayoutItem ColumnName="Activo" />
                            </Items>
                        </dx:GridViewLayoutGroup>
                        <dx:EditModeCommandLayoutItem HorizontalAlign="Right" />
                    </Items>
                </EditFormLayoutProperties>

            </dx:ASPxGridView>
        </div>
    </div>

</asp:Content>
        </dx:ASPxGridView>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="scripts" runat="server">
    <script src="<%= ResolveUrl("~/Scripts/app/Catalogos/proveedores.js") %>"></script>