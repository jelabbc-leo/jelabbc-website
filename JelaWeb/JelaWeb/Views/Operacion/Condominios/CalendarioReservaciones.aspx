<%@ Page Title="Calendario de Reservaciones" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="CalendarioReservaciones.aspx.vb" Inherits="JelaWeb.CalendarioReservaciones" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/reservaciones.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/calendario-reservaciones.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">
        
        <!-- FILTROS -->
        <dx:ASPxFormLayout ID="formFiltros" runat="server" ColCount="4" Width="100%" Theme="Office2010Blue" BackColor="#E4EFFA">
            <Items>
                <dx:LayoutItem Caption="Área Común">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxComboBox ID="cboAreaComun" runat="server" ClientInstanceName="cboAreaComun"
                                Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="cargarCalendario" />
                            </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Mes">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxComboBox ID="cboMes" runat="server" ClientInstanceName="cboMes"
                                Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="cargarCalendario" />
                            </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Año">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxComboBox ID="cboAnio" runat="server" ClientInstanceName="cboAnio"
                                Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                <ClientSideEvents SelectedIndexChanged="cargarCalendario" />
                            </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem ShowCaption="False">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxButton ID="btnRefrescar" runat="server" Text="Refrescar" Theme="Office2010Blue" 
                                AutoPostBack="False">
                                <ClientSideEvents Click="function(s,e){ cargarCalendario(); }" />
                            </dx:ASPxButton>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
            </Items>
        </dx:ASPxFormLayout>

        <br />

        <!-- GRID CALENDARIO -->
        <dx:ASPxGridView ID="gridCalendario" runat="server" ClientInstanceName="gridCalendario"
            KeyFieldName="Id" Width="100%" Theme="Office2010Blue" AutoGenerateColumns="False"
            OnDataBound="gridCalendario_DataBound" OnCustomCallback="gridCalendario_CustomCallback"
            EnableCallBacks="True">
            
            <SettingsPager Mode="ShowAllRecords" />
            <Settings ShowFilterRow="False" ShowFilterRowMenu="True" ShowGroupPanel="True" />
            <SettingsSearchPanel Visible="True" ShowApplyButton="True" ShowClearButton="True" />
            <SettingsBehavior AllowSort="True" AllowGroup="True" AllowFocusedRow="True" />
            
            <Toolbars>
                <dx:GridViewToolbar>
                    <Items>
                        <dx:GridViewToolbarItem BeginGroup="True" Command="Refresh" />
                        <dx:GridViewToolbarItem Command="ExportToXlsx" Text="Exportar Excel" />
                    </Items>
                </dx:GridViewToolbar>
            </Toolbars>
            
            <Columns>
                <dx:GridViewCommandColumn ShowSelectCheckbox="False" VisibleIndex="0" Width="60px" />
            </Columns>
            
        </dx:ASPxGridView>
        
    </div>

</asp:Content>
