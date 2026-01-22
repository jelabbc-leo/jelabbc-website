// ========================================================================
// MÓDULO: Pagos
// ========================================================================
// Gestiona el registro y aplicación de pagos a cuotas
// ========================================================================

var currentPagoId = 0;
var montoPagoActual = 0;
var aplicacionesCuotas = [];

// Nota: showToast está definida globalmente en el Master Page
if (typeof showToast === 'undefined') {
    window.showToast = function(type, message) {
        if (typeof toastr !== 'undefined' && toastr && typeof toastr[type] === 'function') {
            toastr[type](message);
        } else {
            console.log('[' + type.toUpperCase() + '] ' + message);
            if (type === 'error') {
                alert('Error: ' + message);
            }
        }
    };
}

// ========================================================================
// INICIALIZACIÓN
// ========================================================================

$(document).ready(function() {
    if (typeof gridPagos !== 'undefined') {
        console.log('Pagos: Grid inicializado');
    }
    
    // Eventos de tabs
    if (typeof tabsPago !== 'undefined') {
        tabsPago.AddActiveTabChanged(function(s, e) {
            if (e.tab.name === 'tabAplicarCuotas') {
                cargarCuotasPendientes();
            }
        });
    }
});

// ========================================================================
// TOOLBAR DEL GRID
// ========================================================================

function onToolbarPagosClick(s, e) {
    console.log('onToolbarPagosClick: Item clickeado:', e.item.name);
    switch (e.item.name) {
        case 'btnNuevoPago':
            mostrarNuevoPago();
            break;
        case 'btnEditarPago':
            editarPagoSeleccionado();
            break;
        case 'btnAplicarPago':
            aplicarPagoSeleccionado();
            break;
        case 'btnEliminarPago':
            eliminarPagoSeleccionado();
            break;
        default:
            console.log('onToolbarPagosClick: Item no reconocido:', e.item.name);
    }
}

function onRowDblClick(s, e) {
    editarPagoSeleccionado();
}

// ========================================================================
// FUNCIONES DE PAGOS
// ========================================================================

function mostrarNuevoPago() {
    limpiarFormularioPago();
    aplicacionesCuotas = [];
    document.getElementById('hfPagoId').value = '0';
    currentPagoId = 0;
    montoPagoActual = 0;
    popupPago.SetHeaderText('Nuevo Pago');
    tabsPago.SetActiveTabIndex(0);
    popupPago.Show();
}

function editarPagoSeleccionado() {
    var idx = gridPagos.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione un pago para editar');
        return;
    }
    var id = gridPagos.GetRowKey(idx);
    cargarPagoParaEditar(id);
}

function aplicarPagoSeleccionado() {
    var idx = gridPagos.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione un pago para aplicar');
        return;
    }
    var id = gridPagos.GetRowKey(idx);
    cargarPagoParaAplicar(id);
}

