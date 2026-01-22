<%@ Page Title="Fitosanitarios" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Fitosanitarios.aspx.vb" Inherits="JelaWeb.Fitosanitarios" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/css/catalogos.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <dx:ASPxGridView ID="gridFitosanitarios" runat="server" ClientInstanceName="gridFitosanitarios"
        Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
        OnRowInserting="gridFitosanitarios_RowInserting"
        OnRowUpdating="gridFitosanitarios_RowUpdating"
        OnRowDeleting="gridFitosanitarios_RowDeleting"
        OnDataBound="gridFitosanitarios_DataBound">
        
        <SettingsPager Mode="ShowAllRecords" />
        <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
        <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
        <SettingsBehavior AllowSort="True" AllowGroup="True" ConfirmDelete="True" />
        <SettingsEditing Mode="PopupEditForm" />
        <SettingsPopup>
            <EditForm Width="600px" Modal="True" HorizontalAlign="WindowCenter" VerticalAlign="WindowCenter" />
        </SettingsPopup>
        <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" />
        <SettingsText ConfirmDelete="¿Está seguro que desea eliminar este producto fitosanitario?" />
        
        <SettingsCommandButton>
            <NewButton Text="Nuevo Producto Fitosanitario">
                <Image IconID="actions_add_16x16" />
            </NewButton>
            <EditButton Text="Editar Producto Fitosanitario">
                <Image IconID="edit_edit_16x16" />
            </EditButton>
            <DeleteButton Text="Eliminar Producto Fitosanitario">
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
            <dx:GridViewDataTextColumn FieldName="Nombre" Caption="Nombre" VisibleIndex="1" Width="200px">
                <PropertiesTextEdit>
                    <ValidationSettings RequiredField-IsRequired="True" RequiredField-ErrorText="El nombre es requerido" />
                </PropertiesTextEdit>
            </dx:GridViewDataTextColumn>
            <dx:GridViewDataTextColumn FieldName="Tipo" Caption="Tipo" VisibleIndex="2" Width="120px" />
            <dx:GridViewDataTextColumn FieldName="IngredienteActivo" Caption="Ingrediente Activo" VisibleIndex="3" Width="180px" />
            <dx:GridViewDataSpinEditColumn FieldName="Stock" Caption="Stock" VisibleIndex="4" Width="100px" />
            <dx:GridViewDataTextColumn FieldName="UnidadMedida" Caption="Unidad" VisibleIndex="5" Width="80px" />
            <dx:GridViewDataCheckColumn FieldName="Activo" Caption="Activo" VisibleIndex="6" Width="80px" />
        </Columns>
        
        <EditFormLayoutProperties>
            <Items>
                <dx:GridViewLayoutGroup Caption="Datos del Producto Fitosanitario" ColCount="2">
                    <Items>
                        <dx:GridViewColumnLayoutItem ColumnName="Nombre" ColSpan="2" />
                        <dx:GridViewColumnLayoutItem ColumnName="Tipo" />
                        <dx:GridViewColumnLayoutItem ColumnName="IngredienteActivo" />
                        <dx:GridViewColumnLayoutItem ColumnName="Stock" />
                        <dx:GridViewColumnLayoutItem ColumnName="UnidadMedida" />
                        <dx:GridViewColumnLayoutItem ColumnName="Activo" />
                    </Items>
                </dx:GridViewLayoutGroup>
                <dx:EditModeCommandLayoutItem HorizontalAlign="Right" />
            </Items>
        </EditFormLayoutProperties>
        
    </dx:ASPxGridView>

</asp:Content>
