<%@ Page Title="Proveedores" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Proveedores.aspx.vb" Inherits="JelaWeb.Proveedores" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/css/catalogos.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
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
            <EditForm Width="600px" Modal="True" HorizontalAlign="WindowCenter" VerticalAlign="WindowCenter" />
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
                    <dx:GridViewToolbarItem Command="New" />
                    <dx:GridViewToolbarItem Command="Edit" />
                    <dx:GridViewToolbarItem Command="Delete" />
                    <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                    <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                </Items>
            </dx:GridViewToolbar>
        </Toolbars>
        
        <Columns>
            <dx:GridViewCommandColumn ShowEditButton="True" ShowDeleteButton="True" VisibleIndex="0" Width="80px" />
            <dx:GridViewDataTextColumn FieldName="Id" Caption="ID" Visible="False" />
            <dx:GridViewDataTextColumn FieldName="RazonSocial" Caption="Razón Social" VisibleIndex="1" Width="200px">
                <PropertiesTextEdit>
                    <ValidationSettings RequiredField-IsRequired="True" RequiredField-ErrorText="La razón social es requerida" />
                </PropertiesTextEdit>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="NombreComercial" Caption="Nombre Comercial" VisibleIndex="2" Width="180px" />
            <dx:GridViewDataTextColumn FieldName="RFC" Caption="RFC" VisibleIndex="3" Width="130px">
                <PropertiesTextEdit>
                    <ValidationSettings RequiredField-IsRequired="True" RequiredField-ErrorText="El RFC es requerido" />
                </PropertiesTextEdit>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Email" Caption="Email" VisibleIndex="4" Width="180px" />
            <dx:GridViewDataTextColumn FieldName="Telefono" Caption="Teléfono" VisibleIndex="5" Width="120px" />
            <dx:GridViewDataCheckColumn FieldName="Activo" Caption="Activo" VisibleIndex="6" Width="80px" />
        </Columns>
        
        <EditFormLayoutProperties>
            <Items>
                <dx:GridViewLayoutGroup Caption="Datos del Proveedor" ColCount="2">
                    <Items>
                        <dx:GridViewColumnLayoutItem ColumnName="RazonSocial" ColSpan="2" />
                        <dx:GridViewColumnLayoutItem ColumnName="NombreComercial" ColSpan="2" />
                        <dx:GridViewColumnLayoutItem ColumnName="RFC" />
                        <dx:GridViewColumnLayoutItem ColumnName="Telefono" />
                        <dx:GridViewColumnLayoutItem ColumnName="Email" ColSpan="2" />
                        <dx:GridViewColumnLayoutItem ColumnName="Activo" />
                    </Items>
                </dx:GridViewLayoutGroup>
                <dx:EditModeCommandLayoutItem HorizontalAlign="Right" />
            </Items>
        </EditFormLayoutProperties>
        
    </dx:ASPxGridView>

</asp:Content>
