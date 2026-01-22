<%@ Page Title="Reservaciones" Language="vb" MasterPageFile="~/MasterPages/Jela.Master" AutoEventWireup="false" CodeBehind="Reservaciones.aspx.vb" Inherits="JelaWeb.Reservaciones" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Web.ASPxScheduler.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxScheduler" TagPrefix="dxs" %>
<%@ Register Assembly="DevExpress.XtraCharts.v22.2.Web, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraCharts.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraCharts.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraCharts" TagPrefix="cc1" %>

<%@ Register assembly="DevExpress.XtraScheduler.v22.2.Core.Desktop, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" namespace="DevExpress.XtraScheduler" tagprefix="cc2" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/reservaciones.css") %>?v=20260121" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/reservaciones.js") %>?v=20260121d" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container-fluid">

        <dx:ASPxSplitter ID="splitReservaciones" runat="server" Width="100%" Orientation="Horizontal" CssClass="reservaciones-splitter">
            <Panes>
                <dx:SplitterPane Name="paneCalendario" Size="80%" MinSize="700">
                    <ContentCollection>
                        <dx:SplitterContentControl runat="server">
                            <!-- FILTROS: Solo fechas (según Regla 7) -->
                            <dx:ASPxFormLayout ID="formFiltros" runat="server" ColCount="4" Width="100%" Theme="Office2010Blue" BackColor="#E4EFFA">
                                <Items>
                                    <dx:LayoutItem Caption="Fecha Desde" ColSpan="2">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxDateEdit ID="dtFechaDesde" runat="server" ClientInstanceName="dtFechaDesde"
                                                    Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
                                                </dx:ASPxDateEdit>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem Caption="Fecha Hasta" ColSpan="2">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxDateEdit ID="dtFechaHasta" runat="server" ClientInstanceName="dtFechaHasta"
                                                    Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
                                                </dx:ASPxDateEdit>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                    <dx:LayoutItem ShowCaption="False" ColSpan="4">
                                        <LayoutItemNestedControlCollection>
                                            <dx:LayoutItemNestedControlContainer runat="server">
                                                <dx:ASPxButton ID="btnFiltrar" runat="server" Text="Filtrar" Theme="Office2010Blue" 
                                                    AutoPostBack="True" OnClick="btnFiltrar_Click">
                                                    <Image IconID="zoom_zoom_16x16" />
                                                </dx:ASPxButton>
                                                <dx:ASPxButton ID="btnLimpiar" runat="server" Text="Limpiar" Theme="Office2010Blue" 
                                                    AutoPostBack="True" OnClick="btnLimpiar_Click">
                                                    <Image IconID="actions_cancel_16x16" />
                                                </dx:ASPxButton>
                                            </dx:LayoutItemNestedControlContainer>
                                        </LayoutItemNestedControlCollection>
                                    </dx:LayoutItem>
                                </Items>
                            </dx:ASPxFormLayout>

                            <br />

                            <div class="mb-2">
                                <dx:ASPxButton ID="btnNuevaReservacion" runat="server" Text="Nueva Reservación" Theme="Office2010Blue" AutoPostBack="False">
                                    <Image IconID="actions_add_16x16" />
                                    <ClientSideEvents Click="function(s,e){ mostrarNuevaReservacion(); }" />
                                </dx:ASPxButton>
                                <dx:ASPxButton ID="btnEliminarReservacion" runat="server" Text="Eliminar Reservación" Theme="Office2010Blue" AutoPostBack="False">
                                    <Image IconID="actions_trash_16x16" />
                                    <ClientSideEvents Click="function(s,e){ eliminarReservacionActual(); }" />
                                </dx:ASPxButton>
                            </div>

                            <!-- CALENDARIO (TIPO OUTLOOK) -->
                            <dx:ASPxPopupMenu ID="menuReservaciones" runat="server" ClientInstanceName="menuReservaciones" Theme="Office2010Blue">
                                <Items>
                                    <dx:MenuItem Name="nueva" Text="Nueva Reservación" Image-IconID="actions_add_16x16" >
<Image IconID="actions_add_16x16"></Image>
                                    </dx:MenuItem>
                                    <dx:MenuItem Name="eliminar" Text="Eliminar Reservación" Image-IconID="actions_trash_16x16" >
<Image IconID="actions_trash_16x16"></Image>
                                    </dx:MenuItem>
                                </Items>
                                <ClientSideEvents ItemClick="onReservacionesMenuItemClick" />
                            </dx:ASPxPopupMenu>

                            <dxs:ASPxScheduler ID="schedulerReservaciones" runat="server" ClientInstanceName="schedulerReservaciones"
                                Width="100%" CssClass="scheduler-reservaciones" ActiveViewType="Month" GroupType="None"
                                OnAppointmentFormShowing="schedulerReservaciones_AppointmentFormShowing"
                                OnVisibleIntervalChanged="schedulerReservaciones_VisibleIntervalChanged">

                                <Storage>
                                    <Appointments>
                                        <Mappings AppointmentId="Id" Start="StartDate" End="EndDate" Subject="Subject" Description="Description" Location="Location" />
                                    </Appointments>
                                </Storage>

