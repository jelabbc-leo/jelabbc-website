<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/Jela.Master" CodeBehind="CapturaDocumentos.aspx.vb" Inherits="JelaWeb.CapturaDocumentos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/CSS/capturadocumentos.css") %>" rel="stylesheet" type="text/css" />
    <script src="<%= ResolveUrl("~/Scripts/app/Operacion/capturadocumentos.js") %>"></script>

    <!-- Panel superior de filtros -->
    <dx:ASPxPanel ID="panelFiltros" runat="server" Width="100%">
        <PanelCollection>
            <dx:PanelContent>
                <dx:ASPxFormLayout ID="formFiltros" runat="server" ColumnCount="4" Width="100%" AlignItemCaptionsInAllGroups="True" BackColor="#E4EFFA" Theme="Office2010Blue">
                    <Items>

                        <dx:LayoutItem Caption="Tipo de documento">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxGridLookup ID="glTipoDocumento" runat="server" Width="100%" KeyFieldName="Id" ValueField="Id" TextFormatString="{0}-{2}" ClientInstanceName="glTipoDocumento">

                                        <GridViewProperties>
                                            <Settings ShowFilterRow="True" />
                                            <SettingsBehavior AllowFocusedRow="True" />
                                            <SettingsPager Mode="ShowAllRecords" />

                                            <SettingsPopup>
                                                <FilterControl AutoUpdatePosition="False"></FilterControl>
                                            </SettingsPopup>
                                        </GridViewProperties>

                                        <CaptionStyle Font-Size="12pt">
                                        </CaptionStyle>
                                    </dx:ASPxGridLookup>

                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            <CaptionSettings VerticalAlign="Middle" />
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Desde">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dtDesde" runat="server" Width="100%" NullText="Desde" ClientInstanceName="dtDesde">
                                        <CaptionStyle Font-Size="10pt">
                                        </CaptionStyle>
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            <CaptionSettings VerticalAlign="Middle" />
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Hasta">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dtHasta" runat="server" Width="100%" NullText="Hasta" ClientInstanceName="dtHasta">
                                        <CaptionStyle Font-Size="10pt">
                                        </CaptionStyle>
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            <CaptionSettings VerticalAlign="Middle" />
                        </dx:LayoutItem>

                        <dx:LayoutItem ShowCaption="False">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxButton ID="btnFiltrar" runat="server" Text="Solicitar Documentos" AutoPostBack="False" CommandName="btnFiltrar">
                                        <ClientSideEvents Click="function(s,e){ FiltrarDocumentos(); }" />
                                    </dx:ASPxButton>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                            <CaptionSettings VerticalAlign="Middle" />
                        </dx:LayoutItem>

                    </Items>
                </dx:ASPxFormLayout>
            </dx:PanelContent>
        </PanelCollection>
    </dx:ASPxPanel>

      <!-- Grid principal ASPxGridView -->
    <dx:ASPxGridView ID="gridDocumentos" runat="server" ClientInstanceName="gridDocumentos" KeyFieldName="Id" Width="100%" Theme="Office2010Blue">
        
        <ClientSideEvents ToolbarItemClick="onToolbarDocumentosClick" />
        <Settings ShowHeaderFilterButton="True" />

        <SettingsPopup>
            <FilterControl AutoUpdatePosition="False"></FilterControl>
        </SettingsPopup>
        <SettingsSearchPanel Visible="True" />

        <SettingsExport FileName="Documentos">
        </SettingsExport>

        <Toolbars>
            <dx:GridViewToolbar>
                <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                <Items>
                    <dx:GridViewToolbarItem Name="btnNuevoDoc" Text="Nuevo Documento" BeginGroup="True" GroupName="grp1">
                        <Image IconID="actions_add_16x16"></Image>
                    </dx:GridViewToolbarItem>

                    <dx:GridViewToolbarItem Name="btnEditarDoc" Text="Editar Documento">
                        <Image IconID="actions_edit_16x16"></Image>
                    </dx:GridViewToolbarItem>

                    <dx:GridViewToolbarItem Name="btnCancelarDoc" Text="Cancelar Documento">
                        <Image IconID="actions_trash_16x16"></Image>
                    </dx:GridViewToolbarItem>

                    <dx:GridViewToolbarItem BeginGroup="True" GroupName="grp2" Name="btnExportar" Text="Exportar">
                        <Image IconID="actions_converttorange_16x16">
                        </Image>
                    </dx:GridViewToolbarItem>

                    <dx:GridViewToolbarItem BeginGroup="True" GroupName="grp3" Name="btnLiberarDoc" Text="Liberar Tareas">
                        <Image IconID="communication_phone_16x16">
                        </Image>
                    </dx:GridViewToolbarItem>

                    <dx:GridViewToolbarItem BeginGroup="True" GroupName="grp4" Name="btnActualizar" Text="Actualizar">
                        <Image IconID="actions_refresh_16x16">
                        </Image>
                    </dx:GridViewToolbarItem>

                </Items>
            </dx:GridViewToolbar>
        </Toolbars>
    </dx:ASPxGridView>
    <dx:ASPxGridViewExporter ID="gridExporter" runat="server" GridViewID="gridDocumentos"></dx:ASPxGridViewExporter>

    <!-- Popup para nuevo documento -->
    <dx:ASPxPopupControl ID="popupDocumento" runat="server" ClientInstanceName="popupDocumento"
        HeaderText="Nuevo Documento" Width="900px" Height="500px"
        PopupHorizontalAlign="WindowCenter" PopupVerticalAlign="WindowCenter"
        Modal="True" CloseAction="CloseButton" CloseOnEscape="True">
        <ClientSideEvents Shown="function(s, e) { 
            // Asegurar que los grids estén limpios al mostrar el popup
            try {
                var coloniasCount = getGridRowCount ? getGridRowCount(gridColonias) : 0;
                if (coloniasCount === 0) {
                    gridColonias.PerformCallback('LIMPIAR|');
                }
                var conceptosCount = getGridRowCount ? getGridRowCount(gridConceptos) : 0;
                if (conceptosCount === 0) {
                    gridConceptos.PerformCallback('LIMPIAR|');
                }
            } catch (err) {
                console.error('Error en Shown del popup:', err);
            }
        }" />
        
        <ContentStyle BackColor="#E0EDFA">
        </ContentStyle>

        <ContentCollection>
            <dx:PopupControlContentControl runat="server">
                <dx:ASPxHiddenField ID="hfIdDocumento" runat="server" ClientInstanceName="hfIdDocumento" />
                
                <div id="loadingOverlay">
                    <div>
                        <div class="spinner-loader"></div>
                        <p>Guardando documento, por favor espere...</p>
                    </div>
                </div>
                <dx:ASPxFormLayout ID="formDocumento" runat="server" Width="100%" ColumnCount="2" AlignItemCaptionsInAllGroups="True">
                    <Items>

                        <dx:LayoutItem Caption="No. Documento">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtNoDocumento" runat="server" ClientInstanceName="txtNoDocumento" Width="100%" Theme="Office2010Blue" >
                                        <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip" ErrorText="No debe estar vacio">
                                            <RequiredField IsRequired="True" />
                                        </ValidationSettings>
                                    </dx:ASPxTextBox>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Asignar a">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxGridLookup ID="glSubEntidad" runat="server" ClientInstanceName="glSubEntidad" Width="100%" KeyFieldName="Id" TextFormatString="{0}" Theme="Office2010Blue">
                                        <GridViewProperties>
                                            <Settings ShowFilterRow="True" />
                                            <SettingsBehavior AllowFocusedRow="True" />
                                            <SettingsPager Mode="ShowAllRecords" />

                                            <SettingsPopup>
                                                <FilterControl AutoUpdatePosition="False"></FilterControl>
                                            </SettingsPopup>
                                        </GridViewProperties>
                                        <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip" ErrorText="No debe estar vacio">
                                            <RequiredField IsRequired="True" />
                                        </ValidationSettings>
                                    </dx:ASPxGridLookup>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Fecha Asignación">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxDateEdit ID="dtFechaAsignacion" runat="server" ClientInstanceName="dtFechaAsignacion" Width="100%" Theme="Office2010Blue" >
                                        <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip" ErrorText="No debe estar vacio">
                                            <RequiredField IsRequired="True" />
                                        </ValidationSettings>
                                    </dx:ASPxDateEdit>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Referencia">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtReferencia" runat="server" ClientInstanceName="txtReferencia" Width="100%" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Documento Relacionado">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <dx:ASPxTextBox ID="txtDocumentoRelacionado" runat="server" ClientInstanceName="txtDocumentoRelacionado" Width="100%" Theme="Office2010Blue" />
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                    <div></div>
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                        <dx:LayoutItem Caption="Comentarios">
                            <LayoutItemNestedControlCollection>
                                <dx:LayoutItemNestedControlContainer runat="server">
                                </dx:LayoutItemNestedControlContainer>
                            </LayoutItemNestedControlCollection>
                        </dx:LayoutItem>

                    </Items>

                </dx:ASPxFormLayout>
                <div class="form-comentarios">
                    <dx:ASPxMemo ID="txtComentarios" runat="server" Height="71px" Width="100%" Theme="Office2010Blue"></dx:ASPxMemo>
                </div>

                <dx:ASPxPageControl ID="pcDocumentos" runat="server" Width="100%" Height="300px" ActiveTabIndex="0" Theme="Office2010Blue">
                    <TabPages>
                        <dx:TabPage Text="Colonias" Name="tabColonias">
                            <ContentCollection>
                                <dx:ContentControl runat="server">

                                    <dx:ASPxGridView ID="gridColonias" runat="server" ClientInstanceName="gridColonias"
                                        Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" OnCustomCallback="gridColonias_CustomCallback" 
                                        OnCellEditorInitialize="gridColonias_CellEditorInitialize" Theme="Office2010Blue">
                                        <Settings ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                                        <SettingsDataSecurity AllowEdit="True" PreventLoadClientValuesForReadOnlyColumns="False" />
                                        <SettingsEditing Mode="Batch" NewItemRowPosition="Bottom" UseFormLayout="False">
                                            <BatchEditSettings StartEditAction="Click" />
                                        </SettingsEditing>

                                        <ClientSideEvents Init="initGridColonias" 
                                            BatchEditEndEditing="onEndEditingColonias" 
                                            BatchEditStartEditing="onStartEditingColonias"
                                            EndCallback="gridColonias_EndCallback" 
                                            ToolbarItemClick="onToolbarColoniasClick" />

                                        <SettingsBehavior AllowFocusedRow="True" />
                                        <SettingsPager Mode="ShowAllRecords" />

                                        <SettingsPopup>
                                            <FilterControl AutoUpdatePosition="False"></FilterControl>
                                        </SettingsPopup>

                                        <Columns>

                                            <dx:GridViewDataTextColumn FieldName="Id" Caption="No" UnboundType="Integer" Width="50px" ReadOnly="True">
                                                <PropertiesTextEdit DisplayFormatString="N0" />
                                            </dx:GridViewDataTextColumn>

                                            <dx:GridViewDataTextColumn Caption="Clave" FieldName="Clave" ShowInCustomizationForm="True" UnboundType="String" VisibleIndex="1">
                                            </dx:GridViewDataTextColumn>

                                            <dx:GridViewDataComboBoxColumn FieldName="Colonia" Caption="Colonia" UnboundType="String" Width="500px">
                                                <PropertiesComboBox ValueType="System.String" EnableCallbackMode="True" IncrementalFilteringMode="Contains" />
                                            </dx:GridViewDataComboBoxColumn>

                                            <dx:GridViewDataTextColumn FieldName="MontoMin" Caption="Monto Mínimo" UnboundType="Decimal">
                                                <PropertiesTextEdit DisplayFormatString="C2" DisplayFormatInEditMode="True" />
                                            </dx:GridViewDataTextColumn>

                                            <dx:GridViewDataTextColumn FieldName="MontoMax" Caption="Monto Máximo" UnboundType="Decimal">
                                                <PropertiesTextEdit DisplayFormatString="C2" />
                                            </dx:GridViewDataTextColumn>

                                        </Columns>

                                        <Toolbars>
                                            <dx:GridViewToolbar>
                                                <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                                <Items>
                                                    <dx:GridViewToolbarItem Name="btnColAdd" Text="Asignar Sección" Image-IconID="export_exporttopdf_16x16office2013">
                                                        <Image IconID="actions_add_16x16"></Image>
                                                    </dx:GridViewToolbarItem>

                                                    <dx:GridViewToolbarItem Name="btnColDelete" Text="Eliminar" Image-IconID="export_exporttoxls_16x16office2013">
                                                        <Image IconID="actions_trash_16x16"></Image>
                                                    </dx:GridViewToolbarItem>

                                                </Items>
                                            </dx:GridViewToolbar>
                                        </Toolbars>

                                    </dx:ASPxGridView>
                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>

                        <dx:TabPage Text="Conceptos" Name="tabConceptos">
                            <ContentCollection>
                                <dx:ContentControl runat="server">

                                    <dx:ASPxGridView ID="gridConceptos" runat="server" ClientInstanceName="gridConceptos"
                                        Width="100%" KeyFieldName="Id" AutoGenerateColumns="False" OnCustomCallback="gridConceptos_CustomCallback"
                                        OnCellEditorInitialize="gridConceptos_CellEditorInitialize" Theme="Office2010Blue">
                                        <Settings ShowStatusBar="Hidden" VerticalScrollBarMode="Visible" />
                                        <SettingsDataSecurity AllowEdit="True" PreventLoadClientValuesForReadOnlyColumns="False" />
                                        <SettingsEditing Mode="Batch" NewItemRowPosition="Bottom" UseFormLayout="False" />

                                        <ClientSideEvents Init="initGridConceptos" 
                                            BatchEditEndEditing="onEndEditingConceptos" 
                                            BatchEditStartEditing="onStartEditingConceptos"
                                            EndCallback="gridConceptos_EndCallback" 
                                            ToolbarItemClick="onToolbarConceptosClick" />

                                        <SettingsBehavior AllowFocusedRow="True" />
                                        <SettingsPager Mode="ShowAllRecords" />

                                        <SettingsPopup>
                                            <FilterControl AutoUpdatePosition="False"></FilterControl>
                                        </SettingsPopup>

                                        <Columns>

                                            <dx:GridViewDataTextColumn FieldName="Id" Caption="No" UnboundType="Integer" Width="50px" ReadOnly="True" Name="colIdC">
                                                <PropertiesTextEdit DisplayFormatString="N0" />
                                            </dx:GridViewDataTextColumn>

                                            <dx:GridViewDataTextColumn Caption="Clave" FieldName="Clave" ShowInCustomizationForm="True" UnboundType="String" VisibleIndex="1" Name="colClaveC">
                                            </dx:GridViewDataTextColumn>

                                            <dx:GridViewDataComboBoxColumn FieldName="Descripcion" Caption="Concepto"
                                                UnboundType="String" Width="500px" Name="colConceptoC">
                                                <PropertiesComboBox ValueType="System.String" EnableCallbackMode="True" IncrementalFilteringMode="Contains" />
                                            </dx:GridViewDataComboBoxColumn>

                                            <dx:GridViewDataTextColumn FieldName="CostoUnitario" Caption="Costo Unitario" UnboundType="Decimal" Name="colCostoC">
                                                <PropertiesTextEdit DisplayFormatString="C2" DisplayFormatInEditMode="True" />
                                            </dx:GridViewDataTextColumn>

                                        </Columns>

                                        <Toolbars>
                                            <dx:GridViewToolbar>
                                                <SettingsAdaptivity Enabled="true" EnableCollapseRootItemsToIcons="true" />
                                                <Items>
                                                    <dx:GridViewToolbarItem Name="btnConceptoAdd" Text="Asignar Concepto" Image-IconID="export_exporttopdf_16x16office2013">
                                                        <Image IconID="actions_add_16x16"></Image>
                                                    </dx:GridViewToolbarItem>

                                                    <dx:GridViewToolbarItem Name="btnConceptoDel" Text="Eliminar" Image-IconID="export_exporttoxls_16x16office2013">
                                                        <Image IconID="actions_trash_16x16"></Image>
                                                    </dx:GridViewToolbarItem>

                                                </Items>
                                            </dx:GridViewToolbar>
                                        </Toolbars>

                                    </dx:ASPxGridView>


                                </dx:ContentControl>
                            </ContentCollection>
                        </dx:TabPage>
                    </TabPages>
                </dx:ASPxPageControl>

                <div class="form-actions mt-3">
                    <dx:ASPxButton ID="btnGuardarCambios" runat="server" Text="Guardar cambios" Width="150px" CausesValidation="True" 
                        UseSubmitBehavior="True">
                        <ClientSideEvents Click="function(s, e) { 
                            if (ASPxClientEdit.ValidateGroup('formDocumento')) {
                                // Mostrar loading overlay moderno
                                mostrarLoading();
                                e.processOnServer = true;
                            } else {
                                e.processOnServer = false;
                            }
                        }" />
                    </dx:ASPxButton>
                    <%--<asp:Button ID="btnGuardarCambios" runat="server" Text="Guardar cambios"
                        CssClass="metro-button" CausesValidation="True" OnClick="btnGuardarCambios_Click" />--%>
                </div>

            </dx:PopupControlContentControl>
        </ContentCollection>

    </dx:ASPxPopupControl>
    
</asp:Content>
