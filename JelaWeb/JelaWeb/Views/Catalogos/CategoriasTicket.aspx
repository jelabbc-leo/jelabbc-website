<%@ Page Title="Categorías de Tickets" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="CategoriasTicket.aspx.vb" Inherits="JelaWeb.CategoriasTicket" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/css/catalogos.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    
    <dx:ASPxGridView ID="gridCategorias" runat="server" ClientInstanceName="gridCategorias"
        Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" Theme="Office2010Blue"
        OnRowInserting="gridCategorias_RowInserting"
        OnRowUpdating="gridCategorias_RowUpdating"
        OnRowDeleting="gridCategorias_RowDeleting"
        OnDataBound="gridCategorias_DataBound">
        
        <SettingsPager Mode="ShowAllRecords" />
        <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
        <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
        <SettingsBehavior AllowSort="True" AllowGroup="True" ConfirmDelete="True" />
        <SettingsEditing Mode="PopupEditForm" />
        <SettingsPopup>
            <EditForm Width="500px" Modal="True" HorizontalAlign="WindowCenter" VerticalAlign="WindowCenter" />
        </SettingsPopup>
        <SettingsExport EnableClientSideExportAPI="True" ExcelExportMode="WYSIWYG" />
        <SettingsText ConfirmDelete="¿Está seguro que desea eliminar esta categoría?" />
        
        <SettingsCommandButton>
            <NewButton Text="Nueva Categoría">
                <Image IconID="actions_add_16x16" />
            </NewButton>
            <EditButton Text="Editar Categoría">
                <Image IconID="edit_edit_16x16" />
            </EditButton>
            <DeleteButton Text="Eliminar Categoría">
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
            <dx:GridViewDataTextColumn FieldName="Descripcion" Caption="Descripción" VisibleIndex="2" Width="300px" />
            <dx:GridViewDataSpinEditColumn FieldName="TiempoSLA" Caption="Tiempo SLA (hrs)" VisibleIndex="3" Width="120px" />
            <dx:GridViewDataCheckColumn FieldName="Activo" Caption="Activo" VisibleIndex="4" Width="80px" />
        </Columns>
        
        <EditFormLayoutProperties>
            <Items>
                <dx:GridViewLayoutGroup Caption="Datos de la Categoría" ColCount="1">
                    <Items>
                        <dx:GridViewColumnLayoutItem ColumnName="Nombre" />
                        <dx:GridViewColumnLayoutItem ColumnName="Descripcion" />
                        <dx:GridViewColumnLayoutItem ColumnName="TiempoSLA" />
                        <dx:GridViewColumnLayoutItem ColumnName="Activo" />
                    </Items>
                </dx:GridViewLayoutGroup>
                <dx:EditModeCommandLayoutItem HorizontalAlign="Right" />
            </Items>
        </EditFormLayoutProperties>
        
    </dx:ASPxGridView>

</asp:Content>