<Views>
<DayView ViewSelectorItemAdaptivePriority="2"><TimeRulers>
<cc2:TimeRuler></cc2:TimeRuler>
</TimeRulers>

<AppointmentDisplayOptions ColumnPadding-Left="2" ColumnPadding-Right="4"></AppointmentDisplayOptions>
</DayView>

<WorkWeekView ViewSelectorItemAdaptivePriority="6"><TimeRulers>
<cc2:TimeRuler></cc2:TimeRuler>
</TimeRulers>

<AppointmentDisplayOptions ColumnPadding-Left="2" ColumnPadding-Right="4"></AppointmentDisplayOptions>
</WorkWeekView>

<WeekView ViewSelectorItemAdaptivePriority="4"></WeekView>

<MonthView ViewSelectorItemAdaptivePriority="5"></MonthView>

<TimelineView ViewSelectorItemAdaptivePriority="3"></TimelineView>

<FullWeekView ViewSelectorItemAdaptivePriority="7"><TimeRulers>
<cc2:TimeRuler></cc2:TimeRuler>
</TimeRulers>

<AppointmentDisplayOptions ColumnPadding-Left="2" ColumnPadding-Right="4"></AppointmentDisplayOptions>
</FullWeekView>

<AgendaView ViewSelectorItemAdaptivePriority="1"></AgendaView>
</Views>

                                <ClientSideEvents AppointmentDoubleClick="onSchedulerAppointmentDblClick" CellDoubleClick="onSchedulerCellDblClick" Init="onSchedulerInit" />
                            </dxs:ASPxScheduler>
                        </dx:SplitterContentControl>
                    </ContentCollection>
                </dx:SplitterPane>

                <dx:SplitterPane Name="paneDashboard" Size="20%" MinSize="360">
                    <ContentCollection>
                        <dx:SplitterContentControl runat="server">
                            <div class="reservaciones-dashboard">
                                <div class="dashboard-title">Dashboard</div>

                                <div class="kpi-grid">
                                    <div class="kpi-card kpi-primary">
                                        <div class="kpi-label">Reservaciones del mes</div>
                                        <div class="kpi-value" id="kpiTotalMes" runat="server">0</div>
                                    </div>
                                    <div class="kpi-card kpi-info">
                                        <div class="kpi-label">Abiertas</div>
                                        <div class="kpi-value" id="kpiAbiertas" runat="server">0</div>
                                    </div>
                                    <div class="kpi-card kpi-danger">
                                        <div class="kpi-label">Canceladas</div>
                                        <div class="kpi-value" id="kpiCanceladas" runat="server">0</div>
                                    </div>
                                    <div class="kpi-card kpi-warning">
                                        <div class="kpi-label">En uso</div>
                                        <div class="kpi-value" id="kpiEnUso" runat="server">0</div>
                                    </div>
                                    <div class="kpi-card kpi-success">
                                        <div class="kpi-label">Completadas</div>
                                        <div class="kpi-value" id="kpiCompletadas" runat="server">0</div>
                                    </div>
                                    <div class="kpi-card kpi-secondary">
                                        <div class="kpi-label">Próximas 7 días</div>
                                        <div class="kpi-value" id="kpiProximas" runat="server">0</div>
                                    </div>
                                </div>

                                <div class="dashboard-section">
                                    <div class="dashboard-section-title">Reservaciones por estado</div>
                                    <dx:WebChartControl ID="chartReservacionesEstado" runat="server" Width="320px" Height="260px">
                                        <Legend Visibility="True" AlignmentHorizontal="Right" AlignmentVertical="Top" MaxHorizontalPercentage="35"></Legend>
                                        <BorderOptions Visibility="False" />
                                        <DiagramSerializable>
                                            <cc1:SimpleDiagram3D />
                                        </DiagramSerializable>
                                        <SeriesTemplate>
                                            <ViewSerializable>
                                                <cc1:PieSeriesView />
                                            </ViewSerializable>
                                        </SeriesTemplate>
                                    </dx:WebChartControl>
                                </div>

                                <div class="dashboard-section">
                                    <div class="dashboard-section-title">Resumen por área común</div>
                                    <dx:ASPxGridView ID="gridAreasResumen" runat="server" Width="100%" AutoGenerateColumns="True" Theme="Office2010Blue">
                                        <SettingsPager Mode="ShowAllRecords" />
                                        <Settings ShowFilterRow="False" ShowFilterRowMenu="False" ShowGroupPanel="False" />
                                        <SettingsBehavior AllowSort="False" AllowGroup="False" />

