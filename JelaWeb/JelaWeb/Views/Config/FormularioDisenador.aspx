<%@ Page Title="Diseñador de Formulario" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="FormularioDisenador.aspx.vb" Inherits="JelaWeb.FormularioDisenador" %>

<%@ Register Assembly="DevExpress.Web.v22.2, Version=22.2.3.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <link href="/Content/CSS/formulario-disenador.css" rel="stylesheet" type="text/css" />

    <!-- Hidden Fields -->
    <asp:HiddenField ID="hfFormularioId" runat="server" Value="0" />
    <asp:HiddenField ID="hfCamposJSON" runat="server" Value="[]" />

    <!-- HEADER CON DATOS DEL FORMULARIO -->
    <div class="designer-header">
        <div class="header-left">
            <dx:ASPxButton ID="btnVolver" runat="server" Text="Volver" Theme="Office2010Blue" AutoPostBack="False">
                <Image IconID="arrows_prev_16x16"></Image>
                <ClientSideEvents Click="function(s,e) { window.location.href='FormulariosDinamicos.aspx'; }" />
            </dx:ASPxButton>
            <span class="header-title">
                <i class="fas fa-edit"></i>
                <asp:Label ID="lblTitulo" runat="server" Text="Nuevo Formulario" />
            </span>
        </div>
        <div class="header-right">
            <dx:ASPxButton ID="btnVistaPrevia" runat="server" Text="Vista Previa" Theme="Office2010Blue" 
                AutoPostBack="False">
                <Image IconID="actions_eye_16x16"></Image>
                <ClientSideEvents Click="function(s,e) { abrirVistaPrevia(); }" />
            </dx:ASPxButton>
            <dx:ASPxButton ID="btnGuardar" runat="server" Text="Guardar" Theme="Office2010Blue" 
                AutoPostBack="True" OnClick="btnGuardar_Click">
                <Image IconID="save_save_16x16"></Image>
            </dx:ASPxButton>
        </div>
    </div>

    <!-- DATOS GENERALES DEL FORMULARIO -->
    <div class="form-data-panel">
        <dx:ASPxFormLayout ID="formDatos" runat="server" ColCount="4" Width="100%" Theme="Office2010Blue">
            <Items>
                <dx:LayoutItem Caption="Nombre" ColSpan="2">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxTextBox ID="txtNombre" runat="server" ClientInstanceName="txtNombre" 
                                Width="100%" Theme="Office2010Blue" MaxLength="200">
                                <ValidationSettings><RequiredField IsRequired="True" ErrorText="Requerido" /></ValidationSettings>
                            </dx:ASPxTextBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Estado">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxComboBox ID="cmbEstado" runat="server" ClientInstanceName="cmbEstado" 
                                Width="100%" Theme="Office2010Blue" ValueType="System.String">
                                <Items>
                                    <dx:ListEditItem Text="Borrador" Value="borrador" Selected="True" />
                                    <dx:ListEditItem Text="Activo" Value="activo" />
                                    <dx:ListEditItem Text="Inactivo" Value="inactivo" />
                                </Items>
                            </dx:ASPxComboBox>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
                <dx:LayoutItem Caption="Plataformas">
                    <LayoutItemNestedControlCollection>
                        <dx:LayoutItemNestedControlContainer runat="server">
                            <dx:ASPxCheckBoxList ID="chkPlataformas" runat="server" ClientInstanceName="chkPlataformas"
                                RepeatDirection="Horizontal" Theme="Office2010Blue">
                                <Items>
                                    <dx:ListEditItem Text="Web" Value="web" Selected="True" />
                                    <dx:ListEditItem Text="Móvil" Value="movil" Selected="True" />
                                </Items>
                            </dx:ASPxCheckBoxList>
                        </dx:LayoutItemNestedControlContainer>
                    </LayoutItemNestedControlCollection>
                </dx:LayoutItem>
            </Items>
        </dx:ASPxFormLayout>
    </div>

    <!-- DISEÑADOR VISUAL ESTILO VS -->
    <div class="designer-container">
        
        <!-- PANEL IZQUIERDO: TOOLBOX -->
        <div class="toolbox-panel">
            <div class="panel-header">
                <i class="fas fa-toolbox"></i> Controles
            </div>
            <div class="toolbox-items" id="toolboxItems">
                <div class="toolbox-item" draggable="true" data-tipo="texto" data-etiqueta="Texto">
                    <i class="fas fa-font"></i> Texto
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="numero" data-etiqueta="Número">
                    <i class="fas fa-hashtag"></i> Número
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="decimal" data-etiqueta="Decimal">
                    <i class="fas fa-percentage"></i> Decimal
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="fecha" data-etiqueta="Fecha">
                    <i class="fas fa-calendar"></i> Fecha
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="fecha_hora" data-etiqueta="Fecha/Hora">
                    <i class="fas fa-calendar-alt"></i> Fecha/Hora
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="hora" data-etiqueta="Hora">
                    <i class="fas fa-clock"></i> Hora
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="dropdown" data-etiqueta="Lista">
                    <i class="fas fa-caret-square-down"></i> Lista
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="radio" data-etiqueta="Opciones">
                    <i class="fas fa-dot-circle"></i> Opciones
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="checkbox" data-etiqueta="Casilla">
                    <i class="fas fa-check-square"></i> Casilla
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="textarea" data-etiqueta="Área Texto">
                    <i class="fas fa-align-left"></i> Área Texto
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="foto" data-etiqueta="Foto">
                    <i class="fas fa-camera"></i> Foto
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="archivo" data-etiqueta="Archivo">
                    <i class="fas fa-file-upload"></i> Archivo
                </div>
                <div class="toolbox-item" draggable="true" data-tipo="firma" data-etiqueta="Firma">
                    <i class="fas fa-signature"></i> Firma
                </div>
                <div class="toolbox-separator"></div>
                <div class="toolbox-item toolbox-item-action" draggable="true" data-tipo="boton_guardar" data-etiqueta="Guardar">
                    <i class="fas fa-save"></i> Btn Guardar
                </div>
                <div class="toolbox-item toolbox-item-action" draggable="true" data-tipo="boton_cancelar" data-etiqueta="Cancelar">
                    <i class="fas fa-times-circle"></i> Btn Cancelar
                </div>
            </div>
        </div>

        <!-- PANEL CENTRAL: CANVAS DE DISEÑO -->
        <div class="designer-panel">
            <div class="panel-header">
                <i class="fas fa-th-large"></i> Diseño del Formulario
                <div class="toolbar-buttons">
                    <button type="button" class="btn-toolbar" onclick="moverCampoArriba()" title="Mover arriba">
                        <i class="fas fa-arrow-up"></i>
                    </button>
                    <button type="button" class="btn-toolbar" onclick="moverCampoAbajo()" title="Mover abajo">
                        <i class="fas fa-arrow-down"></i>
                    </button>
                    <button type="button" class="btn-toolbar btn-danger" onclick="eliminarCampoSeleccionado()" title="Eliminar">
                        <i class="fas fa-trash"></i>
                    </button>
                </div>
            </div>
            <div class="designer-canvas" id="designerCanvas">
                <div class="designer-placeholder" id="designerPlaceholder">
                    <i class="fas fa-hand-point-left"></i>
                    <p>Arrastra controles desde el Toolbox</p>
                </div>
            </div>
        </div>

        <!-- PANEL DERECHO: PROPIEDADES -->
        <div class="properties-panel">
            <div class="panel-header">
                <i class="fas fa-sliders-h"></i> Propiedades
            </div>
            <div class="properties-content" id="propertiesContent">
                <div class="property-placeholder">
                    <i class="fas fa-mouse-pointer"></i>
                    <p>Selecciona un campo</p>
                </div>
                <div class="property-form" id="propertyForm" style="display:none;">
                    <div class="property-row">
                        <label>Etiqueta</label>
                        <input type="text" id="propEtiqueta" class="prop-input" onchange="actualizarPropiedad('etiqueta', this.value)" />
                    </div>
                    <div class="property-row">
                        <label>Nombre Interno</label>
                        <input type="text" id="propNombre" class="prop-input" onchange="actualizarPropiedad('nombre', this.value)" />
                    </div>
                    <div class="property-row">
                        <label>Tipo</label>
                        <select id="propTipo" class="prop-input" onchange="actualizarPropiedad('tipo', this.value)">
                            <optgroup label="Campos de entrada">
                                <option value="texto">Texto</option>
                                <option value="numero">Número</option>
                                <option value="decimal">Decimal</option>
                                <option value="fecha">Fecha</option>
                                <option value="fecha_hora">Fecha/Hora</option>
                                <option value="hora">Hora</option>
                                <option value="dropdown">Lista</option>
                                <option value="radio">Opciones</option>
                                <option value="checkbox">Casilla</option>
                                <option value="textarea">Área Texto</option>
                                <option value="foto">Foto</option>
                                <option value="archivo">Archivo</option>
                                <option value="firma">Firma</option>
                            </optgroup>
                            <optgroup label="Botones de acción">
                                <option value="boton_guardar">Botón Guardar</option>
                                <option value="boton_cancelar">Botón Cancelar</option>
                            </optgroup>
                        </select>
                    </div>
                    <div class="property-row">
                        <label>Sección</label>
                        <input type="text" id="propSeccion" class="prop-input" value="General" onchange="actualizarPropiedad('seccion', this.value)" />
                    </div>
                    <div class="property-row">
                        <label>Ancho (columnas)</label>
                        <div class="ancho-slider-container">
                            <input type="range" id="propAncho" class="ancho-slider" min="1" max="12" value="12" 
                                oninput="document.getElementById('propAnchoValue').textContent = this.value + '/12';"
                                onchange="actualizarPropiedad('ancho', parseInt(this.value))" />
                            <span id="propAnchoValue" class="ancho-value">12/12</span>
                        </div>
                    </div>
                    <div class="property-row">
                        <label>Placeholder</label>
                        <input type="text" id="propPlaceholder" class="prop-input" onchange="actualizarPropiedad('placeholder', this.value)" />
                    </div>
                    <div class="property-row" id="propAlturaRow" style="display:none;">
                        <label>Altura (px)</label>
                        <div class="ancho-slider-container">
                            <input type="range" id="propAltura" class="ancho-slider" min="80" max="400" step="20" value="150" 
                                oninput="document.getElementById('propAlturaValue').textContent = this.value + 'px';"
                                onchange="actualizarPropiedad('altura', parseInt(this.value))" />
                            <span id="propAlturaValue" class="ancho-value">150px</span>
                        </div>
                    </div>
                    <div class="property-row checkbox-row">
                        <label>
                            <input type="checkbox" id="propRequerido" onchange="actualizarPropiedad('requerido', this.checked)" />
                            Requerido
                        </label>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script src="/Scripts/app/Catalogos/formulario-disenador.js" type="text/javascript"></script>
</asp:Content>