function eliminarPagoSeleccionado() {
    var idx = gridPagos.GetFocusedRowIndex();
    if (idx < 0) {
        showToast('warning', 'Seleccione un pago para eliminar');
        return;
    }
    var id = gridPagos.GetRowKey(idx);
    
    if (confirm('¿Está seguro de eliminar este pago? Se eliminarán todas las aplicaciones a cuotas.')) {
        ajaxCall('EliminarPago', { id: id }, function(r) {
            if (r.success) {
                showToast('success', r.message);
                gridPagos.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function cargarPagoParaEditar(id) {
    ajaxCall('ObtenerPago', { id: id }, function(r) {
        if (r.success && r.data) {
            currentPagoId = id;
            document.getElementById('hfPagoId').value = id;
            montoPagoActual = parseFloat(r.data.Monto || 0);
            cargarDatosPago(r.data);
            popupPago.SetHeaderText('Editar Pago');
            tabsPago.SetActiveTabIndex(0);
            popupPago.Show();
        } else {
            showToast('error', r.message || 'Error al cargar el pago');
        }
    });
}

function cargarPagoParaAplicar(id) {
    ajaxCall('ObtenerPago', { id: id }, function(r) {
        if (r.success && r.data) {
            currentPagoId = id;
            montoPagoActual = parseFloat(r.data.Monto || 0);
            cargarDatosPago(r.data);
            popupPago.SetHeaderText('Aplicar Pago a Cuotas');
            tabsPago.SetActiveTabIndex(1); // Ir al tab de aplicación
            popupPago.Show();
        } else {
            showToast('error', r.message || 'Error al cargar el pago');
        }
    });
}

function cargarDatosPago(d) {
    if (cboPagoEntidad) {
        cboPagoEntidad.SetValue(d.EntidadId);
        onPagoEntidadChanged(null, null);
    }
    if (cboPagoUnidad) {
        setTimeout(function() {
            cboPagoUnidad.SetValue(d.UnidadId);
            onPagoUnidadChanged(null, null);
        }, 500);
    }
    if (cboPagoResidente && d.ResidenteId) {
        setTimeout(function() {
            cboPagoResidente.SetValue(d.ResidenteId);
        }, 1000);
    }
    if (dtFechaPago && d.FechaPago) {
        var fecha = parseDate(d.FechaPago);
        if (fecha) dtFechaPago.SetDate(fecha);
    }
    if (txtPagoMonto) txtPagoMonto.SetNumber(d.Monto || 0);
    if (cboFormaPago) cboFormaPago.SetValue(d.FormaPago || 'Efectivo');
    if (txtReferencia) txtReferencia.SetValue(d.Referencia || '');
    if (txtBanco) txtBanco.SetValue(d.Banco || '');
    if (cboPagoEstado) cboPagoEstado.SetValue(d.Estado || 'Aplicado');
    if (txtPagoObservaciones) txtPagoObservaciones.SetValue(d.Observaciones || '');
}

function limpiarFormularioPago() {
    if (cboPagoEntidad) cboPagoEntidad.SetValue(null);
    if (cboPagoUnidad) cboPagoUnidad.SetValue(null);
    if (cboPagoResidente) cboPagoResidente.SetValue(null);
    if (dtFechaPago) dtFechaPago.SetDate(new Date());
    if (txtPagoMonto) txtPagoMonto.SetNumber(0);
    if (cboFormaPago) cboFormaPago.SetValue('Efectivo');
    if (txtReferencia) txtReferencia.SetValue('');
    if (txtBanco) txtBanco.SetValue('');
    if (cboPagoEstado) cboPagoEstado.SetValue('Aplicado');
    if (txtPagoObservaciones) txtPagoObservaciones.SetValue('');
    actualizarResumenMonto();
}

function guardarPago() {
    var id = parseInt(document.getElementById('hfPagoId').value) || 0;
    var datos = {
        id: id,
        entidadId: cboPagoEntidad.GetValue(),
        unidadId: cboPagoUnidad.GetValue(),
        residenteId: cboPagoResidente.GetValue(),
        monto: txtPagoMonto.GetNumber(),
        formaPago: cboFormaPago.GetValue(),
        referencia: txtReferencia.GetValue(),
        banco: txtBanco.GetValue(),
        fechaPago: dtFechaPago.GetDate(),
        estado: cboPagoEstado.GetValue(),
        observaciones: txtPagoObservaciones.GetValue(),
        aplicaciones: aplicacionesCuotas
    };
    
    ajaxCall('GuardarPago', { datos: datos }, function(r) {
        if (r.success) {
            showToast('success', r.message);
            if (r.id) {
                currentPagoId = r.id;
                document.getElementById('hfPagoId').value = r.id;
            }
            // Si hay aplicaciones, ya se guardaron, solo recargar grid
            gridPagos.PerformCallback('cargar');
            if (aplicacionesCuotas.length === 0) {
                popupPago.Hide();
            } else {
                // Ir al tab de aplicación si hay aplicaciones
                tabsPago.SetActiveTabIndex(1);
                cargarCuotasPendientes();
            }
        } else {
            showToast('error', r.message);
        }
    });
}

// ========================================================================
// EVENTOS DE COMBOBOX
// ========================================================================

function onPagoEntidadChanged(s, e) {
    var entidadId = cboPagoEntidad.GetValue();
    if (!entidadId || entidadId === 0) {
        cboPagoUnidad.SetValue(null);
        cboPagoUnidad.SetItems([]);
        return;
    }
    
    ajaxCall('ListarUnidades', { entidadId: entidadId }, function(r) {
        if (r.success && r.data) {
            cboPagoUnidad.ClearItems();
            for (var i = 0; i < r.data.length; i++) {
                cboPagoUnidad.AddItem(r.data[i].NombreCompleto, r.data[i].Id);
            }
        }
    });
}

function onPagoUnidadChanged(s, e) {
    var unidadId = cboPagoUnidad.GetValue();
    if (!unidadId || unidadId === 0) {
        cboPagoResidente.SetValue(null);
        cboPagoResidente.SetItems([]);
        return;
    }
    
    ajaxCall('ListarResidentes', { unidadId: unidadId }, function(r) {
        if (r.success && r.data) {
            cboPagoResidente.ClearItems();
            cboPagoResidente.AddItem('-- Ninguno --', null);
            for (var i = 0; i < r.data.length; i++) {
                cboPagoResidente.AddItem(r.data[i].NombreCompleto, r.data[i].Id);
            }
        }
    });
    
    // Cargar cuotas pendientes si estamos en el tab de aplicación
    if (tabsPago.GetActiveTabIndex() === 1) {
        cargarCuotasPendientes();
    }
}

function onPagoMontoChanged(s, e) {
    montoPagoActual = txtPagoMonto.GetNumber() || 0;
    actualizarResumenMonto();
}

// ========================================================================
// APLICACIÓN A CUOTAS
// ========================================================================

function cargarCuotasPendientes() {
    var unidadId = cboPagoUnidad.GetValue();
    if (!unidadId || unidadId === 0) {
        showToast('warning', 'Primero seleccione una unidad');
        return;
    }
    
    ajaxCall('ObtenerCuotasPendientes', { unidadId: unidadId }, function(r) {
        if (r.success && r.data) {
            // Limpiar selecciones
            gridCuotasPendientes.UnselectAllRowsOnPage();
            aplicacionesCuotas = [];
            actualizarResumenMonto();
            
            // Recargar grid (el grid se actualizará con los datos)
            gridCuotasPendientes.PerformCallback('cargar|' + unidadId);
        } else {
            showToast('error', r.message || 'Error al cargar cuotas pendientes');
        }
    });
}

function onCuotasPendientesSelectionChanged(s, e) {
    calcularAplicaciones();
    actualizarResumenMonto();
}

function calcularAplicaciones() {
    aplicacionesCuotas = [];
    var selectedKeys = gridCuotasPendientes.GetSelectedKeysOnPage();
    
    for (var i = 0; i < selectedKeys.length; i++) {
        var cuotaId = selectedKeys[i];
        var rowIndex = gridCuotasPendientes.GetRowIndexByKey(cuotaId);
        
        if (rowIndex >= 0) {
            var saldo = parseFloat(gridCuotasPendientes.batchEditApi.GetCellValue(rowIndex, 'Saldo')) || 0;
            aplicacionesCuotas.push({
                cuotaId: cuotaId,
                montoAplicado: saldo
            });
        }
    }
}

function aplicarPagoACuotas() {
    if (!currentPagoId || currentPagoId === 0) {
        showToast('warning', 'Primero guarde el pago antes de aplicarlo a cuotas');
        return;
    }
    
    if (aplicacionesCuotas.length === 0) {
        showToast('warning', 'Seleccione al menos una cuota para aplicar el pago');
        return;
    }
    
    var montoTotalAplicar = 0;
    for (var i = 0; i < aplicacionesCuotas.length; i++) {
        montoTotalAplicar += aplicacionesCuotas[i].montoAplicado;
    }
    
    if (montoTotalAplicar > montoPagoActual) {
        showToast('warning', 'El monto total a aplicar (' + formatCurrency(montoTotalAplicar) + ') excede el monto del pago (' + formatCurrency(montoPagoActual) + ')');
        return;
    }
    
    if (confirm('¿Aplicar el pago a las ' + aplicacionesCuotas.length + ' cuotas seleccionadas?')) {
        ASPxClientUtils.ShowLoadingPanel();
        ajaxCall('AplicarPagoACuotas', { 
            pagoId: currentPagoId, 
            aplicaciones: aplicacionesCuotas 
        }, function(r) {
            ASPxClientUtils.HideLoadingPanel();
            if (r.success) {
                showToast('success', r.message);
                popupPago.Hide();
                gridPagos.PerformCallback('cargar');
            } else {
                showToast('error', r.message);
            }
        });
    }
}

function actualizarResumenMonto() {
    var montoAplicado = 0;
    for (var i = 0; i < aplicacionesCuotas.length; i++) {
        montoAplicado += aplicacionesCuotas[i].montoAplicado;
    }
    var saldoPendiente = montoPagoActual - montoAplicado;
    
    if (document.getElementById('lblMontoPago')) {
        document.getElementById('lblMontoPago').textContent = formatCurrency(montoPagoActual);
    }
    if (document.getElementById('lblMontoAplicado')) {
        document.getElementById('lblMontoAplicado').textContent = formatCurrency(montoAplicado);
    }
    if (document.getElementById('lblSaldoPendiente')) {
        var saldoElement = document.getElementById('lblSaldoPendiente');
        saldoElement.textContent = formatCurrency(saldoPendiente);
        saldoElement.style.color = saldoPendiente > 0 ? '#dc3545' : '#28a745';
    }
}

function formatCurrency(value) {
    return '$' + parseFloat(value || 0).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');
}

// ========================================================================
// UTILIDADES
// ========================================================================

function parseDate(dateString) {
    if (!dateString) return null;
    try {
        var date = new Date(dateString);
        if (isNaN(date.getTime())) return null;
        return date;
    } catch (e) {
        return null;
    }
}
