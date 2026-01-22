<%@ Page Title="Formularios Dinámicos" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="FormulariosDinamicos.aspx.vb" Inherits="JelaWeb.FormulariosDinamicos" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        function onToolbarClick(s, e) {
            var name = e.item.name;
            if (name === 'btnNuevo') {
                window.location.href = 'FormularioDisenador.aspx';
            } else if (name === 'btnEditar') {
                editarSeleccionado();
            } else if (name === 'btnEliminar') {
                eliminarSeleccionado();
            }
        }

        function onRowDblClick(s, e) {
            editarSeleccionado();
        }

        function editarSeleccionado() {
            var keys = gridFormularios.GetSelectedKeysOnPage();
            if (keys.length === 0) {
                alert('Seleccione un formulario para editar');
                return;
            }
            window.location.href = 'FormularioDisenador.aspx?id=' + keys[0];
        }

        function eliminarSeleccionado() {
            var keys = gridFormularios.GetSelectedKeysOnPage();
            if (keys.length === 0) {
                alert('Seleccione un formulario para eliminar');
                return;
            }
            if (confirm('¿Está seguro de eliminar este formulario?')) {
                gridFormularios.PerformCallback('delete|' + keys[0]);
            }
        }
    </script>

    <div class="container-fluid" style="padding: 15px;">

        <!-- GRID CON TOOLBAR INTEGRADO -->
        <dx:ASPxGridView ID="gridFormularios" runat="server" ClientInstanceName="gridFormularios" 
            KeyFieldName="formulario_id" Width="100%" Theme="Office2010Blue" 
            AutoGenerateColumns="False" OnCustomCallback="gridFormularios_CustomCallback">
            <ClientSideEvents ToolbarItemClick="onToolbarClick" RowDblClick="onRowDblClick" />
            <Settings ShowHeaderFilterButton="True" ShowGroupPanel="True" 
                VerticalScrollableHeight="500" VerticalScrollBarMode="Visible" />
            <SettingsSearchPanel Visible="True" />
            <SettingsBehavior AllowFocusedRow="True" AllowSelectByRowClick="True" EnableRowHotTrack="True" />
            <SettingsPager Mode="ShowAllRecords" />
            <SettingsExport FileName="Formularios" />
            <Toolbars>
                <dx:GridViewToolbar>
                    <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                    <Items>
                        <dx:GridViewToolbarItem Name="btnNuevo" Text="Nuevo Formulario" BeginGroup="True">
                            <Image IconID="actions_add_16x16"></Image>
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEditar" Text="Editar">
                            <Image IconID="edit_edit_16x16"></Image>
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem Name="btnEliminar" Text="Eliminar">
                            <Image IconID="edit_delete_16x16"></Image>
                        </dx:GridViewToolbarItem>
                        <dx:GridViewToolbarItem BeginGroup="True" />
                        <dx:GridViewToolbarItem Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            <Columns>
                <dx:GridViewDataTextColumn FieldName="formulario_id" Caption="ID" Width="70px" VisibleIndex="0" />
                <dx:GridViewDataTextColumn FieldName="nombre_formulario" Caption="Nombre" Width="250px" VisibleIndex="1" />
                <dx:GridViewDataTextColumn FieldName="descripcion" Caption="Descripción" Width="300px" VisibleIndex="2" />
                <dx:GridViewDataTextColumn FieldName="plataformas" Caption="Plataformas" Width="120px" VisibleIndex="3" />
                <dx:GridViewDataComboBoxColumn FieldName="estado" Caption="Estado" Width="100px" VisibleIndex="4">
                    <PropertiesComboBox>
                        <Items>
                            <dx:ListEditItem Text="Borrador" Value="borrador" />
                            <dx:ListEditItem Text="Activo" Value="activo" />
                            <dx:ListEditItem Text="Inactivo" Value="inactivo" />
                        </Items>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataTextColumn FieldName="total_campos" Caption="Campos" Width="80px" VisibleIndex="5" />
                <dx:GridViewDataDateColumn FieldName="fecha_creacion" Caption="Fecha Creación" Width="150px" VisibleIndex="6">
                    <PropertiesDateEdit DisplayFormatString="dd/MM/yyyy HH:mm" />
                </dx:GridViewDataDateColumn>
            </Columns>
        </dx:ASPxGridView>
        <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridFormularios" />

    </div>

</asp:Content>