<SettingsPopup>
<FilterControl AutoUpdatePosition="False"></FilterControl>
</SettingsPopup>
                                    </dx:ASPxGridView>
                                </div>
                            </div>
                        </dx:SplitterContentControl>
                    </ContentCollection>
                </dx:SplitterPane>
            </Panes>
        </dx:ASPxSplitter>
    </div>

    <!-- ================================================================== -->
    <!-- POPUP: RESERVACIÓN -->
    <!-- ================================================================== -->
    <dx:ASPxPopupControl ID="popupReservacion" runat="server" ClientInstanceName="popupReservacion"
        HeaderText="Nueva Reservación" Width="900px" Modal="True" 
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        CloseAction="CloseButton" AllowDragging="True" CloseOnEscape="True">
        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxFormLayout ID="formReservacion" runat="server" Width="100%" ColCount="2" Theme="Office2010Blue">
                    <Items>
                        <dx:LayoutItem Caption="Área Común" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboAreaComun" runat="server" ClientInstanceName="cboAreaComun"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Unidad" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboUnidad" runat="server" ClientInstanceName="cboUnidad"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32">
                                        <ClientSideEvents SelectedIndexChanged="onUnidadChanged" />
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Residente">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxComboBox ID="cboResidente" runat="server" ClientInstanceName="cboResidente"
                                        Width="100%" Theme="Office2010Blue" ValueType="System.Int32" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Fecha Reservación" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dteFechaReservacion" runat="server" ClientInstanceName="dteFechaReservacion"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="dd/MM/yyyy">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
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
                                            <dx:ListEditItem Text="Pendiente" Value="Pendiente" Selected="True" />
                                            <dx:ListEditItem Text="Confirmada" Value="Confirmada" />
                                            <dx:ListEditItem Text="EnUso" Value="EnUso" />
                                            <dx:ListEditItem Text="Completada" Value="Completada" />
                                            <dx:ListEditItem Text="Cancelada" Value="Cancelada" />
                                        </Items>
                                    </dx:ASPxComboBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Hora Inicio" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTimeEdit ID="teHoraInicio" runat="server" ClientInstanceName="teHoraInicio"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="HH:mm" EditFormat="Time" TimeSectionProperties-Visible="True">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxTimeEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Hora Fin" RequiredMarkDisplayMode="Required">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTimeEdit ID="teHoraFin" runat="server" ClientInstanceName="teHoraFin"
                                        Width="100%" Theme="Office2010Blue" DisplayFormatString="HH:mm" EditFormat="Time" TimeSectionProperties-Visible="True">
                                        <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                                    </dx:ASPxTimeEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Número de Invitados">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="spNumeroInvitados" runat="server" ClientInstanceName="spNumeroInvitados"
                                        Width="100%" Theme="Office2010Blue" NumberType="Integer" MinValue="0" MaxValue="999" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Costo Total">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="spCostoTotal" runat="server" ClientInstanceName="spCostoTotal"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2" 
                                        MinValue="0" MaxValue="999999.99" DisplayFormatString="c2" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Depósito Pagado">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="spDepositoPagado" runat="server" ClientInstanceName="spDepositoPagado"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2" 
                                        MinValue="0" MaxValue="999999.99" DisplayFormatString="c2" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Depósito Devuelto">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxSpinEdit ID="spDepositoDevuelto" runat="server" ClientInstanceName="spDepositoDevuelto"
                                        Width="100%" Theme="Office2010Blue" NumberType="Float" DecimalPlaces="2" 
                                        MinValue="0" MaxValue="999999.99" DisplayFormatString="c2" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Motivo" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtMotivo" runat="server" ClientInstanceName="txtMotivo"
                                        Width="100%" Theme="Office2010Blue" MaxLength="200" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                        <dx:LayoutItem Caption="Observaciones" ColSpan="2">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxMemo ID="txtObservaciones" runat="server" ClientInstanceName="txtObservaciones"
                                        Width="100%" Height="80px" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>
                    </Items>
                </dx:ASPxFormLayout>
                <div style="margin-top:15px; text-align:right;">
                    <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Theme="Office2010Blue" AutoPostBack="False">
                        <Image IconID="save_save_16x16" />
                        <ClientSideEvents Click="function(s,e){ guardarReservacion(); }" />
                    </dx:ASPxButton>
                    <dx:ASPxButton ID="btnCerrar" runat="server" Text="Cancelar" Theme="Office2010Blue" AutoPostBack="False">
                        <ClientSideEvents Click="function(s,e){ popupReservacion.Hide(); }" />
                    </dx:ASPxButton>
                </div>
            </dx:PopupControlContentControl>
        </ContentCollection>
    </dx:ASPxPopupControl>

    <!-- HIDDEN FIELDS -->
    <asp:HiddenField ID="hfId" runat="server" Value="0" ClientIDMode="Static" />

</asp:Content>