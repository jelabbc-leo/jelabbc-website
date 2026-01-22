<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="Inicio.aspx.vb" Inherits="JelaWeb.Inicio" %>
<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraCharts.v22.2.Web, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraCharts.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.XtraCharts.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.XtraCharts" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="dashboard-container">
        <!-- Header del Dashboard -->
        <div class="dashboard-header mb-4">
            <div class="d-flex justify-content-between align-items-center w-100">
                <div>
                    <h2 class="dashboard-title mb-2">
                        <i class="fas fa-chart-line me-2"></i>
                        <asp:Literal ID="litDashboardTitle" runat="server" Text="Dashboard"></asp:Literal>
                    </h2>
                    <p class="text-muted mb-0">
                        <asp:Literal ID="litWelcomeMessage" runat="server"></asp:Literal>
                    </p>
                </div>
                <div>
                    <asp:LinkButton ID="lnkGestionTickets" runat="server" CssClass="btn btn-light btn-lg" OnClick="lnkGestionTickets_Click">
                        <i class="fas fa-ticket-alt me-2"></i>
                        <asp:Literal ID="litGestionTickets" runat="server" Text="Gestión de Tickets"></asp:Literal>
                    </asp:LinkButton>
                </div>
            </div>
        </div>

        <!-- Cards de Métricas -->
        <div class="row g-3 mb-4">
            <!-- Card: Tickets Abiertos -->
            <div class="col-md-6 col-lg-4">
                <asp:LinkButton ID="lnkTicketsAbiertos" runat="server" OnClick="lnkTicketsAbiertos_Click" style="text-decoration: none; color: inherit; display: block;">
                    <div class="card metric-card metric-card-primary h-100" style="cursor: pointer;">
                        <div class="card-body">
                            <div class="d-flex justify-content-between align-items-start">
                                <div>
                                    <h6 class="card-subtitle mb-2 text-muted">
                                        <i class="fas fa-folder-open me-2"></i>
                                        <asp:Literal ID="litTicketsAbiertos" runat="server" Text="Tickets Abiertos"></asp:Literal>
                                    </h6>
                                    <h2 class="card-title mb-0" id="metricTicketsAbiertos" runat="server">0</h2>
                                </div>
                                <div class="metric-icon">
                                    <i class="fas fa-folder-open fa-2x"></i>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:LinkButton>
            </div>

            <!-- Card: Tickets Cerrados -->
            <div class="col-md-6 col-lg-4">
                <div class="card metric-card metric-card-success h-100">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <h6 class="card-subtitle mb-2 text-muted">
                                    <i class="fas fa-check-circle me-2"></i>
                                    <asp:Literal ID="litTicketsCerrados" runat="server" Text="Tickets Cerrados"></asp:Literal>
                                </h6>
                                <h2 class="card-title mb-0" id="metricTicketsCerrados" runat="server">0</h2>
                            </div>
                            <div class="metric-icon">
                                <i class="fas fa-check-circle fa-2x"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Card: Tickets en Proceso -->
            <div class="col-md-6 col-lg-4">
                <div class="card metric-card metric-card-warning h-100">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <h6 class="card-subtitle mb-2 text-muted">
                                    <i class="fas fa-clock me-2"></i>
                                    <asp:Literal ID="litTicketsEnProceso" runat="server" Text="Tickets en Proceso"></asp:Literal>
                                </h6>
                                <h2 class="card-title mb-0" id="metricTicketsEnProceso" runat="server">0</h2>
                            </div>
                            <div class="metric-icon">
                                <i class="fas fa-clock fa-2x"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Card: Tickets Pendientes -->
            <div class="col-md-6 col-lg-4">
                <div class="card metric-card metric-card-info h-100">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <h6 class="card-subtitle mb-2 text-muted">
                                    <i class="fas fa-hourglass-half me-2"></i>
                                    <asp:Literal ID="litTicketsPendientes" runat="server" Text="Tickets Pendientes"></asp:Literal>
                                </h6>
                                <h2 class="card-title mb-0" id="metricTicketsPendientes" runat="server">0</h2>
                            </div>
                            <div class="metric-icon">
                                <i class="fas fa-hourglass-half fa-2x"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Card: Tickets Urgentes -->
            <div class="col-md-6 col-lg-4">
                <div class="card metric-card metric-card-danger h-100">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <h6 class="card-subtitle mb-2 text-muted">
                                    <i class="fas fa-exclamation-triangle me-2"></i>
                                    <asp:Literal ID="litTicketsUrgentes" runat="server" Text="Tickets Urgentes"></asp:Literal>
                                </h6>
                                <h2 class="card-title mb-0" id="metricTicketsUrgentes" runat="server">0</h2>
                            </div>
                            <div class="metric-icon">
                                <i class="fas fa-exclamation-triangle fa-2x"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Card: Total Tickets -->
            <div class="col-md-6 col-lg-4">
                <div class="card metric-card metric-card-secondary h-100">
                    <div class="card-body">
                        <div class="d-flex justify-content-between align-items-start">
                            <div>
                                <h6 class="card-subtitle mb-2 text-muted">
                                    <i class="fas fa-list me-2"></i>
                                    <asp:Literal ID="litTotalTickets" runat="server" Text="Total de Tickets"></asp:Literal>
                                </h6>
                                <h2 class="card-title mb-0" id="metricTotalTickets" runat="server">0</h2>
                            </div>
                            <div class="metric-icon">
                                <i class="fas fa-list fa-2x"></i>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Gráficas -->
        <div class="row g-3 mb-4">
            <!-- Gráfica: Tickets por Estado -->
            <div class="col-lg-6">
                <div class="card chart-card h-100">
                    <div class="card-header">
                        <h5 class="card-title mb-0">
                            <i class="fas fa-chart-pie me-2"></i>
                            <asp:Literal ID="litChartTicketsByStatus" runat="server" Text="Tickets por Estado"></asp:Literal>
                        </h5>
                    </div>
                    <div class="card-body">
                        <dx:WebChartControl ID="chartTicketsByStatus" runat="server" 
                                           Width="600px" 
                                           Height="400px"
                                           ClientInstanceName="chartTicketsByStatus"
                                           EnableClientSideAPI="true">
                            <Legend Visibility="True" 
                                    AlignmentHorizontal="Right" 
                                    AlignmentVertical="Top" 
                                    MaxHorizontalPercentage="30">
                            </Legend>
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
                </div>
            </div>

            <!-- Gráfica: Tickets por Mes -->
            <div class="col-lg-6">
                <div class="card chart-card h-100">
                    <div class="card-header">
                        <h5 class="card-title mb-0">
                            <i class="fas fa-chart-bar me-2"></i>
                            <asp:Literal ID="litChartTicketsByMonth" runat="server" Text="Tickets por Mes"></asp:Literal>
                        </h5>
                    </div>
                    <div class="card-body">
                        <dx:WebChartControl ID="chartTicketsByMonth" runat="server" 
                                           Width="600px" 
                                           Height="400px"
                                           ClientInstanceName="chartTicketsByMonth"
                                           EnableClientSideAPI="true">
                            <Legend Visibility="False" />
                            <BorderOptions Visibility="False" />
                            <DiagramSerializable>
                                <cc1:XYDiagram>
                                    <AxisX VisibleInPanesSerializable="-1">
                                        <Label Angle="0" />
                                        <GridLines Visible="True" />
                                    </AxisX>
                                    <AxisY VisibleInPanesSerializable="-1">
                                        <Label />
                                        <GridLines Visible="True" />
                                    </AxisY>
                                </cc1:XYDiagram>
                            </DiagramSerializable>
                            <SeriesTemplate>
                                <ViewSerializable>
                                    <cc1:SideBySideBarSeriesView />
                                </ViewSerializable>
                            </SeriesTemplate>
                        </dx:WebChartControl>
                    </div>
                </div>
            </div>
        </div>

        <!-- Información adicional según rol -->
        <div class="row">
            <div class="col-12">
                <div class="alert alert-info" role="alert">
                    <i class="fas fa-info-circle me-2"></i>
                    <strong>
                        <asp:Literal ID="litRoleInfo" runat="server"></asp:Literal>
                    </strong>
                    <asp:Literal ID="litRoleDescription" runat="server"></asp:Literal>
                </div>
            </div>
        </div>
    </div>

    <!-- Estilos CSS personalizados para el dashboard -->
    <style>
        .dashboard-container {
            padding: 20px;
            background-color: #f8f9fa;
            min-height: calc(100vh - 150px);
        }

        .dashboard-header {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .dashboard-title {
            color: white;
            font-weight: 600;
            margin: 0;
        }

        .metric-card {
            border: none;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            transition: transform 0.2s, box-shadow 0.2s;
        }

        .metric-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 4px 20px rgba(0,0,0,0.15);
        }

        .metric-card-primary {
            border-left: 4px solid #007bff;
        }

        .metric-card-success {
            border-left: 4px solid #28a745;
        }

        .metric-card-warning {
            border-left: 4px solid #ffc107;
        }

        .metric-card-info {
            border-left: 4px solid #17a2b8;
        }

        .metric-card-danger {
            border-left: 4px solid #dc3545;
        }

        .metric-card-secondary {
            border-left: 4px solid #6c757d;
        }

        .metric-icon {
            opacity: 0.2;
            color: #6c757d;
        }

        .chart-card {
            border: none;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .chart-card .card-header {
            background-color: #fff;
            border-bottom: 2px solid #f0f0f0;
            padding: 15px 20px;
        }

        .chart-card .card-title {
            font-weight: 600;
            color: #333;
            margin: 0;
        }

        /* Hacer gráficas responsivas usando CSS */
        .chart-card .card-body {
            overflow-x: auto;
        }

        .chart-card .card-body > div {
            width: 100% !important;
            max-width: 100%;
        }

        @media (max-width: 768px) {
            .dashboard-container {
                padding: 10px;
            }

            .chart-card .card-body {
                padding: 10px;
            }
        }
    </style>

    <!-- Chat Widget CSS -->
    <link rel="stylesheet" href="/Content/CSS/chat-widget.css?v=2" />

    <!-- Chat Widget JS -->
    <script src="/Scripts/widgets/chat-widget.js?v=2"></script>

    <!-- Inicialización del Chat Widget -->
    <script>
        // Inicializar el widget cuando el DOM esté listo
        document.addEventListener('DOMContentLoaded', function() {
            JelaChatWidget.init({
                apiUrl: '<%= ConfigurationManager.AppSettings("ApiBaseUrl").Replace("/api/CRUD?strQuery=", "") %>',
                idEntidad: 1,
                autoOpen: false,
                theme: 'blue',
                showBranding: true
            });
        });
    </script>
</asp:Content>
